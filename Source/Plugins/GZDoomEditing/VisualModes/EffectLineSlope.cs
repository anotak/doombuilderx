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
	internal class EffectLineSlope : SectorEffect
	{
		// Linedef that is used to create this effect
		private Linedef linedef;
		
		// Constructor
		public EffectLineSlope(SectorData data, Linedef sourcelinedef) : base(data)
		{
			linedef = sourcelinedef;
			
			// New effect added: This sector needs an update!
			if(data.Mode.VisualSectorExists(data.Sector))
			{
				BaseVisualSector vs = (BaseVisualSector)data.Mode.GetVisualSector(data.Sector);
				vs.UpdateSectorGeometry(true);
			}
		}
		
		// This makes sure we are updated with the source linedef information
		public override void Update()
		{
			Linedef l = linedef;
			
			// Find the vertex furthest from the line
			Vertex foundv = null;
			float founddist = -1.0f;
			foreach(Sidedef sd in data.Sector.Sidedefs)
			{
				Vertex v = sd.IsFront ? sd.Line.Start : sd.Line.End;
				float d = l.DistanceToSq(v.Position, false);
				if(d > founddist)
				{
					foundv = v;
					founddist = d;
				}
			}
			
			// Align floor with back of line
			if((l.Args[0] == 1) && (l.Front.Sector == data.Sector) && (l.Back != null))
			{
				Vector3D v1 = new Vector3D(l.Start.Position.x, l.Start.Position.y, l.Back.Sector.FloorHeight);
				Vector3D v2 = new Vector3D(l.End.Position.x, l.End.Position.y, l.Back.Sector.FloorHeight);
				Vector3D v3 = new Vector3D(foundv.Position.x, foundv.Position.y, data.Sector.FloorHeight);
				if(l.SideOfLine(v3) < 0.0f)
					data.Floor.plane = new Plane(v1, v2, v3, true);
				else
					data.Floor.plane = new Plane(v2, v1, v3, true);
				SectorData sd = data.Mode.GetSectorData(l.Back.Sector);
				sd.AddUpdateSector(data.Sector, true);
			}
			// Align floor with front of line
			else if((l.Args[0] == 2) && (l.Back.Sector == data.Sector) && (l.Front != null))
			{
				Vector3D v1 = new Vector3D(l.Start.Position.x, l.Start.Position.y, l.Front.Sector.FloorHeight);
				Vector3D v2 = new Vector3D(l.End.Position.x, l.End.Position.y, l.Front.Sector.FloorHeight);
				Vector3D v3 = new Vector3D(foundv.Position.x, foundv.Position.y, data.Sector.FloorHeight);
				if(l.SideOfLine(v3) < 0.0f)
					data.Floor.plane = new Plane(v1, v2, v3, true);
				else
					data.Floor.plane = new Plane(v2, v1, v3, true);
				SectorData sd = data.Mode.GetSectorData(l.Front.Sector);
				sd.AddUpdateSector(data.Sector, true);
			}
			
			// Align ceiling with back of line
			if((l.Args[1] == 1) && (l.Front.Sector == data.Sector) && (l.Back != null))
			{
				Vector3D v1 = new Vector3D(l.Start.Position.x, l.Start.Position.y, l.Back.Sector.CeilHeight);
				Vector3D v2 = new Vector3D(l.End.Position.x, l.End.Position.y, l.Back.Sector.CeilHeight);
				Vector3D v3 = new Vector3D(foundv.Position.x, foundv.Position.y, data.Sector.CeilHeight);
				if(l.SideOfLine(v3) > 0.0f)
					data.Ceiling.plane = new Plane(v1, v2, v3, false);
				else
					data.Ceiling.plane = new Plane(v2, v1, v3, false);
				SectorData sd = data.Mode.GetSectorData(l.Back.Sector);
				sd.AddUpdateSector(data.Sector, true);
			}
			// Align ceiling with front of line
			else if((l.Args[1] == 2) && (l.Back.Sector == data.Sector) && (l.Front != null))
			{
				Vector3D v1 = new Vector3D(l.Start.Position.x, l.Start.Position.y, l.Front.Sector.CeilHeight);
				Vector3D v2 = new Vector3D(l.End.Position.x, l.End.Position.y, l.Front.Sector.CeilHeight);
				Vector3D v3 = new Vector3D(foundv.Position.x, foundv.Position.y, data.Sector.CeilHeight);
				if(l.SideOfLine(v3) > 0.0f)
					data.Ceiling.plane = new Plane(v1, v2, v3, false);
				else
					data.Ceiling.plane = new Plane(v2, v1, v3, false);
				SectorData sd = data.Mode.GetSectorData(l.Front.Sector);
				sd.AddUpdateSector(data.Sector, true);
			}
		}
	}
}
