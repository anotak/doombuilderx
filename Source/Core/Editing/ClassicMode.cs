
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
using CodeImp.DoomBuilder.Actions;
using CodeImp.DoomBuilder.Geometry;
using System.Drawing;

#endregion

namespace CodeImp.DoomBuilder.Editing
{
	/// <summary>
	/// Provides specialized functionality for a classic (2D) Doom Builder editing mode.
	/// </summary>
	public abstract class ClassicMode : EditMode
	{
		#region ================== Constants

		private const float SCALE_MAX = 20f;
		private const float SCALE_MIN = 0.01f;
		private const float SELECTION_BORDER_SIZE = 2f;
		private const int SELECTION_ALPHA = 200;
		private const float CENTER_VIEW_PADDING = 0.06f;
		private const float AUTOPAN_BORDER_SIZE = 100.0f;

		#endregion

		#region ================== Variables

		// Cancelled?
		protected bool cancelled;

		// Graphics
		protected IRenderer2D renderer;
		private Renderer2D renderer2d;
		
		// Mouse status
		protected Vector2D mousepos;
        protected Vector2D mouselastpos;
		protected Vector2D mousemappos;
		protected Vector2D mousedownpos;
		protected Vector2D mousedownmappos;
		protected MouseButtons mousebuttons;
		protected bool mouseinside;
		protected MouseButtons mousedragging = MouseButtons.None;
		
		// Selection
		protected bool selecting;
		private Vector2D selectstart;
		protected RectangleF selectionrect;

        // View panning
        protected bool panning;
		private bool autopanenabled;
		
		#endregion

		#region ================== Properties
		
		// Mouse status
		public Vector2D MousePos { get { return mousepos; } }
		public Vector2D MouseLastPos { get { return mouselastpos; } }
		public Vector2D MouseMapPos { get { return mousemappos; } }
		public Vector2D MouseDownPos { get { return mousedownpos; } }
		public Vector2D MouseDownMapPos { get { return mousedownmappos; } }
		public MouseButtons MouseButtons { get { return mousebuttons; } }
		public bool IsMouseInside { get { return mouseinside; } }
		public MouseButtons MouseDragging { get { return mousedragging; } }

		// Selection
		public bool IsSelecting { get { return selecting; } }
		public Vector2D SelectionStart { get { return selectstart; } }
		public RectangleF SelectionRect { get { return selectionrect; } }
		
		// Panning
		public bool IsPanning { get { return panning; } }

		// Rendering
		public IRenderer2D Renderer { get { return renderer; } }

		#endregion

		#region ================== Constructor / Disposer

		/// <summary>
		/// Provides specialized functionality for a classic (2D) Doom Builder editing mode.
		/// </summary>
		public ClassicMode()
		{
			// Initialize
			this.renderer = General.Map.Renderer2D;
			this.renderer2d = (Renderer2D)General.Map.Renderer2D;

			// If the current mode is a ClassicMode, copy mouse properties
			if(General.Editing.Mode is ClassicMode)
			{
				ClassicMode oldmode = General.Editing.Mode as ClassicMode;

				// Copy mouse properties
				mousepos = oldmode.mousepos;
				mousemappos = oldmode.mousemappos;
				mousedownpos = oldmode.mousedownpos;
				mousedownmappos = oldmode.mousedownmappos;
				mousebuttons = oldmode.mousebuttons;
				mouseinside = oldmode.mouseinside;
				mousedragging = oldmode.mousedragging;
			}
		}

		// Disposer
		public override void Dispose()
		{
			// Not already disposed?
			if(!isdisposed)
			{
				// Clean up

				// Dispose base
				base.Dispose();
			}
		}

		#endregion

		#region ================== Scroll / Zoom

		// This scrolls the view north
		[BeginAction("scrollnorth", BaseAction = true)]
		public virtual void ScrollNorth()
		{
			// Scroll
			ScrollBy(0f, 100f / renderer2d.Scale);
		}

		// This scrolls the view south
		[BeginAction("scrollsouth", BaseAction = true)]
		public virtual void ScrollSouth()
		{
			// Scroll
			ScrollBy(0f, -100f / renderer2d.Scale);
		}

		// This scrolls the view west
		[BeginAction("scrollwest", BaseAction = true)]
		public virtual void ScrollWest()
		{
			// Scroll
			ScrollBy(-100f / renderer2d.Scale, 0f);
		}

		// This scrolls the view east
		[BeginAction("scrolleast", BaseAction = true)]
		public virtual void ScrollEast()
		{
			// Scroll
			ScrollBy(100f / renderer2d.Scale, 0f);
		}

		// This zooms in
		[BeginAction("zoomin", BaseAction = true)]
		public virtual void ZoomIn()
		{
			float z = 1.0f + General.Settings.ZoomFactor * 0.1f;

			// Zoom
			ZoomBy(z);
		}

		// This zooms out
		[BeginAction("zoomout", BaseAction = true)]
		public virtual void ZoomOut()
		{
			float z = 1.0f + General.Settings.ZoomFactor * 0.1f;

			// Zoom
			ZoomBy(1.0f / z);
		}

		// This scrolls anywhere
		private void ScrollBy(float deltax, float deltay)
		{
			// Scroll now
			renderer2d.PositionView(renderer2d.OffsetX + deltax, renderer2d.OffsetY + deltay);
			this.OnViewChanged();
			
			// Redraw
			General.MainWindow.RedrawDisplay();

			// Determine new unprojected mouse coordinates
			mousemappos = renderer2d.DisplayToMap(mousepos);
			General.MainWindow.UpdateCoordinates(mousemappos);
		}

        // This sets the view to be centered at x,y
        private void ScrollTo(float x, float y)
        {
            // Scroll now
            renderer2d.PositionView(x, y);
            this.OnViewChanged();

            // Redraw
            General.MainWindow.RedrawDisplay();

            // Determine new unprojected mouse coordinates
            mousemappos = renderer2d.DisplayToMap(mousepos);
            General.MainWindow.UpdateCoordinates(mousemappos);
        }

		// This zooms
		private void ZoomBy(float deltaz)
		{
			Vector2D zoompos, clientsize, diff;
			float newscale;
			
			// This will be the new zoom scale
			newscale = renderer2d.Scale * deltaz;

			// Limit scale
			if(newscale > SCALE_MAX) newscale = SCALE_MAX;
			if(newscale < SCALE_MIN) newscale = SCALE_MIN;
			
			// Get the dimensions of the display
			clientsize = new Vector2D(General.Map.Graphics.RenderTarget.ClientSize.Width,
									  General.Map.Graphics.RenderTarget.ClientSize.Height);
			
			// When mouse is inside display
			if(mouseinside)
			{
				// Zoom into or from mouse position
				zoompos = (mousepos / clientsize) - new Vector2D(0.5f, 0.5f);
			}
			else
			{
				// Zoom into or from center
				zoompos = new Vector2D(0f, 0f);
			}

			// Calculate view position difference
			diff = ((clientsize / newscale) - (clientsize / renderer2d.Scale)) * zoompos;

			// Zoom now
			renderer2d.PositionView(renderer2d.OffsetX - diff.x, renderer2d.OffsetY + diff.y);
			renderer2d.ScaleView(newscale);
			this.OnViewChanged();

			// Redraw
			//General.Map.Map.Update();
			General.MainWindow.RedrawDisplay();
			
			// Give a new mousemove event to update coordinates
			if(mouseinside) OnMouseMove(new MouseEventArgs(mousebuttons, 0, (int)mousepos.x, (int)mousepos.y, 0));
		}

		// This zooms to a specific level
		public void SetZoom(float newscale)
		{
			// Zoom now
			renderer2d.ScaleView(newscale);
			this.OnViewChanged();

			// Redraw
			//General.Map.Map.Update();
			General.MainWindow.RedrawDisplay();

			// Give a new mousemove event to update coordinates
			if(mouseinside) OnMouseMove(new MouseEventArgs(mousebuttons, 0, (int)mousepos.x, (int)mousepos.y, 0));
		}
		
		// This zooms and scrolls to fit the map in the window
		[BeginAction("centerinscreen", BaseAction = true)]
		public void CenterInScreen()
		{
			float left = float.MaxValue;
			float top = float.MaxValue;
			float right = float.MinValue;
			float bottom = float.MinValue;
			bool anything = false;
			
			// Go for all vertices
			foreach(Vertex v in General.Map.Map.Vertices)
			{
				// Vertex used?
				if(v.Linedefs.Count > 0)
				{
					// Adjust boundaries by vertices
					if(v.Position.x < left) left = v.Position.x;
					if(v.Position.x > right) right = v.Position.x;
					if(v.Position.y < top) top = v.Position.y;
					if(v.Position.y > bottom) bottom = v.Position.y;
					anything = true;
				}
			}

			// Not already found something to center in view?
			if(!anything)
			{
				// Go for all things
				foreach(Thing t in General.Map.Map.Things)
				{
					// Adjust boundaries by vertices
					if(t.Position.x < left) left = t.Position.x;
					if(t.Position.x > right) right = t.Position.x;
					if(t.Position.y < top) top = t.Position.y;
					if(t.Position.y > bottom) bottom = t.Position.y;
					anything = true;
				}
			}
			
			// Anything found to center in view?
			if(anything)
			{
				RectangleF area = new RectangleF(left, top, (right - left), (bottom - top));
				CenterOnArea(area, CENTER_VIEW_PADDING);
			}
			else
			{
				// Default view
				SetDefaultZoom();
			}
		}
		
		// This zooms and moves to view the given area
		public void CenterOnArea(RectangleF area, float padding)
		{
			float scalew, scaleh, scale;
			
			// Add size to the area for better overview
			area.Inflate(area.Width * padding, area.Height * padding);
			
			// Calculate scale to view map at
			scalew = (float)General.Map.Graphics.RenderTarget.ClientSize.Width / area.Width;
			scaleh = (float)General.Map.Graphics.RenderTarget.ClientSize.Height / area.Height;
			if(scalew < scaleh) scale = scalew; else scale = scaleh;
			
			// Change the view to see the whole map
			renderer2d.ScaleView(scale);
			renderer2d.PositionView(area.Left + area.Width * 0.5f, area.Top + area.Height * 0.5f);
			this.OnViewChanged();
			
			// Redraw
			General.MainWindow.RedrawDisplay();
			
			// Give a new mousemove event to update coordinates
			if(mouseinside) OnMouseMove(new MouseEventArgs(mousebuttons, 0, (int)mousepos.x, (int)mousepos.y, 0));
		}
		
		// This sets up the default view
		public void SetDefaultZoom()
		{
			// View middle of map at 50% zoom
			renderer2d.ScaleView(0.5f);
			renderer2d.PositionView(0.0f, 0.0f);
			this.OnViewChanged();
			General.MainWindow.RedrawDisplay();

			// Give a new mousemove event to update coordinates
			if(mouseinside) OnMouseMove(new MouseEventArgs(mousebuttons, 0, (int)mousepos.x, (int)mousepos.y, 0));
		}
		
		/// <summary>
		/// This is called when the view changes (scroll/zoom), before the display is redrawn.
		/// </summary>
		protected virtual void OnViewChanged()
		{
		}
		
		// This enabled automatic panning, if preferred
		protected void EnableAutoPanning()
		{
			if(General.Settings.AutoScrollSpeed > 0)
			{
				if(!autopanenabled)
				{
					autopanenabled = true;
					General.MainWindow.EnableProcessing();
				}
			}
		}

		// This disabls automatic panning
		protected void DisableAutoPanning()
		{
			if(autopanenabled)
			{
				autopanenabled = false;
				General.MainWindow.DisableProcessing();
			}
		}
		
		#endregion

		#region ================== Processing

		// Processing
		public override void OnProcess(long deltatime)
		{
			base.OnProcess(deltatime);

			if(autopanenabled)
			{
				Vector2D panamount = new Vector2D();
				
				// How much to pan the view in X?
				if(mousepos.x < AUTOPAN_BORDER_SIZE)
					panamount.x = -AUTOPAN_BORDER_SIZE + mousepos.x;
				else if(mousepos.x > (General.MainWindow.Display.ClientSize.Width - AUTOPAN_BORDER_SIZE))
					panamount.x = mousepos.x - (General.MainWindow.Display.ClientSize.Width - AUTOPAN_BORDER_SIZE);

				// How much to pan the view in Y?
				if(mousepos.y < AUTOPAN_BORDER_SIZE)
					panamount.y = AUTOPAN_BORDER_SIZE - mousepos.y;
				else if(mousepos.y > (General.MainWindow.Display.ClientSize.Height - AUTOPAN_BORDER_SIZE))
					panamount.y = -(mousepos.y - (General.MainWindow.Display.ClientSize.Height - AUTOPAN_BORDER_SIZE));

				// Do any panning?
				if(panamount.GetManhattanLength() > 0.0f)
				{
					// Scale and power this for nicer usability
					Vector2D pansign = panamount.GetSign();
					panamount = (panamount * panamount) * pansign * 0.0001f * (float)General.Settings.AutoScrollSpeed / renderer.Scale;
					
					// Multiply by delta time
					panamount.x = (float)((double)panamount.x * deltatime);
					panamount.y = (float)((double)panamount.y * deltatime);

					// Pan the view
					ScrollBy(panamount.x, panamount.y);
				}
			}
		}

		#endregion

		#region ================== Input

		// Mouse leaves the display
		public override void OnMouseLeave(EventArgs e)
		{
			// Mouse is outside the display
			mouseinside = false;
			mousepos = new Vector2D(float.NaN, float.NaN);
			mousemappos = mousepos;
			mousebuttons = MouseButtons.None;
			
			// Determine new unprojected mouse coordinates
			General.MainWindow.UpdateCoordinates(mousemappos);
			
			// Let the base class know
			base.OnMouseLeave(e);
		}

		// Mouse moved inside the display
		public override void OnMouseMove(MouseEventArgs e)
		{
			Vector2D delta;

			// Record last position
			mouseinside = true;
            mouselastpos = mousepos;
			mousepos = new Vector2D(e.X, e.Y);
			mousemappos = renderer2d.DisplayToMap(mousepos);
			mousebuttons = e.Button;
			
			// Update labels in main window
			General.MainWindow.UpdateCoordinates(mousemappos);
			
			// Holding a button?
			if(e.Button != MouseButtons.None)
			{
				// Not dragging?
				if(mousedragging == MouseButtons.None)
				{
					// Check if moved enough pixels for dragging
					delta = mousedownpos - mousepos;
					if((Math.Abs(delta.x) > DRAG_START_MOVE_PIXELS) ||
					   (Math.Abs(delta.y) > DRAG_START_MOVE_PIXELS))
					{
						// Dragging starts now
						mousedragging = e.Button;
						OnDragStart(e);
					}
				}
			}
			
			// Selecting?
			if(selecting) OnUpdateMultiSelection();

            // Panning?
            if (panning) OnUpdateViewPanning();
			
			// Let the base class know
			base.OnMouseMove(e);
		}

		// Mouse button pressed
		public override void OnMouseDown(MouseEventArgs e)
		{
			// Save mouse down position
			mousedownpos = mousepos;
			mousedownmappos = mousemappos;
			
			// Let the base class know
			base.OnMouseDown(e);
		}

		// Mouse button released
		public override void OnMouseUp(MouseEventArgs e)
		{
			// Releasing drag button?
			if(e.Button == mousedragging)
			{
				// No longer dragging
				OnDragStop(e);
				mousedragging = MouseButtons.None;
			}
			
			// Let the base class know
			base.OnMouseUp(e);
		}

		/// <summary>
		/// Automatically called when dragging operation starts.
		/// </summary>
		protected virtual void OnDragStart(MouseEventArgs e)
		{
		}

		/// <summary>
		/// Automatically called when dragging operation stops.
		/// </summary>
		protected virtual void OnDragStop(MouseEventArgs e)
		{
		}
		
		#endregion

		#region ================== Display

		// This just refreshes the display
		public override void OnPresentDisplay()
		{
			renderer2d.Present();
		}

		// This sets the view mode
		private void SetViewMode(ViewMode mode)
		{
			General.Map.CRenderer2D.SetViewMode(mode);
			General.MainWindow.UpdateInterface();
			General.MainWindow.RedrawDisplay();
		}
		
		#endregion

		#region ================== Methods

		/// <summary>
		/// Automatically called by the core when this editing mode is engaged.
		/// </summary>
		public override void OnEngage()
		{
			// Clear display overlay
			renderer.StartOverlay(true);
			renderer.Finish();
			base.OnEngage();
		}

		/// <summary>
		/// Called when the user requests to cancel this editing mode.
		/// </summary>
		public override void OnCancel()
		{
			cancelled = true;
			base.OnCancel();
		}

		/// <summary>
		/// This is called automatically when the Edit button is pressed.
		/// (in Doom Builder 1, this was always the right mousebutton)
		/// </summary>
		[BeginAction("classicedit", BaseAction = true)]
		protected virtual void OnEditBegin()
		{
		}

		/// <summary>
		/// This is called automatically when the Edit button is released.
		/// (in Doom Builder 1, this was always the right mousebutton)
		/// </summary>
		[EndAction("classicedit", BaseAction = true)]
		protected virtual void OnEditEnd()
		{
		}

		/// <summary>
		/// This is called automatically when the Select button is pressed.
		/// (in Doom Builder 1, this was always the left mousebutton)
		/// </summary>
		[BeginAction("classicselect", BaseAction = true)]
		protected virtual void OnSelectBegin()
		{
		}

		/// <summary>
		/// This is called automatically when the Select button is released.
		/// (in Doom Builder 1, this was always the left mousebutton)
		/// </summary>
		[EndAction("classicselect", BaseAction = true)]
		protected virtual void OnSelectEnd()
		{
			if(selecting) OnEndMultiSelection();
		}

		/// <summary>
		/// This is called automatically when a rectangular multi-selection ends.
		/// </summary>
		protected virtual void OnEndMultiSelection()
		{
			selecting = false;
		}

		/// <summary>
		/// Call this to initiate a rectangular multi-selection.
		/// </summary>
		protected virtual void StartMultiSelection()
		{
			selecting = true;
			selectstart = mousemappos;
			selectionrect = new RectangleF(selectstart.x, selectstart.y, 0, 0);
		}

		/// <summary>
		/// This is called automatically when a multi-selection is updated.
		/// </summary>
		protected virtual void OnUpdateMultiSelection()
		{
			selectionrect.X = selectstart.x;
			selectionrect.Y = selectstart.y;
			selectionrect.Width = mousemappos.x - selectstart.x;
			selectionrect.Height = mousemappos.y - selectstart.y;
			
			if(selectionrect.Width < 0f)
			{
				selectionrect.Width = -selectionrect.Width;
				selectionrect.X -= selectionrect.Width;
			}
			
			if(selectionrect.Height < 0f)
			{
				selectionrect.Height = -selectionrect.Height;
				selectionrect.Y -= selectionrect.Height;
			}
		}

		/// <summary>
		/// Call this to draw the selection on the overlay layer.
		/// Must call renderer.StartOverlay first!
		/// </summary>
		protected virtual void RenderMultiSelection()
		{
			renderer.RenderRectangle(selectionrect, SELECTION_BORDER_SIZE,
				General.Colors.Highlight.WithAlpha(SELECTION_ALPHA), true);
		}

        /// <summary>
        /// This is called automatically when the mouse is moved while panning
        /// </summary>
        protected virtual void OnUpdateViewPanning()
        {
			// We can only drag the map when the mouse pointer is inside
			// otherwise we don't have coordinates where to drag the map to
			if(mouseinside && !float.IsNaN(mouselastpos.x) && !float.IsNaN(mouselastpos.y))
			{
				// Get the map coordinates of the last mouse posision (before it moved)
				Vector2D lastmappos = renderer2d.DisplayToMap(mouselastpos);
				
				// Do the scroll
				ScrollBy(lastmappos.x - mousemappos.x, lastmappos.y - mousemappos.y);
			}
        }
		
		#endregion
		
		#region ================== Actions

		[BeginAction("gridsetup", BaseAction = true)]
		protected void ShowGridSetup()
		{
			General.Map.Grid.ShowGridSetup();
		}
		
        [BeginAction("pan_view", BaseAction = true)]
        protected virtual void BeginViewPan()
        {
            panning = true;
        }

        [EndAction("pan_view", BaseAction = true)]
        protected virtual void EndViewPan()
        {
            panning = false;
        }

		[BeginAction("viewmodenormal", BaseAction = true)]
		protected virtual void ViewModeNormal()
		{
			SetViewMode(ViewMode.Normal);
		}

		[BeginAction("viewmodebrightness", BaseAction = true)]
		protected virtual void ViewModeBrightness()
		{
			SetViewMode(ViewMode.Brightness);
		}

		[BeginAction("viewmodefloors", BaseAction = true)]
		protected virtual void ViewModeFloors()
		{
			SetViewMode(ViewMode.FloorTextures);
		}

		[BeginAction("viewmodeceilings", BaseAction = true)]
		protected virtual void ViewModeCeilings()
		{
			SetViewMode(ViewMode.CeilingTextures);
		}

		#endregion
	}
}
