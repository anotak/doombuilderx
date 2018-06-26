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
    } // class
} // ns