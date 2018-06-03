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

        // ????? should we snaptoaccuracy here
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
