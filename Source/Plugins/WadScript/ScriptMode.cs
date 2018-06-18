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
        internal static bool bScriptSuccess;
        internal static bool bScriptDone;
        internal static bool bScriptCancelled;
        internal static Stopwatch stopwatch;

        public ScriptMode()
            : base()
        {
            bScriptSuccess = true;
            bScriptDone = false;
            bScriptCancelled = false;
            scriptPath = General.Settings.ReadPluginSetting("selectedscriptpath", "");

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
            BuilderPlug.Me.ShowMenu();
            renderer.SetPresentation(Presentation.Things);
        }

        // ano - this code borrowed from codeimp's statistics plug
        // Event called when the user (or the core) wants to cancel this editing mode
        public override void OnCancel()
        {
            base.OnCancel();
            BuilderPlug.Me.HideMenu();
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
                renderer.RenderThingSet(General.Map.ThingsFilter.HiddenThings, 0.66f);
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
                General.Interface.DisplayStatus(StatusType.Info, "Selected script '" + Path.GetFileName(scriptPath) + "', have fun.");
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
        [BeginAction("runluascript")]
        public void DoScriptAtCenterOfScreen() { DoScriptAt(new Vector2D(0f, 0f)); }

        [BeginAction("insertitem", BaseAction = true)]
        public void DoScript() { DoScriptAt(mousemappos); }

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

            if (General.Interface.MouseInDisplay)
            {
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
                bScriptSuccess = true;
                bScriptDone = false;
                bScriptCancelled = false;

                scriptRunner = new ScriptContext(renderer, mousemappos, snaptogrid, snaptonearest);
                scriptThread = new Thread(new ThreadStart(RunScriptThread));
                scriptThread.Priority = ThreadPriority.Highest;
                scriptThread.Start();

                Thread.Sleep(4);
                int scriptTime = 4;

                while (!bScriptDone && !bScriptCancelled && scriptTime < 100)
                {
                    Thread.Sleep(4);
                    scriptTime += 4;
                }

                while (!bScriptDone && !bScriptCancelled && scriptTime < 3600)
                {
                    Thread.Sleep(10);
                    scriptTime += 10;
                }

                // if our script isnt done
                // let's ask the user if they wanna keep waiting
                if (!bScriptDone && !bScriptCancelled)
                {
                    General.Interface.SetCursor(Cursors.Default);
                    General.Interface.DisplayStatus(StatusType.Busy, "Executing script '" + scriptShortName + "', but it's being slow!");

                    ScriptTimeoutForm.ShowTimeout();
                }

                scriptThread.Join();

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
            } // if mouseindisplay
            // add support for mouseless scripts 
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
            

            // FIXME DO LUA

            // Done
            Cursor.Current = Cursors.Default;

            // Return to original mode
            General.Editing.ChangeMode(General.Editing.PreviousStableMode.Name);
        }


        

    } // class
} // ns
