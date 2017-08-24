	
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

#endregion

namespace CodeImp.DoomBuilder.Geometry
{
	public struct Angle2D
	{
		#region ================== Constants

		public const float PI = (float)Math.PI;
		public const float PIHALF = (float)Math.PI * 0.5f;
		public const float PI2 = (float)Math.PI * 2f;
		public const float PIDEG = 57.295779513082320876798154814105f;
		public const float SQRT2 = 1.4142135623730950488016887242097f;
		
		#endregion

		#region ================== Methods

		// This converts doom angle to real angle
		public static float DoomToReal(int doomangle)
		{
			return (float)Math.Round(Normalized(DegToRad((float)(doomangle + 90))), 4);
		}

		// This converts real angle to doom angle
		public static int RealToDoom(float realangle)
		{
			return (int)Math.Round(RadToDeg(Normalized(realangle - PIHALF)));
		}

		// This converts degrees to radians
		public static float DegToRad(float deg)
		{
			return deg / PIDEG;
		}

		// This converts radians to degrees
		public static float RadToDeg(float rad)
		{
			return rad * PIDEG;
		}

		// This normalizes an angle
		public static float Normalized(float a)
		{
			while(a < 0f) a += PI2;
			while(a >= PI2) a -= PI2;
			return a;
		}

		// This returns the difference between two angles
		public static float Difference(float a, float b)
		{
			float d;

			// Calculate delta angle
			d = Normalized(a) - Normalized(b);

			// Make corrections for zero barrier
			if(d < 0f) d += PI2;
			if(d > PI) d = PI2 - d;

			// Return result
			return d;
		}
		
		#endregion
	}
}
