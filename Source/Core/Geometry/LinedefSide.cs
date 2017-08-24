
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
using CodeImp.DoomBuilder.Geometry;
using CodeImp.DoomBuilder.Rendering;
using SlimDX.Direct3D9;
using System.Drawing;
using CodeImp.DoomBuilder.Map;

#endregion

namespace CodeImp.DoomBuilder.Geometry
{
	/// <summary>
	/// This is used to indicate a side of a line without the need for a sidedef.
	/// </summary>
	public sealed class LinedefSide
	{
		#region ================== Constants

		#endregion

		#region ================== Variables

		private Linedef line;
		private bool front;
		
		#endregion

		#region ================== Properties

		public Linedef Line { get { return line; } set { line = value; } }
		public bool Front { get { return front; } set { front = value; } }

		#endregion

		#region ================== Constructor / Disposer

		/// <summary>
		/// This is used to indicate a side of a line without the need for a sidedef.
		/// </summary>
		public LinedefSide()
		{
			// Initialize

		}

		/// <summary>
		/// This is used to indicate a side of a line without the need for a sidedef.
		/// </summary>
		public LinedefSide(Linedef line, bool front)
		{
			// Initialize
			this.line = line;
			this.front = front;
		}

		/// <summary>
		/// This makes a copy of the linedef side.
		/// </summary>
		public LinedefSide(LinedefSide original)
		{
			// Initialize
			this.line = original.line;
			this.front = original.front;
		}

		// Destructor
		~LinedefSide()
		{
		}

		#endregion

		#region ================== Methods

		// This compares a linedef side
		public static bool operator ==(LinedefSide a, LinedefSide b)
		{
			if((object.Equals(a, null)) && (object.Equals(b, null))) return true;
			if((!object.Equals(a, null)) && (object.Equals(b, null))) return false;
			if((object.Equals(a, null)) && (!object.Equals(b, null))) return false;
			return (a.line == b.line) && (a.front == b.front);
		}

		// This compares a linedef side
		public static bool operator !=(LinedefSide a, LinedefSide b)
		{
			if((object.Equals(a, null)) && (object.Equals(b, null))) return false;
			if((!object.Equals(a, null)) && (object.Equals(b, null))) return true;
			if((object.Equals(a, null)) && (!object.Equals(b, null))) return true;
			return (a.line != b.line) || (a.front != b.front);
		}

		#endregion
	}
}
