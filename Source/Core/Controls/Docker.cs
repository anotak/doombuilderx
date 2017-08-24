
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
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32;
using System.Diagnostics;
using CodeImp.DoomBuilder.Actions;
using CodeImp.DoomBuilder.Data;
using CodeImp.DoomBuilder.Config;
using CodeImp.DoomBuilder.Rendering;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using SlimDX.Direct3D9;
using SlimDX;

#endregion

namespace CodeImp.DoomBuilder.Controls
{
	public class Docker
	{
		#region ================== Variables
		
		private string shortname;
		private string fullname;
		private string title;
		private Control control;
		
		#endregion
		
		#region ================== Variables
		
		public string Name { get { return shortname; } }
		internal string FullName { get { return fullname; } }
		public string Title { get { return title; } }
		public Control Control { get { return control; } }
		
		#endregion
		
		#region ================== Constructor
		
		// Constructor
		public Docker(string name, string title, Control control)
		{
			this.shortname = name;
			this.title = title;
			this.control = control;
		}
		
		#endregion
		
		#region ================== Methods
		
		// This makes the full name
		internal void MakeFullName(string prefix)
		{
			fullname = prefix + "_" + shortname;
		}
		
		#endregion
	}
}
