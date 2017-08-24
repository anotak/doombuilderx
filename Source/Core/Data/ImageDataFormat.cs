
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
using System.Drawing.Imaging;
using CodeImp.DoomBuilder.IO;

#endregion

namespace CodeImp.DoomBuilder.Data
{
	internal static class ImageDataFormat
	{
		// Input guess formats
		public const int UNKNOWN = 0;			// No clue.
		public const int DOOMPICTURE = 1;		// Could be Doom Picture format	(column list rendered data)
		public const int DOOMFLAT = 2;			// Could be Doom Flat format	(raw 8-bit pixel data)
		public const int DOOMCOLORMAP = 3;		// Could be Doom Colormap format (raw 8-bit pixel palette mapping)
		
		// File format signatures
		private static readonly int[] PNG_SIGNATURE = new int[] { 137, 80, 78, 71, 13, 10, 26, 10 };
		private static readonly int[] GIF_SIGNATURE = new int[] { 71, 73, 70 };
		private static readonly int[] BMP_SIGNATURE = new int[] { 66, 77 };
		private static readonly int[] DDS_SIGNATURE = new int[] { 68, 68, 83, 32 };

		// This check image data and returns the appropriate image reader
		public static IImageReader GetImageReader(Stream data, int guessformat, Playpal palette)
		{
			BinaryReader bindata = new BinaryReader(data);
			DoomPictureReader picreader;
			DoomFlatReader flatreader;
			DoomColormapReader colormapreader;
			
			// First check the formats that provide the means to 'ensure' that
			// it actually is that format. Then guess the Doom image format.

			// Data long enough to check for signatures?
			if(data.Length > 10)
			{
				// Check for PNG signature
				data.Seek(0, SeekOrigin.Begin);
				if(CheckSignature(data, PNG_SIGNATURE)) return new FileImageReader();

				// Check for DDS signature
				data.Seek(0, SeekOrigin.Begin);
				if(CheckSignature(data, DDS_SIGNATURE)) return new FileImageReader();

				// Check for GIF signature
				data.Seek(0, SeekOrigin.Begin);
				if(CheckSignature(data, GIF_SIGNATURE)) return new FileImageReader();

				// Check for BMP signature
				data.Seek(0, SeekOrigin.Begin);
				if(CheckSignature(data, BMP_SIGNATURE))
				{
					// Check if data size matches the size specified in the data
					if(bindata.ReadUInt32() <= data.Length) return new FileImageReader();
				}
			}
			
			// Could it be a doom picture?
			if(guessformat == DOOMPICTURE)
			{
				// Check if data is valid for a doom picture
				data.Seek(0, SeekOrigin.Begin);
				picreader = new DoomPictureReader(palette);
				if(picreader.Validate(data)) return picreader;
			}
			// Could it be a doom flat?
			else if(guessformat == DOOMFLAT)
			{
				// Check if data is valid for a doom flat
				data.Seek(0, SeekOrigin.Begin);
				flatreader = new DoomFlatReader(palette);
				if(flatreader.Validate(data)) return flatreader;
			}
			// Could it be a doom colormap?
			else if(guessformat == DOOMCOLORMAP)
			{
				// Check if data is valid for a doom colormap
				data.Seek(0, SeekOrigin.Begin);
				colormapreader = new DoomColormapReader(palette);
				if(colormapreader.Validate(data)) return colormapreader;
			}
			
			// Format not supported
			return new UnknownImageReader();
		}

		// This checks a signature as byte array
		// NOTE: Expects the stream position to be at the start of the
		// signature, and expects the stream to be long enough.
		private static bool CheckSignature(Stream data, int[] sig)
		{
			int b;
			
			// Go for all bytes
			for(int i = 0; i < sig.Length; i++)
			{
				// When byte doesnt match the signature, leave
				b = data.ReadByte();
				if(b != sig[i]) return false;
			}

			// Signature matches
			return true;
		}
	}
}
