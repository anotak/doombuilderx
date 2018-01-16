
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
	public struct Plane
	{
		#region ================== Constants
		
		#endregion
		
		#region ================== Variables
		
		//
		// Plane definition:
		// A * x + B * y + C * z + D = 0
		//
		// A, B, C is the normal
		// D is the offset along the normal (negative)
		//
		private Vector3D normal;
		private float offset;
		
		#endregion
		
		#region ================== Properties
		
		public Vector3D Normal { get { return normal; } }
		public float Offset { get { return offset; } set { offset = value; } }
		public float a { get { return normal.x; } }
		public float b { get { return normal.y; } }
		public float c { get { return normal.z; } }
		public float d { get { return offset; } set { offset = value; } }
		
		#endregion
		
		#region ================== Constructors
		
		/// <summary></summary>
		public Plane(Vector3D normal, float offset)
		{
			#if DEBUG
				if(!normal.IsNormalized())
					General.Fail("Attempt to create a plane with a vector that is not normalized!");
			#endif
			this.normal = normal;
			this.offset = offset;
		}

		/// <summary></summary>
		public Plane(Vector3D normal, Vector3D position)
		{
			#if DEBUG
				if(!normal.IsNormalized())
					General.Fail("Attempt to create a plane with a vector that is not normalized!");
			#endif
			this.normal = normal;
			this.offset = -Vector3D.DotProduct(normal, position);
		}

		/// <summary></summary>
		public Plane(Vector3D p1, Vector3D p2, Vector3D p3, bool up)
		{
			this.normal = Vector3D.CrossProduct(p2 - p1, p3 - p1).GetNormal();
			
			if((up && (this.normal.z < 0.0f)) || (!up && (this.normal.z > 0.0f)))
				this.normal = -this.normal;
			
			this.offset = -Vector3D.DotProduct(normal, p3);
		}
		
		#endregion
		
		#region ================== Methods
		
		/// <summary>
		/// This tests for intersection with a line.
		/// See http://local.wasp.uwa.edu.au/~pbourke/geometry/planeline/
		/// </summary>
		public bool GetIntersection(Vector3D from, Vector3D to, ref float u_ray)
		{
			float w = Vector3D.DotProduct(normal, from - to);
			if(w != 0.0f)
			{
				float v = Vector3D.DotProduct(normal, from);
				u_ray = (offset + v) / w;
				return true;
			}
			else
			{
				return false;
			}
		}
		
		/// <summary>
		/// This returns the smallest distance to the plane and the side on which the point lies.
		/// more than 0 means the point lies on the front of the plane
		/// less than 0 means the point lies behind the plane
		/// See http://mathworld.wolfram.com/Point-PlaneDistance.html
		/// </summary>
		public float Distance(Vector3D p)
		{
			return Vector3D.DotProduct(normal, p) + offset;
		}
		
		/// <summary>
		/// This returns a point on the plane closest to the given point
		/// </summary>
		public Vector3D ClosestOnPlane(Vector3D p)
		{
			float d = this.Distance(p);
			return p - normal * d;
		}

		/// <summary>
		/// This returns Z on the plane at X, Y
		/// </summary>
		public float GetZ(Vector2D pos)
		{
			return (-offset - Vector2D.DotProduct(normal, pos)) / normal.z;
		}

		/// <summary>
		/// This returns Z on the plane at X, Y
		/// </summary>
		public float GetZ(float x, float y)
		{
			return (-offset - (normal.x * x + normal.y * y)) / normal.z;
		}

		/// <summary>
		/// This inverts the plane
		/// </summary>
		public Plane GetInverted()
		{
			return new Plane(-normal, -offset);
		}
		
		#endregion
	}
}
