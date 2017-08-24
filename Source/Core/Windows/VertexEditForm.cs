
#region ================== Copyright (c) 2007 Pascal vd Heiden

/*
 * Copyright (c) 2007 Pascal vd Heiden, www.codeimp.com
 * This program is released under GNU General Public License
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 */

#endregion

#region ================== Namespaces

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using CodeImp.DoomBuilder.Geometry;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Data;
using CodeImp.DoomBuilder.IO;
using System.IO;
using CodeImp.DoomBuilder.Config;
using CodeImp.DoomBuilder.Editing;
using CodeImp.DoomBuilder.Controls;

#endregion

namespace CodeImp.DoomBuilder.Windows
{
	public partial class VertexEditForm : DelayedForm
	{
		#region ================== Constants

		#endregion

		#region ================== Variables

		private ICollection<Vertex> vertices;

		#endregion

		#region ================== Properties

		#endregion
		
		#region ================== Constructor

		// Constructor
		public VertexEditForm()
		{
			InitializeComponent();

			// Fill universal fields list
			fieldslist.ListFixedFields(General.Map.Config.VertexFields);

			// Custom fields?
			if(!General.Map.FormatInterface.HasCustomFields)
				tabs.TabPages.Remove(tabcustom);
			
			// Decimals allowed?
			if(General.Map.FormatInterface.VertexDecimals > 0)
			{
				positionx.AllowDecimal = true;
				positiony.AllowDecimal = true;
			}

			// Initialize custom fields editor
			fieldslist.Setup("vertex");
		}

		#endregion
		
		#region ================== Methods

		// This sets up the form to edit the given vertices
		public void Setup(ICollection<Vertex> vertices)
		{
			// Keep this list
			this.vertices = vertices;
			if(vertices.Count > 1) this.Text = "Edit Vertices (" + vertices.Count + ")";

			////////////////////////////////////////////////////////////////////////
			// Set all options to the first vertex properties
			////////////////////////////////////////////////////////////////////////

			// Get first vertex
			Vertex vc = General.GetByIndex(vertices, 0);

			// Position
			positionx.Text = vc.Position.x.ToString();
			positiony.Text = vc.Position.y.ToString();
			
			// Custom fields
			fieldslist.SetValues(vc.Fields, true);
			
			////////////////////////////////////////////////////////////////////////
			// Now go for all sectors and change the options when a setting is different
			////////////////////////////////////////////////////////////////////////

			// Go for all vertices
			foreach(Vertex v in vertices)
			{
				// Position
				if(positionx.Text != v.Position.x.ToString()) positionx.Text = "";
				if(positiony.Text != v.Position.y.ToString()) positiony.Text = "";

				// Custom fields
				fieldslist.SetValues(v.Fields, false);
			}
		}
		
		#endregion
		
		#region ================== Events

		// OK clicked
		private void apply_Click(object sender, EventArgs e)
		{
			string undodesc = "vertex";

			// Verify the coordinates
			if((positionx.GetResultFloat(0.0f) < General.Map.FormatInterface.MinCoordinate) || (positionx.GetResultFloat(0.0f) > General.Map.FormatInterface.MaxCoordinate) ||
			   (positiony.GetResultFloat(0.0f) < General.Map.FormatInterface.MinCoordinate) || (positiony.GetResultFloat(0.0f) > General.Map.FormatInterface.MaxCoordinate))
			{
				General.ShowWarningMessage("Vertex coordinates must be between " + General.Map.FormatInterface.MinCoordinate + " and " + General.Map.FormatInterface.MaxCoordinate + ".", MessageBoxButtons.OK);
				return;
			}
			
			// Make undo
			if(vertices.Count > 1) undodesc = vertices.Count + " vertices";
			General.Map.UndoRedo.CreateUndo("Edit " + undodesc);

			// Go for all vertices
			foreach(Vertex v in vertices)
			{
				// Apply position
				Vector2D p = new Vector2D();
				p.x = General.Clamp(positionx.GetResultFloat(v.Position.x), (float)General.Map.FormatInterface.MinCoordinate, (float)General.Map.FormatInterface.MaxCoordinate);
				p.y = General.Clamp(positiony.GetResultFloat(v.Position.y), (float)General.Map.FormatInterface.MinCoordinate, (float)General.Map.FormatInterface.MaxCoordinate);
				v.Move(p);
				
				// Custom fields
				fieldslist.Apply(v.Fields);
			}
			
			// Done
			General.Map.IsChanged = true;
			this.DialogResult = DialogResult.OK;
			this.Close();
		}

		// Cancel clicked
		private void cancel_Click(object sender, EventArgs e)
		{
			// Just close
			this.DialogResult = DialogResult.Cancel;
			this.Close();
		}

		// Help requested
		private void VertexEditForm_HelpRequested(object sender, HelpEventArgs hlpevent)
		{
			General.ShowHelp("w_vertexeditor.html");
			hlpevent.Handled = true;
		}

		#endregion
	}
}