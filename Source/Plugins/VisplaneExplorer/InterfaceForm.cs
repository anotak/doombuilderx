#region === Copyright (c) 2010 Pascal van der Heiden ===

using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Text;
using System.ComponentModel;
using System.Windows.Forms;
using CodeImp.DoomBuilder.Windows;

#endregion

namespace CodeImp.DoomBuilder.Plugins.VisplaneExplorer
{
	public partial class InterfaceForm : Form
	{
		#region ================== Constants

		#endregion

		#region ================== Variables

		private ViewStats viewstats;
		Point oldttposition;

		#endregion

		#region ================== Properties

		internal ViewStats ViewStats { get { return viewstats; } }

		#endregion

		#region ================== Constructor / Destructor

		// Constructor
		public InterfaceForm()
		{
			InitializeComponent();
		}

		#endregion

		#region ================== Methods

		// This adds the buttons to the toolbar
		public void AddToInterface()
		{
			General.Interface.AddButton(statsbutton, ToolbarSection.Custom);
		}

		// This removes the buttons from the toolbar
		public void RemoveFromInterface()
		{
			General.Interface.RemoveButton(statsbutton);
		}

		// This shows a tooltip
		public void ShowTooltip(string text, Point p)
		{
			Point sp = General.Interface.Display.PointToScreen(p);
			Point fp = (General.Interface as Form).Location;
			Point tp = new Point(sp.X - fp.X, sp.Y - fp.Y);

			if (oldttposition != tp)
			{
				tooltip.Show(text, General.Interface, tp);
				oldttposition = tp;
			}
		}

		// This hides the tooltip
		public void HideTooltip()
		{
			tooltip.Hide(General.Interface);
		}

		#endregion

		#region ================== Events

		// Selecting a type of stats to view
		private void stats_Click(object sender, EventArgs e)
		{
			foreach(ToolStripMenuItem i in statsbutton.DropDownItems)
				i.Checked = false;
			
			ToolStripMenuItem item = (ToolStripMenuItem)sender;
			viewstats = (ViewStats)int.Parse(item.Tag.ToString(), CultureInfo.InvariantCulture);
			item.Checked = true;
			statsbutton.Image = item.Image;

			General.Interface.RedrawDisplay();
		}

		#endregion
	}
}
