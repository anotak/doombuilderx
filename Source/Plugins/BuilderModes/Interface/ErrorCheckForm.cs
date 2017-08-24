
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
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32;
using CodeImp.DoomBuilder.Actions;
using CodeImp.DoomBuilder.Data;
using CodeImp.DoomBuilder.Config;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Controls;
using CodeImp.DoomBuilder.Windows;
using System.Reflection;
using System.Globalization;
using System.Threading;

#endregion

namespace CodeImp.DoomBuilder.BuilderModes
{
	public partial class ErrorCheckForm : DelayedForm
	{
		#region ================== Constants

		#endregion

		#region ================== Delegates

		private delegate void CallVoidMethodDeletage();
		private delegate void CallIntMethodDelegate(int i);
		private delegate void CallResultMethodDelegate(ErrorResult r);
		
		#endregion
		
		#region ================== Variables
		
		private volatile bool running = false;
		private Thread checksthread;
		private BlockMap<BlockEntry> blockmap;
		
		#endregion

		#region ================== Properties
		
		public ErrorResult SelectedResult { get { return results.SelectedItem as ErrorResult; } }
		public BlockMap<BlockEntry> BlockMap { get { return blockmap; } }
		
		#endregion
		
		#region ================== Constructor / Show

		// Constructor
		public ErrorCheckForm()
		{
			// Initialize
			InitializeComponent();
			
			// Find all error checkers
			Type[] checkertypes = BuilderPlug.Me.FindClasses(typeof(ErrorChecker));
			foreach(Type t in checkertypes)
			{
				object[] attr = t.GetCustomAttributes(typeof(ErrorCheckerAttribute), true);
				if(attr.Length > 0)
				{
					ErrorCheckerAttribute checkerattr = (attr[0] as ErrorCheckerAttribute);
					
					// Add the type to the checkbox list
					CheckBox c = checks.Add(checkerattr.DisplayName, t);
					c.Checked = checkerattr.DefaultChecked;
				}
			}
		}
		
		// This shows the window
		public void Show(Form owner)
		{
			// Move controls according to the height of the checkers box
			checks.PerformLayout();
			buttoncheck.Top = checks.Bottom + 14;
			resultspanel.Top = buttoncheck.Bottom + 14;
			
			// First time showing?
			//if((this.Location.X == 0) && (this.Location.Y == 0))
			{
				// Position at left-top of owner
				this.Location = new Point(owner.Location.X + 20, owner.Location.Y + 90);
			}
			
			// Close results part
			resultspanel.Visible = false;
			this.Size = new Size(this.Width, this.Height - this.ClientSize.Height + resultspanel.Top);
			
			// Show window
			base.Show(owner);
		}
		
		#endregion
		
		#region ================== Thread Calls

		public void SubmitResult(ErrorResult result)
		{
			if(results.InvokeRequired)
			{
				CallResultMethodDelegate d = new CallResultMethodDelegate(SubmitResult);
				try { progress.Invoke(d, result); }
				catch(ThreadInterruptedException) { }
			}
			else
			{
				results.Items.Add(result);
			}
		}

		private void SetProgressMaximum(int maximum)
		{
			if(progress.InvokeRequired)
			{
				CallIntMethodDelegate d = new CallIntMethodDelegate(SetProgressMaximum);
				try { progress.Invoke(d, maximum); }
				catch(ThreadInterruptedException) { }
			}
			else
			{
				progress.Maximum = maximum;
			}
		}

		public void AddProgressValue(int value)
		{
			if(progress.InvokeRequired)
			{
				CallIntMethodDelegate d = new CallIntMethodDelegate(AddProgressValue);
				try { progress.Invoke(d, value); }
				catch(ThreadInterruptedException) { }
			}
			else
			{
				progress.Value += value;
			}
		}
		
		// This stops checking (only called from the checking management thread)
		private void StopChecking()
		{
			if(this.InvokeRequired)
			{
				CallVoidMethodDeletage d = new CallVoidMethodDeletage(StopChecking);
				this.Invoke(d);
			}
			else
			{
				checksthread = null;
				progress.Value = 0;
				buttoncheck.Text = "Start Analysis";
				Cursor.Current = Cursors.Default;
				running = false;
				blockmap.Dispose();
				blockmap = null;
				
				// When no results found, show "no results" and disable the list
				if(results.Items.Count == 0)
				{
					results.Items.Add(new ResultNoErrors());
					results.Enabled = false;
				}
			}
		}
		
		// This starts checking
		private void StartChecking()
		{
			if(running) return;
			
			Cursor.Current = Cursors.WaitCursor;
			
			// Make blockmap
			RectangleF area = MapSet.CreateArea(General.Map.Map.Vertices);
			area = MapSet.IncreaseArea(area, General.Map.Map.Things);
			blockmap = new BlockMap<BlockEntry>(area);
			blockmap.AddLinedefsSet(General.Map.Map.Linedefs);
			blockmap.AddSectorsSet(General.Map.Map.Sectors);
			blockmap.AddThingsSet(General.Map.Map.Things);
			
			// Open the results panel
			this.Size = new Size(this.Width, this.Height - this.ClientSize.Height + resultspanel.Top + resultspanel.Height);
			progress.Value = 0;
			results.Items.Clear();
			results.Enabled = true;
			ClearSelectedResult();
			resultspanel.Visible = true;
			buttoncheck.Text = "Abort Analysis";
			General.Interface.RedrawDisplay();
			
			// Start checking
			running = true;
			checksthread = new Thread(new ThreadStart(RunChecks));
			checksthread.Name = "Error Checking Management";
			checksthread.Priority = ThreadPriority.Normal;
			checksthread.Start();
			
			Cursor.Current = Cursors.Default;
		}

		#endregion

		#region ================== Methods
		
		// This stops the checking
		public void CloseWindow()
		{
			// Currently running?
			if(running)
			{
				Cursor.Current = Cursors.WaitCursor;
				checksthread.Interrupt();
			}

			ClearSelectedResult();
			this.Hide();
		}

		// This clears the selected result
		private void ClearSelectedResult()
		{
			results.SelectedIndex = -1;
			resultinfo.Text = "Select a result from the list to see more information.";
			resultinfo.Enabled = false;
			fix1.Visible = false;
			fix2.Visible = false;
			fix3.Visible = false;
		}
		
		// This runs in a seperate thread to manage the checking threads
		private void RunChecks()
		{
			List<ErrorChecker> checkers = new List<ErrorChecker>();
			List<Thread> threads = new List<Thread>();
			int maxthreads = Environment.ProcessorCount;
			int totalprogress = 0;
			int nextchecker = 0;
			
			// Initiate all checkers
			foreach(CheckBox c in checks.Checkboxes)
			{
				// Include this one?
				if(c.Checked)
				{
					Type t = (c.Tag as Type);
					ErrorChecker checker = null;
					
					try
					{
						// Create instance
						checker = (ErrorChecker)Assembly.GetExecutingAssembly().CreateInstance(t.FullName, false, BindingFlags.Default, null, null, CultureInfo.CurrentCulture, new object[0]);
					}
					catch(TargetInvocationException ex)
					{
						// Error!
						General.ErrorLogger.Add(ErrorType.Error, "Failed to create class instance '" + t.Name + "'");
						Logger.WriteLogLine(ex.InnerException.GetType().Name + ": " + ex.InnerException.Message);
						throw ex;
					}
					catch(Exception ex)
					{
						// Error!
						General.ErrorLogger.Add(ErrorType.Error, "Failed to create class instance '" + t.Name + "'");
						Logger.WriteLogLine(ex.GetType().Name + ": " + ex.Message);
						throw ex;
					}
					
					// Add to list
					if(checker != null)
					{
						checkers.Add(checker);
						totalprogress += checker.TotalProgress;
					}
				}
			}
			
			// Sort the checkers with highest cost first
			// See CompareTo method in ErrorChecker for sorting comparison
			checkers.Sort();
			
			// Setup
			SetProgressMaximum(totalprogress);
			
			// Continue while threads are running or checks are to be done
			while((nextchecker < checkers.Count) || (threads.Count > 0))
			{
				// Start new thread when less than maximum number of
				// threads running and there is more work to be done
				while((threads.Count < maxthreads) && (nextchecker < checkers.Count))
				{
					ErrorChecker c = checkers[nextchecker++];
					Thread t = new Thread(new ThreadStart(c.Run));
					t.Name = "Error Checker '" + c.GetType().Name + "'";
					t.Priority = ThreadPriority.BelowNormal;
					t.Start();
					threads.Add(t);
				}
				
				// Remove threads that are done
				for(int i = threads.Count - 1; i >= 0; i--)
					if(!threads[i].IsAlive) threads.RemoveAt(i);
				
				// Handle thread interruption
				try { Thread.Sleep(1); }
				catch(ThreadInterruptedException) { break; }
			}
			
			// Stop all running threads
			foreach(Thread t in threads)
			{
				while(t.IsAlive)
				{
					try
					{ 
						t.Interrupt();
						t.Join(1);
					}
					catch(ThreadInterruptedException)
					{
						// We have to continue, we can't just leave the other threads running!
					}
				}
			}
			
			// Dispose all checkers
			checkers = null;
			
			// Done
			StopChecking();
		}
		
		#endregion
		
		#region ================== Events
		
		// Window closing
		private void ErrorCheckForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			// If the user closes the form, then just cancel the mode
			if(e.CloseReason == CloseReason.UserClosing)
			{
				e.Cancel = true;
				General.Interface.Focus();
				General.Editing.CancelMode();
			}
		}
		
		// Start/stop
		private void buttoncheck_Click(object sender, EventArgs e)
		{
			// Currently running?
			if(running)
			{
				Cursor.Current = Cursors.WaitCursor;
				checksthread.Interrupt();
			}
			else
			{
				StartChecking();
			}
		}

		// Close
		private void closebutton_Click(object sender, EventArgs e)
		{
			General.Interface.Focus();
			General.Editing.CancelMode();
		}
		
		// Results selection changed
		private void results_SelectedIndexChanged(object sender, EventArgs e)
		{
			// Anything selected?
			if(results.SelectedIndex >= 0)
			{
				ErrorResult r = (results.SelectedItem as ErrorResult);
				resultinfo.Text = r.Description;
				resultinfo.Enabled = true;
				fix1.Text = r.Button1Text;
				fix2.Text = r.Button2Text;
				fix3.Text = r.Button3Text;
				fix1.Visible = (r.Buttons >= 1);
				fix2.Visible = (r.Buttons >= 2);
				fix3.Visible = (r.Buttons >= 3);
				r.ZoomToObject();
			}
			else
			{
				ClearSelectedResult();
			}
			
			General.Interface.RedrawDisplay();
		}
		
		// First button
		private void fix1_Click(object sender, EventArgs e)
		{
			// Anything selected?
			if(results.SelectedIndex >= 0)
			{
				if(running)
				{
					General.ShowWarningMessage("You must stop the analysis before you can make changes to your map!", MessageBoxButtons.OK);
				}
				else
				{
					ErrorResult r = (results.SelectedItem as ErrorResult);
					if(r.Button1Click()) StartChecking(); else General.Interface.RedrawDisplay();
				}
			}
		}
		
		// Second button
		private void fix2_Click(object sender, EventArgs e)
		{
			// Anything selected?
			if(results.SelectedIndex >= 0)
			{
				if(running)
				{
					General.ShowWarningMessage("You must stop the analysis before you can make changes to your map!", MessageBoxButtons.OK);
				}
				else
				{
					ErrorResult r = (results.SelectedItem as ErrorResult);
					if(r.Button2Click()) StartChecking(); else General.Interface.RedrawDisplay();
				}
			}
		}
		
		// Third button
		private void fix3_Click(object sender, EventArgs e)
		{
			// Anything selected?
			if(results.SelectedIndex >= 0)
			{
				if(running)
				{
					General.ShowWarningMessage("You must stop the analysis before you can make changes to your map!", MessageBoxButtons.OK);
				}
				else
				{
					ErrorResult r = (results.SelectedItem as ErrorResult);
					if(r.Button3Click()) StartChecking(); else General.Interface.RedrawDisplay();
				}
			}
		}

		private void ErrorCheckForm_HelpRequested(object sender, HelpEventArgs hlpevent)
		{
			General.ShowHelp("e_mapanalysis.html");
		}
		
		#endregion
	}
}