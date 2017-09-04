
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
using ICSharpCode.SharpZipLib.Zip;

#endregion

namespace CodeImp.DoomBuilder.Data
{
    internal sealed class PK3Reader : PK3StructuredReader
    {
        #region ================== Variables

        private DirectoryFilesList files;
        private Dictionary<string, MemoryStream> cache;
        private Dictionary<string, int> accesscount_cache;

        #endregion

        #region ================== Constructor / Disposer

        // Constructor
        public PK3Reader(DataLocation dl) : base(dl)
        {
            Logger.WriteLogLine("Opening PK3 resource '" + location.location + "'");

            if (!File.Exists(location.location))
                throw new FileNotFoundException("Could not find the file \"" + location.location + "\"", location.location);

            cache = new Dictionary<string, MemoryStream>();
            accesscount_cache = new Dictionary<string, int>();
            // Open the zip file
            ZipInputStream zipstream = OpenPK3File();

            // Make list of all files
            List<DirectoryFileEntry> fileentries = new List<DirectoryFileEntry>();
            ZipEntry entry = zipstream.GetNextEntry();
            while (entry != null)
            {
                if (entry.IsFile) fileentries.Add(new DirectoryFileEntry(entry.Name));

                // Next
                entry = zipstream.GetNextEntry();
            }

            // Make files list
            files = new DirectoryFilesList(fileentries);

            // Done with the zip file
            zipstream.Close();
            zipstream.Dispose();

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

        // This opens the zip file for reading
        private ZipInputStream OpenPK3File()
        {
            FileStream filestream = File.Open(location.location, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            filestream.Seek(0, SeekOrigin.Begin);
            return new ZipInputStream(filestream);
        }

        public override void EndLoading()
        {
            cache.Clear();
            accesscount_cache.Clear();
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
                    return null;
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

        // This loads an entire file in memory and returns the stream
        // NOTE: Callers are responsible for disposing the stream!
        protected override MemoryStream LoadFile(string filename)
        {
            MemoryStream filedata = null;
            byte[] copybuffer = new byte[4096];

            filename = filename.ToLowerInvariant();

            if (cache.ContainsKey(filename))
            {
                filedata = cache[filename];
                cache.Remove(filename);
                if (filedata.CanRead)
                {
                    int accesscount = accesscount_cache[filename] + 1;
                    accesscount_cache[filename] = accesscount;
                    return filedata;
                }
                else
                {
                    // been disposed
                    filedata = null;
                }
            }

            // Open the zip file
            ZipInputStream zipstream = OpenPK3File();

            ZipEntry entry = zipstream.GetNextEntry();
            while (entry != null)
            {
                if (entry.IsFile)
                {
                    string entryname = entry.Name.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar).ToLowerInvariant();

                    // Is this the entry we are looking for?
                    if (!cache.ContainsKey(entryname))
                    {
                        int expectedsize = (int)entry.Size;
                        if (expectedsize < 1) expectedsize = 1024;

                        if (entryname != filename)
                        {
                            if (expectedsize >= 32768 // 2^16 >~65 kb
                                || accesscount_cache.ContainsKey(entryname) // already accessed
                                || entryname.StartsWith("sound")
                                || entryname.StartsWith("sprite")
                                || entryname.StartsWith("music")
                                || entryname.StartsWith("acs")
                                || entryname.StartsWith("graphics")
                                || entryname.StartsWith("maps")
                                || entryname.EndsWith(".mp3")
                                || entryname.EndsWith(".wad")
                                || entryname.EndsWith(".wav")
                                || entryname.EndsWith(".txt")
                                )
                            {
                                // Next
                                entry = zipstream.GetNextEntry();
                                continue;
                            }
                        }

                        filedata = new MemoryStream(expectedsize);
                        int readsize = zipstream.Read(copybuffer, 0, copybuffer.Length);
                        while (readsize > 0)
                        {
                            filedata.Write(copybuffer, 0, readsize);
                            readsize = zipstream.Read(copybuffer, 0, copybuffer.Length);
                        }

                        cache.Add(entryname, filedata);
                        if (!accesscount_cache.ContainsKey(entryname))
                        {
                            accesscount_cache.Add(entryname, 0);
                        }

                        if (entryname == filename)
                        {
                            accesscount_cache[entryname]++;
                            break;
                        }
                    }
                }

                // Next
                entry = zipstream.GetNextEntry();
            }

            // Done with the zip file
            zipstream.Close();
            zipstream.Dispose();

            // Nothing found?
            if (filedata == null)
            {
                throw new FileNotFoundException("Cannot find the file " + filename + " in PK3 file " + location.location + ".");
            }
            else
            {
                return filedata;
            }
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
