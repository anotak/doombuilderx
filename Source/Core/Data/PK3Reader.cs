
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
using CodeImp.DoomBuilder.IO;

// ano - switching to sharpcompress like gzdb. based on some mxd code
using SharpCompress.Archives;
using SharpCompress.Common;
using SharpCompress.Readers;

#endregion

namespace CodeImp.DoomBuilder.Data
{
    internal sealed class PK3Reader : PK3StructuredReader
    {
        #region ================== Variables

        private DirectoryFilesList files;
        private IArchive archive; //mxd
        // ano - changed slightly from gzdb behavior but used for cleanup
        private bool bathmode = true; //mxd

        #endregion

        #region ================== Constructor / Disposer

        // Constructor
        public PK3Reader(DataLocation dl) : base(dl)
        {
            Logger.WriteLogLine("Opening PK3 resource '" + location.location + "'");

            if (!File.Exists(location.location))
                throw new FileNotFoundException("Could not find the file \"" + location.location + "\"", location.location);

            // Make list of all files
            List<DirectoryFileEntry> fileentries = new List<DirectoryFileEntry>();

            ReaderOptions options = new ReaderOptions();
            options.LeaveStreamOpen = true;

            // Create archive
            archive = ArchiveFactory.Open(location.location, options);
            
            foreach (IArchiveEntry entry in archive.Entries)
            {
                if (!entry.IsDirectory && CheckInvalidPathChars(entry.Key))
                    fileentries.Add(new DirectoryFileEntry(entry.Key));
            }

            // Get rid of archive
            archive.Dispose();
            archive = null;


            // Make files list
            files = new DirectoryFilesList(fileentries);

            // Initialize without path (because we use paths relative to the PK3 file)
            Initialize();

            // We have no destructor
            GC.SuppressFinalize(this);
        }

        // Disposer
        public override void Dispose()
        {
            // Not already disposed?
            if (!isdisposed)
            {
                Logger.WriteLogLine("Closing PK3 resource '" + location.location + "'");

                // Done
                base.Dispose();
            }
        }

        #endregion

        #region ================== Management
        
        public override void EndLoading()
        {
            // keep from holding lock over pk3 forever
            bathmode = false;

            UpdateArchive(false);
            base.EndLoading();
        }

        #endregion

        #region ================== Textures

        // This finds and returns a patch stream
        public override Stream GetPatchData(string pname)
        {
            // Error when suspended
            if (issuspended) throw new Exception("Data reader is suspended");

            // Find in any of the wad files
            // Note the backward order, because the last wad's images have priority
            for (int i = wads.Count - 1; i >= 0; i--)
            {
                Stream data = wads[i].GetPatchData(pname);
                if (data != null) return data;
            }

            // Find in patches directory
            string filename = FindFirstFile(PATCHES_DIR, pname, true);
            if ((filename != null) && FileExists(filename))
            {
                return LoadFile(filename);
            }

            // Nothing found
            return null;
        }

        // This finds and returns a texture stream
        public override Stream GetTextureData(string pname)
        {
            // Error when suspended
            if (issuspended) throw new Exception("Data reader is suspended");

            // Find in any of the wad files
            // Note the backward order, because the last wad's images have priority
            for (int i = wads.Count - 1; i >= 0; i--)
            {
                Stream data = wads[i].GetTextureData(pname);
                if (data != null) return data;
            }

            // Find in patches directory
            string filename = FindFirstFile(TEXTURES_DIR, pname, true);
            if ((filename != null) && FileExists(filename))
            {
                return LoadFile(filename);
            }

            // Nothing found
            return null;
        }

        // This finds and returns a colormap stream
        public override Stream GetColormapData(string pname)
        {
            // Error when suspended
            if (issuspended) throw new Exception("Data reader is suspended");

            // Find in any of the wad files
            // Note the backward order, because the last wad's images have priority
            for (int i = wads.Count - 1; i >= 0; i--)
            {
                Stream data = wads[i].GetColormapData(pname);
                if (data != null) return data;
            }

            // Find in patches directory
            string filename = FindFirstFile(COLORMAPS_DIR, pname, true);
            if ((filename != null) && FileExists(filename))
            {
                return LoadFile(filename);
            }

            // Nothing found
            return null;
        }

        #endregion

        #region ================== Sprites

        // This finds and returns a sprite stream
        public override Stream GetSpriteData(string pname)
        {
            string pfilename = pname.Replace('\\', '^');

            // Error when suspended
            if (issuspended) throw new Exception("Data reader is suspended");

            // Find in any of the wad files
            for (int i = wads.Count - 1; i >= 0; i--)
            {
                Stream sprite = wads[i].GetSpriteData(pname);
                if (sprite != null) return sprite;
            }

            // Find in sprites directory
            string filename = FindFirstFile(SPRITES_DIR, pfilename, true);
            if ((filename != null) && FileExists(filename))
            {
                return LoadFile(filename);
            }

            // Nothing found
            return null;
        }

        // This checks if the given sprite exists
        public override bool GetSpriteExists(string pname)
        {
            string pfilename = pname.Replace('\\', '^');

            // Error when suspended
            if (issuspended) throw new Exception("Data reader is suspended");

            // Find in any of the wad files
            for (int i = wads.Count - 1; i >= 0; i--)
            {
                if (wads[i].GetSpriteExists(pname)) return true;
            }

            // Find in sprites directory
            string filename = FindFirstFile(SPRITES_DIR, pfilename, true);
            if ((filename != null) && FileExists(filename))
            {
                return true;
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
            switch (imagetype)
            {
                case ImageDataFormat.DOOMFLAT:
                    return new PK3FileImage(this, name, filename, true);

                case ImageDataFormat.DOOMPICTURE:
                    return new PK3FileImage(this, name, filename, false);

                case ImageDataFormat.DOOMCOLORMAP:
                    return new ColormapImage(name);

                default:
                    throw new ArgumentException("Invalid image format specified!");
                    //return null;
            }
        }

        // ano - modified bc we dont have DisplayNames
        //mxd. This replicates System.IO.Path.CheckInvalidPathChars() internal function
        private bool CheckInvalidPathChars(string path)
        {
            foreach (char c in path)
            {
                int num = c;
                switch (num)
                {
                    case 34:
                    case 60:
                    case 62:
                    case 124:
                        General.ErrorLogger.Add(ErrorType.Error, "Error in \"" + location.location + "\": unsupported character \"" + c + "\" in path \"" + path + "\". File loading was skipped.");
                        return false;

                    default:
                        if (num >= 32) continue;
                        else goto case 34;
                }
            }

            return true;
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

        // This returns all files in a given directory that have the given title
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
            if (ext.Length > 1) ext = ext.Substring(1); else ext = "";
            return files.GetFirstFile(path, title, subfolders, ext);
        }

        //mxd
        private void UpdateArchive(bool enable)
        {
            if (enable && archive == null)
            {
                archive = ArchiveFactory.Open(location.location);
            }
            else if (!enable && !bathmode && archive != null)
            {
                archive.Dispose();
                archive = null;
            }
        }

        // This loads an entire file in memory and returns the stream
        // NOTE: Callers are responsible for disposing the stream!
        protected override MemoryStream LoadFile(string filename)
        {
            MemoryStream filedata = null;
            string fn = filename.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar); //mxd

            
            lock (this)
            {
                UpdateArchive(true);

                foreach (var entry in archive.Entries)
                {
                    if (entry.IsDirectory) continue;

                    // Is this the entry we are looking for?
                    if (string.Compare(entry.Key, fn, true) == 0)
                    {
                        filedata = new MemoryStream();
                        entry.WriteTo(filedata);
                        break;
                    }
                }

                UpdateArchive(false);
            }

            // Nothing found?
            if (filedata == null)
            {
                //mxd
                General.ErrorLogger.Add(ErrorType.Error, "Cannot find the file \"" + filename + "\" in archive \"" + location.location + "\".");
                return null;
            }

            filedata.Position = 0; //mxd. rewind before use
            return filedata;
        }

        // This creates a temp file for the speciied file and return the absolute path to the temp file
        // NOTE: Callers are responsible for removing the temp file when done!
        protected override string CreateTempFile(string filename)
        {
            // Just copy the file
            string tempfile = General.MakeTempFilename(General.Map.TempPath, "wad");
            MemoryStream filedata = LoadFile(filename);
            File.WriteAllBytes(tempfile, filedata.ToArray());
            filedata.Dispose();
            return tempfile;
        }

        // Public version to load a file
        internal MemoryStream ExtractFile(string filename)
        {
            return LoadFile(filename);
        }

        #endregion
    }
}
