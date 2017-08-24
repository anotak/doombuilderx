
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
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Reflection;

#endregion

namespace CodeImp.DoomBuilder.Actions
{
	/// <summary>
	/// This binds a method to an action which is then called when the action is started.
	/// </summary>
	public class BeginActionAttribute : ActionAttribute
	{
		/// <summary>
		/// This binds a method to an action which is then called when the action is started.
		/// </summary>
		/// <param name="action">The action name as defined in Actions.cfg resource.</param>
		public BeginActionAttribute(string action) : base(action)
		{
		}
	}
}
