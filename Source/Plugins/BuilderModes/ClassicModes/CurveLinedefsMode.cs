
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
	[EditMode(DisplayName = "Curve Linedefs",
			  AllowCopyPaste = false,
			  Volatile = true)]
	public sealed class CurveLinedefsMode : BaseClassicMode
	{
		#region ================== Constants

		private const float LINE_THICKNESS = 0.6f;

		#endregion

		#region ================== Variables

		// Collections
		private ICollection<Linedef> selectedlines;
		private ICollection<Linedef> unselectedlines;
		
		#endregion

		#region ================== Properties

		// Just keep the base mode button checked
		public override string EditModeButtonName { get { return General.Editing.PreviousStableMode.Name; } }

		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		public CurveLinedefsMode(EditMode basemode)
		{
			// Make collections by selection
			selectedlines = General.Map.Map.GetSelectedLinedefs(true);
			unselectedlines = General.Map.Map.GetSelectedLinedefs(false);
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

		// This generates the vertices to split the line with, from start to end
		private List<Vector2D> GenerateCurve(Linedef line)
		{
			// Fetch settings from window
			int vertices = BuilderPlug.Me.CurveLinedefsForm.Vertices;
			float distance = BuilderPlug.Me.CurveLinedefsForm.Distance;
			float angle = BuilderPlug.Me.CurveLinedefsForm.Angle;
			bool fixedcurve = BuilderPlug.Me.CurveLinedefsForm.FixedCurve;
			bool backwards = BuilderPlug.Me.CurveLinedefsForm.Backwards;

			// Make list
			List<Vector2D> points = new List<Vector2D>(vertices);

			//Added by Anders Åstrand 2008-05-18
			//The formulas used are taken from http://mathworld.wolfram.com/CircularSegment.html
			//c and theta are known (length of line and angle parameter). d, R and h are
			//calculated from those two
			//If the curve is not supposed to be a circular segment it's simply deformed to fit
			//the value set for distance.

			//The vertices are generated to be evenly distributed (by angle) along the curve
			//and lastly they are rotated and moved to fit with the original line

			//calculate some identities of a circle segment (refer to the graph in the url above)
			double c = line.Length;
			double theta = angle;

			double d = (c / Math.Tan(theta / 2)) / 2;
			double R = d / Math.Cos(theta / 2);
			double h = R - d;

			double yDeform = fixedcurve ? 1 : distance / h;
			if(backwards)
				yDeform = -yDeform;

			double a, x, y;
			Vector2D vertex;

			for(int v = 1; v <= vertices; v++)
			{
				//calculate the angle for this vertex
				//the curve starts at PI/2 - theta/2 and is segmented into vertices+1 segments
				//this assumes the line is horisontal and on y = 0, the point is rotated and moved later

				a = (Math.PI - theta) / 2 + v * (theta / (vertices + 1));

				//calculate the coordinates of the point, and distort the y coordinate
				//using the deform factor calculated above
				x = Math.Cos(a) * R;
				y = (Math.Sin(a) * R - d) * yDeform;

				//rotate and transform to fit original line
				vertex = new Vector2D((float)x, (float)y).GetRotated(line.Angle + Angle2D.PIHALF);
				vertex = vertex.GetTransformed(line.GetCenterPoint().x, line.GetCenterPoint().y, 1, 1);

				points.Add(vertex);
			}


			// Done
			return points;
		}
		
		#endregion
		
		#region ================== Events

		public override void OnHelp()
		{
			General.ShowHelp("e_curvelinedefs.html");
		}

		// Cancelled
		public override void OnCancel()
		{
			// Cancel base class
			base.OnCancel();

			// Return to base mode
			General.Editing.ChangeMode(General.Editing.PreviousStableMode.Name);
		}

		// Mode engages
		public override void OnEngage()
		{
			base.OnEngage();
			renderer.SetPresentation(Presentation.Standard);
			
			// Show toolbox window
			BuilderPlug.Me.CurveLinedefsForm.Show((Form)General.Interface);
		}

		// Disenagaging
		public override void OnDisengage()
		{
			base.OnDisengage();

			// Hide toolbox window
			BuilderPlug.Me.CurveLinedefsForm.Hide();
		}

		// This applies the curves and returns to the base mode
		public override void OnAccept()
		{
			// Create undo
			General.Map.UndoRedo.CreateUndo("Curve linedefs");
			
			// Go for all selected lines
			foreach(Linedef ld in selectedlines)
			{
				// Make curve for line
				List<Vector2D> points = GenerateCurve(ld);
				if(points.Count > 0)
				{
					// TODO: We may want some sector create/join code in here
					// to allow curves that overlap lines and some geometry merging

					// Go for all points to split the line
					Linedef splitline = ld;
					for(int i = 0; i < points.Count; i++)
					{
						// Make vertex
						Vertex v = General.Map.Map.CreateVertex(points[i]);
						if(v == null)
						{
							General.Map.UndoRedo.WithdrawUndo();
							return;
						}
						
						// Split the line and move on with this line
						splitline = splitline.Split(v);
						if(splitline == null)
						{
							General.Map.UndoRedo.WithdrawUndo();
							return;
						}
					}
				}
			}

			// Snap to map format accuracy
			General.Map.Map.SnapAllToAccuracy();
			
			// Update caches
			General.Map.Map.Update();
			General.Map.IsChanged = true;
			
			// Return to base mode
			General.Editing.ChangeMode(General.Editing.PreviousStableMode.Name);
		}

		// Redrawing display
		public override void OnRedrawDisplay()
		{
			renderer.RedrawSurface();

			// Render lines
			if(renderer.StartPlotter(true))
			{
				renderer.PlotLinedefSet(unselectedlines);
				renderer.PlotVerticesSet(General.Map.Map.Vertices);
				renderer.Finish();
			}

			// Render things
			if(renderer.StartThings(true))
			{
				renderer.RenderThingSet(General.Map.Map.Things, 1.0f);
				renderer.Finish();
			}

			// Render overlay
			if(renderer.StartOverlay(true))
			{
				// Go for all selected lines
				foreach(Linedef ld in selectedlines)
				{
					// Make curve for line
					List<Vector2D> points = GenerateCurve(ld);
					if(points.Count > 0)
					{
						Vector2D p1 = ld.Start.Position;
						Vector2D p2 = points[0];
						for(int i = 1; i <= points.Count; i++)
						{
							// Draw the line
							renderer.RenderLine(p1, p2, LINE_THICKNESS, General.Colors.Highlight, true);

							// Next points
							p1 = p2;
							if(i < points.Count) p2 = points[i];
						}

						// Draw last line
						renderer.RenderLine(p2, ld.End.Position, LINE_THICKNESS, General.Colors.Highlight, true);
					}
				}
				renderer.Finish();
			}

			renderer.Present();
		}
		
		#endregion
	}
}
