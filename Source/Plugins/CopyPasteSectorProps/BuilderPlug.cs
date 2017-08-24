
#region ================== Copyright (c) 2009 Boris Iwanski

/*
 * Copyright (c) 2009 Boris Iwanski
 * This program is released under GNU General Public License
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 */

#endregion

#region ================== Namespaces

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Reflection;
using CodeImp.DoomBuilder.Windows;
using CodeImp.DoomBuilder.IO;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Rendering;
using CodeImp.DoomBuilder.Geometry;
using System.Drawing;
using CodeImp.DoomBuilder.Editing;
using CodeImp.DoomBuilder.Plugins;
using CodeImp.DoomBuilder.Actions;
using CodeImp.DoomBuilder.Types;
using CodeImp.DoomBuilder.Config;
using CodeImp.DoomBuilder.Data;

#endregion

namespace CodeImp.DoomBuilder.CopyPasteSectorProps
{
	//
	// MANDATORY: The plug!
	// This is an important class to the Doom Builder core. Every plugin must
	// have exactly 1 class that inherits from Plug. When the plugin is loaded,
	// this class is instantiated and used to receive events from the core.
	// Make sure the class is public, because only public classes can be seen
	// by the core.
	//

	public class BuilderPlug : Plug
	{
		// Static instance. We can't use a real static class, because BuilderPlug must
		// be instantiated by the core, so we keep a static reference. (this technique
		// should be familiar to object-oriented programmers)
		private static BuilderPlug me;

		// Static property to access the BuilderPlug
		public static BuilderPlug Me { get { return me; } }

        // These variables will store the properties we want to copy
        private int floorHeight;
        private int ceilHeight;
        private string floorTexture;
        private string ceilTexture;
        private int brightness;
        private int effect;
        private int tag;
		private UniFields fields;
		
        // This is set to true to know that we copied sector properties.
		// If this is false, the variables above are uninitialized.
        bool didCopyProps = false;

		// This event is called when the plugin is initialized
		public override void OnInitialize()
		{
			base.OnInitialize();

			// This binds the methods in this class that have the BeginAction
			// and EndAction attributes with their actions. Without this, the
			// attributes are useless. Note that in classes derived from EditMode
			// this is not needed, because they are bound automatically when the
			// editing mode is engaged.
            General.Actions.BindMethods(this);

			// Keep a static reference
            me = this;
		}

		// This is called when the plugin is terminated
		public override void Dispose()
		{
			base.Dispose();

			// This must be called to remove bound methods for actions.
            General.Actions.UnbindMethods(this);
        }

        #region ================== Actions

		// This is the method that will be called when the "copysectorprops" action is used by the user.
		// The actions are defined in the Actions.cfg file (the name must be exactly that) and the
		// BeginAction attribute indicates that this method must be called when the key for this
		// action is pressed. You can use the EndAction attribute for methods that must be called when
		// the key for an action is released. See above for the BindMethods method call which must be
		// called in order for these methods to work.
        [BeginAction("copysectorprops")]
        public void CopySectorProps()
        {
            // Only make this action possible if a map is actually opened and the we are in sectors mode
            if (General.Editing.Mode == null || General.Editing.Mode.Attributes.SwitchAction != "sectorsmode")
            {
                return;
            }

            // Make sure a sector is highlighted
            if (!(General.Editing.Mode.HighlightedObject is Sector))
            {
                // Show a warning in the status bar
                General.Interface.DisplayStatus(StatusType.Warning, "Please highlight a sector to copy the properties from");
                return;
            }

            // Copy the properties from the sector
            Sector s = (Sector)General.Editing.Mode.HighlightedObject;
            floorHeight = s.FloorHeight;
            ceilHeight = s.CeilHeight;
            floorTexture = s.FloorTexture;
            ceilTexture = s.CeilTexture;
            brightness = s.Brightness;
            effect = s.Effect;
            tag = s.Tag;

            // Remember that we copied the properties
            didCopyProps = true;

            // Let the user know that copying worked
            General.Interface.DisplayStatus(StatusType.Action, "Copied sector properties.");
        }

		// This is the method that will be called when the "pastesectorprops" action is used by the user.
		[BeginAction("pastesectorprops")]
        public void PasteSectorProps()
        {
            // Collection used to store the sectors we want to paste to
            List<Sector> sectors = new List<Sector>();

            // Just for fun we want to let the user know to how many sectors he/she/it pasted
            int pasteCount = 0;

            // Only make this action possible if a map is actually opened and we are in sectors mode
            if (General.Editing.Mode == null || General.Editing.Mode.Attributes.SwitchAction != "sectorsmode")
            {
                return;
            }

            // Make sure there's at least one selected or highlighted sector we can paste the properties to
            if (General.Map.Map.GetSelectedSectors(true).Count == 0 && !(General.Editing.Mode.HighlightedObject is Sector))
            {
                // Show a warning in the status bar
                General.Interface.DisplayStatus(StatusType.Warning, "Please select or highlight at least 1 sector to paste the properties to.");
                return;
            }

            // Check if there's actually a copied sector to get the properties from
            if (didCopyProps == false)
            {
                // Show a warning in the status bar
                General.Interface.DisplayStatus(StatusType.Warning, "Can't paste properties. You need to copy the properties first.");
                return;
            }

            // If there are selected sectors only paste to them
            if (General.Map.Map.GetSelectedSectors(true).Count != 0)
            {
				ICollection<Sector> selectedsectors = General.Map.Map.GetSelectedSectors(true);
                foreach (Sector s in selectedsectors)
                {
                    sectors.Add(s);
                    pasteCount++;
                }
            }
            // No selected sectors. paste to the highlighted one
            else
            {
                sectors.Add((Sector)General.Editing.Mode.HighlightedObject);
                pasteCount++;
            }

            // Make the action undo-able
            General.Map.UndoRedo.CreateUndo("Paste sector properties");

            // Set the properties of all selected sectors
            foreach(Sector s in sectors)
            {
                // Heights
                s.FloorHeight = floorHeight;
                s.CeilHeight = ceilHeight;

                // Textures
                s.SetFloorTexture(floorTexture);
                s.SetCeilTexture(ceilTexture);

                // Other stuff
                s.Brightness = brightness;
                s.Effect = effect;
                s.Tag = tag;
            }

            // Redraw to make the changes visible
            General.Map.Map.Update();
            General.Interface.RedrawDisplay();

            // Let the user know to how many sectors the properties were copied
            General.Interface.DisplayStatus(StatusType.Action, "Pasted sector properties to " + pasteCount.ToString() + " sector(s).");
        }

        #endregion
    }
}
