
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
using CodeImp.DoomBuilder.IO;
using CodeImp.DoomBuilder.Data;
using System.IO;
using System.Diagnostics;

#endregion

namespace CodeImp.DoomBuilder
{
	public class StepsList : List<int>
	{
		// This returns a step higher
		public int GetNextHigher(int level)
		{
			int low = 0;
			int high = base.Count - 1;

			while(low < high)
			{
				int mid = (int)Math.Floor((float)(low + high) * 0.5f);
				int l = base[mid];

				if(l <= level)
					low = mid + 1;
				else
					high = mid;
			}
			
			return base[high];
		}
		
		// This returns a step lower
		public int GetNextLower(int level)
		{
			int low = 0;
			int high = base.Count - 1;

			while(low < high)
			{
				int mid = (int)Math.Ceiling((float)(low + high) * 0.5f);
				int l = base[mid];

				if(l >= level)
					high = mid - 1;
				else
					low = mid;
			}

			return base[low];
		}

		// This returns the nearest step
		public int GetNearest(int level)
		{
			int low = 0;
			int high = base.Count - 1;

			while(low < high)
			{
				int mid = (int)Math.Floor((float)(low + high) * 0.5f);
				int l = base[mid];

				if(l <= level)
					low = mid + 1;
				else
					high = mid;
			}

			// Find which one is nearest
			low = (high > 0) ? (high - 1) : 0;
			int dlow = level - base[low];
			int dhigh = base[high] - level;
			return (dlow < dhigh) ? base[low] : base[high];
		}
	}
}
