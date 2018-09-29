using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Threading;

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
    public enum eCancelReason
    {
        Unknown,
        Timeout,
        UserChoice
    }

    [EditMode(DisplayName = "Lua Mode",
              SwitchAction = "dbxluamode",
              ButtonImage = "WadScriptIcon.png",
              ButtonOrder = 500,
              ButtonGroup = "002_tools",
              UseByDefault = true,
              AllowCopyPaste = false,
              Volatile = false)]
    public class ScriptMode : ClassicMode
    {
        internal static string scriptPath;
        internal static Thread scriptThread;
        internal static ScriptContext scriptRunner;
        internal static volatile bool bScriptSuccess;
        internal static volatile bool bScriptDone;
        internal static volatile eCancelReason CancelReason;
        internal static volatile bool bScriptCancelled;
        internal static volatile bool bScriptUIDone;
        internal static volatile bool bScriptParamsRequested;
        internal static Stopwatch stopwatch;
        protected static Dictionary<string, string> returned_parameters;

        internal SelectableElement highlighted;

        // ano - wish i could access the highlight range from the buildermodes plugin
        // but my life doesn't make sense anymore
        internal const float HIGHLIGHT_RANGE = 20f;
        internal const float HIGHLIGHT_THINGS_RANGE = 10f;
        internal const float HIGHLIGHT_VERTICES_RANGE = 10f;

        public ScriptMode()
            : base()
        {
            bScriptSuccess = true;
            bScriptDone = false;
            bScriptCancelled = false;
            bScriptParamsRequested = false;
            scriptPath = General.Settings.ReadPluginSetting("selectedscriptpath", "");
            CancelReason = eCancelReason.Unknown;
            highlighted = null;
            // load default hello script
            if (!File.Exists(scriptPath))
            {
                scriptPath = Path.Combine(General.AppPath, @"Lua\Examples\hello.lua");
            }
        }


        // ano - this code borrowed from codeimp's statistics plug
        public override void OnEngage()
        {
            base.OnEngage();
            ConvertSelection();
            BuilderPlug.Me.ShowMenu();
            renderer.SetPresentation(Presentation.Things);
        }

        public void ConvertSelection()
        {

            // this is a bit uglier than i'd like bc ConvertSelection does not
            // support SelectionType.All
            // and i'd like to keep plugin compat so... yea

            // FIXME just implement SelectionType.All instead because 
            // if your script selects a sector and theres another sector inside
            // of it, and then go to sectors mode, then both sectors will be
            // selected
            General.Map.Map.ConvertSelection(SelectionType.All);

            
        }

        // ano - this code borrowed from codeimp's statistics plug
        // Event called when the user (or the core) wants to cancel this editing mode
        public override void OnCancel()
        {
            base.OnCancel();
            BuilderPlug.Me.HideMenu();
            General.Editing.ChangeMode(General.Editing.PreviousStableMode.Name);
        }


        // only called from script thread!
        internal static Dictionary<string,string> RequestAndWaitForParams()
        {
            bScriptUIDone = false; // important to do this first bc threading crap
            bScriptParamsRequested = true;

            while (!bScriptUIDone)
            {
                Thread.Sleep(8);
            }
            return returned_parameters;
        }

        public void HighlightThing(Thing t)
        {
            if (highlighted == t)
            {
                return;
            }
            ClearHighlighted();

            // Set new highlight
            highlighted = t;

            if (highlighted == null || highlighted.IsDisposed)
            {
                highlighted = null;
                return;
            }
            
            if (renderer.StartThings(false))
            {
                renderer.RenderThing(t, General.Colors.Highlight, 1.0f);

                renderer.Finish();
                renderer.Present();
            }
        }

        public void HighlightVertex(Vertex v)
        {
            if (highlighted == v)
            {
                return;
            }

            ClearHighlighted();

            highlighted = v;

            if (highlighted == null || highlighted.IsDisposed)
            {
                highlighted = null;
                return;
            }

            if (renderer.StartPlotter(false))
            {
                renderer.PlotVertex(v, ColorCollection.HIGHLIGHT);

                renderer.Finish();
                renderer.Present();
            }
        }

        public void HighlightLinedef(Linedef l)
        {
            if (highlighted == l)
            {
                return;
            }

            ClearHighlighted();

            // Set new highlight
            highlighted = l;

            if (highlighted == null || highlighted.IsDisposed)
            {
                highlighted = null;
                return;
            }

            if (renderer.StartPlotter(false))
            {
                renderer.PlotLinedef(l, General.Colors.Highlight);
                renderer.PlotVertex(l.Start, renderer.DetermineVertexColor(l.Start));
                renderer.PlotVertex(l.End, renderer.DetermineVertexColor(l.End));

                renderer.Finish();
                renderer.Present();
            }
        }

        public void HighlightSector(Sector s)
        {
            if (highlighted == s)
            {
                return;
            }
            ClearHighlighted();

            // Set new highlight
            highlighted = s;

            if (highlighted == null || highlighted.IsDisposed)
            {
                highlighted = null;
                return;
            }

            if (renderer.StartPlotter(false))
            {
                renderer.PlotSector(s, General.Colors.Highlight);
                renderer.Finish();
                renderer.Present();
            }
        }

        // NOTE: must call renderer.Present() at some point after
        public void ClearHighlighted()
        {
            if (highlighted == null || highlighted.IsDisposed)
            {
                return;
            }

            // ano - i don't like this whole "highlighted is " way of doing things
            // but i don't see a better way rn
            if (highlighted is Linedef)
            {
                Linedef l = (Linedef)highlighted;

                if (renderer.StartPlotter(false))
                {

                    // Undraw previous highlight
                    if ((l != null) && !l.IsDisposed)
                    {
                        renderer.PlotLinedef(l, renderer.DetermineLinedefColor(l));
                        renderer.PlotVertex(l.Start, renderer.DetermineVertexColor(l.Start));
                        renderer.PlotVertex(l.End, renderer.DetermineVertexColor(l.End));
                    }

                    renderer.Finish();
                }
            }
            else if (highlighted is Thing)
            {
                Thing t = (Thing)highlighted;

                if (renderer.StartThings(false))
                {
                    renderer.RenderThing(t, renderer.DetermineThingColor(t), 1.0f);

                    renderer.Finish();
                }
            }
            else if (highlighted is Sector)
            {
                Sector s = (Sector)highlighted;

                if (renderer.StartPlotter(false))
                {
                    renderer.PlotSector(s);

                    renderer.Finish();
                }
            }
            else if (highlighted is Vertex)
            {
                Vertex v = (Vertex)highlighted;

                if (renderer.StartPlotter(false))
                {
                    renderer.PlotVertex(v, renderer.DetermineVertexColor(v));

                    renderer.Finish();
                }
            }
            else
            {
                Logger.WriteLogLine("LUA SCRIPTMODE UNKNOWN HIGHLIGHT CLEAR?? " + highlighted);
            }
            
            highlighted = null;
        }

        // ano - helper to make control flow simpler, only used in OnMouseMove
        enum HighlightType
        {
            Null,
            Vertex,
            Sector,
            Thing,
            Linedef
        }

        // Mouse moving
        public override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (selecting)
            {
                ClearHighlighted();
                RenderMultiSelection();
            }
            else
            {
                // ano - a lot of this is from codeimp's buildermodes plugins
                if (e.Button == MouseButtons.None)
                {
                    Linedef l = General.Map.Map.NearestLinedefRange(
                        mousemappos,
                        HIGHLIGHT_RANGE / renderer.Scale);

                    Thing t = MapSet.NearestThingSquareRange(
                        General.Map.ThingsFilter.VisibleThings,
                        mousemappos,
                        HIGHLIGHT_THINGS_RANGE / renderer.Scale);

                    Vertex v = General.Map.Map.NearestVertexSquareRange(
                        mousemappos,
                        HIGHLIGHT_VERTICES_RANGE / renderer.Scale);

                    Sector s = null;
                    Linedef ls = l;
                    if (ls == null)
                    {
                        ls = General.Map.Map.NearestLinedef(mousemappos);
                    }

                    if (ls != null)
                    {
                        // Check on which side of the linedef the mouse is
                        float side = ls.SideOfLine(mousemappos);

                        if (side > 0)
                        {
                            // Is there a sidedef here?
                            if (ls.Back != null)
                            {
                                s = ls.Back.Sector;
                            }
                        }
                        else if (ls.Front != null)
                        {
                            s = ls.Front.Sector;
                        }
                    }

                    HighlightType htype = HighlightType.Null;

                    if (v != null)
                    {
                        htype = HighlightType.Vertex;
                    }

                    
                    if (t != null)
                    {
                        if (htype == HighlightType.Null)
                        {
                            htype = HighlightType.Thing;
                        }
                        else if (htype == HighlightType.Vertex
                            && v.DistanceToSq(mousemappos) > t.DistanceToSq(mousemappos))
                        {
                            // figure out which is closer and highlight that one
                            htype = HighlightType.Thing;
                        }
                    }


                    if (l != null)
                    {
                        switch (htype)
                        {
                            case HighlightType.Vertex:
                                break;
                            case HighlightType.Thing:
                                if (l.SafeDistanceToSq(mousemappos, false) < t.DistanceToSq(mousemappos))
                                {
                                    // figure out which is closer and highlight that one
                                    htype = HighlightType.Linedef;
                                }
                                break;
                            default:
                                htype = HighlightType.Linedef;
                                break;
                        }
                    }

                    if (htype == HighlightType.Null && s != null)
                    {
                        htype = HighlightType.Sector;
                    }

                    switch (htype)
                    {
                        case HighlightType.Linedef:
                            HighlightLinedef(l);
                            break;
                        case HighlightType.Sector:
                            HighlightSector(s);
                            break;
                        case HighlightType.Vertex:
                            HighlightVertex(v);
                            break;
                        case HighlightType.Thing:
                            HighlightThing(t);
                            break;
                        case HighlightType.Null:
                        default:
                            // highlight nothing
                            ClearHighlighted();
                            renderer.Present();
                            break;
                    } // switch
                }

                if (renderer.StartOverlay(true))
                {
                    DrawCursor();

                    renderer.Finish();
                    renderer.Present();
                }
            }
        }

        public override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);

            ClearHighlighted();
        }

        // This is called when the selection box is dragged around
        protected override void OnUpdateMultiSelection()
        {
            base.OnUpdateMultiSelection();

            // Render selection
            if (renderer.StartOverlay(true))
            {
                RenderMultiSelection();
                renderer.Finish();
                renderer.Present();
            }
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

                if (highlighted != null && !highlighted.IsDisposed)
                {
                    if (highlighted is Linedef)
                    {
                        renderer.PlotLinedef((Linedef)highlighted, General.Colors.Highlight);
                    }
                    else if(highlighted is Sector)
                    {
                        renderer.PlotSector((Sector)highlighted, General.Colors.Highlight);
                    }
                }

                renderer.Finish();
            }


            if (renderer.StartThings(true))
            {
                renderer.RenderThingSet(General.Map.ThingsFilter.HiddenThings, 0.66f);
                renderer.RenderThingSet(General.Map.ThingsFilter.VisibleThings, 1.0f);
                if (highlighted != null
                    && !highlighted.IsDisposed
                    && highlighted is Thing)
                {
                    renderer.RenderThing((Thing)highlighted, General.Colors.Highlight, 1.0f);
                }
                renderer.Finish();
            }

            if (renderer.StartOverlay(true))
            {
                // if multiselecting draw the box
                if (selecting)
                {
                    RenderMultiSelection();
                }
                else
                {
                    DrawCursor();
                }

                renderer.Finish();
            }
            
            renderer.Present();
        }

        protected void DrawCursor()
        {
            if(mousemappos.IsFinite())
            {
                bool snaptogrid = General.Interface.ShiftState ^ General.Interface.SnapToGrid;
                bool snaptonearest = General.Interface.CtrlState ^ General.Interface.AutoMerge;
                PixelColor color = General.Colors.Highlight;
                color.a = 128;
                Vector2D position = GetCurrentPosition(mousemappos, snaptogrid, snaptogrid, renderer);
                renderer.RenderLine(position - new Vector2D(6f, 0f), position + new Vector2D(6f, 0f), 0.8f, color, true);
                renderer.RenderLine(position - new Vector2D(0f, 6f), position + new Vector2D(0, 6f), 0.8f, color, true);
            }
        }

        // from CodeImp's DrawGeometryMode
        public static Vector2D GetCurrentPosition(Vector2D mousemappos, bool snaptonearest, bool snaptogrid, IRenderer2D renderer)
        {
            Vector2D output = new Vector2D();
            Vector2D vm = mousemappos;
            float vrange = BuilderPlug.Me.StitchRange / renderer.Scale;

            // Snap to nearest?
            if (snaptonearest)
            {

                // Try the nearest vertex
                Vertex nv = General.Map.Map.NearestVertexSquareRange(mousemappos, vrange);
                if (nv != null)
                {
                    output = nv.Position;
                    return output;
                }

                // Try the nearest linedef
                Linedef nl = General.Map.Map.NearestLinedefRange(mousemappos, BuilderPlug.Me.StitchRange / renderer.Scale);
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
                            output = found_coord;
                            return output;
                        }
                    }
                    else
                    {
                        // Aligned to line
                        output = nl.NearestOnLine(mousemappos);
                        return output;
                    }
                }
            }

            
            // Snap to grid?
            if (snaptogrid)
            {
                // Aligned to grid
                output = General.Map.Grid.SnappedToGrid(vm);

                // special handling 
                if (output.x > General.Map.Config.RightBoundary) output.x = General.Map.Config.RightBoundary;
                if (output.y < General.Map.Config.BottomBoundary) output.y = General.Map.Config.BottomBoundary;
                return output;
            }
            else
            {
                // Normal position
                output = vm;
                return output;
            }
        }

        protected override void OnEditBegin()
        {
            Logger.WriteLogLine("oneditbegin called"); // debugcrap
            DoScript();
        }

        // note - this is indirectly called by lua
        internal static void SelectSector(Sector sector, bool selected)
        {
            if (selected == sector.Selected)
            {
                return;
            }
            if (selected == true)
            {
                sector.Selected = true;

                // "simple" in case of truth
                foreach (Sidedef side in sector.Sidedefs)
                {
                    side.Line.Selected = true;
                    side.Line.Start.Selected = true;
                    side.Line.End.Selected = true;
                }
            }
            else
            {
                // false case less trivial, lines must remain selected if
                // their other side is selected, and vertices must
                // remain selected if they are connected to outside selected lines
                sector.Selected = false;

                HashSet<Vertex> deselect_vertices = new HashSet<Vertex>();
                foreach (Sidedef side in sector.Sidedefs)
                {
                    // skip disposed ones, of course
                    if (side.IsDisposed)
                    {
                        continue;
                    }
                    Linedef l = side.Line;

                    if (l.Back == null || l.Back.IsDisposed)
                    {
                        l.Selected = false;
                    }
                    else if (l.Front == null || l.Front.IsDisposed)
                    {
                        // this case probably shouldn't be possible but
                        // i want lua to be more careful about doing things
                        // that can cause crashes
                        l.Selected = false;
                    }
                    else
                    {
                        // 2sided
                        l.Selected = l.Front.Sector.Selected && l.Back.Sector.Selected;

                        deselect_vertices.Add(l.Start);
                        deselect_vertices.Add(l.End);
                    }
                }

                // we have to handle vertices in a second loop once the lines
                // have already all been deselected if needed
                foreach (Vertex v in deselect_vertices)
                {
                    v.Selected = false;
                    foreach (Linedef l in v.Linedefs)
                    {
                        if (l.Selected)
                        {
                            v.Selected = true;
                            break;
                        }
                    }
                    // done w this vertex
                }
            } // done with true case
        }

        // note - this is indirectly called by lua
        internal static void SelectVertex(Vertex vertex, bool selected)
        {
            if (selected == vertex.Selected)
            {
                return;
            }

            vertex.Selected = selected;

            if (selected)
            {
                foreach (Linedef l in vertex.Linedefs)
                {
                    if (!l.Selected)
                    {
                        if (vertex == l.Start && l.End.Selected)
                        {
                            l.Selected = true;
                        }
                        else if (vertex == l.End && l.Start.Selected)
                        {
                            l.Selected = true;
                        }
                    }
                }
            }
            else
            {
                foreach (Linedef l in vertex.Linedefs)
                {
                    l.Selected = false;
                }
            }
        }

        // note - this is indirectly called by lua
        internal static void SelectLinedef(Linedef linedef, bool selected)
        {
            if (selected == linedef.Selected)
            {
                return;
            }

            if (selected)
            {
                linedef.Start.Selected = true;
                linedef.End.Selected = true;
                linedef.Selected = true;

                if (linedef.Front != null
                    && !linedef.Front.IsDisposed
                    && linedef.Front.Sector != null
                    && !linedef.Front.Sector.IsDisposed)
                {
                    bool select_sector = true;
                    foreach (Sidedef side in linedef.Front.Sector.Sidedefs)
                    {
                        if (!side.Line.Selected)
                        {
                            select_sector = false;
                            break;
                        }
                    }

                    linedef.Front.Sector.Selected = select_sector;
                }

                if (linedef.Back != null
                    && !linedef.Back.IsDisposed
                    && linedef.Back.Sector != null
                    && !linedef.Back.Sector.IsDisposed)
                {
                    bool select_sector = true;
                    foreach (Sidedef side in linedef.Back.Sector.Sidedefs)
                    {
                        if (!side.Line.Selected)
                        {
                            select_sector = false;
                            break;
                        }
                    }

                    linedef.Back.Sector.Selected = select_sector;
                }

            } // done w true case
            else
            {
                // if we're deselecting the line, it's more complex, because
                // each vertex may still be attached to another selected line
                linedef.Selected = false;
                bool result = false;
                foreach (Linedef l in linedef.Start.Linedefs)
                {
                    result &= l.Selected;
                }

                linedef.Start.Selected = result;

                result = false;
                foreach (Linedef l in linedef.End.Linedefs)
                {
                    result &= l.Selected;
                }

                linedef.End.Selected = result;

                if (linedef.Front != null)
                {
                    linedef.Front.Sector.Selected = false;
                }
                if (linedef.Back != null)
                {
                    linedef.Back.Sector.Selected = false;
                }
            } // done w false case
        }

        protected override void OnSelectBegin()
        {
            // FIXME does this handle that annoying doubleclick case?
            if (highlighted != null && !highlighted.IsDisposed)
            {
                if (highlighted is Sector)
                {
                    SelectSector((Sector)highlighted, !highlighted.Selected);
                }
                else if (highlighted is Linedef)
                {
                    SelectLinedef((Linedef)highlighted, !highlighted.Selected);
                }
                else if (highlighted is Vertex)
                {
                    SelectVertex((Vertex)highlighted, !highlighted.Selected);
                }
                else
                {
                    highlighted.Selected = !highlighted.Selected;
                }
            }
            else
            {
                // start drag select
                StartMultiSelection();
            }

            base.OnSelectBegin();
        }

        protected override void OnEndMultiSelection()
        {
            bool selectionvolume = ((Math.Abs(base.selectionrect.Width) > 0.1f) && (Math.Abs(base.selectionrect.Height) > 0.1f));

            if (selectionvolume)
            {
                if (General.Interface.AltState)
                {
                    // ano - subtractive select!
                    foreach (Thing t in General.Map.ThingsFilter.VisibleThings)
                    {
                        if ((t.Position.x >= selectionrect.Left) &&
                                       (t.Position.y >= selectionrect.Top) &&
                                       (t.Position.x <= selectionrect.Right) &&
                                       (t.Position.y <= selectionrect.Bottom))
                        {
                            t.Selected = false;
                        }
                    }
                    // Go for all vertices
                    foreach (Vertex v in General.Map.Map.Vertices)
                    {
                        if ((v.Position.x >= selectionrect.Left) &&
                            (v.Position.y >= selectionrect.Top) &&
                            (v.Position.x <= selectionrect.Right) &&
                            (v.Position.y <= selectionrect.Bottom))
                        {
                            SelectVertex(v, false);
                        }
                    }
                }
                else if (General.Interface.ShiftState)
                {
                    // additive
                    // FIXME figure out a way to make this respect buildermodes selection setting
                    // Go for all things
                    foreach (Thing t in General.Map.ThingsFilter.VisibleThings)
                    {
                        t.Selected |= ((t.Position.x >= selectionrect.Left) &&
                                       (t.Position.y >= selectionrect.Top) &&
                                       (t.Position.x <= selectionrect.Right) &&
                                       (t.Position.y <= selectionrect.Bottom));
                    }
                    // Go for all vertices
                    foreach (Vertex v in General.Map.Map.Vertices)
                    {
                        if ((v.Position.x >= selectionrect.Left) &&
                            (v.Position.y >= selectionrect.Top) &&
                            (v.Position.x <= selectionrect.Right) &&
                            (v.Position.y <= selectionrect.Bottom))
                        {
                            SelectVertex(v, true);
                        }
                    }
                }
                else
                {
                    // Go for all things
                    foreach (Thing t in General.Map.ThingsFilter.VisibleThings)
                    {
                        t.Selected = ((t.Position.x >= selectionrect.Left) &&
                                      (t.Position.y >= selectionrect.Top) &&
                                      (t.Position.x <= selectionrect.Right) &&
                                      (t.Position.y <= selectionrect.Bottom));
                    }
                    // Go for all vertices
                    foreach (Vertex v in General.Map.Map.Vertices)
                    {
                        SelectVertex(v, (v.Position.x >= selectionrect.Left) &&
                            (v.Position.y >= selectionrect.Top) &&
                            (v.Position.x <= selectionrect.Right) &&
                            (v.Position.y <= selectionrect.Bottom));
                    }
                }
            }

            // Go for all sectors
            foreach (Sector s in General.Map.Map.Sectors)
            {
                // Go for all sidedefs
                bool allselected = true;

                foreach (Sidedef sd in s.Sidedefs)
                {
                    if (!sd.Line.Selected)
                    {
                        allselected = false;
                        break;
                    }
                }

                s.Selected = allselected;
            }

            base.OnEndMultiSelection();
            

            // Redraw
            General.Interface.RedrawDisplay();
        }

        // pop up a new file browsing dialog window and ask the user what script to run
        [BeginAction("openluascript")]
        public void ChooseScript()
        {
            OpenFileDialog openfile = new OpenFileDialog();
            string initial_directory = General.Settings.ReadPluginSetting("initialdirectory", "");
            if (!Directory.Exists(initial_directory))
            {
                openfile.InitialDirectory = Path.Combine(General.AppPath, @"Lua");
            }
            if (Directory.Exists(initial_directory))
            {
                openfile.InitialDirectory = initial_directory;
            }
            openfile.Filter = "Lua files (*.lua)|*.lua";
            openfile.Title = "Open Lua Script";
            openfile.AddExtension = true;
            openfile.CheckFileExists = true;
            openfile.Multiselect = false;
            openfile.ValidateNames = true;

            if (openfile.ShowDialog(General.Interface) == DialogResult.OK)
            {
                General.Settings.WritePluginSetting("initialdirectory", Path.GetDirectoryName(openfile.FileName));
                SetCurrentScript(openfile.FileName);
            }
        }

        public void SetCurrentScript(string path)
        {
            if (File.Exists(path))
            {
                scriptPath = path;
                General.Settings.WritePluginSetting("selectedscriptpath", scriptPath);
                BuilderPlug.Me.AddOpened(path);
                General.Interface.DisplayStatus(StatusType.Info, "Selected script '" + Path.GetFileName(scriptPath) + "'.");
            }
            else
            {
                General.Interface.DisplayStatus(StatusType.Warning, "Can't find script! :(");
                BuilderPlug.Me.RemoveOpened(path);
                General.ShowErrorMessage("The Lua script file " + path + " does not exist.", MessageBoxButtons.OK);
            }
        }

        [BeginAction("recentlua1")]
        public void RecentLua1() { SetCurrentScript(BuilderPlug.Me.GetRecentlyOpened(0)); }
        [BeginAction("recentlua2")]
        public void RecentLua2() { SetCurrentScript(BuilderPlug.Me.GetRecentlyOpened(1)); }
        [BeginAction("recentlua3")]
        public void RecentLua3() { SetCurrentScript(BuilderPlug.Me.GetRecentlyOpened(2)); }
        [BeginAction("recentlua4")]
        public void RecentLua4() { SetCurrentScript(BuilderPlug.Me.GetRecentlyOpened(3)); }
        [BeginAction("recentlua5")]
        public void RecentLua5() { SetCurrentScript(BuilderPlug.Me.GetRecentlyOpened(4)); }
        [BeginAction("recentlua6")]
        public void RecentLua6() { SetCurrentScript(BuilderPlug.Me.GetRecentlyOpened(5)); }
        [BeginAction("recentlua7")]
        public void RecentLua7() { SetCurrentScript(BuilderPlug.Me.GetRecentlyOpened(6)); }
        [BeginAction("recentlua8")]
        public void RecentLua8() { SetCurrentScript(BuilderPlug.Me.GetRecentlyOpened(7)); }
        [BeginAction("recentlua9")]
        public void RecentLua9() { SetCurrentScript(BuilderPlug.Me.GetRecentlyOpened(8)); }
        
        // FIXME THIS IS CLEARLY NOT THE CENTER OF THE SCREEN
        // maybe we should prompt the user to explain if a script
        // asks for mouse position with an option to either type
        // in a number or to just cancel and go back and click?
        // consider making the mouse related lua api functions warn in this situation?
        [BeginAction("runluascript")]
        public void DoScriptAtCenterOfScreen() { DoScriptAt(new Vector2D(0f, 0f)); }

        [BeginAction("insertitem", BaseAction = true)]
        public void DoScript()
        {
            if (General.Interface.MouseInDisplay)
            {
                DoScriptAt(mousemappos);
            }
        }

        public void DoScriptAt(Vector2D mappos)
        {
            if (scriptPath == "")
            {
                General.Interface.DisplayStatus(StatusType.Warning,
                   "No Lua file selected.");
                // TODO - think about if this is really the correct way to react
                ChooseScript();
                return;
            }
            if (!File.Exists(scriptPath))
            {
                General.Interface.DisplayStatus(StatusType.Warning,
                    "Can't find Lua file '" + scriptPath + "'");
                ChooseScript();
                return;
            }

            bool snaptogrid = General.Interface.ShiftState ^ General.Interface.SnapToGrid;
            bool snaptonearest = General.Interface.CtrlState ^ General.Interface.AutoMerge;
            
            stopwatch = new Stopwatch();
            stopwatch.Start();

            //string scriptPath = Path.Combine(General.SettingsPath, @"scripts\test.lua");
            string scriptShortName = Path.GetFileName(scriptPath);

            // Make undo for the draw
            General.Map.UndoRedo.CreateUndo("Run script '" + scriptShortName + "'");
                
            string title = "";
            if (General.Interface is MainForm)
            {
                title = ((MainForm)General.Interface).Text;
                ((MainForm)General.Interface).Text = "Running Lua...";
            }

            General.Interface.SetCursor(Cursors.AppStarting);

            General.Interface.DisplayStatus(StatusType.Busy, "Executing script '" + scriptShortName + "'!");
            CancelReason = eCancelReason.Unknown;
            bScriptParamsRequested = false;
            bScriptSuccess = true;
            bScriptDone = false;
            bScriptCancelled = false;

            scriptRunner = new ScriptContext(renderer, mappos, snaptogrid, snaptonearest);
            scriptThread = new Thread(new ThreadStart(RunScriptThread));
            scriptThread.Priority = ThreadPriority.Highest;
            scriptThread.Start();

            Thread.Sleep(4);
            int scriptTime = 4;

            while (!bScriptDone && !bScriptCancelled && scriptTime < 2800)
            {
                Thread.Sleep(10);
                scriptTime += 10;

                if (bScriptParamsRequested)
                {
                    bScriptParamsRequested = false;
                    stopwatch.Stop();
                    returned_parameters = ScriptParamForm.ShowParamsDialog(scriptRunner.ui_parameters);
                    stopwatch.Start();
                    bScriptUIDone = true;
                }
            }

            General.Map.IsMapBeingEdited = true;
            // if our script isnt done
            // let's ask the user if they wanna keep waiting
            if (!bScriptDone && !bScriptCancelled)
            {
                General.Interface.SetCursor(Cursors.Default);
                General.Interface.DisplayStatus(StatusType.Busy, "Executing script '" + scriptShortName + "', but it's being slow.");

                ScriptTimeoutForm.ShowTimeout();
            }

            scriptThread.Join();
            General.Map.IsMapBeingEdited = false;

            General.Map.ThingsFilter.Update();

            // Snap to map format accuracy
            General.Map.Map.SnapAllToAccuracy();
                
            // Update cached values
            General.Map.Map.Update();

            // Update the used textures
            General.Map.Data.UpdateUsedTextures();
                
            // Map is changed
            General.Map.IsChanged = true;

            General.Interface.SetCursor(Cursors.Default);

            General.Interface.RedrawDisplay();

            stopwatch.Stop();

            // check for warnings
            if (bScriptSuccess)
            {
                string warningsText = scriptRunner.GetWarnings();
                if (warningsText.Length > 0)
                {
                    string debugLog = scriptRunner.DebugLog;
                    if (debugLog.Length > 0)
                    {
                        warningsText += "\nSCRIPT DEBUG LOG:\n" + debugLog;
                    }
                    warningsText += debugLog;

                    if (ScriptWarningForm.AskUndo(warningsText))
                    {
                        bScriptSuccess = false;
                    }
                }
            }

            // actual success
            if(bScriptSuccess)
            {
                General.Interface.DisplayStatus(StatusType.Info,
                    "Lua script  '" + scriptShortName + "' success in "
                    + (stopwatch.Elapsed.TotalMilliseconds / 1000d).ToString("########0.00")
                    + " seconds.");

                string scriptLog = scriptRunner.ScriptLog;
                if (scriptLog.Length > 0)
                {
                    ScriptMessageForm.ShowMessage(scriptLog);
                }
            }
            else
            {
                // okay failure
                General.Map.UndoRedo.WithdrawUndo();

                General.Interface.DisplayStatus(StatusType.Warning,
                    "Lua script '" + scriptShortName + "' failed in "
                    + (stopwatch.Elapsed.TotalMilliseconds / 1000d).ToString("########0.00")
                    + " seconds.");

                string errorText = scriptRunner.errorText;

                string warnings = scriptRunner.GetWarnings();

                if (warnings.Length > 0)
                {
                    errorText += "\nSCRIPT WARNING:\n" + warnings;
                }

                string scriptLog = scriptRunner.ScriptLog;
                if (scriptLog.Length > 0)
                {
                    errorText += "\nSCRIPT LOG:\n" + scriptLog;
                }

                string debugLog = scriptRunner.DebugLog;
                if (debugLog.Length > 0)
                {
                    errorText += "\nSCRIPT DEBUG LOG:\n" + debugLog;
                }

                if (errorText.Length > 0)
                {
                    ScriptErrorForm.ShowError(errorText);
                }
                else
                {
                    ScriptErrorForm.ShowError("unable to produce error message. possibly a big problem");
                }
            } // else error

            if (General.Interface is MainForm)
            {
                ((MainForm)General.Interface).Text = title;
            }
        }

        public static void RunScriptThread()
        {
            bScriptSuccess = scriptRunner.RunScript(scriptPath);
            bScriptDone = true;
        }

        // Accepted
        public override void OnAccept()
        {

            General.Settings.FindDefaultDrawSettings();
            

            // FIXME DO LUA (or do we?)
            // need to fully figure out how this function works

            // Done
            Cursor.Current = Cursors.Default;

            // Return to original mode
            General.Editing.ChangeMode(General.Editing.PreviousStableMode.Name);
        }


        // based on CodeImp code
        // This clears the selection
        [BeginAction("clearselection", BaseAction = true)]
        public void ClearSelection()
        {
            // Clear selection
            General.Map.Map.ClearAllSelected();

            // Redraw
            General.Interface.RedrawDisplay();
        }
    } // class
} // ns
