#region === Copyright (c) 2010 Pascal van der Heiden ===

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using CodeImp.DoomBuilder.Geometry;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Rendering;

#endregion

namespace CodeImp.DoomBuilder.GZDoomEditing
{
	internal class SectorLevelComparer : IComparer<SectorLevel>
	{
		// Center of sector to use for plane comparison
		public Vector2D center;
		
		// Constructor
		public SectorLevelComparer(Sector s)
		{
			this.center = new Vector2D(s.BBox.Left + s.BBox.Width / 2, s.BBox.Top + s.BBox.Height / 2);
		}

		// Comparer
		public int Compare(SectorLevel x, SectorLevel y)
		{
			return Math.Sign(x.plane.GetZ(center) - y.plane.GetZ(center));
		}
	}
}
