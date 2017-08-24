
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
using System.Reflection;
using System.Drawing;
using SlimDX.Direct3D9;
using System.Runtime.InteropServices;

#endregion

namespace CodeImp.DoomBuilder.Rendering
{
	public unsafe class PixelColorBlock
	{
		#region ================== Variables

		private int width;
		private int height;
		private int memorysize;
		private PixelColor* memory;
		
		#endregion

		#region ================== Properties

		public int Width { get { return width; } }
		public int Height { get { return height; } }
		public int Length { get { return memorysize; } }
		public PixelColor this[int index] { get { return memory[index]; } set { memory[index] = value; } }
		public PixelColor* Pointer { get { return memory; } }

		#endregion

		#region ================== Constructor / Destructor

		// Constructor
		public PixelColorBlock(int width, int height)
		{
			// Check input
			if((width <= 0) || (height <= 0)) throw new ArgumentException("Cannot allocate a memory block of zero size!");
			
			// Initialize
			this.width = width;
			this.height = height;
			this.memorysize = width * height * sizeof(PixelColor);
			this.memory = (PixelColor*)Marshal.AllocCoTaskMem(memorysize);
			if(this.memory == (PixelColor*)0) throw new OutOfMemoryException();
			GC.AddMemoryPressure(memorysize);
		}

		// Destructor
		~PixelColorBlock()
		{
			// Terminate
			Marshal.FreeCoTaskMem(new IntPtr(memory));
			GC.RemoveMemoryPressure(memorysize);
			memorysize = 0;
		}

		#endregion

		#region ================== Methods

		// This clears the memory black
		public void Clear()
		{
			if(memorysize > 0) General.ZeroMemory(new IntPtr(memory), (int)memorysize);
		}

		#endregion
	}
}
