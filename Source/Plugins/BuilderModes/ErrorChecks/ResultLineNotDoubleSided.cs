
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
	public class ResultLineNotDoubleSided : ErrorResult
	{
		#region ================== Variables
		
		private Linedef line;
		private int buttons;
		private Sidedef copysidedef;
		
		#endregion
		
		#region ================== Properties

		public override int Buttons { get { return buttons; } }
		public override string Button1Text { get { return "Make Single-Sided"; } }
		public override string Button2Text { get { return "Create Sidedef"; } }
		
		#endregion
		
		#region ================== Constructor / Destructor
		
		// Constructor
		public ResultLineNotDoubleSided(Linedef l)
		{
			// Initialize
			this.line = l;
			this.viewobjects.Add(l);
			this.description = "This linedef is marked as double-sided, but is missing the back sidedef. Click Make Single-Sided to remove the double-sided flag from the line.";
			
			// One solution is to remove the double-sided flag
			buttons = 1;
			
			// Check if the linedef can join a sector on the side where it is missing a sidedef
			bool fixable = false;
			List<LinedefSide> sides = Tools.FindPotentialSectorAt(l, false);
			if(sides != null)
			{
				foreach(LinedefSide sd in sides)
				{
					// If any of the sides lies along a sidedef, then we can copy
					// that sidedef to fix the missing sidedef on this line.
					if(sd.Front && (sd.Line.Front != null))
					{
						copysidedef = sd.Line.Front;
						fixable = true;
						break;
					}
					else if(!sd.Front && (sd.Line.Back != null))
					{
						copysidedef = sd.Line.Back;
						fixable = true;
						break;
					}
				}
			}
			
			// Fixable?
			if(fixable)
			{
				buttons++;
				this.description += " Or click Create Sidedef to rebuild the missing sidedef (making the line really double-sided).";
			}
		}
		
		#endregion
		
		#region ================== Methods
		
		// This must return the string that is displayed in the listbox
		public override string ToString()
		{
			return "Linedef is marked double-sided but has no back side";
		}
		
		// Rendering
		public override void PlotSelection(IRenderer2D renderer)
		{
			renderer.PlotLinedef(line, General.Colors.Selection);
			renderer.PlotVertex(line.Start, ColorCollection.VERTICES);
			renderer.PlotVertex(line.End, ColorCollection.VERTICES);
		}
		
		// Fix by flipping linedefs
		public override bool Button1Click()
		{
			General.Map.UndoRedo.CreateUndo("Linedef flags change");
			line.ApplySidedFlags();
			General.Map.Map.Update();
			return true;
		}
		
		// Fix by creating a sidedef
		public override bool Button2Click()
		{
			General.Map.UndoRedo.CreateUndo("Create back sidedef");
			Sidedef newside = General.Map.Map.CreateSidedef(line, false, copysidedef.Sector);
			if(newside == null) return false;
			copysidedef.CopyPropertiesTo(newside);
			line.ApplySidedFlags();
			General.Map.Map.Update();
			return true;
		}
		
		#endregion
	}
}
