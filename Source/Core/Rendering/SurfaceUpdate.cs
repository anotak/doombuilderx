
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
using System.Drawing;
using System.ComponentModel;
using CodeImp.DoomBuilder.Map;
using SlimDX.Direct3D9;
using SlimDX;
using CodeImp.DoomBuilder.Geometry;
using System.Drawing.Imaging;
using CodeImp.DoomBuilder.Data;
using CodeImp.DoomBuilder.Editing;

using Configuration = CodeImp.DoomBuilder.IO.Configuration;

#endregion

namespace CodeImp.DoomBuilder.Rendering
{
	// This contains information to update surface entries with. This may exceed the maximum number
	// of sector vertices, the surface manager will take care of splitting it up in several SurfaceEntries.
	internal class SurfaceUpdate
	{
		public int numvertices;
		
		// Sector geometry (local copy used to quickly refill buffers)
		// The sector must set these!
		public FlatVertex[] floorvertices;
		public FlatVertex[] ceilvertices;
		
		// Sector images
		// The sector must set these!
		public long floortexture;
		public long ceiltexture;
		
		// Constructor
		internal SurfaceUpdate(int numvertices, bool updatefloor, bool updateceiling)
		{
			this.numvertices = numvertices;
			this.floortexture = 0;
			this.ceiltexture = 0;
			
			if(updatefloor)
				this.floorvertices = new FlatVertex[numvertices];
			else
				this.floorvertices = null;

			if(updateceiling)
				this.ceilvertices = new FlatVertex[numvertices];
			else
				this.ceilvertices = null;
		}
	}
}
