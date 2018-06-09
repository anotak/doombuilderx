using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CodeImp.DoomBuilder.Map;

using MoonSharp.Interpreter;

namespace CodeImp.DoomBuilder.DBXLua
{
    // wrapper over Vertex for lua
    [MoonSharpUserData]
    public class LuaVertex
    {
        [MoonSharpHidden]
        internal Vertex vertex;

        public LuaVector2D position {
            get
            {
                if (vertex.IsDisposed)
                {
                    throw new ScriptRuntimeException("Vertex has been disposed, can't get position!");
                }
                return new LuaVector2D(vertex.Position);
            }
            set
            {
                if (vertex.IsDisposed)
                {
                    throw new ScriptRuntimeException("Vertex has been disposed, can't set position!");
                }
                vertex.Move(value.vec);
            }
        }

        [MoonSharpHidden]
        public LuaVertex(Vertex v)
        {
            vertex = v;
        }

        public bool IsDisposed()
        {
            return vertex.IsDisposed;
        }

        public DynValue GetUDMFField(string key)
        {
            if (vertex.IsDisposed)
            {
                throw new ScriptRuntimeException("Vertex has been disposed, can't GetUDMFField()!");
            }
            return LuaTypeConversion.GetUDMFField(vertex, key, General.Map.Config.VertexFields);
        }

        public void SetUDMFField(string key, DynValue value)
        {
            if (vertex.IsDisposed)
            {
                throw new ScriptRuntimeException("Vertex has been disposed, can't SetUDMFField()!");
            }
            LuaTypeConversion.SetUDMFField(vertex, key, value);
        }

        public List<LuaLinedef> GetLines()
        {
            if (vertex.IsDisposed)
            {
                throw new ScriptRuntimeException("Vertex has been disposed, can't GetLines()");
            }
            List<LuaLinedef> output = new List<LuaLinedef>();

            foreach (Linedef l in vertex.Linedefs)
            {
                if (!l.IsDisposed)
                {
                    output.Add(new LuaLinedef(l));
                }
            }

            return output;
        }

        // returns false if other is same vertex
        public bool SharesLine(LuaVertex other)
        {
            if (vertex.IsDisposed || other.vertex.IsDisposed)
            {
                throw new ScriptRuntimeException("Vertex has been disposed, can't SharesLines()");
            }
            if (vertex == other.vertex)
            {
                // FIXME add a warning for this probably
                // is it possible for this to ever actually be correct and true
                return false;
            }
            foreach (Linedef l in vertex.Linedefs)
            {
                if (!l.IsDisposed)
                {
                    if (l.Start == other.vertex || l.End == other.vertex)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public void SnapToGrid()
        {
            if (vertex.IsDisposed)
            {
                throw new ScriptRuntimeException("Vertex has been disposed, can't SnapToGrid!");
            }
            vertex.SnapToGrid();
        }

        public void SnapToAccuracy()
        {
            if (vertex.IsDisposed)
            {
                throw new ScriptRuntimeException("Vertex has been disposed, can't SnapToAccuracy!");
            }
            vertex.SnapToAccuracy();
        }

        public void Join(LuaVertex other)
        {
            if (vertex.IsDisposed)
            {
                throw new ScriptRuntimeException("Joining vertex has been disposed!");
            }
            if (other.vertex.IsDisposed)
            {
                throw new ScriptRuntimeException("Joinee vertex has been disposed!");
            }
            vertex.Join(other.vertex);
        }

        public override string ToString()
        {
            if (vertex.IsDisposed)
            {
                return "Disposed " + vertex.ToString();
            }
            return vertex.ToString();
        }
    } // vertex
} // ns
