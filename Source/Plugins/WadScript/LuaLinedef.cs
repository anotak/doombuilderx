using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Geometry;

using MoonSharp.Interpreter;

namespace CodeImp.DoomBuilder.DBXLua
{
    // wrapper over Linedef for lua
    // TODO: rect, sideddef, Line2D line, activation
    [MoonSharpUserData]
    public class LuaLinedef
    {
        [MoonSharpHidden]
        internal Linedef linedef;

        public LuaVertex start
        {
            get
            {
                if (linedef.IsDisposed)
                {
                    throw new ScriptRuntimeException("Linedef has been disposed, can't get start vertex!");
                }
                return new LuaVertex(linedef.Start);
            }

            set
            {
                if (linedef.IsDisposed)
                {
                    throw new ScriptRuntimeException("Linedef has been disposed, can't set start vertex!");
                }
                if (value.vertex.IsDisposed)
                {
                    throw new ScriptRuntimeException("Vertex has been disposed, can't set start vertex!");
                }
                linedef.SetStartVertex(value.vertex);
            }
        }

        public LuaVertex end
        {
            get
            {
                if (linedef.IsDisposed)
                {
                    throw new ScriptRuntimeException("Linedef has been disposed, can't get end vertex!");
                }
                return new LuaVertex(linedef.End);
            }

            set
            {
                if (linedef.IsDisposed)
                {
                    throw new ScriptRuntimeException("Linedef has been disposed, can't set end vertex!");
                }
                if (value.vertex.IsDisposed)
                {
                    throw new ScriptRuntimeException("Vertex has been disposed, can't set end vertex!");
                }
                linedef.SetEndVertex(value.vertex);
            }
        }

        public int action
        {
            get
            {
                if (linedef.IsDisposed)
                {
                    throw new ScriptRuntimeException("Linedef has been disposed, can't get action!");
                }
                return linedef.Action;
            }
            set
            {
                if (linedef.IsDisposed)
                {
                    throw new ScriptRuntimeException("Linedef has been disposed, can't set action!");
                }
                linedef.Action = value;
            }
        }
        public int tag
        {
            get
            {
                
                if (linedef.IsDisposed)
                {
                    throw new ScriptRuntimeException("Linedef has been disposed, can't get tag!");
                }
                if (General.Map.FormatInterface.HasLinedefTag)
                {
                    return linedef.Tag;
                }
                else
                {
                    ScriptContext.context.WarnFormatIncompatible("Linedef Tag Numbers");
                    return 0;
                }
            }
            set
            {
                if (linedef.IsDisposed)
                {
                    throw new ScriptRuntimeException("Linedef has been disposed, can't set tag!");
                }
                if (General.Map.FormatInterface.HasLinedefTag)
                {
                    linedef.Tag = value;
                }
                else
                {
                    ScriptContext.context.WarnFormatIncompatible("Linedef Tag Numbers");
                }
            }
        }

        public int arg0
        {
            get
            {

                if (linedef.IsDisposed)
                {
                    throw new ScriptRuntimeException("Linedef has been disposed, can't get tag!");
                }
                if (General.Map.FormatInterface.HasActionArgs)
                {
                    return linedef.Args[0];
                }
                else
                {
                    ScriptContext.context.WarnFormatIncompatible("Action Arguments");
                    return 0;
                }
            }
            set
            {
                if (linedef.IsDisposed)
                {
                    throw new ScriptRuntimeException("Linedef has been disposed, can't set tag!");
                }
                if (General.Map.FormatInterface.HasActionArgs)
                {
                    // FIXME this is the extremely hacky workaround to
                    // not having a proper way to call .BeforePropsChange
                    // for setting args
                    // right now. we have to do this because we must maintain
                    // compatibility with non-DBX codebases, but hopefully
                    // we can do this in a better way in the future
                    linedef.Tag = linedef.Tag;

                    linedef.Args[0] = value;
                }
                else
                {
                    ScriptContext.context.WarnFormatIncompatible("Action Arguments");
                }
            }
        }

        public int arg1
        {
            get
            {

                if (linedef.IsDisposed)
                {
                    throw new ScriptRuntimeException("Linedef has been disposed, can't get tag!");
                }
                if (General.Map.FormatInterface.HasActionArgs)
                {
                    return linedef.Args[1];
                }
                else
                {
                    ScriptContext.context.WarnFormatIncompatible("Action Arguments");
                    return 0;
                }
            }
            set
            {
                if (linedef.IsDisposed)
                {
                    throw new ScriptRuntimeException("Linedef has been disposed, can't set tag!");
                }
                if (General.Map.FormatInterface.HasActionArgs)
                {
                    // FIXME this is the extremely hacky workaround to
                    // not having a proper way to call .BeforePropsChange
                    // for setting args
                    // right now. we have to do this because we must maintain
                    // compatibility with non-DBX codebases, but hopefully
                    // we can do this in a better way in the future
                    linedef.Tag = linedef.Tag;

                    linedef.Args[1] = value;
                }
                else
                {
                    ScriptContext.context.WarnFormatIncompatible("Action Arguments");
                }
            }
        }

        public int arg2
        {
            get
            {

                if (linedef.IsDisposed)
                {
                    throw new ScriptRuntimeException("Linedef has been disposed, can't get tag!");
                }
                if (General.Map.FormatInterface.HasActionArgs)
                {
                    return linedef.Args[2];
                }
                else
                {
                    ScriptContext.context.WarnFormatIncompatible("Action Arguments");
                    return 0;
                }
            }
            set
            {
                if (linedef.IsDisposed)
                {
                    throw new ScriptRuntimeException("Linedef has been disposed, can't set tag!");
                }
                if (General.Map.FormatInterface.HasActionArgs)
                {
                    // FIXME this is the extremely hacky workaround to
                    // not having a proper way to call .BeforePropsChange
                    // for setting args
                    // right now. we have to do this because we must maintain
                    // compatibility with non-DBX codebases, but hopefully
                    // we can do this in a better way in the future
                    linedef.Tag = linedef.Tag;

                    linedef.Args[2] = value;
                }
                else
                {
                    ScriptContext.context.WarnFormatIncompatible("Action Arguments");
                }
            }
        }

        public int arg3
        {
            get
            {

                if (linedef.IsDisposed)
                {
                    throw new ScriptRuntimeException("Linedef has been disposed, can't get tag!");
                }
                if (General.Map.FormatInterface.HasActionArgs)
                {
                    return linedef.Args[3];
                }
                else
                {
                    ScriptContext.context.WarnFormatIncompatible("Action Arguments");
                    return 0;
                }
            }
            set
            {
                if (linedef.IsDisposed)
                {
                    throw new ScriptRuntimeException("Linedef has been disposed, can't set tag!");
                }
                if (General.Map.FormatInterface.HasActionArgs)
                {
                    // FIXME this is the extremely hacky workaround to
                    // not having a proper way to call .BeforePropsChange
                    // for setting args
                    // right now. we have to do this because we must maintain
                    // compatibility with non-DBX codebases, but hopefully
                    // we can do this in a better way in the future
                    linedef.Tag = linedef.Tag;

                    linedef.Args[3] = value;
                }
                else
                {
                    ScriptContext.context.WarnFormatIncompatible("Action Arguments");
                }
            }
        }

        public int arg4
        {
            get
            {

                if (linedef.IsDisposed)
                {
                    throw new ScriptRuntimeException("Linedef has been disposed, can't get tag!");
                }
                if (General.Map.FormatInterface.HasActionArgs)
                {
                    return linedef.Args[4];
                }
                else
                {
                    ScriptContext.context.WarnFormatIncompatible("Action Arguments");
                    return 0;
                }
            }
            set
            {
                if (linedef.IsDisposed)
                {
                    throw new ScriptRuntimeException("Linedef has been disposed, can't set tag!");
                }
                if (General.Map.FormatInterface.HasActionArgs)
                {
                    // FIXME this is the extremely hacky workaround to
                    // not having a proper way to call .BeforePropsChange
                    // for setting args
                    // right now. we have to do this because we must maintain
                    // compatibility with non-DBX codebases, but hopefully
                    // we can do this in a better way in the future
                    linedef.Tag = linedef.Tag;

                    linedef.Args[4] = value;
                }
                else
                {
                    ScriptContext.context.WarnFormatIncompatible("Action Arguments");
                }
            }
        }

        [MoonSharpHidden]
        public LuaLinedef(Linedef l)
        {
            linedef = l;
        }

        public bool IsDisposed()
        {
            return linedef.IsDisposed;
        }

        public LuaSidedef GetFront()
        {
            if (linedef.IsDisposed)
            {
                throw new ScriptRuntimeException("Linedef has been disposed, can't get front side!");
            }
            if (linedef.Front == null || linedef.Front.IsDisposed)
            {
                return null;
            }
            return new LuaSidedef(linedef.Front);
        }

        public LuaSidedef GetBack()
        {
            if (linedef.IsDisposed)
            {
                throw new ScriptRuntimeException("Linedef has been disposed, can't get back side!");
            }
            if (linedef.Back == null || linedef.Back.IsDisposed)
            {
                return null;
            }
            return new LuaSidedef(linedef.Back);
        }

        public float GetLength()
        {
            if (linedef.IsDisposed)
            {
                throw new ScriptRuntimeException("Linedef has been disposed, can't GetLength!");
            }
            return linedef.Length;
        }

        public float GetLengthInv()
        {
            if (linedef.IsDisposed)
            {
                throw new ScriptRuntimeException("Linedef has been disposed, can't GetLengthInv!");
            }
            return linedef.LengthInv;
        }

        public float GetLengthSq()
        {
            if (linedef.IsDisposed)
            {
                throw new ScriptRuntimeException("Linedef has been disposed, can't GetLengthSq!");
            }
            return linedef.LengthSq;
        }

        public float GetLengthSqInv()
        {
            if (linedef.IsDisposed)
            {
                throw new ScriptRuntimeException("Linedef has been disposed, can't GetLengthSqInv!");
            }
            return linedef.LengthSqInv;
        }

        public float GetAngle()
        {
            if (linedef.IsDisposed)
            {
                throw new ScriptRuntimeException("Linedef has been disposed, can't GetAngle!");
            }
            return linedef.Angle;
        }

        public float GetAngleDegrees()
        {
            if (linedef.IsDisposed)
            {
                throw new ScriptRuntimeException("Linedef has been disposed, can't GetAngleDegrees!");
            }
            return linedef.AngleDeg;
        }

        public void FlipVertices()
        {
            if (linedef.IsDisposed)
            {
                throw new ScriptRuntimeException("Linedef has been disposed, can't FlipVertices!");
            }
            linedef.FlipVertices();
        }

        public void FlipSidedefs()
        {
            if (linedef.IsDisposed)
            {
                throw new ScriptRuntimeException("Linedef has been disposed, can't FlipSidedefs!");
            }
            linedef.FlipSidedefs();
        }

        public void Flip()
        {
            if (linedef.IsDisposed)
            {
                throw new ScriptRuntimeException("Linedef has been disposed, can't Flip!");
            }
            linedef.FlipVertices();
            linedef.FlipSidedefs();
        }

        public LuaVector2D GetSidePoint(bool front)
        {
            if (linedef.IsDisposed)
            {
                throw new ScriptRuntimeException("Linedef has been disposed, can't GetSidePoint!");
            }
            return new LuaVector2D(linedef.GetSidePoint(front));
        }

        public LuaVector2D GetCenterPoint()
        {
            if (linedef.IsDisposed)
            {
                throw new ScriptRuntimeException("Linedef has been disposed, can't GetCenterPoint!");
            }
            return new LuaVector2D(linedef.GetCenterPoint());
        }

        public List<LuaVector2D> GetGridIntersections()
        {
            if (linedef.IsDisposed)
            {
                throw new ScriptRuntimeException("Linedef has been disposed, can't GetGridIntersections!");
            }

            List<Vector2D> intersections = linedef.GetGridIntersections();
            int count = intersections.Count;

            List<LuaVector2D> output = new List<LuaVector2D>(count);

            for (int i = 0; i < count; i++)
            {
                output.Add(new LuaVector2D(intersections[i]));
            }

            return output;
        }

        public LuaVector2D NearestOnLine(LuaVector2D pos)
        {
            if (linedef.IsDisposed)
            {
                throw new ScriptRuntimeException("Linedef has been disposed, can't NearestOnLine!");
            }

            return new LuaVector2D(linedef.NearestOnLine(pos.vec));
        }

        public float SafeDistanceToSq(LuaVector2D p, bool bounded)
        {
            if (linedef.IsDisposed)
            {
                throw new ScriptRuntimeException("Linedef has been disposed, can't SafeDistanceToSq!");
            }

            return linedef.SafeDistanceToSq(p.vec,bounded);
        }

        public float SafeDistanceTo(LuaVector2D p, bool bounded)
        {
            if (linedef.IsDisposed)
            {
                throw new ScriptRuntimeException("Linedef has been disposed, can't SafeDistanceTo!");
            }

            return linedef.SafeDistanceTo(p.vec, bounded);
        }

        public float DistanceToSq(LuaVector2D p, bool bounded)
        {
            if (linedef.IsDisposed)
            {
                throw new ScriptRuntimeException("Linedef has been disposed, can't DistanceToSq!");
            }

            return linedef.DistanceToSq(p.vec, bounded);
        }

        public float DistanceTo(LuaVector2D p, bool bounded)
        {
            if (linedef.IsDisposed)
            {
                throw new ScriptRuntimeException("Linedef has been disposed, can't DistanceTo!");
            }

            return linedef.DistanceTo(p.vec, bounded);
        }
        
        // This tests on which side of the line the given coordinates are
        // returns < 0 for front (right) side, > 0 for back (left) side and 0 if on the line
        public float SideOfLine(LuaVector2D p)
        {
            if (linedef.IsDisposed)
            {
                throw new ScriptRuntimeException("Linedef has been disposed, can't SideOfLine!");
            }
            return linedef.SideOfLine(p.vec);
        }

        public LuaLinedef Split(LuaVertex v)
        {
            if (linedef.IsDisposed)
            {
                throw new ScriptRuntimeException("Linedef has been disposed, can't Split!");
            }
            if (v.vertex.IsDisposed)
            {
                throw new ScriptRuntimeException("Vertex has been disposed, can't Split!");
            }

            v.vertex.SnapToAccuracy();

            LuaLinedef l = new LuaLinedef(linedef.Split(v.vertex));

            // Clear selection
            General.Map.Map.ClearAllSelected();

            // Update cached values
            General.Map.Map.Update();
            return l;
        }

        // ano - probably a bad idea to let lua access? let's think about this
        /*
        // This joins the line with another line
        // This line will be disposed
        // Returns false when the operation could not be completed
        public bool Join(LuaLinedef other)
        {
            if (linedef.IsDisposed || other.linedef.IsDisposed)
            {
                throw new ScriptRuntimeException("Linedef has been disposed, can't Split!");
            }
            return linedef.Join(other.linedef);
        }
        */

        public override string ToString()
        {
            if (linedef.IsDisposed)
            {
                return "Disposed " + linedef.ToString();
            }
            return linedef.ToString();
        }
    } // class lualinedef
} // ns
