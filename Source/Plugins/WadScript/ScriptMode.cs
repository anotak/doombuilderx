using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;

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
        public virtual void DoScript() { DoScriptAt(mousemappos); }

        public void DoScriptAt(Vector2D mappos)
        {
            bool snaptogrid = General.Interface.ShiftState ^ General.Interface.SnapToGrid;
            bool snaptonearest = General.Interface.CtrlState ^ General.Interface.AutoMerge;

            if (General.Interface.MouseInDisplay)
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();

                // Make undo for the draw
                General.Map.UndoRedo.CreateUndo("Run script");

                General.Interface.SetCursor(Cursors.AppStarting);

                General.Interface.DisplayStatus(StatusType.Info, "Executing script!");
                bool bScriptSuccess = true;
                string scriptPath = Path.Combine(General.SettingsPath, @"scripts\test.lua");
                
                ScriptContext scriptRunner = new ScriptContext(renderer, mousemappos);

                if (bScriptSuccess)
                {
                    bScriptSuccess = scriptRunner.RunScript(scriptPath);
                }

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
                    "Lua script success in "
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
                        "Lua script failed in "
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
                }
            } 
            // add support for mouseless scripts 
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
