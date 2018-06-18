﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

using CodeImp.DoomBuilder.Plugins;
using CodeImp.DoomBuilder.Windows;
using CodeImp.DoomBuilder.IO;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Rendering;
using CodeImp.DoomBuilder.Geometry;
using CodeImp.DoomBuilder.Editing;
using CodeImp.DoomBuilder.Types;
using CodeImp.DoomBuilder.Config;
using CodeImp.DoomBuilder.Data;
using CodeImp.DoomBuilder.Controls;

namespace CodeImp.DoomBuilder.DBXLua
{
    // ano - uses code from codeimp's stats plugin
    public class BuilderPlug : Plug
    {
        #region VARIABLES
        private static BuilderPlug me;
        private float stitchrange;
        internal ToolStripMenuItem menu;

        protected ToolStripMenuItem[] recent_menu;
        internal List<string> recent_files;
        #endregion

        #region PROPERTIES
        public static BuilderPlug Me { get { return me; } }
        public override string Name { get { return "Lua"; } }
        public float StitchRange { get { return stitchrange; } }
        #endregion


        #region METHODS
        private void LoadSettings()
        {
            // should we really have this as a separate setting? and not something set in lua
            stitchrange = (float)General.Settings.ReadPluginSetting("stitchrange", 20);

            // kinda hacky default value, but, yea
            string invalid_path = @"\[]{};:!@#$^&*()_invalid_path1234567890,./";

            // remember extra recent files in case some get moved or something
            for (int i = 0; i <= 12; i++)
            {
                string path = General.Settings.ReadPluginSetting("recentfiles" + i, invalid_path);
                if (path == invalid_path)
                {
                    break;
                }
                if (File.Exists(path))
                {
                    recent_files.Add(path);
                }
            }
        }

        // This event is called when the plugin is initialized
        public override void OnInitialize()
        {
            base.OnInitialize();

            // Keep a static reference
            me = this;

            recent_files = new List<string>();

            LoadSettings();

            InitializeMenu();
        }

        private void InitializeMenu()
        {
            menu = new ToolStripMenuItem("&Lua");
            menu.Visible = false;
            List<ToolStripItem> items = new List<ToolStripItem>();
            List<ToolStripMenuItem> recent_items = new List<ToolStripMenuItem>();

            {
                ToolStripMenuItem item = new ToolStripMenuItem("&Run current script");
                item.Tag = item.Name = "runluascript";
                item.Click += new System.EventHandler(this.InvokeTaggedAction);
                items.Add(item);
            }

            {
                ToolStripMenuItem item = new ToolStripMenuItem("&Open script");
                item.Tag = item.Name = "openluascript";
                item.Click += new System.EventHandler(this.InvokeTaggedAction);
                items.Add(item);
            }

            {
                ToolStripSeparator item = new ToolStripSeparator();
                items.Add(item);
            }

            {
                ToolStripMenuItem item = new ToolStripMenuItem("&1 recentfile");
                item.Tag = item.Name = "recentlua1";
                item.Click += new System.EventHandler(this.InvokeTaggedAction);
                items.Add(item);
                recent_items.Add(item);
            }

            {
                ToolStripMenuItem item = new ToolStripMenuItem("&2 recentfile");
                item.Tag = item.Name = "recentlua2";
                item.Click += new System.EventHandler(this.InvokeTaggedAction);
                items.Add(item);
                recent_items.Add(item);
            }

            {
                ToolStripMenuItem item = new ToolStripMenuItem("&3 recentfile");
                item.Tag = item.Name = "recentlua3";
                item.Click += new System.EventHandler(this.InvokeTaggedAction);
                items.Add(item);
                recent_items.Add(item);
            }

            {
                ToolStripMenuItem item = new ToolStripMenuItem("&4 recentfile");
                item.Tag = item.Name = "recentlua4";
                item.Click += new System.EventHandler(this.InvokeTaggedAction);
                items.Add(item);
                recent_items.Add(item);
            }

            {
                ToolStripMenuItem item = new ToolStripMenuItem("&5 recentfile");
                item.Tag = item.Name = "recentlua5";
                item.Click += new System.EventHandler(this.InvokeTaggedAction);
                items.Add(item);
                recent_items.Add(item);
            }

            {
                ToolStripMenuItem item = new ToolStripMenuItem("&6 recentfile");
                item.Tag = item.Name = "recentlua6";
                item.Click += new System.EventHandler(this.InvokeTaggedAction);
                items.Add(item);
                recent_items.Add(item);
            }

            {
                ToolStripMenuItem item = new ToolStripMenuItem("&7 recentfile");
                item.Tag = item.Name = "recentlua7";
                item.Click += new System.EventHandler(this.InvokeTaggedAction);
                items.Add(item);
                recent_items.Add(item);
            }

            {
                ToolStripMenuItem item = new ToolStripMenuItem("&8 recentfile");
                item.Tag = item.Name = "recentlua8";
                item.Click += new System.EventHandler(this.InvokeTaggedAction);
                items.Add(item);
                recent_items.Add(item);
            }

            {
                ToolStripMenuItem item = new ToolStripMenuItem("&9 recentfile");
                item.Tag = item.Name = "recentlua9";
                item.Click += new System.EventHandler(this.InvokeTaggedAction);
                items.Add(item);
                recent_items.Add(item);
            }

            menu.DropDownItems.AddRange(items.ToArray());
            recent_menu = recent_items.ToArray();
            General.Interface.AddMenu(menu);
        }

        public void ShowMenu()
        {
            menu.Visible = true;

            int files_count = recent_files.Count;
            for (int i = 0; i < recent_menu.Length; i++)
            {
                if (i < files_count)
                {
                    string shortname;

                    try
                    {
                        shortname = Path.GetFileName(recent_files[i]);
                    }
                    catch (Exception) // if there are invalid chars or something else
                    {
                        shortname = recent_files[i];
                        if (shortname.LastIndexOf('/') > -1)
                        {
                            shortname = shortname.Substring(shortname.LastIndexOf('/'));
                        }
                        if (shortname.LastIndexOf('\\') > -1)
                        {
                            shortname = shortname.Substring(shortname.LastIndexOf('\\'));
                        }
                    }

                    if (shortname.Length > 32)
                    {
                        shortname = shortname.Substring(0, 31) + "...";
                    }

                    recent_menu[i].Text = "&" + (i + 1) + " " + shortname;
                    recent_menu[i].Enabled = true;
                    recent_menu[i].Visible = true;
                }
                else
                {
                    recent_menu[i].Enabled = false;
                    recent_menu[i].Visible = false;
                }
            }
        }

        // add a recently opened file to the recent list
        public void AddOpened(string path)
        {
            int index = recent_files.IndexOf(path);

            if (index > -1)
            {
                recent_files.RemoveAt(index);
            }

            recent_files.Insert(0, path);

            ShowMenu();

            for (int i = 0; i < 12 && i < recent_files.Count; i++)
            {
                General.Settings.WritePluginSetting("recentfiles" + i, recent_files[i]);
            }
        }

        // remove from recently opened (in case of missing file for example)
        public void RemoveOpened(string path)
        {
            int index = recent_files.IndexOf(path);

            if (index > -1)
            {
                recent_files.RemoveAt(index);

                ShowMenu();

                for (int i = 0; i < 12 && i < recent_files.Count; i++)
                {
                    General.Settings.WritePluginSetting("recentfiles" + i, recent_files[i]);
                }
            }
        }

        public string GetRecentlyOpened(int index)
        {
            if (recent_files.Count > index)
            {
                return recent_files[index];
            }
            else
            {
                return "";
            }
        }

        public void HideMenu()
        {
            menu.Visible = false;
        }

        public override bool OnModeChange(EditMode oldmode, EditMode newmode)
        {
            if (oldmode is ScriptMode && !(newmode is ScriptMode))
            {
                HideMenu();
            }
            return base.OnModeChange(oldmode, newmode);
        }

        // This invokes an action from control event
        private void InvokeTaggedAction(object sender, EventArgs e)
        {
            General.Interface.InvokeTaggedAction(sender, e);
        }

        // This is called when the plugin is terminated
        public override void Dispose()
        {
            base.Dispose();
        }

        // When the Preferences dialog is closed
        public override void OnClosePreferences(PreferencesController controller)
        {
            base.OnClosePreferences(controller);

            // Apply settings that could have been changed
            LoadSettings();
        }
        #endregion
    }
}
