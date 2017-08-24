
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
using System.Windows.Forms.VisualStyles;
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
using CodeImp.DoomBuilder.Types;
using System.Drawing.Text;

#endregion

namespace CodeImp.DoomBuilder.Controls
{
	internal class DockersTabsControl : TabControl
	{
		#region ================== Constants

		#endregion

		#region ================== Variables
		
		private Bitmap tabsimage;
		private int highlighttab;
		
		#endregion

		#region ================== Properties

		#endregion

		#region ================== Constructor

		// Constructor
		public DockersTabsControl()
		{
			if(VisualStyleInformation.IsSupportedByOS && VisualStyleInformation.IsEnabledByUser)
			{
				// Style settings
				this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
				this.SetStyle(ControlStyles.SupportsTransparentBackColor, false);
				this.SetStyle(ControlStyles.UserPaint, true);
				this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
				this.SetStyle(ControlStyles.Opaque, true);
				this.UpdateStyles();
			}
			
			highlighttab = -1;
		}

		// Disposer
		protected override void Dispose(bool disposing)
		{
			if(tabsimage != null)
			{
				tabsimage.Dispose();
				tabsimage = null;
			}
			
			base.Dispose(disposing);
		}
		
		#endregion

		#region ================== Methods
		
		// This redraws the tabs
		protected unsafe void RedrawTabs()
		{
			// Determine length and width in pixels
			int tabslength = 0;
			for(int i = 0; i < this.TabPages.Count; i++)
			{
				Rectangle r = this.GetTabRect(i);
				tabslength += r.Height;
			}
			tabslength += 4;
			int tabswidth = this.ItemSize.Height + 2;
			
			// Dispose old image
			if(tabsimage != null)
			{
				tabsimage.Dispose();
				tabsimage = null;
			}
			
			if(VisualStyleInformation.IsSupportedByOS && VisualStyleInformation.IsEnabledByUser)
			{
				StringFormat drawformat = new StringFormat();
				drawformat.Alignment = StringAlignment.Center;
				drawformat.HotkeyPrefix = HotkeyPrefix.None;
				drawformat.LineAlignment = StringAlignment.Center;
				
				// Create images
				tabsimage = new Bitmap(tabswidth, tabslength, PixelFormat.Format32bppArgb);
				Bitmap drawimage = new Bitmap(tabslength, tabswidth, PixelFormat.Format32bppArgb);
				Graphics g = Graphics.FromImage(drawimage);
				
				// Render the tabs (backwards when right-aligned)
				int posoffset = 0;
				int selectedposoffset = -1;
				int start = (this.Alignment == TabAlignment.Left) ? 0 : (this.TabPages.Count - 1);
				int end = (this.Alignment == TabAlignment.Left) ? this.TabPages.Count : -1;
				int step = (this.Alignment == TabAlignment.Left) ? 1 : -1;
				for(int i = start; i != end; i += step)
				{
					VisualStyleRenderer renderer;
					Rectangle tr = this.GetTabRect(i);
					
					// Tab selected?
					if(i == this.SelectedIndex)
					{
						// We will draw this later
						selectedposoffset = posoffset;
					}
					else
					{
						if(i == highlighttab)
							renderer = new VisualStyleRenderer(VisualStyleElement.Tab.TabItem.Hot);
						else
							renderer = new VisualStyleRenderer(VisualStyleElement.Tab.TabItem.Normal);
						
						// Draw tab
						Rectangle r = new Rectangle(posoffset + 2, 2, tr.Height, tr.Width - 2);
						renderer.DrawBackground(g, r);
						g.DrawString(this.TabPages[i].Text, this.Font, SystemBrushes.ControlText, new RectangleF(r.Location, r.Size), drawformat);
					}
					
					posoffset += tr.Height;
				}
				
				// Render the selected tab, because it is slightly larger and overlapping the others
				if(selectedposoffset > -1)
				{
					VisualStyleRenderer renderer = new VisualStyleRenderer(VisualStyleElement.Tab.TabItem.Pressed);
					Rectangle tr = this.GetTabRect(this.SelectedIndex);
					Rectangle r = new Rectangle(selectedposoffset, 0, tr.Height + 4, tr.Width);
					renderer.DrawBackground(g, r);
					g.DrawString(this.TabPages[this.SelectedIndex].Text, this.Font, SystemBrushes.ControlText, new RectangleF(r.X, r.Y, r.Width, r.Height - 2), drawformat);
				}
				
				// Rotate the image and copy to tabsimage
				BitmapData drawndata = drawimage.LockBits(new Rectangle(0, 0, drawimage.Size.Width, drawimage.Size.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
				BitmapData targetdata = tabsimage.LockBits(new Rectangle(0, 0, tabsimage.Size.Width, tabsimage.Size.Height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
				int* dd = (int*)drawndata.Scan0.ToPointer();
				int* td = (int*)targetdata.Scan0.ToPointer();
				if(this.Alignment == TabAlignment.Right)
				{
					for(int y = 0; y < drawndata.Height; y++)
					{
						for(int x = 0; x < drawndata.Width; x++)
						{
							td[(drawndata.Width - 1 - x) * targetdata.Width + y] = *dd;
							dd++;
						}
					}
				}
				else
				{
					for(int y = 0; y < drawndata.Height; y++)
					{
						for(int x = 0; x < drawndata.Width; x++)
						{
							td[x * targetdata.Width + (drawndata.Height - 1 - y)] = *dd;
							dd++;
						}
					}
				}
				drawimage.UnlockBits(drawndata);
				tabsimage.UnlockBits(targetdata);
				
				// Clean up
				g.Dispose();
				drawimage.Dispose();
			}
		}
		
		#endregion
		
		#region ================== Events
		
		// Redrawing needed
		protected override void OnPaint(PaintEventArgs e)
		{
			Point p;

			if(VisualStyleInformation.IsSupportedByOS && VisualStyleInformation.IsEnabledByUser)
			{
				RedrawTabs();
				
				e.Graphics.Clear(SystemColors.Control);
				
				if(this.Alignment == TabAlignment.Left)
				{
					p = new Point(0, 0);
				}
				else
				{
					int left = this.ClientSize.Width - tabsimage.Size.Width;
					if(left < 0) left = 0;
					p = new Point(left, 0);
				}
				
				e.Graphics.DrawImage(tabsimage, p);
			}
			else
			{
				base.OnPaint(e);
			}
		}
		
		// Mouse moves
		protected override void OnMouseMove(MouseEventArgs e)
		{
			if(VisualStyleInformation.IsSupportedByOS && VisualStyleInformation.IsEnabledByUser)
			{
				int foundindex = -1;
				Rectangle prect = new Rectangle(e.Location, Size.Empty);
				
				// Check in which tab the mouse is
				for(int i = this.TabPages.Count - 1; i >= 0; i--)
				{
					Rectangle tabrect = this.GetTabRect(i);
					tabrect.Inflate(1, 1);
					if(tabrect.IntersectsWith(prect))
					{
						foundindex = i;
						break;
					}
				}
				
				// Redraw?
				if(foundindex != highlighttab)
				{
					highlighttab = foundindex;
					this.Invalidate();
				}
			}
			
			base.OnMouseMove(e);
		}
		
		// Mouse leaves
		protected override void OnMouseLeave(EventArgs e)
		{
			if(VisualStyleInformation.IsSupportedByOS && VisualStyleInformation.IsEnabledByUser)
			{
				// Redraw?
				if(highlighttab != -1)
				{
					highlighttab = -1;
					this.Invalidate();
				}
			}
			
			base.OnMouseLeave(e);
		}
		
		// Tabs don't process keys
		protected override void OnKeyDown(KeyEventArgs ke)
		{
			if(this.Parent is DockersControl)
			{
				// Only absorb the key press when no focused on an input control, otherwise
				// the input controls may not receive certain keys such as delete and arrow keys
				DockersControl docker = (this.Parent as DockersControl);
				if(!docker.IsFocused)
					ke.Handled = true;
			}
		}
		
		// Tabs don't process keys
		protected override void OnKeyUp(KeyEventArgs e)
		{
			if(this.Parent is DockersControl)
			{
				// Only absorb the key press when no focused on an input control, otherwise
				// the input controls may not receive certain keys such as delete and arrow keys
				DockersControl docker = (this.Parent as DockersControl);
				if(!docker.IsFocused)
					e.Handled = true;
			}
		}
		
		#endregion
	}
}
