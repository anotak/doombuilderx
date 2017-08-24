
#region ================== Copyright (c) 2007 Pascal vd Heiden

/*
 * Copyright (c) 2007 Pascal vd Heiden, www.codeimp.com
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
using System.Globalization;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Reflection;
using System.Drawing;
using System.ComponentModel;
using CodeImp.DoomBuilder.Actions;
using CodeImp.DoomBuilder.Map;
using SlimDX.Direct3D9;
using SlimDX;
using CodeImp.DoomBuilder.Geometry;
using System.Drawing.Imaging;
using CodeImp.DoomBuilder.Data;
using CodeImp.DoomBuilder.Editing;
using CodeImp.DoomBuilder.Controls;

#endregion

namespace CodeImp.DoomBuilder.Windows
{
	public interface IMainForm : IWin32Window
	{
		// Properties
		bool AltState { get; }
		bool CtrlState { get; }
		bool ShiftState { get; }
		bool MouseInDisplay { get; }
		bool AutoMerge { get; }
		bool SnapToGrid { get; }
		bool MouseExclusive { get; }
		MouseButtons MouseButtons { get; }
		bool IsActiveWindow { get; }
		RenderTargetControl Display { get; }
		
		// Methods
		void DisplayReady();
		void DisplayStatus(StatusType type, string message);
		void DisplayStatus(StatusInfo newstatus);
		void RedrawDisplay();
		DialogResult ShowEditVertices(ICollection<Vertex> vertices);
		DialogResult ShowEditLinedefs(ICollection<Linedef> lines);
		DialogResult ShowEditSectors(ICollection<Sector> sectors);
		DialogResult ShowEditThings(ICollection<Thing> things);
		void ShowLinedefInfo(Linedef l);
		void ShowSectorInfo(Sector s);
		void ShowThingInfo(Thing t);
		void ShowVertexInfo(Vertex v);
		void HideInfo();
		void RefreshInfo();
		void UpdateCoordinates(Vector2D coords);
		bool Focus();
		void EnableProcessing();
		void DisableProcessing();
		void StartExclusiveMouseInput();
		void StopExclusiveMouseInput();
		void BreakExclusiveMouseInput();
		void ResumeExclusiveMouseInput();
		void SetCursor(Cursor cursor);
		void MessageBeep(MessageBeepType type);

		/// <summary>
		/// This moves the focus to the editing display.
		/// </summary>
		bool FocusDisplay();

		/// <summary>
		/// This browses the lindef types
		/// </summary>
		/// <returns>Returns the new action or the same action when cancelled</returns>
		int BrowseLinedefActions(IWin32Window owner, int initialvalue);
		
		/// <summary>
		/// This browses sector effects
		/// </summary>
		/// <returns>Returns the new effect or the same effect when cancelled</returns>
		int BrowseSectorEffect(IWin32Window owner, int initialvalue);

		/// <summary>
		/// This browses for a texture
		/// </summary>
		/// <returns>Returns the new texture name or the same texture name when cancelled</returns>
		string BrowseTexture(IWin32Window owner, string initialvalue);

		/// <summary>
		/// This browses for a flat
		/// </summary>
		/// <returns>Returns the new flat name or the same flat name when cancelled</returns>
		string BrowseFlat(IWin32Window owner, string initialvalue);

		/// <summary>
		/// THis browses for a thing type
		/// </summary>
		/// <returns>Returns the new thing type or the same thing type when cancelled</returns>
		int BrowseThingType(IWin32Window owner, int initialvalue);
		
		/// <summary>
		/// This adds a menu to the Doom Builder menu strip.
		/// <para>
		/// NOTE: When the Tag property of menu items is set with a string, this changes the
		/// tag to a fully qualified action name by prefixing it with the assembly name.
		/// </para>
		/// </summary>
		/// <param name="menu">The menu to add to Doom Builder.</param>
		void AddMenu(ToolStripMenuItem menu);
		
		/// <summary>
		/// This adds a menu or menu item to the Doom Builder menu strip in a specific location.
		/// <para>
		/// NOTE: When the Tag property of menu items is set with a string, this changes the
		/// tag to a fully qualified action name by prefixing it with the assembly name.
		/// </para>
		/// </summary>
		/// <param name="menu">The menu to add to Doom Builder.</param>
		/// <param name="section">The location where to insert the menu or item.</param>
		void AddMenu(ToolStripMenuItem menu, MenuSection section);
		
		/// <summary>
		/// This removes a menu from the Doom Builder menu strip.
		/// </summary>
		/// <param name="menu">The menu to remove.</param>
		void RemoveMenu(ToolStripMenuItem menu);
		
		/// <summary>
		/// This method invokes the action specified on the Tag property of the given menu item.
		/// </summary>
		/// <param name="sender">Menu item with Tag property set to the name of an action
		/// that you want to invoke.</param>
		/// <param name="e">Unused.</param>
		void InvokeTaggedAction(object sender, EventArgs e);
		
		/// <summary>
		/// This adds a custom button to the toolbar.
		/// </summary>
		void AddButton(ToolStripItem button);

		/// <summary>
		/// This adds a custom button to a specific section in the toolbar. Note that the visibility of the button will be controlled by the user's preferences of that section!
		/// </summary>
		void AddButton(ToolStripItem button, ToolbarSection section);

		/// <summary>
		/// This removes a custom button from the toolbar.
		/// </summary>
		void RemoveButton(ToolStripItem button);
		
		/// <summary>
		/// This adds a docker to the side panel.
		/// </summary>
		void AddDocker(Docker d);

        // gzdb cross compat
        void AddDocker(Docker d, bool notify);

        /// <summary>
        /// This removes a docker from the side panel.
        /// </summary>
        bool RemoveDocker(Docker d);
		
		/// <summary>
		/// Selects a docker in the side panel.
		/// </summary>
		bool SelectDocker(Docker d);
		
		/// <summary>
		/// This selected the previously selected docker in the side panel.
		/// </summary>
		void SelectPreviousDocker();

        // gzdb cross compat
        void BeginToolbarUpdate();

        // gzdb cross compat
        void EndToolbarUpdate();
    }
}
