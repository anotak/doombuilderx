
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
	public abstract class DragGeometryMode : BaseClassicMode
	{
		#region ================== Constants

		#endregion

		#region ================== Variables
		
		// Mouse position on map where dragging started
		private Vector2D dragstartmappos;

		// Item used as reference for snapping to the grid
		protected Vertex dragitem;
		private Vector2D dragitemposition;

		// List of old vertex positions
		private List<Vector2D> oldpositions;

		// List of selected items
		protected ICollection<Vertex> selectedverts;

		// List of non-selected items
		protected ICollection<Vertex> unselectedverts;

		// List of unstable lines
		protected ICollection<Linedef> unstablelines;
		
		// List of unselected lines
		protected ICollection<Linedef> snaptolines;
		
		// Text labels for all unstable lines
		protected LineLengthLabel[] labels;
		
		// Keep track of view changes
		private float lastoffsetx;
		private float lastoffsety;
		private float lastscale;
		
		// Options
		private bool snaptogrid;		// SHIFT to toggle
		private bool snaptonearest;		// CTRL to enable

		#endregion

		#region ================== Properties

		// Just keep the base mode button checked
		public override string EditModeButtonName { get { return General.Editing.PreviousStableMode.Name; } }
		
		#endregion

		#region ================== Constructor / Disposer

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

		// Constructor to start dragging immediately
		protected void StartDrag(Vector2D dragstartmappos)
		{
			// Initialize
			this.dragstartmappos = dragstartmappos;
			
			Cursor.Current = Cursors.AppStarting;
			
			// We don't want to record this for undoing while we move the geometry around.
			// This will be set back to normal when we're done.
			General.Map.UndoRedo.IgnorePropChanges = true;

			// Make list of selected vertices
			selectedverts = General.Map.Map.GetMarkedVertices(true);

			// Make list of non-selected vertices
			// This will be used for snapping to nearest items
			unselectedverts = General.Map.Map.GetMarkedVertices(false);

			// Get the nearest vertex for snapping
			dragitem = MapSet.NearestVertex(selectedverts, dragstartmappos);
			
			// Lines to snap to
			snaptolines = General.Map.Map.LinedefsFromMarkedVertices(true, false, false);
			
			// Make old positions list
			// We will use this as reference to move the vertices, or to move them back on cancel
			oldpositions = new List<Vector2D>(selectedverts.Count);
			foreach(Vertex v in selectedverts) oldpositions.Add(v.Position);

			// Also keep old position of the dragged item
			dragitemposition = dragitem.Position;

			// Keep view information
			lastoffsetx = renderer.OffsetX;
			lastoffsety = renderer.OffsetY;
			lastscale = renderer.Scale;

			// Make list of unstable lines only
			// These will have their length displayed during the drag
			unstablelines = MapSet.UnstableLinedefsFromVertices(selectedverts);

			// Make text labels
			labels = new LineLengthLabel[unstablelines.Count];
			int index = 0;
			foreach(Linedef l in unstablelines)
				labels[index++] = new LineLengthLabel(l.Start.Position, l.End.Position);
			
			Cursor.Current = Cursors.Default;
		}

		// This moves the selected geometry relatively
		// Returns true when geometry has actually moved
		private bool MoveGeometryRelative(Vector2D offset, bool snapgrid, bool snapnearest)
		{
			Vector2D oldpos = dragitem.Position;
			Vector2D anchorpos = dragitemposition + offset;
			Vector2D tl, br;

			// don't move if the offset contains invalid data
			if (!offset.IsFinite()) return false;

			// Find the outmost vertices
			tl = br = oldpositions[0];
			for (int i = 0; i < oldpositions.Count; i++)
			{
				if (oldpositions[i].x < tl.x) tl.x = (int)oldpositions[i].x;
				if (oldpositions[i].x > br.x) br.x = (int)oldpositions[i].x;
				if (oldpositions[i].y > tl.y) tl.y = (int)oldpositions[i].y;
				if (oldpositions[i].y < br.y) br.y = (int)oldpositions[i].y;
			}
			
			// Snap to nearest?
			if(snapnearest)
			{
				// Find nearest unselected vertex within range
				Vertex nv = MapSet.NearestVertexSquareRange(unselectedverts, anchorpos, BuilderPlug.Me.StitchRange / renderer.Scale);
				if(nv != null)
				{
					// Move the dragged item
					dragitem.Move(nv.Position);

					// Adjust the offset
					offset = nv.Position - dragitemposition;

					// Do not snap to grid!
					snapgrid = false;
				}
				else
				{
					// Find the nearest unselected line within range
					Linedef nl = MapSet.NearestLinedefRange(snaptolines, anchorpos, BuilderPlug.Me.StitchRange / renderer.Scale);
					if(nl != null)
					{
						// Snap to grid?
						if(snaptogrid)
						{
							// Get grid intersection coordinates
							List<Vector2D> coords = nl.GetGridIntersections();

							// Find nearest grid intersection
							float found_distance = float.MaxValue;
							Vector2D found_coord = new Vector2D();
							foreach(Vector2D v in coords)
							{
								Vector2D delta = anchorpos - v;
								if(delta.GetLengthSq() < found_distance)
								{
									found_distance = delta.GetLengthSq();
									found_coord = v;
								}
							}
							
							// Move the dragged item
							dragitem.Move(found_coord);

							// Align to line here
							offset = found_coord - dragitemposition;

							// Do not snap to grid anymore
							snapgrid = false;
						}
						else
						{
							// Move the dragged item
							dragitem.Move(nl.NearestOnLine(anchorpos));

							// Align to line here
							offset = nl.NearestOnLine(anchorpos) - dragitemposition;
						}
					}
				}
			}

			// Snap to grid?
			if(snapgrid)
			{
				// Move the dragged item
				dragitem.Move(anchorpos);

				// Snap item to grid
				dragitem.SnapToGrid();

				// Adjust the offset
				offset += dragitem.Position - anchorpos;
			}

			// Make sure the offset is inside the map boundaries
			if (offset.x + tl.x < General.Map.Config.LeftBoundary) offset.x = General.Map.Config.LeftBoundary - tl.x;
			if (offset.x + br.x > General.Map.Config.RightBoundary) offset.x = General.Map.Config.RightBoundary - br.x;
			if (offset.y + tl.y > General.Map.Config.TopBoundary) offset.y = General.Map.Config.TopBoundary - tl.y;
			if (offset.y + br.y < General.Map.Config.BottomBoundary) offset.y = General.Map.Config.BottomBoundary - br.y;

			// Drag item moved?
			if(!snapgrid || (dragitem.Position != oldpos))
			{
				int i = 0;

				// Move selected geometry
				foreach(Vertex v in selectedverts)
				{
					// Move vertex from old position relative to the
					// mouse position change since drag start
					v.Move(oldpositions[i] + offset);

					// Next
					i++;
				}

				// Update labels
				int index = 0;
				foreach(Linedef l in unstablelines)
					labels[index++].Move(l.Start.Position, l.End.Position);

				// Moved
				return true;
			}
			else
			{
				// No changes
				return false;
			}
		}
		
		// Cancelled
		public override void OnCancel()
		{
			// Move geometry back to original position
			MoveGeometryRelative(new Vector2D(0f, 0f), false, false);
			
			// Resume normal undo/redo recording
			General.Map.UndoRedo.IgnorePropChanges = false;

			// If only a single vertex was selected, deselect it now
			if(selectedverts.Count == 1) General.Map.Map.ClearSelectedVertices();
			
			// Update cached values
			General.Map.Map.Update();
			
			// Cancel base class
			base.OnCancel();
			
			// Return to vertices mode
			General.Editing.ChangeMode(General.Editing.PreviousStableMode.Name);
		}

		// Mode engages
		public override void OnEngage()
		{
			base.OnEngage();
			EnableAutoPanning();
			renderer.SetPresentation(Presentation.Standard);
		}
		
		// Disenagaging
		public override void OnDisengage()
		{
			base.OnDisengage();
			DisableAutoPanning();
			
			// When not cancelled
			if(!cancelled)
			{
				Cursor.Current = Cursors.AppStarting;
				
				// Move geometry back to original position
				MoveGeometryRelative(new Vector2D(0f, 0f), false, false);

				// Resume normal undo/redo recording
				General.Map.UndoRedo.IgnorePropChanges = false;

				// Make undo for the dragging
				General.Map.UndoRedo.CreateUndo("Drag geometry");

				// Move selected geometry to final position
				MoveGeometryRelative(mousemappos - dragstartmappos, snaptogrid, snaptonearest);

				// Stitch geometry
				if(snaptonearest) General.Map.Map.StitchGeometry();

				// Make corrections for backward linedefs
				MapSet.FlipBackwardLinedefs(General.Map.Map.Linedefs);
				
				// Snap to map format accuracy
				General.Map.Map.SnapAllToAccuracy();
				
				// Update cached values
				General.Map.Map.Update();

				// Done
				Cursor.Current = Cursors.Default;
				General.Map.IsChanged = true;
			}
		}

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
			snaptogrid = General.Interface.ShiftState ^ General.Interface.SnapToGrid;
			snaptonearest = General.Interface.CtrlState ^ General.Interface.AutoMerge;
			
			// Move selected geometry
			if(MoveGeometryRelative(mousemappos - dragstartmappos, snaptogrid, snaptonearest))
			{
				// Update cached values
				General.Map.Map.Update(true, false);

				// Redraw
				UpdateRedraw();
				renderer.Present();
				//General.Interface.RedrawDisplay();
			}
		}

		// This redraws only the required things
		protected virtual void UpdateRedraw()
		{
		}

		// When edit button is released
		protected override void OnEditEnd()
		{
			// Just return to base mode, Disengage will be called automatically.
			General.Editing.ChangeMode(General.Editing.PreviousStableMode.Name);

			base.OnEditEnd();
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
	}
}
