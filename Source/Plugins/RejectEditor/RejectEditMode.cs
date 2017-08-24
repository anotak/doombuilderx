#region === Copyright (c) 2010 Pascal van der Heiden ===

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using CodeImp.DoomBuilder.Editing;
using CodeImp.DoomBuilder.Geometry;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Rendering;
using CodeImp.DoomBuilder.Types;
using CodeImp.DoomBuilder.Windows;

#endregion

namespace CodeImp.DoomBuilder.Plugins.RejectEditor
{
	[EditMode(DisplayName = "Reject Editor",
			  SwitchAction = "rejecteditmode",
			  ButtonImage = "visibility.png",
			  ButtonOrder = 360,
			  ButtonGroup = "002_tools",
			  Volatile = true,
			  UseByDefault = true,
			  AllowCopyPaste = false)]
	public class RejectEditMode : ClassicMode
	{
		#region ================== Constants

		private const float OVERLAY_ALPHA = 0.5f;

		#endregion

		#region ================== Variables

		private bool[] manualsectors;
		private Sector highlighted;
		private Sector selected;

		#endregion

		#region ================== Properties

		#endregion

		#region ================== Constructor / Destructor

		// Constructor
		public RejectEditMode()
		{
		}

		// Disposer
		public override void Dispose()
		{
			base.Dispose();
		}

		#endregion

		#region ================== Methods

		// This highlights a new item
		protected void Highlight(Sector s)
		{
			if(s == highlighted) return;
			
			// Update display
			if(renderer.StartPlotter(false))
			{
				if((highlighted != null) && !highlighted.IsDisposed)
				{
					// Undraw previous highlight
					foreach(Sidedef sd in highlighted.Sidedefs)
					{
						if(!ReferenceEquals(sd.Line.Front, sd.Line.Back) || sd.IsFront)
							renderer.PlotLinedef(sd.Line, renderer.DetermineLinedefColor(sd.Line));
					}
				}

				// Set new highlight
				highlighted = s;

				if((highlighted != null) && !highlighted.IsDisposed)
				{
					// Render highlighted item
					foreach(Sidedef sd in highlighted.Sidedefs)
					{
						if(!ReferenceEquals(sd.Line.Front, sd.Line.Back) || sd.IsFront)
							renderer.PlotLinedef(sd.Line, General.Colors.Highlight);
					}
				}

				renderer.Finish();
			}

			UpdateOverlay();
			renderer.Present();

			// Show highlight info
			if((highlighted != null) && !highlighted.IsDisposed)
				General.Interface.ShowSectorInfo(highlighted);
			else
				General.Interface.HideInfo();
		}

		// This updates the overlay
		private void UpdateOverlay()
		{
			MapSet map = General.Map.Map;

			if(renderer.StartOverlay(true))
			{
				if(selected != null)
				{
					int manualhiddencolor = PixelColor.FromColor(Color.Red).ToInt();
					int manualvisiblecolor = PixelColor.FromColor(Color.DeepSkyBlue).ToInt();
					int tablevisiblecolor = PixelColor.FromColor(Color.Blue).ToInt();
					
					Array.Clear(manualsectors, 0, manualsectors.Length);

					List<RejectSet> targets;
					if(BuilderPlug.Me.RejectChanges.TryGetValue(selected.FixedIndex, out targets))
					{
						// Draw all sectors that were set manually
						foreach(RejectSet rs in targets)
						{
							Sector ts = BuilderPlug.Me.GetSectorByFixedIndex(rs.target);
							switch(rs.state)
							{
								case RejectState.ForceHidden: RenderSectorFilled(ts, manualhiddencolor); break;
								case RejectState.ForceVisible: RenderSectorFilled(ts, manualvisiblecolor); break;
								default: throw new NotImplementedException();
							}
							manualsectors[rs.target] = true;
						}
					}

					// For all remaining sectors in the table, render them as the nodebuilder decided
					bool[] tabletargets = BuilderPlug.Me.UnmodifiedTable[selected.Index];
					for(int i = 0; i < tabletargets.Length; i++)
					{
						if(manualsectors[i]) continue;
						if(tabletargets[i]) continue;

						RenderSectorFilled(map.GetSectorByIndex(i), tablevisiblecolor);
					}
				}
				else
				{
					int indicationcolor = General.Colors.Indication.ToInt();
					foreach(KeyValuePair<int, List<RejectSet>> ss in BuilderPlug.Me.RejectChanges)
					{
						RenderSectorFilled(BuilderPlug.Me.GetSectorByFixedIndex(ss.Key), indicationcolor);
					}
				}

				renderer.Finish();
			}
		}

		private void RenderSectorFilled(Sector s, int color)
		{
			// Render the geometry
			FlatVertex[] verts = new FlatVertex[s.FlatVertices.Length];
			s.FlatVertices.CopyTo(verts, 0);
			for(int i = 0; i < verts.Length; i++) verts[i].c = color;
			renderer.RenderGeometry(verts, null, true);
		}

		// This selectes or deselects a sector
		protected void SelectSector(Sector s)
		{
			if(selected != null)
			{
				selected.Selected = false;
				General.Map.Map.ClearSelectedLinedefs();
				General.Map.Map.ClearSelectedSectors();
			}

			selected = s;

			// Select the sector?
			if(selected != null)
			{
				selected.Selected = true;
				foreach(Sidedef sd in selected.Sidedefs)
				{
					sd.Line.Selected = true;
				}
			}

			General.Interface.RedrawDisplay();
		}

		#endregion

		#region ================== Events

		// Mode starts
		public override void OnEngage()
		{
			Cursor.Current = Cursors.WaitCursor;
			base.OnEngage();

			BuilderPlug.Me.CleanUpRejectStates();

			if(!General.Map.LumpExists("REJECT") || General.Map.IsChanged)
			{
				// Outdated or missing reject map
				bool result = General.Map.RebuildNodes(General.Map.ConfigSettings.NodebuilderSave, true);
				if(!result || !General.Map.LumpExists("REJECT"))
				{
					MessageBox.Show("Unable to find the REJECT table. Check your nodebuilder settings and make sure you are using a nodebuilder that generates a reject table.", "Reject Editing mode", MessageBoxButtons.OK, MessageBoxIcon.Error);
					General.Editing.CancelMode();
					return;
				}
			}

			// During overlay rendering, we use this list to keep track of sectors
			// we have manually set. This is only for performance optimization.
			manualsectors = new bool[General.Map.Map.Sectors.Count];

			// If the reject map was not obtained yet, try to rebuild
			// the nodes to get the latest reject map.
			if(BuilderPlug.Me.UnmodifiedTable == null)
			{
				// Invalid reject map
				bool result = General.Map.RebuildNodes(General.Map.ConfigSettings.NodebuilderSave, true);
				if(!result || (BuilderPlug.Me.UnmodifiedTable == null))
				{
					MessageBox.Show("The nodebuilder is generating an invalid REJECT table. Check your nodebuilder settings and make sure you are using a nodebuilder that generates a proper reject table.", "Reject Editing mode", MessageBoxButtons.OK, MessageBoxIcon.Error);
					General.Editing.CancelMode();
					return;
				}
			}

			// Setup presentation
			CustomPresentation presentation = new CustomPresentation();
			presentation.AddLayer(new PresentLayer(RendererLayer.Background, BlendingMode.Mask, General.Settings.BackgroundAlpha));
			presentation.AddLayer(new PresentLayer(RendererLayer.Grid, BlendingMode.Mask));
			presentation.AddLayer(new PresentLayer(RendererLayer.Surface, BlendingMode.Mask));
			presentation.AddLayer(new PresentLayer(RendererLayer.Overlay, BlendingMode.Alpha, OVERLAY_ALPHA, true));
			presentation.AddLayer(new PresentLayer(RendererLayer.Geometry, BlendingMode.Alpha, 1f, true));
			renderer.SetPresentation(presentation);

			Cursor.Current = Cursors.Default;
			General.Interface.DisplayReady();
			General.Interface.RedrawDisplay();
		}
		
		// Mode ends
		public override void OnDisengage()
		{
			base.OnDisengage();
		}

		// Cancelled
		public override void OnCancel()
		{
			// Cancel base class
			base.OnCancel();

			// Return to previous mode
			General.Editing.ChangeMode(General.Editing.PreviousStableMode.Name);
		}

		// Mouse moves
		public override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);

			// Find the nearest linedef within highlight range
			Linedef l = General.Map.Map.NearestLinedef(mousemappos);
			if(l != null)
			{
				// Check on which side of the linedef the mouse is
				float side = l.SideOfLine(mousemappos);
				if(side > 0)
				{
					// Is there a sidedef here?
					if(l.Back != null)
					{
						// Highlight if not the same
						if(l.Back.Sector != highlighted) Highlight(l.Back.Sector);
					}
					else
					{
						// Highlight nothing
						if(highlighted != null) Highlight(null);
					}
				}
				else
				{
					// Is there a sidedef here?
					if(l.Front != null)
					{
						// Highlight if not the same
						if(l.Front.Sector != highlighted) Highlight(l.Front.Sector);
					}
					else
					{
						// Highlight nothing
						if(highlighted != null) Highlight(null);
					}
				}
			}
			else
			{
				// Highlight nothing
				if(highlighted != null) Highlight(null);
			}
		}

		// Mouse leaves
		public override void OnMouseLeave(EventArgs e)
		{
			base.OnMouseLeave(e);
			General.Interface.RedrawDisplay();
		}

		// Mouse clicks to select a subsector
		protected override void OnSelectBegin()
		{
			base.OnSelectBegin();

			SelectSector(highlighted);
		}

		protected override void OnEditBegin()
		{
			base.OnEditBegin();

			if((selected == null) || (highlighted == null)) return;

			// Create undo
			General.Map.UndoRedo.CreateUndo("Toggle sector reject");

			// Get the list of states for target sectors
			List<RejectSet> targets;
			if(!BuilderPlug.Me.RejectChanges.TryGetValue(selected.FixedIndex, out targets))
				targets = new List<RejectSet>();

			// Determine the current state for the highlighted (target) sector
			int targetindex = -1;
			RejectState state = RejectState.None;
			for(int i = 0; i < targets.Count; i++)
			{
				RejectSet s = targets[i];
				if(s.target == highlighted.FixedIndex)
				{
					targetindex = i;
					state = s.state;
					break;
				}
			}

			// Cycle to the next state
			switch(state)
			{
				case RejectState.None: state = RejectState.ForceHidden; break;
				case RejectState.ForceHidden: state = RejectState.ForceVisible; break;
				case RejectState.ForceVisible: state = RejectState.None; break;
				default: throw new NotImplementedException();
			}

			// If the new state is 'None' then remove the target sector from the list,
			// else just update or add the state in the list.
			if(state == RejectState.None)
			{
				targets.RemoveAt(targetindex);
			}
			else if(targetindex > -1)
			{
				targets[targetindex] = new RejectSet(highlighted.FixedIndex, state);
			}
			else
			{
				targets.Add(new RejectSet(highlighted.FixedIndex, state));
			}

			// Make sure the list is stored in our table of source sectors
			// or removed when there are no more manual changes.
			if(targets.Count > 0)
			{
				BuilderPlug.Me.RejectChanges[selected.FixedIndex] = targets;
			}
			else
			{
				BuilderPlug.Me.RejectChanges.Remove(selected.FixedIndex);
			}

			// Update!
			UpdateOverlay();
			renderer.Present();
		}

		// Draw the display
		public override void OnRedrawDisplay()
		{
			renderer.RedrawSurface();

			// Render lines and vertices
			if(renderer.StartPlotter(true))
			{
				renderer.PlotLinedefSet(General.Map.Map.Linedefs);
				if((selected != null) && !selected.IsDisposed)
					renderer.PlotSector(selected, General.Colors.Selection);
				if((highlighted != null) && !highlighted.IsDisposed)
					renderer.PlotSector(highlighted, General.Colors.Highlight);
				renderer.Finish();
			}

			// Render things
			if(renderer.StartThings(true))
			{
				renderer.RenderThingSet(General.Map.ThingsFilter.HiddenThings, Presentation.THINGS_HIDDEN_ALPHA);
				renderer.RenderThingSet(General.Map.ThingsFilter.VisibleThings, 1.0f);
				renderer.Finish();
			}

			// Render overlay
			UpdateOverlay();

			renderer.Present();
		}

		#endregion
	}
}
