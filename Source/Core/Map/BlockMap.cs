
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
using CodeImp.DoomBuilder.Actions;
using CodeImp.DoomBuilder.Geometry;
using System.Drawing;
using SlimDX;
using CodeImp.DoomBuilder.Editing;

#endregion

namespace CodeImp.DoomBuilder.Map
{
	public class BlockMap<BE> where BE : BlockEntry, new()
	{
		#region ================== Constants
		
		#endregion
		
		#region ================== Variables
		
		// Blocks
		protected BE[,] blockmap;
		protected int blocksizeshift;
		protected int blocksize;
		protected Size size;
		protected RectangleF range;
		protected Vector2D rangelefttop;
		
		// State
		private bool isdisposed;
		
		#endregion
		
		#region ================== Properties
		
		public bool IsDisposed { get { return isdisposed; } }
		public Size Size { get { return size; } }
		public RectangleF Range { get { return range; } }
		public int BlockSize { get { return blocksize; } }
		
		#endregion
		
		#region ================== Constructor / Disposer
		
		// Constructor
		public BlockMap(RectangleF range)
		{
			Initialize(range, 128);
		}

		// Constructor
		public BlockMap(RectangleF range, int blocksize)
		{
			Initialize(range, blocksize);
		}

		// This initializes the blockmap
		private void Initialize(RectangleF range, int blocksize)
		{
			// Initialize
			this.range = range;
			this.blocksizeshift = General.BitsForInt(blocksize);
			this.blocksize = 1 << blocksizeshift;
			if((this.blocksize != blocksize) || (this.blocksize <= 1)) throw new ArgumentException("Block size must be a power of 2 greater than 1");
			rangelefttop = new Vector2D(range.Left, range.Top);
			Point lefttop = new Point((int)range.Left >> blocksizeshift, (int)range.Top >> blocksizeshift);
			Point rightbottom = new Point((int)range.Right >> blocksizeshift, (int)range.Bottom >> blocksizeshift);
			size = new Size((rightbottom.X - lefttop.X) + 1, (rightbottom.Y - lefttop.Y) + 1);
			blockmap = new BE[size.Width, size.Height];
			Clear();
		}
		
		// Disposer
		public void Dispose()
		{
			// Not already disposed?
			if(!isdisposed)
			{
				// Clean up
				blockmap = null;
				
				// Done
				isdisposed = true;
			}
		}
		
		#endregion
		
		#region ================== Methods
		
		// This returns the block coordinates
		protected Point GetBlockCoordinates(Vector2D v)
		{
			return new Point((int)(v.x - range.Left) >> blocksizeshift,
							 (int)(v.y - range.Top) >> blocksizeshift);
		}

		// This returns the block center in world coordinates
		protected Vector2D GetBlockCenter(Point p)
		{
			return new Vector2D((float)((p.X << blocksizeshift) + (blocksize >> 1)) + range.Left,
								(float)((p.Y << blocksizeshift) + (blocksize >> 1)) + range.Top);
		}
		
		// This returns true when the given block is inside range
		protected bool IsInRange(Point p)
		{
			return (p.X >= 0) && (p.X < size.Width) && (p.Y >= 0) && (p.Y < size.Height);
		}
		
		// This returns true when the given block is inside range
		public bool IsInRange(Vector2D p)
		{
			return (p.x >= range.Left) && (p.x < range.Right) && (p.y >= range.Top) && (p.y < range.Bottom);
		}
		
		// This crops a point into the range
		protected Point CropToRange(Point p)
		{
			return new Point(Math.Min(Math.Max(p.X, 0), size.Width - 1),
			                 Math.Min(Math.Max(p.Y, 0), size.Height - 1));
		}

		// This crops a point into the range
		protected int CropToRangeX(int x)
		{
			return Math.Min(Math.Max(x, 0), size.Width - 1);
		}

		// This crops a point into the range
		protected int CropToRangeY(int y)
		{
			return Math.Min(Math.Max(y, 0), size.Height - 1);
		}
		
		// This clears the blockmap
		public virtual void Clear()
		{
			for(int x = 0; x < size.Width; x++)
			{
				for(int y = 0; y < size.Height; y++)
				{
					blockmap[x, y] = new BE();
				}
			}
		}

		// This returns a blocks at the given coordinates, if any
		// Returns null when out of range
		public virtual BE GetBlockAt(Vector2D pos)
		{
			// Calculate block coordinates
			Point p = GetBlockCoordinates(pos);
			return IsInRange(p) ? blockmap[p.X, p.Y] : null;
		}

		// This returns a range of blocks in a square
		public virtual List<BE> GetSquareRange(RectangleF rect)
		{
			// Calculate block coordinates
			Point lt = GetBlockCoordinates(new Vector2D(rect.Left, rect.Top));
			Point rb = GetBlockCoordinates(new Vector2D(rect.Right, rect.Bottom));
			
			// Crop coordinates to range
			lt = CropToRange(lt);
			rb = CropToRange(rb);
			
			// Go through the range to make a list
			int entriescount = ((rb.X - lt.X) + 1) * ((rb.Y - lt.Y) + 1);
			List<BE> entries = new List<BE>(entriescount);
			for(int x = lt.X; x <= rb.X; x++)
			{
				for(int y = lt.Y; y <= rb.Y; y++)
				{
					entries.Add(blockmap[x, y]);
				}
			}
			
			// Return list
			return entries;
		}

		// This returns all blocks along the given line
		public virtual List<BE> GetLineBlocks(Vector2D v1, Vector2D v2)
		{
			float deltax, deltay;
			float posx, posy;
			Point pos, end;
			int dirx, diry;
			
			// Estimate number of blocks we will go through and create list
			int entriescount = (int)(Vector2D.ManhattanDistance(v1, v2) * 2.0f) / blocksize;
			List<BE> entries = new List<BE>(entriescount);

			// Find start and end block
			pos = GetBlockCoordinates(v1);
			end = GetBlockCoordinates(v2);
			v1 -= rangelefttop;
			v2 -= rangelefttop;

			// Horizontal straight line?
			if(pos.Y == end.Y)
			{
				// Simple loop
				pos.X = CropToRangeX(pos.X);
				end.X = CropToRangeX(end.X);
				if(IsInRange(new Point(pos.X, pos.Y)))
				{
					dirx = Math.Sign(v2.x - v1.x);
					if(dirx != 0)
					{
						for(int x = pos.X; x != end.X; x += dirx)
						{
							entries.Add(blockmap[x, pos.Y]);
						}
					}
					entries.Add(blockmap[end.X, end.Y]);
				}
			}
			// Vertical straight line?
			else if(pos.X == end.X)
			{
				// Simple loop
				pos.Y = CropToRangeY(pos.Y);
				end.Y = CropToRangeY(end.Y);
				if(IsInRange(new Point(pos.X, pos.Y)))
				{
					diry = Math.Sign(v2.y - v1.y);
					if(diry != 0)
					{
						for(int y = pos.Y; y != end.Y; y += diry)
						{
							entries.Add(blockmap[pos.X, y]);
						}
					}
					entries.Add(blockmap[end.X, end.Y]);
				}
			}
			else
			{
				// Add this block
				if(IsInRange(pos)) entries.Add(blockmap[pos.X, pos.Y]);

				// Moving outside the block?
				if(pos != end)
				{
					// Calculate current block edges
					float cl = pos.X * blocksize;
					float cr = (pos.X + 1) * blocksize;
					float ct = pos.Y * blocksize;
					float cb = (pos.Y + 1) * blocksize;

					// Line directions
					dirx = Math.Sign(v2.x - v1.x);
					diry = Math.Sign(v2.y - v1.y);

					// Calculate offset and delta movement over x
					if(dirx >= 0)
					{
						posx = (cr - v1.x) / (v2.x - v1.x);
						deltax = blocksize / (v2.x - v1.x);
					}
					else
					{
						// Calculate offset and delta movement over x
						posx = (v1.x - cl) / (v1.x - v2.x);
						deltax = blocksize / (v1.x - v2.x);
					}

					// Calculate offset and delta movement over y
					if(diry >= 0)
					{
						posy = (cb - v1.y) / (v2.y - v1.y);
						deltay = blocksize / (v2.y - v1.y);
					}
					else
					{
						posy = (v1.y - ct) / (v1.y - v2.y);
						deltay = blocksize / (v1.y - v2.y);
					}

					// Continue while not reached the end
					while(pos != end)
					{
						// Check in which direction to move
						if(posx < posy)
						{
							// Move horizontally
							posx += deltax;
							if(pos.X != end.X) pos.X += dirx;
						}
						else
						{
							// Move vertically
							posy += deltay;
							if(pos.Y != end.Y) pos.Y += diry;
						}
						
						// Add lines to this block
						if(IsInRange(pos)) entries.Add(blockmap[pos.X, pos.Y]);
					}
				}
			}
			
			// Return list
			return entries;
		}

		// This puts a thing in the blockmap
		public virtual void AddThingsSet(ICollection<Thing> things)
		{
			foreach(Thing t in things) AddThing(t);
		}
		
		// This puts a thing in the blockmap
		public virtual void AddThing(Thing t)
		{
			Point p = GetBlockCoordinates(t.Position);
			if(IsInRange(p)) blockmap[p.X, p.Y].Things.Add(t);
		}

		// This puts a secotr in the blockmap
		public virtual void AddSectorsSet(ICollection<Sector> sectors)
		{
			foreach(Sector s in sectors) AddSector(s);
		}

		// This puts a sector in the blockmap
		public virtual void AddSector(Sector s)
		{
			Point p1 = GetBlockCoordinates(new Vector2D(s.BBox.Left, s.BBox.Top));
			Point p2 = GetBlockCoordinates(new Vector2D(s.BBox.Right, s.BBox.Bottom));
			p1 = CropToRange(p1);
			p2 = CropToRange(p2);
			for(int x = p1.X; x <= p2.X; x++)
			{
				for(int y = p1.Y; y <= p2.Y; y++)
				{
					blockmap[x, y].Sectors.Add(s);
				}
			}
		}
		
		// This puts a whole set of linedefs in the blocks they cross
		public virtual void AddLinedefsSet(ICollection<Linedef> lines)
		{
			foreach(Linedef l in lines) AddLinedef(l);
		}
		
		// This puts a single linedef in all blocks it crosses
		public virtual void AddLinedef(Linedef line)
		{
			Vector2D v1, v2;
			float deltax, deltay;
			float posx, posy;
			Point pos, end;
			int dirx, diry;

			// Get coordinates
			v1 = line.Start.Position;
			v2 = line.End.Position;
			
			// Find start and end block
			pos = GetBlockCoordinates(v1);
			end = GetBlockCoordinates(v2);
			v1 -= rangelefttop;
			v2 -= rangelefttop;
			
			// Horizontal straight line?
			if(pos.Y == end.Y)
			{
				// Simple loop
				pos.X = CropToRangeX(pos.X);
				end.X = CropToRangeX(end.X);
				if(IsInRange(new Point(pos.X, pos.Y)))
				{
					dirx = Math.Sign(v2.x - v1.x);
					if(dirx != 0)
					{
						for(int x = pos.X; x != end.X; x += dirx)
						{
							blockmap[x, pos.Y].Lines.Add(line);
						}
					}
					blockmap[end.X, end.Y].Lines.Add(line);
				}
			}
			// Vertical straight line?
			else if(pos.X == end.X)
			{
				// Simple loop
				pos.Y = CropToRangeY(pos.Y);
				end.Y = CropToRangeY(end.Y);
				if(IsInRange(new Point(pos.X, pos.Y)))
				{
					diry = Math.Sign(v2.y - v1.y);
					if(diry != 0)
					{
						for(int y = pos.Y; y != end.Y; y += diry)
						{
							blockmap[pos.X, y].Lines.Add(line);
						}
					}
					blockmap[end.X, end.Y].Lines.Add(line);
				}
			}
			else
			{
				// Add lines to this block
				if(IsInRange(pos)) blockmap[pos.X, pos.Y].Lines.Add(line);
				
				// Moving outside the block?
				if(pos != end)
				{
					// Calculate current block edges
					float cl = pos.X * blocksize;
					float cr = (pos.X + 1) * blocksize;
					float ct = pos.Y * blocksize;
					float cb = (pos.Y + 1) * blocksize;
					
					// Line directions
					dirx = Math.Sign(v2.x - v1.x);
					diry = Math.Sign(v2.y - v1.y);
					
					// Calculate offset and delta movement over x
					if(dirx == 0)
					{
						posx = float.MaxValue;
						deltax = float.MaxValue;
					}
					else if(dirx > 0)
					{
						posx = (cr - v1.x) / (v2.x - v1.x);
						deltax = blocksize / (v2.x - v1.x);
					}
					else
					{
						// Calculate offset and delta movement over x
						posx = (v1.x - cl) / (v1.x - v2.x);
						deltax = blocksize / (v1.x - v2.x);
					}
					
					// Calculate offset and delta movement over y
					if(diry == 0)
					{
						posy = float.MaxValue;
						deltay = float.MaxValue;
					}
					else if(diry > 0)
					{
						posy = (cb - v1.y) / (v2.y - v1.y);
						deltay = blocksize / (v2.y - v1.y);
					}
					else
					{
						posy = (v1.y - ct) / (v1.y - v2.y);
						deltay = blocksize / (v1.y - v2.y);
					}
					
					// Continue while not reached the end
					while(pos != end)
					{
						// Check in which direction to move
						if(posx < posy)
						{
							// Move horizontally
							posx += deltax;
							if(pos.X != end.X) pos.X += dirx;
						}
						else
						{
							// Move vertically
							posy += deltay;
							if(pos.Y != end.Y) pos.Y += diry;
						}
						
						// Add lines to this block
						if(IsInRange(pos)) blockmap[pos.X, pos.Y].Lines.Add(line);
					}
				}
			}
		}
		
		#endregion
	}
}
