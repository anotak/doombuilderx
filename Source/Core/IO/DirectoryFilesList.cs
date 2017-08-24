
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

#endregion

namespace CodeImp.DoomBuilder.IO
{
	internal sealed class DirectoryFilesList
	{
		#region ================== Variables

		private DirectoryFileEntry[] entries;
		private Dictionary<string, DirectoryFileEntry> hashedentries;
		
		#endregion

		#region ================== Properties

		public int Count { get { return entries.Length; } }

		#endregion

		#region ================== Constructor / Disposer

		// Constructor to fill list from directory and optionally subdirectories
		public DirectoryFilesList(string path, bool subdirectories)
		{
			path = Path.GetFullPath(path);
			string[] files = Directory.GetFiles(path, "*", subdirectories ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
			entries = new DirectoryFileEntry[files.Length];
			hashedentries = new Dictionary<string, DirectoryFileEntry>(files.Length);
			for(int i = 0; i < files.Length; i++)
			{
				entries[i] = new DirectoryFileEntry(files[i], path);
				string hashkey = entries[i].filepathname.ToLowerInvariant();
				if(hashedentries.ContainsKey(hashkey))
					throw new IOException("Multiple files with the same filename in the same directory are not allowed. See: \"" + entries[i].filepathname + "\"");
				hashedentries.Add(hashkey, entries[i]);
			}
		}

		// Constructor for custom list
		public DirectoryFilesList(ICollection<DirectoryFileEntry> sourceentries)
		{
			int index = 0;
			entries = new DirectoryFileEntry[sourceentries.Count];
			hashedentries = new Dictionary<string, DirectoryFileEntry>(sourceentries.Count);
			foreach(DirectoryFileEntry e in sourceentries)
			{
				entries[index] = e;
				string hashkey = e.filepathname.ToLowerInvariant();
				if(hashedentries.ContainsKey(hashkey))
					throw new IOException("Multiple files with the same filename in the same directory are not allowed. See: \"" + e.filepathname + "\"");
				hashedentries.Add(hashkey, e);
				index++;
			}
		}

		#endregion

		#region ================== Methods

		// This checks if a given file exists
		// The given file path must not be absolute
		public bool FileExists(string filepathname)
		{
			return hashedentries.ContainsKey(filepathname.ToLowerInvariant());
		}

		// This returns file information for the given file
		// The given file path must not be absolute
		public DirectoryFileEntry GetFileInfo(string filepathname)
		{
			return hashedentries[filepathname.ToLowerInvariant()];
		}
		
		// This returns a list of all files (filepathname)
		public List<string> GetAllFiles()
		{
			List<string> files = new List<string>(entries.Length);
			for(int i = 0; i < entries.Length; i++) files.Add(entries[i].filepathname);
			return files;
		}

		// This returns a list of all files optionally with subdirectories included
		public List<string> GetAllFiles(bool subdirectories)
		{
			if(subdirectories)
				return GetAllFiles();
			else
			{
				List<string> files = new List<string>(entries.Length);
				for(int i = 0; i < entries.Length; i++)
					if(entries[i].path.Length == 0) files.Add(entries[i].filepathname);
				return files;
			}
		}

		// This returns a list of all files that are in the given path and optionally in subdirectories
		public List<string> GetAllFiles(string path, bool subdirectories)
		{
			path = CorrectPath(path).ToLowerInvariant();
			if(subdirectories)
			{
				List<string> files = new List<string>(entries.Length);
				for(int i = 0; i < entries.Length; i++)
					if(entries[i].path.StartsWith(path)) files.Add(entries[i].filepathname);
				return files;
			}
			else
			{
				List<string> files = new List<string>(entries.Length);
				for(int i = 0; i < entries.Length; i++)
					if(entries[i].path == path) files.Add(entries[i].filepathname);
				return files;
			}
		}

		// This returns a list of all files that are in the given path and subdirectories and have the given title
		public List<string> GetAllFilesWithTitle(string path, string title)
		{
			path = CorrectPath(path).ToLowerInvariant();
			title = title.ToLowerInvariant();
			List<string> files = new List<string>(entries.Length);
			for(int i = 0; i < entries.Length; i++)
				if(entries[i].path.StartsWith(path) && (entries[i].filetitle == title))
					files.Add(entries[i].filepathname);
			return files;
		}

		// This returns a list of all files that are in the given path (optionally in subdirectories) and have the given title
		public List<string> GetAllFilesWithTitle(string path, string title, bool subdirectories)
		{
			if(subdirectories)
				return GetAllFilesWithTitle(path, title);
			else
			{
				path = CorrectPath(path).ToLowerInvariant();
				title = title.ToLowerInvariant();
				List<string> files = new List<string>(entries.Length);
				for(int i = 0; i < entries.Length; i++)
					if((entries[i].path == path) && (entries[i].filetitle == title))
						files.Add(entries[i].filepathname);
				return files;
			}
		}

		// This returns a list of all files that are in the given path and subdirectories and have the given extension
		public List<string> GetAllFiles(string path, string extension)
		{
			path = CorrectPath(path).ToLowerInvariant();
			extension = extension.ToLowerInvariant();
			List<string> files = new List<string>(entries.Length);
			for(int i = 0; i < entries.Length; i++)
				if(entries[i].path.StartsWith(path) && (entries[i].extension == extension))
					files.Add(entries[i].filepathname);
			return files;
		}

		// This returns a list of all files that are in the given path (optionally in subdirectories) and have the given extension
		public List<string> GetAllFiles(string path, string extension, bool subdirectories)
		{
			if(subdirectories)
				return GetAllFiles(path, extension);
			else
			{
				path = CorrectPath(path).ToLowerInvariant();
				extension = extension.ToLowerInvariant();
				List<string> files = new List<string>(entries.Length);
				for(int i = 0; i < entries.Length; i++)
					if((entries[i].path == path) && (entries[i].extension == extension))
						files.Add(entries[i].filepathname);
				return files;
			}
		}

		// This finds the first file that has the specified name, regardless of file extension
		public string GetFirstFile(string title, bool subdirectories)
		{
			title = title.ToLowerInvariant();
			if(subdirectories)
			{
				for(int i = 0; i < entries.Length; i++)
					if(entries[i].filetitle == title)
						return entries[i].filepathname;
			}
			else
			{
				for(int i = 0; i < entries.Length; i++)
					if((entries[i].filetitle == title) && (entries[i].path.Length == 0))
						return entries[i].filepathname;
			}

			return null;
		}

		// This finds the first file that has the specified name and extension
		public string GetFirstFile(string title, bool subdirectories, string extension)
		{
			title = title.ToLowerInvariant();
			extension = extension.ToLowerInvariant();
			if(subdirectories)
			{
				for(int i = 0; i < entries.Length; i++)
					if((entries[i].filetitle == title) && (entries[i].extension == extension))
						return entries[i].filepathname;
			}
			else
			{
				for(int i = 0; i < entries.Length; i++)
					if((entries[i].filetitle == title) && (entries[i].path.Length == 0) && (entries[i].extension == extension))
						return entries[i].filepathname;
			}

			return null;
		}

		// This finds the first file that has the specified name, regardless of file extension, and is in the given path
		public string GetFirstFile(string path, string title, bool subdirectories)
		{
			title = title.ToLowerInvariant();
			path = CorrectPath(path).ToLowerInvariant();
			if(subdirectories)
			{
				for(int i = 0; i < entries.Length; i++)
					if((entries[i].filetitle == title) && entries[i].path.StartsWith(path))
						return entries[i].filepathname;
			}
			else
			{
				for(int i = 0; i < entries.Length; i++)
					if((entries[i].filetitle == title) && (entries[i].path == path))
						return entries[i].filepathname;
			}

			return null;
		}

		// This finds the first file that has the specified name, is in the given path and has the given extension
		public string GetFirstFile(string path, string title, bool subdirectories, string extension)
		{
			title = title.ToLowerInvariant();
			path = CorrectPath(path).ToLowerInvariant();
			extension = extension.ToLowerInvariant();
			if(subdirectories)
			{
				for(int i = 0; i < entries.Length; i++)
					if((entries[i].filetitle == title) && entries[i].path.StartsWith(path) && (entries[i].extension == extension))
						return entries[i].filepathname;
			}
			else
			{
				for(int i = 0; i < entries.Length; i++)
					if((entries[i].filetitle == title) && (entries[i].path == path) && (entries[i].extension == extension))
						return entries[i].filepathname;
			}

			return null;
		}
		
		// This fixes a path so that it doesn't have the \ at the end
		private string CorrectPath(string path)
		{
			if(path.Length > 0)
			{
				if((path[path.Length - 1] == Path.DirectorySeparatorChar) || (path[path.Length - 1] == Path.AltDirectorySeparatorChar))
					return path.Substring(0, path.Length - 1);
				else
					return path;
			}
			else
			{
				return path;
			}
		}

		#endregion
	}
}
