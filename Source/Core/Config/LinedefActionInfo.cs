
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

#endregion

namespace CodeImp.DoomBuilder.Config
{
	public class LinedefActionInfo : INumberedTitle, IComparable<LinedefActionInfo>
	{
		#region ================== Constants

		#endregion

		#region ================== Variables

		// Properties
		private int index;
		private string prefix;
		private string category;
		private string name;
		private string title;
		private ArgumentInfo[] args;
		private bool isgeneralized;
		private bool isknown;
        private bool highlightmatchinglineid;
		
		#endregion

		#region ================== Properties

		public int Index { get { return index; } }
		public string Prefix { get { return prefix; } }
		public string Category { get { return category; } }
		public string Name { get { return name; } }
		public string Title { get { return title; } }
		public bool IsGeneralized { get { return isgeneralized; } }
		public bool IsKnown { get { return isknown; } }
		public bool IsNull { get { return (index == 0); } }
        public bool HighlightMatchingLineID { get { return highlightmatchinglineid; } }
		public ArgumentInfo[] Args { get { return args; } }

		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		internal LinedefActionInfo(int index, Configuration cfg, string categoryname, IDictionary<string, EnumList> enums)
		{
			string actionsetting = "linedeftypes." + categoryname + "." + index.ToString(CultureInfo.InvariantCulture);
			
			// Initialize
			this.index = index;
			this.category = categoryname;
			this.args = new ArgumentInfo[Linedef.NUM_ARGS];
			this.isgeneralized = false;
			this.isknown = true;
			
			// Read settings
			this.name = cfg.ReadSetting(actionsetting + ".title", "Unnamed");
			this.prefix = cfg.ReadSetting(actionsetting + ".prefix", "");
			this.title = this.prefix + " " + this.name;
			this.title = this.title.Trim();

            this.name = cfg.ReadSetting(actionsetting + ".title", "Unnamed");
            
            this.highlightmatchinglineid = cfg.ReadSetting(actionsetting + ".highlightmatchinglineid", false);

            // Read the args
            for (int i = 0; i < Linedef.NUM_ARGS; i++)
				this.args[i] = new ArgumentInfo(cfg, actionsetting, i, enums);
			
			// We have no destructor
			GC.SuppressFinalize(this);
		}
		
		// Constructor for generalized type display
		internal LinedefActionInfo(int index, string title, bool isknown, bool isgeneralized)
		{
			this.index = index;
			this.isgeneralized = isgeneralized;
			this.isknown = isknown;
			this.title = title;
            this.highlightmatchinglineid = false;
			this.args = new ArgumentInfo[Linedef.NUM_ARGS];
			for(int i = 0; i < Linedef.NUM_ARGS; i++)
				this.args[i] = new ArgumentInfo(i);
		}

		#endregion

		#region ================== Methods

		// This presents the item as string
		public override string ToString()
		{
			return index + " - " + title;
		}

		// This compares against another action info
		public int CompareTo(LinedefActionInfo other)
		{
			if(this.index < other.index) return -1;
			else if(this.index > other.index) return 1;
			else return 0;
		}

		#endregion
	}
}
