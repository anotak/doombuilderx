
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
using CodeImp.DoomBuilder.Actions;
using CodeImp.DoomBuilder.Geometry;
using System.Drawing;
using SlimDX;
using CodeImp.DoomBuilder.Editing;

#endregion

namespace CodeImp.DoomBuilder.Map
{
	public class BlockEntry
	{
		#region ================== Variables
		
		// Members
		private List<Linedef> lines;
		private List<Thing> things;
		private List<Sector> sectors;
		
		#endregion
		
		#region ================== Properties
		
		public List<Linedef> Lines { get { return lines; } }
		public List<Thing> Things { get { return things; } }
		public List<Sector> Sectors { get { return sectors; } }
		
		#endregion
		
		#region ================== Constructor
		
		// Constructor for empty block
		public BlockEntry()
		{
			lines = new List<Linedef>(2);
			things = new List<Thing>(2);
			sectors = new List<Sector>(2);
		}
		
		#endregion
	}
}
