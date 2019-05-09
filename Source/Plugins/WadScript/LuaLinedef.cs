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
    // TODO: rect, Line2D line, activation
    [MoonSharpUserData]
    public class LuaLinedef
    {
        [MoonSharpHidden]
        internal Linedef linedef;

        public bool selected
        {
            get
            {
                if (linedef.IsDisposed)
                {
                    throw new ScriptRuntimeException("Linedef has been disposed, can't get selected status.");
                }
                
                return linedef.Selected;
            }

            set
            {
                if (linedef.IsDisposed)
                {
                    throw new ScriptRuntimeException("Linedef has been disposed, can't set selected status.");
                }

                ScriptMode.SelectLinedef(linedef, value);
            } // done w selection setter
        }

        public bool marked
        {
            get
            {
                if (linedef.IsDisposed)
                {
                    throw new ScriptRuntimeException("linedef has been disposed, can't get marked status.");
                }

                return linedef.Marked;
            }

            set
            {
                if (linedef.IsDisposed)
                {
                    throw new ScriptRuntimeException("linedef has been disposed, can't set marked status.");
                }

                linedef.Marked = value;
            }
        }

        public LuaVertex start_vertex
        {
            get
            {
                if (linedef.IsDisposed)
                {
                    throw new ScriptRuntimeException("Linedef has been disposed, can't get start vertex.");
                }
                return new LuaVertex(linedef.Start);
            }

            set
            {
                if (linedef.IsDisposed)
                {
                    throw new ScriptRuntimeException("Linedef has been disposed, can't set start vertex.");
                }
                if (value.vertex.IsDisposed)
                {
                    throw new ScriptRuntimeException("Vertex has been disposed, can't set start vertex.");
                }
                linedef.SetStartVertex(value.vertex);

                linedef.NeedUpdate();
                linedef.UpdateCache();
            }
        }

        // we can't name this 'end' because it's a reserved keyword in lua, alas :<
        public LuaVertex end_vertex
        {
            get
            {
                if (linedef.IsDisposed)
                {
                    throw new ScriptRuntimeException("Linedef has been disposed, can't get end vertex.");
                }
                return new LuaVertex(linedef.End);
            }

            set
            {
                if (linedef.IsDisposed)
                {
                    throw new ScriptRuntimeException("Linedef has been disposed, can't set end vertex.");
                }
                if (value.vertex.IsDisposed)
                {
                    throw new ScriptRuntimeException("Vertex has been disposed, can't set end vertex.");
                }
                linedef.SetEndVertex(value.vertex);

                linedef.NeedUpdate();
                linedef.UpdateCache();
            }
        }

        public int action
        {
            get
            {
                if (linedef.IsDisposed)
                {
                    throw new ScriptRuntimeException("Linedef has been disposed, can't get action.");
                }
                return linedef.Action;
            }
            set
            {
                if (linedef.IsDisposed)
                {
                    throw new ScriptRuntimeException("Linedef has been disposed, can't set action.");
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
                    throw new ScriptRuntimeException("Linedef has been disposed, can't get tag.");
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
                    throw new ScriptRuntimeException("Linedef has been disposed, can't set tag.");
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
                    throw new ScriptRuntimeException("Linedef has been disposed, can't get arg0.");
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
                    throw new ScriptRuntimeException("Linedef has been disposed, can't set arg0.");
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
                    throw new ScriptRuntimeException("Linedef has been disposed, can't get arg1.");
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
                    throw new ScriptRuntimeException("Linedef has been disposed, can't set arg1.");
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
                    throw new ScriptRuntimeException("Linedef has been disposed, can't get arg2.");
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
                    throw new ScriptRuntimeException("Linedef has been disposed, can't set arg2.");
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
                    throw new ScriptRuntimeException("Linedef has been disposed, can't get arg3.");
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
                    throw new ScriptRuntimeException("Linedef has been disposed, can't set arg3.");
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
                    throw new ScriptRuntimeException("Linedef has been disposed, can't get arg4.");
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
                    throw new ScriptRuntimeException("Linedef has been disposed, can't set arg4.");
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

        public int GetIndex()
        {
            if (linedef.IsDisposed)
            {
                throw new ScriptRuntimeException("Linedef has been disposed, can't GetIndex().");
            }

            return linedef.Index;
        }

        public bool IsFlagSet(string flagname)
        {
            if (linedef.IsDisposed)
            {
                throw new ScriptRuntimeException("Linedef has been disposed, can't IsFlagSet().");
            }
            if (flagname == null)
            {
                throw new ScriptRuntimeException("Flag name is nil, can't IsFlagSet() (not enough arguments maybe?).");
            }
            // FIXME warn on no such flag
            return linedef.IsFlagSet(flagname);
        }

        public void SetFlag(string flagname, bool val = true)
        {
            if (linedef.IsDisposed)
            {
                throw new ScriptRuntimeException("Linedef has been disposed, can't SetFlag().");
            }
            if (flagname == null)
            {
                throw new ScriptRuntimeException("flagname is nil, can't SetFlag() (not enough arguments maybe?).");
            }
            // FIXME warn on no such flag
            linedef.SetFlag(flagname, val);
        }

        public void ClearFlags()
        {
            if (linedef.IsDisposed)
            {
                throw new ScriptRuntimeException("Linedef has been disposed, can't ClearFlags().");
            }
            linedef.ClearFlags();
        }

        public DynValue GetUDMFField(string key)
        {
            if (linedef.IsDisposed)
            {
                throw new ScriptRuntimeException("Linedef has been disposed, can't GetUDMFField().");
            }
            return LuaTypeConversion.GetUDMFField(linedef,key, General.Map.Config.LinedefFields);
        }

        public void SetUDMFField(string key, DynValue value)
        {
            if (linedef.IsDisposed)
            {
                throw new ScriptRuntimeException("Linedef has been disposed, can't SetUDMFField().");
            }

            LuaTypeConversion.SetUDMFField(linedef, key, value);
        }

        public Table GetUDMFTable()
        {
            if (linedef.IsDisposed)
            {
                throw new ScriptRuntimeException("Linedef has been disposed, can't GetUDMFTable().");
            }

            return LuaTypeConversion.GetUDMFTable(linedef);
        }

        public LuaSidedef GetFront()
        {
            if (linedef.IsDisposed)
            {
                throw new ScriptRuntimeException("Linedef has been disposed, can't get front side.");
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
                throw new ScriptRuntimeException("Linedef has been disposed, can't get back side.");
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
                throw new ScriptRuntimeException("Linedef has been disposed, can't GetLength.");
            }
            return linedef.Length;
        }

        public float GetLengthInv()
        {
            if (linedef.IsDisposed)
            {
                throw new ScriptRuntimeException("Linedef has been disposed, can't GetLengthInv.");
            }
            return linedef.LengthInv;
        }

        public float GetLengthSq()
        {
            if (linedef.IsDisposed)
            {
                throw new ScriptRuntimeException("Linedef has been disposed, can't GetLengthSq.");
            }
            return linedef.LengthSq;
        }

        public float GetLengthSqInv()
        {
            if (linedef.IsDisposed)
            {
                throw new ScriptRuntimeException("Linedef has been disposed, can't GetLengthSqInv.");
            }
            return linedef.LengthSqInv;
        }

        public float GetAngle()
        {
            if (linedef.IsDisposed)
            {
                throw new ScriptRuntimeException("Linedef has been disposed, can't GetAngle.");
            }
            return linedef.Angle;
        }

        public float GetAngleDegrees()
        {
            if (linedef.IsDisposed)
            {
                throw new ScriptRuntimeException("Linedef has been disposed, can't GetAngleDegrees.");
            }
            return linedef.AngleDeg;
        }

        public void FlipVertices()
        {
            if (linedef.IsDisposed)
            {
                throw new ScriptRuntimeException("Linedef has been disposed, can't FlipVertices.");
            }
            linedef.FlipVertices();
        }

        public void FlipSidedefs()
        {
            if (linedef.IsDisposed)
            {
                throw new ScriptRuntimeException("Linedef has been disposed, can't FlipSidedefs.");
            }
            linedef.FlipSidedefs();
        }

        public void Flip()
        {
            if (linedef.IsDisposed)
            {
                throw new ScriptRuntimeException("Linedef has been disposed, can't Flip.");
            }
            linedef.FlipVertices();
            linedef.FlipSidedefs();
            linedef.NeedUpdate();
            linedef.UpdateCache();
        }

        public LuaVector2D GetSidePoint(bool front)
        {
            if (linedef.IsDisposed)
            {
                throw new ScriptRuntimeException("Linedef has been disposed, can't GetSidePoint.");
            }
            return new LuaVector2D(linedef.GetSidePoint(front));
        }

        public LuaVector2D GetCenterPoint()
        {
            if (linedef.IsDisposed)
            {
                throw new ScriptRuntimeException("Linedef has been disposed, can't GetCenterPoint.");
            }
            return new LuaVector2D(linedef.GetCenterPoint());
        }

        public List<LuaVector2D> GetGridIntersections()
        {
            if (linedef.IsDisposed)
            {
                throw new ScriptRuntimeException("Linedef has been disposed, can't GetGridIntersections.");
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
                throw new ScriptRuntimeException("Linedef has been disposed, can't NearestOnLine.");
            }

            return new LuaVector2D(linedef.NearestOnLine(pos.vec));
        }

        public float SafeDistanceToSq(LuaVector2D p, bool bounded)
        {
            if (linedef.IsDisposed)
            {
                throw new ScriptRuntimeException("Linedef has been disposed, can't SafeDistanceToSq.");
            }

            return linedef.SafeDistanceToSq(p.vec,bounded);
        }

        public float SafeDistanceTo(LuaVector2D p, bool bounded)
        {
            if (linedef.IsDisposed)
            {
                throw new ScriptRuntimeException("Linedef has been disposed, can't SafeDistanceTo.");
            }

            return linedef.SafeDistanceTo(p.vec, bounded);
        }

        public float DistanceToSq(LuaVector2D p, bool bounded)
        {
            if (linedef.IsDisposed)
            {
                throw new ScriptRuntimeException("Linedef has been disposed, can't DistanceToSq.");
            }

            return linedef.DistanceToSq(p.vec, bounded);
        }

        public float DistanceTo(LuaVector2D p, bool bounded)
        {
            if (linedef.IsDisposed)
            {
                throw new ScriptRuntimeException("Linedef has been disposed, can't DistanceTo.");
            }

            return linedef.DistanceTo(p.vec, bounded);
        }

        public LuaVector2D GetNormal()
        {
            if (linedef.IsDisposed)
            {
                throw new ScriptRuntimeException("Linedef has been disposed, can't GetNormal.");
            }

            return new LuaVector2D(linedef.Line.GetPerpendicular().GetNormal());
        }
        
        // This tests on which side of the line the given coordinates are
        // returns < 0 for front (right) side, > 0 for back (left) side and 0 if on the line
        public float SideOfLine(LuaVector2D p)
        {
            if (linedef.IsDisposed)
            {
                throw new ScriptRuntimeException("Linedef has been disposed, can't SideOfLine.");
            }
            return linedef.SideOfLine(p.vec);
        }

        // returns a tuple of vertex, linedef, linedef
        public DynValue Split(LuaVertex v)
        {
            if (linedef.IsDisposed)
            {
                throw new ScriptRuntimeException("Linedef has been disposed, can't Split.");
            }
            if (v == null)
            {
                throw new ScriptRuntimeException("Vertex is null, can't Split (not enough arguments maybe?).");
            }
            if (v.vertex.IsDisposed)
            {
                throw new ScriptRuntimeException("Vertex has been disposed, can't Split.");
            }

            v.vertex.SnapToAccuracy();

            LuaLinedef newline = new LuaLinedef(linedef.Split(v.vertex));

            if (newline.linedef == null)
            {
                throw new ScriptRuntimeException(
                    "Split returned null linedef (max linedef limit reached? current count is "
                    + General.Map.Map.Linedefs.Count + " of " + General.Map.FormatInterface.MaxLinedefs
                    + ")");
            }

            // Update cached values
            General.Map.Map.Update();

            return DynValue.NewTuple(
                DynValue.FromObject(ScriptContext.context.script, v),
                DynValue.FromObject(ScriptContext.context.script, newline),
                DynValue.FromObject(ScriptContext.context.script, this)
                    );
        }

        // splits middle of line
        // returns a tuple of vertex, linedef, linedef
        public DynValue Split()
        {
            if (linedef.IsDisposed)
            {
                throw new ScriptRuntimeException("Linedef has been disposed, can't Split.");
            }

            Vertex v = General.Map.Map.CreateVertex(linedef.GetCenterPoint());
            if (v == null)
            {
                throw new ScriptRuntimeException(
                    "Split returned null vertex (max vertex limit reached? current count is "
                    + General.Map.Map.Vertices.Count
                    + ")");
            }

            v.SnapToAccuracy();

            LuaLinedef newline = new LuaLinedef(linedef.Split(v));

            if (newline.linedef == null)
            {
                throw new ScriptRuntimeException(
                    "Split returned null linedef (max linedef limit reached? current count is "
                    + General.Map.Map.Linedefs.Count + " of " + General.Map.FormatInterface.MaxLinedefs
                    + ")");
            }

            // Update cached values
            General.Map.Map.Update();

            return DynValue.NewTuple(
                DynValue.FromObject(ScriptContext.context.script, new LuaVertex(v)),
                DynValue.FromObject(ScriptContext.context.script, newline),
                DynValue.FromObject(ScriptContext.context.script, this)
                    );
        }

        public List<LuaLinedef> GetOverlappingLinedefs()
        {
            if (linedef.IsDisposed)
            {
                throw new ScriptRuntimeException("Linedef is disposed, can't GetOverlappingLinedefs().");
            }

            List<LuaLinedef> output = new List<LuaLinedef>();

            // create a bounding box for our line
            float bbox_x1 = Math.Min(linedef.Start.Position.x, linedef.End.Position.x);
            float bbox_x2 = Math.Max(linedef.Start.Position.x, linedef.End.Position.x);

            float bbox_y1 = Math.Min(linedef.Start.Position.y, linedef.End.Position.y);
            float bbox_y2 = Math.Max(linedef.Start.Position.y, linedef.End.Position.y);

            Line2D l2d = linedef.Line;

            foreach (Linedef map_line in General.Map.Map.Linedefs)
            {
                // dont test yourself or you'll wreck yourself
                if (map_line.Index == linedef.Index)
                {
                    continue;
                }
                // bounding box check
                if (
                    (map_line.Start.Position.x < bbox_x1 && map_line.End.Position.x < bbox_x1)
                    || (map_line.Start.Position.x > bbox_x2 && map_line.End.Position.x > bbox_x2)
                    || (map_line.Start.Position.y < bbox_y1 && map_line.End.Position.y < bbox_y1)
                    || (map_line.Start.Position.y > bbox_y2 && map_line.End.Position.y > bbox_y2)
                    )
                {
                    continue;
                }

                float u_ray = 0f;

                if (map_line.Line.GetIntersection(l2d, out u_ray))
                {
                    if (u_ray > 0.0 && u_ray < 1.0f && !float.IsNaN(u_ray))
                    {
                        output.Add(new LuaLinedef(map_line));
                    }
                }
            } // foreach (Linedef map_line in General.Map.Map.Linedefs)

            return output;
        }

        // ano - TODO probably a bad idea to let lua access? let's think about this
        /*
        // This joins the line with another line
        // This line will be disposed
        // Returns false when the operation could not be completed
        public bool Join(LuaLinedef other)
        {
            if (linedef.IsDisposed || other.linedef.IsDisposed)
            {
                throw new ScriptRuntimeException("Linedef has been disposed, can't Split.");
            }
            return linedef.Join(other.linedef);
        }
        */

        // used to swap indices sort of
        public void SwapProperties(LuaLinedef other)
        {
            if (linedef.IsDisposed)
            {
                throw new ScriptRuntimeException("Linedef has been disposed, can't SwapProperties.");
            }
            if (other == null)
            {
                throw new ScriptRuntimeException("Other linedef is null, can't SwapProperties (not enough arguments maybe?).");
            }
            if (other.linedef.IsDisposed)
            {
                throw new ScriptRuntimeException("Other linedef has been disposed, can't SwapProperties.");
            }

            linedef.SwapProperties(other.linedef);
        }

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
