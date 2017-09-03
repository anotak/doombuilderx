
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
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;

#endregion

namespace CodeImp.DoomBuilder.Controls
{
	internal class ImageBrowserItem : ListViewItem, IComparable<ImageBrowserItem>
	{
		#region ================== Variables

		// Display image and text
		public ImageData icon;
		public string displaytext;
		
		// Group
		private ListViewGroup listgroup;
		
		// Image cache
		private bool imageloaded;

        private static Brush unselected_backcolor, unselected_forecolor;

        #endregion

        #region ================== Properties

        public ListViewGroup ListGroup { get { return listgroup; } set { listgroup = value; } }
		public bool IsPreviewLoaded { get { return imageloaded; } }

		#endregion

		#region ================== Constructor / Disposer

		// Constructors
		public ImageBrowserItem(string text, ImageData icon, object tag)
		{
			// Initialize
			this.Text = text;
			if(General.Settings.ShowTextureSizes)
				this.displaytext = text + "\n" + icon.ScaledWidth + " x " + icon.ScaledHeight;
			else
				this.displaytext = text;

            if (icon.bIsFlat)
            {
                this.displaytext += "*";
            }

			this.icon = icon;
			this.Tag = tag;
		}
		
		#endregion
		
		#region ================== Methods
		
		// This checks if a redraw is needed
		public bool CheckRedrawNeeded()
		{
			return (icon.IsPreviewLoaded != imageloaded);
		}

        // This draws the images
        public void Draw(Graphics g, Rectangle bounds)
		{
			Brush forecolor;
			Brush backcolor;
			
			// Remember if the preview is loaded
			imageloaded = icon.IsPreviewLoaded;

			// Drawing settings
			g.CompositingQuality = CompositingQuality.HighSpeed;
			g.InterpolationMode = InterpolationMode.NearestNeighbor;
			g.SmoothingMode = SmoothingMode.HighSpeed;
			g.PixelOffsetMode = PixelOffsetMode.None;

			// Determine coordinates
			SizeF textsize = g.MeasureString(displaytext, this.ListView.Font, bounds.Width * 2);
			Rectangle imagerect = new Rectangle(bounds.Left + ((bounds.Width - General.Map.Data.Previews.MaxImageWidth) >> 1),
				bounds.Top + ((bounds.Height - General.Map.Data.Previews.MaxImageHeight - (int)textsize.Height) >> 1),
				General.Map.Data.Previews.MaxImageWidth, General.Map.Data.Previews.MaxImageHeight);
			PointF textpos = new PointF(bounds.Left + ((float)bounds.Width - textsize.Width) * 0.5f, bounds.Bottom - textsize.Height - 2);

			// Determine colors
			if(this.Selected)
			{
                // Highlighted
                /*
				backcolor = new LinearGradientBrush(new Point(0, bounds.Top - 1), new Point(0, bounds.Bottom + 1),
					AdjustedColor(SystemColors.Highlight, 0.2f),
					AdjustedColor(SystemColors.Highlight, -0.1f));
                */
                backcolor = SystemBrushes.Highlight;
				forecolor = SystemBrushes.HighlightText;
			}
			else
			{
                // Normal
                if (unselected_backcolor == null)
                {
                    unselected_backcolor = new SolidBrush(base.ListView.BackColor);
                }
				backcolor = unselected_backcolor;

                if (unselected_forecolor == null)
                {
                    unselected_forecolor = new SolidBrush(base.ListView.ForeColor);
                }
                forecolor = unselected_forecolor;
			}

			// Draw!
			g.FillRectangle(backcolor, bounds);
			icon.DrawPreview(g, imagerect.Location);
			g.DrawString(displaytext, this.ListView.Font, forecolor, textpos);
		}

		// This brightens or darkens a color
		private Color AdjustedColor(Color c, float amount)
		{
			SlimDX.Color4 cc = new SlimDX.Color4(c);
			
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

		// Comparer
		public int CompareTo(ImageBrowserItem other)
		{
			return this.Text.CompareTo(other.Text);
		}

		#endregion
	}
}
