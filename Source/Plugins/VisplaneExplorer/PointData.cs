#region === Copyright (c) 2010 Pascal van der Heiden ===

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

#endregion

namespace CodeImp.DoomBuilder.Plugins.VisplaneExplorer
{
	internal struct PointData
	{
		public TilePoint point;
		public PointResult result;
		public int visplanes;
		public int drawsegs;
		public int solidsegs;
		public int openings;
	}
}
