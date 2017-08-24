using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using CodeImp.DoomBuilder.Windows;
using CodeImp.DoomBuilder.Types;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Controls;

namespace CodeImp.DoomBuilder.TagRange
{
	public partial class TagRangeForm : DelayedForm
	{
		private UniversalType selectiontype;
		private int selectioncount;
		Dictionary<int, bool> usedtags;
		
		public int SelectionCount { get { return selectioncount; } }
		
		// Constructor
		public TagRangeForm()
		{
			InitializeComponent();
		}
		
		// This sets up the form
		public void Setup()
		{
			General.Map.Map.ClearAllMarks(false);
			
			string modename = General.Editing.Mode.GetType().Name;
			if(modename == "SectorsMode")
			{
				selectiontype = UniversalType.SectorTag;
				ICollection<Sector> list = General.Map.Map.GetSelectedSectors(true);
				selectioncount = list.Count;
			}
			else if(modename == "LinedefsMode")
			{
				selectiontype = UniversalType.LinedefTag;
				ICollection<Linedef> list = General.Map.Map.GetSelectedLinedefs(true);
				selectioncount = list.Count;
			}
			else if(modename == "ThingsMode")
			{
				selectiontype = UniversalType.ThingTag;
				ICollection<Thing> list = General.Map.Map.GetSelectedThings(true);
				selectioncount = list.Count;
			}
			
			// Show number of selected items
			itemcountlabel.Text = selectioncount.ToString();
			
			// Find out which tags are used
			usedtags = new Dictionary<int, bool>();
			General.Map.Map.ForAllTags(TagHandler, false, usedtags);
			
			// Find the first unused tag to use as range start
			int starttag = General.Map.Map.GetNewTag();
			rangestart.Text = starttag.ToString();
		}
		
		// Handler for finding a new tag
		private void TagHandler(MapElement element, bool actionargument, UniversalType type, ref int value, Dictionary<int, bool> usedtags)
		{
			usedtags[value] = true;
		}
		
		// This creates a range
		private List<int> CreateRange(int starttag, bool skipusedtags, out bool tagsused, out bool outoftags)
		{
			List<int> newtags = new List<int>(selectioncount);
			outoftags = false;
			tagsused = false;
			
			// Go for the number of tags we need
			for(int i = 0; i < selectioncount; i++)
			{
				if(starttag > General.Map.FormatInterface.MaxTag)
				{
					outoftags = true;
					return newtags;
				}
					
				if(skipusedtags)
				{
					// Find next unused tag
					while(usedtags.ContainsKey(starttag))
					{
						if(starttag == General.Map.FormatInterface.MaxTag)
						{
							outoftags = true;
							return newtags;
						}
						
						starttag++;
					}
				}
				else
				{
					tagsused |= usedtags.ContainsKey(starttag);
				}
				
				newtags.Add(starttag);
				
				if(starttag < General.Map.FormatInterface.MaxTag)
					starttag++;
			}
			
			return newtags;
		}
		
		// This updates the calculated range
		private void UpdateChanges()
		{
			bool outoftags, tagsused;
			int starttag = rangestart.GetResult(0);
			
			List<int> tags = CreateRange(starttag, false, out tagsused, out outoftags);
			
			outoftagswarning.Visible = outoftags;
			doubletagwarning.Visible = tagsused && !outoftags;
			skipdoubletags.Visible = tagsused && !outoftags;
			skipdoubletags.BringToFront();
			
			tags = CreateRange(starttag, skipdoubletags.Checked, out tagsused, out outoftags);
			
			if(tags.Count > 0)
				endtaglabel.Text = tags[tags.Count - 1].ToString();
		}
		
		// Range start changes
		private void rangestart_WhenTextChanged(object sender, EventArgs e)
		{
			UpdateChanges();
		}
		
		// OK clicked
		private void okbutton_Click(object sender, EventArgs e)
		{
			bool outoftags, tagsused;
			int starttag = rangestart.GetResult(0);
			
			List<int> tags = CreateRange(starttag, skipdoubletags.Checked, out tagsused, out outoftags);
			
			if(outoftags)
			{
				General.ShowWarningMessage("The range exceeds the maximum allowed tags and cannot be created.", MessageBoxButtons.OK);
			}
			else
			{
				// Apply tags!
				if(selectiontype == UniversalType.SectorTag)
				{
					General.Map.UndoRedo.CreateUndo("Set " + selectioncount + " sector tags");
					ICollection<Sector> list = General.Map.Map.GetSelectedSectors(true);
					int index = 0;
					foreach(Sector s in list)
						s.Tag = tags[index++];
				}
				else if(selectiontype == UniversalType.LinedefTag)
				{
					General.Map.UndoRedo.CreateUndo("Set " + selectioncount + " linedef tags");
					ICollection<Linedef> list = General.Map.Map.GetSelectedLinedefs(true);
					int index = 0;
					foreach(Linedef l in list)
						l.Tag = tags[index++];
				}
				else if(selectiontype == UniversalType.ThingTag)
				{
					General.Map.UndoRedo.CreateUndo("Set " + selectioncount + " thing tags");
					ICollection<Thing> list = General.Map.Map.GetSelectedThings(true);
					int index = 0;
					foreach(Thing t in list)
						t.Tag = tags[index++];
				}
			}
			
			this.Close();
		}
		
		// Cancel clicked
		private void cancelbutton_Click(object sender, EventArgs e)
		{
			this.Close();
		}
		
		// Skip over used tags changed
		private void skipdoubletags_CheckedChanged(object sender, EventArgs e)
		{
			UpdateChanges();
		}
	}
}

