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
    } // class
} // ns