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
	internal abstract class SectorEffect
	{
		protected SectorData data;
		
		// Constructor
		protected SectorEffect(SectorData data)
		{
			this.data = data;
		}

		// This makes sure we are updated with the source linedef information
		public abstract void Update();
	}
}
