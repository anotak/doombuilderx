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
    [MoonSharpUserData]
    public class Pen
    {
        [MoonSharpHidden]
        public List<PenVertex> drawnVertices;

        public LuaVector2D position;

        public float angle;

        public bool snaptogrid;

        public float stitchrange;
        

        public Pen(LuaVector2D newpos)
        {
            position = newpos;
            drawnVertices = new List<PenVertex>();
            snaptogrid = true;
            stitchrange = 1f;
        }

        public static Pen FromClick()
        {
            return new Pen(new LuaVector2D(General.Map.Grid.SnappedToGrid(ScriptContext.context.mousemappos)));
        }

        public static Pen FromClickNoSnap()
        {
            return new Pen(new LuaVector2D(ScriptContext.context.mousemappos));
        }

        public static Pen From(float x, float y)
        {
            return new Pen(new LuaVector2D(x,y));
        }

        public static Pen From(LuaVector2D v)
        {
            return new Pen(v);
        }

        public void MoveForward(float distance)
        {
            position.vec += Vector2D.FromAngle(angle, distance);
        }

        public void MoveDiagonal(float dx, float dy)
        {
            MoveDiagonal(new LuaVector2D(dx, dy));
        }

        public void MoveDiagonal(LuaVector2D v)
        {
            position.vec += v.vec.GetRotated(angle);
        }

        public void TurnRight()
        {
            angle -= Angle2D.PIHALF;
        }

        public void TurnLeft()
        {
            angle += Angle2D.PIHALF;
        }

        public void TurnRight(float radians)
        {
            angle -= radians;
        }

        public void TurnLeft(float radians)
        {
            angle += radians;
        }

        public void TurnRightDegrees(float degrees)
        {
            angle += degrees / Angle2D.PIDEG;
        }

        public void TurnLeftDegrees(float degrees)
        {
            angle -= degrees / Angle2D.PIDEG;
        }

        public void SetAngleDegrees(float degrees)
        {
            angle = degrees / Angle2D.PIDEG;
        }

        public void MoveToFirst()
        {
            if (drawnVertices.Count > 0)
            {
                position = drawnVertices[0].pos;
            }
        }

        public void DrawVertex()
        {
            drawnVertices.Add(GetVertexAt(position.vec, true, snaptogrid, stitchrange, drawnVertices));
        }

        public void DrawVertex(bool snaptonearest)
        {
            drawnVertices.Add(GetVertexAt(position.vec, snaptonearest, snaptogrid, stitchrange, drawnVertices));
        }

        public void DrawVertex(bool snaptonearest, bool bsnaptogrid)
        {
            drawnVertices.Add(GetVertexAt(position.vec, snaptonearest, bsnaptogrid, stitchrange, drawnVertices));
        }

        public void DrawVertexAt(LuaVector2D newVec)
        {
            drawnVertices.Add(GetVertexAt(newVec.vec, true, snaptogrid, stitchrange, drawnVertices));
        }

        public void DrawVertexAt(LuaVector2D newVec, bool snaptonearest)
        {
            drawnVertices.Add(GetVertexAt(newVec.vec, snaptonearest, snaptogrid, stitchrange, drawnVertices));
        }

        public void DrawVertexAt(LuaVector2D newVec, bool snaptonearest, bool snaptogrid)
        {
            drawnVertices.Add(GetVertexAt(newVec.vec, snaptonearest, snaptogrid, stitchrange, drawnVertices));
        }

        public bool FinishPlacingVertices()
        {
            return FinishDrawingPoints(drawnVertices);
        }

        [MoonSharpHidden]
        public bool FinishDrawingPoints(List<PenVertex> points)
        {
            List<DrawnVertex> d = new List<DrawnVertex>(points.Count);
            foreach (PenVertex p in points)
            {
                DrawnVertex nd = new DrawnVertex();
                nd.pos = p.pos.vec;
                nd.stitch = p.stitch;
                nd.stitchline = p.stitchline;
                d.Add(nd);
            }

            // Make the drawing
            if (!Tools.DrawLines(d))
            {
                throw new ScriptRuntimeException("Unknown failure drawing pen vertices!");
            }
            return true;
        }

        // ano - negative stitchrange defaults to builderplug.me.stitchrange which comes from config file
        // most of this is codeimps code
        // FIXME PREVIOUS COMMENT IS WRONG BUT MAKE IT RIGHT ???
        [MoonSharpHidden]
        internal PenVertex GetVertexAt(
            Vector2D mappos, bool snaptonearest, bool snaptogrid, float stitchrange,
            List<PenVertex> points = null)
        {
            PenVertex p = new PenVertex();
            Vector2D vm = mappos;

            float vrange = stitchrange;

            //float vrange = stitchrange;

            // Snap to nearest?
            if (snaptonearest)
            {
                if (points != null)
                {
                    // Go for all drawn points
                    foreach (PenVertex v in points)
                    {
                        if (Vector2D.DistanceSq(mappos, v.pos.vec) < (vrange * vrange))
                        {
                            p.pos = v.pos;
                            p.stitch = true;
                            p.stitchline = true;
                            //Logger.WriteLogLine("a");//debugcrap
                            return p;
                        }
                    }
                }

                // Try the nearest vertex
                Vertex nv = General.Map.Map.NearestVertexSquareRange(mappos, vrange);
                if (nv != null)
                {
                    p.pos.vec = nv.Position;
                    p.stitch = true;
                    p.stitchline = true;
                    //Logger.WriteLogLine("b");//debugcrap
                    return p;
                }

                // Try the nearest linedef
                Linedef nl = General.Map.Map.NearestLinedefRange(mappos, vrange);
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
                            Vector2D delta = mappos - v;
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
                            p.pos.vec = found_coord;
                            p.stitch = true;
                            p.stitchline = true;
                            //Logger.WriteLogLine("c");//debugcrap
                            return p;
                        }
                    }
                    else
                    {
                        // Aligned to line
                        p.pos.vec = nl.NearestOnLine(mappos);
                        p.stitch = true;
                        p.stitchline = true;
                        //Logger.WriteLogLine("d");//debugcrap
                        return p;
                    }
                }
            }
            else
            {
                // Always snap to the first drawn vertex so that the user can finish a complete sector without stitching
                if (points != null && points.Count > 0)
                {
                    if (Vector2D.DistanceSq(mappos, points[0].pos.vec) < (vrange * vrange))
                    {
                        p.pos = points[0].pos;
                        p.stitch = true;
                        p.stitchline = false;
                        //Logger.WriteLogLine("e");//debugcrap
                        return p;
                    }
                }
            }

            // if the mouse cursor is outside the map bondaries check if the line between the last set point and the
            // mouse cursor intersect any of the boundary lines. If it does, set the position to this intersection
            if (points != null &&
                points.Count > 0 &&
                (mappos.x < General.Map.Config.LeftBoundary || mappos.x > General.Map.Config.RightBoundary ||
                mappos.y > General.Map.Config.TopBoundary || mappos.y < General.Map.Config.BottomBoundary))
            {
                Line2D dline = new Line2D(mappos, points[points.Count - 1].pos.vec);
                bool foundintersection = false;
                float u = 0.0f;
                List<Line2D> blines = new List<Line2D>();

                // lines for left, top, right and bottom bondaries
                blines.Add(new Line2D(General.Map.Config.LeftBoundary, General.Map.Config.BottomBoundary, General.Map.Config.LeftBoundary, General.Map.Config.TopBoundary));
                blines.Add(new Line2D(General.Map.Config.LeftBoundary, General.Map.Config.TopBoundary, General.Map.Config.RightBoundary, General.Map.Config.TopBoundary));
                blines.Add(new Line2D(General.Map.Config.RightBoundary, General.Map.Config.TopBoundary, General.Map.Config.RightBoundary, General.Map.Config.BottomBoundary));
                blines.Add(new Line2D(General.Map.Config.RightBoundary, General.Map.Config.BottomBoundary, General.Map.Config.LeftBoundary, General.Map.Config.BottomBoundary));

                // check for intersections with boundaries
                for (int i = 0; i < blines.Count; i++)
                {
                    if (!foundintersection)
                    {
                        // only check for intersection if the last set point is not on the
                        // line we are checking against
                        if (blines[i].GetSideOfLine(points[points.Count - 1].pos.vec) != 0.0f)
                        {
                            foundintersection = blines[i].GetIntersection(dline, out u);
                        }
                    }
                }

                // if there was no intersection set the position to the last set point
                if (!foundintersection)
                    vm = points[points.Count - 1].pos.vec;
                else
                    vm = dline.GetCoordinatesAt(u);

            }


            // Snap to grid?
            if (snaptogrid)
            {
                // Aligned to grid
                p.pos.vec = General.Map.Grid.SnappedToGrid(vm);

                // special handling 
                if (p.pos.vec.x > General.Map.Config.RightBoundary) p.pos.vec.x = General.Map.Config.RightBoundary;
                if (p.pos.vec.y < General.Map.Config.BottomBoundary) p.pos.vec.y = General.Map.Config.BottomBoundary;
                p.stitch = snaptonearest;
                p.stitchline = snaptonearest;
                //Logger.WriteLogLine("f");//debugcrap
                return p;
            }
            else
            {
                // Normal position
                p.pos.vec = vm;
                p.stitch = snaptonearest;
                p.stitchline = snaptonearest;
                //Logger.WriteLogLine("g");//debugcrap
                return p;
            }
        } // getcurrentposition

    } // pen
} // ns
