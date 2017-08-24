
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
using CodeImp.DoomBuilder.Map;

#endregion

namespace CodeImp.DoomBuilder.Geometry
{
	public sealed class LinedefAngleSorter : IComparer<Linedef>
	{
		// Variables
		private Linedef baseline;
		private bool front;
		private Vertex basevertex;

		// Constructor
		public LinedefAngleSorter(Linedef baseline, bool front, Vertex fromvertex)
		{
			// Initialize
			this.baseline = baseline;
			this.basevertex = fromvertex;
			
			// Determine rotation direction
			if(baseline.End == basevertex) this.front = !front; else this.front = front;

			// We have no destructor
			GC.SuppressFinalize(this);
		}

		// This calculates the relative angle between two lines
		private float CalculateRelativeAngle(Linedef a, Linedef b)
		{
			float s, n, ana, anb;
			Vector2D va, vb;
			
			// Determine angles
			ana = a.Angle; if(a.End == basevertex) ana += Angle2D.PI;
			anb = b.Angle; if(b.End == basevertex) anb += Angle2D.PI;
			
			// Take the difference from angles
			n = Angle2D.Difference(ana, anb);
			
			// Get line end vertices a and b that are not connected to basevertex
			if(a.Start == basevertex) va = a.End.Position; else va = a.Start.Position;
			if(b.Start == basevertex) vb = b.End.Position; else vb = b.Start.Position;
			
			// Check to which side the angle goes and adjust angle as needed
			s = Line2D.GetSideOfLine(va, vb, basevertex.Position);
			if(((s < 0) && front) || ((s > 0) && !front)) n = Angle2D.PI2 - n;
			
			// Return result
			return n;
		}
		
		// Comparer
		public int Compare(Linedef x, Linedef y)
		{
			float ax, ay;
			
			// Calculate angles
			ax = CalculateRelativeAngle(baseline, x);
			ay = CalculateRelativeAngle(baseline, y);
			
			// Compare results
			/*
			if(ax < ay) return 1;
			else if(ax > ay) return -1;
			else return 0;
			*/
			return Math.Sign(ay - ax);
		}
	}
}
