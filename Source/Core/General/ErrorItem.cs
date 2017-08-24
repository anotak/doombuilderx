
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
	internal struct ErrorItem
	{
		public ErrorType type;
		public string message;
		
		internal ErrorItem(ErrorType type, string message)
		{
			this.type = type;
			this.message = message;
		}
	}
	
	public enum ErrorType
	{
		/// <summary>
		/// This indicates a significant error that may cause problems for the data displayed or editing behaviour.
		/// </summary>
		Error,
		
		/// <summary>
		/// This indicates a potential problem.
		/// </summary>
		Warning
	}
}
