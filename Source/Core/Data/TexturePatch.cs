
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
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using CodeImp.DoomBuilder.IO;
using CodeImp.DoomBuilder.Rendering;

#endregion

namespace CodeImp.DoomBuilder.Data
{
	internal enum TexturePathRenderStyle
	{
		Copy,
		Blend,
		Add,
		Subtract,
		ReverseSubtract,
		Modulate,
		CopyAlpha
	}
	
	internal struct TexturePatch
	{
		public string lumpname;
		public int x;
		public int y;
		public bool flipx;
		public bool flipy;
		public int rotate;
		public PixelColor blend;
		public float alpha;
		public TexturePathRenderStyle style;
		
		// Constructor for simple patches
		public TexturePatch(string lumpname, int x, int y)
		{
			// Initialize
			this.lumpname = lumpname;
			this.x = x;
			this.y = y;
			this.flipx = false;
			this.flipy = false;
			this.rotate = 0;
			this.blend = new PixelColor(0, 0, 0, 0);
			this.alpha = 1.0f;
			this.style = TexturePathRenderStyle.Copy;
		}

		// Constructor for hires patches
		public TexturePatch(string lumpname, int x, int y, bool flipx, bool flipy, int rotate, PixelColor blend, float alpha, int style)
		{
			// Initialize
			this.lumpname = lumpname;
			this.x = x;
			this.y = y;
			this.flipx = flipx;
			this.flipy = flipy;
			this.rotate = rotate;
			this.blend = blend;
			this.alpha = alpha;
			this.style = (TexturePathRenderStyle)style;
		}
	}
}
