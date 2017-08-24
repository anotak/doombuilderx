
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
	internal unsafe class DoomPictureReader : IImageReader
	{
		#region ================== Variables

		// Palette to use
		private Playpal palette;
		
		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		public DoomPictureReader(Playpal palette)
		{
			// Initialize
			this.palette = palette;
			
			// We have no destructor
			GC.SuppressFinalize(this);
		}

		#endregion

		#region ================== Methods
		
		// This validates the data as doom picture
		public bool Validate(Stream stream)
		{
			BinaryReader reader = new BinaryReader(stream);
			int width, height;
			int dataoffset;
			int datalength;
			int columnaddr;
			
			// Initialize
			dataoffset = (int)stream.Position;
			datalength = (int)stream.Length - (int)stream.Position;

			// Need at least 4 bytes
			if(datalength < 4) return false;

			// Read size and offset
			width = reader.ReadInt16();
			height = reader.ReadInt16();
			reader.ReadInt16();
			reader.ReadInt16();

			// Valid width and height?
			if((width <= 0) || (height <= 0)) return false;
			
			// Go for all columns
			for(int x = 0; x < width; x++)
			{
				// Get column address
				columnaddr = reader.ReadInt32();
				
				// Check if address is outside valid range
				if((columnaddr < (8 + width * 4)) || (columnaddr >= datalength)) return false;
			}

			// Return success
			return true;
		}
		
		// This creates a Bitmap from the given data
		// Returns null on failure
		public Bitmap ReadAsBitmap(Stream stream)
		{
			int x, y;
			return ReadAsBitmap(stream, out x, out y);
		}
		
		// This creates a Bitmap from the given data
		// Returns null on failure
		public Bitmap ReadAsBitmap(Stream stream, out int offsetx, out int offsety)
		{
			BitmapData bitmapdata;
			PixelColorBlock pixeldata;
			PixelColor* targetdata;
			int width, height;
			Bitmap bmp;

			// Read pixel data
			pixeldata = ReadAsPixelData(stream, out width, out height, out offsetx, out offsety);
			if(pixeldata != null)
			{
				// Create bitmap and lock pixels
				try
				{
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
					General.ErrorLogger.Add(ErrorType.Error, "Unable to make Doom picture data. " + e.GetType().Name + ": " + e.Message);
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
		public void DrawToPixelData(Stream stream, PixelColor* target, int targetwidth, int targetheight, int x, int y)
		{
			PixelColorBlock pixeldata;
			int width, height, ox, oy, tx, ty;

			// Read pixel data
			pixeldata = ReadAsPixelData(stream, out width, out height, out ox, out oy);
			if(pixeldata != null)
			{
				// Go for all source pixels
				// We don't care about the original image offset, so reuse ox/oy
				for(ox = 0; ox < width; ox++)
				{
					for(oy = 0; oy < height; oy++)
					{
						// Copy this pixel?
						if(pixeldata.Pointer[oy * width + ox].a > 0.5f)
						{
							// Calculate target pixel and copy when within bounds
							tx = x + ox;
							ty = y + oy;
							if((tx >= 0) && (tx < targetwidth) && (ty >= 0) && (ty < targetheight))
								target[ty * targetwidth + tx] = pixeldata.Pointer[oy * width + ox];
						}
					}
				}
			}
		}

		// This creates pixel color data from the given data
		// Returns null on failure
		private PixelColorBlock ReadAsPixelData(Stream stream, out int width, out int height, out int offsetx, out int offsety)
		{
			BinaryReader reader = new BinaryReader(stream);
			PixelColorBlock pixeldata = null;
			int y, read_y, count, p;
			int[] columns;
			int dataoffset;
			
			// Initialize
			width = 0;
			height = 0;
			offsetx = 0;
			offsety = 0;
			dataoffset = (int)stream.Position;

			// Need at least 4 bytes
			if((stream.Length - stream.Position) < 4) return null;
			
			#if !DEBUG
			try
			{
			#endif
			
			// Read size and offset
			width = reader.ReadInt16();
			height = reader.ReadInt16();
			offsetx = reader.ReadInt16();
			offsety = reader.ReadInt16();
			
			// Valid width and height?
			if((width <= 0) || (height <= 0)) return null;
			
			// Read the column addresses
			columns = new int[width];
			for(int x = 0; x < width; x++) columns[x] = reader.ReadInt32();
			
			// Allocate memory
			pixeldata = new PixelColorBlock(width, height);
			pixeldata.Clear();
			
			// Go for all columns
			for(int x = 0; x < width; x++)
			{
				// Seek to column start
				stream.Seek(dataoffset + columns[x], SeekOrigin.Begin);
				
				// Read first post start
				y = reader.ReadByte();
				read_y = y;
				
				// Continue while not end of column reached
				while(read_y < 255)
				{
					// Read number of pixels in post
					count = reader.ReadByte();

					// Skip unused pixel
					stream.Seek(1, SeekOrigin.Current);

					// Draw post
					for(int yo = 0; yo < count; yo++)
					{
						// Read pixel color index
						p = reader.ReadByte();

						// Draw pixel
						pixeldata.Pointer[(y + yo) * width + x] = palette[p];
					}
					
					// Skip unused pixel
					stream.Seek(1, SeekOrigin.Current);

					// Read next post start
					read_y = reader.ReadByte();
					if(read_y < y) y += read_y; else y = read_y;
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
