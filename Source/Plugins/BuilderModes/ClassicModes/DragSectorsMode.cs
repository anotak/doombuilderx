
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
	// No action or button for this mode, it is automatic.
	// The EditMode attribute does not have to be specified unless the
	// mode must be activated by class name rather than direct instance.
	// In that case, just specifying the attribute like this is enough:
	// [EditMode]

	[EditMode(DisplayName = "Sectors",
			  AllowCopyPaste = false,
			  Volatile = true)]

	public sealed class DragSectorsMode : DragGeometryMode
	{
		#region ================== Constants

		#endregion

		#region ================== Variables

		private ICollection<Linedef> selectedlines;
		private ICollection<Sector> selectedsectors;

		#endregion

		#region ================== Properties
		
		#endregion

		#region ================== Constructor / Disposer

		// Constructor to start dragging immediately
		public DragSectorsMode(Vector2D dragstartmappos)
		{
			// Mark what we are dragging
			General.Map.Map.ClearAllMarks(false);
			General.Map.Map.MarkSelectedLinedefs(true, true);
			ICollection<Vertex> verts = General.Map.Map.GetVerticesFromLinesMarks(true);
			foreach(Vertex v in verts) v.Marked = true;
			
			// Get selected lines
			selectedlines = General.Map.Map.GetSelectedLinedefs(true);
			selectedsectors = General.Map.Map.GetSelectedSectors(true);
			
			// Initialize
			base.StartDrag(dragstartmappos);
			
			// We have no destructor
			GC.SuppressFinalize(this);
		}

		// Disposer
		public override void Dispose()
		{
			// Not already disposed?
			if(!isdisposed)
			{
				// Clean up

				// Done
				base.Dispose();
			}
		}

		#endregion

		#region ================== Methods

		// Mode engages
		public override void OnEngage()
		{
			base.OnEngage();
			renderer.SetPresentation(Presentation.Standard);
		}
		
		// Disenagaging
		public override void OnDisengage()
		{
			// Select vertices from lines selection
			General.Map.Map.ClearSelectedVertices();
			ICollection<Vertex> verts = General.Map.Map.GetVerticesFromLinesMarks(true);
			foreach(Vertex v in verts) v.Selected = true;

			// Perform normal disengage
			base.OnDisengage();

			// Clear vertex selection
			General.Map.Map.ClearSelectedVertices();
			
			// When not cancelled
			if(!cancelled)
			{
				// If only a single sector was selected, deselect it now
				if(selectedsectors.Count == 1)
				{
					General.Map.Map.ClearSelectedSectors();
					General.Map.Map.ClearSelectedLinedefs();
				}
			}
		}

		// This redraws the display
		public override void OnRedrawDisplay()
		{
            if (cancelled)
            {
                return;
            }
            bool viewchanged = CheckViewChanged();

			renderer.RedrawSurface();

			UpdateRedraw();
			
			// Redraw things when view changed
			if(viewchanged)
			{
				if(renderer.StartThings(true))
				{
					renderer.RenderThingSet(General.Map.Map.Things, 1.0f);
					renderer.Finish();
				}
			}

			renderer.Present();
		}
		
		// This redraws only the required things
		protected override void UpdateRedraw()
		{
            if (cancelled)
            {
                return;
            }
			// Start rendering
			if(renderer.StartPlotter(true))
			{
				// Render lines and vertices
				renderer.PlotLinedefSet(snaptolines);
				renderer.PlotLinedefSet(unstablelines);
				renderer.PlotLinedefSet(selectedlines);
				renderer.PlotVerticesSet(General.Map.Map.Vertices);

				// Draw the dragged item highlighted
				// This is important to know, because this item is used
				// for snapping to the grid and snapping to nearest items
				renderer.PlotVertex(dragitem, ColorCollection.HIGHLIGHT);
				
				// Done
				renderer.Finish();
			}

			// Redraw overlay
			if(renderer.StartOverlay(true))
			{
				foreach(LineLengthLabel l in labels)
				{
					renderer.RenderText(l.TextLabel);
				}
				renderer.Finish();
			}
		}
		
		#endregion
	}
}
