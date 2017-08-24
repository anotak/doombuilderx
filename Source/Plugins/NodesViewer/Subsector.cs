#region === Copyright (c) 2010 Pascal van der Heiden ===

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using CodeImp.DoomBuilder.Editing;
using CodeImp.DoomBuilder.Geometry;
using CodeImp.DoomBuilder.Rendering;

#endregion

namespace CodeImp.DoomBuilder.Plugins.NodesViewer
{
	public struct Subsector
	{
		public int numsegs;
		public int firstseg;

		public Vector2D[] points;
		public FlatVertex[] vertices;
	}
}
