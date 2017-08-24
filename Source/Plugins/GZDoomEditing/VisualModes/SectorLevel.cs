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
	internal class SectorLevel
	{
		// Type of level
		public SectorLevelType type;

		// Sector where this level originates from
		public Sector sector;
		
		// Plane in the sector
		public Plane plane;
		
		// Alpha for translucency (255=opaque)
		public int alpha;
		
		// Color of the plane (includes brightness)
		// When this is 0, it takes the color from the sector above
		public int color;
		
		// Color and brightness below the plane
		// When this is 0, it takes the color from the sector above
		public int brightnessbelow;
		public PixelColor colorbelow;
		
		// Constructor
		public SectorLevel(Sector s, SectorLevelType type)
		{
			this.type = type;
			this.sector = s;
			this.alpha = 255;
		}
		
		// Copy constructor
		public SectorLevel(SectorLevel source)
		{
			source.CopyProperties(this);
		}

		// Copy properties
		public void CopyProperties(SectorLevel target)
		{
			target.sector = this.sector;
			target.type = this.type;
			target.plane = this.plane;
			target.alpha = this.alpha;
			target.color = this.color;
			target.brightnessbelow = this.brightnessbelow;
			target.colorbelow = this.colorbelow;
		}
	}
}
