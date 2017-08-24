
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

namespace CodeImp.DoomBuilder.Data
{
	public struct DataLocation : IComparable<DataLocation>, IComparable, IEquatable<DataLocation>
	{
		// Constants
		public const int RESOURCE_WAD = 0;
		public const int RESOURCE_DIRECTORY = 1;
		public const int RESOURCE_PK3 = 2;
		
		// Members
		public int type;
		public string location;
		public bool option1;
		public bool option2;
		public bool notfortesting;
		
		// Constructor
		public DataLocation(int type, string location, bool option1, bool option2, bool notfortesting)
		{
			// Initialize
			this.type = type;
			this.location = location;
			this.option1 = option1;
			this.option2 = option2;
			this.notfortesting = notfortesting;
		}

		// This displays the struct as string
		public override string ToString()
		{
			// Simply show location
			return location;
		}

		// This compares two locations
		public int CompareTo(DataLocation other)
		{
			return string.Compare(this.location, other.location, true);
		}
		
		// This compares two locations
		public int CompareTo(object obj)
		{
			return string.Compare(this.location, ((DataLocation)obj).location, true);
		}
		
		// This compares two locations
		public bool Equals(DataLocation other)
		{
			return (this.CompareTo(other) == 0);
		}
	}
}
