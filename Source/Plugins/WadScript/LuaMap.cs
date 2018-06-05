using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CodeImp.DoomBuilder.Map;
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
            return new LuaSector(MapSet.NearestSidedef(General.Map.Map.Sidedefs, pos.vec).Sector);
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

            return new LuaSector(sector);
        }

        public static LuaSidedef NearestSidedef(LuaVector2D pos)
        {
            return new LuaSidedef(MapSet.NearestSidedef(General.Map.Map.Sidedefs, pos.vec));
        }

        public static LuaLinedef NearestLinedef(LuaVector2D pos)
        {
            return new LuaLinedef(MapSet.NearestLinedef(General.Map.Map.Linedefs, pos.vec));
        }

        public static LuaLinedef NearestLinedefRange(LuaVector2D pos, float range)
        {
            return new LuaLinedef(MapSet.NearestLinedefRange(General.Map.Map.Linedefs, pos.vec, range));
        }

        public static LuaVertex NearestVertex(LuaVector2D pos)
        {
            return new LuaVertex(MapSet.NearestVertex(General.Map.Map.Vertices, pos.vec));
        }

        public static LuaThing NearestThing(LuaVector2D pos)
        {
            return new LuaThing(MapSet.NearestThing(General.Map.Map.Things, pos.vec));
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
    } // class
} // ns