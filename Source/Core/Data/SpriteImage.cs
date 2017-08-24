
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
	public sealed class SpriteImage : ImageData
	{
		#region ================== Variables

		protected int offsetx;
		protected int offsety;
		
		#endregion

		#region ================== Properties

		public int OffsetX { get { return offsetx; } }
		public int OffsetY { get { return offsety; } }
		
		#endregion
		
		#region ================== Constructor / Disposer

		// Constructor
		internal SpriteImage(string name)
		{
			// Initialize
			SetName(name);

			// We have no destructor
			GC.SuppressFinalize(this);
		}

		#endregion

		#region ================== Methods

		// This loads the image
		protected override void LocalLoadImage()
		{
			Stream lumpdata;
			MemoryStream mem;
			IImageReader reader;
			byte[] membytes;

			// Leave when already loaded
			if(this.IsImageLoaded) return;

			lock(this)
			{
				// Get the lump data stream
				lumpdata = General.Map.Data.GetSpriteData(Name);
				if(lumpdata != null)
				{
					// Copy lump data to memory
					lumpdata.Seek(0, SeekOrigin.Begin);
					membytes = new byte[(int)lumpdata.Length];
					lumpdata.Read(membytes, 0, (int)lumpdata.Length);
					mem = new MemoryStream(membytes);
					mem.Seek(0, SeekOrigin.Begin);
					
					// Get a reader for the data
					reader = ImageDataFormat.GetImageReader(mem, ImageDataFormat.DOOMPICTURE, General.Map.Data.Palette);
					if(reader is UnknownImageReader)
					{
						// Data is in an unknown format!
						General.ErrorLogger.Add(ErrorType.Error, "Sprite lump '" + Name + "' data format could not be read. Does this lump contain valid picture data at all?");
						bitmap = null;
					}
					else
					{
						// Read data as bitmap
						mem.Seek(0, SeekOrigin.Begin);
						if(bitmap != null) bitmap.Dispose();
						bitmap = reader.ReadAsBitmap(mem, out offsetx, out offsety);
					}
					
					// Done
					mem.Dispose();

					if(bitmap != null)
					{
						// Get width and height from image
						width = bitmap.Size.Width;
						height = bitmap.Size.Height;
						scale.x = 1.0f;
						scale.y = 1.0f;
						
						// Make offset corrections if the offset was not given
						if((offsetx == int.MinValue) || (offsety == int.MinValue))
						{
							offsetx = (int)((width * scale.x) * 0.5f);
							offsety = (int)(height * scale.y);
						}
					}
					else
					{
						loadfailed = true;
					}
				}
				else
				{
					// Missing a patch lump!
					General.ErrorLogger.Add(ErrorType.Error, "Missing sprite lump '" + Name + "'. Forgot to include required resources?");
				}

				// Pass on to base
				base.LocalLoadImage();
			}
		}

		#endregion
	}
}
