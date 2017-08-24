
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
using System.Windows.Forms;
using CodeImp.DoomBuilder.Map;

#endregion

namespace CodeImp.DoomBuilder.Config
{
	/// <summary>
	/// Option in generalized types.
	/// </summary>
	public class GeneralizedOption
	{
		#region ================== Constants

		#endregion

		#region ================== Variables

		// Properties
		private string name;
		private List<GeneralizedBit> bits;
		
		#endregion

		#region ================== Properties

		public string Name { get { return name; } }
		public List<GeneralizedBit> Bits { get { return bits; } }

		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		internal GeneralizedOption(string structure, string cat, string name, IDictionary bitslist)
		{
			int index;
			string fullpath;
			
			// Determine path
			if(cat.Length > 0) fullpath = structure + "." + cat;
				else fullpath = structure;

			// Initialize
			this.name = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(name);
			this.bits = new List<GeneralizedBit>();

			// Go for all bits
			foreach(DictionaryEntry de in bitslist)
			{
				// Check if the item key is numeric
				if(int.TryParse(de.Key.ToString(), NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite, CultureInfo.InvariantCulture, out index))
				{
					// Add to list
					this.bits.Add(new GeneralizedBit(index, de.Value.ToString()));
				}
				else
				{
					General.ErrorLogger.Add(ErrorType.Warning, "Structure '" + fullpath + "." + name + "' contains invalid entries. The keys must be numeric.");
				}
			}
			
			// Sort the list
			bits.Sort();
			
			// We have no destructor
			GC.SuppressFinalize(this);
		}

		#endregion

		#region ================== Methods

		// This presents the item as string
		public override string ToString()
		{
			return name;
		}
		
		#endregion
	}
}
