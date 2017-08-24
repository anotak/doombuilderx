
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
using CodeImp.DoomBuilder.Rendering;
using CodeImp.DoomBuilder.IO;
using System.IO;

#endregion

namespace CodeImp.DoomBuilder.Data
{
	public sealed class UnknownImage : ImageData
	{
		#region ================== Variables

		private Bitmap loadbitmap = null;
		
		#endregion
		
		#region ================== Constructor / Disposer

		// Constructor
		public UnknownImage(Bitmap image)
		{
			// Initialize
			this.width = 0;
			this.height = 0;
			this.loadbitmap = image;
			SetName("");
			
			LocalLoadImage();
			
			// We have no destructor
			GC.SuppressFinalize(this);
		}

		#endregion

		#region ================== Methods
		
		// This 'loads' the image
		protected override void LocalLoadImage()
		{
			bitmap = loadbitmap;
			base.LocalLoadImage();
		}

		// This returns a preview image
		public override Image GetPreview()
		{
			lock(this)
			{
				// Make a copy
				return new Bitmap(loadbitmap);
			}
		}

		#endregion
	}
}
