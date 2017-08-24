
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
using System.IO;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Geometry;
using System.Drawing;
using CodeImp.DoomBuilder.Data;
using CodeImp.DoomBuilder.Rendering;
using System.Drawing.Imaging;

#endregion

namespace CodeImp.DoomBuilder.IO
{
	internal unsafe class DoomColormapReader : IImageReader
	{
		#region ================== Variables

		// Palette to use
		private Playpal palette;

		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		public DoomColormapReader(Playpal palette)
		{
			// Initialize
			this.palette = palette;

			// We have no destructor
			GC.SuppressFinalize(this);
		}

		#endregion

		#region ================== Methods

		// This validates the data as doom flat
		public bool Validate(Stream stream)
		{
			int remainder;
			
			// Check if the data can be divided by 256 (each palette is 256 bytes)
			remainder = (int)stream.Length % 256;
			if(remainder == 0)
			{
				// Success when not 0
				return (stream.Length > 0);
			}
			
			// Format invalid
			return false;
		}

		// This creates a Bitmap from the given data
		// Returns null on failure
		public Bitmap ReadAsBitmap(Stream stream, out int offsetx, out int offsety)
		{
			offsetx = int.MinValue;
			offsety = int.MinValue;
			return ReadAsBitmap(stream);
		}

		// This creates a Bitmap from the given data
		// Returns null on failure
		public Bitmap ReadAsBitmap(Stream stream)
		{
			BitmapData bitmapdata;
			PixelColorBlock pixeldata;
			PixelColor* targetdata;
			int width, height;
			Bitmap bmp;

			// Read pixel data
			pixeldata = ReadAsPixelData(stream, out width, out height);
			if(pixeldata != null)
			{
				try
				{
					// Create bitmap and lock pixels
					bmp = new Bitmap(width, height, PixelFormat.Format32bppArgb);
					bitmapdata = bmp.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
					targetdata = (PixelColor*)bitmapdata.Scan0.ToPointer();

					// Copy the pixels
					General.CopyMemory(targetdata, pixeldata.Pointer, (uint)(width * height * sizeof(PixelColor)));

					// Done
					bmp.UnlockBits(bitmapdata);
				}
				catch(Exception e)
				{
					// Unable to make bitmap
					General.ErrorLogger.Add(ErrorType.Error, "Unable to make Doom flat data. " + e.GetType().Name + ": " + e.Message);
					return null;
				}
			}
			else
			{
				// Failed loading picture
				bmp = null;
			}

			// Return result
			return bmp;
		}

		// This draws the picture to the given pixel color data
		// Throws exception on failure
		public unsafe void DrawToPixelData(Stream stream, PixelColor* target, int targetwidth, int targetheight, int x, int y)
		{
			Bitmap bmp;
			BitmapData bmpdata;
			PixelColor* pixels;
			int ox, oy, tx, ty;
			int width, height;

			// Get bitmap
			bmp = ReadAsBitmap(stream);
			width = bmp.Size.Width;
			height = bmp.Size.Height;

			// Lock bitmap pixels
			bmpdata = bmp.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
			pixels = (PixelColor*)bmpdata.Scan0.ToPointer();

			// Go for all pixels in the original image
			for(ox = 0; ox < width; ox++)
			{
				for(oy = 0; oy < height; oy++)
				{
					// Copy this pixel?
					if(pixels[oy * width + ox].a > 0.5f)
					{
						// Calculate target pixel and copy when within bounds
						tx = x + ox;
						ty = y + oy;
						if((tx >= 0) && (tx < targetwidth) && (ty >= 0) && (ty < targetheight))
							target[ty * targetwidth + tx] = pixels[oy * width + ox];
					}
				}
			}

			// Done
			bmp.UnlockBits(bmpdata);
			bmp.Dispose();
		}

		// This creates pixel color data from the given data
		// Returns null on failure
		private PixelColorBlock ReadAsPixelData(Stream stream, out int width, out int height)
		{
			BinaryReader reader = new BinaryReader(stream);
			PixelColorBlock pixeldata = null;
			byte[] bytes;
			
			// Image will be 128x128
			width = 128;
			height = 128;

#if !DEBUG
			try
			{
#endif

			// Allocate memory
			pixeldata = new PixelColorBlock(width, height);
			pixeldata.Clear();

			// Read flat bytes from stream
			bytes = new byte[width * height];
			stream.Read(bytes, 0, width * height);

			// Draw blocks using the palette
			// We want to draw 8x8 blocks for each color
			// 16 wide and 16 high
			uint i = 0;
			for(int by = 0; by < 16; by++)
			{
				for(int bx = 0; bx < 16; bx++)
				{
					PixelColor bc = palette[bytes[i++]];
					PixelColor bc1 = General.Colors.CreateBrightVariant(palette[bytes[i++]]);
					PixelColor bc2 = General.Colors.CreateDarkVariant(palette[bytes[i++]]);
					for(int py = 0; py < 8; py++)
					{
						for(int px = 0; px < 8; px++)
						{
							int p = ((by * 8) + py) * width + (bx * 8) + px;
							
							// We make the borders slightly brighter and darker
							if((py == 0) || (px == 0))
								pixeldata.Pointer[p] = bc1;
							else if((py == 7) || (px  == 7))
								pixeldata.Pointer[p] = bc2;
							else
								pixeldata.Pointer[p] = bc;
						}
					}
				}
			}
			
			// Return pointer
			return pixeldata;

#if !DEBUG
			}
			catch(Exception)
			{
				// Return nothing
				return null;
			}
#endif
		}

		#endregion

	}
}
