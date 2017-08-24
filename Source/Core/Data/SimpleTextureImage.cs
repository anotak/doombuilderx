
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
	internal class SimpleTextureImage : ImageData
	{
		#region ================== Constants

		#endregion

		#region ================== Variables

		private string lumpname;

		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		public SimpleTextureImage(string name, string lumpname, float scalex, float scaley)
		{
			// Initialize
			this.scale.x = scalex;
			this.scale.y = scaley;
			this.lumpname = lumpname;
			SetName(name);
			
			// We have no destructor
			GC.SuppressFinalize(this);
		}

		#endregion

		#region ================== Methods

		// This loads the image
		protected override void LocalLoadImage()
		{
			IImageReader reader;
			MemoryStream mem;
			Stream patchdata;
			byte[] membytes;

			// Checks
			if(this.IsImageLoaded) return;

			lock(this)
			{
				// Get the patch data stream
				if(bitmap != null) bitmap.Dispose(); bitmap = null;
				patchdata = General.Map.Data.GetTextureData(lumpname);
				if(patchdata != null)
				{
					// Copy patch data to memory
					patchdata.Seek(0, SeekOrigin.Begin);
					membytes = new byte[(int)patchdata.Length];
					patchdata.Read(membytes, 0, (int)patchdata.Length);
					mem = new MemoryStream(membytes);
					mem.Seek(0, SeekOrigin.Begin);

					// Get a reader for the data
					reader = ImageDataFormat.GetImageReader(mem, ImageDataFormat.DOOMPICTURE, General.Map.Data.Palette);
					if(!(reader is UnknownImageReader))
					{
						// Load the image
						mem.Seek(0, SeekOrigin.Begin);
						try { bitmap = reader.ReadAsBitmap(mem); }
						catch(InvalidDataException)
						{
							// Data cannot be read!
							bitmap = null;
						}
					}

					// Not loaded?
					if(bitmap == null)
					{
						General.ErrorLogger.Add(ErrorType.Error, "Image lump '" + lumpname + "' data format could not be read, while loading texture '" + this.Name + "'. Does this lump contain valid picture data at all?");
						loadfailed = true;
					}
					else
					{
						// Get width and height from image
						width = bitmap.Size.Width;
						height = bitmap.Size.Height;
					}

					// Done
					mem.Dispose();
				}
				else
				{
					General.ErrorLogger.Add(ErrorType.Error, "Image lump '" + lumpname + "' could not be found, while loading texture '" + this.Name + "'. Did you forget to include required resources?");
					loadfailed = true;
				}
				
				// Pass on to base
				base.LocalLoadImage();
			}
		}
		
		#endregion
	}
}
