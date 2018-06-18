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

        public bool selected
        {
            get
            {
                if (sector.IsDisposed)
                {
                    throw new ScriptRuntimeException("sector has been disposed, can't get selected status!");
                }

                return sector.Selected;
            }

            set
            {
                if (sector.IsDisposed)
                {
                    throw new ScriptRuntimeException("sector has been disposed, can't set selected status!");
                }

                sector.Selected = value;
            }
        }

        public int floorheight
        {
            get
            {
                if (sector.IsDisposed)
                {
                    throw new ScriptRuntimeException("Sector has been disposed, can't get floorheight!");
                }
                return sector.FloorHeight;
            }
            set
            {
                if (sector.IsDisposed)
                {
                    throw new ScriptRuntimeException("Sector has been disposed, can't set floorheight!");
                }
                sector.FloorHeight = value;
            }
        }

        public int ceilheight
        {
            get
            {
                if (sector.IsDisposed)
                {
                    throw new ScriptRuntimeException("Sector has been disposed, can't get ceilheight!");
                }
                return sector.CeilHeight;
            }
            set
            {
                if (sector.IsDisposed)
                {
                    throw new ScriptRuntimeException("Sector has been disposed, can't set ceilheight!");
                }
                sector.CeilHeight = value;
            }
        }

        public string floortex
        {
            get
            {
                if (sector.IsDisposed)
                {
                    throw new ScriptRuntimeException("Sector has been disposed, can't get floortex!");
                }
                return sector.FloorTexture;
            }
            set
            {
                if (sector.IsDisposed)
                {
                    throw new ScriptRuntimeException("Sector has been disposed, can't set floortex!");
                }
                sector.SetFloorTexture(value);
            }
        }

        public string ceiltex
        {
            get
            {
                if (sector.IsDisposed)
                {
                    throw new ScriptRuntimeException("Sector has been disposed, can't get ceiltex!");
                }
                return sector.CeilTexture;
            }
            set
            {
                if (sector.IsDisposed)
                {
                    throw new ScriptRuntimeException("Sector has been disposed, can't set ceiltex!");
                }
                sector.SetCeilTexture(value);
            }
        }
        public int effect
        {
            get
            {
                if (sector.IsDisposed)
                {
                    throw new ScriptRuntimeException("Sector has been disposed, can't get effect!");
                }
                return sector.Effect;
            }
            set
            {
                if (sector.IsDisposed)
                {
                    throw new ScriptRuntimeException("Sector has been disposed, can't set effect!");
                }
                sector.Effect = value;
            }
        }
        public int tag
        {
            get
            {
                if (sector.IsDisposed)
                {
                    throw new ScriptRuntimeException("Sector has been disposed, can't get tag!");
                }
                return sector.Tag;
            }
            set
            {
                if (sector.IsDisposed)
                {
                    throw new ScriptRuntimeException("Sector has been disposed, can't set tag!");
                }
                sector.Tag = value;
            }
        }
        public int brightness
        {
            get
            {
                if (sector.IsDisposed)
                {
                    throw new ScriptRuntimeException("Sector has been disposed, can't get brightness!");
                }
                return sector.Brightness;
            }
            set
            {
                if (sector.IsDisposed)
                {
                    throw new ScriptRuntimeException("Sector has been disposed, can't set brightness!");
                }
                sector.Brightness = value;
            }
        }

        [MoonSharpHidden]
        public LuaSector(Sector s)
        {
            sector = s;
        }

        public bool IsDisposed()
        {
            return sector.IsDisposed;
        }

        public int GetIndex()
        {
            if (sector.IsDisposed)
            {
                throw new ScriptRuntimeException("Sector has been disposed, can't GetIndex()!");
            }

            return sector.Index;
        }

        public DynValue GetUDMFField(string key)
        {
            if (sector.IsDisposed)
            {
                throw new ScriptRuntimeException("Sector has been disposed, can't GetUDMFField()!");
            }
            return LuaTypeConversion.GetUDMFField(sector, key, General.Map.Config.SectorFields);
        }

        public void SetUDMFField(string key, DynValue value)
        {
            if (sector.IsDisposed)
            {
                throw new ScriptRuntimeException("Sector has been disposed, can't SetUDMFField()!");
            }
            LuaTypeConversion.SetUDMFField(sector, key, value);
        }

        public Table GetSidedefs()
        {
            if (sector.IsDisposed)
            {
                throw new ScriptRuntimeException("Sector has been disposed, can't GetSidedefs()!");
            }

            Table output = new Table(ScriptContext.context.script);
            foreach (Sidedef s in sector.Sidedefs)
            {
                if (!s.IsDisposed)
                {
                    output.Append(DynValue.FromObject(ScriptContext.context.script, new LuaSidedef(s)));
                }
            }

            return output;
        }

        public bool Intersect(LuaVector2D p)
        {
            if (sector.IsDisposed)
            {
                throw new ScriptRuntimeException("Sector has been disposed, can't Intersect()!");
            }
            return sector.Intersect(p.vec);
        }

        public override string ToString()
        {
            if (sector.IsDisposed)
            {
                return "Disposed " + sector.ToString();
            }
            return sector.ToString();
        }

        // TODO  join, createbbox

    } // sector
} // ns
