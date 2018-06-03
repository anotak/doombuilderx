using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CodeImp.DoomBuilder.Map;
using MoonSharp.Interpreter;

namespace CodeImp.DoomBuilder.DBXLua
{
    // wrapper over Map for lua
    [MoonSharpUserData]
    public class LuaMap
    {
        public static List<LuaSector> GetSectors()
        {
            List<LuaSector> l = new List<LuaSector>();
            foreach (Sector s in General.Map.Map.Sectors)
            {
                l.Add(new LuaSector(s));
            }
            return l;
        }

        public static List<LuaVertex> GetVertices()
        {
            List<LuaVertex> l = new List<LuaVertex>();
            foreach (Vertex v in General.Map.Map.Vertices)
            {
                l.Add(new LuaVertex(v));
            }
            return l;
        }
    } // class
} // ns