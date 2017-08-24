
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
using CodeImp.DoomBuilder.VisualModes;

#endregion

namespace CodeImp.DoomBuilder.BuilderModes
{
	// This doesn't do jack shit.
	internal class NullVisualEventReceiver : IVisualEventReceiver
	{
		public NullVisualEventReceiver()
		{
		}
		
		public void OnSelectBegin()
		{
		}

		public void OnSelectEnd()
		{
		}

		public void OnEditBegin()
		{
		}

		public void OnEditEnd()
		{
		}

		public void OnMouseMove(MouseEventArgs e)
		{
		}

		public void OnChangeTargetHeight(int amount)
		{
		}

		public void OnChangeTargetBrightness(bool up)
		{
		}

		public void OnChangeTextureOffset(int horizontal, int vertical)
		{
		}

		public void OnResetTextureOffset()
		{
		}

		public void OnSelectTexture()
		{
		}

		public void OnCopyTexture()
		{
		}

		public void OnPasteTexture()
		{
		}

		public void OnCopyTextureOffsets()
		{
		}

		public void OnPasteTextureOffsets()
		{
		}

		public void OnCopyProperties()
		{
		}

		public void OnPasteProperties()
		{
		}

		public void OnTextureAlign(bool alignx, bool aligny)
		{
		}

		public void OnTextureFloodfill()
		{
		}

		public void OnToggleUpperUnpegged()
		{
		}

		public void OnToggleLowerUnpegged()
		{
		}

		public void OnProcess(long deltatime)
		{
		}

		public void OnInsert()
		{
		}

		public void OnDelete()
		{
		}

		public void ApplyTexture(string texture)
		{
		}

		public void ApplyUpperUnpegged(bool set)
		{
		}

		public void ApplyLowerUnpegged(bool set)
		{
		}

		public string GetTextureName()
		{
			return "";
		}
	}
}
