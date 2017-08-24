
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
using SlimDX;
using SlimDX.Direct3D9;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;

#endregion

namespace CodeImp.DoomBuilder.Controls
{
	/// <summary>
	/// Abstract control that provides a list of images.
	/// </summary>
	public abstract partial class ImageSelectorControl : UserControl
	{
		#region ================== Variables

		private Bitmap bmp;
		private bool ispressed;
		private bool ismouseinside;
		private MouseButtons button;
		protected bool allowclear;
		
		#endregion

		#region ================== Properties
		
		public string TextureName { get { return name.Text; } set { name.Text = value; } }

		#endregion

		#region ================== Constructor / Destructor

		// Constructor
		public ImageSelectorControl()
		{
			// Initialize
			InitializeComponent();
		}
		
		// Setup
		public virtual void Initialize()
		{
			// set the max length of texture names
			name.MaxLength = General.Map.Config.MaxTextureNamelength;
		}
		
		#endregion

		#region ================== Events

		// When resized
		private void ImageSelectorControl_Resize(object sender, EventArgs e)
		{
			// Fixed size
			preview.Width = this.ClientSize.Width;
			preview.Height = this.ClientSize.Height - name.Height - 4;
			name.Width = this.ClientSize.Width;
			name.Top = this.ClientSize.Height - name.Height;
		}
		
		// Layout change
		private void ImageSelectorControl_Layout(object sender, LayoutEventArgs e)
		{
			ImageSelectorControl_Resize(sender, EventArgs.Empty);
		}
		
		// Image clicked
		private void preview_Click(object sender, EventArgs e)
		{
			ispressed = false;
			preview.BackColor = SystemColors.Highlight;
			ShowPreview(FindImage(name.Text));
			if(button == MouseButtons.Right)
			{
				if(allowclear) name.Text = "-";
			}
			else if(button == MouseButtons.Left)
			{
				name.Text = BrowseImage(name.Text);
			}
		}
		
		// Name text changed
		private void name_TextChanged(object sender, EventArgs e)
		{
			// Show it centered
			ShowPreview(FindImage(name.Text));
		}
		
		// Mouse pressed
		private void preview_MouseDown(object sender, MouseEventArgs e)
		{
			button = e.Button;
			if((button == MouseButtons.Left) || ((button == MouseButtons.Right) && allowclear))
			{
				ispressed = true;
				preview.BackColor = AdjustedColor(SystemColors.Highlight, 0.2f);
				ShowPreview(FindImage(name.Text));
			}
		}

		// Mouse released
		private void preview_MouseUp(object sender, MouseEventArgs e)
		{
			ispressed = false;
			ShowPreview(FindImage(name.Text));
		}

		// Mouse leaves
		private void preview_MouseLeave(object sender, EventArgs e)
		{
			ispressed = false;
			ismouseinside = false;
			preview.BackColor = SystemColors.AppWorkspace;
		}
		
		// Mouse enters
		private void preview_MouseEnter(object sender, EventArgs e)
		{
			ismouseinside = true;
			preview.BackColor = SystemColors.Highlight;
			ShowPreview(FindImage(name.Text));
		}

		// Mouse moves
		private void preview_MouseMove(object sender, MouseEventArgs e)
		{
			if(!ismouseinside)
			{
				ismouseinside = true;
				preview.BackColor = SystemColors.Highlight;
				ShowPreview(FindImage(name.Text));
			}
		}
		
		#endregion

		#region ================== Methods
		
		// This refreshes the control
		new public void Refresh()
		{
			ShowPreview(FindImage(name.Text));
			base.Refresh();
		}
		
		// This redraws the image preview
		private void ShowPreview(Image image)
		{
			// Dispose old image
			preview.BackgroundImage = null;
			if(bmp != null)
			{
				bmp.Dispose();
				bmp = null;
			}
			
			if(image != null)
			{
				// Show it centered
				General.DisplayZoomedImage(preview, image);
				preview.Refresh();
			}
		}
		
		// This must determine and return the image to show
		protected abstract Image FindImage(string imagename);

		// This must show the image browser and return the selected texture name
		protected abstract string BrowseImage(string imagename);

		// This determines the result value
		public string GetResult(string original)
		{
			// Anyting entered?
			if(name.Text.Trim().Length > 0)
			{
				// Return the new value
				return name.Text;
			}
			else
			{
				// Nothing given, keep original value
				return original;
			}
		}

		// This brightens or darkens a color
		private Color AdjustedColor(Color c, float amount)
		{
			Color4 cc = new Color4(c);

			// Adjust color
			cc.Red = Saturate((cc.Red * (1f + amount)) + (amount * 0.5f));
			cc.Green = Saturate((cc.Green * (1f + amount)) + (amount * 0.5f));
			cc.Blue = Saturate((cc.Blue * (1f + amount)) + (amount * 0.5f));

			// Return result
			return Color.FromArgb(cc.ToArgb());
		}

		// This clamps a value between 0 and 1
		private float Saturate(float v)
		{
			if(v < 0f) return 0f; else if(v > 1f) return 1f; else return v;
		}
		
		#endregion
	}
}
