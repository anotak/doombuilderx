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

        public int Type
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
            }
        }

        public int Tag
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

        [MoonSharpHidden]
        public LuaThing(Thing t)
        {
            thing = t;
        }

        public bool IsDisposed()
        {
            return thing.IsDisposed;
        }
    } // class luathing
}// ns
