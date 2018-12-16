using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Geometry;
using MoonSharp.Interpreter;

namespace CodeImp.DoomBuilder.DBXLua
{
    // wrapper over MapSet for lua
    // TODO: GetUnselectedblah, getmarkedblah, joinoverlappinglines,
    // removeloopedlinedefs, removeloopedandzerolengthlinedefs, joinverticesonelist, joinvertices
    // flipbackwardslinedefs, splitlinesbyvertices
    // nearests with selection
    //  NearestVertexSquareRange
    //  NearestThingSquareRange
    [MoonSharpUserData]
    public class LuaMap
    {
        public static List<LuaSector> GetSectors()
        {
            List<LuaSector> output = new List<LuaSector>();
            foreach (Sector s in General.Map.Map.Sectors)
            {
                output.Add(new LuaSector(s));
            }
            return output;
        }

        public static List<LuaVertex> GetVertices()
        {
            List<LuaVertex> output = new List<LuaVertex>();
            foreach (Vertex v in General.Map.Map.Vertices)
            {
                output.Add(new LuaVertex(v));
            }
            return output;
        }

        public static List<LuaLinedef> GetLinedefs()
        {
            List<LuaLinedef> output = new List<LuaLinedef>();
            foreach (Linedef l in General.Map.Map.Linedefs)
            {
                output.Add(new LuaLinedef(l));
            }
            return output;
        }

        public static List<LuaSidedef> GetSidedefs()
        {
            List<LuaSidedef> output = new List<LuaSidedef>();
            foreach (Sidedef l in General.Map.Map.Sidedefs)
            {
                output.Add(new LuaSidedef(l));
            }
            return output;
        }

        public static List<LuaThing> GetThings()
        {
            List<LuaThing> output = new List<LuaThing>();
            foreach (Thing l in General.Map.Map.Things)
            {
                output.Add(new LuaThing(l));
            }
            return output;
        }

        public static List<LuaSector> GetSelectedSectors()
        {
            List<LuaSector> output = new List<LuaSector>();
            ICollection<Sector> selected = General.Map.Map.GetSelectedSectors(true);
            foreach (Sector s in selected)
            {
                output.Add(new LuaSector(s));
            }
            return output;
        }

        public static List<LuaSidedef> GetSelectedSidedefs()
        {
            List<LuaSidedef> output = new List<LuaSidedef>();
            ICollection<Linedef> selected = General.Map.Map.GetSelectedLinedefs(true);
            foreach (Linedef l in selected)
            {
                if (l.Front != null)
                {
                    output.Add(new LuaSidedef(l.Front));
                }
                if (l.Back != null)
                {
                    output.Add(new LuaSidedef(l.Back));
                }
            }
            return output;
        }

        public static List<LuaVertex> GetSelectedVertices()
        {
            List<LuaVertex> output = new List<LuaVertex>();
            ICollection<Vertex> selected = General.Map.Map.GetSelectedVertices(true);
            foreach (Vertex v in selected)
            {
                output.Add(new LuaVertex(v));
            }
            return output;
        }

        public static List<LuaLinedef> GetSelectedLinedefs()
        {
            List<LuaLinedef> output = new List<LuaLinedef>();
            ICollection<Linedef> selected = General.Map.Map.GetSelectedLinedefs(true);
            foreach (Linedef l in selected)
            {
                output.Add(new LuaLinedef(l));
            }
            return output;
        }

        public static List<LuaThing> GetSelectedThings()
        {
            List<LuaThing> output = new List<LuaThing>();
            ICollection<Thing> selected = General.Map.Map.GetSelectedThings(true);
            foreach (Thing l in selected)
            {
                output.Add(new LuaThing(l));
            }
            return output;
        }

        public static List<LuaSector> GetMarkedSectors()
        {
            List<LuaSector> output = new List<LuaSector>();
            ICollection<Sector> marked = General.Map.Map.GetMarkedSectors(true);
            foreach (Sector s in marked)
            {
                output.Add(new LuaSector(s));
            }
            return output;
        }

        public static List<LuaSidedef> GetMarkedSidedefs()
        {
            List<LuaSidedef> output = new List<LuaSidedef>();
            ICollection<Sidedef> marked = General.Map.Map.GetMarkedSidedefs(true);
            foreach (Sidedef l in marked)
            {
                output.Add(new LuaSidedef(l));
            }
            return output;
        }

        public static List<LuaVertex> GetMarkedVertices()
        {
            List<LuaVertex> output = new List<LuaVertex>();
            ICollection<Vertex> marked = General.Map.Map.GetMarkedVertices(true);
            foreach (Vertex v in marked)
            {
                output.Add(new LuaVertex(v));
            }
            return output;
        }

        public static List<LuaLinedef> GetMarkedLinedefs()
        {
            List<LuaLinedef> output = new List<LuaLinedef>();
            ICollection<Linedef> marked = General.Map.Map.GetMarkedLinedefs(true);
            foreach (Linedef l in marked)
            {
                output.Add(new LuaLinedef(l));
            }
            return output;
        }

        public static List<LuaThing> GetMarkedThings()
        {
            List<LuaThing> output = new List<LuaThing>();
            ICollection<Thing> marked = General.Map.Map.GetMarkedThings(true);
            foreach (Thing l in marked)
            {
                output.Add(new LuaThing(l));
            }
            return output;
        }

        public static void StitchGeometry()
        {
            if (!General.Map.Map.StitchGeometry())
            {
                throw new ScriptRuntimeException("stitch failed");
            }
        }

        public static LuaSector NearestSector(LuaVector2D pos)
        {
            Sidedef nearest_sidedef = MapSet.NearestSidedef(General.Map.Map.Sidedefs, pos.vec);
            if (nearest_sidedef == null)
            {
                return null;
            }
            Sector sector = nearest_sidedef.Sector;
            if (sector == null)
            {
                return null;
            }
            return new LuaSector(sector);
        }

        // ano - based on Thing.DetermineSector by CodeImp
        public static LuaSector DetermineSector(LuaVector2D pos)
        {
            Linedef nl;

            // Find the nearest linedef on the map
            nl = General.Map.Map.NearestLinedef(pos.vec);
            Sector sector;
            if (nl != null)
            {
                // Check what side of line we are at
                if (nl.SideOfLine(pos.vec) < 0f)
                {
                    // Front side
                    if (nl.Front != null) sector = nl.Front.Sector; else sector = null;
                }
                else
                {
                    // Back side
                    if (nl.Back != null) sector = nl.Back.Sector; else sector = null;
                }
            }
            else
            {
                sector = null;
            }

            if (sector == null)
            {
                return null;
            }
            return new LuaSector(sector);
        }

        public static LuaSidedef NearestSidedef(LuaVector2D pos)
        {
            Sidedef side = MapSet.NearestSidedef(General.Map.Map.Sidedefs, pos.vec);
            if (side == null)
            {
                return null;
            }
            return new LuaSidedef(side);
        }

        public static LuaLinedef NearestLinedef(LuaVector2D pos)
        {
            Linedef l = MapSet.NearestLinedef(General.Map.Map.Linedefs, pos.vec);
            if (l == null)
            {
                return null;
            }
            return new LuaLinedef(l);
        }

        public static LuaLinedef NearestLinedefRange(LuaVector2D pos, float range)
        {
            Linedef l = MapSet.NearestLinedefRange(General.Map.Map.Linedefs, pos.vec, range);
            if (l == null)
            {
                return null;
            }
            return new LuaLinedef(l);
        }

        public static LuaVertex NearestVertex(LuaVector2D pos)
        {
            Vertex v = MapSet.NearestVertex(General.Map.Map.Vertices, pos.vec);
            if (v == null)
            {
                return null;
            }
            return new LuaVertex(v);
        }

        public static LuaThing NearestThing(LuaVector2D pos)
        {
            Thing t = MapSet.NearestThing(General.Map.Map.Things, pos.vec);
            if (t == null)
            {
                return null;
            }
            return new LuaThing(t);
        }

        public static void SnapAllToAccuracy()
        {
            General.Map.Map.SnapAllToAccuracy();
        }

        public static int GetNewTag()
        {
            return General.Map.Map.GetNewTag();
        }

        public static void RemoveUnusedVertices()
        {
            General.Map.Map.RemoveUnusedVertices();
        }

        public static void DeselectAll()
        {
            General.Map.Map.ClearAllSelected();
        }

        public static void DeselectThings()
        {
            General.Map.Map.ClearSelectedThings();
        }

        public static void DeselectGeometry()
        {
            General.Map.Map.ClearSelectedVertices();
            General.Map.Map.ClearSelectedLinedefs();
            General.Map.Map.ClearSelectedSectors();
        }

        public static LuaThing InsertThing(LuaVector2D pos)
        {
            return InsertThing(pos.x, pos.y);
        }

        // based on the InsertThing from the things mode by codeimp
        public static LuaThing InsertThing(float x, float y)
        {
            if (x < General.Map.Config.LeftBoundary || x > General.Map.Config.RightBoundary ||
                y > General.Map.Config.TopBoundary || y < General.Map.Config.BottomBoundary)
            {
                throw new ScriptRuntimeException(
                    "Failed to insert thing: coordinates ("
                    + x
                    + ","
                    + y
                    + ") outside of map boundaries.");
            }

            if (float.IsNaN(x) || float.IsNaN(y) ||
                   float.IsInfinity(x) || float.IsInfinity(y))
            {
                throw new ScriptRuntimeException(
                    "Invalid thing position! The given thing coordinates ("
                    + x
                    + ","
                    + y
                    + ") cannot be NaN or Infinite. (You might have divided by 0 somewhere?)");
            }

            // Create thing
            Thing t = General.Map.Map.CreateThing();
            if (t != null)
            {
                General.Settings.ApplyDefaultThingSettings(t);

                t.Move(new Vector2D(x, y));

                t.UpdateConfiguration();

                // Snap to map format accuracy
                t.SnapToAccuracy();
            }
            else
            {
                throw new ScriptRuntimeException(
                    "Insert thing returned null thing (max thing limit reached? current count is "
                    + General.Map.Map.Things.Count + " of " + General.Map.FormatInterface.MaxThings
                    + ")");
            }

            return new LuaThing(t);
        }

        // ano -  this is helper for stuff like joining and merging
        [MoonSharpHidden]
        public static HashSet<Sector> UniqueSectorsFromTable(Table table)
        {

            HashSet<Sector> sectors = new HashSet<Sector>();

            int table_index = 1; // ano - lua is 1 indexed
            foreach (DynValue d in table.Values)
            {

                object potential_sector = d.ToObject();

                if (potential_sector is LuaSector)
                {
                    sectors.Add(((LuaSector)potential_sector).sector);
                }
                else
                {
                    throw new ScriptRuntimeException("Element #" + table_index + ": '" + d.ToString() + "' is not a sector, can't join.");
                }

                table_index++;
            }

            return sectors;
        }

        // ano - internal side of joining sectors, used for join and merge functions separately
        // indirectly based on CodeImp's SectorsMode.JoinMergeSectors
        [MoonSharpHidden]
        public static LuaSector JoinUniqueSectors(HashSet<Sector> sectors)
        {
            // ano - joining sectors will dispose them
            // should we make an initial pass to check if sectors are disposed ahead of time?
            // my gut says no because people may want to do several joins and it sounds like
            // a pain to worry about
            Sector first = null;
            bool selected = true;
            foreach (Sector s in sectors)
            {
                if (!s.IsDisposed)
                {
                    selected = selected && s.Selected;

                    if (first == null)
                    {
                        first = s;
                    }
                    else
                    {
                        s.Join(first);
                    }
                }
            }

            if (first == null || first.IsDisposed)
            {
                // ano - should we warn/error on null first?
                return null;
            }
            else
            {
                LuaSector output = new LuaSector(first);
                output.selected = selected;
                return output;
            }
        }

        // TODO - add a version of this to LuaSector
        // ano - based on CodeImp's SectorsMode.JoinMergeSectors
        public static LuaSector JoinSectors(Table table)
        {
            if (table == null || table.Length <= 0)
            {
                // ano - should we error or warn here?
                return null;
            }

            HashSet<Sector> sectors = UniqueSectorsFromTable(table);

            // ano - bit of a hacky mess to retrieve only element of 1-sized hashset
            if (sectors.Count == 1)
            {
                foreach (Sector s in sectors)
                {
                    if (s.IsDisposed)
                    {
                        return null;
                    }

                    return new LuaSector(s);
                }
            }

            return JoinUniqueSectors(sectors);
        }

        // ano - based on CodeImp's SectorsMode.JoinMergeSectors
        public static LuaSector MergeSectors(Table table)
        {
            if (table == null || table.Length <= 0)
            {
                // ano - should we error or warn here?
                return null;
            }

            HashSet<Sector> sectors = UniqueSectorsFromTable(table);

            // ano - bit of a hacky mess to retrieve only element of 1-sized hashset
            if (sectors.Count == 1)
            {
                foreach (Sector s in sectors)
                {
                    if (s.IsDisposed)
                    {
                        return null;
                    }

                    return new LuaSector(s);
                }
            }

            // ano - for merge we need to remove linedefs
            // let's get count instances of the unique lines from the sectors
            Dictionary<Linedef, int> linedefs = new Dictionary<Linedef, int>();

            foreach (Sector s in sectors)
            {
                if (s.IsDisposed)
                {
                    continue;
                }

                foreach (Sidedef side in s.Sidedefs)
                {
                    Linedef line = side.Line;

                    if (linedefs.ContainsKey(line))
                    {
                        linedefs[line]++;
                    }
                    else if (
                        line.Front != null
                        && line.Back != null
                        && line.Front.Sector != line.Back.Sector)
                    {
                        linedefs.Add(line, 1);
                    }
                }
            }

            foreach (KeyValuePair<Linedef, int> pair in linedefs)
            {
                if (pair.Value > 1)
                {
                    pair.Key.Dispose();
                }
            }

            return JoinUniqueSectors(sectors);
        }

        public static LuaSector MakeSectorAt(Vector2D v)
        {
            // ano - based on CodeImp's MakeSectorMode code
            List<LinedefSide> allsides = Tools.FindPotentialSectorAt(v);
            if (allsides == null)
            {
                return null;
            }

            List<Linedef> alllines = new List<Linedef>(allsides.Count);
            foreach (LinedefSide sd in allsides) alllines.Add(sd.Line);

            // Mark the lines we are going to use for this sector
            General.Map.Map.ClearAllMarks(true);

            foreach (LinedefSide ls in allsides) ls.Line.Marked = false;
            List<Linedef> oldlines = General.Map.Map.GetMarkedLinedefs(true);

            // Make the sector
            Sector s = Tools.MakeSector(allsides, oldlines);
            if (s != null)
            {
                // Now we go for all the lines along the sector to
                // see if they only have a back side. In that case we want
                // to flip the linedef to that it only has a front side.
                foreach (Sidedef sd in s.Sidedefs)
                {
                    if ((sd.Line.Front == null) && (sd.Line.Back != null))
                    {
                        // Flip linedef
                        sd.Line.FlipVertices();
                        sd.Line.FlipSidedefs();
                    }
                }
            }

            return new LuaSector(s);
        }

    } // class
} // ns