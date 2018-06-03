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
        internal Script script;
        internal float rendererscale;
        internal List<DrawnVertex> drawnVertices;
        internal Vector2D mappos;
        private StringBuilder scriptlog_sb;
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
            mappos = inmappos;
            rendererscale = renderer.Scale;
            drawnVertices = new List<DrawnVertex>();
            scriptlog_sb = new StringBuilder();

            UserData.RegisterAssembly();
            script = new Script(CoreModules.Preset_SoftSandbox);

            script.Globals["Vector2D"] = typeof(LuaVector2D);
            script.Globals["Map"] = typeof(LuaMap);

            script.Globals["LogLine"] = (Func<string,bool>)LogLine;
            script.Globals["DrawVertexAtCursor"] = (Func<int>)DrawVertexAtCursor;
            script.Globals["MoveCursorX"] = (Func<float, float>)MoveCursorX;
            script.Globals["MoveCursorY"] = (Func<float, float>)MoveCursorY;
            script.Globals["FinishPlacingVertices"] = (Func<bool>)FinishPlacingVertices;
            script.Globals["FinishPlacingVertices"] = (Func<bool>)FinishPlacingVertices;
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
                General.WriteLogLine(e.DecoratedMessage);
                return false;
            }
        }

        #region LuaFunctions
        protected bool LogLine(string line)
        {
            scriptlog_sb.Append(line);
            scriptlog_sb.Append('\n');
            return true;
        }

        protected int DrawVertexAtCursor()
        {
            drawnVertices.Add(GetVertexAt(mappos, true, true, 0.001f, false, drawnVertices));
            return 1;
        }

        protected float MoveCursorX(float dx)
        {
            mappos.x += dx;
            return mappos.x;
        }

        protected float MoveCursorY(float dy)
        {
            mappos.y += dy;
            return mappos.y;
        }

        protected bool FinishPlacingVertices()
        {
            return FinishDrawingPoints(drawnVertices);
        }
        #endregion

        // these aren't called by lua directly
        #region LuaHelperFunctions

        public bool FinishDrawingPoints(List<DrawnVertex> points)
        {
            // Make the drawing
            if (!Tools.DrawLines(points))
            {
                // FIXME TODO MOVE THIS INTO DOSCRIPTAT();
                // Drawing failed
                General.Map.UndoRedo.WithdrawUndo();
                return false;
            }
            return true;
        }

        // ano - negative stitchrange defaults to builderplug.me.stitchrange which comes from config file
        // most of this is codeimps code
        public DrawnVertex GetVertexAt(
            Vector2D mappos, bool snaptonearest, bool snaptogrid, float stitchrange, bool scaletorenderer,
            List<DrawnVertex> points = null)
        {
            DrawnVertex p = new DrawnVertex();
            Vector2D vm = mappos;

            float vrange = stitchrange;
            if (scaletorenderer)
            {
                vrange = stitchrange / rendererscale;
            }

            //float vrange = stitchrange;

            // Snap to nearest?
            if (snaptonearest)
            {
                if (points != null)
                {
                    // Go for all drawn points
                    foreach (DrawnVertex v in points)
                    {
                        if (Vector2D.DistanceSq(mappos, v.pos) < (vrange * vrange))
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
                    p.pos = nv.Position;
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
                            p.pos = found_coord;
                            p.stitch = true;
                            p.stitchline = true;
                            //Logger.WriteLogLine("c");//debugcrap
                            return p;
                        }
                    }
                    else
                    {
                        // Aligned to line
                        p.pos = nl.NearestOnLine(mappos);
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
                    if (Vector2D.DistanceSq(mappos, points[0].pos) < (vrange * vrange))
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
                Line2D dline = new Line2D(mappos, points[points.Count - 1].pos);
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
                        if (blines[i].GetSideOfLine(points[points.Count - 1].pos) != 0.0f)
                        {
                            foundintersection = blines[i].GetIntersection(dline, out u);
                        }
                    }
                }

                // if there was no intersection set the position to the last set point
                if (!foundintersection)
                    vm = points[points.Count - 1].pos;
                else
                    vm = dline.GetCoordinatesAt(u);

            }


            // Snap to grid?
            if (snaptogrid)
            {
                // Aligned to grid
                p.pos = General.Map.Grid.SnappedToGrid(vm);

                // special handling 
                if (p.pos.x > General.Map.Config.RightBoundary) p.pos.x = General.Map.Config.RightBoundary;
                if (p.pos.y < General.Map.Config.BottomBoundary) p.pos.y = General.Map.Config.BottomBoundary;
                p.stitch = snaptonearest;
                p.stitchline = snaptonearest;
                //Logger.WriteLogLine("f");//debugcrap
                return p;
            }
            else
            {
                // Normal position
                p.pos = vm;
                p.stitch = snaptonearest;
                p.stitchline = snaptonearest;
                //Logger.WriteLogLine("g");//debugcrap
                return p;
            }
        } // getcurrentposition
        #endregion
    }
}
