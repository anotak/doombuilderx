
#region ================== Copyright (c) 2012 Pascal vd Heiden

/*
 * Copyright (c) 2012 Pascal vd Heiden, www.codeimp.com
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
using System.IO;
using System.Reflection;
using CodeImp.DoomBuilder.Actions;
using CodeImp.DoomBuilder.Plugins.VisplaneExplorer.Properties;
using CodeImp.DoomBuilder.Windows;
using CodeImp.DoomBuilder.IO;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Rendering;
using CodeImp.DoomBuilder.Geometry;
using System.Drawing;
using CodeImp.DoomBuilder.Editing;
using CodeImp.DoomBuilder.Plugins;
using CodeImp.DoomBuilder.Types;
using CodeImp.DoomBuilder.Config;
using CodeImp.DoomBuilder.Data;

#endregion

namespace CodeImp.DoomBuilder.Plugins.VisplaneExplorer
{
	public class BuilderPlug : Plug
	{
		#region ================== Variables

		// Objects
		private static BuilderPlug me;
		private VPOManager vpo;
		private InterfaceForm interfaceform;

		// Palettes
		private Palette[] palettes;
		
		#endregion

		#region ================== Properties
		
		// Properties
		public static BuilderPlug Me { get { return me; } }
		public override string Name { get { return "VisplaneExplorer"; } }
		internal static VPOManager VPO { get { return me.vpo; } }
		internal static InterfaceForm InterfaceForm { get { return me.interfaceform; } }
		internal static Palette[] Palettes { get { return me.palettes; } }
		public override int MinimumRevision { get { return 1545; } }
		
		#endregion

		#region ================== Initialize / Dispose

		// This event is called when the plugin is initialized
		public override void OnInitialize()
		{
			base.OnInitialize();
			
			General.Actions.BindMethods(this);
			
			// Load interface controls
			interfaceform = new InterfaceForm();

			// Load VPO manager (manages multithreading and communication with vpo.dll)
			vpo = new VPOManager();

			// Keep a static reference
			me = this;
		}

		// Some things cannot be initialized at plugin start, so we do them here
		public override void OnMapOpenBegin()
		{
			base.OnMapOpenBegin();

			if(palettes == null)
			{
				// Load palettes
				palettes = new Palette[(int)ViewStats.NumStats];
				palettes[(int)ViewStats.Visplanes] = new Palette(Resources.Visplanes_pal);
				palettes[(int)ViewStats.Drawsegs] = new Palette(Resources.Drawsegs_pal);
				palettes[(int)ViewStats.Solidsegs] = new Palette(Resources.Solidsegs_pal);
				palettes[(int)ViewStats.Openings] = new Palette(Resources.Openings_pal);
				ApplyUserColors();
			}
		}

		// Preferences changed
		public override void OnClosePreferences(PreferencesController controller)
		{
			base.OnClosePreferences(controller);
			ApplyUserColors();
		}

		// This is called when the plugin is terminated
		public override void Dispose()
		{
			// Clean up
			interfaceform.Dispose();
			interfaceform = null;
			vpo.Dispose();
			vpo = null;
			General.Actions.UnbindMethods(this);
			base.Dispose();
		}

		#endregion

		#region ================== Methods

		// This applies user-defined appearance colors to the palettes
		private void ApplyUserColors()
		{
			if(palettes != null)
			{
				// Override special palette indices with user-defined colors
				for(int i = 0; i < palettes.Length; i++)
				{
					palettes[i].SetColor(Tile.POINT_VOID_B, General.Colors.Background.WithAlpha(0).ToInt());
				}
			}
		}

		// This returns a unique temp filename
		public static string MakeTempFilename(string extension)
		{
			string filename;
			string chars = "abcdefghijklmnopqrstuvwxyz1234567890";
			Random rnd = new Random();
			int i;

			do
			{
				// Generate a filename
				filename = "";
				for(i = 0; i < 8; i++) filename += chars[rnd.Next(chars.Length)];
				filename = Path.Combine(General.TempPath, filename + extension);
			}
			// Continue while file is not unique
			while(File.Exists(filename) || Directory.Exists(filename));

			// Return the filename
			return filename;
		}

		#endregion
	}
}
