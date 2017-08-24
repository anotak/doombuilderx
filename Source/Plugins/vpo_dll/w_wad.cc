// Emacs style mode select   -*- C++ -*- 
//-----------------------------------------------------------------------------
//
// Copyright(C) 1993-1996 Id Software, Inc.
// Copyright(C) 2005 Simon Howard
//
// This program is free software; you can redistribute it and/or
// modify it under the terms of the GNU General Public License
// as published by the Free Software Foundation; either version 2
// of the License, or (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA
// 02111-1307, USA.
//
// DESCRIPTION:
//	Handles WAD file header, directory, lump I/O.
//
//-----------------------------------------------------------------------------

#include "vpo_local.h"

namespace vpo
{


typedef struct
{
    // Should be "IWAD" or "PWAD".
    char		identification[4];		
    int			numlumps;
    int			infotableofs;
} PACKEDATTR wadinfo_t;


typedef struct
{
    int			filepos;
    int			size;
    char		name[8];
} PACKEDATTR filelump_t;

//
// GLOBALS
//

// Location of each lump on disk.

lumpinfo_t *lumpinfo;	
unsigned int numlumps = 0;




//
// LUMP BASED ROUTINES.
//

// andrewj: added this to find level names
bool CheckMapHeader(filelump_t *lumps, int num_after)
{
  static const char *level_lumps[] =
  {
    "THINGS", "LINEDEFS", "SIDEDEFS", "VERTEXES",
    "SEGS", "SSECTORS", "NODES", "SECTORS",

    NULL // end of list
  };

  if (num_after < 4)
    return false;

  int seen_mask  = 0;
  int seen_count = 0;

  for (int i = 0 ; level_lumps[i] ; i++)
  {
    const char *name = level_lumps[i];

    // level lumps are never valid map header names
    if (strncmp(lumps[0].name, name, 8) == 0)
      return false;

    for (int pos = 1 ; pos <= 4 ; pos++)
    {
      if (strncmp(lumps[pos].name, name, 8) == 0)
      {
        int mask = 1 << i;

        // check if name was duplicated
        if (seen_mask & mask)
          return false;

        seen_mask |= mask;
        seen_count += 1;
      }
    }

    // found enough level lumps
    if (seen_count >= 4)
      return true;
  }

  return false;
}


//
// W_AddFile
//
// All files are optional, but at least one file must be
//  found (PWAD, if all required lumps are present).
// Files with a .wad extension are wadlink files
//  with multiple lumps.
// Other files are single lumps with the base filename
//  for the lump name.

wad_file_t *W_AddFile (const char *filename)
{
    wadinfo_t header;
    lumpinfo_t *lump_p;
    unsigned int i;
    wad_file_t *wad_file;
    int length;
    int startlump;
    filelump_t *fileinfo;
    filelump_t *filerover;
    
    // open the file and add to directory

    wad_file = W_OpenFile(filename);

    if (wad_file == NULL)
    {
///	printf (" couldn't open %s\n", filename);
	return NULL;
    }

    startlump = numlumps;
	
    W_Read(wad_file, 0, &header, sizeof(header));

	if (strncmp(header.identification,"IWAD",4))
	{
	    // Homebrew levels?
	    if (strncmp(header.identification,"PWAD",4))
	    {
		I_Error ("Wad file %s doesn't have IWAD "
			 "or PWAD id\n", filename);
	    }
	    
	    // ???modifiedgame = true;		
	}

	header.numlumps = LONG(header.numlumps);
	header.infotableofs = LONG(header.infotableofs);

	length = header.numlumps * sizeof(filelump_t);

	fileinfo = new filelump_t[header.numlumps];

  W_Read(wad_file, header.infotableofs, fileinfo, length);
	numlumps += header.numlumps;


    // Fill in lumpinfo
    lumpinfo = (lumpinfo_t *)realloc(lumpinfo, numlumps * sizeof(lumpinfo_t));

    if (lumpinfo == NULL)
    {
	I_Error ("Couldn't realloc lumpinfo");
    }

    lump_p = &lumpinfo[startlump];
	
    filerover = fileinfo;

    for (i=startlump; i<numlumps; ++i)
    {
      lump_p->wad_file = wad_file;
      lump_p->position = LONG(filerover->filepos);
      lump_p->size = LONG(filerover->size);

      strncpy(lump_p->name, filerover->name, 8);

      lump_p->is_map_header = CheckMapHeader(filerover, numlumps - i - 1);

        ++lump_p;
        ++filerover;
    }
	
    delete[] fileinfo;

    return wad_file;
}


void W_RemoveFile(void)
{
    if (lumpinfo)
    {
        // andrewj: horrible hack, assumes a single wad file
        W_CloseFile(lumpinfo[0].wad_file);

        free(lumpinfo);

        lumpinfo = NULL;
        numlumps = 0;
    }
}



//
// W_NumLumps
//
int W_NumLumps (void)
{
    return numlumps;
}


//
// W_CheckNumForName
// Returns -1 if name not found.
//

int W_CheckNumForName (const char* name)
{
    int i;

    {
        // scan backwards so patch lump files take precedence

        for (i=numlumps-1; i >= 0; --i)
        {
            if (!strncasecmp(lumpinfo[i].name, name, 8))
            {
                return i;
            }
        }
    }

    // TFB. Not found.

    return -1;
}


//
// W_GetNumForName
// Calls W_CheckNumForName, but bombs out if not found.
//
int W_GetNumForName (const char* name)
{
    int	i;

    i = W_CheckNumForName (name);
    
    if (i < 0)
    {
        I_Error ("W_GetNumForName: %s not found!", name);
    }
 
    return i;
}


//
// W_LumpLength
// Returns the buffer size needed to load the given lump.
//
int W_LumpLength (unsigned int lump)
{
    if (lump >= numlumps)
    {
	I_Error ("W_LumpLength: %i >= numlumps", lump);
    }

    return lumpinfo[lump].size;
}



//
// W_ReadLump
// Loads the lump into the given buffer,
//  which must be >= W_LumpLength().
//
void W_ReadLump(unsigned int lump, void *dest)
{
    int c;
    lumpinfo_t *l;
	
    if (lump >= numlumps)
    {
	I_Error ("W_ReadLump: %i >= numlumps", lump);
    }

    l = lumpinfo+lump;
	
///    I_BeginRead ();
	
    c = W_Read(l->wad_file, l->position, dest, l->size);

    if (c < l->size)
    {
	I_Error ("W_ReadLump: only read %i of %i on lump %i",
		 c, l->size, lump);	
    }

///    I_EndRead ();
}



//
// W_LoadLump
//
// Load a lump into memory and return a pointer to a buffer containing
// the lump data.
//

byte * W_LoadLump(int lumpnum)
{
    byte *result;
    lumpinfo_t *lump;

    if ((unsigned)lumpnum >= numlumps)
    {
	I_Error ("W_LoadLump: %i >= numlumps", lumpnum);
    }

    lump = &lumpinfo[lumpnum];

    // load it now

    result = new byte[W_LumpLength(lumpnum) + 1];

    W_ReadLump (lumpnum, result);

    return result;
}


void W_FreeLump(byte * data)
{
    delete[] data;
}


} // namespace vpo

