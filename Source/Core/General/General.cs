
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
using CodeImp.DoomBuilder.Data;
using CodeImp.DoomBuilder.Windows;
using CodeImp.DoomBuilder.IO;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Geometry;
using System.Runtime.InteropServices;
using CodeImp.DoomBuilder.Actions;
using System.Diagnostics;
using CodeImp.DoomBuilder.Rendering;
using CodeImp.DoomBuilder.Config;
using Microsoft.Win32;
using SlimDX.Direct3D9;
using System.Drawing;
using CodeImp.DoomBuilder.Plugins;
using CodeImp.DoomBuilder.Types;
using System.Collections.ObjectModel;
using System.Threading;
using CodeImp.DoomBuilder.Editing;

#endregion

namespace CodeImp.DoomBuilder
{
	public static class General
	{
		#region ================== API Declarations

		[DllImport("devil.dll")]
		private static extern void ilInit();

		[DllImport("user32.dll")]
		internal static extern bool LockWindowUpdate(IntPtr hwnd);

		[DllImport("kernel32.dll", EntryPoint = "RtlZeroMemory", SetLastError = false)]
		internal static extern void ZeroMemory(IntPtr dest, int size);

		[DllImport("kernel32.dll", EntryPoint = "RtlMoveMemory", SetLastError = false)]
		internal static extern unsafe void CopyMemory(void* dst, void* src, uint length);

		[DllImport("user32.dll", EntryPoint = "SendMessage", SetLastError = true, CallingConvention = CallingConvention.StdCall)]
		internal static extern int SendMessage(IntPtr hwnd, uint Msg, int wParam, int lParam);

		[DllImport("user32.dll", SetLastError = true)]
		internal static extern bool MessageBeep(MessageBeepType type);

		[DllImport("kernel32.dll")]
		internal extern static IntPtr LoadLibrary(string filename);

		[DllImport("kernel32.dll")]
		internal extern static bool FreeLibrary(IntPtr moduleptr);

		[DllImport("user32.dll")]
		internal static extern IntPtr CreateWindowEx(uint exstyle, string classname, string windowname, uint style,
												   int x, int y, int width, int height, IntPtr parentptr, int menu,
												   IntPtr instanceptr, string param);

		[DllImport("user32.dll")]
		internal static extern bool DestroyWindow(IntPtr windowptr);

		[DllImport("user32.dll")]
		internal static extern int SetWindowPos(IntPtr windowptr, int insertafterptr, int x, int y, int cx, int cy, int flags);
		
		[DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern uint GetShortPathName([MarshalAs(UnmanagedType.LPTStr)] string longpath, [MarshalAs(UnmanagedType.LPTStr)]StringBuilder shortpath, uint buffersize);

		[DllImport("user32.dll")]
		internal static extern int SetScrollInfo(IntPtr windowptr, int bar, IntPtr scrollinfo, bool redraw);

		[DllImport("user32.dll")]
		internal static extern int GetScrollInfo(IntPtr windowptr, int bar, IntPtr scrollinfo);

		#endregion

		#region ================== Constants

		// SendMessage API
		internal const int WM_USER = 0x400;
		internal const int WM_SYSCOMMAND = 0x112;
		internal const int SC_KEYMENU = 0xF100;
		internal const int CB_SETITEMHEIGHT = 0x153;
		internal const int CB_SHOWDROPDOWN = 0x14F;
		internal const int EM_GETSCROLLPOS = WM_USER + 221;
		internal const int EM_SETSCROLLPOS = WM_USER + 222;
		internal const int SB_HORZ = 0;
		internal const int SB_VERT = 1;
		internal const int SB_CTL = 2;
		internal const int SIF_RANGE = 0x1;
		internal const int SIF_PAGE = 0x2;
		internal const int SIF_POS = 0x4;
		internal const int SIF_DISABLENOSCROLL = 0x8;
		internal const int SIF_TRACKPOS = 0x16;
		internal const int SIF_ALL = SIF_RANGE + SIF_PAGE + SIF_POS + SIF_TRACKPOS;
		
		// Files and Folders
		private const string SETTINGS_FILE = "BuilderX.cfg";
        private const string DB2_SETTINGS_FILE = "Builder.cfg";
        private const string SETTINGS_DIR = "Doom Builder";
        private const string TEMP_DIR = "DBX";
        private const string LOG_FILE = "BuilderX.log";
        private const string CRASH_LOG_FILE = "BuilderX-lastcrash.log";
        private const string CRASH_SAVE_FILE = "crashbackup.wad";
        private const string GAME_CONFIGS_DIR = "Configurations";
		private const string COMPILERS_DIR = "Compilers";
		private const string PLUGINS_DIR = "Plugins";
		private const string SCRIPTS_DIR = "Scripting";
		private const string SETUP_DIR = "Setup";
		private const string SPRITES_DIR = "Sprites";
		private const string HELP_FILE = "Refmanual.chm";

		// SCROLLINFO structure
        /*
		internal struct ScrollInfo
		{
			public int size;		// size of this structure
			public uint mask;		// combination of SIF_ constants
			public int min;			// minimum scrolling position
			public int max;			// maximum scrolling position
			public uint page;		// page size (scroll bar uses this value to determine the appropriate size of the proportional scroll box)
			public int pos;			// position of the scroll box
			public int trackpos;	// immediate position of a scroll box that the user is dragging
		}
        */
		#endregion

		#region ================== Variables

		// Files and Folders
		private static string apppath;
		private static string setuppath;
		private static string settingspath;
		private static string temppath;
		private static string configspath;
		private static string compilerspath;
		private static string scriptspath;
		private static string pluginspath;
		private static string spritespath;
		
		// Main objects
		private static Assembly thisasm;
		private static MainForm mainwindow;
		private static ProgramConfiguration settings;
		private static MapManager map;
		private static EditingManager editing;
		private static ActionManager actions;
		private static PluginManager plugins;
		private static ColorCollection colors;
		private static TypesManager types;
		private static ErrorLogger errorlogger;
		private static Mutex appmutex;
		
		// Configurations
		private static List<ConfigurationInfo> configs;
		private static List<CompilerInfo> compilers;
		private static List<NodebuilderInfo> nodebuilders;
		private static Dictionary<string, ScriptConfiguration> scriptconfigs;
		
		// States
		private static bool debugbuild;
		
		// Command line arguments
		private static string[] cmdargs;
		private static string autoloadfile = null;
		private static string autoloadmap = null;
		private static string autoloadconfig = null;
		private static bool autoloadstrictpatches = false;
		private static DataLocationList autoloadresources = null;
		private static bool delaymainwindow;
		private static bool nosettings;

        private static Random random;

		#endregion

		#region ================== Properties

		public static Assembly ThisAssembly { get { return thisasm; } }
		public static string AppPath { get { return apppath; } }
		public static string TempPath { get { return temppath; } }
		public static string ConfigsPath { get { return configspath; } }
		public static string CompilersPath { get { return compilerspath; } }
		public static string PluginsPath { get { return pluginspath; } }
		public static string SpritesPath { get { return spritespath; } }
        public static string SettingsPath { get { return settingspath; } }
		public static ICollection<string> CommandArgs { get { return Array.AsReadOnly<string>(cmdargs); } }
		internal static MainForm MainWindow { get { return mainwindow; } }
		public static IMainForm Interface { get { return mainwindow; } }
		public static ProgramConfiguration Settings { get { return settings; } }
		public static ColorCollection Colors { get { return colors; } }
		internal static List<ConfigurationInfo> Configs { get { return configs; } }
		internal static List<NodebuilderInfo> Nodebuilders { get { return nodebuilders; } }
		internal static List<CompilerInfo> Compilers { get { return compilers; } }
		internal static Dictionary<string, ScriptConfiguration> ScriptConfigs { get { return scriptconfigs; } }
		public static MapManager Map { get { return map; } }
		public static ActionManager Actions { get { return actions; } }
		internal static PluginManager Plugins { get { return plugins; } }
        public static Stopwatch stopwatch; // instead of clock
		public static bool DebugBuild { get { return debugbuild; } }
		internal static TypesManager Types { get { return types; } }
		public static string AutoLoadFile { get { return autoloadfile; } }
		public static string AutoLoadMap { get { return autoloadmap; } }
		public static string AutoLoadConfig { get { return autoloadconfig; } }
		public static bool AutoLoadStrictPatches { get { return autoloadstrictpatches; } }
		public static DataLocationList AutoLoadResources { get { return new DataLocationList(autoloadresources); } }
		public static bool DelayMainWindow { get { return delaymainwindow; } }
		public static bool NoSettings { get { return nosettings; } }
		public static EditingManager Editing { get { return editing; } }
		public static ErrorLogger ErrorLogger { get { return errorlogger; } }
        // ano - having a global one is just better
        public static Random Random { get { if (random == null) { random = new Random(); } return random; } }
		
		#endregion

		#region ================== Configurations

		// This returns the game configuration info by filename
		internal static ConfigurationInfo GetConfigurationInfo(string filename)
		{
			// Go for all config infos
			foreach(ConfigurationInfo ci in configs)
			{
				// Check if filename matches
				if(string.Compare(Path.GetFileNameWithoutExtension(ci.Filename),
								  Path.GetFileNameWithoutExtension(filename), true) == 0)
				{
					// Return this info
					return ci;
				}
			}

			// None found
			return null;
		}

		// This loads and returns a game configuration
		internal static Configuration LoadGameConfiguration(string filename)
		{
			Configuration cfg;
			
			// Make the full filepathname
			string filepathname = Path.Combine(configspath, filename);
			
			// Load configuration
			try
			{
				// Try loading the configuration
				cfg = new Configuration(filepathname, true);

				// Check for erors
				if(cfg.ErrorResult)
				{
					// Error in configuration
					errorlogger.Add(ErrorType.Error, "Unable to load the game configuration file \"" + filename + "\". " +
													 "Error in file \"" + cfg.ErrorFile + "\" near line " + cfg.ErrorLine + ": " + cfg.ErrorDescription);
					return null;
				}
				// Check if this is a Doom Builder 2 config
				else if(cfg.ReadSetting("type", "") != "Doom Builder 2 Game Configuration")
				{
					// Old configuration
					errorlogger.Add(ErrorType.Error, "Unable to load the game configuration file \"" + filename + "\". " +
													 "This configuration is not a Doom Builder 2 game configuration.");
					return null;
				}
				else
				{
					// Return config
					return cfg;
				}
			}
			catch(Exception e)
			{
				// Unable to load configuration
				errorlogger.Add(ErrorType.Error, "Unable to load the game configuration file \"" + filename + "\". " + e.GetType().Name + ": " + e.Message);
				Logger.WriteLogLine(e.StackTrace);
				return null;
			}
		}

		// This loads all game configurations
		private static void LoadAllGameConfigurations()
		{
			Configuration cfg;
			string[] filenames;
			string name, fullfilename;
			
			// Display status
			mainwindow.DisplayStatus(StatusType.Busy, "Loading game configurations...");

			// Make array
			configs = new List<ConfigurationInfo>();

			// Go for all cfg files in the configurations directory
			filenames = Directory.GetFiles(configspath, "*.cfg", SearchOption.TopDirectoryOnly);
			foreach(string filepath in filenames)
			{
				// Check if it can be loaded
				cfg = LoadGameConfiguration(Path.GetFileName(filepath));
				if(cfg != null)
				{
					fullfilename = Path.GetFileName(filepath);
					ConfigurationInfo cfginfo = new ConfigurationInfo(cfg, fullfilename);
					
					// Add to lists
					Logger.WriteLogLine("Registered game configuration '" + cfginfo.Name + "' from '" + fullfilename + "'");
					configs.Add(cfginfo);
				}
			}

			// Sort the list
			configs.Sort();
		}

		// This loads all nodebuilder configurations
		private static void LoadAllNodebuilderConfigurations()
		{
			Configuration cfg;
			IDictionary builderslist;
			string[] filenames;
			
			// Display status
			mainwindow.DisplayStatus(StatusType.Busy, "Loading nodebuilder configurations...");

			// Make array
			nodebuilders = new List<NodebuilderInfo>();

			// Go for all cfg files in the compilers directory
			filenames = Directory.GetFiles(compilerspath, "*.cfg", SearchOption.AllDirectories);
			foreach(string filepath in filenames)
			{
				try
				{
					// Try loading the configuration
					cfg = new Configuration(filepath, true);

					// Check for erors
					if(cfg.ErrorResult)
					{
						// Error in configuration
						errorlogger.Add(ErrorType.Error, "Unable to load the compiler configuration file \"" + Path.GetFileName(filepath) + "\". " +
										                 "Error in file \"" + cfg.ErrorFile + "\" near line " + cfg.ErrorLine + ": " + cfg.ErrorDescription);
					}
					else
					{
						// Get structures
						builderslist = cfg.ReadSetting("nodebuilders", new Hashtable());
						foreach(DictionaryEntry de in builderslist)
						{
							// Check if this is a structure
							if(de.Value is IDictionary)
							{
								try
								{
									// Make nodebuilder info
									nodebuilders.Add(new NodebuilderInfo(Path.GetFileName(filepath), de.Key.ToString(), cfg));
								}
								catch(Exception e)
								{
									// Unable to load configuration
									errorlogger.Add(ErrorType.Error, "Unable to load the nodebuilder configuration '" + de.Key.ToString() + "' from \"" + Path.GetFileName(filepath) + "\". Error: " + e.Message);
								}
							}
						}
					}
				}
				catch(Exception)
				{
					// Unable to load configuration
					errorlogger.Add(ErrorType.Error, "Unable to load the compiler configuration file \"" + Path.GetFileName(filepath) + "\".");
				}
			}

			// Sort the list
			nodebuilders.Sort();
		}

		// This loads all script configurations
		private static void LoadAllScriptConfigurations()
		{
			Configuration cfg;
			string[] filenames;
			
			// Display status
			mainwindow.DisplayStatus(StatusType.Busy, "Loading script configurations...");
			
			// Make collection
			scriptconfigs = new Dictionary<string, ScriptConfiguration>();
			
			// Go for all cfg files in the scripts directory
			filenames = Directory.GetFiles(scriptspath, "*.cfg", SearchOption.TopDirectoryOnly);
			foreach(string filepath in filenames)
			{
				try
				{
					// Try loading the configuration
					cfg = new Configuration(filepath, true);
					
					// Check for erors
					if(cfg.ErrorResult)
					{
						// Error in configuration
						errorlogger.Add(ErrorType.Error, "Unable to load the script configuration file \"" + Path.GetFileName(filepath) + "\". " +
														"Error in file \"" + cfg.ErrorFile + "\" near line " + cfg.ErrorLine + ": " + cfg.ErrorDescription);
					}
					else
					{
						try
						{
							// Make script configuration
							ScriptConfiguration scfg = new ScriptConfiguration(cfg);
							string filename = Path.GetFileName(filepath);
							scriptconfigs.Add(filename.ToLowerInvariant(), scfg);
						}
						catch(Exception e)
						{
							// Unable to load configuration
							errorlogger.Add(ErrorType.Error, "Unable to load the script configuration \"" + Path.GetFileName(filepath) + "\". Error: " + e.Message);
						}
					}
				}
				catch(Exception e)
				{
					// Unable to load configuration
					errorlogger.Add(ErrorType.Error, "Unable to load the script configuration file \"" + Path.GetFileName(filepath) + "\". Error: " + e.Message);
					Logger.WriteLogLine(e.StackTrace);
				}
			}
		}

		// This loads all compiler configurations
		private static void LoadAllCompilerConfigurations()
		{
			Configuration cfg;
			Dictionary<string, CompilerInfo> addedcompilers = new Dictionary<string,CompilerInfo>();
			IDictionary compilerslist;
			string[] filenames;

			// Display status
			mainwindow.DisplayStatus(StatusType.Busy, "Loading compiler configurations...");

			// Make array
			compilers = new List<CompilerInfo>();

			// Go for all cfg files in the compilers directory
			filenames = Directory.GetFiles(compilerspath, "*.cfg", SearchOption.AllDirectories);
			foreach(string filepath in filenames)
			{
				try
				{
					// Try loading the configuration
					cfg = new Configuration(filepath, true);

					// Check for erors
					if(cfg.ErrorResult)
					{
						// Error in configuration
						errorlogger.Add(ErrorType.Error, "Unable to load the compiler configuration file \"" + Path.GetFileName(filepath) + "\". " +
														 "Error in file \"" + cfg.ErrorFile + "\" near line " + cfg.ErrorLine + ": " + cfg.ErrorDescription);
					}
					else
					{
						// Get structures
						compilerslist = cfg.ReadSetting("compilers", new Hashtable());
						foreach(DictionaryEntry de in compilerslist)
						{
							// Check if this is a structure
							if(de.Value is IDictionary)
							{
								// Make compiler info
								CompilerInfo info = new CompilerInfo(Path.GetFileName(filepath), de.Key.ToString(), Path.GetDirectoryName(filepath), cfg);
								if(!addedcompilers.ContainsKey(info.Name))
								{
									compilers.Add(info);
									addedcompilers.Add(info.Name, info);
								}
								else
								{
									errorlogger.Add(ErrorType.Error, "Compiler \"" + info.Name + "\" is defined more than once. The first definition in " + addedcompilers[info.Name].FileName + " will be used.");
								}
							}
						}
					}
				}
				catch(Exception e)
				{
					// Unable to load configuration
					errorlogger.Add(ErrorType.Error, "Unable to load the compiler configuration file \"" + Path.GetFileName(filepath) + "\". " + e.GetType().Name + ": " + e.Message);
					Logger.WriteLogLine(e.StackTrace);
				}
			}
		}
		
		// This returns a nodebuilder by name
		internal static NodebuilderInfo GetNodebuilderByName(string name)
		{
			// Go for all nodebuilders
			foreach(NodebuilderInfo n in nodebuilders)
			{
				// Name matches?
				if(n.Name == name) return n;
			}

			// Cannot find that nodebuilder
			return null;
		}

        #endregion

        #region ================== Startup

        // Main program entry
        [STAThread]
        internal static void Main(string[] args)
        {
            settingspath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), SETTINGS_DIR);
            string logfile = Path.Combine(settingspath, LOG_FILE);
            // Make program settings directory if missing
            if (!Directory.Exists(settingspath)) Directory.CreateDirectory(settingspath);

            // Remove the previous log file and start logging
            Logger.StartLogging(logfile);

            
            // ano - shoutouts to MXD on this bit, our actual handlers are different tho
#if DEBUG
            debugbuild = true;
            RealMain(args);
#else
            debugbuild = false;
            //mxd. Custom exception dialog.
            AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;
            Application.ThreadException += Application_ThreadException;

            try {
                RealMain(args);
            }
            catch(Exception e)
            {
                HandleException(e);
            }
#endif
        }

        // original entry
        internal static void RealMain(string[] args)
        {
            settingspath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), SETTINGS_DIR);
            string logfile = Path.Combine(settingspath, LOG_FILE);

            // Make program settings directory if missing
            if (!Directory.Exists(settingspath)) Directory.CreateDirectory(settingspath);

            stopwatch = new Stopwatch();
            stopwatch.Start();

			Uri localpath;
			Version thisversion;
			
			// Determine states
			#if DEBUG
				debugbuild = true;
			#else
				debugbuild = false;
			#endif
			
			// Enable OS visual styles
			Application.EnableVisualStyles();
			Application.DoEvents();		// This must be here to work around a .NET bug
			ToolStripManager.Renderer = new ToolStripProfessionalRenderer(new TanColorTable());
			
			// Hook to DLL loading failure event
			AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(CurrentDomain_AssemblyResolve);
			
			// Set current thread name
			Thread.CurrentThread.Name = "Main Application";

			// Application is running
			appmutex = new Mutex(false, "doombuilder2");
			
			// Get a reference to this assembly
			thisasm = Assembly.GetExecutingAssembly();
			thisversion = thisasm.GetName().Version;
			
			// Find application path
			localpath = new Uri(Path.GetDirectoryName(thisasm.GetName().CodeBase));
			apppath = Uri.UnescapeDataString(localpath.AbsolutePath);
			
			// Setup directories
			temppath = Path.Combine(Path.GetTempPath(), TEMP_DIR);
			setuppath = Path.Combine(apppath, SETUP_DIR);
            settingspath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), SETTINGS_DIR);
            configspath = Path.Combine(apppath, GAME_CONFIGS_DIR);
			compilerspath = Path.Combine(apppath, COMPILERS_DIR);
			pluginspath = Path.Combine(apppath, PLUGINS_DIR);
			scriptspath = Path.Combine(apppath, SCRIPTS_DIR);
			spritespath = Path.Combine(apppath, SPRITES_DIR);
			
			Logger.WriteLogLine("DBX " + thisversion.Major + "." + thisversion.Minor + " startup");
			Logger.WriteLogLine("Application path:        " + apppath);
			Logger.WriteLogLine("Temporary path:          " + temppath);
			Logger.WriteLogLine("Local settings path:     " + settingspath);
			Logger.WriteLogLine("Command-line arguments:  " + args.Length);
			for(int i = 0; i < args.Length; i++)
				Logger.WriteLogLine("Argument " + i + ":   \"" + args[i] + "\"");
			
			// Parse command-line arguments
			ParseCommandLineArgs(args);
			
			// Load configuration
			Logger.WriteLogLine("RealMain: Loading program configuration...");
			settings = new ProgramConfiguration();
			string defaultsettingsfile = Path.Combine(apppath, SETTINGS_FILE);
			string usersettingsfile = nosettings ? defaultsettingsfile : Path.Combine(settingspath, SETTINGS_FILE);

            // try to copy old db2 config
            if (!nosettings
                && !File.Exists(usersettingsfile)
                && File.Exists(Path.Combine(settingspath, DB2_SETTINGS_FILE)))
            {
                try
                {
                    Logger.WriteLogLine("RealMain: Missing cfg file but detected an old DB2 config, attempting to copy...");
                    File.Copy(Path.Combine(settingspath, DB2_SETTINGS_FILE), usersettingsfile);
                }
                catch (Exception e)
                {
                    Logger.WriteLogLine("RealMain: db2 config copy failed!");

                }
            }

			if(settings.Load(usersettingsfile, defaultsettingsfile))
			{
				// Create error logger
				errorlogger = new ErrorLogger();
				
				// Create action manager
				actions = new ActionManager();
				
				// Bind static methods to actions
				General.Actions.BindMethods(typeof(General));

				// Initialize static classes
				MapSet.Initialize();
				ilInit();
                
				// Create main window
				Logger.WriteLogLine("RealMain: Loading main interface window...");
				mainwindow = new MainForm();
				mainwindow.SetupInterface();
				mainwindow.UpdateInterface();
				mainwindow.UpdateThingsFilters();

				if(!delaymainwindow)
				{
					// Show main window
					Logger.WriteLogLine("RealMain: Showing main interface window...");
					mainwindow.Show();
					mainwindow.Update();
				}
				
				// Start Direct3D
				Logger.WriteLogLine("RealMain: Starting Direct3D driver");
				try { D3DDevice.Startup(); }
				catch(Direct3D9NotFoundException) { AskDownloadDirectX(); return; }
				catch(Direct3DX9NotFoundException) { AskDownloadDirectX(); return; }
				
				// Load plugin manager
				Logger.WriteLogLine("RealMain: Loading plugins");
				plugins = new PluginManager();
				plugins.LoadAllPlugins();
				
				// Load game configurations
				Logger.WriteLogLine("RealMain: Loading game cfg");
				LoadAllGameConfigurations();

				// Create editing modes
				Logger.WriteLogLine("RealMain: Creating editing modes manager");
				editing = new EditingManager();
				
				// Now that all settings have been combined (core & plugins) apply the defaults
				Logger.WriteLogLine("RealMain: Applying configuration settings...");
				actions.ApplyDefaultShortcutKeys();
				mainwindow.ApplyShortcutKeys();
				foreach(ConfigurationInfo info in configs) info.ApplyDefaults(null);
				
				// Load compiler configurations
				Logger.WriteLogLine("RealMain: Loading compiler configs...");
				LoadAllCompilerConfigurations();

				// Load nodebuilder configurations
				Logger.WriteLogLine("RealMain: Loading nodebuilder configs...");
				LoadAllNodebuilderConfigurations();
				
				// Load script configurations
				Logger.WriteLogLine("RealMain: Loading script configs...");
				LoadAllScriptConfigurations();
				
				// Load color settings
				Logger.WriteLogLine("RealMain: Loading color settings...");
				colors = new ColorCollection(settings.Config);
				
				// Create types manager
				Logger.WriteLogLine("RealMain: Creating types manager...");
				types = new TypesManager();
				
				// Do auto map loading when window is delayed
				if(delaymainwindow)
					mainwindow.PerformAutoMapLoading();
				
				// All done
				Logger.WriteLogLine("RealMain: Startup done\n-------------------\n");
				mainwindow.DisplayReady();
				
				// Show any errors if preferred
				if(errorlogger.IsErrorAdded)
				{
					mainwindow.DisplayStatus(StatusType.Warning, "There were errors during program statup!");
					if(!delaymainwindow && General.Settings.ShowErrorsWindow) mainwindow.ShowErrors();
				}
				
				// Run application from the main window
				Application.Run(mainwindow);
			}
			else
			{
				// Terminate
				Terminate(false);
			}
		}

		// This handles DLL linking errors
		private static System.Reflection.Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
		{
			// Check if SlimDX failed loading
			if(args.Name.Contains("SlimDX")) AskDownloadDirectX();

			// Return null
			return null;
		}
		
		// This asks the user to download DirectX
		private static void AskDownloadDirectX()
		{
			// Cancel loading map from command-line parameters, if any.
			// This causes problems, because when the window is shown, the map will
			// be loaded and DirectX is initialized (which we seem to be missing)
			CancelAutoMapLoad();
			
			// Ask the user to download DirectX
			if(MessageBox.Show("This application requires the latest version of Microsoft DirectX installed on your computer." + Environment.NewLine +
				"Do you want to install and/or update Microsoft DirectX now?", "DirectX Error", System.Windows.Forms.MessageBoxButtons.YesNo,
				System.Windows.Forms.MessageBoxIcon.Exclamation) == System.Windows.Forms.DialogResult.Yes)
			{
                // Open DX web setup
                //System.Diagnostics.Process.Start("http://www.microsoft.com/downloads/details.aspx?FamilyId=2DA43D38-DB71-4C1B-BC6A-9B6652CD92A3").WaitForExit(1000);
                try
                {
                    Process.Start(Path.Combine(setuppath, "dxwebsetup.exe")).WaitForExit(1000);
                }
                catch
                {
                    Process.Start(@"https://www.microsoft.com/en-us/download/details.aspx?id=35&44F86079-8679-400C-BFF2-9CA5F2BCBDFC=1");
                }
			}

			// End program here
			Terminate(false);
		}

		// This parses the command line arguments
		private static void ParseCommandLineArgs(string[] args)
		{
			autoloadresources = new DataLocationList();
			
			// Keep a copy
			cmdargs = args;
			
			// Make a queue so we can parse the values from left to right
			Queue<string> argslist = new Queue<string>(args);
			
			// Parse list
			while(argslist.Count > 0)
			{
				// Get next arg
				string curarg = argslist.Dequeue();
				
				// Delay window?
				if(string.Compare(curarg, "-DELAYWINDOW", true) == 0)
				{
					// Delay showing the main window
					delaymainwindow = true;
				}
				// No settings?
				else if(string.Compare(curarg, "-NOSETTINGS", true) == 0)
				{
					// Don't load or save program settings
					nosettings = true;
				}
				// Map name info?
				else if(string.Compare(curarg, "-MAP", true) == 0)
				{
					// Store next arg as map name information
					autoloadmap = argslist.Dequeue();
				}
				// Config name info?
				else if((string.Compare(curarg, "-CFG", true) == 0) ||
						(string.Compare(curarg, "-CONFIG", true) == 0))
				{
					// Store next arg as config filename information
					autoloadconfig = argslist.Dequeue();
				}
				// Strict patches rules?
				else if(string.Compare(curarg, "-STRICTPATCHES", true) == 0)
				{
					autoloadstrictpatches = true;
				}
				// Resource?
				else if(string.Compare(curarg, "-RESOURCE", true) == 0)
				{
					DataLocation dl = new DataLocation();

					// Parse resource type
					string resourcetype = argslist.Dequeue();
					if(string.Compare(resourcetype, "WAD", true) == 0)
						dl.type = DataLocation.RESOURCE_WAD;
					else if(string.Compare(resourcetype, "DIR", true) == 0)
						dl.type = DataLocation.RESOURCE_DIRECTORY;
					else if(string.Compare(resourcetype, "PK3", true) == 0)
						dl.type = DataLocation.RESOURCE_PK3;
					else
					{
						Logger.WriteLogLine("Unexpected resource type \"" + resourcetype + "\" in program parameters. Expected \"wad\", \"dir\" or \"pk3\".");
						break;
					}

					// We continue parsing args until an existing filename is found
					// all other arguments must be one of the optional keywords.
					while(string.IsNullOrEmpty(dl.location))
					{
						curarg = argslist.Dequeue();

						if((string.Compare(curarg, "ROOTTEXTURES", true) == 0) &&
						   (dl.type == DataLocation.RESOURCE_DIRECTORY))
						{
							// Load images in the root directory of the resource as textures
							dl.option1 = true;
						}
						else if((string.Compare(curarg, "ROOTFLATS", true) == 0) &&
								(dl.type == DataLocation.RESOURCE_DIRECTORY))
						{
							// Load images in the root directory of the resource as flats
							dl.option2 = true;
						}
						else if((string.Compare(curarg, "STRICTPATCHES", true) == 0) &&
								(dl.type == DataLocation.RESOURCE_WAD))
						{
							// Use strict rules for patches
							dl.option1 = true;
						}
						else if(string.Compare(curarg, "NOTEST", true) == 0)
						{
							// Exclude this resource from testing parameters
							dl.notfortesting = true;
						}
						else
						{
							// This must be an existing file, or it is an invalid argument
							if(dl.type == DataLocation.RESOURCE_DIRECTORY)
							{
								if(Directory.Exists(curarg))
									dl.location = curarg;
							}
							else
							{
								if(File.Exists(curarg))
									dl.location = curarg;
							}
							
							if(string.IsNullOrEmpty(dl.location))
							{
								Logger.WriteLogLine("Unexpected argument \"" + curarg + "\" in program parameters. Expected a valid resource option or a resource filename.");
								break;
							}
						}
					}

					// Add resource to list
					if(!string.IsNullOrEmpty(dl.location))
						autoloadresources.Add(dl);
				}
				// Every other arg
				else
				{
					// No command to load file yet?
					if(autoloadfile == null)
					{
						// Check if this is a file we can load
						if(File.Exists(curarg))
						{
							// Load this file!
							autoloadfile = curarg.Trim();
						}
						else
						{
							// Note in the log that we cannot find this file
							Logger.WriteLogLine("Cannot find the specified file \"" + curarg + "\"");
						}
					}
				}
			}
		}
		
		// This cancels automatic map loading
		internal static void CancelAutoMapLoad()
		{
			autoloadfile = null;
		}
		
		#endregion
		
		#region ================== Terminate

		// This is for plugins to use
		public static void Exit(bool properexit)
		{
			// Plugin wants to exit nicely?
			if(properexit)
			{
				// Close dialog forms first
				while((Form.ActiveForm != mainwindow) && (Form.ActiveForm != null))
					Form.ActiveForm.Close();

				// Close main window
				mainwindow.Close();
			}
			else
			{
				// Terminate, no questions asked
				Terminate(true);
			}
		}
		
		// This terminates the program
		internal static void Terminate(bool properexit)
		{
			// Terminate properly?
			if(properexit)
			{
				Logger.WriteLogLine("Terminate: Termination requested");
				
				// Unbind static methods from actions
				General.Actions.UnbindMethods(typeof(General));
				
				// Save colors
				colors.SaveColors(settings.Config);
				
				// Save action controls
				actions.SaveSettings();
				
				// Save game configuration settings
				foreach(ConfigurationInfo ci in configs) ci.SaveSettings();
				
				// Save settings configuration
				if(!General.NoSettings)
				{
					Logger.WriteLogLine("Terminate: Saving program configuration...");
					settings.Save(Path.Combine(settingspath, SETTINGS_FILE));
				}
				
				// Clean up
				if(map != null) map.Dispose(); map = null;
				if(editing != null) editing.Dispose(); editing = null;
				if(mainwindow != null) mainwindow.Dispose();
				if(actions != null) actions.Dispose();
				if(plugins != null) plugins.Dispose();
				if(types != null) types.Dispose();
				try { D3DDevice.Terminate(); } catch(Exception) { }

				// Application ends right here. right now.
				Logger.WriteLogLine("Terminate: Termination done");
                Logger.bLogging = false;
                Logger.StopLogging();
                Application.Exit();
			}
			else
			{
				// Just end now
				Logger.WriteLogLine("Terminate: Immediate program termination");
                Logger.StopLogging();
                Application.Exit();
			}

			// Die.
			Process.GetCurrentProcess().Kill();
		}
		
		#endregion
		
		#region ================== Management
		
		// This creates a new map
		[BeginAction("newmap")]
		internal static void NewMap()
		{
            Logger.WriteLogLine("-------------------\nNewMap()");

            MapOptions newoptions = new MapOptions();
			MapOptionsForm optionswindow;
			
			// Cancel volatile mode, if any
			General.Editing.DisengageVolatileMode();
			
			// Ask the user to save changes (if any)
			if(General.AskSaveMap())
			{
				// Open map options dialog
				optionswindow = new MapOptionsForm(newoptions);
				optionswindow.IsForNewMap = true;
				if(optionswindow.ShowDialog(mainwindow) == DialogResult.OK)
				{
					// Display status
					mainwindow.DisplayStatus(StatusType.Busy, "Creating new map...");
					Cursor.Current = Cursors.WaitCursor;
					
					// Clear the display
					mainwindow.ClearDisplay();

					// Trash the current map, if any
					if(map != null) map.Dispose();
					
					// Let the plugins know
					plugins.OnMapNewBegin();

					// Set this to false so we can see if errors are added
					General.ErrorLogger.IsErrorAdded = false;
					
					// Create map manager with given options
					map = new MapManager();
					if(map.InitializeNewMap(newoptions))
					{
						// Done
					}
					else
					{
						// Unable to create map manager
						map.Dispose();
						map = null;

						// Show splash logo on display
						mainwindow.ShowSplashDisplay();
					}

					// Let the plugins know
					plugins.OnMapNewEnd();

					// All done
					settings.FindDefaultDrawSettings();
					mainwindow.SetupInterface();
					mainwindow.RedrawDisplay();
					mainwindow.UpdateThingsFilters();
					mainwindow.UpdateInterface();
					mainwindow.HideInfo();

					if(errorlogger.IsErrorAdded)
					{
						// Show any errors if preferred
						mainwindow.DisplayStatus(StatusType.Warning, "There were errors during loading!");
						if(!delaymainwindow && General.Settings.ShowErrorsWindow) mainwindow.ShowErrors();
					}
					else
						mainwindow.DisplayReady();
					
					Cursor.Current = Cursors.Default;
				}
			}
		}

		// This closes the current map
		[BeginAction("closemap")]
		internal static void ActionCloseMap() { CloseMap(); }
		internal static bool CloseMap()
		{
			// Cancel volatile mode, if any
			General.Editing.DisengageVolatileMode();

			// Ask the user to save changes (if any)
			if(General.AskSaveMap())
			{
				// Display status
				mainwindow.DisplayStatus(StatusType.Busy, "Closing map...");
				Logger.WriteLogLine("-------------------\nCloseMap: Unloading map...");
				Cursor.Current = Cursors.WaitCursor;
				
				// Trash the current map
				if(map != null) map.Dispose();
				map = null;
				
				// Clear errors
				General.ErrorLogger.Clear();
				
				// Show splash logo on display
				mainwindow.ShowSplashDisplay();
				
				// Done
				Cursor.Current = Cursors.Default;
				editing.UpdateCurrentEditModes();
				mainwindow.SetupInterface();
				mainwindow.RedrawDisplay();
				mainwindow.HideInfo();
				mainwindow.UpdateThingsFilters();
				mainwindow.UpdateInterface();
				mainwindow.DisplayReady();
				Logger.WriteLogLine("CloseMap: Map unload done");
				return true;
			}
			else
			{
				// User cancelled
				return false;
			}
		}

		// This loads a map from file
		[BeginAction("openmap")]
		internal static void OpenMap()
		{
			OpenFileDialog openfile;

			// Cancel volatile mode, if any
			General.Editing.DisengageVolatileMode();

			// Open map file dialog
			openfile = new OpenFileDialog();
			openfile.Filter = "Doom WAD Files (*.wad)|*.wad";
			openfile.Title = "Open Map";
			openfile.AddExtension = false;
			openfile.CheckFileExists = true;
			openfile.Multiselect = false;
			openfile.ValidateNames = true;
			if(openfile.ShowDialog(mainwindow) == DialogResult.OK)
			{
				// Update main window
				mainwindow.Update();

				// Open map file
				OpenMapFile(openfile.FileName, null);
			}

			openfile.Dispose();
		}
		
		// This opens the specified file
		internal static void OpenMapFile(string filename, MapOptions options)
		{
			OpenMapOptionsForm openmapwindow;

			// Cancel volatile mode, if any
			General.Editing.DisengageVolatileMode();
			
			// Ask the user to save changes (if any)
			if(General.AskSaveMap())
			{
				// Open map options dialog
				if(options != null)
					openmapwindow = new OpenMapOptionsForm(filename, options);
				else
					openmapwindow = new OpenMapOptionsForm(filename);

				if(openmapwindow.ShowDialog(mainwindow) == DialogResult.OK)
					OpenMapFileWithOptions(filename, openmapwindow.Options);
			}
		}
		
		// This opens the specified file without dialog
		internal static void OpenMapFileWithOptions(string filename, MapOptions options)
		{
            Logger.WriteLogLine("-------------------\nOpenMapFileWithOptions()");
            // Display status
            mainwindow.DisplayStatus(StatusType.Busy, "Opening map file...");
			Cursor.Current = Cursors.WaitCursor;

			// Clear the display
			mainwindow.ClearDisplay();

			// Trash the current map, if any
			if(map != null) map.Dispose();

			// Let the plugins know
			plugins.OnMapOpenBegin();

			// Set this to false so we can see if errors are added
			General.ErrorLogger.IsErrorAdded = false;

			// Create map manager with given options
			map = new MapManager();
			if(map.InitializeOpenMap(filename, options))
			{
				// Add recent file
				mainwindow.AddRecentFile(filename);
			}
			else
			{
				// Unable to create map manager
				map.Dispose();
				map = null;

				// Show splash logo on display
				mainwindow.ShowSplashDisplay();
			}

			// Let the plugins know
			plugins.OnMapOpenEnd();

			// All done
			settings.FindDefaultDrawSettings();
			mainwindow.SetupInterface();
			mainwindow.RedrawDisplay();
			mainwindow.UpdateThingsFilters();
			mainwindow.UpdateInterface();
			mainwindow.HideInfo();

			if(errorlogger.IsErrorAdded)
			{
				// Show any errors if preferred
				mainwindow.DisplayStatus(StatusType.Warning, "There were errors during loading!");
				if(!delaymainwindow && General.Settings.ShowErrorsWindow) mainwindow.ShowErrors();
			}
			else
				mainwindow.DisplayReady();
			
			Cursor.Current = Cursors.Default;
		}
		
		// This saves the current map
		// Returns tre when saved, false when cancelled or failed
		[BeginAction("savemap")]
		internal static void ActionSaveMap() { SaveMap(); }
		internal static bool SaveMap()
		{
            Logger.WriteLogLine("SaveMap() called");
			bool result = false;

			if(map == null)
				return false;
			
			// Cancel volatile mode, if any
			General.Editing.DisengageVolatileMode();
			
			// Check if a wad file is known
			if(map.FilePathName == "")
			{
				// Call to SaveMapAs
				result = SaveMapAs();
			}
			else
			{
				// Display status
				mainwindow.DisplayStatus(StatusType.Busy, "Saving map file...");
				Cursor.Current = Cursors.WaitCursor;

				// Set this to false so we can see if errors are added
				General.ErrorLogger.IsErrorAdded = false;
				
				// Save the map
				General.Plugins.OnMapSaveBegin(SavePurpose.Normal);
				if(map.SaveMap(map.FilePathName, SavePurpose.Normal))
				{
					// Add recent file
					mainwindow.AddRecentFile(map.FilePathName);
					result = true;
				}
				General.Plugins.OnMapSaveEnd(SavePurpose.Normal);

				// All done
				mainwindow.UpdateInterface();

				if(errorlogger.IsErrorAdded)
				{
					// Show any errors if preferred
					mainwindow.DisplayStatus(StatusType.Warning, "There were errors during saving!");
					if(!delaymainwindow && General.Settings.ShowErrorsWindow) mainwindow.ShowErrors();
				}
				else
					mainwindow.DisplayStatus(StatusType.Info, "Map saved in " + map.FileTitle + ".");

				Cursor.Current = Cursors.Default;
			}

			return result;
		}


		// This saves the current map as a different file
		// Returns tre when saved, false when cancelled or failed
		[BeginAction("savemapas")]
		internal static void ActionSaveMapAs() { SaveMapAs(); }
		internal static bool SaveMapAs()
		{
            Logger.WriteLogLine("SaveMapAs() called");
			SaveFileDialog savefile;
			bool result = false;

			if(map == null)
				return false;

			// Cancel volatile mode, if any
			General.Editing.DisengageVolatileMode();

			// Show save as dialog
			savefile = new SaveFileDialog();
			savefile.Filter = "Doom WAD Files (*.wad)|*.wad";
			savefile.Title = "Save Map As";
			savefile.AddExtension = true;
			savefile.CheckPathExists = true;
			savefile.OverwritePrompt = true;
			savefile.ValidateNames = true;
			if(savefile.ShowDialog(mainwindow) == DialogResult.OK)
			{
				// Check if we're saving to the same file as the original.
				// Because some muppets use Save As even when saving to the same file.
				string currentfilename = (map.FilePathName.Length > 0) ? Path.GetFullPath(map.FilePathName).ToLowerInvariant() : "";
				string savefilename = Path.GetFullPath(savefile.FileName).ToLowerInvariant();
				if(currentfilename == savefilename)
				{
					SaveMap();
				}
				else
				{
					// Display status
					mainwindow.DisplayStatus(StatusType.Busy, "Saving map file...");
					Cursor.Current = Cursors.WaitCursor;
					
					// Set this to false so we can see if errors are added
					General.ErrorLogger.IsErrorAdded = false;
					
					// Save the map
					General.Plugins.OnMapSaveBegin(SavePurpose.AsNewFile);
					if(map.SaveMap(savefile.FileName, SavePurpose.AsNewFile))
					{
						// Add recent file
						mainwindow.AddRecentFile(map.FilePathName);
						result = true;
					}
					General.Plugins.OnMapSaveEnd(SavePurpose.AsNewFile);
					
					// All done
					mainwindow.UpdateInterface();
					
					if(errorlogger.IsErrorAdded)
					{
						// Show any errors if preferred
						mainwindow.DisplayStatus(StatusType.Warning, "There were errors during saving!");
						if(!delaymainwindow && General.Settings.ShowErrorsWindow) mainwindow.ShowErrors();
					}
					else
						mainwindow.DisplayStatus(StatusType.Info, "Map saved in " + map.FileTitle + ".");
					
					Cursor.Current = Cursors.Default;
				}
			}
			
			savefile.Dispose();
			return result;
		}


		// This saves the current map as a different file
		// Returns tre when saved, false when cancelled or failed
		[BeginAction("savemapinto")]
		internal static void ActionSaveMapInto() { SaveMapInto(); }
		internal static bool SaveMapInto()
		{
            Logger.WriteLogLine("SaveMapInto called");
			SaveFileDialog savefile;
			bool result = false;

			if(map == null)
				return false;

			// Cancel volatile mode, if any
			General.Editing.DisengageVolatileMode();

			// Show save as dialog
			savefile = new SaveFileDialog();
			savefile.Filter = "Doom WAD Files (*.wad)|*.wad";
			savefile.Title = "Save Map Into";
			savefile.AddExtension = true;
			savefile.CheckPathExists = true;
			savefile.OverwritePrompt = false;
			savefile.ValidateNames = true;
			if(savefile.ShowDialog(mainwindow) == DialogResult.OK)
			{
				// Display status
				mainwindow.DisplayStatus(StatusType.Busy, "Saving map file...");
				Cursor.Current = Cursors.WaitCursor;

				// Set this to false so we can see if errors are added
				General.ErrorLogger.IsErrorAdded = false;
				
				// Save the map
				General.Plugins.OnMapSaveBegin(SavePurpose.IntoFile);
				if(map.SaveMap(savefile.FileName, SavePurpose.IntoFile))
				{
					// Add recent file
					mainwindow.AddRecentFile(map.FilePathName);
					result = true;
				}
				General.Plugins.OnMapSaveEnd(SavePurpose.IntoFile);

				// All done
				mainwindow.UpdateInterface();

				if(errorlogger.IsErrorAdded)
				{
					// Show any errors if preferred
					mainwindow.DisplayStatus(StatusType.Warning, "There were errors during saving!");
					if(!delaymainwindow && General.Settings.ShowErrorsWindow) mainwindow.ShowErrors();
				}
				else
					mainwindow.DisplayStatus(StatusType.Info, "Map saved into " + map.FileTitle + ".");

				Cursor.Current = Cursors.Default;
			}

			savefile.Dispose();
			return result;
		}
		
		// This asks to save the map if needed
		// Returns false when action was cancelled
		internal static bool AskSaveMap()
		{
			DialogResult result;
			
			// Map open and not saved?
			if(map != null)
			{
				if(map.IsChanged)
				{
					// Ask to save changes
					result = MessageBox.Show(mainwindow, "Do you want to save changes to " + map.FileTitle + " (" + map.Options.CurrentName + ")?", Application.ProductName, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
					if(result == DialogResult.Yes)
					{
						// Save map
						if(SaveMap())
						{
							// Ask to save changes to scripts
							return map.AskSaveScriptChanges();
						}
						else
						{
							// Failed to save map
							return false;
						}
					}
					else if(result == DialogResult.Cancel)
					{
						// Abort
						return false;
					}
					else
					{
						// Ask to save changes to scripts
						return map.AskSaveScriptChanges();
					}
				}
				else
				{
					// Ask to save changes to scripts
					return map.AskSaveScriptChanges();
				}
			}
			else
			{
				return true;
			}
		}
		
		#endregion

		#region ================== Debug
		
		// This shows a major failure
		public static void Fail(string message)
		{
			Logger.WriteLogLine("-------------------\nFAIL: " + message);
			Debug.Fail(message);
			Terminate(false);
		}

        // This is mainly here for support w legacy plugins
        public static void WriteLogLine(string line)
        {
            Logger.WriteLogLine(line);
        }

        // This is mainly here for support w legacy plugins
        public static void WriteLog(string text)
        {
            Logger.WriteLogLine(text);
        }


        #endregion

        #region ================== Tools

        // This swaps two pointers
        public static void Swap<T>(ref T a, ref T b)
		{
			T t = a;
			a = b;
			b = t;
		}

		// This calculates the bits needed for a number
		public static int BitsForInt(int v)
		{
			int[] LOGTABLE = new int[] {
			  0, 0, 1, 1, 2, 2, 2, 2, 3, 3, 3, 3, 3, 3, 3, 3,
			  4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4,
			  5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5,
			  5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5,
			  6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6,
			  6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6,
			  6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6,
			  6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6,
			  7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7,
			  7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7,
			  7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7,
			  7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7,
			  7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7,
			  7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7,
			  7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7,
			  7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7 };
			
			int r;	// r will be lg(v)
			int t, tt;

			if(Int2Bool(tt = v >> 16))
			{
				r = Int2Bool(t = tt >> 8) ? 24 + LOGTABLE[t] : 16 + LOGTABLE[tt];
			}
			else
			{
				r = Int2Bool(t = v >> 8) ? 8 + LOGTABLE[t] : LOGTABLE[v];
			}
			
			return r;
		}
		
		// This clamps a value
		public static float Clamp(float value, float min, float max)
		{
			return Math.Min(Math.Max(min, value), max);
		}

		// This clamps a value
		public static int Clamp(int value, int min, int max)
		{
			return Math.Min(Math.Max(min, value), max);
		}

		// This clamps a value
		public static byte Clamp(byte value, byte min, byte max)
		{
			return Math.Min(Math.Max(min, value), max);
		}
		
		// This returns an element from a collection by index
		public static T GetByIndex<T>(ICollection<T> collection, int index)
		{
			IEnumerator<T> e = collection.GetEnumerator();
			for(int i = -1; i < index; i++) e.MoveNext();
			return e.Current;
		}

		// This returns the next power of 2
		public static int NextPowerOf2(int v)
		{
			int p = 0;

			// Continue increasing until higher than v
			while(Math.Pow(2, p) < v) p++;

			// Return power
			return (int)Math.Pow(2, p);
		}
		
		// Convert bool to integer
		internal static int Bool2Int(bool v)
		{
			return v ? 1 : 0;
		}

		// Convert integer to bool
		internal static bool Int2Bool(int v)
		{
			return (v != 0);
		}

		// This shows a message and logs the message
		public static DialogResult ShowErrorMessage(string message, MessageBoxButtons buttons)
		{
			Cursor oldcursor;
			DialogResult result;
			
			// Log the message
			Logger.WriteLogLine(message);
			
			// Use normal cursor
			oldcursor = Cursor.Current;
			Cursor.Current = Cursors.Default;
			
			// Show message
			IWin32Window window = null;
			if((Form.ActiveForm != null) && Form.ActiveForm.Visible) window = Form.ActiveForm;
			result = MessageBox.Show(window, message, Application.ProductName, buttons, MessageBoxIcon.Error);

			// Restore old cursor
			Cursor.Current = oldcursor;
			
			// Return result
			return result;
		}

		// This shows a message and logs the message
		public static DialogResult ShowWarningMessage(string message, MessageBoxButtons buttons)
		{
			return ShowWarningMessage(message, buttons, MessageBoxDefaultButton.Button1);
		}

		// This shows a message and logs the message
		public static DialogResult ShowWarningMessage(string message, MessageBoxButtons buttons, MessageBoxDefaultButton defaultbutton)
		{
			Cursor oldcursor;
			DialogResult result;

            // Log the message
            Logger.WriteLogLine(message);

			// Use normal cursor
			oldcursor = Cursor.Current;
			Cursor.Current = Cursors.Default;

			// Show message
			IWin32Window window = null;
			if((Form.ActiveForm != null) && Form.ActiveForm.Visible) window = Form.ActiveForm;
			result = MessageBox.Show(window, message, Application.ProductName, buttons, MessageBoxIcon.Warning, defaultbutton);

			// Restore old cursor
			Cursor.Current = oldcursor;

			// Return result
			return result;
		}

		// This shows the reference manual
		public static void ShowHelp(string pagefile)
		{
			ShowHelp(pagefile, HELP_FILE);
		}

		// This shows the reference manual
		public static void ShowHelp(string pagefile, string chmfile)
		{
			// Check if the file can be found in the root
			string filepathname = Path.Combine(apppath, chmfile);
			if(!File.Exists(filepathname))
			{
				// Check if the file exists in the plugins directory
				filepathname = Path.Combine(pluginspath, chmfile);
				if(!File.Exists(filepathname))
				{
                    // Fail
                    Logger.WriteLogLine("ERROR: Can't find the help file \"" + chmfile + "\"");
					return;
				}
			}
			
			// Show help file
			Help.ShowHelp(mainwindow, filepathname, HelpNavigator.Topic, pagefile);
		}

		// This returns a unique temp filename
		internal static string MakeTempFilename(string tempdir)
		{
			return MakeTempFilename(tempdir, "tmp");
		}

		// This returns a unique temp filename
		internal static string MakeTempFilename(string tempdir, string extension)
		{
			string filename;
			string chars = "abcdefghijklmnopqrstuvwxyz1234567890";
			Random rnd = Random;
			int i;

			do
			{
				// Generate a filename
				filename = "";
				for(i = 0; i < 8; i++) filename += chars[rnd.Next(chars.Length)];
				filename = Path.Combine(tempdir, filename + "." + extension);
			}
			// Continue while file is not unique
			while(File.Exists(filename) || Directory.Exists(filename));

			// Return the filename
			return filename;
		}

		// This returns a unique temp directory name
		internal static string MakeTempDirname()
		{
			string dirname;
			const string chars = "abcdefghijklmnopqrstuvwxyz1234567890";
            Random rnd = Random;
            int i;

			do
			{
				// Generate a filename
				dirname = "";
				for(i = 0; i < 8; i++) dirname += chars[rnd.Next(chars.Length)];
				dirname = Path.Combine(temppath, dirname);
			}
			// Continue while file is not unique
			while(File.Exists(dirname) || Directory.Exists(dirname));

			// Return the filename
			return dirname;
		}

		// This shows an image in a panel either zoomed or centered depending on size
		public static void DisplayZoomedImage(Panel panel, Image image)
		{
			// Set the image
			panel.BackgroundImage = image;
			
			// Image not null?
			if(image != null)
			{
				// Small enough to fit in panel?
				if((image.Size.Width < panel.ClientRectangle.Width) &&
				   (image.Size.Height < panel.ClientRectangle.Height))
				{
					// Display centered
					panel.BackgroundImageLayout = ImageLayout.Center;
				}
				else
				{
					// Display zoomed
					panel.BackgroundImageLayout = ImageLayout.Zoom;
				}
			}
		}

		// This calculates the new rectangle when one is scaled into another keeping aspect ratio
		public static RectangleF MakeZoomedRect(Size source, RectangleF target)
		{
			return MakeZoomedRect(new SizeF((int)source.Width, (int)source.Height), target);
		}

		// This calculates the new rectangle when one is scaled into another keeping aspect ratio
		public static RectangleF MakeZoomedRect(Size source, Rectangle target)
		{
			return MakeZoomedRect(new SizeF((int)source.Width, (int)source.Height),
								  new RectangleF((int)target.Left, (int)target.Top, (int)target.Width, (int)target.Height));
		}
		
		// This calculates the new rectangle when one is scaled into another keeping aspect ratio
		public static RectangleF MakeZoomedRect(SizeF source, RectangleF target)
		{
			float scale;
			
			// Image fits?
			if((source.Width <= target.Width) &&
			   (source.Height <= target.Height))
			{
				// Just center
				scale = 1.0f;
			}
			// Image is wider than tall?
			else if((source.Width - target.Width) > (source.Height - target.Height))
			{
				// Scale down by width
				scale = target.Width / source.Width;
			}
			else
			{
				// Scale down by height
				scale = target.Height / source.Height;
			}
			
			// Return centered and scaled
			return new RectangleF(target.Left + (target.Width - source.Width * scale) * 0.5f,
								  target.Top + (target.Height - source.Height * scale) * 0.5f,
								  source.Width * scale, source.Height * scale);
		}

		// This opens a URL in the default browser
		public static void OpenWebsite(string url)
		{
			RegistryKey key = null;
			Process p = null;
			string browser;

			try
			{
				// Get the registry key where default browser is stored
				key = Registry.ClassesRoot.OpenSubKey(@"HTTP\shell\open\command", false);

				// Trim off quotes
				browser = key.GetValue(null).ToString().ToLower().Replace("\"", "");

				// String doesnt end in EXE?
				if(!browser.EndsWith("exe"))
				{
					// Get rid of everything after the ".exe"
					browser = browser.Substring(0, browser.LastIndexOf(".exe") + 4);
				}
			}
			finally
			{
				// Clean up
				if(key != null) key.Close();
			}

			try
			{
				// Fork a process
				p = new Process();
				p.StartInfo.FileName = browser;
				p.StartInfo.Arguments = url;
				p.Start();
			}
			catch(Exception) { }

			// Clean up
			if(p != null) p.Dispose();
		}
		
		// This returns the short path name for a file
		public static string GetShortFilePath(string longpath)
		{
			int maxlen = 256;
			StringBuilder shortname = new StringBuilder(maxlen);
			uint len = GetShortPathName(longpath, shortname, (uint)maxlen);
			return shortname.ToString();
		}

        #endregion

        /*
		[BeginAction("testaction")]
		internal static void TestAction()
		{
			ScriptEditorForm t = new ScriptEditorForm();
			t.ShowDialog(mainwindow);
			t.Dispose();
		}
		*/

        // ano - general skeleton of this is same as some mxd's gzdb code,
        // but content is just different.  we also don't try to keep running
        // after an exception.
        #region ==================  Uncaught exceptions handling
        private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            HandleException(e.Exception);
        }
        
        private static void CurrentDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            HandleException((Exception)e.ExceptionObject);
        }

        private static void HandleException(Exception e)
        {
            bool bAttemptSave = map != null && !map.IsSaving && !map.IsDisposed;
            string crashlogfile = Path.Combine(settingspath, CRASH_LOG_FILE);
            string crashsavefile = Path.Combine(settingspath, CRASH_SAVE_FILE);
            string errorMsg = "UNHAPPY ERROR :(\nyou should save the "
                            + crashlogfile
                            + " and post it at \nhttps://github.com/anotak/doombuilderx/issues\n--------------\n";
            if (bAttemptSave)
            {
                errorMsg += "attempting to save map at " + crashsavefile + "\n";
            }

            if (e == null)
            {
                errorMsg += "bizarre null exception..\n";
                Logger.WriteLogLine("extremely bizarre: null exception caught");
            }
            else
            {
                Logger.WriteLogLine("Exception: " + e.Message);

                Logger.WriteLogLine("Exception: " + e.StackTrace);

                int i = 1;
                Exception a = e;
                while (a.InnerException != null)
                {
                    a = a.InnerException;
                    Logger.WriteLogLine("InnerException " + i + ": " + a.Message);

                    Logger.WriteLogLine("InnerException " + i + ": " + a.StackTrace);
                    i++;
                }
                errorMsg += e.Message + "\n";
                errorMsg += e.StackTrace + "\n";
            }
            MessageBox.Show(errorMsg, "Exception!", MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Exclamation);

            if (bAttemptSave)
            {
                try
                {
                    Logger.WriteLogLine("attempting to crash-save map to " + crashsavefile);
                    Map.SaveMap(crashsavefile, SavePurpose.AsNewFile);
                }
                catch (Exception a)
                {
                    Logger.WriteLogLine("Crashsaving Exception: " + a.Message);
                    Logger.WriteLogLine("Crashsaving Exception: " + a.StackTrace);
                    int i = 1;
                    while (a.InnerException != null)
                    {
                        a = a.InnerException;
                        Logger.WriteLogLine("Crashsaving InnerException " + i + ": " + a.Message);

                        Logger.WriteLogLine("Crashsaving InnerException " + i + ": " + a.StackTrace);
                        i++;
                    }

                    MessageBox.Show("We're really sorry, trying to save the map also failed.",
                        "Exception!", MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Exclamation);
                }
            }

            try
            {
                if (map != null && !map.IsDisposed)
                {
                    Logger.WriteLogLine("map info at crash time:::");
                    Logger.WriteLogLine("filename: " + map.FilePathName);
                    Logger.WriteLogLine("was saving?: " + map.IsSaving);
                    Logger.WriteLogLine("UDMF: " + map.UDMF);
                    Logger.WriteLogLine("config: " + map.ConfigSettings.Name + " ..... " + map.ConfigSettings.Filename);
                    Logger.WriteLogLine("linecount: " + map.Map.Linedefs.Count);
                    Logger.WriteLogLine("thingcount: " + map.Map.Things.Count);
                    Logger.WriteLogLine("sectorcount: " + map.Map.Sectors.Count);
                    Logger.WriteLogLine("sidedefcount: " + map.Map.Sidedefs.Count);
                    if (General.Map.UndoRedo != null)
                    {
                        List<UndoSnapshot> snapshots = map.UndoRedo.GetUndoList();
                        Logger.WriteLogLine("undo list:");
                        foreach (UndoSnapshot s in snapshots)
                        {
                            Logger.WriteLogLine(s.Description);
                        }
                        Logger.WriteLogLine("-------------\nend undo list");
                    }
                    else
                    {
                        Logger.WriteLogLine("map.UndoRedo is null");
                    }
                }
                else
                {
                    Logger.WriteLogLine("map is null or disposed at crash time");
                }
            }
            catch (Exception a)
            {
                Logger.WriteLogLine("Crash Data Logging Exception: " + a.Message);
                Logger.WriteLogLine("Crash Data Logging Exception: " + a.StackTrace);
                int i = 1;
                while (a.InnerException != null)
                {
                    a = a.InnerException;
                    Logger.WriteLogLine("Crash Data Logging InnerException " + i + ": " + a.Message);

                    Logger.WriteLogLine("Crash Data Logging InnerException " + i + ": " + a.StackTrace);
                    i++;
                }

                MessageBox.Show("We're really sorry, trying to save the map also failed.",
                    "Exception!", MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Exclamation);
            }

            Logger.WaitForBufferToClear();

            try
            {
                if (File.Exists(crashlogfile))
                {
                    File.Delete(crashlogfile);
                }
                File.Copy(Logger.logfile, crashlogfile);
            }
            catch (Exception a)
            {
                Logger.WriteLogLine("Crashlogging Exception: " + a.Message);
                Logger.WriteLogLine("Crashlogging Exception: " + a.StackTrace);
                int i = 1;
                while (a.InnerException != null)
                {
                    a = a.InnerException;
                    Logger.WriteLogLine("Crashlogging InnerException " + i + ": " + a.Message);

                    Logger.WriteLogLine("Crashlogging InnerException " + i + ": " + a.StackTrace);
                    i++;
                }
            }

            // hasta la vista, baby
            Terminate(false);
        }

        #endregion

    }
}

