
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
using System.Runtime.InteropServices;
using System.Diagnostics;
using ICSharpCode.SharpZipLib.Checksums;

#endregion

namespace CodeImp.DoomBuilder
{
	public class CRC
	{
		#region ================== Constants

		#endregion

		#region ================== Variables

		private Crc32 crc;

		#endregion

		#region ================== Properties

		public long Value { get { return crc.Value; } }

		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		public CRC()
		{
			crc = new Crc32();
			
			// We have no destructor
			GC.SuppressFinalize(this);
		}

		#endregion

		#region ================== Methods

		public void Add(long value)
		{
			uint lo = (uint)((ulong)value & 0x00000000FFFFFFFF);
			uint hi = (uint)(((ulong)value & 0xFFFFFFFF00000000) >> 32);
			crc.Update(unchecked((int)lo));
			crc.Update(unchecked((int)hi));
		}

		public void Add(int value)
		{
			crc.Update(value);
		}

		public void Add(byte[] data)
		{
			crc.Update(data);
		}

		public void Reset()
		{
			crc.Reset();
		}

		#endregion
	}
}
