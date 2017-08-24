
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
using CodeImp.DoomBuilder.Map;
using System.Collections.ObjectModel;

#endregion

namespace CodeImp.DoomBuilder.Config
{
	public struct MapLumpInfo
	{
		// Members
		public string name;
		public bool required;
		public bool blindcopy;
		public bool nodebuild;
		public bool allowempty;
		internal ScriptConfiguration script;
		
		// Construct from IDictionary
		internal MapLumpInfo(string name, Configuration cfg)
		{
			string scriptconfig = "";
			
			// Apply settings
			this.name = name;
			this.script = null;
			this.required = cfg.ReadSetting("maplumpnames." + name + ".required", false);
			this.blindcopy = cfg.ReadSetting("maplumpnames." + name + ".blindcopy", false);
			this.nodebuild = cfg.ReadSetting("maplumpnames." + name + ".nodebuild", false);
			this.allowempty = cfg.ReadSetting("maplumpnames." + name + ".allowempty", false);
			scriptconfig = cfg.ReadSetting("maplumpnames." + name + ".script", "");
			
			// Find script configuration
			if(scriptconfig.Length > 0)
			{
				if(General.ScriptConfigs.ContainsKey(scriptconfig.ToLowerInvariant()))
				{
					this.script = General.ScriptConfigs[scriptconfig.ToLowerInvariant()];
				}
				else
				{
					General.ErrorLogger.Add(ErrorType.Warning, "Map lump '" + name + "' in the current game configuration specifies an unknown script configuration '" + scriptconfig + "'. Using plain text instead.");
					this.script = new ScriptConfiguration();
				}
			}
		}
	}
}

