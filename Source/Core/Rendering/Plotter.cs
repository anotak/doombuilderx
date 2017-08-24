
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
using System.Drawing;
using System.ComponentModel;
using CodeImp.DoomBuilder.Map;
using SlimDX.Direct3D9;
using SlimDX;
using CodeImp.DoomBuilder.Geometry;
using System.Drawing.Imaging;

#endregion

namespace CodeImp.DoomBuilder.Rendering
{
	internal unsafe sealed class Plotter
	{
		#region ================== Constants

		#endregion

		#region ================== Variables

		// Memory
		private PixelColor* pixels;
		private int width;
		private int height;
		private int visiblewidth;
		private int visibleheight;

		#endregion

		#region ================== Properties

		public int VisibleWidth { get { return visiblewidth; } }
		public int VisibleHeight { get { return visibleheight; } }
		public int Width { get { return width; } }
		public int Height { get { return height; } }

		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		public Plotter(PixelColor* pixels, int width, int height, int visiblewidth, int visibleheight)
		{
			// Initialize
			this.pixels = pixels;
			this.width = width;
			this.height = height;
			this.visiblewidth = width;
			this.visibleheight = height;
			
			// We have no destructor
			GC.SuppressFinalize(this);
		}

		#endregion

		#region ================== Pixel Rendering
		
		// This clears all pixels black
		public void Clear()
		{
			// Clear memory
			General.ZeroMemory(new IntPtr(pixels), width * height * sizeof(PixelColor));
		}
		
		// This draws a pixel normally
		public void DrawPixelSolid(int x, int y, ref PixelColor c)
		{
			// Draw pixel when within range
			if((x >= 0) && (x < visiblewidth) && (y >= 0) && (y < visibleheight))
				pixels[y * width + x] = c;
		}

		// This draws a pixel normally
		public void DrawVertexSolid(int x, int y, int size, ref PixelColor c, ref PixelColor l, ref PixelColor d)
		{
			int x1 = x - size;
			int x2 = x + size;
			int y1 = y - size;
			int y2 = y + size;
			int xp, yp;

			// Do unchecked?
			if((x1 >= 0) && (x2 < visiblewidth) && (y1 >= 0) && (y2 < visibleheight))
			{
				// Filled square
				for(yp = y1; yp <= y2; yp++)
					for(xp = x1; xp <= x2; xp++)
						pixels[yp * width + xp] = c;

				// Vertical edges
				for(yp = y1 + 1; yp <= y2 - 1; yp++)
				{
					pixels[yp * width + x1] = l;
					pixels[yp * width + x2] = d;
				}

				// Horizontal edges
				for(xp = x1 + 1; xp <= x2 - 1; xp++)
				{
					pixels[y1 * width + xp] = l;
					pixels[y2 * width + xp] = d;
				}

				// Corners
				pixels[y2 * width + x2] = d;
				pixels[y1 * width + x1] = l;
			}
			/*
			else
			{
				// Filled square
				for(yp = y - size; yp <= y + size; yp++)
					for(xp = x - size; xp <= x + size; xp++)
						DrawPixelSolid(xp, yp, c);

				// Vertical edges
				for(yp = y - size + 1; yp <= y + size - 1; yp++)
				{
					DrawPixelSolid(x - size, yp, l);
					DrawPixelSolid(x + size, yp, d);
				}

				// Horizontal edges
				for(xp = x - size + 1; xp <= x + size - 1; xp++)
				{
					DrawPixelSolid(xp, y - size, l);
					DrawPixelSolid(xp, y + size, d);
				}

				// Corners
				DrawPixelSolid(x + size, y + size, d);
				DrawPixelSolid(x - size, y - size, l);
			}
			*/
		}

		// This draws a dotted grid line horizontally
		public void DrawGridLineH(int y, int x1, int x2, ref PixelColor c)
		{
			int numpixels = visiblewidth >> 1;
			int offset = y & 0x01;
			int ywidth = y * width;
			x1 = General.Clamp(x1 >> 1, 0, numpixels - 1);
			x2 = General.Clamp(x2 >> 1, 0, numpixels - 1);
			
			if ((y >= 0) && (y < height))
			{
				// Draw all pixels on this line
				for (int i = x1; i < x2; i++) pixels[ywidth + ((i << 1) | offset)] = c;
			}
		}

		// This draws a dotted grid line vertically
		public void DrawGridLineV(int x, int y1, int y2, ref PixelColor c)
		{
			int numpixels = visibleheight >> 1;
			int offset = x & 0x01;
			y1 = General.Clamp(y1 >> 1, 0, numpixels - 1);
			y2 = General.Clamp(y2 >> 1, 0, numpixels - 1);
			
			if((x >= 0) && (x < width))
			{
				// Draw all pixels on this line
				for (int i = y1; i < y2; i++) pixels[((i << 1) | offset) * width + x] = c;
			}
		}

		// This draws a pixel alpha blended
		public void DrawPixelAlpha(int x, int y, ref PixelColor c)
		{
			float a;

			// Draw only when within range
			if((x >= 0) && (x < visiblewidth) && (y >= 0) && (y < visibleheight))
			{
				// Get the target pixel
				PixelColor* p = pixels + (y * width + x);

				// Not drawn on target yet?
				if(*(int*)p == 0)
				{
					// Simply apply color to pixel
					*p = c;
				}
				else
				{
					// Blend with pixel
					a = (float)c.a * 0.003921568627450980392156862745098f;
					if((int)p->a + (int)c.a > 255) p->a = 255; else p->a += c.a;
					p->r = (byte)((float)p->r * (1f - a) + (float)c.r * a);
					p->g = (byte)((float)p->g * (1f - a) + (float)c.g * a);
					p->b = (byte)((float)p->b * (1f - a) + (float)c.b * a);
				}
			}
		}

		// This draws a line normally
		// See: http://en.wikipedia.org/wiki/Bresenham%27s_line_algorithm
		public void DrawLineSolid(int x1, int y1, int x2, int y2, ref PixelColor c)
		{
			int i;

			// Check if the line is outside the screen for sure.
			// This is quickly done by checking in which area both points are. When this
			// is above, below, right or left of the screen, then skip drawing the line.
			if(((x1 < 0) && (x2 < 0)) ||
			   ((x1 > visiblewidth) && (x2 > visiblewidth)) ||
			   ((y1 < 0) && (y2 < 0)) ||
			   ((y1 > visibleheight) && (y2 > visibleheight))) return;

			// Distance of the line
			int dx = x2 - x1;
			int dy = y2 - y1;

			// Positive (absolute) distance
			int dxabs = Math.Abs(dx);
			int dyabs = Math.Abs(dy);

			// Half distance
			int x = dyabs >> 1;
			int y = dxabs >> 1;

			// Direction
			int sdx = Math.Sign(dx);
			int sdy = Math.Sign(dy);

			// Start position
			int px = x1;
			int py = y1;

			// When the line is completely inside screen,
			// then do an unchecked draw, because all of its pixels are
			// guaranteed to be within the memory range
			if((x1 >= 0) && (x2 >= 0) && (x1 < visiblewidth) && (x2 < visiblewidth) &&
			   (y1 >= 0) && (y2 >= 0) && (y1 < visibleheight) && (y2 < visibleheight))
			{
				// Draw first pixel
				pixels[py * width + px] = c;

				// Check if the line is more horizontal than vertical
				if(dxabs >= dyabs)
				{
					for(i = 0; i < dxabs; i++)
					{
						y += dyabs;
						if(y >= dxabs)
						{
							y -= dxabs;
							py += sdy;
						}
						px += sdx;

						// Draw pixel
						pixels[py * width + px] = c;
					}
				}
				// Else the line is more vertical than horizontal
				else
				{
					for(i = 0; i < dyabs; i++)
					{
						x += dxabs;
						if(x >= dyabs)
						{
							x -= dyabs;
							px += sdx;
						}
						py += sdy;

						// Draw pixel
						pixels[py * width + px] = c;
					}
				}
			}
			else
			{
				// Draw first pixel
				if((px >= 0) && (px < visiblewidth) && (py >= 0) && (py < visibleheight))
					pixels[py * width + px] = c;
				
				// Check if the line is more horizontal than vertical
				if(dxabs >= dyabs)
				{
					for(i = 0; i < dxabs; i++)
					{
						y += dyabs;
						if(y >= dxabs)
						{
							y -= dxabs;
							py += sdy;
						}
						px += sdx;
						
						// Draw pixel
						if((px >= 0) && (px < visiblewidth) && (py >= 0) && (py < visibleheight))
							pixels[py * width + px] = c;
					}
				}
				// Else the line is more vertical than horizontal
				else
				{
					for(i = 0; i < dyabs; i++)
					{
						x += dxabs;
						if(x >= dyabs)
						{
							x -= dyabs;
							px += sdx;
						}
						py += sdy;
						
						// Draw pixel
						if((px >= 0) && (px < visiblewidth) && (py >= 0) && (py < visibleheight))
							pixels[py * width + px] = c;
					}
				}
			}
		}

		#endregion
	}
}
