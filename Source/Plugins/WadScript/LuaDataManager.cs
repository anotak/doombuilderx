using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CodeImp.DoomBuilder.Map;
using MoonSharp.Interpreter;

namespace CodeImp.DoomBuilder.DBXLua
{
    // TODO GetThingInfo
    [MoonSharpUserData]
    public class LuaDataManager
    {
        public static bool DoesTextureExist(string name)
        {
            if (name == null)
            {
                throw new ScriptRuntimeException("Name is nil, can't DoesTextureExist() (not enough arguments?)");
            }
            return General.Map.Data.GetTextureExists(name);
        }

        public static bool DoesFlatExist(string name)
        {
            if (name == null)
            {
                throw new ScriptRuntimeException("Name is nil, can't DoesFlatExist() (not enough arguments?)");
            }
            return General.Map.Data.GetFlatExists(name);
        }

        public static List<string> GetTextureNames()
        {
            return General.Map.Data.OnlyTextureNames;
        }

        public static List<string> GetFlatNames()
        {
            return General.Map.Data.OnlyFlatNames;
        }

        // TODO not currently possible because it's internal, check what we can do about that
        /*
        public static bool GetSpriteExists(string name)
        {
            return General.Map.Data.GetSpriteExists(name);
        }*/



    } // data manager
} // ns
