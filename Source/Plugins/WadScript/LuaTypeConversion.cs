using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MoonSharp.Interpreter;

using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Types;
using CodeImp.DoomBuilder.Config;

namespace CodeImp.DoomBuilder.DBXLua
{
    public class LuaTypeConversion
    {
        // returns a tuple, of (value, default),
        // where default is true if set to a default value
        public static DynValue GetUDMFField(MapElement element, string key, List<UniversalFieldInfo> defaults)
        {
            if (key == null)
            {
                throw new ScriptRuntimeException("key is nil, can't GetUDMFField() (not enough arguments maybe?)");
            }
            if (element == null)
            {
                throw new ScriptRuntimeException("map element is nil, can't GetUDMFField()");
            }
            if (element.IsDisposed)
            {
                throw new ScriptRuntimeException("map element is disposed, can't GetUDMFField()");
            }

            DynValue[] output = new DynValue[2];
            if (!General.Map.FormatInterface.HasCustomFields)
            {
                ScriptContext.context.WarnFormatIncompatible("UDMF fields not supported!");

                output[0] = DynValue.NewNumber(0);
                output[1] = DynValue.NewBoolean(true);
                return DynValue.NewTuple(output);
            }
            key = UniValue.ValidateName(key);

            if (key == "")
            {
                output[0] = DynValue.NewNumber(0);
                output[1] = DynValue.NewBoolean(true);
                return DynValue.NewTuple(output);
            }

            if (element.Fields.ContainsKey(key))
            {
                UniValue field = element.Fields[key];

                output[0] = DynValue.FromObject(ScriptContext.context.script, field.Value);

                output[1] = DynValue.NewBoolean(true);
                return DynValue.NewTuple(output);
            }
            else
            {
                output[0] = DynValue.NewNumber(0);

                if (defaults != null)
                {
                    // TODO - for performance, reimplement as an O(1) operation
                    foreach (UniversalFieldInfo d in defaults)
                    {
                        if (d.Name == key)
                        {
                            output[0] = DynValue.FromObject(ScriptContext.context.script, d.Default);
                        }
                    }
                }
                
                output[1] = DynValue.NewBoolean(true);
                return DynValue.NewTuple(output);
            }
        }

        public static void SetUDMFField(MapElement element, string key, DynValue value)
        {
            if (!General.Map.FormatInterface.HasCustomFields)
            {
                ScriptContext.context.WarnFormatIncompatible("UDMF fields not supported!");
                return;
            }

            if (key == null)
            {
                throw new ScriptRuntimeException("key is nil, can't SetUDMFField() (not enough arguments maybe?)");
            }
            if (element == null)
            {
                throw new ScriptRuntimeException("map element is nil, can't SetUDMFField()");
            }
            if (element.IsDisposed)
            {
                throw new ScriptRuntimeException("map element is disposed, can't SetUDMFField()");
            }

            if (value.IsNilOrNan() || value.IsVoid())
            {
                throw new ScriptRuntimeException("value is nil, nan, or void. can't SetUDMFField() (not enough arguments maybe?)");
            }
            
            key = UniValue.ValidateName(key);

            if (key == "")
            {
                return;
            }

            UniFields fields = element.Fields;
            if (fields.ContainsKey(key))
            {
                try
                {
                    fields[key].Value = value.ToObject();
                }
                catch (ArgumentException)
                {
                    throw new ScriptRuntimeException("error setting UDMF field " + key + ", must be int, float, string or bool");
                }
            }
            else
            {
                UniValue new_field = new UniValue();

                try
                {
                    new_field.Value = value.ToObject();
                }
                catch (ArgumentException)
                {
                    throw new ScriptRuntimeException("error setting UDMF field " + key + ", must be int, float, string or bool");
                }

                fields.Add(key, new_field);
            }
        }

        // TODO determine if do we need this for real?
        public static Table GetUDMFTable(MapElement element)
        {
            Table output = new Table(ScriptContext.context.script);

            if (!General.Map.FormatInterface.HasCustomFields)
            {
                ScriptContext.context.WarnFormatIncompatible("UDMF fields not supported!");
                return output;
            }
            
            foreach (KeyValuePair<string, UniValue> pair in element.Fields)
            {
                output.Set(pair.Key, DynValue.FromObject(ScriptContext.context.script, pair.Value));
            }

            return output;
        }

        public static Table TableFromFieldInfos(List<UniversalFieldInfo> fieldinfos)
        {
            Table output = new Table(ScriptContext.context.script);
            foreach (UniversalFieldInfo f in fieldinfos)
            {
                output.Set(DynValue.NewString(f.Name), DynValue.FromObject(ScriptContext.context.script,f.Default));
            }
            return output;
        }
    } // class
} // ns
