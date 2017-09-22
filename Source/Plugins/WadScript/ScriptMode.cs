using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CodeImp.DoomBuilder.Windows;
using CodeImp.DoomBuilder.IO;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Rendering;
using CodeImp.DoomBuilder.Geometry;
using CodeImp.DoomBuilder.Editing;
using CodeImp.DoomBuilder.Actions;
using CodeImp.DoomBuilder.Types;
using CodeImp.DoomBuilder.Config;

namespace CodeImp.DoomBuilder.WadScript
{
    [EditMode(DisplayName = "WadScript Mode",
              SwitchAction = "wadscriptmode",
              ButtonImage = "WadScriptIcon.png",
              ButtonOrder = 500,
              ButtonGroup = "002_tools",
              UseByDefault = true,
              AllowCopyPaste = false,
              Volatile = false)]
    public class ScriptMode : ClassicMode
    {
        // ano - this code borrowed from codeimp's statistics plug
        public override void OnEngage()
        {
            base.OnEngage();
            
            renderer.SetPresentation(Presentation.Standard);
        }

        // ano - this code borrowed from codeimp's statistics plug
        // Event called when the user (or the core) wants to cancel this editing mode
        public override void OnCancel()
        {
            base.OnCancel();

            General.Editing.ChangeMode(General.Editing.PreviousStableMode.Name);
        }

        // ano - this code borrowed from codeimp's statistics plug
        public override void OnRedrawDisplay()
        {
            base.OnRedrawDisplay();

            renderer.RedrawSurface();
            
            // StartPlotter() returns true when drawing is possible.
            if (renderer.StartPlotter(true))
            {
                renderer.PlotLinedefSet(General.Map.Map.Linedefs);
                renderer.PlotVerticesSet(General.Map.Map.Vertices);

                renderer.Finish();
            }


            if (renderer.StartThings(true))
            {
                renderer.RenderThingSet(General.Map.ThingsFilter.HiddenThings, Presentation.THINGS_HIDDEN_ALPHA);
                renderer.RenderThingSet(General.Map.ThingsFilter.VisibleThings, 1.0f);
                
                renderer.Finish();
            }

            if (renderer.StartOverlay(true))
            {
                renderer.Finish();
            }
            
            renderer.Present();
        }

        protected override void OnEditBegin()
        {
            Logger.WriteLogLine("oneditbegin called"); // debugcrap
            DoScript();
        }

        [BeginAction("insertitem", BaseAction = true)]
        public virtual void DoScript() { DoScriptAt(mousemappos, renderer.Scale); }

        public void DoScriptAt(Vector2D mappos, float rendererscale)
        {
            bool snaptogrid = General.Interface.ShiftState ^ General.Interface.SnapToGrid;
            bool snaptonearest = General.Interface.CtrlState ^ General.Interface.AutoMerge;

            if (General.Interface.MouseInDisplay)
            {
                // Make undo for the draw
                General.Map.UndoRedo.CreateUndo("Run script");

                General.Interface.SetCursor(Cursors.AppStarting);

                General.Interface.DisplayStatus(StatusType.Info, "Executing script!");
                
                /*
                List<Sector> oldsectors = new List<Sector>(General.Map.Map.Sectors);
                List<Sector> allnewsectors = new List<Sector>();
                */
                
                WadScriptVM vm = new WadScriptVM(this);

                vm.RunVM(snaptogrid,snaptonearest,mappos);

                /*
                int i = 1;
                foreach (Sector s in General.Map.Map.Sectors)
                {
                    if (!oldsectors.Contains(s))
                    {
                        allnewsectors.Add(s);
                        s.SetFloorTexture("FLAT" + i++);
                    }
                }
                */

                // Snap to map format accuracy
                General.Map.Map.SnapAllToAccuracy();

                // Clear selection
                General.Map.Map.ClearAllSelected();

                // Update cached values
                General.Map.Map.Update();

                // Update the used textures
                General.Map.Data.UpdateUsedTextures();
                
                // Map is changed
                General.Map.IsChanged = true;

                General.Interface.SetCursor(Cursors.Default);

                General.Interface.RedrawDisplay();

                General.Interface.DisplayStatus(StatusType.Info, "Execution done!");
            } 
            // add support for mouseless scripts 
        }

        public bool FinishDrawingPoints(List<DrawnVertex> points)
        {
            // Make the drawing
            if (!Tools.DrawLines(points))
            {
                // FIXME TODO MOVE THIS INTO DOSCRIPTAT();
                // Drawing failed
                General.Map.UndoRedo.WithdrawUndo();
                General.Interface.SetCursor(Cursors.Default);
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
                vrange = stitchrange / renderer.Scale;
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


        // Accepted
        public override void OnAccept()
        {

            General.Settings.FindDefaultDrawSettings();
            

            // FIXME DO VM

            // Done
            Cursor.Current = Cursors.Default;

            // Return to original mode
            General.Editing.ChangeMode(General.Editing.PreviousStableMode.Name);
        }


    } // class
} // ns
