
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
using CodeImp.DoomBuilder.Data;
using CodeImp.DoomBuilder.Windows;
using CodeImp.DoomBuilder.IO;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Rendering;
using CodeImp.DoomBuilder.Geometry;
using CodeImp.DoomBuilder.Editing;
using System.Drawing;
using CodeImp.DoomBuilder.Actions;

#endregion

namespace CodeImp.DoomBuilder.GZDoomEditing
{
	[EditMode(DisplayName = "Ceiling Align Mode",
			  SwitchAction = "ceilingalignmode",
			  ButtonImage = "CeilingAlign.png",
			  ButtonOrder = int.MinValue + 211,
			  ButtonGroup = "000_editing",
			  Volatile = true)]

	public class CeilingAlignMode : FlatAlignMode
	{
		#region ================== Constants

		#endregion

		#region ================== Variables

		private ViewMode prevviewmode;

		#endregion

		#region ================== Properties

		public override string XScaleName { get { return "xscaleceiling"; } }
		public override string YScaleName { get { return "yscaleceiling"; } }
		public override string XOffsetName { get { return "xpanningceiling"; } }
		public override string YOffsetName { get { return "ypanningceiling"; } }
		public override string RotationName { get { return "rotationceiling"; } }
		public override string UndoDescription { get { return "Ceiling Alignment"; } }
		
		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		public CeilingAlignMode()
		{
		}

		#endregion

		#region ================== Methods

		// Get the texture data to align
		protected override ImageData GetTexture(Sector editsector)
		{
			return General.Map.Data.GetFlatImage(editsector.LongCeilTexture);
		}

		#endregion

		#region ================== Events

		// Mode engages
		public override void OnEngage()
		{
			prevviewmode = General.Map.Renderer2D.ViewMode;

			base.OnEngage();
			
			General.Actions.InvokeAction("builder_viewmodeceilings");
		}

		// Mode disengages
		public override void OnDisengage()
		{
			switch(prevviewmode)
			{
				case ViewMode.Normal: General.Actions.InvokeAction("builder_viewmodenormal"); break;
				case ViewMode.FloorTextures: General.Actions.InvokeAction("builder_viewmodefloors"); break;
				case ViewMode.CeilingTextures: General.Actions.InvokeAction("builder_viewmodeceilings"); break;
				case ViewMode.Brightness: General.Actions.InvokeAction("builder_viewmodebrightness"); break;
			}
			
			base.OnDisengage();
		}

		#endregion
	}
}
