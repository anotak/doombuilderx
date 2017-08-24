
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
	public class ResultUnknownTexture : ErrorResult
	{
		#region ================== Variables
		
		private Sidedef side;
		private SidedefPart part;
		
		#endregion
		
		#region ================== Properties

		public override int Buttons { get { return 2; } }
		public override string Button1Text { get { return "Remove Texture"; } }
		public override string Button2Text { get { return "Add Default Texture"; } }
		
		#endregion
		
		#region ================== Constructor / Destructor
		
		// Constructor
		public ResultUnknownTexture(Sidedef sd, SidedefPart part)
		{
			// Initialize
			this.side = sd;
			this.part = part;
			this.viewobjects.Add(sd);
			this.description = "This sidedef uses an unknown texture. This could be the result of missing resources, or a mistyped texture name. Click the Remove Texture button to remove the texture or click on Add Default Texture to use a known texture instead.";
		}
		
		#endregion
		
		#region ================== Methods
		
		// This must return the string that is displayed in the listbox
		public override string ToString()
		{
			switch(part)
			{
				case SidedefPart.Upper:
					return "Sidedef has unknown upper texture \"" + side.HighTexture + "\"";
					
				case SidedefPart.Middle:
					return "Sidedef has unknown middle texture \"" + side.MiddleTexture + "\"";
					
				case SidedefPart.Lower:
					return "Sidedef has unknown lower texture \"" + side.LowTexture + "\"";
					
				default:
					return "ERROR";
			}
		}
		
		// Rendering
		public override void PlotSelection(IRenderer2D renderer)
		{
			renderer.PlotLinedef(side.Line, General.Colors.Selection);
			renderer.PlotVertex(side.Line.Start, ColorCollection.VERTICES);
			renderer.PlotVertex(side.Line.End, ColorCollection.VERTICES);
		}
		
		// Fix by removing texture
		public override bool Button1Click()
		{
			General.Map.UndoRedo.CreateUndo("Remove unknown texture");
			switch(part)
			{
				case SidedefPart.Upper: side.SetTextureHigh("-"); break;
				case SidedefPart.Middle: side.SetTextureMid("-"); break;
				case SidedefPart.Lower: side.SetTextureLow("-"); break;
			}
			
			General.Map.Map.Update();
			return true;
		}
		
		// Fix by setting default texture
		public override bool Button2Click()
		{
			General.Map.UndoRedo.CreateUndo("Unknown texture correction");
			General.Settings.FindDefaultDrawSettings();
			switch(part)
			{
				case SidedefPart.Upper: side.SetTextureHigh(General.Settings.DefaultTexture); break;
				case SidedefPart.Middle: side.SetTextureMid(General.Settings.DefaultTexture); break;
				case SidedefPart.Lower: side.SetTextureLow(General.Settings.DefaultTexture); break;
			}
			
			General.Map.Map.Update();
			return true;
		}
		
		#endregion
	}
}
