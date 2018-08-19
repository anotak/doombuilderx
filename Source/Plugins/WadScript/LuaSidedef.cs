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
                    throw new ScriptRuntimeException("Sidedef has been disposed, can't get offsetx.");
                }
                return sidedef.OffsetX;
            }
            set
            {
                if (sidedef.IsDisposed)
                {
                    throw new ScriptRuntimeException("Sidedef has been disposed, can't set offsetx.");
                }

                if (value > General.Map.FormatInterface.MaxTextureOffset
                    ||
                    value > General.Map.FormatInterface.MaxTextureOffset)
                {
                    ScriptContext.context.WarnFormatIncompatible("Sidedef offset out of range, must be between " + General.Map.FormatInterface.MinTextureOffset + " and " + General.Map.FormatInterface.MaxTextureOffset);

                    value = Math.Max(
                        General.Map.FormatInterface.MinTextureOffset,
                        Math.Min(
                            General.Map.FormatInterface.MaxTextureOffset,
                            value
                            )
                        );
                    return;
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
                    throw new ScriptRuntimeException("Sidedef has been disposed, can't get offsety.");
                }
                return sidedef.OffsetY;
            }
            set
            {
                if (sidedef.IsDisposed)
                {
                    throw new ScriptRuntimeException("Sidedef has been disposed, can't set offsety.");
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
                    throw new ScriptRuntimeException("Sidedef has been disposed, can't get uppertex.");
                }
                return sidedef.HighTexture;
            }
            set
            {
                if (sidedef.IsDisposed)
                {
                    throw new ScriptRuntimeException("Sidedef has been disposed, can't set uppertex.");
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
                    throw new ScriptRuntimeException("Sidedef has been disposed, can't get midtex.");
                }
                return sidedef.MiddleTexture;
            }
            set
            {
                if (sidedef.IsDisposed)
                {
                    throw new ScriptRuntimeException("Sidedef has been disposed, can't set midtex.");
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
                    throw new ScriptRuntimeException("Sidedef has been disposed, can't get lowertex.");
                }
                return sidedef.LowTexture;
            }
            set
            {
                if (sidedef.IsDisposed)
                {
                    throw new ScriptRuntimeException("Sidedef has been disposed, can't set lowertex.");
                }
                sidedef.SetTextureLow(value);
            }
        }

        [MoonSharpHidden]
        public LuaSidedef(Sidedef s)
        {
            sidedef = s;
        }

        public int GetIndex()
        {
            if (sidedef.IsDisposed)
            {
                throw new ScriptRuntimeException("Sidedef has been disposed, can't GetIndex().");
            }

            return sidedef.Index;
        }

        public bool IsDisposed()
        {
            return sidedef.IsDisposed;
        }

        public DynValue GetUDMFField(string key)
        {
            if (sidedef.IsDisposed)
            {
                throw new ScriptRuntimeException("Sidedef has been disposed, can't GetUDMFField().");
            }
            return LuaTypeConversion.GetUDMFField(sidedef, key, General.Map.Config.SidedefFields);
        }

        public void SetUDMFField(string key, DynValue value)
        {
            if (sidedef.IsDisposed)
            {
                throw new ScriptRuntimeException("Sidedef has been disposed, can't SetUDMFField().");
            }
            
            LuaTypeConversion.SetUDMFField(sidedef, key, value);
        }

        public Table GetUDMFTable()
        {
            if (sidedef.IsDisposed)
            {
                throw new ScriptRuntimeException("Sidedef has been disposed, can't GetUDMFTable().");
            }

            return LuaTypeConversion.GetUDMFTable(sidedef);
        }

        public bool IsFront()
        {
            if (sidedef.IsDisposed || sidedef.Line.IsDisposed)
            {
                throw new ScriptRuntimeException("Sidedef has been disposed, can't IsFront().");
            }
            return sidedef.IsFront;
        }

        public LuaSidedef GetOther()
        {
            if (sidedef.IsDisposed || sidedef.Line.IsDisposed)
            {
                throw new ScriptRuntimeException("Sidedef has been disposed, can't GetOther().");
            }
            if (sidedef.Other == null)
            {
                return null;
            }
            return new LuaSidedef(sidedef.Other);
        }

        public float GetAngle()
        {
            if (sidedef.IsDisposed || sidedef.Line.IsDisposed)
            {
                throw new ScriptRuntimeException("Sidedef has been disposed, can't GetAngle.");
            }
            return sidedef.Angle;
        }

        public LuaSector GetSector()
        {
            if (sidedef.IsDisposed)
            {
                throw new ScriptRuntimeException("Sidedef has been disposed, can't GetSector.");
            }
            return new LuaSector(sidedef.Sector);
        }

        public LuaLinedef GetLinedef()
        {
            if (sidedef.IsDisposed || sidedef.Line.IsDisposed)
            {
                throw new ScriptRuntimeException("Sidedef has been disposed, can't GetLinedef.");
            }
            return new LuaLinedef(sidedef.Line);
        }

        public override string ToString()
        {
            if (sidedef.IsDisposed)
            {
                return "Disposed sidedef " + sidedef.Index;
            }
            return "Sidedef " + sidedef.Index;
        }

    } // class luasidedef
} // ns
