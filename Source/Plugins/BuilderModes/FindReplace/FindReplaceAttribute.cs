
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
	[AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
	internal class FindReplaceAttribute : Attribute
	{
		#region ================== Variables

		private string displayname;
		private bool browsebutton;
		private bool replacable;

		#endregion

		#region ================== Properties

		public string DisplayName { get { return displayname; } set { displayname = value; } }
		public bool BrowseButton { get { return browsebutton; } set { browsebutton = value; } }
		public bool Replacable { get { return replacable; } set { replacable = value; } }

		#endregion

		#region ================== Constructor / Destructor

		// Constructor
		public FindReplaceAttribute(string displayname)
		{
			// Initialize
			this.displayname = displayname;
			this.replacable = true;
		}

		#endregion

		#region ================== Methods

		// String representation
		public override string ToString()
		{
			return displayname;
		}

		#endregion
	}
}
