using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using CodeImp.DoomBuilder.Windows;
using CodeImp.DoomBuilder.IO;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Rendering;
using CodeImp.DoomBuilder.Geometry;
using CodeImp.DoomBuilder.Editing;
using CodeImp.DoomBuilder.Actions;
using CodeImp.DoomBuilder.Types;
using CodeImp.DoomBuilder.Config;

using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Loaders;

namespace CodeImp.DoomBuilder.DBXLua
{
    public class ScriptContext
    {
        internal static ScriptContext context;

        internal Script script;

        internal float rendererscale;
        internal Vector2D mousemappos;
        internal bool snaptogrid;
        internal bool snaptonearest;

        private HashSet<string> features_warning;
        private HashSet<string> general_warnings;
        private StringBuilder scriptlog_sb;
        private StringBuilder debuglog_sb;

        public string errorText;

        public string scriptPath;

        // used for asking for input from ScriptParamForm, see also LuaUI
        public LuaUIParameters ui_parameters;

        // hacky workaround for .NET framework bug, see RandomSeed()
        public bool bInitializedSeed;

        public bool bInitializedRequireSandbox;
        

        public string ScriptLog {
            get
            {
                if (scriptlog_sb == null)
                {
                    return "";
                }
                return scriptlog_sb.ToString();
            }
        }

        public string DebugLog
        {
            get
            {
                if (debuglog_sb == null)
                {
                    return "";
                }
                return debuglog_sb.ToString();
            }
        }

        public ScriptContext(IRenderer2D renderer, Vector2D inmappos, bool insnaptogrid, bool insnaptonearest)
        {
            errorText = "";
            context = this;


            snaptogrid = insnaptogrid;
            snaptonearest = insnaptonearest;
            mousemappos = inmappos;
            ui_parameters = new LuaUIParameters(16);
            if (float.IsNaN(mousemappos.x) || float.IsInfinity(mousemappos.x))
            {
                mousemappos.x = 0f;
            }
            if (float.IsNaN(mousemappos.y) || float.IsInfinity(mousemappos.y))
            {
                mousemappos.y = 0f;
            }
            rendererscale = renderer.Scale;
            scriptlog_sb = new StringBuilder();
            debuglog_sb = new StringBuilder();

            UserData.RegisterAssembly();
            script = new Script(CoreModules.Preset_SoftSandbox);

            script.Options.DebugPrint = s => { LuaUI.DebugLogLine(s); };

            script.Globals["Line2D"] = typeof(LuaLine2D);
            script.Globals["Vector2D"] = typeof(LuaVector2D);
            script.Globals["Vector3D"] = typeof(LuaVector3D);
            script.Globals["Map"] = typeof(LuaMap);
            script.Globals["Pen"] = typeof(Pen);
            script.Globals["UI"] = typeof(LuaUI);
            script.Globals["Data"] = typeof(LuaDataManager);
            script.Globals["MapFormat"] = typeof(LuaMapFormat);
            script.Globals["dofile"] = (Func<string, DynValue>)DoFile;
            script.Globals["dostring"] = (Func<string, DynValue>)DoString;
            script.Globals["require"] = (Func<string, DynValue>)Require;

            script.Options.DebugPrint = DebugLogLine;

            bInitializedRequireSandbox = false;

            // hacky workaround for .NET 3.5 bug, see RandomSeed for more info
            {
                object math = script.Globals["math"];
                if (math is Table)
                {
                    Table math_table = (Table)math;
                    math_table["randomseed"] = (Func<DynValue, DynValue>)RandomSeed;
                }
            }
        }

        public bool RunScript(string nscriptpath)
        {
            scriptPath = nscriptpath;

            try
            {
                try
                {
                    DynValue returnValue = script.DoFile(scriptPath);
                    //General.WriteLogLine(returnValue.ToDebugPrintString());
                    return true;
                }
                catch (InterpreterException e)
                {
                    errorText = e.DecoratedMessage;

                    if (errorText.StartsWith(Path.GetFullPath(scriptPath))
                        && Path.GetDirectoryName(scriptPath).Length + 1 < errorText.Length)
                    {
                        errorText = errorText.Substring(Path.GetDirectoryName(scriptPath).Length + 1);
                    }

                    General.WriteLogLine(e.DecoratedMessage);
                    return false;
                }
                catch (IndexOutOfRangeException e)
                {
                    // ano - this is the worst hack in the universe to address
                    // https://github.com/xanathar/moonsharp/issues/221 this still-open problem
                    if (e.Source == "MoonSharp.Interpreter")
                    {
                        errorText = "There was an error in the Lua interpreter.\n";
                        errorText += "The most common reason for this is if the stack overflowed. ";
                        errorText += "This can happen if you call a function from itself infinitely, for example.\n";
                        errorText += "Apologies that we are unable to give more detailed information. ";
                        return false;
                    } else {
                        throw;
                    }
                }
            }
            catch (IOException e)
            {
                errorText = "Unable to open file:\n" + e.Message;
                return false;
            }
        }

        public void Kill()
        {
            script.AttachDebugger(new DBXDebugger());
            script.DebuggerEnabled = true;
        }

        public string GetWarnings()
        {
            StringBuilder sb = new StringBuilder();
            if (features_warning != null)
            {
                sb.Append("Map format '");
                sb.Append(General.Map.Config.Name);
                sb.Append("' does not support the following features:\n    ");
                foreach (string s in features_warning)
                {
                    sb.Append(s);
                    sb.Append(",");
                }
                sb.Append("\n");
            }
            if (general_warnings != null)
            {
                foreach (string s in general_warnings)
                {
                    sb.Append(s);
                    sb.Append('\n');
                }
            }
            return sb.ToString();
        }

        internal void WarnFormatIncompatible(string name)
        {
            if (features_warning == null)
            {
                features_warning = new HashSet<string>();
            }
            features_warning.Add(name);
        }

        internal void Warn(string name)
        {
            if (general_warnings == null)
            {
                general_warnings = new HashSet<string>();
            }
            general_warnings.Add(name);
        }

        #region LuaFunctions
        internal void LogLine(string line)
        {
            scriptlog_sb.Append(line);
            scriptlog_sb.Append('\n');
        }

        internal void DebugLogLine(string line)
        {
            debuglog_sb.Append(line);
            debuglog_sb.Append('\n');
        }

        /* ano - ugly hack to work around https://github.com/xanathar/moonsharp/issues/230
         see also
         https://github.com/xanathar/moonsharp/blob/b811c97699b08eb025829272cc9a09044a81361f/src/MoonSharp.Interpreter/CoreLib/MathModule.cs#L284
         and
         https://github.com/xanathar/moonsharp/blob/b811c97699b08eb025829272cc9a09044a81361f/src/MoonSharp.Interpreter/CoreLib/MathModule.cs#L26
         if those links were down, basically new Random(seed) throws an exception if seed is
         equal to int.MinValue, and very large numbers in lua being passed to it were casting
         to int.MinValue, which, well, was causing this whole exception problem
        */
        internal DynValue RandomSeed(DynValue input)
        {
            int seed;
            if (input.IsNilOrNan())
            {
                seed = Environment.TickCount;
            }
            else
            {
                seed = (int)input.Number;
            }

            if (seed == int.MinValue)
            {
                // we dont warn about this more than once
                if (!bInitializedSeed) 
                {
                    // should we warn about this in the parameterless case?
                    Warn(
                        "math.randomseed of "
                        + input.CastToString()
                        + " was out of bounds.\nseed must be greater than "
                        + int.MinValue + " and less than or equal to " + int.MaxValue
                        + ". a random seed chosen based on the current system uptime was used instead."
                        );
                }

                seed = Environment.TickCount;
                /*
                as per the
                https://msdn.microsoft.com/en-us/library/system.environment.tickcount(v=vs.110).aspx
                Environment.TickCount documentation:
                TickCount will increment from zero to Int32.MaxValue for approximately 24.9 days,
                then jump to Int32.MinValue
                */
                while (seed == int.MinValue)
                {
                    // sleeping here sucks but it's supposed to happen only precisely at
                    // int.MaxValue milliseconds of system uptime
                    System.Threading.Thread.Sleep(1);
                    seed = Environment.TickCount;
                }
            }

            Random rand = new Random(seed);
            
            script.Registry.Set("F61E3AA7247D4D1EB7A45430B0C8C9BB_MATH_RANDOM",
                UserData.Create(new MoonSharp.Interpreter.Interop.AnonWrapper<Random>(rand))
                );
            bInitializedSeed = true;
            return DynValue.Nil;
        }

        internal DynValue DoString(string evalstring)
        {
            return script.DoString(evalstring);
        }

        internal DynValue Require(string module)
        {
            if (Path.GetExtension(module).ToLowerInvariant() != ".lua")
            {
                module = module + ".lua";
            }

            module = CheckFilename(module, "require");

            // don't actually assign value, just do our own checking to see if it's safe
            GetActualPath(module, "require");

            module = module.Substring(0, module.Length - 4);



            if (!bInitializedRequireSandbox)
            {
                FileSystemScriptLoader loader = new FileSystemScriptLoader();

                loader.IgnoreLuaPathGlobal = true;

                loader.ModulePaths =
                    new string[] {
                        Path.Combine(Path.GetDirectoryName(scriptPath), "?.lua"),
                        Path.Combine(General.Map.FilePathName, "?.lua"),
                        Path.Combine(Path.Combine(General.Map.FilePathName, "lua"), "?.lua"),
                        Path.Combine(Path.Combine(General.SettingsPath, "lua"), "?.lua"),
                        Path.Combine(Path.Combine(General.PluginsPath, "lua"), "?.lua"),
                        Path.Combine(Path.Combine(General.AppPath, "lua"), "?.lua"),
                    };

                script.Options.ScriptLoader = loader;

                bInitializedRequireSandbox = true;
            }

            return script.RequireModule(module);
        }

        internal string CheckFilename(string filename, string error_source)
        {
            // references used:
            // https://msdn.microsoft.com/en-us/library/windows/desktop/aa365247(v=vs.85).aspx
            // http://gavinmiller.io/2016/creating-a-secure-sanitization-function/
            // https://github.com/madrobby/zaru/blob/master/test/test_zaru.rb
            filename = filename.Trim();

            filename.Replace('/', '\\');
            filename.Replace("\x00", "");

            string[] unsafeChars = { "?", "%", "*", ":", "|", "<", ">", "\"", "..", @".\", @"\." };
            foreach (string c in unsafeChars)
            {
                if (filename.Contains(c))
                {
                    throw new ScriptRuntimeException("improper filename '" + filename + "' for " + error_source + ", contains '" + c + "'");
                }
            }

            if (Path.IsPathRooted(filename) || filename.StartsWith("\\"))
            {
                throw new ScriptRuntimeException("improper filename '" + filename + "' for " + error_source +", must use relative path");
            }


            if (filename.StartsWith("."))
            {
                throw new ScriptRuntimeException("improper filename '" + filename + "' for " + error_source + ", can't start with '.'");
            }

            return filename;
        }

        internal string GetActualPath(string filename, string error_source)
        {
            filename = CheckFilename(filename, error_source);

            if (!Path.GetExtension(filename).ToLower().StartsWith(".lua"))
            {
                throw new ScriptRuntimeException("improper filename '" + filename + "' for " + error_source + ", must be '.lua' file, was '" + Path.GetExtension(filename) + "'");
            }

            // try current lua file path -> wad path -> wad path\lua
            // -> settings path\lua -> plugin path\lua -> app path\lua
            string tryFile = Path.Combine(Path.GetDirectoryName(scriptPath), filename);
            string levelDirectory = Path.GetDirectoryName(Path.GetDirectoryName(General.Map.FilePathName));
            if (General.Map.FilePathName.Length > 1)
            {
                if (!File.Exists(tryFile))
                {
                    tryFile = Path.Combine(levelDirectory, filename);
                }
                if (!File.Exists(tryFile))
                {
                    tryFile = Path.Combine(Path.Combine(levelDirectory, "lua"), filename);
                }
            }
            if (!File.Exists(tryFile))
            {
                tryFile = Path.Combine(Path.Combine(General.SettingsPath, "lua"), filename);
            }
            if (!File.Exists(tryFile))
            {
                tryFile = Path.Combine(Path.Combine(General.PluginsPath, "lua"), filename);
            }
            if (!File.Exists(tryFile))
            {
                tryFile = Path.Combine(Path.Combine(General.AppPath, "lua"), filename);
            }
            if (!File.Exists(tryFile))
            {
                tryFile = Path.Combine(Path.Combine(Path.GetDirectoryName(scriptPath), ".."), filename);
            }

            if (!File.Exists(tryFile))
            {
                if (General.Map.FilePathName.Length > 1)
                {
                    throw new ScriptRuntimeException(
                        error_source
                        + " target '" + filename + "' does not exist in any of the searched paths."
                        + "\nthe currently searched paths (in order of preference) were:\n"
                        + Path.Combine(Path.GetDirectoryName(scriptPath), filename) + "\n"
                        + Path.Combine(levelDirectory, filename) + "\n"
                        + Path.Combine(Path.Combine(levelDirectory, "lua"), filename) + "\n"
                        + Path.Combine(Path.Combine(General.SettingsPath, "lua"), filename) + "\n"
                        + Path.Combine(Path.Combine(General.PluginsPath, "lua"), filename) + "\n"
                        + Path.Combine(Path.Combine(General.AppPath, "lua"), filename) + "\n"
                        + Path.Combine(Path.Combine(Path.GetDirectoryName(scriptPath), ".."),filename) + "\n");
                }
                else
                {
                    throw new ScriptRuntimeException(
                        error_source
                        + " target '" + filename + "' does not exist in any of the searched paths."
                        + "\nthe currently searched paths (in order of preference) were:\n"
                        + Path.Combine(Path.GetDirectoryName(scriptPath), filename) + "\n"
                        + Path.Combine(Path.Combine(General.SettingsPath, "lua"), filename) + "\n"
                        + Path.Combine(Path.Combine(General.PluginsPath, "lua"), filename) + "\n"
                        + Path.Combine(Path.Combine(General.AppPath, "lua"), filename) + "\n");
                }
            }

            return tryFile;
        }

        // cleaned up version of dofile in order to sandbox a bit better
        // only should accept relative paths
        internal DynValue DoFile(string filename)
        {
            filename = GetActualPath(filename, "dofile");

            try
            {
                return script.DoFile(filename);
            }
            catch (Exception e)
            {
                throw new ScriptRuntimeException(e);
            }
        }
        #endregion
    }
}
