
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
using CodeImp.DoomBuilder.Types;
using CodeImp.DoomBuilder.Config;

#endregion

namespace CodeImp.DoomBuilder.BuilderModes
{
	public abstract class BaseClassicMode : ClassicMode
	{
		#region ================== Constants

		#endregion

		#region ================== Variables

		#endregion

		#region ================== Properties

		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		public BaseClassicMode()
		{
			// Initialize

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

		// This occurs when the user presses Copy. All selected geometry must be marked for copying!
		public override bool OnCopyBegin()
		{
			General.Map.Map.MarkAllSelectedGeometry(true, false, true, true, false);

			// Return true when anything is selected so that the copy continues
			// We only have to check vertices for the geometry, because without selected
			// vertices, no complete structure can exist.
			return (General.Map.Map.GetMarkedVertices(true).Count > 0) ||
				   (General.Map.Map.GetMarkedThings(true).Count > 0);
		}
		
		// This is called when pasting begins
		public override bool OnPasteBegin(PasteOptions options)
		{
			// These modes support pasting
			return true;
		}
		
		// This is called when something was pasted.
		public override void OnPasteEnd(PasteOptions options)
		{
			General.Map.Map.ClearAllSelected();
			General.Map.Map.SelectMarkedGeometry(true, true);
			
			// Switch to EditSelectionMode
			EditSelectionMode editmode = new EditSelectionMode();
			editmode.Pasting = true;
			editmode.PasteOptions = options;
			General.Editing.ChangeMode(editmode);
		}

		// Double-clicking
		public override void OnMouseDoubleClick(MouseEventArgs e)
		{
			base.OnMouseDoubleClick(e);

			int k = 0;
			if(e.Button == MouseButtons.Left) k = (int)Keys.LButton;
			if(e.Button == MouseButtons.Middle) k = (int)Keys.MButton;
			if(e.Button == MouseButtons.Right) k = (int)Keys.RButton;
			if(e.Button == MouseButtons.XButton1) k = (int)Keys.XButton1;
			if(e.Button == MouseButtons.XButton2) k = (int)Keys.XButton2;
			
			// Double select-click? Make that the same as single edit-click
			if(General.Actions.GetActionByName("builder_classicselect").KeyMatches(k))
			{
				Actions.Action a = General.Actions.GetActionByName("builder_classicedit");
				if(a != null) a.Invoke();
			}
		}
		
		#endregion

		#region ================== Actions

		[BeginAction("placevisualstart")]
		public void PlaceVisualStartThing()
		{
			Thing thingfound = null;
			
			// Not during volatile mode
			if(this.Attributes.Volatile) return;
			
			// Mouse must be inside window
			if(!mouseinside) return;
			
			General.Interface.DisplayStatus(StatusType.Action, "Placed Visual Mode camera start thing.");
			
			// Go for all things
			List<Thing> things = new List<Thing>(General.Map.Map.Things);
			foreach(Thing t in things)
			{
				if(t.Type == General.Map.Config.Start3DModeThingType)
				{
					if(thingfound == null)
					{
						// Move this thing
						t.Move(mousemappos);
						thingfound = t;
					}
					else
					{
						// One was already found and moved, delete this one
						t.Dispose();
					}
				}
			}
			
			// No thing found?
			if(thingfound == null)
			{
				// Make a new one
				Thing t = General.Map.Map.CreateThing();
				if(t != null)
				{
					t.Type = General.Map.Config.Start3DModeThingType;
					t.Move(mousemappos);
					t.UpdateConfiguration();
					General.Map.ThingsFilter.Update();
					thingfound = t;
				}
			}

			if(thingfound != null)
			{
				// Make sure that the found thing is between ceiling and floor
				thingfound.DetermineSector();
				if(thingfound.Position.z < 0.0f) thingfound.Move(thingfound.Position.x, thingfound.Position.y, 0.0f);
				if(thingfound.Sector != null)
				{
					if((thingfound.Position.z + 50.0f) > (thingfound.Sector.CeilHeight - thingfound.Sector.FloorHeight))
						thingfound.Move(thingfound.Position.x, thingfound.Position.y,
							thingfound.Sector.CeilHeight - thingfound.Sector.FloorHeight - 50.0f);
				}
			}
			
			// Update Visual Mode camera
			General.Map.VisualCamera.PositionAtThing();
			
			// Redraw display to show changes
			General.Interface.RedrawDisplay();
		}

		#endregion
	}
}
