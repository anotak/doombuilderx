
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
using CodeImp.DoomBuilder.Actions;

#endregion

namespace CodeImp.DoomBuilder.BuilderModes
{
	[EditMode(DisplayName = "Drawing Mode",
			  SwitchAction = "drawlinesmode",
			  AllowCopyPaste = false,
			  Volatile = true,
			  UseByDefault = true,
			  Optional = false)]

	public class DrawGeometryMode : BaseClassicMode
	{
		#region ================== Constants

		private const float LINE_THICKNESS = 0.8f;

		#endregion

		#region ================== Variables

		// Drawing points
		private List<DrawnVertex> points;
		private List<LineLengthLabel> labels;
        private DrawnVertex prevcurp;

		// Keep track of view changes
		private float lastoffsetx;
		private float lastoffsety;
		private float lastscale;

		// Options
		private bool snaptogrid;		// SHIFT to toggle
		private bool snaptonearest;		// CTRL to enable
        private bool bPenMode;          // ALT to enable
		
		#endregion

		#region ================== Properties

		// Just keep the base mode button checked
		public override string EditModeButtonName { get { return General.Editing.PreviousStableMode.Name; } }

		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		public DrawGeometryMode()
		{
			// Initialize
			points = new List<DrawnVertex>();

            labels = new List<LineLengthLabel>();
			
			// No selection in this mode
			General.Map.Map.ClearAllSelected();
			General.Map.Map.ClearAllMarks(false);
			
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
				if(labels != null)
					foreach(LineLengthLabel l in labels) l.Dispose();
				
				// Done
				base.Dispose();
			}
		}

		#endregion

		#region ================== Methods

		// This checks if the view offset/zoom changed and updates the check
		protected bool CheckViewChanged()
		{
			bool viewchanged = false;

			// View changed?
			if(renderer.OffsetX != lastoffsetx) viewchanged = true;
			if(renderer.OffsetY != lastoffsety) viewchanged = true;
			if(renderer.Scale != lastscale) viewchanged = true;

			// Keep view information
			lastoffsetx = renderer.OffsetX;
			lastoffsety = renderer.OffsetY;
			lastscale = renderer.Scale;

			// Return result
			return viewchanged;
		}
		
		// This updates the dragging
		private void Update()
		{
			PixelColor stitchcolor = General.Colors.Highlight;
			PixelColor losecolor = General.Colors.Selection;
			PixelColor color;

			snaptogrid = General.Interface.ShiftState ^ General.Interface.SnapToGrid;
			snaptonearest = General.Interface.CtrlState ^ General.Interface.AutoMerge;
            bPenMode = General.Interface.AltState;

            DrawnVertex lastp = new DrawnVertex();
			DrawnVertex curp = GetCurrentPosition();
			float vsize = ((float)renderer.VertexSize + 1.0f) / renderer.Scale;
			float vsizeborder = ((float)renderer.VertexSize + 3.0f) / renderer.Scale;

			// The last label's end must go to the mouse cursor
			if(labels.Count > 0) labels[labels.Count - 1].End = curp.pos;

			// Render drawing lines
			if(renderer.StartOverlay(true))
			{
				// Go for all points to draw lines
				if(points.Count > 0)
				{
					// Render lines
					lastp = points[0];
					for(int i = 1; i < points.Count; i++)
					{
						// Determine line color
						if(lastp.stitchline && points[i].stitchline) color = stitchcolor;
						else color = losecolor;

						// Render line
						renderer.RenderLine(lastp.pos, points[i].pos, LINE_THICKNESS, color, true);
						lastp = points[i];
					}

					// Determine line color
					if(lastp.stitchline && snaptonearest) color = stitchcolor;
					else color = losecolor;

					// Render line to cursor
					renderer.RenderLine(lastp.pos, curp.pos, LINE_THICKNESS, color, true);

					// Render vertices
					for(int i = 0; i < points.Count; i++)
					{
						// Determine vertex color
						if(points[i].stitch) color = stitchcolor;
						else color = losecolor;

						// Render vertex
						renderer.RenderRectangleFilled(new RectangleF(points[i].pos.x - vsize, points[i].pos.y - vsize, vsize * 2.0f, vsize * 2.0f), color, true);
					}
				}

				// Determine point color
				if(snaptonearest) color = stitchcolor;
				else color = losecolor;

				// Render vertex at cursor
				renderer.RenderRectangleFilled(new RectangleF(curp.pos.x - vsize, curp.pos.y - vsize, vsize * 2.0f, vsize * 2.0f), color, true);

				// Go for all labels
				foreach(LineLengthLabel l in labels) renderer.RenderText(l.TextLabel);

				// Done
				renderer.Finish();
			}

			// Done
			renderer.Present();

            if (bPenMode && !(lastp.pos.x == curp.pos.x && lastp.pos.y == curp.pos.y))
            {
                if (!(curp.pos.x == prevcurp.pos.x && curp.pos.y == prevcurp.pos.y)
                    &&
                    !(lastp.pos.x == prevcurp.pos.x && lastp.pos.y == prevcurp.pos.y))
                {
                    float curangle = Vector2D.GetAngle(prevcurp.pos, curp.pos);
                    float oldangle = Vector2D.GetAngle(lastp.pos, prevcurp.pos);
                    //float oldangle = Vector2D.GetAngle(points[points.Count - 2].pos, lastp.pos);
                    if (Math.Abs(curangle - oldangle) > 0.001f)
                    {
                        /*
                        General.Interface.DisplayStatus(StatusType.Info,
                            "(" + lastp.pos.x + "," + lastp.pos.y + "); " +
                            "(" + prevcurp.pos.x + "," + prevcurp.pos.y + "); " +
                            "(" + curp.pos.x + "," + curp.pos.y + "); "
                            );
                        */
                        if (!DrawPointAt(prevcurp)) General.Interface.DisplayStatus(StatusType.Warning, "Failed to draw point: outside of map boundaries.");
                        //DrawPoint();
                    }
                }
                else if (points.Count >= 2 && points[0].pos.x == curp.pos.x && points[0].pos.y == curp.pos.y)
                {
                    DrawPoint();
                }
            }
            prevcurp = curp;
		}
		
		// This returns the aligned and snapped draw position
		public static DrawnVertex GetCurrentPosition(Vector2D mousemappos, bool snaptonearest, bool snaptogrid, IRenderer2D renderer, List<DrawnVertex> points)
		{
			DrawnVertex p = new DrawnVertex();
			Vector2D vm = mousemappos;
			float vrange = BuilderPlug.Me.StitchRange / renderer.Scale;

			// Snap to nearest?
			if(snaptonearest)
			{
				// Go for all drawn points
				foreach(DrawnVertex v in points)
				{
					if(Vector2D.DistanceSq(mousemappos, v.pos) < (vrange * vrange))
					{
						p.pos = v.pos;
						p.stitch = true;
						p.stitchline = true;
						return p;
					}
				}

				// Try the nearest vertex
				Vertex nv = General.Map.Map.NearestVertexSquareRange(mousemappos, vrange);
				if(nv != null)
				{
					p.pos = nv.Position;
					p.stitch = true;
					p.stitchline = true;
					return p;
				}

				// Try the nearest linedef
				Linedef nl = General.Map.Map.NearestLinedefRange(mousemappos, BuilderPlug.Me.StitchRange / renderer.Scale);
				if(nl != null)
				{
					// Snap to grid?
					if(snaptogrid)
					{
						// Get grid intersection coordinates
						List<Vector2D> coords = nl.GetGridIntersections();

						// Find nearest grid intersection
						bool found = false;
						float found_distance = float.MaxValue;
						Vector2D found_coord = new Vector2D();
						foreach(Vector2D v in coords)
						{
							Vector2D delta = mousemappos - v;
							if(delta.GetLengthSq() < found_distance)
							{
								found_distance = delta.GetLengthSq();
								found_coord = v;
								found = true;
							}
						}

						if(found)
						{
							// Align to the closest grid intersection
							p.pos = found_coord;
							p.stitch = true;
							p.stitchline = true;
							return p;
						}
					}
					else
					{
						// Aligned to line
						p.pos = nl.NearestOnLine(mousemappos);
						p.stitch = true;
						p.stitchline = true;
						return p;
					}
				}
			}
			else
			{
				// Always snap to the first drawn vertex so that the user can finish a complete sector without stitching
				if(points.Count > 0)
				{
					if(Vector2D.DistanceSq(mousemappos, points[0].pos) < (vrange * vrange))
					{
						p.pos = points[0].pos;
						p.stitch = true;
						p.stitchline = false;
						return p;
					}
				}
			}

			// if the mouse cursor is outside the map bondaries check if the line between the last set point and the
			// mouse cursor intersect any of the boundary lines. If it does, set the position to this intersection
			if (points.Count > 0 &&
				(mousemappos.x < General.Map.Config.LeftBoundary || mousemappos.x > General.Map.Config.RightBoundary ||
				mousemappos.y > General.Map.Config.TopBoundary || mousemappos.y < General.Map.Config.BottomBoundary))
			{
				Line2D dline = new Line2D(mousemappos, points[points.Count - 1].pos);
				bool foundintersection = false;
				float u = 0.0f;
				List<Line2D> blines = new List<Line2D>();

				// lines for left, top, right and bottom bondaries
				blines.Add(new Line2D(General.Map.Config.LeftBoundary, General.Map.Config.BottomBoundary, General.Map.Config.LeftBoundary, General.Map.Config.TopBoundary));
				blines.Add(new Line2D(General.Map.Config.LeftBoundary, General.Map.Config.TopBoundary, General.Map.Config.RightBoundary, General.Map.Config.TopBoundary));
				blines.Add(new Line2D(General.Map.Config.RightBoundary, General.Map.Config.TopBoundary, General.Map.Config.RightBoundary, General.Map.Config.BottomBoundary));
				blines.Add(new Line2D(General.Map.Config.RightBoundary, General.Map.Config.BottomBoundary, General.Map.Config.LeftBoundary, General.Map.Config.BottomBoundary));

				// check for intersections with boundaries
				for (int i = 0; i < blines.Count; i++)
				{
					if (!foundintersection)
					{
						// only check for intersection if the last set point is not on the
						// line we are checking against
						if (blines[i].GetSideOfLine(points[points.Count - 1].pos) != 0.0f)
						{
							foundintersection = blines[i].GetIntersection(dline, out u);
						}
					}
				}

				// if there was no intersection set the position to the last set point
				if (!foundintersection)
					vm = points[points.Count - 1].pos;
				else
					vm = dline.GetCoordinatesAt(u);

			}


			// Snap to grid?
			if(snaptogrid)
			{
				// Aligned to grid
				p.pos = General.Map.Grid.SnappedToGrid(vm);

				// special handling 
				if (p.pos.x > General.Map.Config.RightBoundary) p.pos.x = General.Map.Config.RightBoundary;
				if (p.pos.y < General.Map.Config.BottomBoundary) p.pos.y = General.Map.Config.BottomBoundary;
				p.stitch = snaptonearest;
				p.stitchline = snaptonearest;
				return p;
			}
			else
			{
				// Normal position
				p.pos = vm;
				p.stitch = snaptonearest;
				p.stitchline = snaptonearest;
				return p;
			}
		}
		
		// This gets the aligned and snapped draw position
		private DrawnVertex GetCurrentPosition()
		{
			return GetCurrentPosition(mousemappos, snaptonearest, snaptogrid, renderer, points);
		}
		
		// This draws a point at a specific location
		public bool DrawPointAt(DrawnVertex p)
		{
			return DrawPointAt(p.pos, p.stitch, p.stitchline);
		}
		
		// This draws a point at a specific location
		public bool DrawPointAt(Vector2D pos, bool stitch, bool stitchline)
		{
			if (pos.x < General.Map.Config.LeftBoundary || pos.x > General.Map.Config.RightBoundary ||
				pos.y > General.Map.Config.TopBoundary || pos.y < General.Map.Config.BottomBoundary)
				return false;

            if (points.Count > 0 && (points[points.Count - 1].pos - pos).GetLengthSq() < 0.001f)
            {
                return true;
            }

			DrawnVertex newpoint = new DrawnVertex();
			newpoint.pos = pos;
			newpoint.stitch = stitch;
			newpoint.stitchline = stitchline;
			points.Add(newpoint);
			labels.Add(new LineLengthLabel());
			labels[labels.Count - 1].Start = newpoint.pos;
			if(labels.Count > 1) labels[labels.Count - 2].End = newpoint.pos;
			Update();

			// Check if point stitches with the first
			if((points.Count > 1) && points[points.Count - 1].stitch)
			{
				Vector2D p1 = points[0].pos;
				Vector2D p2 = points[points.Count - 1].pos;
				Vector2D delta = p1 - p2;
				if((Math.Abs(delta.x) <= 0.001f) && (Math.Abs(delta.y) <= 0.001f))
				{
					// Finish drawing
					FinishDraw();
				}
			}

			return true;
		}
		
		#endregion

		#region ================== Events

		public override void OnHelp()
		{
			General.ShowHelp("e_drawgeometry.html");
		}

		// Engaging
		public override void OnEngage()
		{
			base.OnEngage();
			EnableAutoPanning();
			renderer.SetPresentation(Presentation.Standard);
			
			// Set cursor
			General.Interface.SetCursor(Cursors.Cross);
            General.Interface.DisplayStatus(StatusType.Info, "Drawing geometry! Hold shift to disable snap-to-grid, ctrl to disable snap-to-vertices, alt for 'pen mode'!");
        }

		// Disengaging
		public override void OnDisengage()
		{
			base.OnDisengage();
			DisableAutoPanning();
		}
		
		// Cancelled
		public override void OnCancel()
		{
			// Cancel base class
			base.OnCancel();
			
			// Return to original mode
			General.Editing.ChangeMode(General.Editing.PreviousStableMode.Name);
		}

		// Accepted
		public override void OnAccept()
		{
			Cursor.Current = Cursors.AppStarting;

			General.Settings.FindDefaultDrawSettings();

			// When points have been drawn
			if(points.Count > 0)
			{
				// Make undo for the draw
				General.Map.UndoRedo.CreateUndo("Line draw");
				
				// Make an analysis and show info
				string[] adjectives = new string[]
				{ "beautiful", "lovely", "romantic", "stylish", "cheerful", "comical",
				  "awesome", "accurate", "adorable", "adventurous", "attractive", "cute",
				  "elegant", "glamorous", "gorgeous", "handsome", "magnificent", "unusual",
				  "outstanding", "mysterious", "amusing", "charming", "fantastic", "jolly",
                  "demonic", "magical", "otherworldly", "wonderful", "enigmatic",
                  "provocative", "shapely", "captivating", "intriguing", "refreshing" };
				string word = adjectives[points.Count % adjectives.Length];
				word = (points.Count > adjectives.Length) ? "very " + word : word;
				string a = ((word[0] == 'a') || (word[0] == 'e') || (word[0] == 'i') || (word[0] == 'o') || (word[0] == 'a')) ? "an " : "a ";
				General.Interface.DisplayStatus(StatusType.Action, "Created " + a + word + " drawing.");
				
				// Make the drawing
				if(!Tools.DrawLines(points))
				{
					// Drawing failed
					// NOTE: I have to call this twice, because the first time only cancels this volatile mode
					General.Map.UndoRedo.WithdrawUndo();
					General.Map.UndoRedo.WithdrawUndo();
					return;
				}

				// Snap to map format accuracy
				General.Map.Map.SnapAllToAccuracy();
				
				// Clear selection
				General.Map.Map.ClearAllSelected();
				
				// Update cached values
				General.Map.Map.Update();

				// Edit new sectors?
				List<Sector> newsectors = General.Map.Map.GetMarkedSectors(true);
				if(BuilderPlug.Me.EditNewSector && (newsectors.Count > 0))
					General.Interface.ShowEditSectors(newsectors);
				
				// Update the used textures
				General.Map.Data.UpdateUsedTextures();
				
				// Map is changed
				General.Map.IsChanged = true;
			}

			// Done
			Cursor.Current = Cursors.Default;
			
			// Return to original mode
			General.Editing.ChangeMode(General.Editing.PreviousStableMode.Name);
		}

		// This redraws the display
		public override void OnRedrawDisplay()
		{
			renderer.RedrawSurface();

			// Render lines
			if(renderer.StartPlotter(true))
			{
				renderer.PlotLinedefSet(General.Map.Map.Linedefs);
				renderer.PlotVerticesSet(General.Map.Map.Vertices);
				renderer.Finish();
			}

			// Render things
			if(renderer.StartThings(true))
			{
				renderer.RenderThingSet(General.Map.Map.Things, 1.0f);
				renderer.Finish();
			}

			// Normal update
			Update();
		}
		
		// Mouse moving
		public override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);
			Update();
		}

		// When a key is released
		public override void OnKeyUp(KeyEventArgs e)
		{
			base.OnKeyUp(e);
			if((snaptogrid != (General.Interface.ShiftState ^ General.Interface.SnapToGrid)) ||
			   (snaptonearest != (General.Interface.CtrlState ^ General.Interface.AutoMerge))) Update();
		}

		// When a key is pressed
		public override void OnKeyDown(KeyEventArgs e)
		{
			base.OnKeyDown(e);
			if((snaptogrid != (General.Interface.ShiftState ^ General.Interface.SnapToGrid)) ||
			   (snaptonearest != (General.Interface.CtrlState ^ General.Interface.AutoMerge))) Update();
		}
		
		#endregion
		
		#region ================== Actions
		
		// Drawing a point
		[BeginAction("drawpoint")]
		public void DrawPoint()
		{
			// Mouse inside window?
			if(General.Interface.MouseInDisplay)
			{
				DrawnVertex newpoint = GetCurrentPosition();

				if(!DrawPointAt(newpoint)) General.Interface.DisplayStatus(StatusType.Warning, "Failed to draw point: outside of map boundaries.");
			}
		}
		
		// Remove a point
		[BeginAction("removepoint")]
		public void RemovePoint()
		{
			if(points.Count > 0) points.RemoveAt(points.Count - 1);
			if(labels.Count > 0)
			{
				labels[labels.Count - 1].Dispose();
				labels.RemoveAt(labels.Count - 1);
			}
			
			Update();
		}
		
		// Finish drawing
		[BeginAction("finishdraw")]
		public void FinishDraw()
		{
			// Accept the changes
			General.Editing.AcceptMode();
		}
		
		#endregion
	}
}
