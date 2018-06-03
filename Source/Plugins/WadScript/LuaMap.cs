using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CodeImp.DoomBuilder.Map;
using MoonSharp.Interpreter;

namespace CodeImp.DoomBuilder.DBXLua
{
    // wrapper over Map for lua
    // TODO: GetUnselectedblah
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
    } // class
} // ns