
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
	public sealed class PK3FileImage : ImageData
	{
		#region ================== Variables

		private PK3Reader datareader;
		private string filepathname;
		private int probableformat;
		
		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		internal PK3FileImage(PK3Reader datareader, string name, string filepathname, bool asflat)
		{
			// Initialize
			this.datareader = datareader;
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
				MemoryStream filedata = datareader.ExtractFile(filepathname);

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
					General.ErrorLogger.Add(ErrorType.Error, "Image file '" + filepathname + "' data format could not be read, while loading texture '" + this.Name + "'");
					loadfailed = true;
				}
				else
				{
					// Get width and height from image
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
