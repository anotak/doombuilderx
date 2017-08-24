
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
using System.Runtime.InteropServices;
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
using CodeImp.DoomBuilder.Types;
using CodeImp.DoomBuilder.Config;
using CodeImp.DoomBuilder.Data;
using CodeImp.DoomBuilder.Controls;

#endregion

namespace CodeImp.DoomBuilder.BuilderModes
{
	public class BuilderPlug : Plug
	{
		#region ================== API Declarations

		[DllImport("user32.dll")]
		internal static extern int GetWindowLong(IntPtr hWnd, int nIndex);
		
		#endregion
		
		#region ================== Constants

		internal const int WS_HSCROLL = 0x100000;
		internal const int WS_VSCROLL = 0x200000;
		internal const int GWL_STYLE = -16;
		
		#endregion

		#region ================== Variables

		// Static instance
		private static BuilderPlug me;
		
		// Main objects
		private MenusForm menusform;
		private CurveLinedefsForm curvelinedefsform;
		private FindReplaceForm findreplaceform;
		private ErrorCheckForm errorcheckform;
		private PreferencesForm preferencesform;
		
		// Dockers
		private UndoRedoPanel undoredopanel;
		private Docker undoredodocker;
		
		// Settings
		private int showvisualthings;			// 0 = none, 1 = sprite only, 2 = sprite caged
		private bool usegravity;
		private int changeheightbysidedef;		// 0 = nothing, 1 = change ceiling, 2 = change floor
		private int splitlinebehavior;			// 0 = adjust texcoords, 1 = copy texcoords, 2 = reset texcoords
		private bool editnewthing;
		private bool editnewsector;
		private bool additiveselect;
		private bool autoclearselection;
		private bool visualmodeclearselection;
		private string copiedtexture;
		private string copiedflat;
		private Point copiedoffsets;
		private VertexProperties copiedvertexprops;
		private SectorProperties copiedsectorprops;
		private SidedefProperties copiedsidedefprops;
		private LinedefProperties copiedlinedefprops;
		private ThingProperties copiedthingprops;
		private bool viewselectionnumbers;
		private float stitchrange;
		private float highlightrange;
		private float highlightthingsrange;
		private float splitlinedefsrange;
		private bool usehighlight;
		private bool autodragonpaste;
		
		#endregion

		#region ================== Properties
		
		public override string Name { get { return "Doom Builder"; } }
		public static BuilderPlug Me { get { return me; } }

		// It is only safe to do this dynamically because we compile and distribute both
		// the core and this plugin together with the same revision number! In third party
		// plugins this should just contain a fixed number.
		public override int MinimumRevision { get { return Assembly.GetExecutingAssembly().GetName().Version.Revision; } }
		
		public MenusForm MenusForm { get { return menusform; } }
		public CurveLinedefsForm CurveLinedefsForm { get { return curvelinedefsform; } }
		public FindReplaceForm FindReplaceForm { get { return findreplaceform; } }
		public ErrorCheckForm ErrorCheckForm { get { return errorcheckform; } }
		public PreferencesForm PreferencesForm { get { return preferencesform; } }

		// Settings
		public int ShowVisualThings { get { return showvisualthings; } set { showvisualthings = value; } }
		public bool UseGravity { get { return usegravity; } set { usegravity = value; } }
		public int ChangeHeightBySidedef { get { return changeheightbysidedef; } }
		public int SplitLineBehavior { get { return splitlinebehavior; } }
		public bool EditNewThing { get { return editnewthing; } }
		public bool EditNewSector { get { return editnewsector; } }
		public bool AdditiveSelect { get { return additiveselect; } }
		public bool AutoClearSelection { get { return autoclearselection; } }
		public bool VisualModeClearSelection { get { return visualmodeclearselection; } }
		public string CopiedTexture { get { return copiedtexture; } set { copiedtexture = value; } }
		public string CopiedFlat { get { return copiedflat; } set { copiedflat = value; } }
		public Point CopiedOffsets { get { return copiedoffsets; } set { copiedoffsets = value; } }
		public VertexProperties CopiedVertexProps { get { return copiedvertexprops; } set { copiedvertexprops = value; } }
		public SectorProperties CopiedSectorProps { get { return copiedsectorprops; } set { copiedsectorprops = value; } }
		public SidedefProperties CopiedSidedefProps { get { return copiedsidedefprops; } set { copiedsidedefprops = value; } }
		public LinedefProperties CopiedLinedefProps { get { return copiedlinedefprops; } set { copiedlinedefprops = value; } }
		public ThingProperties CopiedThingProps { get { return copiedthingprops; } set { copiedthingprops = value; } }
		public bool ViewSelectionNumbers { get { return viewselectionnumbers; } set { viewselectionnumbers = value; } }
		public float StitchRange { get { return stitchrange; } }
		public float HighlightRange { get { return highlightrange; } }
		public float HighlightThingsRange { get { return highlightthingsrange; } }
		public float SplitLinedefsRange { get { return splitlinedefsrange; } }
		public bool UseHighlight { get { return usehighlight; } set { usehighlight = value; } }
		public bool AutoDragOnPaste { get { return autodragonpaste; } set { autodragonpaste = value; } }
		
		#endregion

		#region ================== Initialize / Dispose

		// When plugin is initialized
		public override void OnInitialize()
		{
			// Setup
			me = this;

			// Settings
			showvisualthings = 2;
			usegravity = false;
			usehighlight = true;
			LoadSettings();
			
			// Load menus form and register it
			menusform = new MenusForm();
			menusform.Register();
			
			// Load curve linedefs form
			curvelinedefsform = new CurveLinedefsForm();
			
			// Load find/replace form
			findreplaceform = new FindReplaceForm();
			
			// Load error checking form
			errorcheckform = new ErrorCheckForm();
			
			// Load Undo\Redo docker
			undoredopanel = new UndoRedoPanel();
			undoredodocker = new Docker("undoredo", "Undo / Redo", undoredopanel);
			General.Interface.AddDocker(undoredodocker);
		}
		
		// Disposer
		public override void Dispose()
		{
			// Not already disposed?
			if(!IsDisposed)
			{
				// Clean up
				General.Interface.RemoveDocker(undoredodocker);
				undoredopanel.Dispose();
				menusform.Unregister();
				menusform.Dispose();
				menusform = null;
				curvelinedefsform.Dispose();
				curvelinedefsform = null;
				findreplaceform.Dispose();
				findreplaceform = null;
				errorcheckform.Dispose();
				errorcheckform = null;
				
				// Done
				me = null;
				base.Dispose();
			}
		}

		#endregion

		#region ================== Methods

		// This loads the plugin settings
		private void LoadSettings()
		{
			changeheightbysidedef = General.Settings.ReadPluginSetting("changeheightbysidedef", 0);
			splitlinebehavior = General.Settings.ReadPluginSetting("splitlinebehavior", 0);
			editnewthing = General.Settings.ReadPluginSetting("editnewthing", true);
			editnewsector = General.Settings.ReadPluginSetting("editnewsector", false);
			additiveselect = General.Settings.ReadPluginSetting("additiveselect", false);
			autoclearselection = General.Settings.ReadPluginSetting("autoclearselection", false);
			visualmodeclearselection = General.Settings.ReadPluginSetting("visualmodeclearselection", false);
			viewselectionnumbers = General.Settings.ReadPluginSetting("viewselectionnumbers", true);
			stitchrange = (float)General.Settings.ReadPluginSetting("stitchrange", 20);
			highlightrange = (float)General.Settings.ReadPluginSetting("highlightrange", 20);
			highlightthingsrange = (float)General.Settings.ReadPluginSetting("highlightthingsrange", 10);
			splitlinedefsrange = (float)General.Settings.ReadPluginSetting("splitlinedefsrange", 10);
			autodragonpaste = General.Settings.ReadPluginSetting("autodragonpaste", false);
		}

		#endregion

		#region ================== Events

		// When floor surface geometry is created for classic modes
		public override void OnSectorFloorSurfaceUpdate(Sector s, ref FlatVertex[] vertices)
		{
			ImageData img = General.Map.Data.GetFlatImage(s.LongFloorTexture);
			if((img != null) && img.IsImageLoaded)
			{
				// Make scalars
				float sw = 1.0f / img.ScaledWidth;
				float sh = 1.0f / img.ScaledHeight;
				
				// Make proper texture coordinates
				for(int i = 0; i < vertices.Length; i++)
				{
					vertices[i].u = vertices[i].u * sw;
					vertices[i].v = -vertices[i].v * sh;
				}
			}
		}

		// When ceiling surface geometry is created for classic modes
		public override void OnSectorCeilingSurfaceUpdate(Sector s, ref FlatVertex[] vertices)
		{
			ImageData img = General.Map.Data.GetFlatImage(s.LongCeilTexture);
			if((img != null) && img.IsImageLoaded)
			{
				// Make scalars
				float sw = 1.0f / img.ScaledWidth;
				float sh = 1.0f / img.ScaledHeight;

				// Make proper texture coordinates
				for(int i = 0; i < vertices.Length; i++)
				{
					vertices[i].u = vertices[i].u * sw;
					vertices[i].v = -vertices[i].v * sh;
				}
			}
		}

		// When the editing mode changes
		public override bool OnModeChange(EditMode oldmode, EditMode newmode)
		{
			// Show the correct menu for the new mode
			menusform.ShowEditingModeMenu(newmode);
			
			return base.OnModeChange(oldmode, newmode);
		}

		// When the Preferences dialog is shown
		public override void OnShowPreferences(PreferencesController controller)
		{
			base.OnShowPreferences(controller);

			// Load preferences
			preferencesform = new PreferencesForm();
			preferencesform.Setup(controller);
		}

		// When the Preferences dialog is closed
		public override void OnClosePreferences(PreferencesController controller)
		{
			base.OnClosePreferences(controller);

			// Apply settings that could have been changed
			LoadSettings();
			
			// Unload preferences
			preferencesform.Dispose();
			preferencesform = null;
		}
		
		// New map created
		public override void OnMapNewEnd()
		{
			base.OnMapNewEnd();
			undoredopanel.SetBeginDescription("New Map");
			undoredopanel.UpdateList();
		}
		
		// Map opened
		public override void OnMapOpenEnd()
		{
			base.OnMapOpenEnd();
			undoredopanel.SetBeginDescription("Opened Map");
			undoredopanel.UpdateList();
		}
		
		// Map closed
		public override void OnMapCloseEnd()
		{
			base.OnMapCloseEnd();
			undoredopanel.UpdateList();
		}
		
		// Redo performed
		public override void OnRedoEnd()
		{
			base.OnRedoEnd();
			undoredopanel.UpdateList();
		}
		
		// Undo performed
		public override void OnUndoEnd()
		{
			base.OnUndoEnd();
			undoredopanel.UpdateList();
		}
		
		// Undo created
		public override void OnUndoCreated()
		{
			base.OnUndoCreated();
			undoredopanel.UpdateList();
		}
		
		// Undo withdrawn
		public override void OnUndoWithdrawn()
		{
			base.OnUndoWithdrawn();
			undoredopanel.UpdateList();
		}
		
		#endregion
		
		#region ================== Tools

		// This adjusts texture coordinates for splitted lines according to the user preferences
		public void AdjustSplitCoordinates(Linedef oldline, Linedef newline)
		{
			// Copy X and Y coordinates
			if(splitlinebehavior == 1)
			{
				if((oldline.Front != null) && (newline.Front != null))
				{
					newline.Front.OffsetX = oldline.Front.OffsetX;
					newline.Front.OffsetY = oldline.Front.OffsetY;
				}
				
				if((oldline.Back != null) && (newline.Back != null))
				{
					newline.Back.OffsetX = oldline.Back.OffsetX;
					newline.Back.OffsetY = oldline.Back.OffsetY;
				}
			}
			// Reset X coordinate, copy Y coordinate
			else if(splitlinebehavior == 2)
			{
				if((oldline.Front != null) && (newline.Front != null))
				{
					newline.Front.OffsetX = 0;
					newline.Front.OffsetY = oldline.Front.OffsetY;
				}
				
				if((oldline.Back != null) && (newline.Back != null))
				{
					newline.Back.OffsetX = 0;
					newline.Back.OffsetY = oldline.Back.OffsetY;
				}
			}
		}
		
		// This finds all class types that inherits from the given type
		public Type[] FindClasses(Type t)
		{
			List<Type> found = new List<Type>();
			Type[] types;

			// Get all exported types
			types = Assembly.GetExecutingAssembly().GetTypes();
			foreach(Type it in types)
			{
				// Compare types
				if(t.IsAssignableFrom(it)) found.Add(it);
			}

			// Return list
			return found.ToArray();
		}
		
		// This renders the associated sectors/linedefs with the indication color
		public void PlotAssociations(IRenderer2D renderer, Association asso)
		{
			// Tag must be above zero
			if(asso.tag <= 0) return;
			
			// Sectors?
			if(asso.type == UniversalType.SectorTag)
			{
				foreach(Sector s in General.Map.Map.Sectors)
					if(s.Tag == asso.tag) renderer.PlotSector(s, General.Colors.Indication);
			}
			// Linedefs?
			else if(asso.type == UniversalType.LinedefTag)
			{
				foreach(Linedef l in General.Map.Map.Linedefs)
					if(l.Tag == asso.tag) renderer.PlotLinedef(l, General.Colors.Indication);
			}
            else if (General.Map.Config.UDMF && asso.type == UniversalType.PortalTag)
            {
                foreach (Linedef l in General.Map.Map.Linedefs)
                {
                    if (l.Fields.ContainsKey("portal")
                        && l.Fields["portal"].Type == (int)UniversalType.PortalTag
                        && Math.Abs((int)l.Fields["portal"].Value) == asso.tag)
                    {
                        renderer.PlotLinedef(l, General.Colors.Portals);
                    }
                }
                foreach (Sector s in General.Map.Map.Sectors)
                {
                    if ((s.Fields.ContainsKey("portalfloor")
                        && s.Fields["portalfloor"].Type == (int)UniversalType.PortalTag
                        && Math.Abs((int)s.Fields["portalfloor"].Value) == asso.tag)
                        ||
                        (s.Fields.ContainsKey("portalceiling")
                        && s.Fields["portalceiling"].Type == (int)UniversalType.PortalTag
                        && Math.Abs((int)s.Fields["portalceiling"].Value) == asso.tag))
                    {
                        renderer.PlotSector(s, General.Colors.Portals);
                    }
                }
            }
        } // plotassociations
		

		// This renders the associated things with the indication color
		public void RenderAssociations(IRenderer2D renderer, Association asso)
		{
			// Tag must be above zero
			if(asso.tag <= 0) return;

			// Things?
			if(asso.type == UniversalType.ThingTag)
			{
				foreach(Thing t in General.Map.Map.Things)
					if(t.Tag == asso.tag) renderer.RenderThing(t, General.Colors.Indication, 1.0f);
			}
		}
		

		// This renders the associated sectors/linedefs with the indication color
		public void PlotReverseAssociations(IRenderer2D renderer, Association asso)
		{
			// Tag must be above zero
			if(asso.tag <= 0) return;
			
			// Doom style referencing to sectors?
			if(General.Map.Config.LineTagIndicatesSectors && (asso.type == UniversalType.SectorTag))
			{
				// Linedefs
				foreach(Linedef l in General.Map.Map.Linedefs)
				{
					// Any action on this line?
					if(l.Action > 0)
					{
						if(l.Tag == asso.tag) renderer.PlotLinedef(l, General.Colors.Indication);
					}
				}
			}
			else
			{
				// Linedefs
				foreach(Linedef l in General.Map.Map.Linedefs)
				{
					// Known action on this line?
					if((l.Action > 0) && General.Map.Config.LinedefActions.ContainsKey(l.Action))
					{
						LinedefActionInfo action = General.Map.Config.LinedefActions[l.Action];
						if((action.Args[0].Type == (int)asso.type) && (l.Args[0] == asso.tag)) renderer.PlotLinedef(l, General.Colors.Indication);
						if((action.Args[1].Type == (int)asso.type) && (l.Args[1] == asso.tag)) renderer.PlotLinedef(l, General.Colors.Indication);
						if((action.Args[2].Type == (int)asso.type) && (l.Args[2] == asso.tag)) renderer.PlotLinedef(l, General.Colors.Indication);
						if((action.Args[3].Type == (int)asso.type) && (l.Args[3] == asso.tag)) renderer.PlotLinedef(l, General.Colors.Indication);
						if((action.Args[4].Type == (int)asso.type) && (l.Args[4] == asso.tag)) renderer.PlotLinedef(l, General.Colors.Indication);
					}
				}
			}
		}
		

		// This renders the associated things with the indication color
		public void RenderReverseAssociations(IRenderer2D renderer, Association asso)
		{
			// Tag must be above zero
			if(asso.tag <= 0) return;

			// Things
			foreach(Thing t in General.Map.Map.Things)
			{
				// Known action on this thing?
				if((t.Action > 0) && General.Map.Config.LinedefActions.ContainsKey(t.Action))
				{
					LinedefActionInfo action = General.Map.Config.LinedefActions[t.Action];
					if((action.Args[0].Type == (int)asso.type) && (t.Args[0] == asso.tag)) renderer.RenderThing(t, General.Colors.Indication, 1.0f);
					if((action.Args[1].Type == (int)asso.type) && (t.Args[1] == asso.tag)) renderer.RenderThing(t, General.Colors.Indication, 1.0f);
					if((action.Args[2].Type == (int)asso.type) && (t.Args[2] == asso.tag)) renderer.RenderThing(t, General.Colors.Indication, 1.0f);
					if((action.Args[3].Type == (int)asso.type) && (t.Args[3] == asso.tag)) renderer.RenderThing(t, General.Colors.Indication, 1.0f);
					if((action.Args[4].Type == (int)asso.type) && (t.Args[4] == asso.tag)) renderer.RenderThing(t, General.Colors.Indication, 1.0f);
				}
			}
		}

		#endregion
	}
}
