
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
using CodeImp.DoomBuilder.Windows;
using CodeImp.DoomBuilder.IO;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Rendering;
using CodeImp.DoomBuilder.Geometry;
using CodeImp.DoomBuilder.Editing;
using System.Drawing;
using CodeImp.DoomBuilder.Actions;
using CodeImp.DoomBuilder.Types;
using CodeImp.DoomBuilder.BuilderModes.Interface;

#endregion

namespace CodeImp.DoomBuilder.BuilderModes
{
	[EditMode(DisplayName = "Sectors Mode",
			  SwitchAction = "sectorsmode",		// Action name used to switch to this mode
		      ButtonImage = "SectorsMode.png",	// Image resource name for the button
			  ButtonOrder = int.MinValue + 200,	// Position of the button (lower is more to the left)
			  ButtonGroup = "000_editing",
			  UseByDefault = true,
			  SafeStartMode = true)]

	public class SectorsMode : BaseClassicMode
	{
		#region ================== Constants

		#endregion

		#region ================== Variables

		// Highlighted item
		protected Sector highlighted;
		private Association highlightasso = new Association();
        public const int NUM_PORTAL_ASSOCIATIONS = 2;
        private Association[] portalasso = new Association[NUM_PORTAL_ASSOCIATIONS];

        // Interface
        protected bool editpressed;

		// Labels
		private Dictionary<Sector, TextLabel[]> labels;
		
		#endregion

		#region ================== Properties

		public override object HighlightedObject { get { return highlighted; } }

		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		public SectorsMode()
		{
		}

		// Disposer
		public override void Dispose()
		{
			// Not already disposed?
			if(!isdisposed)
			{
				// Dispose old labels
				foreach(KeyValuePair<Sector, TextLabel[]> lbl in labels)
					foreach(TextLabel l in lbl.Value) l.Dispose();

				// Dispose base
				base.Dispose();
			}
		}

		#endregion

		#region ================== Methods

		// This makes a CRC for the selection
		public int CreateSelectionCRC()
		{
			CRC crc = new CRC();
			ICollection<Sector> orderedselection = General.Map.Map.GetSelectedSectors(true);
			crc.Add(orderedselection.Count);
			foreach(Sector s in orderedselection)
			{
				crc.Add(s.FixedIndex);
			}
			return (int)(crc.Value & 0xFFFFFFFF);
		}

		// This sets up new labels
		private void SetupLabels()
		{
			if(labels != null)
			{
				// Dispose old labels
				foreach(KeyValuePair<Sector, TextLabel[]> lbl in labels)
					foreach(TextLabel l in lbl.Value) l.Dispose();
			}

			// Make text labels for sectors
			labels = new Dictionary<Sector, TextLabel[]>(General.Map.Map.Sectors.Count);
			foreach(Sector s in General.Map.Map.Sectors)
			{
				// Setup labels
				TextLabel[] labelarray = new TextLabel[s.Labels.Count];
				for(int i = 0; i < s.Labels.Count; i++)
				{
					Vector2D v = s.Labels[i].position;
					labelarray[i] = new TextLabel(20);
					labelarray[i].TransformCoords = true;
					labelarray[i].Rectangle = new RectangleF(v.x, v.y, 0.0f, 0.0f);
					labelarray[i].AlignX = TextAlignmentX.Center;
					labelarray[i].AlignY = TextAlignmentY.Middle;
					labelarray[i].Scale = 14f;
					labelarray[i].Color = General.Colors.Highlight.WithAlpha(255);
					labelarray[i].Backcolor = General.Colors.Background.WithAlpha(255);
				}
				labels.Add(s, labelarray);
			}
		}

		// This updates the overlay
		private void UpdateOverlay()
		{
			if(renderer.StartOverlay(true))
			{
				if(BuilderPlug.Me.ViewSelectionNumbers)
				{
					// Go for all selected sectors
					ICollection<Sector> orderedselection = General.Map.Map.GetSelectedSectors(true);
					foreach(Sector s in orderedselection)
					{
						// Render labels
						TextLabel[] labelarray = labels[s];
						for(int i = 0; i < s.Labels.Count; i++)
						{
							TextLabel l = labelarray[i];

							// Render only when enough space for the label to see
							float requiredsize = (l.TextSize.Height / 2) / renderer.Scale;
							if(requiredsize < s.Labels[i].radius) renderer.RenderText(l);
						}
					}
				}
				
				renderer.Finish();
			}
		}
		
		// Support function for joining and merging sectors
		private void JoinMergeSectors(bool removelines)
		{
			// Remove lines in betwen joining sectors?
			if(removelines)
			{
				// Go for all selected linedefs
				List<Linedef> selectedlines = new List<Linedef>(General.Map.Map.GetSelectedLinedefs(true));
				foreach(Linedef ld in selectedlines)
				{
					// Front and back side?
					if((ld.Front != null) && (ld.Back != null))
					{
						// Both a selected sector, but not the same?
						if(ld.Front.Sector.Selected && ld.Back.Sector.Selected &&
						   (ld.Front.Sector != ld.Back.Sector))
						{
							// Remove this line
							ld.Dispose();
						}
					}
				}
			}

			// Find the first sector that is not disposed
			List<Sector> orderedselection = new List<Sector>(General.Map.Map.GetSelectedSectors(true));
			Sector first = null;
			foreach(Sector s in orderedselection)
				if(!s.IsDisposed) { first = s; break; }
			
			// Join all selected sectors with the first
			for(int i = 0; i < orderedselection.Count; i++)
				if((orderedselection[i] != first) && !orderedselection[i].IsDisposed)
					orderedselection[i].Join(first);

			// Clear selection
			General.Map.Map.ClearAllSelected();
			
			// Update
			General.Map.Map.Update();
			
			// Make text labels for sectors
			SetupLabels();
			UpdateSelectedLabels();
		}

		// This highlights a new item
		protected void Highlight(Sector s)
		{
			bool completeredraw = false;

			// Often we can get away by simply undrawing the previous
			// highlight and drawing the new highlight. But if associations
			// are or were drawn we need to redraw the entire display.

			// Previous association highlights something?
			if((highlighted != null) && (highlighted.Tag > 0)) completeredraw = true;

            // Set highlight association
            if (s != null)
            {
                highlightasso.Set(s.Tag, UniversalType.SectorTag);
                // portals
                if (General.Map.Config.UDMF)
                {
                    // floor
                    if (portalasso[0].type != 0)
                    {
                        completeredraw = true;
                    }
                    portalasso[0].Set(0, 0);
                    if (s != null
                        && s.Fields.ContainsKey("portalfloor")
                        && s.Fields["portalfloor"].Type == (int)UniversalType.PortalTag)
                    {

                       portalasso[0].Set(Math.Abs((int)s.Fields["portalfloor"].Value), (int)UniversalType.PortalTag);
                    }
                    if (portalasso[0].type != 0)
                    {
                        completeredraw = true;
                    }

                    // ceil
                    if (portalasso[1].type != 0)
                    {
                        completeredraw = true;
                    }
                    portalasso[1].Set(0, 0);
                    if (s != null
                        && s.Fields.ContainsKey("portalceiling")
                        && s.Fields["portalceiling"].Type == (int)UniversalType.PortalTag)
                    {

                        portalasso[1].Set(Math.Abs((int)s.Fields["portalceiling"].Value), (int)UniversalType.PortalTag);
                    }
                    if (portalasso[1].type != 0)
                    {
                        completeredraw = true;
                    }
                } // portals
            }
            else
            {
                highlightasso.Set(0, 0);
                for (int i = 0; i < NUM_PORTAL_ASSOCIATIONS; i++)
                {
                    if (portalasso[i].type != 0)
                    {
                        completeredraw = true;
                    }
                    portalasso[i].Set(0, 0);
                }
            }

            // New association highlights something?
            if ((s != null) && (s.Tag > 0)) completeredraw = true;

			// Change label color
			if((highlighted != null) && !highlighted.IsDisposed)
			{
				TextLabel[] labelarray = labels[highlighted];
				foreach(TextLabel l in labelarray) l.Color = General.Colors.Selection;
			}
			
			// Change label color
			if((s != null) && !s.IsDisposed)
			{
				TextLabel[] labelarray = labels[s];
				foreach(TextLabel l in labelarray) l.Color = General.Colors.Highlight;
			}
			
			// If we're changing associations, then we
			// need to redraw the entire display
			if(completeredraw)
			{
				// Set new highlight and redraw completely
				highlighted = s;
				General.Interface.RedrawDisplay();
			}
			else
			{
				// Update display
				if(renderer.StartPlotter(false))
				{
					// Undraw previous highlight
					if((highlighted != null) && !highlighted.IsDisposed)
						renderer.PlotSector(highlighted);
					
					/*
					// Undraw highlighted things
					if(highlighted != null)
						foreach(Thing t in highlighted.Things)
							renderer.RenderThing(t, renderer.DetermineThingColor(t));
					*/

					// Set new highlight
					highlighted = s;

					// Render highlighted item
					if((highlighted != null) && !highlighted.IsDisposed)
						renderer.PlotSector(highlighted, General.Colors.Highlight);
					
					/*
					// Render highlighted things
					if(highlighted != null)
						foreach(Thing t in highlighted.Things)
							renderer.RenderThing(t, General.Colors.Highlight);
					*/

					// Done
					renderer.Finish();
				}
				
				UpdateOverlay();
				renderer.Present();
			}

			// Show highlight info
			if((highlighted != null) && !highlighted.IsDisposed)
				General.Interface.ShowSectorInfo(highlighted);
			else
				General.Interface.HideInfo();
		}

		// This selectes or deselects a sector
		protected void SelectSector(Sector s, bool selectstate, bool update)
		{
			bool selectionchanged = false;

			if(!s.IsDisposed)
			{
				// Select the sector?
				if(selectstate && !s.Selected)
				{
					s.Selected = true;
					selectionchanged = true;
					
					// Setup labels
					ICollection<Sector> orderedselection = General.Map.Map.GetSelectedSectors(true);
					TextLabel[] labelarray = labels[s];
					foreach(TextLabel l in labelarray)
					{
						l.Text = orderedselection.Count.ToString();
						l.Color = General.Colors.Selection;
					}
				}
				// Deselect the sector?
				else if(!selectstate && s.Selected)
				{
					s.Selected = false;
					selectionchanged = true;

					// Clear labels
					TextLabel[] labelarray = labels[s];
					foreach(TextLabel l in labelarray) l.Text = "";

					// Update all other labels
					UpdateSelectedLabels();
				}

				// Selection changed?
				if(selectionchanged)
				{
					// Make update lines selection
					foreach(Sidedef sd in s.Sidedefs)
					{
						bool front, back;
						if(sd.Line.Front != null) front = sd.Line.Front.Sector.Selected; else front = false;
						if(sd.Line.Back != null) back = sd.Line.Back.Sector.Selected; else back = false;
						sd.Line.Selected = front | back;
					}
				}

				if(update)
				{
					UpdateOverlay();
					renderer.Present();
				}
			}
		}

		// This updates labels from the selected sectors
		private void UpdateSelectedLabels()
		{
			// Go for all labels in all selected sectors
			ICollection<Sector> orderedselection = General.Map.Map.GetSelectedSectors(true);
			int index = 0;
			foreach(Sector s in orderedselection)
			{
				TextLabel[] labelarray = labels[s];
				foreach(TextLabel l in labelarray)
				{
					// Make sure the text and color are right
					int labelnum = index + 1;
					l.Text = labelnum.ToString();
					l.Color = General.Colors.Selection;
				}
				index++;
			}
		}

		#endregion
		
		#region ================== Events

		public override void OnHelp()
		{
			General.ShowHelp("e_sectors.html");
		}

		// Cancel mode
		public override void OnCancel()
		{
			base.OnCancel();

			// Return to this mode
			General.Editing.ChangeMode(new SectorsMode());
		}

		// Mode engages
		public override void OnEngage()
		{
			base.OnEngage();
			renderer.SetPresentation(Presentation.Standard);

			// Add toolbar buttons
			General.Interface.AddButton(BuilderPlug.Me.MenusForm.CopyProperties);
			General.Interface.AddButton(BuilderPlug.Me.MenusForm.PasteProperties);
			General.Interface.AddButton(BuilderPlug.Me.MenusForm.SeparatorCopyPaste);
			General.Interface.AddButton(BuilderPlug.Me.MenusForm.ViewSelectionNumbers);
			General.Interface.AddButton(BuilderPlug.Me.MenusForm.SeparatorSectors1);
			General.Interface.AddButton(BuilderPlug.Me.MenusForm.MakeGradientBrightness);
			General.Interface.AddButton(BuilderPlug.Me.MenusForm.MakeGradientFloors);
			General.Interface.AddButton(BuilderPlug.Me.MenusForm.MakeGradientCeilings);
			
			// Convert geometry selection to sectors only
			General.Map.Map.ConvertSelection(SelectionType.Sectors);

			// Make text labels for sectors
			SetupLabels();
			
			// Update
			UpdateSelectedLabels();
			UpdateOverlay();
		}
		
		// Mode disengages
		public override void OnDisengage()
		{
			base.OnDisengage();

			// Remove toolbar buttons
			General.Interface.RemoveButton(BuilderPlug.Me.MenusForm.CopyProperties);
			General.Interface.RemoveButton(BuilderPlug.Me.MenusForm.PasteProperties);
			General.Interface.RemoveButton(BuilderPlug.Me.MenusForm.SeparatorCopyPaste);
			General.Interface.RemoveButton(BuilderPlug.Me.MenusForm.ViewSelectionNumbers);
			General.Interface.RemoveButton(BuilderPlug.Me.MenusForm.SeparatorSectors1);
			General.Interface.RemoveButton(BuilderPlug.Me.MenusForm.MakeGradientBrightness);
			General.Interface.RemoveButton(BuilderPlug.Me.MenusForm.MakeGradientFloors);
			General.Interface.RemoveButton(BuilderPlug.Me.MenusForm.MakeGradientCeilings);
			
			// Keep only sectors selected
			General.Map.Map.ClearSelectedLinedefs();
			
			// Going to EditSelectionMode?
			if(General.Editing.NewMode is EditSelectionMode)
			{
				// Not pasting anything?
				EditSelectionMode editmode = (General.Editing.NewMode as EditSelectionMode);
				if(!editmode.Pasting)
				{
					// No selection made? But we have a highlight!
					if((General.Map.Map.GetSelectedSectors(true).Count == 0) && (highlighted != null))
					{
						// Make the highlight the selection
						SelectSector(highlighted, true, false);
					}
				}
			}
			
			// Hide highlight info
			General.Interface.HideInfo();
		}

		// This redraws the display
		public override void OnRedrawDisplay()
		{
			renderer.RedrawSurface();
			
			// Render lines and vertices
			if(renderer.StartPlotter(true))
			{
				renderer.PlotLinedefSet(General.Map.Map.Linedefs);
				renderer.PlotVerticesSet(General.Map.Map.Vertices);
				if((highlighted != null) && !highlighted.IsDisposed)
                {
                    BuilderPlug.Me.PlotAssociations(renderer, portalasso[0]);
                    BuilderPlug.Me.PlotAssociations(renderer, portalasso[1]);
                    renderer.PlotSector(highlighted, General.Colors.Highlight);
					BuilderPlug.Me.PlotReverseAssociations(renderer, highlightasso);
                }
				renderer.Finish();
			}

			// Render things
			if(renderer.StartThings(true))
			{
				renderer.RenderThingSet(General.Map.ThingsFilter.HiddenThings, Presentation.THINGS_HIDDEN_ALPHA);
				renderer.RenderThingSet(General.Map.ThingsFilter.VisibleThings, 1.0f);
				renderer.Finish();
			}

			// Render selection
			if(renderer.StartOverlay(true))
			{
				if((highlighted != null) && !highlighted.IsDisposed) BuilderPlug.Me.RenderReverseAssociations(renderer, highlightasso);
				if(selecting) RenderMultiSelection();
				renderer.Finish();
			}

			// Render overlay
			UpdateOverlay();
			
			renderer.Present();
		}

		// Selection
		protected override void OnSelectBegin()
		{
			// Item highlighted?
			if((highlighted != null) && !highlighted.IsDisposed)
			{
				// Flip selection
				SelectSector(highlighted, !highlighted.Selected, true);

				// Update display
				if(renderer.StartPlotter(false))
				{
					// Redraw highlight to show selection
					renderer.PlotSector(highlighted);
					renderer.Finish();
					renderer.Present();
				}
			}
			else
			{
				// Start making a selection
				StartMultiSelection();
			}

			base.OnSelectBegin();
		}

		// End selection
		protected override void OnSelectEnd()
		{
			// Not stopping from multiselection?
			if(!selecting)
			{
				// Item highlighted?
				if((highlighted != null) && !highlighted.IsDisposed)
				{
					// Update display
					if(renderer.StartPlotter(false))
					{
						// Render highlighted item
						renderer.PlotSector(highlighted, General.Colors.Highlight);
						renderer.Finish();
						renderer.Present();
					}

					// Update overlay
					TextLabel[] labelarray = labels[highlighted];
					foreach(TextLabel l in labelarray) l.Color = General.Colors.Highlight;
					UpdateOverlay();
					renderer.Present();
				}
			}

			base.OnSelectEnd();
		}

		// Start editing
		protected override void OnEditBegin()
		{
			// Item highlighted?
			if((highlighted != null) && !highlighted.IsDisposed)
			{
				// Edit pressed in this mode
				editpressed = true;

				// Highlighted item not selected?
				if(!highlighted.Selected && (BuilderPlug.Me.AutoClearSelection || (General.Map.Map.SelectedSectorsCount == 0)))
				{
					// Make this the only selection
					General.Map.Map.ClearSelectedSectors();
					General.Map.Map.ClearSelectedLinedefs();
					SelectSector(highlighted, true, false);
					General.Interface.RedrawDisplay();
				}

				// Update display
				if(renderer.StartPlotter(false))
				{
					// Redraw highlight to show selection
					renderer.PlotSector(highlighted);
					renderer.Finish();
					renderer.Present();
				}
			}
			else
			{
				// Start drawing mode
				DrawGeometryMode drawmode = new DrawGeometryMode();
				bool snaptogrid = General.Interface.ShiftState ^ General.Interface.SnapToGrid;
				bool snaptonearest = General.Interface.CtrlState ^ General.Interface.AutoMerge;
				DrawnVertex v = DrawGeometryMode.GetCurrentPosition(mousemappos, snaptonearest, snaptogrid, renderer, new List<DrawnVertex>());
				
				if(drawmode.DrawPointAt(v))
					General.Editing.ChangeMode(drawmode);
				else
					General.Interface.DisplayStatus(StatusType.Warning, "Failed to draw point: outside of map boundaries.");
			}
			
			base.OnEditBegin();
		}

		// Done editing
		protected override void OnEditEnd()
		{
			// Edit pressed in this mode?
			if(editpressed)
			{
				// Anything selected?
				ICollection<Sector> selected = General.Map.Map.GetSelectedSectors(true);
				if(selected.Count > 0)
				{
					if(General.Interface.IsActiveWindow)
					{
						// Show sector edit dialog
						General.Interface.ShowEditSectors(selected);
						General.Map.Map.Update();
						
						// When a single sector was selected, deselect it now
						if(selected.Count == 1)
						{
							General.Map.Map.ClearSelectedSectors();
							General.Map.Map.ClearSelectedLinedefs();
						}

						// Update entire display
						General.Interface.RedrawDisplay();
					}
				}
			}

			editpressed = false;
			base.OnEditEnd();
		}
		
		// Mouse moves
		public override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);

			// Not holding any buttons?
			if(e.Button == MouseButtons.None)
			{
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
		}

		// Mouse leaves
		public override void OnMouseLeave(EventArgs e)
		{
			base.OnMouseLeave(e);

			// Highlight nothing
			Highlight(null);
		}

		// Mouse wants to drag
		protected override void OnDragStart(MouseEventArgs e)
		{
			base.OnDragStart(e);

			// Edit button used?
			if(General.Actions.CheckActionActive(null, "classicedit"))
			{
				// Anything highlighted?
				if((highlighted != null) && !highlighted.IsDisposed)
				{
					// Highlighted item not selected?
					if(!highlighted.Selected)
					{
						// Select only this sector for dragging
						General.Map.Map.ClearSelectedSectors();
						SelectSector(highlighted, true, true);
					}

					// Start dragging the selection
					General.Editing.ChangeMode(new DragSectorsMode(mousedownmappos));
				}
			}
		}

		// This is called wheh selection ends
		protected override void OnEndMultiSelection()
		{
			bool selectionvolume = ((Math.Abs(base.selectionrect.Width) > 0.1f) && (Math.Abs(base.selectionrect.Height) > 0.1f));

			if(BuilderPlug.Me.AutoClearSelection && !selectionvolume)
			{
				General.Map.Map.ClearSelectedLinedefs();
				General.Map.Map.ClearSelectedSectors();
			}

			if(selectionvolume)
			{
                if (General.Interface.AltState)
                {
                    // ano - subtractive select!
                    foreach (Linedef l in General.Map.Map.Linedefs)
                    {
                        if ((l.Start.Position.x >= selectionrect.Left) &&
                                       (l.Start.Position.y >= selectionrect.Top) &&
                                       (l.Start.Position.x <= selectionrect.Right) &&
                                       (l.Start.Position.y <= selectionrect.Bottom) &&
                                       (l.End.Position.x >= selectionrect.Left) &&
                                       (l.End.Position.y >= selectionrect.Top) &&
                                       (l.End.Position.x <= selectionrect.Right) &&
                                       (l.End.Position.y <= selectionrect.Bottom))
                        {
                            l.Selected = false;
                        }
                    }
                }
                else if (General.Interface.ShiftState ^ BuilderPlug.Me.AdditiveSelect)
				{
					// Go for all lines
					foreach(Linedef l in General.Map.Map.Linedefs)
					{
						l.Selected |= ((l.Start.Position.x >= selectionrect.Left) &&
									   (l.Start.Position.y >= selectionrect.Top) &&
									   (l.Start.Position.x <= selectionrect.Right) &&
									   (l.Start.Position.y <= selectionrect.Bottom) &&
									   (l.End.Position.x >= selectionrect.Left) &&
									   (l.End.Position.y >= selectionrect.Top) &&
									   (l.End.Position.x <= selectionrect.Right) &&
									   (l.End.Position.y <= selectionrect.Bottom));
					}
				}
				else
				{
					// Go for all lines
					foreach(Linedef l in General.Map.Map.Linedefs)
					{
						l.Selected = ((l.Start.Position.x >= selectionrect.Left) &&
									  (l.Start.Position.y >= selectionrect.Top) &&
									  (l.Start.Position.x <= selectionrect.Right) &&
									  (l.Start.Position.y <= selectionrect.Bottom) &&
									  (l.End.Position.x >= selectionrect.Left) &&
									  (l.End.Position.y >= selectionrect.Top) &&
									  (l.End.Position.x <= selectionrect.Right) &&
									  (l.End.Position.y <= selectionrect.Bottom));
					}
				}
				
				// Go for all sectors
				foreach(Sector s in General.Map.Map.Sectors)
				{
					// Go for all sidedefs
					bool allselected = true;
					foreach(Sidedef sd in s.Sidedefs)
					{
						if(!sd.Line.Selected)
						{
							allselected = false;
							break;
						}
					}
					
					// Sector completely selected?
					SelectSector(s, allselected, false);
				}
				
				// Make sure all linedefs reflect selected sectors
				foreach(Sidedef sd in General.Map.Map.Sidedefs)
					if(!sd.Sector.Selected && ((sd.Other == null) || !sd.Other.Sector.Selected))
						sd.Line.Selected = false;
			}
			
			base.OnEndMultiSelection();
			if(renderer.StartOverlay(true)) renderer.Finish();
			General.Interface.RedrawDisplay();
		}

		// This is called when the selection is updated
		protected override void OnUpdateMultiSelection()
		{
			base.OnUpdateMultiSelection();

			// Render selection
			if(renderer.StartOverlay(true))
			{
				RenderMultiSelection();
				renderer.Finish();
				renderer.Present();
			}
		}

		// When copying
		public override bool OnCopyBegin()
		{
			// No selection made? But we have a highlight!
			if((General.Map.Map.GetSelectedSectors(true).Count == 0) && (highlighted != null))
			{
				// Make the highlight the selection
				SelectSector(highlighted, true, true);
			}

			return base.OnCopyBegin();
		}

		// When undo is used
		public override bool OnUndoBegin()
		{
			// Clear ordered selection
			General.Map.Map.ClearAllSelected();

			return base.OnUndoBegin();
		}

		// When undo is performed
		public override void OnUndoEnd()
		{
			// Clear labels
			SetupLabels();
		}
		
		// When redo is used
		public override bool OnRedoBegin()
		{
			// Clear ordered selection
			General.Map.Map.ClearAllSelected();

			return base.OnRedoBegin();
		}

		// When redo is performed
		public override void OnRedoEnd()
		{
			// Clear labels
			SetupLabels();
		}
		
		#endregion

		#region ================== Actions

		// This copies the properties
		[BeginAction("classiccopyproperties")]
		public void CopyProperties()
		{
			// Determine source sectors
			ICollection<Sector> sel = null;
			if(General.Map.Map.SelectedSectorsCount > 0)
				sel = General.Map.Map.GetSelectedSectors(true);
			else if(highlighted != null)
			{
				sel = new List<Sector>();
				sel.Add(highlighted);
			}
			
			if(sel != null)
			{
				// Copy properties from first source sectors
				BuilderPlug.Me.CopiedSectorProps = new SectorProperties(General.GetByIndex(sel, 0));
				General.Interface.DisplayStatus(StatusType.Action, "Copied sector properties.");
			}
		}

		// This pastes the properties
		[BeginAction("classicpasteproperties")]
		public void PasteProperties()
		{
			if(BuilderPlug.Me.CopiedSectorProps != null)
			{
				// Determine target sectors
				ICollection<Sector> sel = null;
				if(General.Map.Map.SelectedSectorsCount > 0)
					sel = General.Map.Map.GetSelectedSectors(true);
				else if(highlighted != null)
				{
					sel = new List<Sector>();
					sel.Add(highlighted);
				}
				
				if(sel != null)
				{
					// Apply properties to selection
					General.Map.UndoRedo.CreateUndo("Paste sector properties");
					foreach(Sector s in sel)
					{
						BuilderPlug.Me.CopiedSectorProps.Apply(s);
						s.UpdateCeilingSurface();
						s.UpdateFloorSurface();
					}
					General.Interface.DisplayStatus(StatusType.Action, "Pasted sector properties.");
					
					// Update and redraw
					General.Map.IsChanged = true;
					General.Interface.RefreshInfo();
					General.Interface.RedrawDisplay();
				}
			}
		}

		// This creates a new vertex at the mouse position
		[BeginAction("insertitem", BaseAction = true)]
		public virtual void InsertVertexAction()
		{
			// Start drawing mode
			DrawGeometryMode drawmode = new DrawGeometryMode();
			if(mouseinside)
			{
				bool snaptogrid = General.Interface.ShiftState ^ General.Interface.SnapToGrid;
				bool snaptonearest = General.Interface.CtrlState ^ General.Interface.AutoMerge;
				DrawnVertex v = DrawGeometryMode.GetCurrentPosition(mousemappos, snaptonearest, snaptogrid, renderer, new List<DrawnVertex>());
				drawmode.DrawPointAt(v);
			}
			General.Editing.ChangeMode(drawmode);
		}

		[BeginAction("makedoor")]
		public void MakeDoor()
		{
			// Highlighted item not selected?
			if((highlighted != null) && !highlighted.Selected)
			{
				// Make this the only selection
				General.Map.Map.ClearSelectedSectors();
				General.Map.Map.ClearSelectedLinedefs();
				SelectSector(highlighted, true, false);
				General.Interface.RedrawDisplay();
			}
			
			// Anything selected?
			ICollection<Sector> orderedselection = General.Map.Map.GetSelectedSectors(true);
			if(orderedselection.Count > 0)
			{
				string doortex = "";
				string tracktex = General.Map.Config.MakeDoorTrack;
				string floortex = null;
				string ceiltex = null;
				bool resetoffsets = true;
				
				// Find ceiling and floor textures
				foreach(Sector s in orderedselection)
				{
					if(floortex == null) floortex = s.FloorTexture; else if(floortex != s.FloorTexture) floortex = "";
					if(ceiltex == null) ceiltex = s.CeilTexture; else if(ceiltex != s.CeilTexture) ceiltex = "";
				}
				
				// Show the dialog
				MakeDoorForm form = new MakeDoorForm();
				if(form.Show(General.Interface, doortex, tracktex, ceiltex, floortex, resetoffsets) == DialogResult.OK)
				{
					doortex = form.DoorTexture;
					tracktex = form.TrackTexture;
					ceiltex = form.CeilingTexture;
					floortex = form.FloorTexture;
					resetoffsets = form.ResetOffsets;
					
					// Create undo
					General.Map.UndoRedo.CreateUndo("Make door (" + doortex + ")");
					General.Interface.DisplayStatus(StatusType.Action, "Created a " + doortex + " door.");
					
					// Go for all selected sectors
					foreach(Sector s in orderedselection)
					{
						// Lower the ceiling down to the floor
						s.CeilHeight = s.FloorHeight;

						// Make a unique tag (not sure if we need it yet, depends on the args)
						int tag = General.Map.Map.GetNewTag();

						// Go for all it's sidedefs
						foreach(Sidedef sd in s.Sidedefs)
						{
							// Singlesided?
							if(sd.Other == null)
							{
								// Make this a doortrak
								sd.SetTextureHigh("-");
								sd.SetTextureMid(tracktex);
								sd.SetTextureLow("-");

								// Set upper/lower unpegged flags
								sd.Line.SetFlag(General.Map.Config.UpperUnpeggedFlag, false);
								sd.Line.SetFlag(General.Map.Config.LowerUnpeggedFlag, true);
							}
							else
							{
								// Set textures
								if(floortex.Length > 0) s.SetFloorTexture(floortex);
								if(ceiltex.Length > 0) s.SetCeilTexture(ceiltex);
								if(doortex.Length > 0) sd.Other.SetTextureHigh(doortex);

								// Set upper/lower unpegged flags
								sd.Line.SetFlag(General.Map.Config.UpperUnpeggedFlag, false);
								sd.Line.SetFlag(General.Map.Config.LowerUnpeggedFlag, false);
								
								// Get door linedef type from config
								sd.Line.Action = General.Map.Config.MakeDoorAction;

								// Set activation type
								sd.Line.Activate = General.Map.Config.MakeDoorActivate;

								// Set the flags
								foreach(var flagpair in General.Map.Config.MakeDoorFlags)
									sd.Line.SetFlag(flagpair.Key, flagpair.Value);

								// Set the linedef args
								for(int i = 0; i < Linedef.NUM_ARGS; i++)
								{
									// A -1 arg indicates that the arg must be set to the new sector tag
									// and only in this case we set the tag on the sector, because only
									// then we know for sure that we need a tag.
									if(General.Map.Config.MakeDoorArgs[i] == -1)
									{
										sd.Line.Args[i] = tag;
										s.Tag = tag;
									}
									else
									{
										sd.Line.Args[i] = General.Map.Config.MakeDoorArgs[i];
									}
								}

								// Make sure the line is facing outwards
								if(sd.IsFront)
								{
									sd.Line.FlipVertices();
									sd.Line.FlipSidedefs();
								}
							}

							// Reset the texture offsets if required
							if (resetoffsets)
							{
								sd.OffsetX = 0;
								sd.OffsetY = 0;

								if (sd.Other != null)
								{
									sd.Other.OffsetX = 0;
									sd.Other.OffsetY = 0;
								}
							}
						}
					}
					
					// When a single sector was selected, deselect it now
					if(orderedselection.Count == 1)
					{
						orderedselection.Clear();
						General.Map.Map.ClearSelectedSectors();
						General.Map.Map.ClearSelectedLinedefs();
					}
				}
				
				// Done
				form.Dispose();
				General.Interface.RedrawDisplay();
			}
		}
		
		[BeginAction("deleteitem", BaseAction = true)]
		public void DeleteItem()
		{
			// Make list of selected sectors
			List<Sector> selected = new List<Sector>(General.Map.Map.GetSelectedSectors(true));
			if((selected.Count == 0) && (highlighted != null) && !highlighted.IsDisposed) selected.Add(highlighted);

			// Anything to do?
			if(selected.Count > 0)
			{
				// Make undo
				if(selected.Count > 1)
				{
					General.Map.UndoRedo.CreateUndo("Delete " + selected.Count + " sectors");
					General.Interface.DisplayStatus(StatusType.Action, "Deleted " + selected.Count + " sectors.");
				}
				else
				{
					General.Map.UndoRedo.CreateUndo("Delete sector");
					General.Interface.DisplayStatus(StatusType.Action, "Deleted sector.");
				}

				// Dispose selected sectors
				foreach(Sector s in selected)
				{
					// Get all the linedefs
					General.Map.Map.ClearMarkedLinedefs(false);
					foreach(Sidedef sd in s.Sidedefs) sd.Line.Marked = true;
					List<Linedef> lines = General.Map.Map.GetMarkedLinedefs(true);
					
					// Dispose the sector
					s.Dispose();

					// Check all the lines
					for(int i = lines.Count - 1; i >= 0; i--)
					{
						// If the line has become orphaned, remove it
						if((lines[i].Front == null) && (lines[i].Back == null))
						{
							// Remove line
							lines[i].Dispose();
						}
						else
						{
							// If the line only has a back side left, flip the line and sides
							if((lines[i].Front == null) && (lines[i].Back != null))
							{
								lines[i].FlipVertices();
								lines[i].FlipSidedefs();
							}
							
							// Update sided flags
							lines[i].ApplySidedFlags();
						}
					}
				}

				// Update cache values
				General.Map.IsChanged = true;
				General.Map.Map.Update();
				
				// Make text labels for sectors
				SetupLabels();
				UpdateSelectedLabels();
				
				// Redraw screen
				General.Interface.RedrawDisplay();
			}
		}
		
		// This joins sectors together and keeps all lines
		[BeginAction("joinsectors")]
		public void JoinSectors()
		{
			// Worth our money?
			int count = General.Map.Map.GetSelectedSectors(true).Count;
			if(count > 1)
			{
				// Make undo
				General.Map.UndoRedo.CreateUndo("Join " + count + " sectors");
				General.Interface.DisplayStatus(StatusType.Action, "Joined " + count + " sectors.");

				// Merge
				JoinMergeSectors(false);

				// Deselect
				General.Map.Map.ClearSelectedSectors();
				General.Map.Map.ClearSelectedLinedefs();
				General.Map.IsChanged = true;

				// Redraw display
				General.Interface.RedrawDisplay();
			}
		}

		// This joins sectors together and removes the lines in between
		[BeginAction("mergesectors")]
		public void MergeSectors()
		{
			// Worth our money?
			int count = General.Map.Map.GetSelectedSectors(true).Count;
			if(count > 1)
			{
				// Make undo
				General.Map.UndoRedo.CreateUndo("Merge " + count + " sectors");
				General.Interface.DisplayStatus(StatusType.Action, "Merged " + count + " sectors.");

				// Merge
				JoinMergeSectors(true);

				// Deselect
				General.Map.Map.ClearSelectedSectors();
				General.Map.Map.ClearSelectedLinedefs();
				General.Map.IsChanged = true;

				// Redraw display
				General.Interface.RedrawDisplay();
			}
		}

		// Make gradient brightness
		[BeginAction("gradientbrightness")]
		public void MakeGradientBrightness()
		{
			General.Interface.DisplayStatus(StatusType.Action, "Created gradient brightness over selected sectors.");
			General.Map.UndoRedo.CreateUndo("Gradient brightness");

			// Need at least 3 selected sectors
			// The first and last are not modified
			ICollection<Sector> orderedselection = General.Map.Map.GetSelectedSectors(true);
			if(orderedselection.Count > 2)
			{
				float startbrightness = (float)General.GetByIndex(orderedselection, 0).Brightness;
				float endbrightness = (float)General.GetByIndex(orderedselection, orderedselection.Count - 1).Brightness;
				float delta = endbrightness - startbrightness;

				// Go for all sectors in between first and last
				int index = 0;
				foreach(Sector s in orderedselection)
				{
					float u = (float)index / (float)(orderedselection.Count - 1);
					float b = startbrightness + delta * u;
					s.Brightness = (int)b;
					index++;
				}
			}

			// Update
			General.Map.Map.Update();
			UpdateOverlay();
			renderer.Present();
			General.Interface.RedrawDisplay();
			General.Interface.RefreshInfo();
			General.Map.IsChanged = true;
		}

		// Make gradient floors
		[BeginAction("gradientfloors")]
		public void MakeGradientFloors()
		{
			General.Interface.DisplayStatus(StatusType.Action, "Created gradient floor heights over selected sectors.");
			General.Map.UndoRedo.CreateUndo("Gradient floor heights");

			// Need at least 3 selected sectors
			// The first and last are not modified
			ICollection<Sector> orderedselection = General.Map.Map.GetSelectedSectors(true);
			if(orderedselection.Count > 2)
			{
				float startlevel = (float)General.GetByIndex(orderedselection, 0).FloorHeight;
				float endlevel = (float)General.GetByIndex(orderedselection, orderedselection.Count - 1).FloorHeight;
				float delta = endlevel - startlevel;

				// Go for all sectors in between first and last
				int index = 0;
				foreach(Sector s in orderedselection)
				{
					float u = (float)index / (float)(orderedselection.Count - 1);
					float b = startlevel + delta * u;
					s.FloorHeight = (int)b;
					index++;
				}
			}

			// Update
			General.Interface.RefreshInfo();
			General.Map.IsChanged = true;
		}

		// Make gradient ceilings
		[BeginAction("gradientceilings")]
		public void MakeGradientCeilings()
		{
			General.Interface.DisplayStatus(StatusType.Action, "Created gradient ceiling heights over selected sectors.");
			General.Map.UndoRedo.CreateUndo("Gradient ceiling heights");

			// Need at least 3 selected sectors
			// The first and last are not modified
			ICollection<Sector> orderedselection = General.Map.Map.GetSelectedSectors(true);
			if(orderedselection.Count > 2)
			{
				float startlevel = (float)General.GetByIndex(orderedselection, 0).CeilHeight;
				float endlevel = (float)General.GetByIndex(orderedselection, orderedselection.Count - 1).CeilHeight;
				float delta = endlevel - startlevel;

				// Go for all sectors in between first and last
				int index = 0;
				foreach(Sector s in orderedselection)
				{
					float u = (float)index / (float)(orderedselection.Count - 1);
					float b = startlevel + delta * u;
					s.CeilHeight = (int)b;
					index++;
				}
			}

			// Update
			General.Interface.RefreshInfo();
			General.Map.IsChanged = true;
		}
		
		// Change heights
		[BeginAction("lowerfloor8")]
		public void LowerFloors8()
		{
			General.Interface.DisplayStatus(StatusType.Action, "Lowered floor heights by 8mp.");
			General.Map.UndoRedo.CreateUndo("Floor heights change", this, UndoGroup.FloorHeightChange, CreateSelectionCRC());

			// Change heights
			ICollection<Sector> selected = General.Map.Map.GetSelectedSectors(true);
			if((selected.Count == 0) && (highlighted != null) && !highlighted.IsDisposed) selected.Add(highlighted);
			foreach(Sector s in selected)
			{
				s.FloorHeight -= 8;
			}
			
			// Update
			General.Interface.RefreshInfo();
			General.Map.IsChanged = true;
		}

		// Change heights
		[BeginAction("raisefloor8")]
		public void RaiseFloors8()
		{
			General.Interface.DisplayStatus(StatusType.Action, "Raised floor heights by 8mp.");
			General.Map.UndoRedo.CreateUndo("Floor heights change", this, UndoGroup.FloorHeightChange, CreateSelectionCRC());

			// Change heights
			ICollection<Sector> selected = General.Map.Map.GetSelectedSectors(true);
			if((selected.Count == 0) && (highlighted != null) && !highlighted.IsDisposed) selected.Add(highlighted);
			foreach(Sector s in selected)
			{
				s.FloorHeight += 8;
			}

			// Update
			General.Interface.RefreshInfo();
			General.Map.IsChanged = true;
		}

		// Change heights
		[BeginAction("lowerceiling8")]
		public void LowerCeilings8()
		{
			General.Interface.DisplayStatus(StatusType.Action, "Lowered ceiling heights by 8mp.");
			General.Map.UndoRedo.CreateUndo("Ceiling heights change", this, UndoGroup.CeilingHeightChange, CreateSelectionCRC());

			// Change heights
			ICollection<Sector> selected = General.Map.Map.GetSelectedSectors(true);
			if((selected.Count == 0) && (highlighted != null) && !highlighted.IsDisposed) selected.Add(highlighted);
			foreach(Sector s in selected)
			{
				s.CeilHeight -= 8;
			}

			// Update
			General.Interface.RefreshInfo();
			General.Map.IsChanged = true;
		}

		// Change heights
		[BeginAction("raiseceiling8")]
		public void RaiseCeilings8()
		{
			General.Interface.DisplayStatus(StatusType.Action, "Raised ceiling heights by 8mp.");
			General.Map.UndoRedo.CreateUndo("Ceiling heights change", this, UndoGroup.CeilingHeightChange, CreateSelectionCRC());

			// Change heights
			ICollection<Sector> selected = General.Map.Map.GetSelectedSectors(true);
			if((selected.Count == 0) && (highlighted != null) && !highlighted.IsDisposed) selected.Add(highlighted);
			foreach(Sector s in selected)
			{
				s.CeilHeight += 8;
			}

			// Update
			General.Interface.RefreshInfo();
			General.Map.IsChanged = true;
		}

        // ano - increment tags
        [BeginAction("incrementtag", BaseAction = true)]
        public void IncrementTags()
        {
            General.Interface.DisplayStatus(StatusType.Action, "Incremented sector tag(s).");
            General.Map.UndoRedo.CreateUndo("Increment sector tag(s)");

            // Change heights
            ICollection<Sector> selected = General.Map.Map.GetSelectedSectors(true);
            if ((selected.Count == 0) && (highlighted != null) && !highlighted.IsDisposed) selected.Add(highlighted);
            foreach (Sector s in selected)
            {
                s.Tag++;
            }

            // Update
            General.Interface.RefreshInfo();
            General.Map.IsChanged = true;
        }

        // ano - increment tags
        [BeginAction("decrementtag", BaseAction = true)]
        public void DecrementTags()
        {
            General.Interface.DisplayStatus(StatusType.Action, "Decremented sector tag(s).");
            General.Map.UndoRedo.CreateUndo("Decrement sector tag(s)");

            // Change heights
            ICollection<Sector> selected = General.Map.Map.GetSelectedSectors(true);
            if ((selected.Count == 0) && (highlighted != null) && !highlighted.IsDisposed) selected.Add(highlighted);
            foreach (Sector s in selected)
            {
                if (s.Tag > 0)
                {
                    s.Tag--;
                }
            }

            // Update
            General.Interface.RefreshInfo();
            General.Map.IsChanged = true;
        }

        // This clears the selection
        [BeginAction("clearselection", BaseAction = true)]
		public void ClearSelection()
		{
			// Clear selection
			General.Map.Map.ClearAllSelected();

			// Clear labels
			foreach(TextLabel[] labelarray in labels.Values)
				foreach(TextLabel l in labelarray) l.Text = "";
			
			// Redraw
			General.Interface.RedrawDisplay();
		}
		
		#endregion
	}
}
