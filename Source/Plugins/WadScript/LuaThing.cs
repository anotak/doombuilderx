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

        public int arg0
        {
            get
            {

                if (thing.IsDisposed)
                {
                    throw new ScriptRuntimeException("Thing has been disposed, can't get arg0!");
                }
                if (General.Map.FormatInterface.HasActionArgs)
                {
                    return thing.Args[0];
                }
                else
                {
                    ScriptContext.context.WarnFormatIncompatible("Action Arguments");
                    return 0;
                }
            }
            set
            {
                if (thing.IsDisposed)
                {
                    throw new ScriptRuntimeException("Thing has beend disposed, can't set arg0!");
                }
                if (General.Map.FormatInterface.HasActionArgs)
                {
                    // FIXME this is the extremely hacky workaround to
                    // not having a proper way to call .BeforePropsChange
                    // for setting args
                    // right now. we have to do this because we must maintain
                    // compatibility with non-DBX codebases, but hopefully
                    // we can do this in a better way in the future
                    thing.Tag = thing.Tag;

                    thing.Args[0] = value;
                }
                else
                {
                    ScriptContext.context.WarnFormatIncompatible("Action Arguments");
                }
            }
        }

        public int arg1
        {
            get
            {

                if (thing.IsDisposed)
                {
                    throw new ScriptRuntimeException("Thing has been disposed, can't get arg1!");
                }
                if (General.Map.FormatInterface.HasActionArgs)
                {
                    return thing.Args[1];
                }
                else
                {
                    ScriptContext.context.WarnFormatIncompatible("Action Arguments");
                    return 0;
                }
            }
            set
            {
                if (thing.IsDisposed)
                {
                    throw new ScriptRuntimeException("Thing has beend disposed, can't set arg1!");
                }
                if (General.Map.FormatInterface.HasActionArgs)
                {
                    // FIXME this is the extremely hacky workaround to
                    // not having a proper way to call .BeforePropsChange
                    // for setting args
                    // right now. we have to do this because we must maintain
                    // compatibility with non-DBX codebases, but hopefully
                    // we can do this in a better way in the future
                    thing.Tag = thing.Tag;

                    thing.Args[1] = value;
                }
                else
                {
                    ScriptContext.context.WarnFormatIncompatible("Action Arguments");
                }
            }
        }

        public int arg2
        {
            get
            {

                if (thing.IsDisposed)
                {
                    throw new ScriptRuntimeException("Thing has been disposed, can't get arg2!");
                }
                if (General.Map.FormatInterface.HasActionArgs)
                {
                    return thing.Args[2];
                }
                else
                {
                    ScriptContext.context.WarnFormatIncompatible("Action Arguments");
                    return 0;
                }
            }
            set
            {
                if (thing.IsDisposed)
                {
                    throw new ScriptRuntimeException("Thing has beend disposed, can't set arg2!");
                }
                if (General.Map.FormatInterface.HasActionArgs)
                {
                    // FIXME this is the extremely hacky workaround to
                    // not having a proper way to call .BeforePropsChange
                    // for setting args
                    // right now. we have to do this because we must maintain
                    // compatibility with non-DBX codebases, but hopefully
                    // we can do this in a better way in the future
                    thing.Tag = thing.Tag;

                    thing.Args[2] = value;
                }
                else
                {
                    ScriptContext.context.WarnFormatIncompatible("Action Arguments");
                }
            }
        }

        public int arg3
        {
            get
            {

                if (thing.IsDisposed)
                {
                    throw new ScriptRuntimeException("Thing has been disposed, can't get arg3!");
                }
                if (General.Map.FormatInterface.HasActionArgs)
                {
                    return thing.Args[3];
                }
                else
                {
                    ScriptContext.context.WarnFormatIncompatible("Action Arguments");
                    return 0;
                }
            }
            set
            {
                if (thing.IsDisposed)
                {
                    throw new ScriptRuntimeException("Thing has beend disposed, can't set arg3!");
                }
                if (General.Map.FormatInterface.HasActionArgs)
                {
                    // FIXME this is the extremely hacky workaround to
                    // not having a proper way to call .BeforePropsChange
                    // for setting args
                    // right now. we have to do this because we must maintain
                    // compatibility with non-DBX codebases, but hopefully
                    // we can do this in a better way in the future
                    thing.Tag = thing.Tag;

                    thing.Args[3] = value;
                }
                else
                {
                    ScriptContext.context.WarnFormatIncompatible("Action Arguments");
                }
            }
        }

        public int arg4
        {
            get
            {

                if (thing.IsDisposed)
                {
                    throw new ScriptRuntimeException("Thing has been disposed, can't get arg4!");
                }
                if (General.Map.FormatInterface.HasActionArgs)
                {
                    return thing.Args[4];
                }
                else
                {
                    ScriptContext.context.WarnFormatIncompatible("Action Arguments");
                    return 0;
                }
            }
            set
            {
                if (thing.IsDisposed)
                {
                    throw new ScriptRuntimeException("Thing has beend disposed, can't set arg4!");
                }
                if (General.Map.FormatInterface.HasActionArgs)
                {
                    // FIXME this is the extremely hacky workaround to
                    // not having a proper way to call .BeforePropsChange
                    // for setting args
                    // right now. we have to do this because we must maintain
                    // compatibility with non-DBX codebases, but hopefully
                    // we can do this in a better way in the future
                    thing.Tag = thing.Tag;

                    thing.Args[4] = value;
                }
                else
                {
                    ScriptContext.context.WarnFormatIncompatible("Action Arguments");
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

        public bool IsFlagSet(string flagname)
        {
            if (thing.IsDisposed)
            {
                throw new ScriptRuntimeException("Thing has been disposed, can't IsFlagSet()!");
            }
            // FIXME warn on no such flag
            return thing.IsFlagSet(flagname);
        }

        public void SetFlag(string flagname, bool val)
        {
            if (thing.IsDisposed)
            {
                throw new ScriptRuntimeException("Thing has been disposed, can't SetFlag()!");
            }
            // FIXME warn on no such flag
            thing.SetFlag(flagname, val);
        }

        public void ClearFlags()
        {
            if (thing.IsDisposed)
            {
                throw new ScriptRuntimeException("Thing has been disposed, can't ClearFlags()!");
            }
            thing.ClearFlags();
        }

        public DynValue GetUDMFField(string key)
        {
            if (thing.IsDisposed)
            {
                throw new ScriptRuntimeException("Thing has been disposed, can't GetUDMFField()!");
            }
            return LuaTypeConversion.GetUDMFField(thing, key, General.Map.Config.ThingFields);
        }

        public void SetUDMFField(string key, DynValue value)
        {
            if (thing.IsDisposed)
            {
                throw new ScriptRuntimeException("Thing has been disposed, can't SetUDMFField()!");
            }
            LuaTypeConversion.SetUDMFField(thing, key, value);
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
            thing.DetermineSector();
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
