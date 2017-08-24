
#region ================== Copyright (c) 2009 Pascal vd Heiden

/*
 * Copyright (c) 2009 Pascal vd Heiden, www.codeimp.com
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

namespace CodeImp.DoomBuilder.Statistics
{
	//
	// This class defines an classic editing mode. You can also inherit from VisualMode for a
	// visual editing mode or inherit from EditMode if you feel creative. The ClassicMode
	// provides basic 2D editing mode features such as zooming in/out and panning the view as
	// well as a bunch of usefull events.
	//
	
	//
	// This EditMode attribute allows you to link the editing mode into Doom Builder. The core
	// only sees your editing mode with this attribute and uses the settings to register it.
	//
	// DisplayName: This is the name you want to display to the user for this mode.
	//
	// SwitchAction: This is the action (see Actions.cfg) that engages this mode.
	//
	// ButtonImage: If set, creates a button on the toolbar for this mode and uses this image.
	//              Note that the image is included in this project as "Embedded Resource".
	//
	// ButtonOrder: Ordering number for the position of the button on the toolbar.
	//
	// ButtonGroup: Group in which to put the button on the toolbar.
	//              (Groups are buttons that are grouped together between separators)
	//
	// UseByDefault: THIS OPTION MAY BE INTRUSIVE TO THE USER, USE WITH GREAT CARE!
	//               Set this to enable this editing mode for use in all game configurations
	//               by default. This only applies the first time a plugin is loaded and can
	//               still be changed by the user. But it may not be desired and may conflict
	//               with other editing modes.
	//
	// Volatile: Set this to make this mode a volatile mode. When volatile, the mode is
	//           automatically cancelled when certain major actions are performed (such as
	//           when the map is saved)
	//

	[EditMode(DisplayName = "Statistics",
			  SwitchAction = "statisticsmode",
			  ButtonImage = "StatisticsIcon.png",
			  ButtonOrder = 300,
			  ButtonGroup = "002_tools",
			  UseByDefault = true,
			  AllowCopyPaste = false,
			  Volatile = true)]
	
	public class StatisticsMode : ClassicMode
	{
		
		//
		// Order in which events occur when switching editing modes:
		// 
		// - Constructor of new mode is called
		// - OnDisengage() of old mode is called
		// ----- Mode switches -----
		// - OnEngage() of new mode is called
		// - Dispose() of old mode is called
		//
		// This function is called when this editing mode is engaged
		public override void OnEngage()
		{
			base.OnEngage();

			// This tells the renderer how to display the map.
			// The renderer works with several different layers, each with its own purpose
			// and features. A "presentation" defines how to combine these layers when
			// presented to the user on the display. You can make your own presentation
			// using the CustomPresentation class, but the standard one is good enough for
			// what we need here.
			renderer.SetPresentation(Presentation.Standard);
			
			// We now want to show our interface window.
			// General.Interface is the main Doom Builder window. We want to use
			// it as "owner" so that our window stays on top of the main window.
			BuilderPlug.Me.StatsForm.ShowWindow((Form)General.Interface);
		}
		
		// This function is called when this editing mode is disengaged
		public override void OnDisengage()
		{
			base.OnDisengage();

			// Hide the window
			BuilderPlug.Me.StatsForm.Hide();
		}

		// Event called when the user (or the core) wants to cancel this editing mode
		public override void OnCancel()
		{
			base.OnCancel();

			// Return to previous stable mode
			General.Editing.ChangeMode(General.Editing.PreviousStableMode.Name);
		}
		
		// MANDATORY: You must override this event and handle it to draw the map
		public override void OnRedrawDisplay()
		{
			base.OnRedrawDisplay();

			// This redraws the surface layer of a presentation. Depending on the
			// chosen view mode, it contains the floor or ceiling textures of the
			// sectors, or the light levels of the sectors. If your presentation
			// does not include the Suface layer, you do not need to call this.
			// Calling renderer.Finish() is never needed for this.
			renderer.RedrawSurface();

			// Here we begin drawing on the Geometry layer (which can only draw lines
			// and dots, hence why the renderer is called a 'plotter')
			// StartPlotter() returns true when drawing is possible.
			if(renderer.StartPlotter(true))
			{
				// We simply draw all the linedefs and then we draw all the vertices on top.
				renderer.PlotLinedefSet(General.Map.Map.Linedefs);
				renderer.PlotVerticesSet(General.Map.Map.Vertices);

				// This must be called to finish the rendering.
				renderer.Finish();
			}

			// Here we begin drawing on the Things layer (which can only display things)
			// StartThings returns true when drawing is possible.
			if(renderer.StartThings(true))
			{
				// We draw the "visible" things opaque and the "invisible" things with the default translucency.
				// Visible and invisible things are controlled by the Things Filter in the core.
				renderer.RenderThingSet(General.Map.ThingsFilter.HiddenThings, Presentation.THINGS_HIDDEN_ALPHA);
				renderer.RenderThingSet(General.Map.ThingsFilter.VisibleThings, 1.0f);

				// This must be called to finish the rendering.
				renderer.Finish();
			}

			// We don't need the overlay at all, so we just clear it here. An optimisation
			// could be to write a custom presentation that excludes the Overlay layer so
			// we don't need to clear it, but let's keep it simple for now.
			if(renderer.StartOverlay(true))
			{
				// You know this must be called to finish the rendering, right?
				renderer.Finish();
			}
			
			// All layers we need have been updated. We now present it to the user
			// on the display, with the previously defined presentation settings.
			renderer.Present();
		}
	}
}
