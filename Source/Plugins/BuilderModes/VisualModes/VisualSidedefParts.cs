
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
using CodeImp.DoomBuilder.Editing;
using CodeImp.DoomBuilder.VisualModes;

#endregion

namespace CodeImp.DoomBuilder.BuilderModes
{
	internal struct VisualSidedefParts
	{
		// Members
		public VisualUpper upper;
		public VisualLower lower;
		public VisualMiddleDouble middledouble;
		public VisualMiddleSingle middlesingle;

		// Constructor
		public VisualSidedefParts(VisualUpper u, VisualLower l, VisualMiddleDouble m)
		{
			this.upper = u;
			this.lower = l;
			this.middledouble = m;
			this.middlesingle = null;
		}

		// Constructor
		public VisualSidedefParts(VisualMiddleSingle m)
		{
			this.upper = null;
			this.lower = null;
			this.middledouble = null;
			this.middlesingle = m;
		}

		// This calls Setup() on all parts
		public void SetupAllParts()
		{
			if(lower != null) lower.Setup();
			if(middledouble != null) middledouble.Setup();
			if(middlesingle != null) middlesingle.Setup();
			if(upper != null) upper.Setup();
		}
	}
}
