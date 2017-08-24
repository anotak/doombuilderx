
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
using CodeImp.DoomBuilder.Plugins;
using CodeImp.DoomBuilder.Types;
using CodeImp.DoomBuilder.Config;

#endregion

namespace CodeImp.DoomBuilder.BuilderModes
{
	public struct Association
	{
		public int tag;
		public UniversalType type;

		// This sets up the association
		public Association(int tag, int type)
		{
			this.tag = tag;
			this.type = (UniversalType)type;
		}

		// This sets up the association
		public Association(int tag, UniversalType type)
		{
			this.tag = tag;
			this.type = type;
		}

		// This sets up the association
		public void Set(int tag, int type)
		{
			this.tag = tag;
			this.type = (UniversalType)type;
		}

		// This sets up the association
		public void Set(int tag, UniversalType type)
		{
			this.tag = tag;
			this.type = type;
		}

		// This compares an association
		public static bool operator ==(Association a, Association b)
		{
			return (a.tag == b.tag) && (a.type == b.type);
		}

		// This compares an association
		public static bool operator !=(Association a, Association b)
		{
			return (a.tag != b.tag) || (a.type != b.type);
		}
	}
}
