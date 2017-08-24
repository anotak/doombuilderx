
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

#endregion

namespace CodeImp.DoomBuilder.Data
{
	public sealed class FileImage : ImageData
	{
		#region ================== Variables

		private string filepathname;
		private int probableformat;
		
		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		public FileImage(string name, string filepathname, bool asflat)
		{
			// Initialize
			this.filepathname = filepathname;
			SetName(name);

			if(asflat)
			{
                bIsFlat = true;
				probableformat = ImageDataFormat.DOOMFLAT;
				this.scale.x = General.Map.Config.DefaultFlatScale;
				this.scale.y = General.Map.Config.DefaultFlatScale;
			}
			else
			{
				probableformat = ImageDataFormat.DOOMPICTURE;
				this.scale.x = General.Map.Config.DefaultTextureScale;
				this.scale.y = General.Map.Config.DefaultTextureScale;
			}
			
			// We have no destructor
			GC.SuppressFinalize(this);
		}

		// Constructor
		public FileImage(string name, string filepathname, bool asflat, float scalex, float scaley)
		{
			// Initialize
			this.filepathname = filepathname;
			this.scale.x = scalex;
			this.scale.y = scaley;
			SetName(name);

			if(asflat)
				probableformat = ImageDataFormat.DOOMFLAT;
			else
				probableformat = ImageDataFormat.DOOMPICTURE;

			// We have no destructor
			GC.SuppressFinalize(this);
		}

		#endregion

		#region ================== Methods

		// This loads the image
		protected override void LocalLoadImage()
		{
			// Leave when already loaded
			if(this.IsImageLoaded) return;

			lock(this)
			{
				// Load file data
				if(bitmap != null) bitmap.Dispose(); bitmap = null;
				MemoryStream filedata = new MemoryStream(File.ReadAllBytes(filepathname));

				// Get a reader for the data
				IImageReader reader = ImageDataFormat.GetImageReader(filedata, probableformat, General.Map.Data.Palette);
				if(!(reader is UnknownImageReader))
				{
					// Load the image
					filedata.Seek(0, SeekOrigin.Begin);
					try { bitmap = reader.ReadAsBitmap(filedata); }
					catch(InvalidDataException)
					{
						// Data cannot be read!
						bitmap = null;
					}
				}
				
				// Not loaded?
				if(bitmap == null)
				{
					General.ErrorLogger.Add(ErrorType.Error, "Image file '" + filepathname + "' data format could not be read, while loading image '" + this.Name + "'. Is this a valid picture file at all?");
					loadfailed = true;
				}
				else
				{
					// Get width and height
					width = bitmap.Size.Width;
					height = bitmap.Size.Height;
				}
				
				// Pass on to base
				filedata.Dispose();
				base.LocalLoadImage();
			}
		}
		
		#endregion
	}
}
