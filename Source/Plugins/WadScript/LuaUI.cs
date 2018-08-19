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

        // FIXME provide a version with snaptogrid based on shift key but no snaptonearest
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

        public static void AddParameter(string key, string label, DynValue defaultvalue)
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

            if (defaultvalue == null || defaultvalue.IsNil() || defaultvalue.IsVoid())
            {
                ScriptContext.context.ui_parameters.defaultvalues.Add("");
                ScriptContext.context.ui_parameters.datatypes.Add(DataType.String); // bad choice? dunno
            }
            else
            {
                switch (defaultvalue.Type)
                {
                    case DataType.String:
                        ScriptContext.context.ui_parameters.defaultvalues.Add(defaultvalue.String);
                        ScriptContext.context.ui_parameters.datatypes.Add(defaultvalue.Type);
                        break;
                    case DataType.Number:
                    case DataType.Boolean:
                        ScriptContext.context.ui_parameters.defaultvalues.Add(defaultvalue.ToString());
                        ScriptContext.context.ui_parameters.datatypes.Add(defaultvalue.Type);
                        break;
                    case DataType.UserData:
                        // TODO - add handling for some of our types like Vector2D?
                        string error_value = "";
                        if (defaultvalue.UserData.Object != null)
                        {
                            error_value = defaultvalue.UserData.Object.ToString();
                        }
                        throw new ScriptRuntimeException(
                            "weird default value data type ("
                            + defaultvalue.UserData.Descriptor.Name +
                            ") for parameter "
                            + key
                            + ": " + error_value);
                    case DataType.Function: // is there some way to handle a function for this?
                    case DataType.ClrFunction:
                    case DataType.Nil:
                    case DataType.Void:
                    case DataType.Table:
                    case DataType.Thread:
                    case DataType.Tuple:
                    case DataType.TailCallRequest:
                    case DataType.YieldRequest:
                    default:
                        throw new ScriptRuntimeException(
                            "weird default value data type ("
                            + defaultvalue.Type.ToErrorTypeString() +
                            ") for parameter "
                            + key
                            + ": " + defaultvalue);
                }
            }
        }

        public static Table AskForParameters()
        {
            Table output = AskForParametersNoClear();
            ClearParameters();
            return output;
        }

        public static Table AskForParametersNoClear()
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

            Dictionary<string, string> dict = ScriptMode.RequestAndWaitForParams();

            if (dict == null)
            {
                // FIXME do this without an error message probably?
                throw new ScriptRuntimeException("User cancelled script execution.");
            }
            
            Table output = new Table(ScriptContext.context.script);

            foreach (KeyValuePair<string, string> pair in dict)
            {
                output.Set(
                    pair.Key,
                    DynValue.FromObject(ScriptContext.context.script, pair.Value)
                    );
            }

            int count = ScriptContext.context.ui_parameters.datatypes.Count;
            if (ScriptContext.context.ui_parameters.keys.Count != count)
            {
                Logger.WriteLogLine("Inconsistent parameter counts in Lua script!");
                Logger.WriteLogLine("datatypes: ");
                foreach (DataType o in ScriptContext.context.ui_parameters.datatypes)
                {
                    Logger.WriteLogLine(o.ToString());
                }

                Logger.WriteLogLine("keys: ");
                foreach (string o in ScriptContext.context.ui_parameters.keys)
                {
                    if (o == null)
                    {
                        Logger.WriteLogLine("null");
                    }
                    else
                    {
                        Logger.WriteLogLine(o);
                    }
                }

                Logger.WriteLogLine("labels: ");
                foreach (string o in ScriptContext.context.ui_parameters.labels)
                {
                    if (o == null)
                    {
                        Logger.WriteLogLine("null");
                    }
                    else
                    {
                        Logger.WriteLogLine(o);
                    }
                }

                Logger.WriteLogLine("labels: ");
                foreach (string o in ScriptContext.context.ui_parameters.defaultvalues)
                {
                    if (o == null)
                    {
                        Logger.WriteLogLine("null");
                    }
                    else
                    {
                        Logger.WriteLogLine(o);
                    }
                }
                throw new ScriptRuntimeException("Inconsistent parameter counts in Lua script, might be a DBXLua bug. :(");
            }

            for (int i = 0; i < count; i++)
            {
                DataType cur_type = ScriptContext.context.ui_parameters.datatypes[i];
                DynValue cur_value = output.Get(ScriptContext.context.ui_parameters.keys[i]);
                switch (cur_type)
                {
                    case DataType.Boolean:
                        cur_value.Assign(DynValue.NewBoolean(cur_value.CastToBool()));
                        //output.Set(ScriptContext.context.ui_parameters.keys[i], cur_value.CastToBool());
                        break;
                    case DataType.Number:
                        double d = cur_value.CastToNumber() ?? 0;
                        cur_value.Assign(DynValue.NewNumber(d));
                        break;
                    default:
                        break; // nope
                }
            }

            return output;
        }
    } // luaui class


    public struct LuaUIParameters
    {
        public List<string> keys;
        public List<string> labels;
        public List<string> defaultvalues;
        public List<DataType> datatypes;

        // default parameter workaround for C# not letting us do parameterless constructors
        public LuaUIParameters(int size)
        {
            keys = new List<string>(size);
            labels = new List<string>(size);
            defaultvalues = new List<string>(size);
            datatypes = new List<DataType>(size);
        }
    } // luauiparameters
} // ns
