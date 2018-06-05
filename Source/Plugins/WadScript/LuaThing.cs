using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CodeImp.DoomBuilder.Map;

using MoonSharp.Interpreter;

namespace CodeImp.DoomBuilder.DBXLua
{
    // wrapper over Thing for lua
    // 
    [MoonSharpUserData]
    public class LuaThing
    {
        // ano - have i ever mentioned how much i hate this being called a "thing"
        [MoonSharpHidden]
        internal Thing thing;

        public int type
        {
            get
            {
                if (thing.IsDisposed)
                {
                    throw new ScriptRuntimeException("Thing has been disposed, can't get type!");
                }
                return thing.Type;
            }
            set
            {
                if (thing.IsDisposed)
                {
                    throw new ScriptRuntimeException("Thing has been disposed, can't set type!");
                }
                thing.Type = value;
                thing.UpdateConfiguration();
            }
        }

        public int tag
        {
            get
            {
                if (thing.IsDisposed)
                {
                    throw new ScriptRuntimeException("Thing has been disposed, can't get tag!");
                }
                if (General.Map.FormatInterface.HasThingTag)
                {
                    return thing.Tag;
                }
                else
                {
                    ScriptContext.context.WarnFormatIncompatible("Thing Tag Numbers");
                    return 0;
                }
            }
            set
            {
                if (thing.IsDisposed)
                {
                    throw new ScriptRuntimeException("Thing has been disposed, can't set tag!");
                }
                if (General.Map.FormatInterface.HasThingTag)
                {
                    thing.Tag = value;
                }
                else
                {
                    ScriptContext.context.WarnFormatIncompatible("Thing Tag Numbers");
                }
            }
        }

        public int action
        {
            get
            {
                if (thing.IsDisposed)
                {
                    throw new ScriptRuntimeException("Thing has been disposed, can't get action!");
                }
                if (General.Map.FormatInterface.HasThingAction)
                {
                    return thing.Action;
                }
                else
                {
                    ScriptContext.context.WarnFormatIncompatible("Thing Actions");
                    return 0;
                }
            }
            set
            {
                if (thing.IsDisposed)
                {
                    throw new ScriptRuntimeException("Thing has been disposed, can't set action!");
                }
                if (General.Map.FormatInterface.HasThingAction)
                {
                    thing.Action = value;
                }
                else
                {
                    ScriptContext.context.WarnFormatIncompatible("Thing Actions");
                }
            }
        }

        [MoonSharpHidden]
        public LuaThing(Thing t)
        {
            thing = t;
        }

        public bool IsDisposed()
        {
            return thing.IsDisposed;
        }

        public float GetSize()
        {
            if (thing.IsDisposed)
            {
                throw new ScriptRuntimeException("Thing has been disposed, can't GetSize()!");
            }
            return thing.Size;
        }

        public LuaSector GetSector()
        {
            if (thing.IsDisposed)
            {
                throw new ScriptRuntimeException("Thing has been disposed, can't GetSector()!");
            }
            Sector s = thing.Sector;

            if (s == null || s.IsDisposed)
            {
                return null;
            }

            return new LuaSector(s);
        }

        public void SetAngleRadians(float newangle)
        {
            if (thing.IsDisposed)
            {
                throw new ScriptRuntimeException("Thing has been disposed, can't GetSetAngleRadians()!");
            }
            thing.Rotate(newangle);
        }

        public void SetAngleDoom(int newangle)
        {
            if (thing.IsDisposed)
            {
                throw new ScriptRuntimeException("Thing has been disposed, can't GetSetAngleDoom()!");
            }
            thing.Rotate(newangle);
        }

        public void SnapToGrid()
        {
            if (thing.IsDisposed)
            {
                throw new ScriptRuntimeException("Thing has been disposed, can't SnapToGrid()!");
            }
            thing.SnapToGrid();
        }

        public void SnapToAccuracy()
        {
            if (thing.IsDisposed)
            {
                throw new ScriptRuntimeException("Thing has been disposed, can't SnapToGrid()!");
            }
            thing.SnapToAccuracy();
        }

        public float DistanceToSq(LuaVector2D p)
        {
            if (thing.IsDisposed)
            {
                throw new ScriptRuntimeException("Thing has been disposed, can't DistanceToSq()!");
            }
            return thing.DistanceToSq(p.vec);
        }

        public float DistanceTo(LuaVector2D p)
        {
            if (thing.IsDisposed)
            {
                throw new ScriptRuntimeException("Thing has been disposed, can't DistanceTo()!");
            }
            return thing.DistanceTo(p.vec);
        }
    } // class luathing
}// ns
