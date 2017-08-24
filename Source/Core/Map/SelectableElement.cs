
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

#endregion

namespace CodeImp.DoomBuilder.Map
{
	public abstract class SelectableElement : MapElement
	{
		#region ================== Constants
		
		#endregion
		
		#region ================== Variables
		
		// Selected or not?
		private bool selected;
		
		// Group bitmask
		private int groups;
		
		#endregion
		
		#region ================== Properties

		public bool Selected { get { return selected; } set { if(value && !selected) DoSelect(); else if(!value && selected) DoUnselect(); } }
		
		#endregion
		
		#region ================== Constructor / Disposer
		
		// Constructor
		internal SelectableElement()
		{
		}
		
		// Disposer
		public override void Dispose()
		{
			// Remove from selection
			if(selected) Selected = false;
			
			// Done
			base.Dispose();
		}
		
		#endregion
		
		#region ================== Methods

		// This makes the selection
		protected virtual void DoSelect()
		{
			selected = true;
		}

		// This removes the selection
		protected virtual void DoUnselect()
		{
			selected = false;
		}
		
		// This copies properties to any other element
		public void CopyPropertiesTo(SelectableElement element)
		{
			element.groups = this.groups;
			element.Selected = this.selected;
			base.CopyPropertiesTo(element);
		}
		
		// This adds the element to one or more groups
		public void AddToGroup(int groupsmask)
		{
			groups |= groupsmask;
		}
		
		// This removes the elements from one or more groups
		public void RemoveFromGroup(int groupsmask)
		{
			groups &= ~groupsmask;
		}
		
		// This selects by group
		public void SelectByGroup(int groupsmask)
		{
			this.Selected = ((groups & groupsmask) != 0);
		}
		
		#endregion
	}
}
