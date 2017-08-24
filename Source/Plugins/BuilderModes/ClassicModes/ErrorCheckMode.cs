
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
	[EditMode(DisplayName = "Map Analysis Mode",
			  SwitchAction = "errorcheckmode",
			  ButtonImage = "MapAnalysisMode.png",
			  ButtonOrder = 200,
			  ButtonGroup = "002_tools",
			  AllowCopyPaste = false,
			  Volatile = true,
			  UseByDefault = true)]

	public sealed class ErrorCheckMode : BaseClassicMode
	{
		#region ================== Constants

		#endregion

		#region ================== Variables

		#endregion

		#region ================== Properties

		#endregion

		#region ================== Constructor / Disposer

		#endregion

		#region ================== Events

		public override void OnHelp()
		{
			General.ShowHelp("e_mapanalysis.html");
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

			// Save selection as marks
			General.Map.Map.ClearAllMarks(false);
			General.Map.Map.MarkAllSelectedGeometry(true, false, false, false, false);
			General.Map.Map.ClearAllSelected();
			General.Map.Map.SelectionType = SelectionType.All;
			
			// Show toolbox window
			BuilderPlug.Me.ErrorCheckForm.Show((Form)General.Interface);
		}

		// Disenagaging
		public override void OnDisengage()
		{
			base.OnDisengage();

			// Hide object info
			General.Interface.HideInfo();
			
			// Restore selection
			General.Map.Map.SelectMarkedGeometry(true, true);
			General.Map.Map.ClearAllMarks(false);
			
			// Hide toolbox window
			BuilderPlug.Me.ErrorCheckForm.CloseWindow();
		}

		// This applies the curves and returns to the base mode
		public override void OnAccept()
		{
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
			// Get the selection
			ErrorResult selection = BuilderPlug.Me.ErrorCheckForm.SelectedResult;
			
			renderer.RedrawSurface();
			
			// Render lines
			if(renderer.StartPlotter(true))
			{
				renderer.PlotLinedefSet(General.Map.Map.Linedefs);
				renderer.PlotVerticesSet(General.Map.Map.Vertices);
				if(selection != null) selection.PlotSelection(renderer);
				renderer.Finish();
			}
			
			// Render things
			if(renderer.StartThings(true))
			{
				renderer.RenderThingSet(General.Map.Map.Things, 1.0f);
				if(selection != null) selection.RenderThingsSelection(renderer);
				renderer.Finish();
			}
			
			// Render overlay
			if(renderer.StartOverlay(true))
			{
				if(selection != null) selection.RenderOverlaySelection(renderer);
				renderer.Finish();
			}
			
			renderer.Present();
		}

		#endregion
	}
}
