
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

#endregion

namespace CodeImp.DoomBuilder
{
	public class ErrorLogger
	{
		#region ================== Constants

		#endregion

		#region ================== Variables
		
		private List<ErrorItem> errors;
		private volatile bool changed;
		private volatile bool erroradded;
		private volatile bool warningadded;
		
		#endregion

		#region ================== Properties
		
		public bool HasErrors { get { return (errors.Count > 0); } }
		public bool HasChanged { get { return changed; } set { changed = value; } }
		public bool IsErrorAdded { get { return erroradded; } set { erroradded = value; } }
		public bool IsWarningAdded { get { return warningadded; } set { warningadded = value; } }
		
		#endregion

		#region ================== Constructor / Disposer
		
		// Constructor
		internal ErrorLogger()
		{
			errors = new List<ErrorItem>();
		}
		
		#endregion
		
		#region ================== Methods

		// This clears the errors
		public void Clear()
		{
			lock(this)
			{
				changed = false;
				erroradded = false;
				warningadded = false;
				errors.Clear();
			}
		}
		
		// This adds a new error
		public void Add(ErrorType type, string message)
		{
			string prefix = "";
			
			lock(this)
			{
				errors.Add(new ErrorItem(type, message));
				switch(type)
				{
					case ErrorType.Error:
						erroradded = true;
						prefix = "ERROR: ";
						break;
						
					case ErrorType.Warning:
						warningadded = true;
						prefix = "WARNING: ";
						break;
				}
				changed = true;
				Logger.WriteLogLine(prefix + message);
			}
		}
		
		// This returns the list of errors
		internal List<ErrorItem> GetErrors()
		{
			lock(this)
			{
				List<ErrorItem> copylist = new List<ErrorItem>(errors);
				return copylist;
			}
		}
		
		#endregion
	}
}
