
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
using System.Windows.Forms;
using Microsoft.Win32;
using System.Diagnostics;
using CodeImp.DoomBuilder.Actions;
using CodeImp.DoomBuilder.Data;
using CodeImp.DoomBuilder.Config;
using CodeImp.DoomBuilder.Rendering;
using SlimDX.Direct3D9;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Windows;

#endregion

namespace CodeImp.DoomBuilder.Controls
{
	public class TextureSelectorControl : ImageSelectorControl
	{
		// Variables
		private bool required;
		
		// Properties
		public bool Required { get { return required; } set { required = value; } }

		// Setup
		public override void Initialize()
		{
			base.Initialize();
			
			// Fill autocomplete list
			name.AutoCompleteCustomSource.AddRange(General.Map.Data.TextureNames.ToArray());
			allowclear = true;
		}
		
		// This finds the image we need for the given texture name
		protected override Image FindImage(string imagename)
		{
			// Check if name is a "none" texture
			if((imagename.Length < 1) || (imagename[0] == '-'))
			{
				// Determine image to show
				if(required)
					return CodeImp.DoomBuilder.Properties.Resources.MissingTexture;
				else
					return null;
			}
			else
			{
				// Set the image
				return General.Map.Data.GetTextureImage(imagename).GetPreview();
			}
		}

		// This browses for a texture
		protected override string BrowseImage(string imagename)
		{
			string result;

			// Browse for texture
			result = TextureBrowserForm.Browse(this.ParentForm, imagename);
			if(result != null) return result; else return imagename;
		}
	}
}
