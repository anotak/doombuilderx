
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
using System.Reflection;
using System.IO;
using CodeImp.DoomBuilder.Config;
using CodeImp.DoomBuilder.IO;

#endregion

namespace CodeImp.DoomBuilder.Compilers
{
	public abstract class Compiler : IDisposable
	{
		#region ================== Variables
		
		// Parameters
		protected CompilerInfo info;
		protected string parameters;
		protected string workingdir;
		protected string sourcefile;
		protected string outputfile;
		protected string inputfile;
		
		// Files
		protected DirectoryInfo tempdir;

		// Errors
		private List<CompilerError> errors;
		
		// Disposing
		protected bool isdisposed;
		
		#endregion
		
		#region ================== Properties
		
		public string Parameters { get { return parameters; } set { parameters = value; } }
		public string WorkingDirectory { get { return workingdir; } set { workingdir = value; } }
		public string SourceFile { get { return sourcefile; } set { sourcefile = value; } }
		public string InputFile { get { return inputfile; } set { inputfile = value; } }
		public string OutputFile { get { return outputfile; } set { outputfile = value; } }
		public string Location { get { return tempdir.FullName; } }
		public bool IsDisposed { get { return isdisposed; } }
		public CompilerError[] Errors { get { return errors.ToArray(); } }
		
		#endregion
		
		#region ================== Constructor / Disposer
		
		// Constructor
		public Compiler(CompilerInfo info)
		{
			// Initialize
			this.info = info;
			this.errors = new List<CompilerError>();
			
			Logger.WriteLogLine("Creating compiler '" + info.Name + "' on interface '" + this.GetType().Name + "'...");
			
			// Create temporary directory
			tempdir = Directory.CreateDirectory(General.MakeTempDirname());
			workingdir = tempdir.FullName;
			
			// Copy required files to the temp directory
			Logger.WriteLogLine("Copying required files for compiler...");
			CopyRequiredFiles();
		}
		
		// Disposer
		public virtual void Dispose()
		{
			if(!isdisposed)
			{
				Exception deleteerror = null;
				double starttime = General.stopwatch.Elapsed.TotalMilliseconds;
				
				do
				{
					try
					{
						// Remove temporary directory
						Logger.WriteLogLine("Removing temporary compiler files...");
						tempdir.Delete(true);
						deleteerror = null;
					}
					catch(Exception e)
					{
						deleteerror = e;
					}
                    
					// Bail out when it takes too long
					if((General.stopwatch.Elapsed.TotalMilliseconds - starttime) > 2000) break;
				}
				while(deleteerror != null);
				
				// Report error if we have one
				if(deleteerror != null)
				{
					General.ErrorLogger.Add(ErrorType.Error, "Unable to remove temporary compiler files. " + deleteerror.GetType().Name + ": " + deleteerror.Message);
					Logger.WriteLogLine(deleteerror.StackTrace);
				}
				
				// Disposed
				isdisposed = true;
			}
		}
		
		#endregion
		
		#region ================== Methods

		// This copies all compiler files to a given destination
		private void CopyRequiredFiles()
		{
			// Copy files
			foreach(string f in info.Files)
			{
				string sourcefile = Path.Combine(info.Path, f);
				string targetfile = Path.Combine(tempdir.FullName, f);
				if(!File.Exists(sourcefile)) General.ErrorLogger.Add(ErrorType.Error, "The file '" + f + "' required by the '" + info.Name + "' compiler is missing. According to the compiler configuration in '" + info.FileName + "', the was expected to be found in the following path: " + info.Path);
				File.Copy(sourcefile, targetfile, true);
			}
		}
		
		/// <summary>
		/// This runs the compiler.
		/// </summary>
		/// <returns>Returns false when failed to start.</returns>
		public virtual bool Run() { return false; }

		/// <summary>
		/// Use this to report an error.
		/// </summary>
		protected void ReportError(CompilerError err)
		{
			this.errors.Add(err);
		}
		
		// This creates a compiler by interface name
		internal static Compiler Create(CompilerInfo info)
		{
			Compiler result;
			
			// Make list of assemblies to search in
			List<Assembly> asms = General.Plugins.GetPluginAssemblies();
			asms.Add(General.ThisAssembly);
			
			// Constructor arguments
			object[] args = new object[1];
			args[0] = info;
			
			try
			{
				// Go for all assemblies
				foreach(Assembly a in asms)
				{
					Type[] types;
					
					// Find the class
					if(a == General.ThisAssembly)
						types = a.GetTypes();
					else
						types = a.GetExportedTypes();
					
					foreach(Type t in types)
					{
						if(t.IsSubclassOf(typeof(Compiler)) && (t.Name == info.ProgramInterface))
						{
							// Create instance
							result = (Compiler)a.CreateInstance(t.FullName, false, BindingFlags.Default,
												null, args, CultureInfo.CurrentCulture, new object[0]);
							return result;
						}
					}
				}
			}
			// Catch errors
			catch(TargetInvocationException e)
			{
				// Throw the actual exception
				Debug.WriteLine(DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString());
				Debug.WriteLine(e.InnerException.Source + " throws " + e.InnerException.GetType().Name + ":");
				Debug.WriteLine(e.InnerException.Message);
				Debug.WriteLine(e.InnerException.StackTrace);
				throw e.InnerException;
			}

			// No such compiler
			return null;
		}
		
		#endregion
	}
}

