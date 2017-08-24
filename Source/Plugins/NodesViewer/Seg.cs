#region === Copyright (c) 2010 Pascal van der Heiden ===

using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using CodeImp.DoomBuilder.Editing;
using CodeImp.DoomBuilder.Geometry;

#endregion

namespace CodeImp.DoomBuilder.Plugins.NodesViewer
{
	public struct Seg
	{
		public int startvertex;
		public int endvertex;
		public float angle;
		public int lineindex;
		public bool leftside;
		public float offset;
		public int ssector;
	}
}
