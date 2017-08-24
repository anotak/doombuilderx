
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
using SlimDX.Direct3D9;
using System.ComponentModel;
using CodeImp.DoomBuilder.Geometry;
using SlimDX;

#endregion

namespace CodeImp.DoomBuilder.Rendering
{
	internal interface ID3DResource
	{
		// This is used to unload the resouce
		void UnloadResource();
		
		// This is used to reload the resource
		void ReloadResource();
	}
}
