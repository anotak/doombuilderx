
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
	internal interface IVisualEventReceiver
	{
		// The events that must be handled
		void OnSelectBegin();
		void OnSelectEnd();
		void OnEditBegin();
		void OnEditEnd();
		void OnMouseMove(MouseEventArgs e);
		void OnChangeTargetHeight(int amount);
		void OnChangeTargetBrightness(bool up);
		void OnChangeTextureOffset(int horizontal, int vertical);
		void OnResetTextureOffset();
		void OnSelectTexture();
		void OnCopyTexture();
		void OnPasteTexture();
		void OnCopyTextureOffsets();
		void OnPasteTextureOffsets();
		void OnCopyProperties();
		void OnPasteProperties();
		void OnTextureAlign(bool alignx, bool aligny);
		void OnTextureFloodfill();
		void OnToggleUpperUnpegged();
		void OnToggleLowerUnpegged();
		void OnProcess(long deltatime);
		void OnInsert();
		void OnDelete();

		// Assist functions
		void ApplyTexture(string texture);
		void ApplyUpperUnpegged(bool set);
		void ApplyLowerUnpegged(bool set);
		
		// Other methods
		string GetTextureName();
	}
}
