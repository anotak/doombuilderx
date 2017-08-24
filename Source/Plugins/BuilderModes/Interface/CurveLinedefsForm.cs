
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
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Reflection;
using CodeImp.DoomBuilder.Windows;
using CodeImp.DoomBuilder.IO;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Rendering;
using CodeImp.DoomBuilder.Geometry;
using System.Drawing;
using CodeImp.DoomBuilder.Editing;

#endregion

namespace CodeImp.DoomBuilder.BuilderModes
{
	public partial class CurveLinedefsForm : DelayedForm
	{
		#region ================== Constants

		private int MIN_VERTICES = 1;
		private int MAX_VERTICES = 200;
		private int MIN_DISTANCE = 0;
		private int MAX_DISTANCE = 10000;
		private int MIN_ANGLE = 1;
		private int MAX_ANGLE = 350;

		#endregion

		#region ================== Properties

		public int Vertices { get { return -verticesbar.Value; } }
		public float Distance { get { return (float)-distancebar.Value; } }
		public float Angle { get { return Angle2D.DegToRad((float)-anglebar.Value); } }
		public bool FixedCurve { get { return circular.Checked; } }
		public bool Backwards { get { return backwards.Checked; } }

		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		public CurveLinedefsForm()
		{
			// Initialize
			InitializeComponent();

			// Set negative properties for stupid
			// scrollbars that work the other way around
			verticesbar.Maximum = -MIN_VERTICES;
			verticesbar.Minimum = -MAX_VERTICES;
			distancebar.Maximum = -MIN_DISTANCE;
			distancebar.Minimum = -MAX_DISTANCE;
			anglebar.Maximum = -MIN_ANGLE;
			anglebar.Minimum = -MAX_ANGLE;
		}

		#endregion

		#region ================== Interface
		
		// Window closing
		private void CurveLinedefsForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			// User closing the window?
			if(e.CloseReason == CloseReason.UserClosing)
			{
				// Just cancel
				General.Editing.CancelMode();
				e.Cancel = true;
			}
		}

		// This shows the window
		public void Show(Form owner)
		{
			// First time showing?
			//if((this.Location.X == 0) && (this.Location.Y == 0))
			{
				// Position at left-top of owner
				this.Location = new Point(owner.Location.X + 20, owner.Location.Y + 90); 
			}

			// Show window
			base.Show(owner);
		}
		
		// Vertices bar changed
		private void verticesbar_ValueChanged(object sender, EventArgs e)
		{
			int v = -verticesbar.Value;
			vertices.Text = v.ToString();
			General.Interface.RedrawDisplay();
		}

		// Vertices loses focus
		private void vertices_Leave(object sender, EventArgs e)
		{
			int v = -verticesbar.Value;
			vertices.Text = v.ToString();
		}

		// Vertices change
		private void vertices_TextChanged(object sender, EventArgs e)
		{
			int result = -vertices.GetResult(-verticesbar.Value);
			if((result >= verticesbar.Minimum) && (result <= verticesbar.Maximum)) verticesbar.Value = result;
		}

		// Distance bar changed
		private void distancebar_ValueChanged(object sender, EventArgs e)
		{
			int v = -distancebar.Value;
			distance.Text = v.ToString();
			General.Interface.RedrawDisplay();
		}

		// Distance loses focus
		private void distance_Leave(object sender, EventArgs e)
		{
			int v = -distancebar.Value;
			distance.Text = v.ToString();
		}

		// Distance changed
		private void distance_TextChanged(object sender, EventArgs e)
		{
			int result = -distance.GetResult(-distancebar.Value);
			if((result >= distancebar.Minimum) && (result <= distancebar.Maximum)) distancebar.Value = result;
		}

		// Angle bar changed
		private void anglebar_ValueChanged(object sender, EventArgs e)
		{
			int v = -anglebar.Value;
			angle.Text = v.ToString();
			General.Interface.RedrawDisplay();
		}

		// Angle loses focus
		private void angle_Leave(object sender, EventArgs e)
		{
			int v = -anglebar.Value;
			angle.Text = v.ToString();
		}

		// Angle changed
		private void angle_TextChanged(object sender, EventArgs e)
		{
			int result = -angle.GetResult(-anglebar.Value);
			if((result >= anglebar.Minimum) && (result <= anglebar.Maximum)) anglebar.Value = result;
		}

		// Circular curve switched
		private void circular_CheckedChanged(object sender, EventArgs e)
		{
			// Enable/disable controls
			distance.Enabled = !circular.Checked;
			distancebar.Enabled = !circular.Checked;
			distancelabel.Enabled = !circular.Checked;
			General.Interface.RedrawDisplay();
		}

		// Curve backwards switched
		private void backwards_CheckedChanged(object sender, EventArgs e)
		{
			General.Interface.RedrawDisplay();
		}
		
		// Cancel clicked
		private void cancel_Click(object sender, EventArgs e)
		{
			// Cancel now
			General.Editing.CancelMode();
		}

		// Apply clicked
		private void apply_Click(object sender, EventArgs e)
		{
			// Apply now
			General.Editing.AcceptMode();
		}

		private void CurveLinedefsForm_HelpRequested(object sender, HelpEventArgs hlpevent)
		{
			General.ShowHelp("e_curvelinedefs.html");
		}
		
		#endregion
	}
}