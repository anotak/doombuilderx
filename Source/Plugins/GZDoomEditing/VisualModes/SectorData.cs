#region === Copyright (c) 2010 Pascal van der Heiden ===

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using CodeImp.DoomBuilder.Geometry;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Rendering;

#endregion

namespace CodeImp.DoomBuilder.GZDoomEditing
{
	internal class SectorData
	{
		#region ================== Variables
		
		// VisualMode
		private BaseVisualMode mode;
		
		// Sector for which this data is
		private Sector sector;
		
		// Levels have been updated?
		private bool updated;
		
		// This prevents recursion
		private bool isupdating;
		
		// All planes in the sector that cast or are affected by light
		private List<SectorLevel> lightlevels;
		
		// Effects
		private List<SectorEffect> alleffects;
		private List<Effect3DFloor> extrafloors;
		
		// Sectors that must be updated when this sector is changed
		// The boolean value is the 'includeneighbours' of the UpdateSectorGeometry function which
		// indicates if the sidedefs of neighbouring sectors should also be rebuilt.
		private Dictionary<Sector, bool> updatesectors;
		
		// Original floor and ceiling levels
		private SectorLevel floor;
		private SectorLevel ceiling;
		
		// This helps keeping track of changes
		// otherwise we update ceiling/floor too much
		private bool floorchanged;
		private bool ceilingchanged;

		#endregion
		
		#region ================== Properties
		
		public Sector Sector { get { return sector; } }
		public bool Updated { get { return updated; } }
		public bool FloorChanged { get { return floorchanged; } set { floorchanged |= value; } }
		public bool CeilingChanged { get { return ceilingchanged; } set { ceilingchanged |= value; } }
		public List<SectorLevel> LightLevels { get { return lightlevels; } }
		public List<Effect3DFloor> ExtraFloors { get { return extrafloors; } }
		public SectorLevel Floor { get { return floor; } }
		public SectorLevel Ceiling { get { return ceiling; } }
		public BaseVisualMode Mode { get { return mode; } }
		public Dictionary<Sector, bool> UpdateAlso { get { return updatesectors; } }
		
		#endregion
		
		#region ================== Constructor / Destructor
		
		// Constructor
		public SectorData(BaseVisualMode mode, Sector s)
		{
			// Initialize
			this.mode = mode;
			this.sector = s;
			this.updated = false;
			this.floorchanged = false;
			this.ceilingchanged = false;
			this.lightlevels = new List<SectorLevel>(2);
			this.extrafloors = new List<Effect3DFloor>(1);
			this.alleffects = new List<SectorEffect>(1);
			this.updatesectors = new Dictionary<Sector, bool>(2);
			this.floor = new SectorLevel(sector, SectorLevelType.Floor);
			this.ceiling = new SectorLevel(sector, SectorLevelType.Ceiling);
			
			BasicSetup();
			
			// Add ceiling and floor
			lightlevels.Add(floor);
			lightlevels.Add(ceiling);
		}
		
		#endregion
		
		#region ================== Public Methods
		
		// 3D Floor effect
		public void AddEffect3DFloor(Linedef sourcelinedef)
		{
			Effect3DFloor e = new Effect3DFloor(this, sourcelinedef);
			extrafloors.Add(e);
			alleffects.Add(e);
		}
		
		// Brightness level effect
		public void AddEffectBrightnessLevel(Linedef sourcelinedef)
		{
			EffectBrightnessLevel e = new EffectBrightnessLevel(this, sourcelinedef);
			alleffects.Add(e);
		}

		// Line slope effect
		public void AddEffectLineSlope(Linedef sourcelinedef)
		{
			EffectLineSlope e = new EffectLineSlope(this, sourcelinedef);
			alleffects.Add(e);
		}

		// Copy slope effect
		public void AddEffectCopySlope(Thing sourcething)
		{
			EffectCopySlope e = new EffectCopySlope(this, sourcething);
			alleffects.Add(e);
		}

		// Thing line slope effect
		public void AddEffectThingLineSlope(Thing sourcething)
		{
			EffectThingLineSlope e = new EffectThingLineSlope(this, sourcething);
			alleffects.Add(e);
		}

		// Thing vertex slope effect
		public void AddEffectThingVertexSlope(List<Thing> sourcethings, bool slopefloor)
		{
			EffectThingVertexSlope e = new EffectThingVertexSlope(this, sourcethings, slopefloor);
			alleffects.Add(e);
		}
		
		// This adds a sector for updating
		public void AddUpdateSector(Sector s, bool includeneighbours)
		{
			updatesectors[s] = includeneighbours;
		}
		
		// This adds a sector level
		public void AddSectorLevel(SectorLevel level)
		{
			// Note: Inserting before the end so that the ceiling stays
			// at the end and the floor at the beginning
			lightlevels.Insert(lightlevels.Count - 1, level);
		}
		
		// This resets this sector data and all sectors that require updating after me
		public void Reset()
		{
			if(isupdating)
				return;

			isupdating = true;

			// This is set to false so that this sector is rebuilt the next time it is needed!
			updated = false;

			// The visual sector associated is now outdated
			if(mode.VisualSectorExists(sector))
			{
				BaseVisualSector vs = (BaseVisualSector)mode.GetVisualSector(sector);
				vs.UpdateSectorGeometry(false);
			}
			
			// Also reset the sectors that depend on this sector
			foreach(KeyValuePair<Sector, bool> s in updatesectors)
			{
				SectorData sd = mode.GetSectorData(s.Key);
				sd.Reset();
			}

			isupdating = false;
		}

		// This sets up the basic floor and ceiling, as they would be in normal Doom circumstances
		private void BasicSetup()
		{
			// Normal (flat) floor and ceiling planes
			floor.plane = new Plane(new Vector3D(0, 0, 1), -sector.FloorHeight);
			ceiling.plane = new Plane(new Vector3D(0, 0, -1), sector.CeilHeight);
			
			// Fetch ZDoom fields
			int color = sector.Fields.GetValue("lightcolor", -1);
			int flight = sector.Fields.GetValue("lightfloor", 0);
			bool fabs = sector.Fields.GetValue("lightfloorabsolute", false);
			int clight = sector.Fields.GetValue("lightceiling", 0);
			bool cabs = sector.Fields.GetValue("lightceilingabsolute", false);

			// Determine colors & light levels
			PixelColor lightcolor = PixelColor.FromInt(color);
			if(!fabs) flight = sector.Brightness + flight;
			if(!cabs) clight = sector.Brightness + clight;
			PixelColor floorbrightness = PixelColor.FromInt(mode.CalculateBrightness(flight));
			PixelColor ceilingbrightness = PixelColor.FromInt(mode.CalculateBrightness(clight));
			PixelColor floorcolor = PixelColor.Modulate(lightcolor, floorbrightness);
			PixelColor ceilingcolor = PixelColor.Modulate(lightcolor, ceilingbrightness);
			floor.color = floorcolor.WithAlpha(255).ToInt();
			floor.brightnessbelow = sector.Brightness;
			floor.colorbelow = lightcolor.WithAlpha(255);
			ceiling.color = ceilingcolor.WithAlpha(255).ToInt();
			ceiling.brightnessbelow = sector.Brightness;
			ceiling.colorbelow = lightcolor.WithAlpha(255);
		}

		// When no geometry has been changed and no effects have been added or removed,
		// you can call this again to update existing effects. The effects will update
		// the existing SectorLevels to match with any changes.
		public void Update()
		{
			if(isupdating || updated) return;
			isupdating = true;
			
			// Set floor/ceiling to their original setup
			BasicSetup();

			// Update all effects
			foreach(SectorEffect e in alleffects)
				e.Update();
			
			// Sort the levels (but not the first and the last)
			SectorLevelComparer comparer = new SectorLevelComparer(sector);
			lightlevels.Sort(1, lightlevels.Count - 2, comparer);
			
			// Now that we know the levels in this sector (and in the right order) we
			// can determine the lighting in between and on the levels.
			// Start from the absolute ceiling and go down to 'cast' the lighting
			for(int i = lightlevels.Count - 2; i >= 0; i--)
			{
				SectorLevel l = lightlevels[i];
				SectorLevel pl = lightlevels[i + 1];
				
				// Set color when no color is specified, or when a 3D floor is placed above the absolute floor
				if((l.color == 0) || ((l == floor) && (lightlevels.Count > 2)))
				{
					PixelColor floorbrightness = PixelColor.FromInt(mode.CalculateBrightness(pl.brightnessbelow));
					PixelColor floorcolor = PixelColor.Modulate(pl.colorbelow, floorbrightness);
					l.color = floorcolor.WithAlpha(255).ToInt();
				}
				
				if(l.colorbelow.a == 0)
					l.colorbelow = pl.colorbelow;
				
				if(l.brightnessbelow == -1)
					l.brightnessbelow = pl.brightnessbelow;
			}

			floorchanged = false;
			ceilingchanged = false;
			updated = true;
			isupdating = false;
		}

		// This returns the level above the given point
		public SectorLevel GetLevelAbove(Vector3D pos)
		{
			SectorLevel found = null;
			float dist = float.MaxValue;
			
			foreach(SectorLevel l in lightlevels)
			{
				float d = l.plane.GetZ(pos) - pos.z;
				if((d > 0.0f) && (d < dist))
				{
					dist = d;
					found = l;
				}
			}
			
			return found;
		}

		// This returns the level above the given point
		public SectorLevel GetCeilingAbove(Vector3D pos)
		{
			SectorLevel found = null;
			float dist = float.MaxValue;

			foreach(SectorLevel l in lightlevels)
			{
				if(l.type == SectorLevelType.Ceiling)
				{
					float d = l.plane.GetZ(pos) - pos.z;
					if((d > 0.0f) && (d < dist))
					{
						dist = d;
						found = l;
					}
				}
			}

			return found;
		}

		// This returns the level below the given point
		public SectorLevel GetLevelBelow(Vector3D pos)
		{
			SectorLevel found = null;
			float dist = float.MaxValue;

			foreach(SectorLevel l in lightlevels)
			{
				float d = pos.z - l.plane.GetZ(pos);
				if((d > 0.0f) && (d < dist))
				{
					dist = d;
					found = l;
				}
			}

			return found;
		}

		// This returns the floor below the given point
		public SectorLevel GetFloorBelow(Vector3D pos)
		{
			SectorLevel found = null;
			float dist = float.MaxValue;

			foreach(SectorLevel l in lightlevels)
			{
				if(l.type == SectorLevelType.Floor)
				{
					float d = pos.z - l.plane.GetZ(pos);
					if((d > 0.0f) && (d < dist))
					{
						dist = d;
						found = l;
					}
				}
			}

			return found;
		}

		
		#endregion
	}
}
