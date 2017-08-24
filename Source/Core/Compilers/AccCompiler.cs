
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
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.IO;
using CodeImp.DoomBuilder.Config;
using CodeImp.DoomBuilder.IO;
using System.Windows.Forms;
using System.Text.RegularExpressions;

#endregion

namespace CodeImp.DoomBuilder.Compilers
{
	internal sealed class AccCompiler : Compiler
	{
		#region ================== Constants
		
		private const string ACS_ERROR_FILE = "acs.err";
		
		#endregion
		
		#region ================== Variables
		
		#endregion
		
		#region ================== Constructor
		
		// Constructor
		public AccCompiler(CompilerInfo info) : base(info)
		{
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
		
		// This runs the compiler
		public override bool Run()
		{
			ProcessStartInfo processinfo;
			Process process;
			TimeSpan deltatime;
			int line = 0;
			string sourcedir = Path.GetDirectoryName(sourcefile);
			
			// Create parameters
			string args = this.parameters;
			args = args.Replace("%FI", inputfile);
			args = args.Replace("%FO", outputfile);
			args = args.Replace("%FS", sourcefile);
			args = args.Replace("%PT", this.tempdir.FullName);
			args = args.Replace("%PS", sourcedir);
			
			// Setup process info
			processinfo = new ProcessStartInfo();
			processinfo.Arguments = args;
			processinfo.FileName = Path.Combine(this.tempdir.FullName, info.ProgramFile);
			processinfo.CreateNoWindow = false;
			processinfo.ErrorDialog = false;
			processinfo.UseShellExecute = true;
			processinfo.WindowStyle = ProcessWindowStyle.Hidden;
			processinfo.WorkingDirectory = this.workingdir;
			
			// Output info
			Logger.WriteLogLine("Running compiler...");
			Logger.WriteLogLine("Program:    " + processinfo.FileName);
			Logger.WriteLogLine("Arguments:  " + processinfo.Arguments);
			
			try
			{
				// Start the compiler
				process = Process.Start(processinfo);
			}
			catch(Exception e)
			{
				// Unable to start the compiler
				General.ShowErrorMessage("Unable to start the compiler (" + info.Name + "). " + e.GetType().Name + ": " + e.Message, MessageBoxButtons.OK);
				return false;
			}
			
			// Wait for compiler to complete
			process.WaitForExit();
			deltatime = TimeSpan.FromTicks(process.ExitTime.Ticks - process.StartTime.Ticks);
			Logger.WriteLogLine("Compiler process has finished.");
			Logger.WriteLogLine("Compile time: " + deltatime.TotalSeconds.ToString("########0.00") + " seconds");
			
			// Now find the error file
			string errfile = Path.Combine(this.workingdir, ACS_ERROR_FILE);
			if(File.Exists(errfile))
			{
				try
				{
					// Regex to find error lines
					Regex errlinematcher = new Regex(":[0-9]+: ", RegexOptions.Compiled | RegexOptions.CultureInvariant);
					
					// Read all lines
					string[] errlines = File.ReadAllLines(errfile);
					while(line < errlines.Length)
					{
						// Check line
						string linestr = errlines[line];
						Match match = errlinematcher.Match(linestr);
						if(match.Success && (match.Index > 0))
						{
							CompilerError err = new CompilerError();
							
							// The match without spaces and semicolon is the line number
							string linenr = match.Value.Replace(":", "").Trim();
							if(!int.TryParse(linenr, out err.linenumber))
								err.linenumber = CompilerError.NO_LINE_NUMBER;
							else
								err.linenumber--;
							
							// Everything before the match is the filename
							err.filename = linestr.Substring(0, match.Index);
							if(!Path.IsPathRooted(err.filename))
							{
								// Add working directory to filename
								err.filename = Path.Combine(processinfo.WorkingDirectory, err.filename);
							}
							
							// Everything after the match is the description
							err.description = linestr.Substring(match.Index + match.Length).Trim();
							
							// Report the error
							ReportError(err);
						}
						
						// Next line
						line++;
					}
				}
				catch(Exception e)
				{
					// Error reading errors (ironic, isn't it)
					ReportError(new CompilerError("Failed to retrieve compiler error report. " + e.GetType().Name + ": " + e.Message));
				}
			}
			
			return true;
		}
		
		#endregion
	}
}

