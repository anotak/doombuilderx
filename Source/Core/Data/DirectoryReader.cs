
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
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;
using CodeImp.DoomBuilder.IO;

#endregion

namespace CodeImp.DoomBuilder.Data
{
	internal sealed class DirectoryReader : PK3StructuredReader
	{
		#region ================== Variables

		private DirectoryFilesList files;

		#endregion
		
		#region ================== Constructor / Disposer

		// Constructor
		public DirectoryReader(DataLocation dl) : base(dl)
		{
			Logger.WriteLogLine("Opening directory resource '" + location.location + "'");
			
			// Initialize
			files = new DirectoryFilesList(dl.location, true);
			Initialize();
			
			// We have no destructor
			GC.SuppressFinalize(this);
		}
		
		// Disposer
		public override void Dispose()
		{
			// Not already disposed?
			if(!isdisposed)
			{
				Logger.WriteLogLine("Closing directory resource '" + location.location + "'");
				
				// Done
				base.Dispose();
			}
		}
		
		#endregion

		#region ================== Textures

		// This finds and returns a patch stream
		public override Stream GetPatchData(string pname)
		{
			// Error when suspended
			if(issuspended) throw new Exception("Data reader is suspended");

			// Find in any of the wad files
			// Note the backward order, because the last wad's images have priority
			for(int i = wads.Count - 1; i >= 0; i--)
			{
				Stream data = wads[i].GetPatchData(pname);
				if(data != null) return data;
			}
			
			try
			{
				// Find in patches directory
				string path = Path.Combine(PATCHES_DIR, Path.GetDirectoryName(pname));
				string filename = FindFirstFile(path, Path.GetFileName(pname), true);
				if((filename != null) && FileExists(filename))
				{
					return LoadFile(filename);
				}
			}
			catch(Exception e)
			{
				General.ErrorLogger.Add(ErrorType.Error, e.GetType().Name + " while loading patch '" + pname + "' from directory: " + e.Message);
			}

			// Nothing found
			return null;
		}

		// This finds and returns a textue stream
		public override Stream GetTextureData(string pname)
		{
			// Error when suspended
			if(issuspended) throw new Exception("Data reader is suspended");

			// Find in any of the wad files
			// Note the backward order, because the last wad's images have priority
			for(int i = wads.Count - 1; i >= 0; i--)
			{
				Stream data = wads[i].GetTextureData(pname);
				if(data != null) return data;
			}
			
			try
			{
				// Find in patches directory
				string path = Path.Combine(TEXTURES_DIR, Path.GetDirectoryName(pname));
				string filename = FindFirstFile(path, Path.GetFileName(pname), true);
				if((filename != null) && FileExists(filename))
				{
					return LoadFile(filename);
				}
			}
			catch(Exception e)
			{
				General.ErrorLogger.Add(ErrorType.Error, e.GetType().Name + " while loading texture '" + pname + "' from directory: " + e.Message);
			}

			// Nothing found
			return null;
		}

		// This finds and returns a colormap stream
		public override Stream GetColormapData(string pname)
		{
			// Error when suspended
			if(issuspended) throw new Exception("Data reader is suspended");

			// Find in any of the wad files
			// Note the backward order, because the last wad's images have priority
			for(int i = wads.Count - 1; i >= 0; i--)
			{
				Stream data = wads[i].GetColormapData(pname);
				if(data != null) return data;
			}

			try
			{
				// Find in patches directory
				string path = Path.Combine(COLORMAPS_DIR, Path.GetDirectoryName(pname));
				string filename = FindFirstFile(path, Path.GetFileName(pname), true);
				if((filename != null) && FileExists(filename))
				{
					return LoadFile(filename);
				}
			}
			catch(Exception e)
			{
				General.ErrorLogger.Add(ErrorType.Error, e.GetType().Name + " while loading colormap '" + pname + "' from directory: " + e.Message);
			}

			// Nothing found
			return null;
		}

		#endregion

		#region ================== Sprites

		// This finds and returns a sprite stream
		public override Stream GetSpriteData(string pname)
		{
			// Error when suspended
			if(issuspended) throw new Exception("Data reader is suspended");

			// Find in any of the wad files
			for(int i = wads.Count - 1; i >= 0; i--)
			{
				Stream sprite = wads[i].GetSpriteData(pname);
				if(sprite != null) return sprite;
			}
			
			try
			{
				// Find in sprites directory
				string path = Path.Combine(SPRITES_DIR, Path.GetDirectoryName(pname));
				string filename = FindFirstFile(path, Path.GetFileName(pname), true);
				if((filename != null) && FileExists(filename))
				{
					return LoadFile(filename);
				}
			}
			catch(Exception e)
			{
				General.ErrorLogger.Add(ErrorType.Error, e.GetType().Name + " while loading sprite '" + pname + "' from directory: " + e.Message);
			}
			
			// Nothing found
			return null;
		}

		// This checks if the given sprite exists
		public override bool GetSpriteExists(string pname)
		{
			// Error when suspended
			if(issuspended) throw new Exception("Data reader is suspended");

			// Find in any of the wad files
			for(int i = wads.Count - 1; i >= 0; i--)
			{
				if(wads[i].GetSpriteExists(pname)) return true;
			}

			// Find in sprites directory
			try
			{
				string path = Path.Combine(SPRITES_DIR, Path.GetDirectoryName(pname));
				string filename = FindFirstFile(path, Path.GetFileName(pname), true);
				if((filename != null) && FileExists(filename))
				{
					return true;
				}
			}
			catch(Exception e)
			{
				General.ErrorLogger.Add(ErrorType.Error, e.GetType().Name + " while checking sprite '" + pname + "' existance in directory: " + e.Message);
			}
			
			// Nothing found
			return false;
		}

		#endregion
		
		#region ================== Methods

		// Return a short name for this data location
		public override string GetTitle()
		{
			return Path.GetFileName(location.location);
		}

		// This creates an image
		protected override ImageData CreateImage(string name, string filename, int imagetype)
		{
			switch(imagetype)
			{
				case ImageDataFormat.DOOMFLAT:
					return new FileImage(name, Path.Combine(location.location, filename), true);
				
				case ImageDataFormat.DOOMPICTURE:
					return new FileImage(name, Path.Combine(location.location, filename), false);
					
				case ImageDataFormat.DOOMCOLORMAP:
					return new ColormapImage(name);

				default:
					throw new ArgumentException("Invalid image format specified!");
					//return null;
			}
		}

		// This returns true if the specified file exists
		protected override bool FileExists(string filename)
		{
			return files.FileExists(filename);
		}
		
		// This returns all files in a given directory
		protected override string[] GetAllFiles(string path, bool subfolders)
		{
			return files.GetAllFiles(path, subfolders).ToArray();
		}
		
		// This returns all files in a given directory that have the given file title
		protected override string[] GetAllFilesWithTitle(string path, string title, bool subfolders)
		{
			return files.GetAllFilesWithTitle(path, title, subfolders).ToArray();
		}
		
		// This returns all files in a given directory that match the given extension
		protected override string[] GetFilesWithExt(string path, string extension, bool subfolders)
		{
			return files.GetAllFiles(path, extension, subfolders).ToArray();
		}

		// This finds the first file that has the specific name, regardless of file extension
		protected override string FindFirstFile(string beginswith, bool subfolders)
		{
			return files.GetFirstFile(beginswith, subfolders);
		}

		// This finds the first file that has the specific name, regardless of file extension
		protected override string FindFirstFile(string path, string beginswith, bool subfolders)
		{
			return files.GetFirstFile(path, beginswith, subfolders);
		}

		// This finds the first file that has the specific name
		protected override string FindFirstFileWithExt(string path, string beginswith, bool subfolders)
		{
			string title = Path.GetFileNameWithoutExtension(beginswith);
			string ext = Path.GetExtension(beginswith);
			if(ext.Length > 1) ext = ext.Substring(1); else ext = "";
			return files.GetFirstFile(path, title, subfolders, ext);
		}
		
		// This loads an entire file in memory and returns the stream
		// NOTE: Callers are responsible for disposing the stream!
		protected override MemoryStream LoadFile(string filename)
		{
			return new MemoryStream(File.ReadAllBytes(Path.Combine(location.location, filename)));
		}

		// This creates a temp file for the speciied file and return the absolute path to the temp file
		// NOTE: Callers are responsible for removing the temp file when done!
		protected override string CreateTempFile(string filename)
		{
			// Just copy the file
			string tempfile = General.MakeTempFilename(General.Map.TempPath, "wad");
			File.Copy(Path.Combine(location.location, filename), tempfile);
			return tempfile;
		}

		#endregion
	}
}
