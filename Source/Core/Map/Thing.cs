
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
using CodeImp.DoomBuilder.Geometry;
using CodeImp.DoomBuilder.Rendering;
using CodeImp.DoomBuilder.Config;
using System.Drawing;
using CodeImp.DoomBuilder.IO;
using CodeImp.DoomBuilder.VisualModes;

#endregion

namespace CodeImp.DoomBuilder.Map
{
	public sealed class Thing : SelectableElement
	{
		#region ================== Constants

		public const int NUM_ARGS = 5;

		#endregion

		#region ================== Variables

		// Map
		private MapSet map;

		// Sector
		private Sector sector = null;

		// List items
		private LinkedListNode<Thing> selecteditem;
		
		// Properties
		private int type;
		private Vector3D pos;
		private int angledoom;		// Angle as entered / stored in file
		private float anglerad;		// Angle in radians
		private Dictionary<string, bool> flags;
		private int tag;
		private int action;
		private int[] args;

		// Configuration
		private float size;
		private PixelColor color;
		private bool fixedsize;
		private float iconoffset;	// Arrow or dot coordinate offset on the texture

		#endregion

		#region ================== Properties

		public MapSet Map { get { return map; } }
		public int Type { get { return type; } set { BeforePropsChange(); type = value; } }
		public Vector3D Position { get { return pos; } }
		public float Angle { get { return anglerad; } }
		public int AngleDoom { get { return angledoom; } }
		internal Dictionary<string, bool> Flags { get { return flags; } }
		public int Action { get { return action; } set { BeforePropsChange(); action = value; } }
        /*
         * ano - warning, you should not set args by
         *       Args[some_index] = some_value;
         * because it does not call BeforePropsChange() and therefore
         * it does not create an undo snapshot for this Thing!
        */
        public int[] Args { get { return args; } }
		public float Size { get { return size; } }
		public float IconOffset { get { return iconoffset; } }
		public PixelColor Color { get { return color; } }
		public bool FixedSize { get { return fixedsize; } }
		public int Tag { get { return tag; } set { BeforePropsChange(); tag = value; if((tag < General.Map.FormatInterface.MinTag) || (tag > General.Map.FormatInterface.MaxTag)) throw new ArgumentOutOfRangeException("Tag", "Invalid tag number"); } }
		public Sector Sector { get { return sector; } }

		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		internal Thing(MapSet map, int listindex)
		{
			// Initialize
			this.map = map;
			this.listindex = listindex;
			this.flags = new Dictionary<string, bool>();
			this.args = new int[NUM_ARGS];
			
			if(map == General.Map.Map)
				General.Map.UndoRedo.RecAddThing(this);
			
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
					General.Map.UndoRedo.RecRemThing(this);

				// Remove from main list
				map.RemoveThing(listindex);

				// Remove from sector
				//if(sector != null) sector.DetachThing(sectorlistitem);
				
				// Clean up
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
				General.Map.UndoRedo.RecPrpThing(this);
		}

        // Serialize / deserialize
        // ano - using new keyword to shadow (and get rid of old DB2 warning)
        internal new void ReadWrite(IReadWriteStream s)
		{
			if(!s.IsWriting) BeforePropsChange();
			
			base.ReadWrite(s);

			if(s.IsWriting)
			{
				s.wInt(flags.Count);
				
				foreach(KeyValuePair<string, bool> f in flags)
				{
					s.wString(f.Key);
					s.wBool(f.Value);
				}
			}
			else
			{
				int c; s.rInt(out c);

				flags = new Dictionary<string, bool>(c);
				for(int i = 0; i < c; i++)
				{
					string t; s.rString(out t);
					bool b; s.rBool(out b);
					flags.Add(t, b);
				}
			}
			
			s.rwInt(ref type);
			s.rwVector3D(ref pos);
			s.rwInt(ref angledoom);
			s.rwInt(ref tag);
			s.rwInt(ref action);
			for(int i = 0; i < NUM_ARGS; i++) s.rwInt(ref args[i]);
			
			if(!s.IsWriting)
				anglerad = Angle2D.DoomToReal(angledoom);
		}

		// This copies all properties to another thing
		public void CopyPropertiesTo(Thing t)
		{
			t.BeforePropsChange();
			
			// Copy properties
			t.type = type;
			t.anglerad = anglerad;
			t.angledoom = angledoom;
			t.pos = pos;
			t.flags = new Dictionary<string,bool>(flags);
			t.tag = tag;
			t.action = action;
			t.args = (int[])args.Clone();
			t.size = size;
			t.color = color;
			t.iconoffset = iconoffset;
			t.fixedsize = fixedsize;
			base.CopyPropertiesTo(t);
		}

		// This determines which sector the thing is in and links it
		public void DetermineSector()
		{
			Linedef nl;

			// Find the nearest linedef on the map
			nl = map.NearestLinedef(pos);
			if(nl != null)
			{
				// Check what side of line we are at
				if(nl.SideOfLine(pos) < 0f)
				{
					// Front side
					if(nl.Front != null) sector = nl.Front.Sector; else sector = null;
				}
				else
				{
					// Back side
					if(nl.Back != null) sector = nl.Back.Sector; else sector = null;
				}
			}
			else
			{
				sector = null;
			}
		}

		// This determines which sector the thing is in and links it
		public void DetermineSector(VisualBlockMap blockmap)
		{
			// Find nearest sectors using the blockmap
			List<Sector> possiblesectors = blockmap.GetBlock(blockmap.GetBlockCoordinates(pos)).Sectors;

			// Check in which sector we are
			sector = null;
			foreach(Sector s in possiblesectors)
			{
				if(s.Intersect(pos))
				{
					sector = s;
					break;
				}
			}
		}

		// This translates the flags into UDMF fields
		internal void TranslateToUDMF()
		{
			// First make a single integer with all flags
			int bits = 0;
			int flagbit = 0;
			foreach(KeyValuePair<string, bool> f in flags)
				if(int.TryParse(f.Key, out flagbit) && f.Value) bits |= flagbit;

			// Now make the new flags
			flags.Clear();
			foreach(FlagTranslation f in General.Map.Config.ThingFlagsTranslation)
			{
				// Flag found in bits?
				if((bits & f.Flag) == f.Flag)
				{
					// Add fields and remove bits
					bits &= ~f.Flag;
					for(int i = 0; i < f.Fields.Count; i++)
						flags[f.Fields[i]] = f.FieldValues[i];
				}
				else
				{
					// Add fields with inverted value
					for(int i = 0; i < f.Fields.Count; i++)
						flags[f.Fields[i]] = !f.FieldValues[i];
				}
			}
		}

		// This translates UDMF fields back into the normal flags
		internal void TranslateFromUDMF()
		{
			// Make copy of the flags
			Dictionary<string, bool> oldfields = new Dictionary<string, bool>(flags);

			// Make the flags
			flags.Clear();
			foreach(KeyValuePair<string, string> f in General.Map.Config.ThingFlags)
			{
				// Flag must be numeric
				int flagbit = 0;
				if(int.TryParse(f.Key, out flagbit))
				{
					foreach(FlagTranslation ft in General.Map.Config.ThingFlagsTranslation)
					{
						if(ft.Flag == flagbit)
						{
							// Only set this flag when the fields match
							bool fieldsmatch = true;
							for(int i = 0; i < ft.Fields.Count; i++)
							{
								if(!oldfields.ContainsKey(ft.Fields[i]) || (oldfields[ft.Fields[i]] != ft.FieldValues[i]))
								{
									fieldsmatch = false;
									break;
								}
							}

							// Field match? Then add the flag.
							if(fieldsmatch)
							{
								flags.Add(f.Key, true);
								break;
							}
						}
					}
				}
			}
		}

		// Selected
		protected override void DoSelect()
		{
			base.DoSelect();
			selecteditem = map.SelectedThings.AddLast(this);
		}

		// Deselect
		protected override void DoUnselect()
		{
			base.DoUnselect();
			if(selecteditem.List != null) selecteditem.List.Remove(selecteditem);
			selecteditem = null;
		}
		
		#endregion
		
		#region ================== Changes

		// This moves the thing
		// NOTE: This does not update sector! (call DetermineSector)
		public void Move(Vector3D newpos)
		{
			BeforePropsChange();
			
			// Change position
			this.pos = newpos;
			
			if(type != General.Map.Config.Start3DModeThingType)
				General.Map.IsChanged = true;
		}

		// This moves the thing
		// NOTE: This does not update sector! (call DetermineSector)
		public void Move(Vector2D newpos)
		{
			BeforePropsChange();
			
			// Change position
			this.pos = new Vector3D(newpos.x, newpos.y, pos.z);
			
			if(type != General.Map.Config.Start3DModeThingType)
				General.Map.IsChanged = true;
		}

		// This moves the thing
		// NOTE: This does not update sector! (call DetermineSector)
		public void Move(float x, float y, float zoffset)
		{
			BeforePropsChange();
			
			// Change position
			this.pos = new Vector3D(x, y, zoffset);
			
			if(type != General.Map.Config.Start3DModeThingType)
				General.Map.IsChanged = true;
		}
		
		// This rotates the thing
		public void Rotate(float newangle)
		{
			BeforePropsChange();
			
			// Change angle
			this.anglerad = newangle;
			this.angledoom = Angle2D.RealToDoom(newangle);
			
			if(type != General.Map.Config.Start3DModeThingType)
				General.Map.IsChanged = true;
		}
		
		// This rotates the thing
		public void Rotate(int newangle)
		{
			BeforePropsChange();
			
			// Change angle
			this.anglerad = Angle2D.DoomToReal(newangle);
			this.angledoom = newangle;
			
			if(type != General.Map.Config.Start3DModeThingType)
				General.Map.IsChanged = true;
		}
		
		// This updates all properties
		// NOTE: This does not update sector! (call DetermineSector)
		public void Update(int type, float x, float y, float zoffset, int angle,
						   Dictionary<string, bool> flags, int tag, int action, int[] args)
		{
			// Apply changes
			this.type = type;
			this.anglerad = Angle2D.DoomToReal(angle);
			this.angledoom = angle;
			this.flags = new Dictionary<string, bool>(flags);
			this.tag = tag;
			this.action = action;
			this.args = new int[NUM_ARGS];
			args.CopyTo(this.args, 0);
			this.Move(x, y, zoffset);
		}
		
		// This updates the settings from configuration
		public void UpdateConfiguration()
		{
			ThingTypeInfo ti;
			
			// Lookup settings
			ti = General.Map.Data.GetThingInfo(type);
			
			// Apply size
			size = ti.Radius;
			fixedsize = ti.FixedSize;
			
			// Color valid?
			if((ti.Color >= 0) && (ti.Color < ColorCollection.NUM_THING_COLORS))
			{
				// Apply color
				color = General.Colors.Colors[ti.Color + ColorCollection.THING_COLORS_OFFSET];
			}
			else
			{
				// Unknown thing color
				color = General.Colors.Colors[ColorCollection.THING_COLORS_OFFSET];
			}
			
			// Apply icon offset (arrow or dot)
			if(ti.Arrow) iconoffset = 0f; else iconoffset = 0.25f;
		}
		
		#endregion

		#region ================== Methods
		
		// This checks and returns a flag without creating it
		public bool IsFlagSet(string flagname)
		{
			if(flags.ContainsKey(flagname))
				return flags[flagname];
			else
				return false;
		}
		
		// This sets a flag
		public void SetFlag(string flagname, bool value)
		{
			if(!flags.ContainsKey(flagname) || (IsFlagSet(flagname) != value))
			{
				BeforePropsChange();

				flags[flagname] = value;
			}
		}
		
		// This returns a copy of the flags dictionary
		public Dictionary<string, bool> GetFlags()
		{
			return new Dictionary<string,bool>(flags);
		}

		// This clears all flags
		public void ClearFlags()
		{
			BeforePropsChange();
			
			flags.Clear();
		}
		
		// This snaps the vertex to the grid
		public void SnapToGrid()
		{
			// Calculate nearest grid coordinates
			this.Move(General.Map.Grid.SnappedToGrid((Vector2D)pos));
		}

		// This snaps the vertex to the map format accuracy
		public void SnapToAccuracy()
		{
			// Round the coordinates
			Vector3D newpos = new Vector3D((float)Math.Round(pos.x, General.Map.FormatInterface.VertexDecimals),
										   (float)Math.Round(pos.y, General.Map.FormatInterface.VertexDecimals),
										   (float)Math.Round(pos.z, General.Map.FormatInterface.VertexDecimals));
			this.Move(newpos);
		}
		
		// This returns the distance from given coordinates
		public float DistanceToSq(Vector2D p)
		{
			return Vector2D.DistanceSq(p, pos);
		}

		// This returns the distance from given coordinates
		public float DistanceTo(Vector2D p)
		{
			return Vector2D.Distance(p, pos);
		}

		#endregion
	}
}
