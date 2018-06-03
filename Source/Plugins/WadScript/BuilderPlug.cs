using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
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
        #endregion

        #region PROPERTIES
        public static BuilderPlug Me { get { return me; } }
        public override string Name { get { return "Lua"; } }
        public float StitchRange { get { return stitchrange; } }
        #endregion


        #region METHODS
        private void LoadSettings()
        {
            stitchrange = (float)General.Settings.ReadPluginSetting("stitchrange", 20);
        }

        // This event is called when the plugin is initialized
        public override void OnInitialize()
        {
            base.OnInitialize();

            // Keep a static reference
            me = this;

            LoadSettings();
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
