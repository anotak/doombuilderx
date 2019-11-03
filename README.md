# Doom Builder X

Doom Builder X is a Doom 2 map editor. Doom Builder X is a fork of CodeImp's [Doom Builder 2](http://www.doombuilder.com/), mainly focused on long term maintenence, bugfixes, and improved responsiveness where possible.

See the [releases](https://github.com/anotak/doombuilderx/releases) page for downloads.

### Prerequisites for running

if you already have DB2 or GZDB working, you already have these:

[.NET 3.5](https://www.microsoft.com/en-us/download/details.aspx?id=21) - (You should have this already if you have Windows 7 or newer)  
and  
[DirectX 9.0c Runtime](https://www.microsoft.com/en-us/download/details.aspx?id=35) - (This is not included in DirectX10 or 11)
and  
SlimDX - The SlimDX website is down permanently, but the 32-bit SlimDX runtime installer (slimdx.msi) is present in the Setup folder.  

### Supported systems

DBX is 32-bit, and supports Windows XP or newer. The hardware requirements are lower than that of Doom Builder 2, if you are familiar with that.

We do not currently directly support Linux or Mac, but ideally I'd like to do so eventually. The way SlimDX was used in DB2 makes that project very difficult, unfortunately. Although, [some people have found a workaround with WINE that may work on some systems](https://www.doomworld.com/forum/topic/106271-gzdoombuilder-wine-tutorial/). Several well-known mappers use a Windows XP VM to run DBX on Linux or Mac as well.

### Compiling

Nuget in Visual Studio should handle getting SlimDX, SharpCompress, and Moonsharp. You need to install the SlimDX runtime in the Setup folder as well, which is present in the Data/Setup/ folder.  
The RejectEditor project should not be built. It was broken when I first got it from the Doom Builder 2 SVN and I have not had time to determine the cause.  
The Builder40 project is the same as the Builder project, just on .NET 4.0. Builder is what I release, but sometimes Builder40 is useful, as Visual Studio's debugging and profiling tools support .NET 4.0 much better. However, compiling Builder40 is completely optional.

## Authors

* CodeImp (Doom Builder 2)

and the rest listed alphabetically

* Anotak (Doom Builder X)
* Altazimuth (some bug fixes)
* Anders Astrand (some DB2 code)
* Andrew Apted (included glBSP (like DB2))
* Boris (several plugins used, contributed source to DB2)
* Bmsq (Long texture name support)
* Davidmerkt (fixed a crash)
* Randi Heit (included Zdoom ACC and ZDBSP (like DB2))
* Simon Howard, Lee Killough, Colin Phipps, Colin Reed (BSP-W32 is used like in DB2)
* MaxED (some GZDoomBuilder source used)
* Raven Software (included Hexen ACC (like DB2))
* Marc Rousseau (included ZenNode (like DB2))
* Sensor Based Systems Software / SBSoftware (included DeepBSP (like DB2))
* Volte (rotatable grid)
* Zokum (included ZokumBSP)
* ZZYZX (some GZDB-BF source used)

if you notice you are missing from here, please inform me, as I have not found full credits for DoomBuilder 2.

## License

Doom Builder X is licensed under the GPL3 License - see the [LICENSE](LICENSE) file for details.
Individual components like the nodebuilders and ACS compilers have their own licenses.

## Acknowledgments

* Doom Builder 2 by CodeImp
* Doom by id Software and owned by Zenimax
* Heretic, Hexen, & ACC by Raven Software
* Strife by Rogue Entertainment
* GZDoomBuilder by MaxED
* GZDoomBuilder-Bugfix by ZZYZX
* Eternity Engine by the Eternity team
* ZDoom by Randi Heit
* [the template for this document by PurpleBooth](https://gist.github.com/PurpleBooth/109311bb0361f32d87a2)
