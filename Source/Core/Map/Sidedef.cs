
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
using CodeImp.DoomBuilder.IO;
using CodeImp.DoomBuilder.Geometry;
using CodeImp.DoomBuilder.Types;

#endregion

namespace CodeImp.DoomBuilder.Map
{
	public sealed class Sidedef : MapElement
	{
		#region ================== Constants

		#endregion

		#region ================== Variables

		// Map
		private MapSet map;

		// List items
		private LinkedListNode<Sidedef> sectorlistitem;

		// Owner
		private Linedef linedef;
		
		// Sector
		private Sector sector;

		// Properties
		private int offsetx;
		private int offsety;
		private string texnamehigh;
		private string texnamemid;
		private string texnamelow;
		private long longtexnamehigh;
		private long longtexnamemid;
		private long longtexnamelow;

		// Clone
		private int serializedindex;
		
		#endregion

		#region ================== Properties

		public MapSet Map { get { return map; } }
		public bool IsFront { get { return (linedef != null) ? (this == linedef.Front) : false; } }
		public Linedef Line { get { return linedef; } }
		public Sidedef Other { get { if(this == linedef.Front) return linedef.Back; else return linedef.Front; } }
		public Sector Sector { get { return sector; } }
		public float Angle { get { if(IsFront) return linedef.Angle; else return Angle2D.Normalized(linedef.Angle + Angle2D.PI); } }
		public int OffsetX { get { return offsetx; } set { BeforePropsChange(); offsetx = value; } }
		public int OffsetY { get { return offsety; } set { BeforePropsChange(); offsety = value; } }
		public string HighTexture { get { return texnamehigh; } }
		public string MiddleTexture { get { return texnamemid; } }
		public string LowTexture { get { return texnamelow; } }
		public long LongHighTexture { get { return longtexnamehigh; } }
		public long LongMiddleTexture { get { return longtexnamemid; } }
		public long LongLowTexture { get { return longtexnamelow; } }
		internal int SerializedIndex { get { return serializedindex; } set { serializedindex = value; } }
		
		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		internal Sidedef(MapSet map, int listindex, Linedef l, bool front, Sector s)
		{
			// Initialize
			this.map = map;
			this.listindex = listindex;
			this.texnamehigh = "-";
			this.texnamemid = "-";
			this.texnamelow = "-";
			this.longtexnamehigh = MapSet.EmptyLongName;
			this.longtexnamemid = MapSet.EmptyLongName;
			this.longtexnamelow = MapSet.EmptyLongName;
			
			// Attach linedef
			this.linedef = l;
			if(l != null)
			{
				if(front)
					l.AttachFrontP(this);
				else
					l.AttachBackP(this);
			}
			
			// Attach sector
			SetSectorP(s);
			
			if(map == General.Map.Map)
				General.Map.UndoRedo.RecAddSidedef(this);
			
			// We have no destructor
			GC.SuppressFinalize(this);
		}

		// Disposer
		public override void Dispose()
		{
			// Not already disposed?
			if(!isdisposed)
			{
				// Already set isdisposed so that changes can be prohibited
				isdisposed = true;

				if(map == General.Map.Map)
					General.Map.UndoRedo.RecRemSidedef(this);

				// Remove from main list
				map.RemoveSidedef(listindex);

				// Detach from linedef
				if(linedef != null) linedef.DetachSidedefP(this);
				
				// Detach from sector
				SetSectorP(null);

				// Clean up
				sectorlistitem = null;
				linedef = null;
				map = null;
				sector = null;

				// Dispose base
				base.Dispose();
			}
		}

		#endregion

		#region ================== Management
		
		// Call this before changing properties
		protected override void BeforePropsChange()
		{
			if(map == General.Map.Map)
				General.Map.UndoRedo.RecPrpSidedef(this);
		}

        // Serialize / deserialize (passive: this doesn't record)
        // ano - using new keyword to shadow (and get rid of old DB2 warning)
        internal new void ReadWrite(IReadWriteStream s)
		{
			if(!s.IsWriting) BeforePropsChange();
			
			base.ReadWrite(s);

			s.rwInt(ref offsetx);
			s.rwInt(ref offsety);
			s.rwString(ref texnamehigh);
			s.rwString(ref texnamemid);
			s.rwString(ref texnamelow);
			s.rwLong(ref longtexnamehigh);
			s.rwLong(ref longtexnamemid);
			s.rwLong(ref longtexnamelow);
		}
		
		// This copies all properties to another sidedef
		public void CopyPropertiesTo(Sidedef s)
		{
			s.BeforePropsChange();
			
			// Copy properties
			s.offsetx = offsetx;
			s.offsety = offsety;
			s.texnamehigh = texnamehigh;
			s.texnamemid = texnamemid;
			s.texnamelow = texnamelow;
			s.longtexnamehigh = longtexnamehigh;
			s.longtexnamemid = longtexnamemid;
			s.longtexnamelow = longtexnamelow;
			base.CopyPropertiesTo(s);
		}

		// This changes sector
		public void SetSector(Sector newsector)
		{
			if(map == General.Map.Map)
				General.Map.UndoRedo.RecRefSidedefSector(this);
			
			// Change sector
			SetSectorP(newsector);
		}

		internal void SetSectorP(Sector newsector)
		{
            // ano - if we don't do this, the sector can get disposed in
            // the detach call if it is the only sidedef for the sector
            // (see the unfortunate design of MapSet.AutoRemove)
            if (sector == newsector)
                return;

			// Detach from sector
			if(sector != null) sector.DetachSidedefP(sectorlistitem);

			// Change sector
			sector = newsector;

			// Attach to sector
			if(sector != null)
				sectorlistitem = sector.AttachSidedefP(this);

			General.Map.IsChanged = true;
		}

		// This sets the linedef
		public void SetLinedef(Linedef ld, bool front)
		{
			if(linedef != null) linedef.DetachSidedefP(this);
			
			if(ld != null)
			{
				if(front)
					ld.AttachFront(this);
				else
					ld.AttachBack(this);
			}
		}

		// This sets the linedef (passive: this doesn't tell the linedef and doesn't record)
		internal void SetLinedefP(Linedef ld)
		{
			linedef = ld;
		}
		
		#endregion

		#region ================== Methods
		
		// This removes textures that are not required
		public void RemoveUnneededTextures(bool removemiddle)
		{
			RemoveUnneededTextures(removemiddle, false);
		}
		
		// This removes textures that are not required
		public void RemoveUnneededTextures(bool removemiddle, bool force)
		{
			BeforePropsChange();
			
			// The middle texture can be removed regardless of any sector tag or linedef action
			if(!MiddleRequired() && removemiddle)
			{
				this.texnamemid = "-";
				this.longtexnamemid = MapSet.EmptyLongName;
				General.Map.IsChanged = true;
			}

			// Check if the line or sectors have no action or tags because
			// if they do, any texture on this side could be needed
			if(((linedef.Tag <= 0) && (linedef.Action == 0) && (sector.Tag <= 0) &&
			    ((Other == null) || (Other.sector.Tag <= 0))) ||
			   force)
			{
				if(!HighRequired())
				{
					this.texnamehigh = "-";
					this.longtexnamehigh = MapSet.EmptyLongName;
					General.Map.IsChanged = true;
				}

				if(!LowRequired())
				{
					this.texnamelow = "-";
					this.longtexnamelow = MapSet.EmptyLongName;
					General.Map.IsChanged = true;
				}
			}
		}
		
		/// <summary>
		/// This checks if a texture is required
		/// </summary>
		public bool HighRequired()
		{
			// Doublesided?
			if(Other != null)
			{
				// Texture is required when ceiling of other side is lower
				return (Other.sector.CeilHeight < this.sector.CeilHeight);
			}
			else
			{
				return false;
			}
		}

		/// <summary>
		/// This checks if a texture is required
		/// </summary>
		public bool MiddleRequired()
		{
			// Texture is required when the line is singlesided
			return (Other == null);
		}

		/// <summary>
		/// This checks if a texture is required
		/// </summary>
		public bool LowRequired()
		{
			// Doublesided?
			if(Other != null)
			{
				// Texture is required when floor of other side is higher
				return (Other.sector.FloorHeight > this.sector.FloorHeight);
			}
			else
			{
				return false;
			}
		}

		/// <summary>
		/// This returns the height of the upper wall part. Returns 0 when no upper part exists.
		/// </summary>
		public int GetHighHeight()
		{
			Sidedef other = this.Other;
			if(other != null)
			{
				int top = this.sector.CeilHeight;
				int bottom = other.sector.CeilHeight;
				int height = top - bottom;
				return (height > 0) ? height : 0;
			}
			else
			{
				return 0;
			}
		}

		/// <summary>
		/// This returns the height of the middle wall part.
		/// </summary>
		public int GetMiddleHeight()
		{
			Sidedef other = this.Other;
			if(other != null)
			{
				int top = Math.Min(this.Sector.CeilHeight, other.Sector.CeilHeight);
				int bottom = Math.Max(this.Sector.FloorHeight, other.Sector.FloorHeight);
				int height = top - bottom;
				return (height > 0) ? height : 0;
			}
			else
			{
				int top = this.Sector.CeilHeight;
				int bottom = this.Sector.FloorHeight;
				int height = top - bottom;
				return (height > 0) ? height : 0;
			}
		}

		/// <summary>
		/// This returns the height of the lower wall part. Returns 0 when no lower part exists.
		/// </summary>
		public int GetLowHeight()
		{
			Sidedef other = this.Other;
			if(other != null)
			{
				int top = other.sector.FloorHeight;
				int bottom = this.sector.FloorHeight;
				int height = top - bottom;
				return (height > 0) ? height : 0;
			}
			else
			{
				return 0;
			}
		}
		
		// This creates a checksum from the sidedef properties
		// Used for faster sidedefs compression
		public uint GetChecksum()
		{
			CRC crc = new CRC();
			crc.Add(sector.FixedIndex);
			crc.Add(offsetx);
			crc.Add(offsety);
			crc.Add(longtexnamehigh);
			crc.Add(longtexnamelow);
			crc.Add(longtexnamemid);
			return (uint)(crc.Value & 0x00000000FFFFFFFF);
		}

		// This copies textures to another sidedef
		// And possibly also the offsets
		public void AddTexturesTo(Sidedef s)
		{
			int copyoffsets = 0;

			// s cannot be null
			if(s == null) return;

			s.BeforePropsChange();

			// Upper texture set?
			if((texnamehigh.Length > 0) && (texnamehigh[0] != '-'))
			{
				// Copy upper texture
				s.texnamehigh = texnamehigh;
				s.longtexnamehigh = longtexnamehigh;

				// Counts as a half coice for copying offsets
				copyoffsets += 1;
			}

			// Middle texture set?
			if((texnamemid.Length > 0) && (texnamemid[0] != '-'))
			{
				// Copy middle texture
				s.texnamemid = texnamemid;
				s.longtexnamemid = longtexnamemid;

				// Counts for copying offsets
				copyoffsets += 2;
			}

			// Lower texture set?
			if((texnamelow.Length > 0) && (texnamelow[0] != '-'))
			{
				// Copy middle texture
				s.texnamelow = texnamelow;
				s.longtexnamelow = longtexnamelow;

				// Counts as a half coice for copying offsets
				copyoffsets += 1;
			}

			// Copy offsets also?
			if(copyoffsets >= 2)
			{
				// Copy offsets
				s.offsetx = offsetx;
				s.offsety = offsety;
			}

			General.Map.IsChanged = true;
		}
		
		#endregion

		#region ================== Changes

		// This updates all properties
		public void Update(int offsetx, int offsety, string thigh, string tmid, string tlow)
		{
			BeforePropsChange();
			
			// Apply changes
			this.offsetx = offsetx;
			this.offsety = offsety;
			SetTextureHigh(thigh);
			SetTextureMid(tmid);
			SetTextureLow(tlow);
		}

		// This sets texture
		public void SetTextureHigh(string name)
		{
			BeforePropsChange();
			
			texnamehigh = name;
			longtexnamehigh = Lump.MakeLongName(name);
			General.Map.IsChanged = true;
		}

		// This sets texture
		public void SetTextureMid(string name)
		{
			BeforePropsChange();
			
			texnamemid = name;
			longtexnamemid = Lump.MakeLongName(name);
			General.Map.IsChanged = true;
		}

		// This sets texture
		public void SetTextureLow(string name)
		{
			BeforePropsChange();
			
			texnamelow = name;
			longtexnamelow = Lump.MakeLongName(name);
			General.Map.IsChanged = true;
		}
		
		#endregion
	}
}
