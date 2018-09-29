
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

	[EditMode(DisplayName = "Things",
			  AllowCopyPaste = false,
			  Volatile = true)]
	public sealed class DuplicateThingsMode : BaseClassicMode
	{
		#region ================== Constants

		#endregion

		#region ================== Variables

		// Mode to return to
		private EditMode basemode;
		
		// Mouse position on map where dragging started
		private Vector2D dragstartmappos;

		// Item used as reference for snapping to the grid
		private Thing dragitem;
		private Vector2D dragitemposition;

		// List of old thing positions
		private List<Vector2D> oldpositions;

		// List of selected items
		private ICollection<Thing> selectedthings;

		// List of non-selected items
		private ICollection<Thing> unselectedthings;
		
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
        public override string EditModeButtonName { get { return basemode.GetType().Name; } }

		internal EditMode BaseMode { get { return basemode; } }
		
		#endregion

		#region ================== Constructor / Disposer

		// Constructor to start dragging immediately
		public DuplicateThingsMode(EditMode basemode, Vector2D dragstartmappos)
		{
			// Initialize
			this.dragstartmappos = dragstartmappos;
			this.basemode = basemode;

			Cursor.Current = Cursors.AppStarting;

            // Make undo for the dragging
            General.Map.UndoRedo.CreateUndo("Duplicate things");
            
            // Mark what we are dragging
            General.Map.Map.ClearAllMarks(false);
			General.Map.Map.MarkSelectedThings(true, true);

            // Get selected things
            List<Thing> oldthings = General.Map.Map.GetMarkedThings(true);

            selectedthings = new List<Thing>(oldthings.Count);
            
            foreach (Thing t in oldthings)
            {
                Thing newt = General.Map.Map.CreateThing();
                t.CopyPropertiesTo(newt);
                t.Selected = false;
                t.Marked = false;
                selectedthings.Add(newt);
                newt.Marked = true;
                newt.Marked = true;
            }

			unselectedthings = new List<Thing>();
			foreach(Thing t in General.Map.ThingsFilter.VisibleThings) if(!t.Marked) unselectedthings.Add(t);
			
			// Get the nearest thing for snapping
			dragitem = MapSet.NearestThing(selectedthings, dragstartmappos);

			// Make old positions list
			// We will use this as reference to move the vertices, or to move them back on cancel
			oldpositions = new List<Vector2D>(selectedthings.Count);
			foreach(Thing t in selectedthings) oldpositions.Add(t.Position);

			// Also keep old position of the dragged item
			dragitemposition = dragitem.Position;

			// Keep view information
			lastoffsetx = renderer.OffsetX;
			lastoffsety = renderer.OffsetY;
			lastscale = renderer.Scale;
			
			Cursor.Current = Cursors.Default;
			
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
		
		// This moves the selected things relatively
		// Returns true when things has actually moved
		private bool MoveThingsRelative(Vector2D offset, bool snapgrid, bool snapnearest)
		{
			Vector2D oldpos = dragitem.Position;
			Thing nearest;
			Vector2D tl, br;

			// don't move if the offset contains invalid data
			if (!offset.IsFinite())	return false;

			// Find the outmost things
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
				// Find nearest unselected item within selection range
				nearest = MapSet.NearestThingSquareRange(unselectedthings, mousemappos, BuilderPlug.Me.StitchRange / renderer.Scale);
				if(nearest != null)
				{
					// Move the dragged item
					dragitem.Move((Vector2D)nearest.Position);

					// Adjust the offset
					offset = (Vector2D)nearest.Position - dragitemposition;

					// Do not snap to grid!
					snapgrid = false;
				}
			}

			// Snap to grid?
			if(snapgrid)
			{
				// Move the dragged item
				dragitem.Move(dragitemposition + offset);

				// Snap item to grid
				dragitem.SnapToGrid();

				// Adjust the offset
				offset += (Vector2D)dragitem.Position - (dragitemposition + offset);
			}

			// Make sure the offset is inside the map boundaries
			if (offset.x + tl.x < General.Map.Config.LeftBoundary) offset.x = General.Map.Config.LeftBoundary - tl.x;
			if (offset.x + br.x > General.Map.Config.RightBoundary) offset.x = General.Map.Config.RightBoundary - br.x;
			if (offset.y + tl.y > General.Map.Config.TopBoundary) offset.y = General.Map.Config.TopBoundary - tl.y;
			if (offset.y + br.y < General.Map.Config.BottomBoundary) offset.y = General.Map.Config.BottomBoundary - br.y;

			// Drag item moved?
			if(!snapgrid || ((Vector2D)dragitem.Position != oldpos))
			{
				int i = 0;

				// Move selected geometry
				foreach(Thing t in selectedthings)
				{
					// Move vertex from old position relative to the
					// mouse position change since drag start
					t.Move(oldpositions[i] + offset);

					// Next
					i++;
				}

				// Moved
				return true;
			}
			else
			{
				// No changes
				return false;
			}
		}

		// This redraws the display
		public override void OnRedrawDisplay()
		{
			bool viewchanged = CheckViewChanged();

			if(viewchanged)
			{
				renderer.RedrawSurface();

				// Render lines and vertices
				if(renderer.StartPlotter(true))
				{
					renderer.PlotLinedefSet(General.Map.Map.Linedefs);
					renderer.PlotVerticesSet(General.Map.Map.Vertices);
					renderer.Finish();
				}
			}
			
			// Render things
			UpdateRedraw();

			renderer.Present();
		}

		// This redraws only changed things
		private void UpdateRedraw()
		{
			// Render things
			if(renderer.StartThings(true))
			{
				// Render things
				renderer.RenderThingSet(General.Map.ThingsFilter.HiddenThings, Presentation.THINGS_HIDDEN_ALPHA);
				renderer.RenderThingSet(unselectedthings, 1.0f);
				renderer.RenderThingSet(selectedthings, 1.0f);

				// Draw the dragged item highlighted
				// This is important to know, because this item is used
				// for snapping to the grid and snapping to nearest items
				renderer.RenderThing(dragitem, General.Colors.Highlight, 1.0f);

				// Done
				renderer.Finish();
			}
		}
		
		// Cancelled
		public override void OnCancel()
		{
            General.Map.UndoRedo.WithdrawUndo();

            // Cancel base class
            base.OnCancel();
			
			// Return to vertices mode
			General.Editing.ChangeMode(basemode);
		}

		// Mode engages
		public override void OnEngage()
		{
			base.OnEngage();
			renderer.SetPresentation(Presentation.Things);
		}
		
		// Disenagaging
		public override void OnDisengage()
		{
			base.OnDisengage();
			Cursor.Current = Cursors.AppStarting;

            // When not cancelled
            if (!cancelled)
            {
                // Move geometry back to original position
                MoveThingsRelative(new Vector2D(0f, 0f), false, false);

                // Move selected geometry to final position
                MoveThingsRelative(mousemappos - dragstartmappos, snaptogrid, snaptonearest);

                // Snap to map format accuracy
                General.Map.Map.SnapAllToAccuracy();

                // Update cached values
                General.Map.Map.Update(false, false);

                // Map is changed
                General.Map.IsChanged = true;

                General.Map.Map.ClearSelectedThings();
                General.Map.Map.ClearMarkedThings(false);

                General.Map.ThingsFilter.Update();

                General.Interface.RedrawDisplay();
            }

			// Hide highlight info
			General.Interface.HideInfo();

			// Done
			Cursor.Current = Cursors.Default;
		}

		// This checks if the view offset/zoom changed and updates the check
		private bool CheckViewChanged()
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
			snaptonearest = General.Interface.CtrlState;
			
			// Move selected geometry
			if(MoveThingsRelative(mousemappos - dragstartmappos, snaptogrid, snaptonearest))
			{
				// Update cached values
				//General.Map.Map.Update(true, false);
				General.Map.Map.Update();

				// Redraw
				UpdateRedraw();
				renderer.Present();
				//General.Interface.RedrawDisplay();
			}
		}

        // When edit button is released
        protected override void OnEditEnd()
		{
			// Just return to vertices mode, geometry will be merged on disengage.
			General.Editing.ChangeMode(basemode);

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
			if(snaptogrid != General.Interface.ShiftState ^ General.Interface.SnapToGrid) Update();
			if(snaptonearest != General.Interface.CtrlState) Update();
		}

		// When a key is pressed
		public override void OnKeyDown(KeyEventArgs e)
		{
			base.OnKeyDown(e);
			if(snaptogrid != General.Interface.ShiftState ^ General.Interface.SnapToGrid) Update();
			if(snaptonearest != General.Interface.CtrlState) Update();
		}
		
		#endregion
    }
}
