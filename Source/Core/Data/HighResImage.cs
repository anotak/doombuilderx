
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
	internal sealed unsafe class HighResImage : ImageData
	{
		#region ================== Variables

		private List<TexturePatch> patches;
		
		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		public HighResImage(string name, int width, int height, float scalex, float scaley, bool worldpanning)
		{
			// Initialize
			this.width = width;
			this.height = height;
			this.scale.x = scalex;
			this.scale.y = scaley;
			this.worldpanning = worldpanning;
			this.patches = new List<TexturePatch>();
			SetName(name);
			
			// We have no destructor
			GC.SuppressFinalize(this);
		}

		#endregion

		#region ================== Methods

		// This adds a patch to the texture
		public void AddPatch(TexturePatch patch)
		{
			// Add it
			patches.Add(patch);
		}
		
		// This loads the image
		protected override void LocalLoadImage()
		{
			IImageReader reader;
			MemoryStream mem;
			byte[] membytes;
			Graphics g = null;
			
			// Checks
			if(this.IsImageLoaded) return;
			if((width == 0) || (height == 0)) return;
			
			lock(this)
			{
				// Create texture bitmap
				try
				{
					if(bitmap != null) bitmap.Dispose();
					bitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb);
					BitmapData bitmapdata = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
					PixelColor* pixels = (PixelColor*)bitmapdata.Scan0.ToPointer();
					General.ZeroMemory(new IntPtr(pixels), width * height * sizeof(PixelColor));
					bitmap.UnlockBits(bitmapdata);
					g = Graphics.FromImage(bitmap);
				}
				catch(Exception e)
				{
					// Unable to make bitmap
					General.ErrorLogger.Add(ErrorType.Error, "Unable to load texture image '" + this.Name + "'. " + e.GetType().Name + ": " + e.Message);
					loadfailed = true;
				}

				if(!loadfailed)
				{
					// Go for all patches
					foreach(TexturePatch p in patches)
					{
						// Get the patch data stream
						Stream patchdata = General.Map.Data.GetPatchData(p.lumpname);
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
							if(reader is UnknownImageReader)
							{
								// Data is in an unknown format!
								General.ErrorLogger.Add(ErrorType.Error, "Patch lump '" + p.lumpname + "' data format could not be read, while loading texture '" + this.Name + "'");
								loadfailed = true;
							}
							else
							{
								// Get the patch
								mem.Seek(0, SeekOrigin.Begin);
								Bitmap patchbmp = null;
								try { patchbmp = reader.ReadAsBitmap(mem); }
								catch(InvalidDataException)
								{
									// Data cannot be read!
									General.ErrorLogger.Add(ErrorType.Error, "Patch lump '" + p.lumpname + "' data format could not be read, while loading texture '" + this.Name + "'");
									loadfailed = true;
								}
								if(patchbmp != null)
								{
									// Adjust patch alpha
									if(p.alpha < 1.0f)
									{
										BitmapData bmpdata = null;
										try
										{
											bmpdata = patchbmp.LockBits(new Rectangle(0, 0, patchbmp.Size.Width, patchbmp.Size.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
										}
										catch(Exception e)
										{
											General.ErrorLogger.Add(ErrorType.Error, "Cannot lock image '" + p.lumpname + "' for alpha adjustment. " + e.GetType().Name + ": " + e.Message);
										}

										if(bmpdata != null)
										{
											PixelColor* pixels = (PixelColor*)(bmpdata.Scan0.ToPointer());
											int numpixels = bmpdata.Width * bmpdata.Height;
											for(PixelColor* cp = pixels + numpixels - 1; cp >= pixels; cp--)
											{
												cp->a = (byte)((((float)cp->a * PixelColor.BYTE_TO_FLOAT) * p.alpha) * 255.0f);
											}
											patchbmp.UnlockBits(bmpdata);
										}
									}
									
									// Draw the patch on the texture image
									Rectangle tgtrect = new Rectangle(p.x, p.y, patchbmp.Size.Width, patchbmp.Size.Height);
									g.DrawImageUnscaledAndClipped(patchbmp, tgtrect);
									patchbmp.Dispose();
								}
							}

							// Done
							mem.Dispose();
						}
						else
						{
							// Missing a patch lump!
							General.ErrorLogger.Add(ErrorType.Error, "Missing patch lump '" + p.lumpname + "' while loading texture '" + this.Name + "'");
							loadfailed = true;
						}
					}
				}
				
				// Dispose bitmap if load failed
				if(loadfailed && (bitmap != null))
				{
					bitmap.Dispose();
					bitmap = null;
				}

				// Pass on to base
				base.LocalLoadImage();
			}
		}

		#endregion
	}
}
