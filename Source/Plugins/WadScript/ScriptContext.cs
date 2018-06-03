using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
        private HashSet<string> features_warning;
        private StringBuilder scriptlog_sb;

        public string errorText;

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

        public ScriptContext(IRenderer2D renderer, Vector2D inmappos)
        {
            errorText = "";
            context = this;

            mousemappos = inmappos;
            rendererscale = renderer.Scale;
            scriptlog_sb = new StringBuilder();

            UserData.RegisterAssembly();
            script = new Script(CoreModules.Preset_SoftSandbox);

            script.Globals["Vector2D"] = typeof(LuaVector2D);
            script.Globals["Map"] = typeof(LuaMap);
            script.Globals["Pen"] = typeof(Pen);

            script.Globals["LogLine"] = (Func<string,bool>)LogLine;
        }

        public bool RunScript(string scriptCode)
        {
            try
            {
                DynValue returnValue = script.DoString(scriptCode);
                General.WriteLogLine(returnValue.ToDebugPrintString());
                return true;
            }
            catch (InterpreterException e)
            {
                errorText = e.DecoratedMessage;
                
                General.WriteLogLine(e.DecoratedMessage);
                return false;
            }
        }

        public string GetWarnings()
        {
            StringBuilder sb = new StringBuilder();
            if (features_warning != null)
            {
                sb.Append("Map format '");
                sb.Append(General.Map.Config.Name);
                sb.Append("' does not support the following features:\n");
                foreach (string s in features_warning)
                {
                    sb.Append(s);
                    sb.Append(",");
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

        #region LuaFunctions
        protected bool LogLine(string line)
        {
            scriptlog_sb.Append(line);
            scriptlog_sb.Append('\n');
            return true;
        }
        #endregion
    }
}
