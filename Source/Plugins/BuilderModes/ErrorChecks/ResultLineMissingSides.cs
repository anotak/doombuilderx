
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
	public class ResultLineMissingSides : ErrorResult
	{
		#region ================== Variables
		
		private Linedef line;
		private int buttons;
		private Sidedef copysidedeffront;
		private Sidedef copysidedefback;
		
		#endregion
		
		#region ================== Properties

		public override int Buttons { get { return buttons; } }
		public override string Button1Text { get { return "Create One Side"; } }
		public override string Button2Text { get { return "Create Both Sides"; } }
		
		#endregion
		
		#region ================== Constructor / Destructor
		
		// Constructor
		public ResultLineMissingSides(Linedef l)
		{
			List<LinedefSide> sides;
			bool fixable;
			
			// Initialize
			this.line = l;
			this.viewobjects.Add(l);
			this.description = "This linedef is missing front and back sidedefs." +
							   "A line must have at least a front side and optionally a back side!";
			
			buttons = 0;
			
			// Check if we can join a sector on the front side
			fixable = false;
			sides = Tools.FindPotentialSectorAt(l, true);
			if(sides != null)
			{
				foreach(LinedefSide sd in sides)
				{
					// If any of the sides lies along a sidedef, then we can copy
					// that sidedef to fix the missing sidedef on this line.
					if(sd.Front && (sd.Line.Front != null))
					{
						copysidedeffront = sd.Line.Front;
						fixable = true;
						break;
					}
					else if(!sd.Front && (sd.Line.Back != null))
					{
						copysidedeffront = sd.Line.Back;
						fixable = true;
						break;
					}
				}
			}
			
			// Fixable?
			if(fixable)	buttons++;
			
			// Check if we can join a sector on the back side
			fixable = false;
			sides = Tools.FindPotentialSectorAt(l, false);
			if(sides != null)
			{
				foreach(LinedefSide sd in sides)
				{
					// If any of the sides lies along a sidedef, then we can copy
					// that sidedef to fix the missing sidedef on this line.
					if(sd.Front && (sd.Line.Front != null))
					{
						copysidedefback = sd.Line.Front;
						fixable = true;
						break;
					}
					else if(!sd.Front && (sd.Line.Back != null))
					{
						copysidedefback = sd.Line.Back;
						fixable = true;
						break;
					}
				}
			}
			
			// Fixable?
			if(fixable) buttons++;
			
			// Now make a fine description
			switch(buttons)
			{
				case 0: description += " Doom Builder could not find a solution to fix this line."; break;
				case 1: description += " Click Create One Side to rebuild a single sidedef, making this line single-sided."; break;
				case 2: description += " Click Create Both Side to rebuild both sides of the line, making this line double-sided."; break;
			}
		}
		
		#endregion
		
		#region ================== Methods
		
		// This must return the string that is displayed in the listbox
		public override string ToString()
		{
			return "Linedef is missing both sides";
		}
		
		// Rendering
		public override void PlotSelection(IRenderer2D renderer)
		{
			renderer.PlotLinedef(line, General.Colors.Selection);
			renderer.PlotVertex(line.Start, ColorCollection.VERTICES);
			renderer.PlotVertex(line.End, ColorCollection.VERTICES);
		}
		
		// Fix a single side
		public override bool Button1Click()
		{
			// On which side can we fix?
			if(copysidedeffront != null)
			{
				// Front
				General.Map.UndoRedo.CreateUndo("Create front sidedef");
				Sidedef newside = General.Map.Map.CreateSidedef(line, true, copysidedeffront.Sector);
				if(newside == null) return false;
				copysidedeffront.CopyPropertiesTo(newside);
			}
			else if(copysidedefback != null)
			{
				// Back
				// Because the line is single-sided, we make the sidedef on the front.
				// We will then flip it to make sure to ends up in the right position.
				General.Map.UndoRedo.CreateUndo("Create front sidedef");
				Sidedef newside = General.Map.Map.CreateSidedef(line, true, copysidedefback.Sector);
				if(newside == null) return false;
				copysidedefback.CopyPropertiesTo(newside);
				line.FlipVertices();
			}
			
			line.ApplySidedFlags();
			General.Map.Map.Update();
			return true;
		}
		
		// Fix both sides
		public override bool Button2Click()
		{
			Sidedef newside;
			General.Map.UndoRedo.CreateUndo("Create sidedefs");

			// Front
			newside = General.Map.Map.CreateSidedef(line, true, copysidedeffront.Sector);
			if(newside == null) return false;
			copysidedeffront.CopyPropertiesTo(newside);
			
			// Back
			newside = General.Map.Map.CreateSidedef(line, false, copysidedefback.Sector);
			if(newside == null) return false;
			copysidedefback.CopyPropertiesTo(newside);
			
			line.ApplySidedFlags();
			General.Map.Map.Update();
			return true;
		}
		
		#endregion
	}
}
