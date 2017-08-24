
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
using System.IO;
using CodeImp.DoomBuilder.Data;
using System.Diagnostics;
using CodeImp.DoomBuilder.Actions;
using System.Windows.Forms;
using CodeImp.DoomBuilder.Windows;

#endregion

namespace CodeImp.DoomBuilder
{
	internal class Launcher : IDisposable
	{
		#region ================== Constants

		private const string NUMBERS = "0123456789";

		#endregion

		#region ================== Variables

		private string tempwad;

		private bool isdisposed;
		
		#endregion

		#region ================== Properties

		public string TempWAD { get { return tempwad; } }
		
		#endregion

		#region ================== Constructor / Destructor

		// Constructor
		public Launcher(MapManager manager)
		{
			// Initialize
			CleanTempFile(manager);

			// Bind actions
			General.Actions.BindMethods(this);
		}

		// Disposer
		public void Dispose()
		{
			// Not yet disposed?
			if(!isdisposed)
			{
				// Unbind actions
				General.Actions.UnbindMethods(this);
				
				// Remove temporary file
				try { File.Delete(tempwad); }
				catch(Exception) { }
				
				// Done
				isdisposed = true;
			}
		}

		#endregion

		#region ================== Parameters

		// This takes the unconverted parameters (with placeholders) and converts it
		// to parameters with full paths, names and numbers where placeholders were put.
		// The tempfile must be the full path and filename to the PWAD file to test.
		public string ConvertParameters(string parameters, int skill, bool shortpaths)
		{
			string outp = parameters;
			DataLocation iwadloc;
			string p_wp = "", p_wf = "";
			string p_ap = "", p_apq = "";
			string p_l1 = "", p_l2 = "";
			string p_nm = "";
			string f = tempwad;
			
			// Make short path if needed
			if(shortpaths) f = General.GetShortFilePath(f);
			
			// Find the first IWAD file
			if(General.Map.Data.FindFirstIWAD(out iwadloc))
			{
				// %WP and %WF result in IWAD file
				p_wp = iwadloc.location;
				p_wf = Path.GetFileName(p_wp);
				if(shortpaths)
				{
					p_wp = General.GetShortFilePath(p_wp);
					p_wf = General.GetShortFilePath(p_wf);
				}
			}
			
			// Make a list of all data locations, including map location
			DataLocation maplocation = new DataLocation(DataLocation.RESOURCE_WAD, General.Map.FilePathName, false, false, false);
			DataLocationList locations = new DataLocationList();
			locations.AddRange(General.Map.ConfigSettings.Resources);
			locations.AddRange(General.Map.Options.Resources);
			locations.Add(maplocation);
			
			// Go for all data locations
			foreach(DataLocation dl in locations)
			{
				// Location not the IWAD file?
				if((dl.type != DataLocation.RESOURCE_WAD) || (dl.location != iwadloc.location))
				{
					// Location not included?
					if(!dl.notfortesting)
					{
						// Add to string of files
						if(shortpaths)
						{
							p_ap += General.GetShortFilePath(dl.location) + " ";
							p_apq += "\"" + General.GetShortFilePath(dl.location) + "\" ";
						}
						else
						{
							p_ap += dl.location + " ";
							p_apq += "\"" + dl.location + "\" ";
						}
					}
				}
			}

			// Trim last space from resource file locations
			p_ap = p_ap.TrimEnd(' ');
			p_apq = p_apq.TrimEnd(' ');

			// Try finding the L1 and L2 numbers from the map name
			string numstr = "";
			bool first = true;
			foreach(char c in General.Map.Options.CurrentName)
			{
				// Character is a number?
				if(NUMBERS.IndexOf(c) > -1)
				{
					// Include it
					numstr += c;
				}
				else
				{
					// Store the number if we found one
					if(numstr.Length > 0)
					{
						int num = 0;
						int.TryParse(numstr, out num);
						if(first) p_l1 = num.ToString(); else p_l2 = num.ToString();
						numstr = "";
						first = false;
					}
				}
			}
			
			// Store the number if we found one
			if(numstr.Length > 0)
			{
				int num = 0;
				int.TryParse(numstr, out num);
				if(first) p_l1 = num.ToString(); else p_l2 = num.ToString();
			}

			// No monsters?
			if(!General.Settings.TestMonsters) p_nm = "-nomonsters";
			
			// Make sure all our placeholders are in uppercase
			outp = outp.Replace("%f", "%F");
			outp = outp.Replace("%wp", "%WP");
			outp = outp.Replace("%wf", "%WF");
			outp = outp.Replace("%wP", "%WP");
			outp = outp.Replace("%wF", "%WF");
			outp = outp.Replace("%Wp", "%WP");
			outp = outp.Replace("%Wf", "%WF");
			outp = outp.Replace("%l1", "%L1");
			outp = outp.Replace("%l2", "%L2");
			outp = outp.Replace("%l", "%L");
			outp = outp.Replace("%ap", "%AP");
			outp = outp.Replace("%aP", "%AP");
			outp = outp.Replace("%Ap", "%AP");
			outp = outp.Replace("%s", "%S");
			outp = outp.Replace("%nM", "%NM");
			outp = outp.Replace("%Nm", "%NM");
			outp = outp.Replace("%nm", "%NM");
			
			// Replace placeholders with actual values
			outp = outp.Replace("%F", f);
			outp = outp.Replace("%WP", p_wp);
			outp = outp.Replace("%WF", p_wf);
			outp = outp.Replace("%L1", p_l1);
			outp = outp.Replace("%L2", p_l2);
			outp = outp.Replace("%L", General.Map.Options.CurrentName);
			outp = outp.Replace("\"%AP\"", p_apq);
			outp = outp.Replace("%AP", p_ap);
			outp = outp.Replace("%S", skill.ToString());
			outp = outp.Replace("%NM", p_nm);
			
			// Return result
			return outp;
		}

		#endregion

		#region ================== Test

		// This saves the map to a temporary file and launches a test
		[BeginAction("testmap")]
		public void Test()
		{
			TestAtSkill(General.Map.ConfigSettings.TestSkill);
		}
		
		// This saves the map to a temporary file and launches a test wit hthe given skill
		public void TestAtSkill(int skill)
		{
			Cursor oldcursor = Cursor.Current;
			ProcessStartInfo processinfo;
			Process process;
			TimeSpan deltatime;
			string args;

			// Check if configuration is OK
			if((General.Map.ConfigSettings.TestProgram == "") ||
			   !File.Exists(General.Map.ConfigSettings.TestProgram))
			{
				// Show message
				Cursor.Current = Cursors.Default;
				DialogResult result = General.ShowWarningMessage("Your test program is not set for the current game configuration. Would you like to set up your test program now?", MessageBoxButtons.YesNo);
				if(result == DialogResult.Yes)
				{
					// Show game configuration on the right page
					General.MainWindow.ShowConfigurationPage(2);
				}
				return;
			}

			// No custom parameters?
			if(!General.Map.ConfigSettings.CustomParameters)
			{
				// Set parameters to the default ones
				General.Map.ConfigSettings.TestParameters = General.Map.Config.TestParameters;
				General.Map.ConfigSettings.TestShortPaths = General.Map.Config.TestShortPaths;
			}
			
			// Remove temporary file
			try { File.Delete(tempwad); }
			catch(Exception) { }
			
			// Save map to temporary file
			Cursor.Current = Cursors.WaitCursor;
			tempwad = General.MakeTempFilename(General.Map.TempPath, "wad");
			General.Plugins.OnMapSaveBegin(SavePurpose.Testing);
			if(General.Map.SaveMap(tempwad, SavePurpose.Testing))
			{
				// No compiler errors?
				if(General.Map.Errors.Count == 0)
				{
					// Make arguments
					args = ConvertParameters(General.Map.ConfigSettings.TestParameters, skill, General.Map.ConfigSettings.TestShortPaths);

					// Setup process info
					processinfo = new ProcessStartInfo();
					processinfo.Arguments = args;
					processinfo.FileName = General.Map.ConfigSettings.TestProgram;
					processinfo.CreateNoWindow = false;
					processinfo.ErrorDialog = false;
					processinfo.UseShellExecute = true;
					processinfo.WindowStyle = ProcessWindowStyle.Normal;
					processinfo.WorkingDirectory = Path.GetDirectoryName(processinfo.FileName);

					// Output info
					Logger.WriteLogLine("Running test program: " + processinfo.FileName);
					Logger.WriteLogLine("Program parameters:  " + processinfo.Arguments);

					// Disable interface
					General.MainWindow.DisplayStatus(StatusType.Busy, "Waiting for game application to finish...");

					try
					{
						// Start the program
						process = Process.Start(processinfo);

						// Wait for program to complete
						while(!process.WaitForExit(10))
						{
							General.MainWindow.Update();
						}

						// Done
						deltatime = TimeSpan.FromTicks(process.ExitTime.Ticks - process.StartTime.Ticks);
						Logger.WriteLogLine("Test program has finished.");
						Logger.WriteLogLine("Run time: " + deltatime.TotalSeconds.ToString("###########0.00") + " seconds");
					}
					catch(Exception e)
					{
						// Unable to start the program
						General.ShowErrorMessage("Unable to start the test program, " + e.GetType().Name + ": " + e.Message, MessageBoxButtons.OK); ;
					}
					
					General.MainWindow.DisplayReady();
				}
				else
				{
					General.MainWindow.DisplayStatus(StatusType.Warning, "Unable to test the map due to script errors.");
				}
			}
			
			// Clean up temp file
			CleanTempFile(General.Map);
			
			// Done
			General.Map.Graphics.Reset();
			General.Plugins.OnMapSaveEnd(SavePurpose.Testing);
			General.MainWindow.RedrawDisplay();
			General.MainWindow.FocusDisplay();
			Cursor.Current = oldcursor;
		}
		
		// This deletes the previous temp file and creates a new, empty temp file
		private void CleanTempFile(MapManager manager)
		{
			// Remove temporary file
			try { File.Delete(tempwad); }
			catch(Exception) { }
			
			// Make new empty temp file
			tempwad = General.MakeTempFilename(manager.TempPath, "wad");
			File.WriteAllText(tempwad, "");
		}

		#endregion
	}
}
