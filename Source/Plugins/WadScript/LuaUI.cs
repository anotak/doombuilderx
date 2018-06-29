using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CodeImp.DoomBuilder.Geometry;
using CodeImp.DoomBuilder.Map;
using MoonSharp.Interpreter;

namespace CodeImp.DoomBuilder.DBXLua
{
    // referred to in Lua as simply "UI"
    [MoonSharpUserData]
    public class LuaUI
    {
        public static void LogLine(string line)
        {
            if (line == null)
            {
                line = "nil";
            }
            ScriptContext.context.LogLine(line);
        }

        public static void DebugLogLine(string line)
        {
            if (line == null)
            {
                line = "nil";
            }
            ScriptContext.context.DebugLogLine(line);
        }

        public static LuaVector2D GetMouseMapPosition()
        {
            return GetMouseMapPosition(ScriptContext.context.snaptogrid, ScriptContext.context.snaptonearest);
        }

        public static LuaVector2D GetMouseMapPosition(bool snaptogrid)
        {
            return GetMouseMapPosition(snaptogrid, ScriptContext.context.snaptonearest);
        }

        public static LuaVector2D GetMouseMapPosition(bool snaptogrid, bool snaptonearest)
        {
            Vector2D mousemappos = ScriptContext.context.mousemappos;

            // Snap to nearest?
            if (snaptonearest)
            {
                float vrange = BuilderPlug.Me.StitchRange / ScriptContext.context.rendererscale;

                // Try the nearest vertex
                Vertex nv = General.Map.Map.NearestVertexSquareRange(mousemappos, vrange);
                if (nv != null)
                {
                    return new LuaVector2D(nv.Position);
                }

                // Try the nearest linedef
                Linedef nl = General.Map.Map.NearestLinedefRange(mousemappos, vrange);
                if (nl != null)
                {
                    // Snap to grid?
                    if (snaptogrid)
                    {
                        // Get grid intersection coordinates
                        List<Vector2D> coords = nl.GetGridIntersections();

                        // Find nearest grid intersection
                        bool found = false;
                        float found_distance = float.MaxValue;
                        Vector2D found_coord = new Vector2D();
                        foreach (Vector2D v in coords)
                        {
                            Vector2D delta = mousemappos - v;
                            if (delta.GetLengthSq() < found_distance)
                            {
                                found_distance = delta.GetLengthSq();
                                found_coord = v;
                                found = true;
                            }
                        }

                        if (found)
                        {
                            // Align to the closest grid intersection
                            return new LuaVector2D(found_coord);
                        }
                    }
                    else
                    {
                        return new LuaVector2D(nl.NearestOnLine(mousemappos));
                    }
                }
            }

            if (snaptogrid)
            {
                return new LuaVector2D(General.Map.Grid.SnappedToGrid(mousemappos));
            }
            else
            {
                return new LuaVector2D(new Vector2D(
                        (float)Math.Round(mousemappos.x, General.Map.FormatInterface.VertexDecimals),
                        (float)Math.Round(mousemappos.y, General.Map.FormatInterface.VertexDecimals)
                    ));
            }
        }

        public static void ClearParameters()
        {
            ScriptContext.context.ui_parameters = new LuaUIParameters(16);
        }

        public static void AddParameter(string key, string label, string defaultvalue)
        {
            if (key == null || key == "")
            {
                throw new ScriptRuntimeException("AddParameter called with nil or empty key.");
            }

            if (label == null || label == "")
            {
                label = key;
            }

            

            if (ScriptContext.context.ui_parameters.keys == null)
            {
                ScriptContext.context.ui_parameters = new LuaUIParameters(16);
            }
            ScriptContext.context.ui_parameters.keys.Add(key);
            ScriptContext.context.ui_parameters.labels.Add(label);

            if (defaultvalue == null)
            {
                ScriptContext.context.ui_parameters.defaultvalues.Add("");
            }
            else
            {
                ScriptContext.context.ui_parameters.defaultvalues.Add(defaultvalue);
            }
        }

        public static Dictionary<string, string> AskForParameters()
        {
            Dictionary<string,string> output = AskForParametersNoClear();
            ClearParameters();
            return output;
        }

        public static Dictionary<string,string> AskForParametersNoClear()
        {
            // TODO add repeating with previous parameters
            // FIXME  check for duplicate keys

            if (ScriptContext.context.ui_parameters.keys == null
                || 
                ScriptContext.context.ui_parameters.keys.Count <= 0)
            {
                ScriptContext.context.Warn("AskForParameters called with no parameters set up.");
                return null;
            }

            Dictionary<string, string> output = ScriptMode.RequestAndWaitForParams();

            if (output == null)
            {
                throw new ScriptRuntimeException("User cancelled script execution.");
            }
            return output;
        }
    } // luaui class


    public struct LuaUIParameters
    {
        public List<string> keys;
        public List<string> labels;
        public List<string> defaultvalues;

        // default parameter workaround for C# not letting us do parameterless constructors
        public LuaUIParameters(int size)
        {
            keys = new List<string>(size);
            labels = new List<string>(size);
            defaultvalues = new List<string>(size);
        }
    } // luauiparameters
} // ns
