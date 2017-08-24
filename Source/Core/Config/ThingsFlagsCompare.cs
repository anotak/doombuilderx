
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
using CodeImp.DoomBuilder.IO;
using CodeImp.DoomBuilder.Data;
using System.IO;
using System.Diagnostics;
using System.Windows.Forms;
using CodeImp.DoomBuilder.Rendering;
using CodeImp.DoomBuilder.Map;

#endregion

namespace CodeImp.DoomBuilder.Config
{
	public class ThingFlagsCompare
	{
		public enum CompareMethod
		{
			Equal,
			And
		};

		#region ================== Constants

		#endregion

		#region ================== Variables

		private string flag;
		private CompareMethod comparemethod;
		private bool invert;
		private string group;

		// Disposing
		private bool isdisposed = false;

		#endregion

		#region ================== Properties

		public string Flag { get { return flag; } }
		public string Group { get { return group; } }
		public bool IsDisposed { get { return isdisposed; } }

		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		public ThingFlagsCompare(Configuration cfg, string group, string flag)
		{
			string cfgpath = "thingflagscompare." + group + "." + flag;
			this.flag = flag;
			this.group = group;

			string cm = cfg.ReadSetting(cfgpath + ".comparemethod", "and");

			switch (cm)
			{
				default:
					General.ErrorLogger.Add(ErrorType.Warning, "Unrecognized value \"" + cm + "\" for comparemethod in " + cfgpath + " in game configuration " + cfg.ReadSetting("game", "<unnamed game>") + ". Defaulting to \"and\".");
					goto case "and";
				case "and":
					comparemethod = CompareMethod.And;
					break;
				case "equal":
					comparemethod = CompareMethod.Equal;
					break;
			}

			invert = cfg.ReadSetting(cfgpath + ".invert", false);
			
			// We have no destructor
			GC.SuppressFinalize(this);
		}

		// Disposer
		internal void Dispose()
		{
			// Not already disposed?
			if (!isdisposed)
			{
				// Done
				isdisposed = true;
			}
		}

		#endregion

		#region ================== Methods

		// Compares the flag of the two things.
		// Returns:
		// -1 if the flag does not overlap
		//	0 if the flag should be ignored
		//	1 if the flag overlaps
		public int Compare(Thing t1, Thing t2)
		{
			bool t1flag;
			bool t2flag;

			// Check if the flags exist
			if (!t1.Flags.ContainsKey(flag) || !t2.Flags.ContainsKey(flag))
				return 0;

			// tag flag inversion into account
			t1flag = invert ? !t1.Flags[flag] : t1.Flags[flag];
			t2flag = invert ? !t2.Flags[flag] : t2.Flags[flag];

			if (comparemethod == CompareMethod.And && (t1flag && t2flag))
				return 1;
			else if (comparemethod == CompareMethod.Equal && (t1flag == t2flag))
				return 1;
	
			return 0;
		}

		#endregion
	}
}

