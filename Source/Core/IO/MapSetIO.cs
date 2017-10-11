
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
using System.Reflection;
using System.Diagnostics;

#endregion

namespace CodeImp.DoomBuilder.IO
{
	internal abstract class MapSetIO : IMapSetIO
	{
		#region ================== Constants

		#endregion

		#region ================== Variables

		// WAD File
		protected WAD wad;

		// Map manager
		protected MapManager manager;

		#endregion

		#region ================== Properties

		public abstract int MaxSidedefs { get; }
		public abstract int MaxVertices { get; }
		public abstract int MaxLinedefs { get; }
		public abstract int MaxSectors { get; }
		public abstract int MaxThings { get; }
		public abstract int MinTextureOffset { get; }
		public abstract int MaxTextureOffset { get; }
		public abstract int VertexDecimals { get; }
        public abstract float MinLineLength { get; } // ano - related to VertexDecimals
		public abstract string DecimalsFormat { get; }
		public abstract bool HasLinedefTag { get; }
		public abstract bool HasThingTag { get; }
		public abstract bool HasThingAction { get; }
		public abstract bool HasCustomFields { get; }
		public abstract bool HasThingHeight { get; }
		public abstract bool HasActionArgs { get; }
		public abstract bool HasMixedActivations { get; }
		public abstract bool HasPresetActivations { get; }
		public abstract bool HasBuiltInActivations { get; }
		public abstract bool HasNumericLinedefFlags { get; }
		public abstract bool HasNumericThingFlags { get; }
		public abstract bool HasNumericLinedefActivations { get; }
		public abstract int MaxTag { get; }
		public abstract int MinTag { get; }
		public abstract int MaxAction { get; }
		public abstract int MinAction { get; }
		public abstract int MaxArgument { get; }
		public abstract int MinArgument { get; }
		public abstract int MaxEffect { get; }
		public abstract int MinEffect { get; }
		public abstract int MaxBrightness { get; }
		public abstract int MinBrightness { get; }
		public abstract int MaxThingType { get; }
		public abstract int MinThingType { get; }
		public abstract double MaxCoordinate { get; }
		public abstract double MinCoordinate { get; }
		public abstract int MaxThingAngle { get; }
		public abstract int MinThingAngle { get; }
		
		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		internal MapSetIO(WAD wad, MapManager manager)
		{
			// Initialize
			this.wad = wad;
			this.manager = manager;
		}
		
		#endregion

		#region ================== Static Methods

		// This returns and instance of the specified IO class
		public static MapSetIO Create(string classname)
		{
			return Create(classname, null, null);
		}
		
		// This returns and instance of the specified IO class
		public static MapSetIO Create(string classname, WAD wadfile, MapManager manager)
		{
			object[] args;
			MapSetIO result;
			string fullname;
			
			try
			{
				// Create arguments
				args = new object[2];
				args[0] = wadfile;
				args[1] = manager;
				
				// Make the full class name
				fullname = "CodeImp.DoomBuilder.IO." + classname;
				
				// Create IO class
				result = (MapSetIO)General.ThisAssembly.CreateInstance(fullname, false,
					BindingFlags.Default, null, args, CultureInfo.CurrentCulture, new object[0]);
				
				// Check result
				if(result != null)
				{
					// Success
					return result;
				}
				else
				{
					// No such class
					throw new ArgumentException("No such map format interface found: \"" + classname + "\"");
				}
			}
			// Catch errors
			catch(TargetInvocationException e)
			{
				// Throw the actual exception
				Debug.WriteLine(DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString());
				Debug.WriteLine(e.InnerException.Source + " throws " + e.InnerException.GetType().Name + ":");
				Debug.WriteLine(e.InnerException.Message);
				Debug.WriteLine(e.InnerException.StackTrace);
				throw e.InnerException;
			}
		}
		
		#endregion
		
		#region ================== Methods

		// Required implementations
		public abstract MapSet Read(MapSet map, string mapname);
		public abstract void Write(MapSet map, string mapname, int position);
		
		#endregion
	}
}
