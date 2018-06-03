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
    public class LuaSidedef
    {
        [MoonSharpHidden]
        internal Sidedef sidedef;

        public int offsetx
        {
            get
            {
                if (sidedef.IsDisposed)
                {
                    throw new ScriptRuntimeException("Sidedef has been disposed, can't get offsetx!");
                }
                return sidedef.OffsetX;
            }
            set
            {
                if (sidedef.IsDisposed)
                {
                    throw new ScriptRuntimeException("Sidedef has been disposed, can't set offsetx!");
                }
                sidedef.OffsetX = value;
            }
        }

        public int offsety
        {
            get
            {
                if (sidedef.IsDisposed)
                {
                    throw new ScriptRuntimeException("Sidedef has been disposed, can't get offsety!");
                }
                return sidedef.OffsetY;
            }
            set
            {
                if (sidedef.IsDisposed)
                {
                    throw new ScriptRuntimeException("Sidedef has been disposed, can't set offsety!");
                }
                sidedef.OffsetY = value;
            }
        }

        public string uppertex
        {
            get
            {
                if (sidedef.IsDisposed)
                {
                    throw new ScriptRuntimeException("Sidedef has been disposed, can't get uppertex!");
                }
                return sidedef.HighTexture;
            }
            set
            {
                if (sidedef.IsDisposed)
                {
                    throw new ScriptRuntimeException("Sidedef has been disposed, can't set uppertex!");
                }
                sidedef.SetTextureHigh(value);
            }
        }

        public string midtex
        {
            get
            {
                if (sidedef.IsDisposed)
                {
                    throw new ScriptRuntimeException("Sidedef has been disposed, can't get midtex!");
                }
                return sidedef.MiddleTexture;
            }
            set
            {
                if (sidedef.IsDisposed)
                {
                    throw new ScriptRuntimeException("Sidedef has been disposed, can't set midtex!");
                }
                sidedef.SetTextureMid(value);
            }
        }

        public string lowertex
        {
            get
            {
                if (sidedef.IsDisposed)
                {
                    throw new ScriptRuntimeException("Sidedef has been disposed, can't get lowertex!");
                }
                return sidedef.MiddleTexture;
            }
            set
            {
                if (sidedef.IsDisposed)
                {
                    throw new ScriptRuntimeException("Sidedef has been disposed, can't set lowertex!");
                }
                sidedef.SetTextureMid(value);
            }
        }

        [MoonSharpHidden]
        public LuaSidedef(Sidedef s)
        {
            sidedef = s;
        }

        public bool IsDisposed()
        {
            return sidedef.IsDisposed;
        }

        public bool IsFront()
        {
            if (sidedef.IsDisposed || sidedef.Line.IsDisposed)
            {
                throw new ScriptRuntimeException("Sidedef has been disposed, can't IsFront!");
            }
            return sidedef.IsFront;
        }

        public float GetAngle()
        {
            if (sidedef.IsDisposed || sidedef.Line.IsDisposed)
            {
                throw new ScriptRuntimeException("Sidedef has been disposed, can't GetAngle!");
            }
            return sidedef.Angle;
        }

        public LuaSector GetSector()
        {
            if (sidedef.IsDisposed)
            {
                throw new ScriptRuntimeException("Sidedef has been disposed, can't GetSector!");
            }
            return new LuaSector(sidedef.Sector);
        }

        public LuaLinedef GetLinedef()
        {
            if (sidedef.IsDisposed || sidedef.Line.IsDisposed)
            {
                throw new ScriptRuntimeException("Sidedef has been disposed, can't GetLinedef!");
            }
            return new LuaLinedef(sidedef.Line);
        }

    } // class luasidedef
} // ns
