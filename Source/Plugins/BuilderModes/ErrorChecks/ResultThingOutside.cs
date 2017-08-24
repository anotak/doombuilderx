
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
using CodeImp.DoomBuilder.Windows;
using CodeImp.DoomBuilder.IO;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Rendering;
using CodeImp.DoomBuilder.Geometry;
using CodeImp.DoomBuilder.Editing;
using CodeImp.DoomBuilder.Actions;
using CodeImp.DoomBuilder.Types;
using CodeImp.DoomBuilder.Config;

#endregion

namespace CodeImp.DoomBuilder.BuilderModes
{
	public class ResultThingOutside : ErrorResult
	{
		#region ================== Variables

		private Thing thing;

		#endregion

		#region ================== Properties
		
		public override int Buttons { get { return 1; } }
		public override string Button1Text { get { return "Delete Thing"; } }
		
		#endregion

		#region ================== Constructor / Destructor

		// Constructor
		public ResultThingOutside(Thing t)
		{
			// Initialize
			this.thing = t;
			this.viewobjects.Add(t);
			this.description = "This thing is completely outside the map.";
		}

		#endregion

		#region ================== Methods

		// This must return the string that is displayed in the listbox
		public override string ToString()
		{
			return General.Map.Data.GetThingInfo(thing.Type).Title + " is outside the map at " + thing.Position.x + ", " + thing.Position.y;
		}

		// Rendering
		public override void  RenderOverlaySelection(IRenderer2D renderer)
		{
			renderer.RenderThing(thing, renderer.DetermineThingColor(thing), 1.0f);
		}
		
		// This removes the thing
		public override bool Button1Click()
		{
			General.Map.UndoRedo.CreateUndo("Delete thing");
			thing.Dispose();
			General.Map.IsChanged = true;
			General.Map.ThingsFilter.Update();
			return true;
		}
		
		#endregion
	}
}
