
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
using CodeImp.DoomBuilder.Compilers;

#endregion

namespace CodeImp.DoomBuilder.Config
{
	public class PasteOptions
	{
		#region ================== Constants
		
		public const int TAGS_KEEP = 0;
		public const int TAGS_RENUMBER = 1;
		public const int TAGS_REMOVE = 2;
		
		#endregion
		
		#region ================== Variables
		
		private int changetags;				// See TAGS_ constants
		private bool removeactions;
		private bool adjustheights;
		
		#endregion
		
		#region ================== Properties
		
		public int ChangeTags { get { return changetags; } set { changetags = value; } }
		public bool RemoveActions { get { return removeactions; } set { removeactions = value; } }
		public bool AdjustHeights { get { return adjustheights; } set { adjustheights = value; } }
		
		#endregion
		
		#region ================== Constructor / Disposer
		
		// Constructor
		public PasteOptions()
		{
		}
		
		// Copy Constructor
		public PasteOptions(PasteOptions p)
		{
			this.changetags = p.changetags;
			this.removeactions = p.removeactions;
			this.adjustheights = p.adjustheights;
		}
		
		#endregion
		
		#region ================== Methods
		
		// Make a copy
		public PasteOptions Copy()
		{
			return new PasteOptions(this);
		}
		
		// This reads from configuration
		internal void ReadConfiguration(Configuration cfg, string path)
		{
			changetags = cfg.ReadSetting(path + ".changetags", 0);
			removeactions = cfg.ReadSetting(path + ".removeactions", false);
			adjustheights = cfg.ReadSetting(path + ".adjustheights", true);
		}
		
		// This writes to configuration
		internal void WriteConfiguration(Configuration cfg, string path)
		{
			cfg.WriteSetting(path + ".changetags", changetags);
			cfg.WriteSetting(path + ".removeactions", removeactions);
			cfg.WriteSetting(path + ".adjustheights", adjustheights);
		}
		
		#endregion
	}
}
