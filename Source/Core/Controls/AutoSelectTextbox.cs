
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
using System.Globalization;
using System.Windows.Forms;
using CodeImp.DoomBuilder.Actions;
using CodeImp.DoomBuilder.Geometry;
using CodeImp.DoomBuilder.Rendering;
using CodeImp.DoomBuilder.Editing;

#endregion

namespace CodeImp.DoomBuilder.Controls
{
	public class AutoSelectTextbox : TextBox
	{
		#region ================== Constants

		#endregion

		#region ================== Variables

		private int eventcount = 0;
		
		#endregion

		#region ================== Properties

		#endregion

		#region ================== Constructor / Disposer

		#endregion

		#region ================== Methods

		// When gaining focus
		protected override void OnGotFocus(EventArgs e)
		{
			base.OnGotFocus(e);

			// Reset counter
			eventcount = 0;
		}
		
		// When losing focus
		protected override void OnLostFocus(EventArgs e)
		{
			base.OnLostFocus(e);

			// Reset counter
			eventcount = 0;
		}
		
		// When mouse pressed down
		protected override void OnMouseDown(MouseEventArgs e)
		{
			base.OnMouseDown(e);

			// Select all text when this mouseclick gives focus
			if(eventcount == 0) this.SelectAll();
			eventcount++;
		}

		// When key is pressed
		protected override void OnKeyDown(KeyEventArgs e)
		{
			base.OnKeyDown(e);
			eventcount++;
		}
		
		#endregion
	}
}
