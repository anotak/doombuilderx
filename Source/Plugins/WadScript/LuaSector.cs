using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeImp.DoomBuilder.Map;

using MoonSharp.Interpreter;

namespace CodeImp.DoomBuilder.DBXLua
{
    // wrapper over Sector for lua
    [MoonSharpUserData]
    public class LuaSector
    {
        [MoonSharpHidden]
        internal Sector sector;

        public int floorheight;
        public int ceilheight;
        public string floortexname;
        public string ceiltexname;
        public int effect;
        public int tag;
        public int brightness;

        [MoonSharpHidden]
        public LuaSector(Sector s)
        {
            floorheight = s.FloorHeight;
            ceilheight = s.CeilHeight;
            floortexname = s.FloorTexture;
            ceiltexname = s.CeilTexture;
            effect = s.Effect;
            tag = s.Tag;
            brightness = s.Brightness;
        }

        public bool Intersect(LuaVector2D p)
        {
            return sector.Intersect(p.vec);
        }

        public override string ToString()
        {
            return sector.ToString();
        }

        // TODO: join, createbbox

    } // sector
} // ns
