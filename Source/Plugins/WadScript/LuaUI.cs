using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CodeImp.DoomBuilder.Geometry;
using MoonSharp.Interpreter;

namespace CodeImp.DoomBuilder.DBXLua
{
    // referred to in Lua as simply "UI"
    [MoonSharpUserData]
    public class LuaUI
    {
        public static void LogLine(string line)
        {
            ScriptContext.context.LogLine(line);
        }

        public static void DebugLogLine(string line)
        {
            ScriptContext.context.DebugLogLine(line);
        }
    } // class
} // ns
