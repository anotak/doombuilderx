
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
using System.Drawing;
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
using CodeImp.DoomBuilder.Editing;
using CodeImp.DoomBuilder.Actions;
using CodeImp.DoomBuilder.VisualModes;
using CodeImp.DoomBuilder.Config;

#endregion

namespace CodeImp.DoomBuilder.GZDoomEditing
{
	[EditMode(DisplayName = "GZDoom Visual Mode",
			  SwitchAction = "gzdoomvisualmode", // Action name used to switch to this mode
			  ButtonImage = "VisualModeZ.png",	// Image resource name for the button
			  ButtonOrder = 1,					// Position of the button (lower is more to the left)
			  ButtonGroup = "001_visual")]

	public class BaseVisualMode : VisualMode
	{
		#region ================== Constants
		
		// Object picking
		private const double PICK_INTERVAL = 80.0d;
		private const float PICK_RANGE = 0.98f;

		// Gravity
		private const float GRAVITY = -0.06f;
		
		#endregion
		
		#region ================== Variables

		// Gravity
		private Vector3D gravity;
		private float cameraflooroffset = 41f;		// same as in doom
		private float cameraceilingoffset = 10f;
		
		// Object picking
		private VisualPickResult target;
		private double lastpicktime;
		private bool locktarget;

		// This keeps extra element info
		private Dictionary<Sector, SectorData> sectordata;
		private Dictionary<Thing , ThingData> thingdata;
		
		// This is true when a selection was made because the action is performed
		// on an object that was not selected. In this case the previous selection
		// is cleared and the targeted object is temporarely selected to perform
		// the action on. After the action is completed, the object is deselected.
		private bool singleselection;
		
		// We keep these to determine if we need to make a new undo level
		private bool selectionchanged;
		private int lastundogroup;
		private VisualActionResult actionresult;
		private bool undocreated;

		// List of selected objects when an action is performed
		private List<IVisualEventReceiver> selectedobjects;
		
		#endregion
		
		#region ================== Properties

		public override object HighlightedObject
		{
			get
			{
				// Geometry picked?
				if(target.picked is VisualGeometry)
				{
					VisualGeometry pickedgeo = (target.picked as VisualGeometry);

					if(pickedgeo.Sidedef != null)
						return pickedgeo.Sidedef;
					else if(pickedgeo.Sector != null)
						return pickedgeo.Sector;
					else
						return null;
				}
				// Thing picked?
				else if(target.picked is VisualThing)
				{
					VisualThing pickedthing = (target.picked as VisualThing);
					return pickedthing.Thing;
				}
				else
				{
					return null;
				}
			}
		}

		public IRenderer3D Renderer { get { return renderer; } }
		
		public bool IsSingleSelection { get { return singleselection; } }
		public bool SelectionChanged { get { return selectionchanged; } set { selectionchanged |= value; } }

		#endregion
		
		#region ================== Constructor / Disposer

		// Constructor
		public BaseVisualMode()
		{
			// Initialize
			this.gravity = new Vector3D(0.0f, 0.0f, 0.0f);
			this.selectedobjects = new List<IVisualEventReceiver>();
			
			// We have no destructor
			GC.SuppressFinalize(this);
		}

		// Disposer
		public override void Dispose()
		{
			// Not already disposed?
			if(!isdisposed)
			{
				// Clean up
				
				// Done
				base.Dispose();
			}
		}

		#endregion
		
		#region ================== Methods

		// This calculates brightness level
		internal int CalculateBrightness(int level)
		{
			return renderer.CalculateBrightness(level);
		}
		
		// This adds a selected object
		internal void AddSelectedObject(IVisualEventReceiver obj)
		{
			selectedobjects.Add(obj);
			selectionchanged = true;
		}
		
		// This removes a selected object
		internal void RemoveSelectedObject(IVisualEventReceiver obj)
		{
			selectedobjects.Remove(obj);
			selectionchanged = true;
		}
		
		// This is called before an action is performed
		public void PreAction(int multiselectionundogroup)
		{
			actionresult = new VisualActionResult();
			
			PickTargetUnlocked();
			
			// If the action is not performed on a selected object, clear the
			// current selection and make a temporary selection for the target.
			if((target.picked != null) && !target.picked.Selected && (BuilderPlug.Me.VisualModeClearSelection || (selectedobjects.Count == 0)))
			{
				// Single object, no selection
				singleselection = true;
				ClearSelection();
				undocreated = false;
			}
			else
			{
				singleselection = false;
				
				// Check if we should make a new undo level
				// We don't want to do this if this is the same action with the same
				// selection and the action wants to group the undo levels
				if((lastundogroup != multiselectionundogroup) || (lastundogroup == UndoGroup.None) ||
				   (multiselectionundogroup == UndoGroup.None) || selectionchanged)
				{
					// We want to create a new undo level, but not just yet
					lastundogroup = multiselectionundogroup;
					undocreated = false;
				}
				else
				{
					// We don't want to make a new undo level (changes will be combined)
					undocreated = true;
				}
			}
		}

		// Called before an action is performed. This does not make an undo level
		private void PreActionNoChange()
		{
			actionresult = new VisualActionResult();
			singleselection = false;
			undocreated = false;
		}
		
		// This is called after an action is performed
		private void PostAction()
		{
			if(!string.IsNullOrEmpty(actionresult.displaystatus))
				General.Interface.DisplayStatus(StatusType.Action, actionresult.displaystatus);

			// Reset changed flags
			foreach(KeyValuePair<Sector, VisualSector> vs in allsectors)
			{
				BaseVisualSector bvs = (vs.Value as BaseVisualSector);
				foreach(VisualFloor vf in bvs.ExtraFloors) vf.Changed = false;
				foreach(VisualCeiling vc in bvs.ExtraCeilings) vc.Changed = false;
				bvs.Floor.Changed = false;
				bvs.Ceiling.Changed = false;
			}
			
			selectionchanged = false;
			
			if(singleselection)
				ClearSelection();
			
			UpdateChangedObjects();
			ShowTargetInfo();
		}
		
		// This sets the result for an action
		public void SetActionResult(VisualActionResult result)
		{
			actionresult = result;
		}

		// This sets the result for an action
		public void SetActionResult(string displaystatus)
		{
			actionresult = new VisualActionResult();
			actionresult.displaystatus = displaystatus;
		}
		
		// This creates an undo, when only a single selection is made
		// When a multi-selection is made, the undo is created by the PreAction function
		public int CreateUndo(string description, int group, int grouptag)
		{
			if(!undocreated)
			{
				undocreated = true;

				if(singleselection)
					return General.Map.UndoRedo.CreateUndo(description, this, group, grouptag);
				else
					return General.Map.UndoRedo.CreateUndo(description, this, UndoGroup.None, 0);
			}
			else
			{
				return 0;
			}
		}

		// This creates an undo, when only a single selection is made
		// When a multi-selection is made, the undo is created by the PreAction function
		public int CreateUndo(string description)
		{
			return CreateUndo(description, UndoGroup.None, 0);
		}

		// This makes a list of the selected object
		private void RebuildSelectedObjectsList()
		{
			// Make list of selected objects
			selectedobjects = new List<IVisualEventReceiver>();
			foreach(KeyValuePair<Sector, VisualSector> vs in allsectors)
			{
				if(vs.Value != null)
				{
					BaseVisualSector bvs = (BaseVisualSector)vs.Value;
					if((bvs.Floor != null) && bvs.Floor.Selected) selectedobjects.Add(bvs.Floor);
					if((bvs.Ceiling != null) && bvs.Ceiling.Selected) selectedobjects.Add(bvs.Ceiling);
					foreach(Sidedef sd in vs.Key.Sidedefs)
					{
						List<VisualGeometry> sidedefgeos = bvs.GetSidedefGeometry(sd);
						foreach(VisualGeometry sdg in sidedefgeos)
						{
							if(sdg.Selected) selectedobjects.Add((sdg as IVisualEventReceiver));
						}
					}
				}
			}

			foreach(KeyValuePair<Thing, VisualThing> vt in allthings)
			{
				if(vt.Value != null)
				{
					BaseVisualThing bvt = (BaseVisualThing)vt.Value;
					if(bvt.Selected) selectedobjects.Add(bvt);
				}
			}
		}

		// This creates a visual sector
		protected override VisualSector CreateVisualSector(Sector s)
		{
			BaseVisualSector vs = new BaseVisualSector(this, s);
			return vs;
		}
		
		// This creates a visual thing
		protected override VisualThing CreateVisualThing(Thing t)
		{
			BaseVisualThing vt = new BaseVisualThing(this, t);
			return vt.Setup() ? vt : null;
		}

		// This locks the target so that it isn't changed until unlocked
		public void LockTarget()
		{
			locktarget = true;
		}
		
		// This unlocks the target so that is changes to the aimed geometry again
		public void UnlockTarget()
		{
			locktarget = false;
		}
		
		// This picks a new target, if not locked
		private void PickTargetUnlocked()
		{
			if(!locktarget) PickTarget();
		}
		
		// This picks a new target
		private void PickTarget()
		{
			// Find the object we are aiming at
			Vector3D start = General.Map.VisualCamera.Position;
			Vector3D delta = General.Map.VisualCamera.Target - General.Map.VisualCamera.Position;
			delta = delta.GetFixedLength(General.Settings.ViewDistance * PICK_RANGE);
			VisualPickResult newtarget = PickObject(start, start + delta);
			
			// Should we update the info on panels?
			bool updateinfo = (newtarget.picked != target.picked);
			
			// Apply new target
			target = newtarget;

			// Show target info
			if(updateinfo) ShowTargetInfo();
		}

		// This shows the picked target information
		public void ShowTargetInfo()
		{
			// Any result?
			if(target.picked != null)
			{
				// Geometry picked?
				if(target.picked is VisualGeometry)
				{
					VisualGeometry pickedgeo = (target.picked as VisualGeometry);
					
					// Sidedef?
					if(pickedgeo is BaseVisualGeometrySidedef)
					{
						BaseVisualGeometrySidedef pickedsidedef = (pickedgeo as BaseVisualGeometrySidedef);
						General.Interface.ShowLinedefInfo(pickedsidedef.Sidedef.Line);
					}
					// Sector?
					else if(pickedgeo is BaseVisualGeometrySector)
					{
						BaseVisualGeometrySector pickedsector = (pickedgeo as BaseVisualGeometrySector);
						General.Interface.ShowSectorInfo(pickedsector.Level.sector);
					}
					else
					{
						General.Interface.HideInfo();
					}
				}
				// Thing picked?
				if(target.picked is VisualThing)
				{
					VisualThing pickedthing = (target.picked as VisualThing);
					General.Interface.ShowThingInfo(pickedthing.Thing);
				}
			}
			else
			{
				General.Interface.HideInfo();
			}
		}
		
		// This updates the VisualSectors and VisualThings that have their Changed property set
		private void UpdateChangedObjects()
		{
			foreach(KeyValuePair<Sector, VisualSector> vs in allsectors)
			{
				if(vs.Value != null)
				{
					BaseVisualSector bvs = (BaseVisualSector)vs.Value;
					if(bvs.Changed) bvs.Rebuild();
				}
			}

			foreach(KeyValuePair<Thing, VisualThing> vt in allthings)
			{
				if(vt.Value != null)
				{
					BaseVisualThing bvt = (BaseVisualThing)vt.Value;
					if(bvt.Changed) bvt.Rebuild();
				}
			}
		}
		
		#endregion

		#region ================== Extended Methods

		// This requests a sector's extra data
		internal SectorData GetSectorData(Sector s)
		{
			// Make fresh sector data when it doesn't exist yet
			if(!sectordata.ContainsKey(s))
				sectordata[s] = new SectorData(this, s);
			
			return sectordata[s];
		}
		
		// This requests a things's extra data
		internal ThingData GetThingData(Thing t)
		{
			// Make fresh sector data when it doesn't exist yet
			if(!thingdata.ContainsKey(t))
				thingdata[t] = new ThingData(this, t);
			
			return thingdata[t];
		}
		
		// This rebuilds the sector data
		// This requires that the blockmap is up-to-date!
		internal void RebuildElementData()
		{
			Dictionary<int, List<Sector>> sectortags = new Dictionary<int, List<Sector>>();
			sectordata = new Dictionary<Sector, SectorData>(General.Map.Map.Sectors.Count);
			thingdata = new Dictionary<Thing,ThingData>(General.Map.Map.Things.Count);
			
			// Find all sector who's tag is not 0 and hash them so that we can find them quicly
			foreach(Sector s in General.Map.Map.Sectors)
			{
				if(s.Tag != 0)
				{
					if(!sectortags.ContainsKey(s.Tag)) sectortags[s.Tag] = new List<Sector>();
					sectortags[s.Tag].Add(s);
				}
			}

			// Find sectors with 3 vertices, because they can be sloped
			foreach(Sector s in General.Map.Map.Sectors)
			{
				// ========== Thing vertex slope ==========
				if(s.Sidedefs.Count == 3)
				{
					List<Thing> slopeceilingthings = new List<Thing>(3);
					List<Thing> slopefloorthings = new List<Thing>(3);
					foreach(Sidedef sd in s.Sidedefs)
					{
						Vertex v;
						if(sd.IsFront)
							v = sd.Line.End;
						else
							v = sd.Line.Start;

						// Check if a thing is at this vertex
						VisualBlockEntry b = blockmap.GetBlock(blockmap.GetBlockCoordinates(v.Position));
						foreach(Thing t in b.Things)
						{
							if((Vector2D)t.Position == v.Position)
							{
								if(t.Type == 1504)
									slopefloorthings.Add(t);
								else if(t.Type == 1505)
									slopeceilingthings.Add(t);
							}
						}
					}

					// Slope any floor vertices?
					if(slopefloorthings.Count > 0)
					{
						SectorData sd = GetSectorData(s);
						sd.AddEffectThingVertexSlope(slopefloorthings, true);
					}

					// Slope any ceiling vertices?
					if(slopeceilingthings.Count > 0)
					{
						SectorData sd = GetSectorData(s);
						sd.AddEffectThingVertexSlope(slopeceilingthings, false);
					}
				}
			}
			
			// Find interesting linedefs (such as line slopes)
			foreach(Linedef l in General.Map.Map.Linedefs)
			{
				// ========== Plane Align (see http://zdoom.org/wiki/Plane_Align) ==========
				if(l.Action == 181)
				{
					// Slope front
					if(((l.Args[0] == 1) || (l.Args[1] == 1)) && (l.Front != null))
					{
						SectorData sd = GetSectorData(l.Front.Sector);
						sd.AddEffectLineSlope(l);
					}
					
					// Slope back
					if(((l.Args[0] == 2) || (l.Args[1] == 2)) && (l.Back != null))
					{
						SectorData sd = GetSectorData(l.Back.Sector);
						sd.AddEffectLineSlope(l);
					}
				}
				// ========== Sector 3D floor (see http://zdoom.org/wiki/Sector_Set3dFloor) ==========
				else if((l.Action == 160) && (l.Front != null))
				{
					int sectortag = l.Args[0] + (l.Args[4] << 8);
					if(sectortags.ContainsKey(sectortag))
					{
						List<Sector> sectors = sectortags[sectortag];
						foreach(Sector s in sectors)
						{
							SectorData sd = GetSectorData(s);
							sd.AddEffect3DFloor(l);
						}
					}
				}
				// ========== Transfer Brightness (see http://zdoom.org/wiki/ExtraFloor_LightOnly) =========
				else if((l.Action == 50) && (l.Front != null))
				{
					if(sectortags.ContainsKey(l.Args[0]))
					{
						List<Sector> sectors = sectortags[l.Args[0]];
						foreach(Sector s in sectors)
						{
							SectorData sd = GetSectorData(s);
							sd.AddEffectBrightnessLevel(l);
						}
					}
				}
			}

			// Find interesting things (such as sector slopes)
			foreach(Thing t in General.Map.Map.Things)
			{
				// ========== Copy slope ==========
				if((t.Type == 9510) || (t.Type == 9511))
				{
					t.DetermineSector(blockmap);
					if(t.Sector != null)
					{
						SectorData sd = GetSectorData(t.Sector);
						sd.AddEffectCopySlope(t);
					}
				}
				// ========== Thing line slope ==========
				else if((t.Type == 9500) || (t.Type == 9501))
				{
					t.DetermineSector(blockmap);
					if(t.Sector != null)
					{
						SectorData sd = GetSectorData(t.Sector);
						sd.AddEffectThingLineSlope(t);
					}
				}
			}
		}
		
		#endregion

		#region ================== Events

		// Help!
		public override void OnHelp()
		{
			General.ShowHelp("e_visual.html");
		}

		// When entering this mode
		public override void OnEngage()
		{
			base.OnEngage();
			
			// Read settings
			cameraflooroffset = General.Map.Config.ReadSetting("cameraflooroffset", cameraflooroffset);
			cameraceilingoffset = General.Map.Config.ReadSetting("cameraceilingoffset", cameraceilingoffset);

			RebuildElementData();
		}

		// When returning to another mode
		public override void OnDisengage()
		{
			base.OnDisengage();
			General.Map.Map.Update();
		}
		
		// Processing
		public override void OnProcess(long deltatime)
		{
			// Process things?
			base.ProcessThings = (BuilderPlug.Me.ShowVisualThings != 0);
			
			// Setup the move multiplier depending on gravity
			Vector3D movemultiplier = new Vector3D(1.0f, 1.0f, 1.0f);
			if(BuilderPlug.Me.UseGravity) movemultiplier.z = 0.0f;
			General.Map.VisualCamera.MoveMultiplier = movemultiplier;
			
			// Apply gravity?
			if(BuilderPlug.Me.UseGravity && (General.Map.VisualCamera.Sector != null))
			{
				SectorData sd = GetSectorData(General.Map.VisualCamera.Sector);
				if(!sd.Updated) sd.Update();

				// Camera below floor level?
				Vector3D feetposition = General.Map.VisualCamera.Position;
				SectorLevel floorlevel = sd.GetFloorBelow(feetposition) ?? sd.LightLevels[0];
				float floorheight = floorlevel.plane.GetZ(General.Map.VisualCamera.Position);
				if(General.Map.VisualCamera.Position.z < (floorheight + cameraflooroffset + 0.1f))
				{
					// Stay above floor
					gravity = new Vector3D(0.0f, 0.0f, 0.0f);
					General.Map.VisualCamera.Position = new Vector3D(General.Map.VisualCamera.Position.x,
																	 General.Map.VisualCamera.Position.y,
																	 floorheight + cameraflooroffset);
				}
				else
				{
					// Fall down
					gravity.z += (float)(GRAVITY * deltatime);
					if(gravity.z > 3.0f) gravity.z = 3.0f;

					// Test if we don't go through a floor
					if((General.Map.VisualCamera.Position.z + gravity.z) < (floorheight + cameraflooroffset + 0.1f))
					{
						// Stay above floor
						gravity = new Vector3D(0.0f, 0.0f, 0.0f);
						General.Map.VisualCamera.Position = new Vector3D(General.Map.VisualCamera.Position.x,
																		 General.Map.VisualCamera.Position.y,
																		 floorheight + cameraflooroffset);
					}
					else
					{
						// Apply gravity vector
						General.Map.VisualCamera.Position += gravity;
					}
				}

				// Camera above ceiling?
				feetposition = General.Map.VisualCamera.Position - new Vector3D(0, 0, cameraflooroffset - 7.0f);
				SectorLevel ceillevel = sd.GetCeilingAbove(feetposition) ?? sd.LightLevels[sd.LightLevels.Count - 1];
				float ceilheight = ceillevel.plane.GetZ(General.Map.VisualCamera.Position);
				if(General.Map.VisualCamera.Position.z > (ceilheight - cameraceilingoffset - 0.01f))
				{
					// Stay below ceiling
					General.Map.VisualCamera.Position = new Vector3D(General.Map.VisualCamera.Position.x,
																	 General.Map.VisualCamera.Position.y,
																	 ceilheight - cameraceilingoffset);
				}
			}
			else
			{
				gravity = new Vector3D(0.0f, 0.0f, 0.0f);
			}
			
			// Do processing
			base.OnProcess(deltatime);

			// Process visible geometry
			foreach(IVisualEventReceiver g in visiblegeometry)
			{
				g.OnProcess(deltatime);
			}
			
			// Time to pick a new target?
			if(General.stopwatch.Elapsed.TotalMilliseconds > (lastpicktime + PICK_INTERVAL))
			{
				PickTargetUnlocked();
				lastpicktime = General.stopwatch.Elapsed.TotalMilliseconds;
			}
			
			// The mouse is always in motion
			MouseEventArgs args = new MouseEventArgs(General.Interface.MouseButtons, 0, 0, 0, 0);
			OnMouseMove(args);
		}
		
		// This draws a frame
		public override void OnRedrawDisplay()
		{
			// Start drawing
			if(renderer.Start())
			{
				// Use fog!
				renderer.SetFogMode(true);

				// Set target for highlighting
				if(BuilderPlug.Me.UseHighlight)
					renderer.SetHighlightedObject(target.picked);
				
				// Begin with geometry
				renderer.StartGeometry();

				// Render all visible sectors
				foreach(VisualGeometry g in visiblegeometry)
					renderer.AddSectorGeometry(g);

				if(BuilderPlug.Me.ShowVisualThings != 0)
				{
					// Render things in cages?
					renderer.DrawThingCages = ((BuilderPlug.Me.ShowVisualThings & 2) != 0);
					
					// Render all visible things
					foreach(VisualThing t in visiblethings)
						renderer.AddThingGeometry(t);
				}
				
				// Done rendering geometry
				renderer.FinishGeometry();
				
				// Render crosshair
				renderer.RenderCrosshair();
				
				// Present!
				renderer.Finish();
			}
		}
		
		// After resources were reloaded
		protected override void ResourcesReloaded()
		{
			base.ResourcesReloaded();
			RebuildElementData();
			PickTarget();
		}
		
		// This usually happens when geometry is changed by undo, redo, cut or paste actions
		// and uses the marks to check what needs to be reloaded.
		protected override void ResourcesReloadedPartial()
		{
			bool sectorsmarked = false;
			
			if(General.Map.UndoRedo.GeometryChanged)
			{
				// Let the core do this (it will just dispose the sectors that were changed)
				base.ResourcesReloadedPartial();
			}
			else
			{
				// Neighbour sectors must be updated as well
				foreach(Sector s in General.Map.Map.Sectors)
				{
					if(s.Marked)
					{
						sectorsmarked = true;
						foreach(Sidedef sd in s.Sidedefs)
						{
							sd.Marked = true;
							if(sd.Other != null) sd.Other.Marked = true;
						}
					}
				}
				
				// Go for all sidedefs to update
				foreach(Sidedef sd in General.Map.Map.Sidedefs)
				{
					if(sd.Marked && VisualSectorExists(sd.Sector))
					{
						BaseVisualSector vs = (BaseVisualSector)GetVisualSector(sd.Sector);
						VisualSidedefParts parts = vs.GetSidedefParts(sd);
						parts.SetupAllParts();
					}
				}
				
				// Go for all sectors to update
				foreach(Sector s in General.Map.Map.Sectors)
				{
					if(s.Marked)
					{
						SectorData sd = GetSectorData(s);
						sd.Reset();
						
						// UpdateSectorGeometry for associated sectors (sd.UpdateAlso) as well!
						foreach(KeyValuePair<Sector, bool> us in sd.UpdateAlso)
						{
							if(VisualSectorExists(us.Key))
							{
								BaseVisualSector vs = (BaseVisualSector)GetVisualSector(us.Key);
								vs.UpdateSectorGeometry(us.Value);
							}
						}
						
						// And update for this sector ofcourse
						if(VisualSectorExists(s))
						{
							BaseVisualSector vs = (BaseVisualSector)GetVisualSector(s);
							vs.UpdateSectorGeometry(false);
						}
					}
				}
				
				if(!sectorsmarked)
				{
					// No sectors or geometry changed. So we only have
					// to update things when they have changed.
					foreach(KeyValuePair<Thing, VisualThing> vt in allthings)
						if((vt.Value != null) && vt.Key.Marked) (vt.Value as BaseVisualThing).Rebuild();
				}
				else
				{
					// Things depend on the sector they are in and because we can't
					// easily determine which ones changed, we dispose all things
					foreach(KeyValuePair<Thing, VisualThing> vt in allthings)
						if(vt.Value != null) vt.Value.Dispose();
					
					// Apply new lists
					allthings = new Dictionary<Thing, VisualThing>(allthings.Count);
				}
				
				// Clear visibility collections
				visiblesectors.Clear();
				visibleblocks.Clear();
				visiblegeometry.Clear();
				visiblethings.Clear();
				
				// Make new blockmap
				if(sectorsmarked || General.Map.UndoRedo.PopulationChanged)
					FillBlockMap();
				
				RebuildElementData();
				UpdateChangedObjects();
				
				// Visibility culling (this re-creates the needed resources)
				DoCulling();
			}
			
			// Determine what we're aiming at now
			PickTarget();
		}
		
		// Mouse moves
		public override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);
			GetTargetEventReceiver(true).OnMouseMove(e);
		}
		
		// Undo performed
		public override void OnUndoEnd()
		{
			base.OnUndoEnd();
			RebuildSelectedObjectsList();
			
			// We can't group with this undo level anymore
			lastundogroup = UndoGroup.None;
		}
		
		// Redo performed
		public override void OnRedoEnd()
		{
			base.OnRedoEnd();
			RebuildSelectedObjectsList();
		}
		
		#endregion

		#region ================== Action Assist

		// Because some actions can only be called on a single (the targeted) object because
		// they show a dialog window or something, these functions help applying the result
		// to all compatible selected objects.
		
		// Apply texture offsets
		public void ApplyTextureOffsetChange(int dx, int dy)
		{
			Dictionary<Sidedef, int> donesides = new Dictionary<Sidedef, int>(selectedobjects.Count);
			List<IVisualEventReceiver> objs = GetSelectedObjects(false, true, false);
			foreach(IVisualEventReceiver i in objs)
			{
				if(i is BaseVisualGeometrySidedef)
				{
					if(!donesides.ContainsKey((i as BaseVisualGeometrySidedef).Sidedef))
					{
						i.OnChangeTextureOffset(dx, dy);
						donesides.Add((i as BaseVisualGeometrySidedef).Sidedef, 0);
					}
				}
			}
		}

		// Apply flat offsets
		public void ApplyFlatOffsetChange(int dx, int dy)
		{
			Dictionary<Sector, int> donesectors = new Dictionary<Sector, int>(selectedobjects.Count);
			List<IVisualEventReceiver> objs = GetSelectedObjects(true, false, false);
			foreach(IVisualEventReceiver i in objs)
			{
				if(i is BaseVisualGeometrySector)
				{
					if(!donesectors.ContainsKey((i as BaseVisualGeometrySector).Sector.Sector))
					{
						i.OnChangeTextureOffset(dx, dy);
						donesectors.Add((i as BaseVisualGeometrySector).Sector.Sector, 0);
					}
				}
			}
		}

		// Apply upper unpegged flag
		public void ApplyUpperUnpegged(bool set)
		{
			List<IVisualEventReceiver> objs = GetSelectedObjects(true, true, true);
			foreach(IVisualEventReceiver i in objs)
			{
				i.ApplyUpperUnpegged(set);
			}
		}

		// Apply lower unpegged flag
		public void ApplyLowerUnpegged(bool set)
		{
			List<IVisualEventReceiver> objs = GetSelectedObjects(true, true, true);
			foreach(IVisualEventReceiver i in objs)
			{
				i.ApplyLowerUnpegged(set);
			}
		}

		// Apply texture change
		public void ApplySelectTexture(string texture, bool flat)
		{
			List<IVisualEventReceiver> objs;
			
			if(General.Map.Config.MixTexturesFlats)
			{
				// Apply on all compatible types
				objs = GetSelectedObjects(true, true, false);
			}
			else
			{
				// We don't want to mix textures and flats, so apply only on the appropriate type
				objs = GetSelectedObjects(flat, !flat, false);
			}
			
			foreach(IVisualEventReceiver i in objs)
			{
				i.ApplyTexture(texture);
			}
		}

		// This returns all selected objects
		internal List<IVisualEventReceiver> GetSelectedObjects(bool includesectors, bool includesidedefs, bool includethings)
		{
			List<IVisualEventReceiver> objs = new List<IVisualEventReceiver>();
			foreach(IVisualEventReceiver i in selectedobjects)
			{
				if((i is BaseVisualGeometrySector) && includesectors) objs.Add(i);
				else if((i is BaseVisualGeometrySidedef) && includesidedefs) objs.Add(i);
				else if((i is BaseVisualThing) && includethings) objs.Add(i);
			}

			// Add highlight?
			if(selectedobjects.Count == 0)
			{
				IVisualEventReceiver i = (target.picked as IVisualEventReceiver);
				if((i is BaseVisualGeometrySector) && includesectors) objs.Add(i);
				else if((i is BaseVisualGeometrySidedef) && includesidedefs) objs.Add(i);
				else if((i is BaseVisualThing) && includethings) objs.Add(i);
			}

			return objs;
		}

		// This returns all selected sectors, no doubles
		public List<Sector> GetSelectedSectors()
		{
			Dictionary<Sector, int> added = new Dictionary<Sector, int>();
			List<Sector> sectors = new List<Sector>();
			foreach(IVisualEventReceiver i in selectedobjects)
			{
				if(i is BaseVisualGeometrySector)
				{
					Sector s = (i as BaseVisualGeometrySector).Level.sector;
					if(!added.ContainsKey(s))
					{
						sectors.Add(s);
						added.Add(s, 0);
					}
				}
			}

			// Add highlight?
			if((selectedobjects.Count == 0) && (target.picked is BaseVisualGeometrySector))
			{
				Sector s = (target.picked as BaseVisualGeometrySector).Level.sector;
				if(!added.ContainsKey(s))
					sectors.Add(s);
			}
			
			return sectors;
		}

		// This returns all selected linedefs, no doubles
		public List<Linedef> GetSelectedLinedefs()
		{
			Dictionary<Linedef, int> added = new Dictionary<Linedef, int>();
			List<Linedef> linedefs = new List<Linedef>();
			foreach(IVisualEventReceiver i in selectedobjects)
			{
				if(i is BaseVisualGeometrySidedef)
				{
					Linedef l = (i as BaseVisualGeometrySidedef).Sidedef.Line;
					if(!added.ContainsKey(l))
					{
						linedefs.Add(l);
						added.Add(l, 0);
					}
				}
			}

			// Add highlight?
			if((selectedobjects.Count == 0) && (target.picked is BaseVisualGeometrySidedef))
			{
				Linedef l = (target.picked as BaseVisualGeometrySidedef).Sidedef.Line;
				if(!added.ContainsKey(l))
					linedefs.Add(l);
			}

			return linedefs;
		}

		// This returns all selected sidedefs, no doubles
		public List<Sidedef> GetSelectedSidedefs()
		{
			Dictionary<Sidedef, int> added = new Dictionary<Sidedef, int>();
			List<Sidedef> sidedefs = new List<Sidedef>();
			foreach(IVisualEventReceiver i in selectedobjects)
			{
				if(i is BaseVisualGeometrySidedef)
				{
					Sidedef sd = (i as BaseVisualGeometrySidedef).Sidedef;
					if(!added.ContainsKey(sd))
					{
						sidedefs.Add(sd);
						added.Add(sd, 0);
					}
				}
			}

			// Add highlight?
			if((selectedobjects.Count == 0) && (target.picked is BaseVisualGeometrySidedef))
			{
				Sidedef sd = (target.picked as BaseVisualGeometrySidedef).Sidedef;
				if(!added.ContainsKey(sd))
					sidedefs.Add(sd);
			}

			return sidedefs;
		}

		// This returns all selected things, no doubles
		public List<Thing> GetSelectedThings()
		{
			Dictionary<Thing, int> added = new Dictionary<Thing, int>();
			List<Thing> things = new List<Thing>();
			foreach(IVisualEventReceiver i in selectedobjects)
			{
				if(i is BaseVisualThing)
				{
					Thing t = (i as BaseVisualThing).Thing;
					if(!added.ContainsKey(t))
					{
						things.Add(t);
						added.Add(t, 0);
					}
				}
			}

			// Add highlight?
			if((selectedobjects.Count == 0) && (target.picked is BaseVisualThing))
			{
				Thing t = (target.picked as BaseVisualThing).Thing;
				if(!added.ContainsKey(t))
					things.Add(t);
			}

			return things;
		}
		
		// This returns the IVisualEventReceiver on which the action must be performed
		private IVisualEventReceiver GetTargetEventReceiver(bool targetonly)
		{
			if(target.picked != null)
			{
				if(singleselection || target.picked.Selected || targetonly)
				{
					return (IVisualEventReceiver)target.picked;
				}
				else if(selectedobjects.Count > 0)
				{
					return selectedobjects[0];
				}
				else
				{
					return (IVisualEventReceiver)target.picked;
				}
			}
			else
			{
				return new NullVisualEventReceiver();
			}
		}
		
		#endregion

		#region ================== Actions

		[BeginAction("clearselection", BaseAction = true)]
		public void ClearSelection()
		{
			selectedobjects = new List<IVisualEventReceiver>();
			
			foreach(KeyValuePair<Sector, VisualSector> vs in allsectors)
			{
				if(vs.Value != null)
				{
					BaseVisualSector bvs = (BaseVisualSector)vs.Value;
					if(bvs.Floor != null) bvs.Floor.Selected = false;
					if(bvs.Ceiling != null) bvs.Ceiling.Selected = false;
					foreach(VisualFloor vf in bvs.ExtraFloors) vf.Selected = false;
					foreach(VisualCeiling vc in bvs.ExtraCeilings) vc.Selected = false;
					foreach(Sidedef sd in vs.Key.Sidedefs)
					{
						List<VisualGeometry> sidedefgeos = bvs.GetSidedefGeometry(sd);
						foreach(VisualGeometry sdg in sidedefgeos)
						{
							sdg.Selected = false;
						}
					}
				}
			}

			foreach(KeyValuePair<Thing, VisualThing> vt in allthings)
			{
				if(vt.Value != null)
				{
					BaseVisualThing bvt = (BaseVisualThing)vt.Value;
					bvt.Selected = false;
				}
			}
		}

		[BeginAction("visualselect", BaseAction = true)]
		public void BeginSelect()
		{
			PreActionNoChange();
			PickTargetUnlocked();
			GetTargetEventReceiver(true).OnSelectBegin();
			PostAction();
		}

		[EndAction("visualselect", BaseAction = true)]
		public void EndSelect()
		{
			//PreActionNoChange();
			GetTargetEventReceiver(true).OnSelectEnd();
			Renderer.ShowSelection = true;
			Renderer.ShowHighlight = true;
			PostAction();
		}

		[BeginAction("visualedit", BaseAction = true)]
		public void BeginEdit()
		{
			PreAction(UndoGroup.None);
			GetTargetEventReceiver(false).OnEditBegin();
			PostAction();
		}

		[EndAction("visualedit", BaseAction = true)]
		public void EndEdit()
		{
			PreActionNoChange();
			GetTargetEventReceiver(false).OnEditEnd();
			PostAction();
		}

		[BeginAction("raisesector8", Library = "BuilderModes")]
		public void RaiseSector8()
		{
			PreAction(UndoGroup.SectorHeightChange);
			List<IVisualEventReceiver> objs = GetSelectedObjects(true, true, true);
			foreach(IVisualEventReceiver i in objs) i.OnChangeTargetHeight(8);
			PostAction();
		}

		[BeginAction("lowersector8", Library = "BuilderModes")]
		public void LowerSector8()
		{
			PreAction(UndoGroup.SectorHeightChange);
			List<IVisualEventReceiver> objs = GetSelectedObjects(true, true, true);
			foreach(IVisualEventReceiver i in objs) i.OnChangeTargetHeight(-8);
			PostAction();
		}

		[BeginAction("raisesector1", Library = "BuilderModes")]
		public void RaiseSector1()
		{
			PreAction(UndoGroup.SectorHeightChange);
			List<IVisualEventReceiver> objs = GetSelectedObjects(true, true, true);
			foreach(IVisualEventReceiver i in objs) i.OnChangeTargetHeight(1);
			PostAction();
		}

		[BeginAction("lowersector1", Library = "BuilderModes")]
		public void LowerSector1()
		{
			PreAction(UndoGroup.SectorHeightChange);
			List<IVisualEventReceiver> objs = GetSelectedObjects(true, true, true);
			foreach(IVisualEventReceiver i in objs) i.OnChangeTargetHeight(-1);
			PostAction();
		}

		[BeginAction("showvisualthings", Library = "BuilderModes")]
		public void ShowVisualThings()
		{
			BuilderPlug.Me.ShowVisualThings++;
			if(BuilderPlug.Me.ShowVisualThings > 2) BuilderPlug.Me.ShowVisualThings = 0;
		}

		[BeginAction("raisebrightness8", Library = "BuilderModes")]
		public void RaiseBrightness8()
		{
			PreAction(UndoGroup.SectorBrightnessChange);
			List<IVisualEventReceiver> objs = GetSelectedObjects(true, true, true);
			foreach(IVisualEventReceiver i in objs) i.OnChangeTargetBrightness(true);
			PostAction();
		}

		[BeginAction("lowerbrightness8", Library = "BuilderModes")]
		public void LowerBrightness8()
		{
			PreAction(UndoGroup.SectorBrightnessChange);
			List<IVisualEventReceiver> objs = GetSelectedObjects(true, true, true);
			foreach(IVisualEventReceiver i in objs) i.OnChangeTargetBrightness(false);
			PostAction();
		}

		[BeginAction("movetextureleft", Library = "BuilderModes")]
		public void MoveTextureLeft1()
		{
			PreAction(UndoGroup.TextureOffsetChange);
			List<IVisualEventReceiver> objs = GetSelectedObjects(true, true, true);
			foreach(IVisualEventReceiver i in objs) i.OnChangeTextureOffset(-1, 0);
			PostAction();
		}

		[BeginAction("movetextureright", Library = "BuilderModes")]
		public void MoveTextureRight1()
		{
			PreAction(UndoGroup.TextureOffsetChange);
			List<IVisualEventReceiver> objs = GetSelectedObjects(true, true, true);
			foreach(IVisualEventReceiver i in objs) i.OnChangeTextureOffset(1, 0);
			PostAction();
		}

		[BeginAction("movetextureup", Library = "BuilderModes")]
		public void MoveTextureUp1()
		{
			PreAction(UndoGroup.TextureOffsetChange);
			List<IVisualEventReceiver> objs = GetSelectedObjects(true, true, true);
			foreach(IVisualEventReceiver i in objs) i.OnChangeTextureOffset(0, -1);
			PostAction();
		}

		[BeginAction("movetexturedown", Library = "BuilderModes")]
		public void MoveTextureDown1()
		{
			PreAction(UndoGroup.TextureOffsetChange);
			List<IVisualEventReceiver> objs = GetSelectedObjects(true, true, true);
			foreach(IVisualEventReceiver i in objs) i.OnChangeTextureOffset(0, 1);
			PostAction();
		}

		[BeginAction("movetextureleft8", Library = "BuilderModes")]
		public void MoveTextureLeft8()
		{
			PreAction(UndoGroup.TextureOffsetChange);
			List<IVisualEventReceiver> objs = GetSelectedObjects(true, true, true);
			foreach(IVisualEventReceiver i in objs) i.OnChangeTextureOffset(-8, 0);
			PostAction();
		}

		[BeginAction("movetextureright8", Library = "BuilderModes")]
		public void MoveTextureRight8()
		{
			PreAction(UndoGroup.TextureOffsetChange);
			List<IVisualEventReceiver> objs = GetSelectedObjects(true, true, true);
			foreach(IVisualEventReceiver i in objs) i.OnChangeTextureOffset(8, 0);
			PostAction();
		}

		[BeginAction("movetextureup8", Library = "BuilderModes")]
		public void MoveTextureUp8()
		{
			PreAction(UndoGroup.TextureOffsetChange);
			List<IVisualEventReceiver> objs = GetSelectedObjects(true, true, true);
			foreach(IVisualEventReceiver i in objs) i.OnChangeTextureOffset(0, -8);
			PostAction();
		}

		[BeginAction("movetexturedown8", Library = "BuilderModes")]
		public void MoveTextureDown8()
		{
			PreAction(UndoGroup.TextureOffsetChange);
			List<IVisualEventReceiver> objs = GetSelectedObjects(true, true, true);
			foreach(IVisualEventReceiver i in objs) i.OnChangeTextureOffset(0, 8);
			PostAction();
		}

		[BeginAction("textureselect", Library = "BuilderModes")]
		public void TextureSelect()
		{
			PreAction(UndoGroup.None);
			renderer.SetCrosshairBusy(true);
			General.Interface.RedrawDisplay();
			GetTargetEventReceiver(false).OnSelectTexture();
			UpdateChangedObjects();
			renderer.SetCrosshairBusy(false);
			PostAction();
		}

		[BeginAction("texturecopy", Library = "BuilderModes")]
		public void TextureCopy()
		{
			PreActionNoChange();
			GetTargetEventReceiver(false).OnCopyTexture();
			PostAction();
		}

		[BeginAction("texturepaste", Library = "BuilderModes")]
		public void TexturePaste()
		{
			PreAction(UndoGroup.None);
			List<IVisualEventReceiver> objs = GetSelectedObjects(true, true, true);
			foreach(IVisualEventReceiver i in objs) i.OnPasteTexture();
			PostAction();
		}

		[BeginAction("visualautoalignx", Library = "BuilderModes")]
		public void TextureAutoAlignX()
		{
			PreAction(UndoGroup.None);
			renderer.SetCrosshairBusy(true);
			General.Interface.RedrawDisplay();
			GetTargetEventReceiver(false).OnTextureAlign(true, false);
			UpdateChangedObjects();
			renderer.SetCrosshairBusy(false);
			PostAction();
		}

		[BeginAction("visualautoaligny", Library = "BuilderModes")]
		public void TextureAutoAlignY()
		{
			PreAction(UndoGroup.None);
			renderer.SetCrosshairBusy(true);
			General.Interface.RedrawDisplay();
			GetTargetEventReceiver(false).OnTextureAlign(false, true);
			UpdateChangedObjects();
			renderer.SetCrosshairBusy(false);
			PostAction();
		}

		[BeginAction("toggleupperunpegged", Library = "BuilderModes")]
		public void ToggleUpperUnpegged()
		{
			PreAction(UndoGroup.None);
			GetTargetEventReceiver(false).OnToggleUpperUnpegged();
			PostAction();
		}

		[BeginAction("togglelowerunpegged", Library = "BuilderModes")]
		public void ToggleLowerUnpegged()
		{
			PreAction(UndoGroup.None);
			GetTargetEventReceiver(false).OnToggleLowerUnpegged();
			PostAction();
		}

		[BeginAction("togglegravity", Library = "BuilderModes")]
		public void ToggleGravity()
		{
			BuilderPlug.Me.UseGravity = !BuilderPlug.Me.UseGravity;
			string onoff = BuilderPlug.Me.UseGravity ? "ON" : "OFF";
			General.Interface.DisplayStatus(StatusType.Action, "Gravity is now " + onoff + ".");
		}

		[BeginAction("togglebrightness", Library = "BuilderModes")]
		public void ToggleBrightness()
		{
			renderer.FullBrightness = !renderer.FullBrightness;
			string onoff = renderer.FullBrightness ? "ON" : "OFF";
			General.Interface.DisplayStatus(StatusType.Action, "Full Brightness is now " + onoff + ".");
		}

		[BeginAction("togglehighlight", Library = "BuilderModes")]
		public void ToggleHighlight()
		{
			BuilderPlug.Me.UseHighlight = !BuilderPlug.Me.UseHighlight;
			string onoff = BuilderPlug.Me.UseHighlight ? "ON" : "OFF";
			General.Interface.DisplayStatus(StatusType.Action, "Highlight is now " + onoff + ".");
		}

		[BeginAction("resettexture", Library = "BuilderModes")]
		public void ResetTexture()
		{
			PreAction(UndoGroup.None);
			List<IVisualEventReceiver> objs = GetSelectedObjects(true, true, true);
			foreach(IVisualEventReceiver i in objs) i.OnResetTextureOffset();
			PostAction();
		}

		[BeginAction("floodfilltextures", Library = "BuilderModes")]
		public void FloodfillTextures()
		{
			PreAction(UndoGroup.None);
			GetTargetEventReceiver(false).OnTextureFloodfill();
			PostAction();
		}

		[BeginAction("texturecopyoffsets", Library = "BuilderModes")]
		public void TextureCopyOffsets()
		{
			PreActionNoChange();
			GetTargetEventReceiver(false).OnCopyTextureOffsets();
			PostAction();
		}

		[BeginAction("texturepasteoffsets", Library = "BuilderModes")]
		public void TexturePasteOffsets()
		{
			PreAction(UndoGroup.None);
			List<IVisualEventReceiver> objs = GetSelectedObjects(true, true, true);
			foreach(IVisualEventReceiver i in objs) i.OnPasteTextureOffsets();
			PostAction();
		}

		[BeginAction("copyproperties", Library = "BuilderModes")]
		public void CopyProperties()
		{
			PreActionNoChange();
			GetTargetEventReceiver(false).OnCopyProperties();
			PostAction();
		}

		[BeginAction("pasteproperties", Library = "BuilderModes")]
		public void PasteProperties()
		{
			PreAction(UndoGroup.None);
			List<IVisualEventReceiver> objs = GetSelectedObjects(true, true, true);
			foreach(IVisualEventReceiver i in objs) i.OnPasteProperties();
			PostAction();
		}
		
		[BeginAction("insertitem", BaseAction = true)]
		public void Insert()
		{
			PreAction(UndoGroup.None);
			List<IVisualEventReceiver> objs = GetSelectedObjects(true, true, true);
			foreach(IVisualEventReceiver i in objs) i.OnInsert();
			PostAction();
		}

		[BeginAction("deleteitem", BaseAction = true)]
		public void Delete()
		{
			PreAction(UndoGroup.None);
			List<IVisualEventReceiver> objs = GetSelectedObjects(true, true, true);
			foreach(IVisualEventReceiver i in objs) i.OnDelete();
			PostAction();
		}
		
		#endregion
	}
}
