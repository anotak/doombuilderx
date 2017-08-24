
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
	[EditMode(DisplayName = "Floor Align Mode",
			  SwitchAction = "flooralignmode",
			  ButtonImage = "FloorAlign.png",
			  ButtonOrder = int.MinValue + 210,
			  ButtonGroup = "000_editing",
			  Volatile = true)]

	public class FloorAlignMode : FlatAlignMode
	{
		#region ================== Constants

		#endregion

		#region ================== Variables

		private ViewMode prevviewmode;

		#endregion

		#region ================== Properties

		public override string XScaleName { get { return "xscalefloor"; } }
		public override string YScaleName { get { return "yscalefloor"; } }
		public override string XOffsetName { get { return "xpanningfloor"; } }
		public override string YOffsetName { get { return "ypanningfloor"; } }
		public override string RotationName { get { return "rotationfloor"; } }
		public override string UndoDescription { get { return "Floor Alignment"; } }

		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		public FloorAlignMode()
		{
		}

		#endregion

		#region ================== Methods

		// Get the texture data to align
		protected override ImageData GetTexture(Sector editsector)
		{
			return General.Map.Data.GetFlatImage(editsector.LongFloorTexture);
		}

		#endregion

		#region ================== Events

		// Mode engages
		public override void OnEngage()
		{
			prevviewmode = General.Map.Renderer2D.ViewMode;

			base.OnEngage();
			
			General.Actions.InvokeAction("builder_viewmodefloors");
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
