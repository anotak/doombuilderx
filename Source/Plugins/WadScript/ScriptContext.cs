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
        private StringBuilder scriptlog_sb;
        private StringBuilder debuglog_sb;

        public string errorText;

        public string scriptPath;

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

            script.Globals["Vector2D"] = typeof(LuaVector2D);
            script.Globals["Vector3D"] = typeof(LuaVector3D);
            script.Globals["Map"] = typeof(LuaMap);
            script.Globals["Pen"] = typeof(Pen);
            script.Globals["UI"] = typeof(LuaUI);
            script.Globals["Data"] = typeof(LuaDataManager);
            script.Globals["MapFormat"] = typeof(LuaMapFormat);
            script.Globals["dofile"] = (Func<string, DynValue>)DoFile;
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

        internal DynValue DoFile(string filename)
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
                    throw new ScriptRuntimeException("improper filename " + filename + " for DoFile, contains '" + c + "'");
                }
            }

            if (Path.IsPathRooted(filename) || filename.StartsWith("\\"))
            {
                throw new ScriptRuntimeException("improper filename " + filename + " for DoFile, must use relative path");
            }

            if (filename.StartsWith("."))
            {
                throw new ScriptRuntimeException("improper filename " + filename + " for DoFile, can't start with '.'");
            }

            if (!Path.GetExtension(filename).ToLower().StartsWith(".lua"))
            {
                throw new ScriptRuntimeException("improper filename " + filename + " for DoFile, must be .lua file, was " + Path.GetExtension(filename));
            }

            // try current lua file path -> wad path -> wad path\lua
            // -> settings path\lua -> plugin path\lua -> app path\lua
            string tryFile = Path.Combine(Path.GetDirectoryName(scriptPath),filename);
            if (General.Map.FilePathName.Length > 1)
            {
                if (!File.Exists(tryFile))
                {
                    tryFile = Path.Combine(General.Map.FilePathName, filename);
                }
                if (!File.Exists(tryFile))
                {
                    tryFile = Path.Combine(Path.Combine(General.Map.FilePathName, "lua"), filename);
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
                if (General.Map.FilePathName.Length > 1)
                {
                    throw new ScriptRuntimeException(
                        "dofile target " + filename + " does not exist in any of the searched paths."
                        + "\nthe currently searched paths (in order of preference) were:\n"
                        + Path.Combine(Path.GetDirectoryName(scriptPath), filename) + "\n"
                        + Path.Combine(General.Map.FilePathName, filename) + "\n"
                        + Path.Combine(Path.Combine(General.Map.FilePathName, "lua"), filename) + "\n"
                        + Path.Combine(Path.Combine(General.SettingsPath, "lua"), filename) + "\n"
                        + Path.Combine(Path.Combine(General.PluginsPath, "lua"), filename) + "\n"
                        + Path.Combine(Path.Combine(General.AppPath, "lua"), filename) + "\n");
                }
                else
                {
                    throw new ScriptRuntimeException(
                        "dofile target " + filename + " does not exist in any of the searched paths."
                        + "\nthe currently searched paths (in order of preference) were:\n"
                        + Path.Combine(Path.GetDirectoryName(scriptPath), filename) + "\n"
                        + Path.Combine(Path.Combine(General.SettingsPath, "lua"), filename) + "\n"
                        + Path.Combine(Path.Combine(General.PluginsPath, "lua"), filename) + "\n"
                        + Path.Combine(Path.Combine(General.AppPath, "lua"), filename) + "\n");
                }
            }

            filename = tryFile;

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
