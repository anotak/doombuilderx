
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
using System.IO;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Geometry;

#endregion

namespace CodeImp.DoomBuilder.IO
{
	internal class HexenMapSetIO : MapSetIO
	{
		#region ================== Constants

		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		public HexenMapSetIO(WAD wad, MapManager manager) : base(wad, manager)
		{
		}

		#endregion

		#region ================== Properties

		public override int MaxSidedefs { get { return ushort.MaxValue; } }
		public override int MaxVertices { get { return ushort.MaxValue; } }
		public override int MaxLinedefs { get { return ushort.MaxValue; } }
		public override int MaxSectors { get { return ushort.MaxValue; } }
		public override int MaxThings { get { return int.MaxValue; } }
		public override int MinTextureOffset { get { return short.MinValue; } }
		public override int MaxTextureOffset { get { return short.MaxValue; } }
		public override int VertexDecimals { get { return 0; } }
        public override float MinLineLength { get { return 1f; } } // ano - related to VertexDecimals
        public override float StitchDistance { get { return 0.1f; } } // ano - related to VertexDecimals and MinLineLength
        public override string DecimalsFormat { get { return "0"; } }
		public override bool HasLinedefTag { get { return false; } }
		public override bool HasThingTag { get { return true; } }
		public override bool HasThingAction { get { return true; } }
		public override bool HasCustomFields { get { return false; } }
		public override bool HasThingHeight { get { return true; } }
		public override bool HasActionArgs { get { return true; } }
		public override bool HasMixedActivations { get { return false; } }
		public override bool HasPresetActivations { get { return true; } }
		public override bool HasBuiltInActivations { get { return false; } }
		public override bool HasNumericLinedefFlags { get { return true; } }
		public override bool HasNumericThingFlags { get { return true; } }
		public override bool HasNumericLinedefActivations { get { return true; } }
		public override int MaxTag { get { return ushort.MaxValue; } }
		public override int MinTag { get { return ushort.MinValue; } }
		public override int MaxAction { get { return byte.MaxValue; } }
		public override int MinAction { get { return byte.MinValue; } }
		public override int MaxArgument { get { return byte.MaxValue; } }
		public override int MinArgument { get { return byte.MinValue; } }
		public override int MaxEffect { get { return ushort.MaxValue; } }
		public override int MinEffect { get { return ushort.MinValue; } }
		public override int MaxBrightness { get { return short.MaxValue; } }
		public override int MinBrightness { get { return short.MinValue; } }
		public override int MaxThingType { get { return ushort.MaxValue; } }
		public override int MinThingType { get { return ushort.MinValue; } }
		public override double MaxCoordinate { get { return (double)short.MaxValue; } }
		public override double MinCoordinate { get { return (double)short.MinValue; } }
		public override int MaxThingAngle { get { return short.MaxValue; } }
		public override int MinThingAngle { get { return short.MinValue; } }
		
		#endregion

		#region ================== Reading

		// This reads a map from the file and returns a MapSet
		public override MapSet Read(MapSet map, string mapname)
		{
			int firstindex;
			Dictionary<int, Vertex> vertexlink;
			Dictionary<int, Sector> sectorlink;
			
			// Find the index where first map lump begins
			firstindex = wad.FindLumpIndex(mapname) + 1;

			// Read vertices
			vertexlink = ReadVertices(map, firstindex);

			// Read sectors
			sectorlink = ReadSectors(map, firstindex);

			// Read linedefs and sidedefs
			ReadLinedefs(map, firstindex, vertexlink, sectorlink);

			// Read things
			ReadThings(map, firstindex);
			
			// Remove unused vertices
			map.RemoveUnusedVertices();
			
			// Return result;
			return map;
		}

		// This reads the THINGS from WAD file
		private void ReadThings(MapSet map, int firstindex)
		{
			MemoryStream mem;
			BinaryReader reader;
			int num, i, tag, z, action, x, y, type, flags, angle;
			int[] args = new int[Thing.NUM_ARGS];
			Dictionary<string, bool> stringflags;
			Thing t;
			
			// Get the lump from wad file
			Lump lump = wad.FindLump("THINGS", firstindex);
			if(lump == null) throw new Exception("Could not find required lump THINGS!");
			
			// Prepare to read the items
			mem = new MemoryStream(lump.Stream.ReadAllBytes());
			num = (int)lump.Stream.Length / 20;
			reader = new BinaryReader(mem);
			
			// Read items from the lump
			map.SetCapacity(0, 0, 0, 0, map.Things.Count + num);
			for(i = 0; i < num; i++)
			{
				// Read properties from stream
				tag = reader.ReadUInt16();
				x = reader.ReadInt16();
				y = reader.ReadInt16();
				z = reader.ReadInt16();
				angle = reader.ReadInt16();
				type = reader.ReadUInt16();
				flags = reader.ReadUInt16();
				action = reader.ReadByte();
				args[0] = reader.ReadByte();
				args[1] = reader.ReadByte();
				args[2] = reader.ReadByte();
				args[3] = reader.ReadByte();
				args[4] = reader.ReadByte();

				// Make string flags
				stringflags = new Dictionary<string, bool>();
				foreach(KeyValuePair<string, string> f in manager.Config.ThingFlags)
				{
					int fnum;
					if(int.TryParse(f.Key, out fnum)) stringflags[f.Key] = ((flags & fnum) == fnum);
				}
				
				// Create new item
				t = map.CreateThing();
				t.Update(type, x, y, z, angle, stringflags, tag, action, args);
			}

			// Done
			mem.Dispose();
		}

		// This reads the VERTICES from WAD file
		// Returns a lookup table with indices
		private Dictionary<int, Vertex> ReadVertices(MapSet map, int firstindex)
		{
			MemoryStream mem;
			Dictionary<int, Vertex> link;
			BinaryReader reader;
			int num, i, x, y;
			Vertex v;
			
			// Get the lump from wad file
			Lump lump = wad.FindLump("VERTEXES", firstindex);
			if(lump == null) throw new Exception("Could not find required lump VERTEXES!");

			// Prepare to read the items
			mem = new MemoryStream(lump.Stream.ReadAllBytes());
			num = (int)lump.Stream.Length / 4;
			reader = new BinaryReader(mem);

			// Create lookup table
			link = new Dictionary<int, Vertex>(num);

			// Read items from the lump
			map.SetCapacity(map.Vertices.Count + num, 0, 0, 0, 0);
			for(i = 0; i < num; i++)
			{
				// Read properties from stream
				x = reader.ReadInt16();
				y = reader.ReadInt16();

				// Create new item
				v = map.CreateVertex(new Vector2D((float)x, (float)y));
				
				// Add it to the lookup table
				link.Add(i, v);
			}

			// Done
			mem.Dispose();

			// Return lookup table
			return link;
		}

		// This reads the SECTORS from WAD file
		// Returns a lookup table with indices
		private Dictionary<int, Sector> ReadSectors(MapSet map, int firstindex)
		{
			MemoryStream mem;
			Dictionary<int, Sector> link;
			BinaryReader reader;
			int num, i, hfloor, hceil, bright, special, tag;
			string tfloor, tceil;
			Sector s;

			// Get the lump from wad file
			Lump lump = wad.FindLump("SECTORS", firstindex);
			if(lump == null) throw new Exception("Could not find required lump SECTORS!");

			// Prepare to read the items
			mem = new MemoryStream(lump.Stream.ReadAllBytes());
			num = (int)lump.Stream.Length / 26;
			reader = new BinaryReader(mem);

			// Create lookup table
			link = new Dictionary<int, Sector>(num);

			// Read items from the lump
			map.SetCapacity(0, 0, 0, map.Sectors.Count + num, 0);
			for(i = 0; i < num; i++)
			{
				// Read properties from stream
				hfloor = reader.ReadInt16();
				hceil = reader.ReadInt16();
				tfloor = Lump.MakeNormalName(reader.ReadBytes(8), WAD.ENCODING);
				tceil = Lump.MakeNormalName(reader.ReadBytes(8), WAD.ENCODING);
				bright = reader.ReadInt16();
				special = reader.ReadUInt16();
				tag = reader.ReadUInt16();
				
				// Create new item
				s = map.CreateSector();
				s.Update(hfloor, hceil, tfloor, tceil, special, tag, bright);

				// Add it to the lookup table
				link.Add(i, s);
			}

			// Done
			mem.Dispose();

			// Return lookup table
			return link;
		}
		
		// This reads the LINEDEFS and SIDEDEFS from WAD file
		private void ReadLinedefs(MapSet map, int firstindex,
			Dictionary<int, Vertex> vertexlink, Dictionary<int, Sector> sectorlink)
		{
			MemoryStream linedefsmem, sidedefsmem;
			BinaryReader readline, readside;
			Lump linedefslump, sidedefslump;
			int num, numsides, i, offsetx, offsety, v1, v2;
			int s1, s2, flags, action, sc;
			int[] args = new int[Linedef.NUM_ARGS];
			Dictionary<string, bool> stringflags;
			string thigh, tmid, tlow;
			Linedef l;
			Sidedef s;

			// Get the linedefs lump from wad file
			linedefslump = wad.FindLump("LINEDEFS", firstindex);
			if(linedefslump == null) throw new Exception("Could not find required lump LINEDEFS!");

			// Get the sidedefs lump from wad file
			sidedefslump = wad.FindLump("SIDEDEFS", firstindex);
			if(sidedefslump == null) throw new Exception("Could not find required lump SIDEDEFS!");

			// Prepare to read the items
			linedefsmem = new MemoryStream(linedefslump.Stream.ReadAllBytes());
			sidedefsmem = new MemoryStream(sidedefslump.Stream.ReadAllBytes());
			num = (int)linedefslump.Stream.Length / 16;
			numsides = (int)sidedefslump.Stream.Length / 30;
			readline = new BinaryReader(linedefsmem);
			readside = new BinaryReader(sidedefsmem);

			// Read items from the lump
			map.SetCapacity(0, map.Linedefs.Count + num, map.Sidedefs.Count + numsides, 0, 0);
			for(i = 0; i < num; i++)
			{
				// Read properties from stream
				v1 = readline.ReadUInt16();
				v2 = readline.ReadUInt16();
				flags = readline.ReadUInt16();
				action = readline.ReadByte();
				args[0] = readline.ReadByte();
				args[1] = readline.ReadByte();
				args[2] = readline.ReadByte();
				args[3] = readline.ReadByte();
				args[4] = readline.ReadByte();
				s1 = readline.ReadUInt16();
				s2 = readline.ReadUInt16();
				
				// Make string flags
				stringflags = new Dictionary<string, bool>();
				foreach(string f in manager.Config.SortedLinedefFlags)
				{
					int fnum;
					if(int.TryParse(f, out fnum)) stringflags[f] = ((flags & fnum) == fnum);
				}
				
				// Create new linedef
				if(vertexlink.ContainsKey(v1) && vertexlink.ContainsKey(v2))
				{
					// Check if not zero-length
					if(Vector2D.ManhattanDistance(vertexlink[v1].Position, vertexlink[v2].Position) > 0.0001f)
					{
						l = map.CreateLinedef(vertexlink[v1], vertexlink[v2]);
						l.Update(stringflags, (flags & manager.Config.LinedefActivationsFilter), 0, action, args);
						l.UpdateCache();

						// Line has a front side?
						if(s1 != ushort.MaxValue)
						{
							// Read front sidedef
							sidedefsmem.Seek(s1 * 30, SeekOrigin.Begin);
							if((s1 * 30L) <= (sidedefsmem.Length - 30L))
							{
								offsetx = readside.ReadInt16();
								offsety = readside.ReadInt16();
								thigh = Lump.MakeNormalName(readside.ReadBytes(8), WAD.ENCODING);
								tlow = Lump.MakeNormalName(readside.ReadBytes(8), WAD.ENCODING);
								tmid = Lump.MakeNormalName(readside.ReadBytes(8), WAD.ENCODING);
								sc = readside.ReadUInt16();

								// Create front sidedef
								if(sectorlink.ContainsKey(sc))
								{
									s = map.CreateSidedef(l, true, sectorlink[sc]);
									s.Update(offsetx, offsety, thigh, tmid, tlow);
								}
								else
								{
									General.ErrorLogger.Add(ErrorType.Warning, "Sidedef " + s1 + " references invalid sector " + sc + ". Sidedef has been removed.");
								}
							}
							else
							{
								General.ErrorLogger.Add(ErrorType.Warning, "Linedef " + i + " references invalid sidedef " + s1 + ". Sidedef has been removed.");
							}
						}

						// Line has a back side?
						if(s2 != ushort.MaxValue)
						{
							// Read back sidedef
							sidedefsmem.Seek(s2 * 30, SeekOrigin.Begin);
							if((s2 * 30L) <= (sidedefsmem.Length - 30L))
							{
								offsetx = readside.ReadInt16();
								offsety = readside.ReadInt16();
								thigh = Lump.MakeNormalName(readside.ReadBytes(8), WAD.ENCODING);
								tlow = Lump.MakeNormalName(readside.ReadBytes(8), WAD.ENCODING);
								tmid = Lump.MakeNormalName(readside.ReadBytes(8), WAD.ENCODING);
								sc = readside.ReadUInt16();

								// Create back sidedef
								if(sectorlink.ContainsKey(sc))
								{
									s = map.CreateSidedef(l, false, sectorlink[sc]);
									s.Update(offsetx, offsety, thigh, tmid, tlow);
								}
								else
								{
									General.ErrorLogger.Add(ErrorType.Warning, "Sidedef " + s2 + " references invalid sector " + sc + ". Sidedef has been removed.");
								}
							}
							else
							{
								General.ErrorLogger.Add(ErrorType.Warning, "Linedef " + i + " references invalid sidedef " + s2 + ". Sidedef has been removed.");
							}
						}
					}
					else
					{
						General.ErrorLogger.Add(ErrorType.Warning, "Linedef " + i + " is zero-length. Linedef has been removed.");
					}
				}
				else
				{
					General.ErrorLogger.Add(ErrorType.Warning, "Linedef " + i + " references one or more invalid vertices. Linedef has been removed.");
				}
			}

			// Done
			linedefsmem.Dispose();
			sidedefsmem.Dispose();
		}
		
		#endregion

		#region ================== Writing

		// This writes a MapSet to the file
		public override void Write(MapSet map, string mapname, int position)
		{
			Dictionary<Vertex, int> vertexids = new Dictionary<Vertex,int>();
			Dictionary<Sidedef, int> sidedefids = new Dictionary<Sidedef,int>();
			Dictionary<Sector, int> sectorids = new Dictionary<Sector,int>();
			
			// First index everything
			foreach(Vertex v in map.Vertices) vertexids.Add(v, vertexids.Count);
			foreach(Sidedef sd in map.Sidedefs) sidedefids.Add(sd, sidedefids.Count);
			foreach(Sector s in map.Sectors) sectorids.Add(s, sectorids.Count);
			
			// Write lumps to wad (note the backwards order because they
			// are all inserted at position+1 when not found)
			WriteSectors(map, position, manager.Config.MapLumpNames);
			WriteVertices(map, position, manager.Config.MapLumpNames);
			WriteSidedefs(map, position, manager.Config.MapLumpNames, sectorids);
			WriteLinedefs(map, position, manager.Config.MapLumpNames, sidedefids, vertexids);
			WriteThings(map, position, manager.Config.MapLumpNames);
		}

		// This writes the THINGS to WAD file
		private void WriteThings(MapSet map, int position, IDictionary maplumps)
		{
			MemoryStream mem;
			BinaryWriter writer;
			Lump lump;
			int insertpos;
			int flags;
			
			// Create memory to write to
			mem = new MemoryStream();
			writer = new BinaryWriter(mem, WAD.ENCODING);
			
			// Go for all things
			foreach(Thing t in map.Things)
			{
				// Convert flags
				flags = 0;
				foreach(KeyValuePair<string, bool> f in t.Flags)
				{
					int fnum;
					if(f.Value && int.TryParse(f.Key, out fnum)) flags |= fnum;
				}

				// Write properties to stream
				// Write properties to stream
				writer.Write((UInt16)t.Tag);
				writer.Write((Int16)t.Position.x);
				writer.Write((Int16)t.Position.y);
				writer.Write((Int16)t.Position.z);
				writer.Write((Int16)t.AngleDoom);
				writer.Write((UInt16)t.Type);
				writer.Write((UInt16)flags);
				writer.Write((Byte)t.Action);
				writer.Write((Byte)t.Args[0]);
				writer.Write((Byte)t.Args[1]);
				writer.Write((Byte)t.Args[2]);
				writer.Write((Byte)t.Args[3]);
				writer.Write((Byte)t.Args[4]);
			}
			
			// Find insert position and remove old lump
			insertpos = MapManager.RemoveSpecificLump(wad, "THINGS", position, MapManager.TEMP_MAP_HEADER, maplumps);
			if(insertpos == -1) insertpos = position + 1;
			if(insertpos > wad.Lumps.Count) insertpos = wad.Lumps.Count;
			
			// Create the lump from memory
			lump = wad.Insert("THINGS", insertpos, (int)mem.Length);
			lump.Stream.Seek(0, SeekOrigin.Begin);
			mem.WriteTo(lump.Stream);
		}

		// This writes the VERTEXES to WAD file
		private void WriteVertices(MapSet map, int position, IDictionary maplumps)
		{
			MemoryStream mem;
			BinaryWriter writer;
			Lump lump;
			int insertpos;

			// Create memory to write to
			mem = new MemoryStream();
			writer = new BinaryWriter(mem, WAD.ENCODING);

			// Go for all vertices
			foreach(Vertex v in map.Vertices)
			{
				// Write properties to stream
				writer.Write((Int16)(int)Math.Round(v.Position.x));
				writer.Write((Int16)(int)Math.Round(v.Position.y));
			}

			// Find insert position and remove old lump
			insertpos = MapManager.RemoveSpecificLump(wad, "VERTEXES", position, MapManager.TEMP_MAP_HEADER, maplumps);
			if(insertpos == -1) insertpos = position + 1;
			if(insertpos > wad.Lumps.Count) insertpos = wad.Lumps.Count;

			// Create the lump from memory
			lump = wad.Insert("VERTEXES", insertpos, (int)mem.Length);
			lump.Stream.Seek(0, SeekOrigin.Begin);
			mem.WriteTo(lump.Stream);
		}

		// This writes the LINEDEFS to WAD file
		private void WriteLinedefs(MapSet map, int position, IDictionary maplumps, IDictionary<Sidedef, int> sidedefids, IDictionary<Vertex, int> vertexids)
		{
			MemoryStream mem;
			BinaryWriter writer;
			Lump lump;
			ushort sid;
			int insertpos;
			int flags;
			
			// Create memory to write to
			mem = new MemoryStream();
			writer = new BinaryWriter(mem, WAD.ENCODING);

			// Go for all lines
			foreach(Linedef l in map.Linedefs)
			{
				// Convert flags
				flags = 0;
				foreach(KeyValuePair<string, bool> f in l.Flags)
				{
					int fnum;
					if(f.Value && int.TryParse(f.Key, out fnum)) flags |= fnum;
				}

				// Add activates to flags
				flags |= (l.Activate & manager.Config.LinedefActivationsFilter);
				
				// Write properties to stream
				writer.Write((UInt16)vertexids[l.Start]);
				writer.Write((UInt16)vertexids[l.End]);
				writer.Write((UInt16)flags);
				writer.Write((Byte)l.Action);
				writer.Write((Byte)l.Args[0]);
				writer.Write((Byte)l.Args[1]);
				writer.Write((Byte)l.Args[2]);
				writer.Write((Byte)l.Args[3]);
				writer.Write((Byte)l.Args[4]);

				// Front sidedef
				if(l.Front == null) sid = ushort.MaxValue;
					else sid = (UInt16)sidedefids[l.Front];
				writer.Write(sid);

				// Back sidedef
				if(l.Back == null) sid = ushort.MaxValue;
					else sid = (UInt16)sidedefids[l.Back];
				writer.Write(sid);
			}

			// Find insert position and remove old lump
			insertpos = MapManager.RemoveSpecificLump(wad, "LINEDEFS", position, MapManager.TEMP_MAP_HEADER, maplumps);
			if(insertpos == -1) insertpos = position + 1;
			if(insertpos > wad.Lumps.Count) insertpos = wad.Lumps.Count;

			// Create the lump from memory
			lump = wad.Insert("LINEDEFS", insertpos, (int)mem.Length);
			lump.Stream.Seek(0, SeekOrigin.Begin);
			mem.WriteTo(lump.Stream);
		}

		// This writes the SIDEDEFS to WAD file
		private void WriteSidedefs(MapSet map, int position, IDictionary maplumps, IDictionary<Sector, int> sectorids)
		{
			MemoryStream mem;
			BinaryWriter writer;
			Lump lump;
			int insertpos;

			// Create memory to write to
			mem = new MemoryStream();
			writer = new BinaryWriter(mem, WAD.ENCODING);

			// Go for all sidedefs
			foreach(Sidedef sd in map.Sidedefs)
			{
				// Write properties to stream
				writer.Write((Int16)sd.OffsetX);
				writer.Write((Int16)sd.OffsetY);
				writer.Write(Lump.MakeFixedName(sd.HighTexture, WAD.ENCODING));
				writer.Write(Lump.MakeFixedName(sd.LowTexture, WAD.ENCODING));
				writer.Write(Lump.MakeFixedName(sd.MiddleTexture, WAD.ENCODING));
				writer.Write((UInt16)sectorids[sd.Sector]);
			}

			// Find insert position and remove old lump
			insertpos = MapManager.RemoveSpecificLump(wad, "SIDEDEFS", position, MapManager.TEMP_MAP_HEADER, maplumps);
			if(insertpos == -1) insertpos = position + 1;
			if(insertpos > wad.Lumps.Count) insertpos = wad.Lumps.Count;

			// Create the lump from memory
			lump = wad.Insert("SIDEDEFS", insertpos, (int)mem.Length);
			lump.Stream.Seek(0, SeekOrigin.Begin);
			mem.WriteTo(lump.Stream);
		}

		// This writes the SECTORS to WAD file
		private void WriteSectors(MapSet map, int position, IDictionary maplumps)
		{
			MemoryStream mem;
			BinaryWriter writer;
			Lump lump;
			int insertpos;

			// Create memory to write to
			mem = new MemoryStream();
			writer = new BinaryWriter(mem, WAD.ENCODING);

			// Go for all sectors
			foreach(Sector s in map.Sectors)
			{
				// Write properties to stream
				writer.Write((Int16)s.FloorHeight);
				writer.Write((Int16)s.CeilHeight);
				writer.Write(Lump.MakeFixedName(s.FloorTexture, WAD.ENCODING));
				writer.Write(Lump.MakeFixedName(s.CeilTexture, WAD.ENCODING));
				writer.Write((Int16)s.Brightness);
				writer.Write((UInt16)s.Effect);
				writer.Write((UInt16)s.Tag);
			}

			// Find insert position and remove old lump
			insertpos = MapManager.RemoveSpecificLump(wad, "SECTORS", position, MapManager.TEMP_MAP_HEADER, maplumps);
			if(insertpos == -1) insertpos = position + 1;
			if(insertpos > wad.Lumps.Count) insertpos = wad.Lumps.Count;

			// Create the lump from memory
			lump = wad.Insert("SECTORS", insertpos, (int)mem.Length);
			lump.Stream.Seek(0, SeekOrigin.Begin);
			mem.WriteTo(lump.Stream);
		}
		
		#endregion
	}
}
