using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Geometry;

using MoonSharp.Interpreter;

namespace CodeImp.DoomBuilder.DBXLua
{
    // wrapper over Vertex for lua
    [MoonSharpUserData]
    public class LuaVertex
    {
        [MoonSharpHidden]
        internal Vertex vertex;

        public bool selected
        {
            get
            {
                if (vertex.IsDisposed)
                {
                    throw new ScriptRuntimeException("vertex has been disposed, can't get selected status!");
                }

                return vertex.Selected;
            }

            set
            {
                if (vertex.IsDisposed)
                {
                    throw new ScriptRuntimeException("vertex has been disposed, can't set selected status!");
                }

                vertex.Selected = value;
            }
        }

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

                if (float.IsNaN(value.x) || float.IsNaN(value.y) ||
                   float.IsInfinity(value.x) || float.IsInfinity(value.y))
                {
                    throw new ScriptRuntimeException("Invalid vertex position! The given vertex coordinates cannot be NaN or Infinite.");
                }

                if (value.vec != vertex.Position)
                {
                    vertex.Move(value.vec);

                    foreach (Linedef our_line in vertex.Linedefs)
                    {
                        our_line.UpdateCache();
                    }
                }
            }
        }

        [MoonSharpHidden]
        public LuaVertex(Vertex v)
        {
            vertex = v;
        }

        public int GetIndex()
        {
            if (vertex.IsDisposed)
            {
                throw new ScriptRuntimeException("Vertex has been disposed, can't GetIndex()!");
            }

            return vertex.Index;
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

        public bool TryToMove(LuaVector2D v)
        {
            return TryToMove(v.x, v.y);
        }

        // tries to move to a new position and then moves back if any of
        // the vertex's lines overlap other lines
        public bool TryToMove(float x, float y)
        {
            if (float.IsNaN(x) || float.IsNaN(y) ||
                   float.IsInfinity(x) || float.IsInfinity(y))
            {
                throw new ScriptRuntimeException("Invalid vertex position! The given vertex coordinates cannot be NaN or Infinite.");
            }
            if (vertex.IsDisposed)
            {
                throw new ScriptRuntimeException("Vertex is disposed, can't TryToMove()!");
            }

            if (vertex.Linedefs.Count <= 0)
            {
                vertex.Move(new Vector2D(x, y));
                vertex.SnapToAccuracy();
                return true;
            }
            
            if (LuaMap.NearestSector(position).sector.Index != LuaMap.NearestSector(new LuaVector2D(x, y)).sector.Index)
            {
                return false;
            }

            Vector2D oldPosition = vertex.Position;
            vertex.Move(new Vector2D(x, y));
            vertex.SnapToAccuracy();

            // create a bounding box for our vertex's lines
            float bbox_x1 = (float)General.Map.FormatInterface.MaxCoordinate;
            float bbox_x2 = (float)General.Map.FormatInterface.MinCoordinate;
            float bbox_y1 = (float)General.Map.FormatInterface.MaxCoordinate;
            float bbox_y2 = (float)General.Map.FormatInterface.MinCoordinate;
            foreach (Linedef l in vertex.Linedefs)
            {
                bbox_x1 = Math.Min(bbox_x1,
                        Math.Min(l.Start.Position.x, l.End.Position.x)
                    );
                bbox_x2 = Math.Max(bbox_x2,
                        Math.Max(l.Start.Position.x, l.End.Position.x)
                    );

                bbox_y1 = Math.Min(bbox_y1,
                        Math.Min(l.Start.Position.y, l.End.Position.y)
                    );
                bbox_y2 = Math.Max(bbox_y2,
                        Math.Max(l.Start.Position.y, l.End.Position.y)
                    );
            }

            // a little wiggle room for floating point imprecisions
            bbox_x1 -= 0.001f;
            bbox_x2 += 0.001f;
            bbox_y1 -= 0.001f;
            bbox_x2 += 0.001f;

            List<Linedef> map_test_lines = new List<Linedef>(General.Map.Map.Linedefs.Count / 3);

            // compare all the map's lines to the bounding boxes,
            // rejecting those that are outside
            foreach (Linedef l in General.Map.Map.Linedefs)
            {
                float startx = l.Start.Position.x;
                float starty = l.Start.Position.y;
                float endx = l.End.Position.x;
                float endy = l.End.Position.y;

                if (startx < bbox_x1 && endx < bbox_x1)
                {
                    continue;
                }

                if (startx > bbox_x2 && endx > bbox_x2)
                {
                    continue;
                }

                if (starty < bbox_y1 && endy < bbox_y1)
                {
                    continue;
                }

                if (starty > bbox_y2 && endy > bbox_y2)
                {
                    continue;
                }

                // dont test yourself or you'll wreck yourself
                bool is_one_of_ours = false;
                foreach (Linedef our_line in vertex.Linedefs)
                {
                    if (object.ReferenceEquals(our_line, l))
                    {
                        is_one_of_ours = true;
                        break; // just out of the inner loop
                    }
                }

                if (is_one_of_ours)
                {
                    continue;
                }

                map_test_lines.Add(l);
            }

            int line_count = map_test_lines.Count;

            foreach (Linedef our_line in vertex.Linedefs)
            {
                Line2D l2d = our_line.Line;

                for (int i = 0; i < line_count; i++)
                {
                    Linedef map_line = map_test_lines[i];
                    
                    float u_ray = 0f;

                    if (map_line.Line.GetIntersection(l2d, out u_ray))
                    {
                        // we need to check vertex ends too to make sure we dont get bad geometry
                        if (
                            (u_ray > 0.0 && u_ray < 1.0f && !float.IsNaN(u_ray))
                            || vertex.Position == map_line.Start.Position
                            || vertex.Position == map_line.End.Position
                            )
                        {
                            vertex.Move(oldPosition);
                            return false;
                        }
                    }

                } // for each map lines
            } // for each vertex's lines

            foreach (Linedef our_line in vertex.Linedefs)
            {
                our_line.NeedUpdate();
                our_line.UpdateCache();
            }

            return true;
        }

        // returns false if other is same vertex
        public bool SharesLine(LuaVertex other)
        {
            if (vertex.IsDisposed || other.vertex.IsDisposed)
            {
                throw new ScriptRuntimeException("Vertex has been disposed, can't SharesLine()");
            }
            if (other == null)
            {
                throw new ScriptRuntimeException("Other vertex is null, can't SharesLine() (not enough arguments maybe?)");
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

            Vector2D oldPosition = vertex.Position;

            vertex.SnapToGrid();

            if (oldPosition != vertex.Position)
            {
                foreach (Linedef l in vertex.Linedefs)
                {
                    l.NeedUpdate();
                    l.UpdateCache();
                }
            }
        }

        public void SnapToAccuracy()
        {
            if (vertex.IsDisposed)
            {
                throw new ScriptRuntimeException("Vertex has been disposed, can't SnapToAccuracy!");
            }

            Vector2D oldPosition = vertex.Position;

            vertex.SnapToAccuracy();

            if (oldPosition != vertex.Position)
            {
                foreach (Linedef l in vertex.Linedefs)
                {
                    l.NeedUpdate();
                    l.UpdateCache();
                }
            }
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
            if (other == null)
            {
                throw new ScriptRuntimeException("Other vertex is null, can't Join() (not enough arguments maybe?)");
            }
            vertex.Join(other.vertex);

            // Update cached values
            General.Map.Map.Update();
        }

        public void Dispose()
        {
            // Not already removed automatically?
            if (!vertex.IsDisposed)
            {
                // If the vertex only has 2 linedefs attached, then merge the linedefs
                if (vertex.Linedefs.Count == 2)
                {
                    Linedef ld1 = General.GetByIndex(vertex.Linedefs, 0);
                    Linedef ld2 = General.GetByIndex(vertex.Linedefs, 1);
                    Vertex v1 = (ld1.Start == vertex) ? ld1.End : ld1.Start;
                    Vertex v2 = (ld2.Start == vertex) ? ld2.End : ld2.Start;
                    if (ld1.Start == vertex) ld1.SetStartVertex(v2); else ld1.SetEndVertex(v2);
                    ld2.Dispose();
                }

                // Trash vertex
                vertex.Dispose();
            }
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
