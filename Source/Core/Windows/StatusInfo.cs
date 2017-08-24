
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
using System.Windows.Forms;
using System.IO;
using System.Reflection;
using System.Drawing;
using System.ComponentModel;
using CodeImp.DoomBuilder.Map;
using SlimDX.Direct3D9;
using SlimDX;
using CodeImp.DoomBuilder.Geometry;
using System.Drawing.Imaging;
using CodeImp.DoomBuilder.Data;
using CodeImp.DoomBuilder.Editing;

#endregion

namespace CodeImp.DoomBuilder.Windows
{
	public struct StatusInfo
	{
		public StatusType type;
		public string message;
		internal bool displayed;
		
		internal StatusInfo(StatusType type, string message)
		{
			this.type = type;
			this.message = message;
			this.displayed = false;
		}
	}
	
	public enum StatusType : int
	{
		/// <summary>
		/// When no particular information is to be displayed. The messages displayed depends on running background processes.
		/// </summary>
		Ready,
		
		/// <summary>
		/// Shows action information and flashes up the status icon once.
		/// </summary>
		Action,
		
		/// <summary>
		/// Shows information without flashing the icon.
		/// </summary>
		Info,
		
		/// <summary>
		/// Shows information with the busy icon.
		/// </summary>
		Busy,
		
		/// <summary>
		/// Shows a warning, makes a warning sound and flashes a warning icon.
		/// </summary>
		Warning
	}
}
