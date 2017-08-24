
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
using CodeImp.DoomBuilder.Editing;
using CodeImp.DoomBuilder.Actions;

#endregion

namespace CodeImp.DoomBuilder.BuilderModes
{
	[EditMode(DisplayName = "Vertices Mode",
			  SwitchAction = "verticesmode",	// Action name used to switch to this mode
		      ButtonImage = "VerticesMode.png",	// Image resource name for the button
			  ButtonOrder = int.MinValue + 0,	// Position of the button (lower is more to the left)
			  ButtonGroup = "000_editing",
			  UseByDefault = true,
			  SafeStartMode = true)]

	public class VerticesMode : BaseClassicMode
	{
		#region ================== Constants

		#endregion

		#region ================== Variables

		// Highlighted item
		protected Vertex highlighted;

		// Interface
		private bool editpressed;

		#endregion

		#region ================== Properties

		public override object HighlightedObject { get { return highlighted; } }

		#endregion

		#region ================== Constructor / Disposer

		#endregion

		#region ================== Methods

		public override void OnHelp()
		{
			General.ShowHelp("e_vertices.html");
		}

		// Cancel mode
		public override void OnCancel()
		{
			base.OnCancel();
			
			// Return to this mode
			General.Editing.ChangeMode(new VerticesMode());
		}

		// Mode engages
		public override void OnEngage()
		{
			base.OnEngage();
			renderer.SetPresentation(Presentation.Standard);

			// Add toolbar buttons
			General.Interface.AddButton(BuilderPlug.Me.MenusForm.CopyProperties);
			General.Interface.AddButton(BuilderPlug.Me.MenusForm.PasteProperties);
			
			// Convert geometry selection to vertices only
			General.Map.Map.ConvertSelection(SelectionType.Vertices);
		}

		// Mode disengages
		public override void OnDisengage()
		{
			base.OnDisengage();

			// Remove toolbar buttons
			General.Interface.RemoveButton(BuilderPlug.Me.MenusForm.CopyProperties);
			General.Interface.RemoveButton(BuilderPlug.Me.MenusForm.PasteProperties);
			
			// Going to EditSelectionMode?
			if(General.Editing.NewMode is EditSelectionMode)
			{
				// Not pasting anything?
				EditSelectionMode editmode = (General.Editing.NewMode as EditSelectionMode);
				if(!editmode.Pasting)
				{
					// No selection made? But we have a highlight!
					if((General.Map.Map.GetSelectedVertices(true).Count == 0) && (highlighted != null))
					{
						// Make the highlight the selection
						highlighted.Selected = true;
					}
				}
			}

			// Hide highlight info
			General.Interface.HideInfo();
		}

		// This redraws the display
		public override void OnRedrawDisplay()
		{
			renderer.RedrawSurface();

			// Render lines and vertices
			if(renderer.StartPlotter(true))
			{
				renderer.PlotLinedefSet(General.Map.Map.Linedefs);
				renderer.PlotVerticesSet(General.Map.Map.Vertices);
				if((highlighted != null) && !highlighted.IsDisposed)
					renderer.PlotVertex(highlighted, ColorCollection.HIGHLIGHT);
				renderer.Finish();
			}

			// Render things
			if(renderer.StartThings(true))
			{
				renderer.RenderThingSet(General.Map.ThingsFilter.HiddenThings, Presentation.THINGS_HIDDEN_ALPHA);
				renderer.RenderThingSet(General.Map.ThingsFilter.VisibleThings, 1.0f);
				renderer.Finish();
			}

			// Selecting?
			if(selecting)
			{
				// Render selection
				if(renderer.StartOverlay(true))
				{
					RenderMultiSelection();
					renderer.Finish();
				}
			}

			renderer.Present();
		}
		
		// This highlights a new item
		protected void Highlight(Vertex v)
		{
			// Update display
			if(renderer.StartPlotter(false))
			{
				// Undraw previous highlight
				if((highlighted != null) && !highlighted.IsDisposed)
					renderer.PlotVertex(highlighted, renderer.DetermineVertexColor(highlighted));

				// Set new highlight
				highlighted = v;

				// Render highlighted item
				if((highlighted != null) && !highlighted.IsDisposed)
					renderer.PlotVertex(highlighted, ColorCollection.HIGHLIGHT);
				
				// Done
				renderer.Finish();
				renderer.Present();
			}

			// Show highlight info
			if((highlighted != null) && !highlighted.IsDisposed)
				General.Interface.ShowVertexInfo(highlighted);
			else
				General.Interface.HideInfo();
		}
		
		// Selection
		protected override void OnSelectBegin()
		{
			// Item highlighted?
			if((highlighted != null) && !highlighted.IsDisposed)
			{
				// Flip selection
				highlighted.Selected = !highlighted.Selected;

				// Redraw highlight to show selection
				if(renderer.StartPlotter(false))
				{
					renderer.PlotVertex(highlighted, renderer.DetermineVertexColor(highlighted));
					renderer.Finish();
					renderer.Present();
				}
			}
			else
			{
				// Start making a selection
				StartMultiSelection();
			}

			base.OnSelectBegin();
		}
		
		// End selection
		protected override void OnSelectEnd()
		{
			// Not stopping from multiselection?
			if(!selecting)
			{
				// Item highlighted?
				if((highlighted != null) && !highlighted.IsDisposed)
				{
					// Render highlighted item
					if(renderer.StartPlotter(false))
					{
						renderer.PlotVertex(highlighted, ColorCollection.HIGHLIGHT);
						renderer.Finish();
						renderer.Present();
					}
				}
			}

			base.OnSelectEnd();
		}
		
		// Start editing
		protected override void OnEditBegin()
		{
			bool snaptogrid = General.Interface.ShiftState ^ General.Interface.SnapToGrid;
			bool snaptonearest = General.Interface.CtrlState ^ General.Interface.AutoMerge;
			
			// Vertex highlighted?
			if((highlighted != null) && !highlighted.IsDisposed)
			{
				// Edit pressed in this mode
				editpressed = true;

				// Highlighted item not selected?
				if(!highlighted.Selected && (BuilderPlug.Me.AutoClearSelection || (General.Map.Map.SelectedVerticessCount == 0)))
				{
					// Make this the only selection
					General.Map.Map.ClearSelectedVertices();
					highlighted.Selected = true;
					General.Interface.RedrawDisplay();
				}

				// Update display
				if(renderer.StartPlotter(false))
				{
					// Redraw highlight to show selection
					renderer.PlotVertex(highlighted, renderer.DetermineVertexColor(highlighted));
					renderer.Finish();
					renderer.Present();
				}
			}
			else
			{
				// Find the nearest linedef within highlight range
				Linedef l = General.Map.Map.NearestLinedefRange(mousemappos, BuilderPlug.Me.SplitLinedefsRange / renderer.Scale);
				if(l != null)
				{
					// Create undo
					General.Map.UndoRedo.CreateUndo("Split linedef");

					Vector2D insertpos;

					// Snip to grid also?
					if(snaptogrid)
					{
						// Find all points where the grid intersects the line
						List<Vector2D> points = l.GetGridIntersections();
						insertpos = mousemappos;
						float distance = float.MaxValue;
						foreach(Vector2D p in points)
						{
							float pdist = Vector2D.DistanceSq(p, mousemappos);
							if(pdist < distance)
							{
								insertpos = p;
								distance = pdist;
							}
						}
					}
					else
					{
						// Just use the nearest point on line
						insertpos = l.NearestOnLine(mousemappos);
					}

					// Make the vertex
					Vertex v = General.Map.Map.CreateVertex(insertpos);
					if(v == null)
					{
						General.Map.UndoRedo.WithdrawUndo();
						return;
					}
					
					// Snap to map format accuracy
					v.SnapToAccuracy();

					// Split the line with this vertex
					Linedef sld = l.Split(v);
					if(sld == null)
					{
						General.Map.UndoRedo.WithdrawUndo();
						return;
					}
					BuilderPlug.Me.AdjustSplitCoordinates(l, sld);
					
					// Update
					General.Map.Map.Update();

					// Highlight it
					Highlight(v);

					// Redraw display
					General.Interface.RedrawDisplay();
				}
				else
				{
					// Start drawing mode
					DrawGeometryMode drawmode = new DrawGeometryMode();
					DrawnVertex v = DrawGeometryMode.GetCurrentPosition(mousemappos, snaptonearest, snaptogrid, renderer, new List<DrawnVertex>());

					if (drawmode.DrawPointAt(v))
						General.Editing.ChangeMode(drawmode);
					else
						General.Interface.DisplayStatus(StatusType.Warning, "Failed to draw point: outside of map boundaries.");
				}
			}
			
			base.OnEditBegin();
		}
		
		// Done editing
		protected override void OnEditEnd()
		{
			// Edit pressed in this mode?
			if(editpressed)
			{
				// Anything selected?
				ICollection<Vertex> selected = General.Map.Map.GetSelectedVertices(true);
				if(selected.Count > 0)
				{
					if(General.Interface.IsActiveWindow)
					{
						// Show line edit dialog
						General.Interface.ShowEditVertices(selected);
						General.Map.Map.Update();

						// When a single vertex was selected, deselect it now
						if(selected.Count == 1) General.Map.Map.ClearSelectedVertices();

						// Update entire display
						General.Interface.RedrawDisplay();
					}
				}
			}

			editpressed = false;
			base.OnEditEnd();
		}
		
		// Mouse moves
		public override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);

			// Not holding any buttons?
			if(e.Button == MouseButtons.None)
			{
				// Find the nearest vertex within highlight range
				Vertex v = General.Map.Map.NearestVertexSquareRange(mousemappos, BuilderPlug.Me.HighlightRange / renderer.Scale);

				// Highlight if not the same
				if(v != highlighted) Highlight(v);
			}
		}

		// Mouse leaves
		public override void OnMouseLeave(EventArgs e)
		{
			base.OnMouseLeave(e);
			
			// Highlight nothing
			Highlight(null);
		}

		// Mouse wants to drag
		protected override void OnDragStart(MouseEventArgs e)
		{
			base.OnDragStart(e);

			// Edit button used?
			if(General.Actions.CheckActionActive(null, "classicedit"))
			{
				// Anything highlighted?
				if((highlighted != null) && !highlighted.IsDisposed)
				{
					// Highlighted item not selected?
					if(!highlighted.Selected)
					{
						// Select only this vertex for dragging
						General.Map.Map.ClearSelectedVertices();
						highlighted.Selected = true;
					}

					// Start dragging the selection
					General.Editing.ChangeMode(new DragVerticesMode(highlighted, mousedownmappos));
				}
			}
		}

		// This is called wheh selection ends
		protected override void OnEndMultiSelection()
		{
			bool selectionvolume = ((Math.Abs(base.selectionrect.Width) > 0.1f) && (Math.Abs(base.selectionrect.Height) > 0.1f));

			if(BuilderPlug.Me.AutoClearSelection && !selectionvolume)
				General.Map.Map.ClearSelectedVertices();

			if(selectionvolume)
			{
				if(General.Interface.ShiftState ^ BuilderPlug.Me.AdditiveSelect)
				{
					// Go for all vertices
					foreach(Vertex v in General.Map.Map.Vertices)
					{
						v.Selected |= ((v.Position.x >= selectionrect.Left) &&
									   (v.Position.y >= selectionrect.Top) &&
									   (v.Position.x <= selectionrect.Right) &&
									   (v.Position.y <= selectionrect.Bottom));
					}
				}
				else
				{
					// Go for all vertices
					foreach(Vertex v in General.Map.Map.Vertices)
					{
						v.Selected = ((v.Position.x >= selectionrect.Left) &&
									  (v.Position.y >= selectionrect.Top) &&
									  (v.Position.x <= selectionrect.Right) &&
									  (v.Position.y <= selectionrect.Bottom));
					}
				}
			}
			
			base.OnEndMultiSelection();

			// Clear overlay
			if(renderer.StartOverlay(true)) renderer.Finish();

			// Redraw
			General.Interface.RedrawDisplay();
		}

		// This is called when the selection is updated
		protected override void OnUpdateMultiSelection()
		{
			base.OnUpdateMultiSelection();

			// Render selection
			if(renderer.StartOverlay(true))
			{
				RenderMultiSelection();
				renderer.Finish();
				renderer.Present();
			}
		}
		
		// When copying
		public override bool OnCopyBegin()
		{
			// No selection made? But we have a highlight!
			if((General.Map.Map.GetSelectedVertices(true).Count == 0) && (highlighted != null))
			{
				// Make the highlight the selection
				highlighted.Selected = true;
			}
			
			return base.OnCopyBegin();
		}
		
		#endregion

		#region ================== Actions

		// This copies the properties
		[BeginAction("classiccopyproperties")]
		public void CopyProperties()
		{
			// Determine source vertices
			ICollection<Vertex> sel = null;
			if(General.Map.Map.SelectedVerticessCount > 0)
				sel = General.Map.Map.GetSelectedVertices(true);
			else if(highlighted != null)
			{
				sel = new List<Vertex>();
				sel.Add(highlighted);
			}

			if(sel != null)
			{
				// Copy properties from first source vertex
				BuilderPlug.Me.CopiedVertexProps = new VertexProperties(General.GetByIndex(sel, 0));
				General.Interface.DisplayStatus(StatusType.Action, "Copied vertex properties.");
			}
		}

		// This pastes the properties
		[BeginAction("classicpasteproperties")]
		public void PasteProperties()
		{
			if(BuilderPlug.Me.CopiedVertexProps != null)
			{
				// Determine target vertices
				ICollection<Vertex> sel = null;
				if(General.Map.Map.SelectedVerticessCount > 0)
					sel = General.Map.Map.GetSelectedVertices(true);
				else if(highlighted != null)
				{
					sel = new List<Vertex>();
					sel.Add(highlighted);
				}

				if(sel != null)
				{
					// Apply properties to selection
					General.Map.UndoRedo.CreateUndo("Paste vertex properties");
					foreach(Vertex v in sel)
					{
						BuilderPlug.Me.CopiedVertexProps.Apply(v);
					}
					General.Interface.DisplayStatus(StatusType.Action, "Pasted vertex properties.");

					// Update and redraw
					General.Map.IsChanged = true;
					General.Interface.RefreshInfo();
					General.Interface.RedrawDisplay();
				}
			}
		}

		// This clears the selection
		[BeginAction("clearselection", BaseAction = true)]
		public void ClearSelection()
		{
			// Clear selection
			General.Map.Map.ClearAllSelected();

			// Redraw
			General.Interface.RedrawDisplay();
		}
		
		// This creates a new vertex at the mouse position
		[BeginAction("insertitem", BaseAction = true)]
		public virtual void InsertVertexAction() { VerticesMode.InsertVertex(mousemappos, renderer.Scale); }
		public static void InsertVertex(Vector2D mousemappos, float rendererscale)
		{
			bool snaptogrid = General.Interface.ShiftState ^ General.Interface.SnapToGrid;
			bool snaptonearest = General.Interface.CtrlState ^ General.Interface.AutoMerge;

			// Mouse in window?
			if(General.Interface.MouseInDisplay)
			{
				Vector2D insertpos;
				Linedef l = null;
				
				// Create undo
				General.Map.UndoRedo.CreateUndo("Insert vertex");

				// Snap to geometry?
				l = General.Map.Map.NearestLinedefRange(mousemappos, BuilderPlug.Me.SplitLinedefsRange / rendererscale);
				if(snaptonearest && (l != null))
				{
					// Snip to grid also?
					if(snaptogrid)
					{
						// Find all points where the grid intersects the line
						List<Vector2D> points = l.GetGridIntersections();
						insertpos = mousemappos;
						float distance = float.MaxValue;
						foreach(Vector2D p in points)
						{
							float pdist = Vector2D.DistanceSq(p, mousemappos);
							if(pdist < distance)
							{
								insertpos = p;
								distance = pdist;
							}
						}
					}
					else
					{
						// Just use the nearest point on line
						insertpos = l.NearestOnLine(mousemappos);
					}
				}
				// Snap to grid?
				else if(snaptogrid)
				{
					// Snap to grid
					insertpos = General.Map.Grid.SnappedToGrid(mousemappos);
				}
				else
				{
					// Just insert here, don't snap to anything
					insertpos = mousemappos;
				}

				// Make the vertex
				Vertex v = General.Map.Map.CreateVertex(insertpos);
				if(v == null)
				{
					General.Map.UndoRedo.WithdrawUndo();
					return;
				}

				// Snap to map format accuracy
				v.SnapToAccuracy();

				// Split the line with this vertex
				if(snaptonearest && (l != null))
				{
					General.Interface.DisplayStatus(StatusType.Action, "Split a linedef.");
					Linedef sld = l.Split(v);
					if(sld == null)
					{
						General.Map.UndoRedo.WithdrawUndo();
						return;
					}
					BuilderPlug.Me.AdjustSplitCoordinates(l, sld);
				}
				else
				{
					General.Interface.DisplayStatus(StatusType.Action, "Inserted a vertex.");
				}

				// Update
				General.Map.Map.Update();

				// Redraw screen
				General.Interface.RedrawDisplay();
			}
		}

		[BeginAction("deleteitem", BaseAction = true)]
		public void DeleteItem()
		{
			// Make list of selected vertices
			ICollection<Vertex> selected = General.Map.Map.GetSelectedVertices(true);
			if((selected.Count == 0) && (highlighted != null) && !highlighted.IsDisposed) selected.Add(highlighted);

			// Anything to do?
			if(selected.Count > 0)
			{
				// Make undo
				if(selected.Count > 1)
				{
					General.Map.UndoRedo.CreateUndo("Delete " + selected.Count + " vertices");
					General.Interface.DisplayStatus(StatusType.Action, "Deleted " + selected.Count + " vertices.");
				}
				else
				{
					General.Map.UndoRedo.CreateUndo("Delete vertex");
					General.Interface.DisplayStatus(StatusType.Action, "Deleted a vertex.");
				}

				// Go for all vertices that need to be removed
				foreach(Vertex v in selected)
				{
					// Not already removed automatically?
					if(!v.IsDisposed)
					{
						// If the vertex only has 2 linedefs attached, then merge the linedefs
						if(v.Linedefs.Count == 2)
						{
							Linedef ld1 = General.GetByIndex(v.Linedefs, 0);
							Linedef ld2 = General.GetByIndex(v.Linedefs, 1);
							Vertex v1 = (ld1.Start == v) ? ld1.End : ld1.Start;
							Vertex v2 = (ld2.Start == v) ? ld2.End : ld2.Start;
							if(ld1.Start == v) ld1.SetStartVertex(v2); else ld1.SetEndVertex(v2);
							//if(ld2.Start == v) ld2.SetStartVertex(v1); else ld2.SetEndVertex(v1);
							//ld1.Join(ld2);
							ld2.Dispose();
						}

						// Trash vertex
						v.Dispose();
					}
				}

				// Update cache values
				General.Map.IsChanged = true;
				General.Map.Map.Update();

				// Invoke a new mousemove so that the highlighted item updates
				MouseEventArgs e = new MouseEventArgs(MouseButtons.None, 0, (int)mousepos.x, (int)mousepos.y, 0);
				OnMouseMove(e);

				// Redraw screen
				General.Interface.RedrawDisplay();
			}
		}
		
		#endregion
	}
}
