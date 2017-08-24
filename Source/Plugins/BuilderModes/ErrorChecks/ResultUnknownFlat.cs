
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
	public class ResultUnknownFlat : ErrorResult
	{
		#region ================== Variables
		
		private Sector sector;
		private bool ceiling;
		
		#endregion
		
		#region ================== Properties

		public override int Buttons { get { return 1; } }
		public override string Button1Text { get { return "Add Default Flat"; } }
		
		#endregion
		
		#region ================== Constructor / Destructor
		
		// Constructor
		public ResultUnknownFlat(Sector s, bool ceiling)
		{
			// Initialize
			this.sector = s;
			this.ceiling = ceiling;
			this.viewobjects.Add(s);
			
			string objname = ceiling ? "ceiling" : "floor";
			this.description = "This sector " + objname + " uses an unknown flat. This could be the result of missing resources, or a mistyped flat name. Click the Add Default Flat button to use a known flat instead.";
		}
		
		#endregion
		
		#region ================== Methods
		
		// This must return the string that is displayed in the listbox
		public override string ToString()
		{
			if(ceiling)
				return "Sector has unknown ceiling flat \"" + sector.CeilTexture + "\"";
			else
				return "Sector has unknown floor flat \"" + sector.FloorTexture + "\"";
		}
		
		// Rendering
		public override void PlotSelection(IRenderer2D renderer)
		{
			renderer.PlotSector(sector, General.Colors.Selection);
		}
		
		// Fix by setting default flat
		public override bool Button1Click()
		{
			General.Map.UndoRedo.CreateUndo("Unknown flat correction");
			General.Settings.FindDefaultDrawSettings();
			
			if(ceiling)
				sector.SetCeilTexture(General.Settings.DefaultCeilingTexture);
			else
				sector.SetFloorTexture(General.Settings.DefaultFloorTexture);
			
			General.Map.Map.Update();
			return true;
		}
		
		#endregion
	}
}
