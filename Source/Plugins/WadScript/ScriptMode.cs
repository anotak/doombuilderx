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

        public ScriptMode()
            : base()
        {
            bScriptSuccess = true;
            bScriptDone = false;
            bScriptCancelled = false;
            bScriptParamsRequested = false;
            scriptPath = General.Settings.ReadPluginSetting("selectedscriptpath", "");
            CancelReason = eCancelReason.Unknown;
            // load default hello script
            if (!File.Exists(scriptPath))
            {
                scriptPath = Path.Combine(General.AppPath, @"Lua\hello.lua");
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
            General.Map.Map.ConvertSelection(SelectionType.Vertices);

            HashSet<Sector> potentially_selected_sectors = new HashSet<Sector>();
            foreach (Linedef l in General.Map.Map.Linedefs)
            {
                l.Selected = l.Start.Selected && l.End.Selected;

                if (l.Front != null)
                {
                    potentially_selected_sectors.Add(l.Front.Sector);
                }

                if (l.Back != null)
                {
                    potentially_selected_sectors.Add(l.Front.Sector);
                }
            }

            foreach (Sector s in potentially_selected_sectors)
            {
                bool select_sector = true;
                foreach (Sidedef side in s.Sidedefs)
                {
                    if (!side.Line.Selected)
                    {
                        select_sector = false;
                        break;
                    }
                }

                s.Selected = select_sector;
            }
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

        // Mouse moving
        public override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (renderer.StartOverlay(true))
            {
                DrawCursor();

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

                renderer.Finish();
            }


            if (renderer.StartThings(true))
            {
                renderer.RenderThingSet(General.Map.ThingsFilter.HiddenThings, 0.66f);
                renderer.RenderThingSet(General.Map.ThingsFilter.VisibleThings, 1.0f);
                
                renderer.Finish();
            }

            if (renderer.StartOverlay(true))
            {
                DrawCursor();

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

        // pop up a new file browsing dialog window and ask the user what script to run
        [BeginAction("openluascript")]
        public void ChooseScript()
        {
            OpenFileDialog openfile = new OpenFileDialog();
            string initial_directory = General.Settings.ReadPluginSetting("initialdirectory", "");
            if (!Directory.Exists(initial_directory))
            {
                Path.Combine(General.AppPath, @"Lua");
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
                General.Interface.DisplayStatus(StatusType.Busy, "Executing script '" + scriptShortName + "', but it's being slow!");

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
