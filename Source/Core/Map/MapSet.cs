
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
using CodeImp.DoomBuilder.Geometry;
using CodeImp.DoomBuilder.Windows;
using SlimDX.Direct3D9;
using CodeImp.DoomBuilder.Rendering;
using SlimDX;
using System.Drawing;
using CodeImp.DoomBuilder.Editing;
using CodeImp.DoomBuilder.IO;
using CodeImp.DoomBuilder.Types;
using System.IO;
using CodeImp.DoomBuilder.Config;

#endregion

namespace CodeImp.DoomBuilder.Map
{
	/// <summary>
	/// Manages all geometry structures and things in a map. Also provides
	/// methods to works with selections and marking elements for any purpose.
	/// Note that most methods are of O(n) complexity.
	/// </summary>
	public sealed class MapSet
	{
		#region ================== Constants

		/// <summary>Stiching distance. This is only to get around inaccuracies. Basically,
		/// geometry only stitches when exactly on top of each other.</summary>
		public const float STITCH_DISTANCE = 0.001f;
		
		// Virtual sector identification
		// This contains a character that is invalid in the UDMF standard, but valid
		// in our parser, so that it can only be used by Doom Builder and will never
		// conflict with any other valid UDMF field.
		internal const string VIRTUAL_SECTOR_FIELD = "!virtual_sector";
		
		// Handler for tag fields
		public delegate void TagHandler<T>(MapElement element, bool actionargument, UniversalType type, ref int value, T obj);
		
		#endregion

		#region ================== Variables

		// Sector indexing
		private List<int> indexholes;
		private int lastsectorindex;
		
		// Sidedef indexing for (de)serialization
		private Sidedef[] sidedefindices;
		
		// Map structures
		private Vertex[] vertices;
		private Linedef[] linedefs;
		private Sidedef[] sidedefs;
		private Sector[] sectors;
		private Thing[] things;
		private int numvertices;
		private int numlinedefs;
		private int numsidedefs;
		private int numsectors;
		private int numthings;

        // ano - universal fields for preserving unknown UDMF data as
        // the spec requires
        private UniversalCollection unidentified_udmf;
		
		// Behavior
		private int freezearrays;
		private bool autoremove;
		
		// Selected elements
		private LinkedList<Vertex> sel_vertices;
		private LinkedList<Linedef> sel_linedefs;
		private LinkedList<Sector> sel_sectors;
		private LinkedList<Thing> sel_things;
		private SelectionType sel_type;
		
		// Statics
		private static long emptylongname;
		private static UniValue virtualsectorvalue;
		
		// Disposing
		private bool isdisposed = false;

		#endregion

		#region ================== Properties
		
		/// <summary>Returns the number of selected sectors.</summary>
		public int SelectedSectorsCount { get { return sel_sectors.Count; } }

		/// <summary>Returns the number of selected linedefs.</summary>
		public int SelectedLinedefsCount { get { return sel_linedefs.Count; } }

		/// <summary>Returns the number of selected vertices.</summary>
		public int SelectedVerticessCount { get { return sel_vertices.Count; } }

		/// <summary>Returns the number of selected things.</summary>
		public int SelectedThingsCount { get { return sel_things.Count; } }
		
		/// <summary>Returns a reference to the list of all vertices.</summary>
		public ICollection<Vertex> Vertices { get { if(freezearrays == 0) return vertices; else return new MapElementCollection<Vertex>(ref vertices, numvertices); } }

		/// <summary>Returns a reference to the list of all linedefs.</summary>
		public ICollection<Linedef> Linedefs { get { if(freezearrays == 0) return linedefs; else return new MapElementCollection<Linedef>(ref linedefs, numlinedefs); } }

		/// <summary>Returns a reference to the list of all sidedefs.</summary>
		public ICollection<Sidedef> Sidedefs { get { if(freezearrays == 0) return sidedefs; else return new MapElementCollection<Sidedef>(ref sidedefs, numsidedefs); } }

		/// <summary>Returns a reference to the list of all sectors.</summary>
		public ICollection<Sector> Sectors { get { if(freezearrays == 0) return sectors; else return new MapElementCollection<Sector>(ref sectors, numsectors); } }

		/// <summary>Returns a reference to the list of all things.</summary>
		public ICollection<Thing> Things { get { if(freezearrays == 0) return things; else return new MapElementCollection<Thing>(ref things, numthings); } }

		/// <summary>Indicates if the map is disposed.</summary>
		public bool IsDisposed { get { return isdisposed; } }
		
		/// <summary>Returns a reference to the list of selected vertices.</summary>
		internal LinkedList<Vertex> SelectedVertices { get { return sel_vertices; } }

		/// <summary>Returns a reference to the list of selected linedefs.</summary>
		internal LinkedList<Linedef> SelectedLinedefs { get { return sel_linedefs; } }

		/// <summary>Returns a reference to the list of selected sectors.</summary>
		internal LinkedList<Sector> SelectedSectors { get { return sel_sectors; } }

		/// <summary>Returns a reference to the list of selected things.</summary>
		internal LinkedList<Thing> SelectedThings { get { return sel_things; } }
		
		/// <summary>Returns the current type of selection.</summary>
		public SelectionType SelectionType { get { return sel_type; } set { sel_type = value; } }

		/// <summary>Long name to indicate "no texture". This is the long name for a single dash.</summary>
		public static long EmptyLongName { get { return emptylongname; } }

		/// <summary>Returns the name of the custom field that marks virtual sectors in pasted geometry.</summary>
		public static string VirtualSectorField { get { return VIRTUAL_SECTOR_FIELD; } }

		/// <summary>Returns the value of the custom field that marks virtual sectors in pasted geometry.</summary>
		public static UniValue VirtualSectorValue { get { return virtualsectorvalue; } }
		
		internal Sidedef[] SidedefIndices { get { return sidedefindices; } }

		internal bool AutoRemove { get { return autoremove; } set { autoremove = value; } }

        // ano
        public UniversalCollection UnidentifiedUDMF { get { return unidentified_udmf; }
            set {
                if (this == General.Map.Map)
                {
                    General.Map.UndoRedo.RecChangeMapUDMF();
                }
                unidentified_udmf = value;
            }
        }

        #endregion

        #region ================== Constructor / Disposer

        // Constructor for new empty map
        internal MapSet()
		{
			// Initialize
			vertices = new Vertex[0];
			linedefs = new Linedef[0];
			sidedefs = new Sidedef[0];
			sectors = new Sector[0];
			things = new Thing[0];
			sel_vertices = new LinkedList<Vertex>();
			sel_linedefs = new LinkedList<Linedef>();
			sel_sectors = new LinkedList<Sector>();
			sel_things = new LinkedList<Thing>();
			indexholes = new List<int>();
            unidentified_udmf = new UniversalCollection();
			lastsectorindex = 0;
			autoremove = true;
			
			// We have no destructor
			GC.SuppressFinalize(this);
		}

		// Constructor for map to deserialize
		internal MapSet(MemoryStream stream)
		{
			// Initialize
			vertices = new Vertex[0];
			linedefs = new Linedef[0];
			sidedefs = new Sidedef[0];
			sectors = new Sector[0];
			things = new Thing[0];
			sel_vertices = new LinkedList<Vertex>();
			sel_linedefs = new LinkedList<Linedef>();
			sel_sectors = new LinkedList<Sector>();
			sel_things = new LinkedList<Thing>();
			indexholes = new List<int>();
			lastsectorindex = 0;
			autoremove = true;

			// Deserialize
			Deserialize(stream);

			// We have no destructor
			GC.SuppressFinalize(this);
		}

		// Disposer
		internal void Dispose()
		{
			ArrayList list;
			
			// Not already disposed?
			if(!isdisposed)
			{
				// Already set isdisposed so that changes can be prohibited
				isdisposed = true;
				BeginAddRemove();
				
				// Dispose all things
				while((things.Length > 0) && (things[0] != null))
					things[0].Dispose();

				// Dispose all sectors
				while((sectors.Length > 0) && (sectors[0] != null))
					sectors[0].Dispose();

				// Dispose all sidedefs
				while((sidedefs.Length > 0) && (sidedefs[0] != null))
					sidedefs[0].Dispose();

				// Dispose all linedefs
				while((linedefs.Length > 0) && (linedefs[0] != null))
					linedefs[0].Dispose();

				// Dispose all vertices
				while((vertices.Length > 0) && (vertices[0] != null))
					vertices[0].Dispose();

				// Clean up
				vertices = null;
				linedefs = null;
				sidedefs = null;
				sectors = null;
				things = null;
				sel_vertices = null;
				sel_linedefs = null;
				sel_sectors = null;
				sel_things = null;
				indexholes = null;
				
				// Done
				isdisposed = true;
			}
		}

		// Static initializer
		internal static void Initialize()
		{
			emptylongname = Lump.MakeLongName("-");
			virtualsectorvalue = new UniValue((int)UniversalType.Integer, (int)0);
		}

		#endregion

		#region ================== Management
		
		// This begins large add/remove operations
		public void BeginAddRemove()
		{
			freezearrays++;
		}

		// This allocates the arrays to a minimum size so that
		// a lot of items can be created faster. This function will never
		// allocate less than the current number of items.
		public void SetCapacity(int nvertices, int nlinedefs, int nsidedefs, int nsectors, int nthings)
		{
			if(freezearrays == 0)
				throw new Exception("You must call BeginAddRemove before setting the reserved capacity.");

			if(numvertices < nvertices)
				Array.Resize(ref vertices, nvertices);

			if(numlinedefs < nlinedefs)
				Array.Resize(ref linedefs, nlinedefs);

			if(numsidedefs < nsidedefs)
				Array.Resize(ref sidedefs, nsidedefs);

			if(numsectors < nsectors)
				Array.Resize(ref sectors, nsectors);

			if(numthings < nthings)
				Array.Resize(ref things, nthings);
		}
		
		// This ends add/remove operations and crops the arrays
		public void EndAddRemove()
		{
			if(freezearrays > 0)
				freezearrays--;

			if(freezearrays == 0)
			{
				if(numvertices < vertices.Length)
					Array.Resize(ref vertices, numvertices);

				if(numlinedefs < linedefs.Length)
					Array.Resize(ref linedefs, numlinedefs);

				if(numsidedefs < sidedefs.Length)
					Array.Resize(ref sidedefs, numsidedefs);

				if(numsectors < sectors.Length)
					Array.Resize(ref sectors, numsectors);

				if(numthings < things.Length)
					Array.Resize(ref things, numthings);
			}
		}
		
		/// <summary>
		/// This makes a deep copy and returns the new MapSet.
		/// </summary>
		public MapSet Clone()
		{
			Linedef nl;
			Sidedef nd;
			
			// Create the map set
			MapSet newset = new MapSet();
			newset.BeginAddRemove();
			newset.SetCapacity(numvertices, numlinedefs, numsidedefs, numsectors, numthings);

            if (unidentified_udmf != null)
            {
                UniversalCollection newcollection = new UniversalCollection();

                foreach (UniversalEntry u in unidentified_udmf)
                {
                    newcollection.Add(u.Key, u.Value);
                }

                newset.unidentified_udmf = unidentified_udmf;
            }
			
			// Go for all vertices
			foreach(Vertex v in vertices)
			{
				// Make new vertex
				v.Clone = newset.CreateVertex(v.Position);
				v.CopyPropertiesTo(v.Clone);
			}

			// Go for all sectors
			foreach(Sector s in sectors)
			{
				// Make new sector
				s.Clone = newset.CreateSector();
				s.CopyPropertiesTo(s.Clone);
			}

			// Go for all linedefs
			foreach(Linedef l in linedefs)
			{
				// Make new linedef
				nl = newset.CreateLinedef(l.Start.Clone, l.End.Clone);
				l.CopyPropertiesTo(nl);

				// Linedef has a front side?
				if(l.Front != null)
				{
					// Make new sidedef
					nd = newset.CreateSidedef(nl, true, l.Front.Sector.Clone);
					l.Front.CopyPropertiesTo(nd);
				}

				// Linedef has a back side?
				if(l.Back != null)
				{
					// Make new sidedef
					nd = newset.CreateSidedef(nl, false, l.Back.Sector.Clone);
					l.Back.CopyPropertiesTo(nd);
				}
			}
			
			// Go for all things
			foreach(Thing t in things)
			{
				// Make new thing
				Thing nt = newset.CreateThing();
				t.CopyPropertiesTo(nt);
			}
			
			// Remove clone references
			foreach(Vertex v in vertices) v.Clone = null;
			foreach(Sector s in sectors) s.Clone = null;
			
			// Return the new set
			newset.EndAddRemove();
			return newset;
		}

		// This makes a deep copy of the marked geometry and binds missing sectors to a virtual sector
		internal MapSet CloneMarked()
		{
			Sector virtualsector = null;
			
			// Create the map set
			MapSet newset = new MapSet();
			newset.BeginAddRemove();

			// Get marked geometry
			ICollection<Vertex> mvertices = GetMarkedVertices(true);
			ICollection<Linedef> mlinedefs = GetMarkedLinedefs(true);
			ICollection<Sector> msectors = GetMarkedSectors(true);
			ICollection<Thing> mthings = GetMarkedThings(true);
			newset.SetCapacity(mvertices.Count, mlinedefs.Count, numsidedefs, msectors.Count, mthings.Count);
			
			// Go for all vertices
			foreach(Vertex v in mvertices)
			{
				// Make new vertex
				v.Clone = newset.CreateVertex(v.Position);
				v.CopyPropertiesTo(v.Clone);
			}

			// Go for all sectors
			foreach(Sector s in msectors)
			{
				// Make new sector
				s.Clone = newset.CreateSector();
				s.CopyPropertiesTo(s.Clone);
			}

			// Go for all linedefs
			foreach(Linedef l in mlinedefs)
			{
				// Make new linedef
				Linedef nl = newset.CreateLinedef(l.Start.Clone, l.End.Clone);
				l.CopyPropertiesTo(nl);

				// Linedef has a front side?
				if(l.Front != null)
				{
					Sidedef nd;
					
					// Sector on front side marked?
					if(l.Front.Sector.Marked)
					{
						// Make new sidedef
						nd = newset.CreateSidedef(nl, true, l.Front.Sector.Clone);
					}
					else
					{
						// Make virtual sector if needed
						if(virtualsector == null)
						{
							virtualsector = newset.CreateSector();
							l.Front.Sector.CopyPropertiesTo(virtualsector);
							virtualsector.Fields.BeforeFieldsChange();
							virtualsector.Fields[VIRTUAL_SECTOR_FIELD] = new UniValue(virtualsectorvalue);
						}
						
						// Make new sidedef that links to the virtual sector
						nd = newset.CreateSidedef(nl, true, virtualsector);
					}
					
					l.Front.CopyPropertiesTo(nd);
				}

				// Linedef has a back side?
				if(l.Back != null)
				{
					Sidedef nd;

					// Sector on front side marked?
					if(l.Back.Sector.Marked)
					{
						// Make new sidedef
						nd = newset.CreateSidedef(nl, false, l.Back.Sector.Clone);
					}
					else
					{
						// Make virtual sector if needed
						if(virtualsector == null)
						{
							virtualsector = newset.CreateSector();
							l.Back.Sector.CopyPropertiesTo(virtualsector);
							virtualsector.Fields.BeforeFieldsChange();
							virtualsector.Fields[VIRTUAL_SECTOR_FIELD] = new UniValue(virtualsectorvalue);
						}

						// Make new sidedef that links to the virtual sector
						nd = newset.CreateSidedef(nl, false, virtualsector);
					}
					
					l.Back.CopyPropertiesTo(nd);
				}
			}

			// Go for all things
			foreach(Thing t in mthings)
			{
				// Make new thing
				Thing nt = newset.CreateThing();
				t.CopyPropertiesTo(nt);
			}

			// Remove clone references
			foreach(Vertex v in vertices) v.Clone = null;
			foreach(Sector s in sectors) s.Clone = null;

			// Return the new set
			newset.EndAddRemove();
			return newset;
		}
		
		/// <summary>This creates a new vertex and returns it.</summary>
		public Vertex CreateVertex(Vector2D pos)
		{
			if(numvertices == General.Map.FormatInterface.MaxVertices)
			{
				General.Interface.DisplayStatus(StatusType.Warning, "Failed to complete operation: maximum number of vertices reached.");
				return null;
			}
			
			// Make the vertex
			Vertex v = new Vertex(this, numvertices, pos);
			AddItem(v, ref vertices, numvertices, ref numvertices);
			return v;
		}

		/// <summary>This creates a new vertex and returns it.</summary>
		public Vertex CreateVertex(int index, Vector2D pos)
		{
			if(numvertices == General.Map.FormatInterface.MaxVertices)
			{
				General.Interface.DisplayStatus(StatusType.Warning, "Failed to complete operation: maximum number of vertices reached.");
				return null;
			}

			// Make the vertex
			Vertex v = new Vertex(this, index, pos);
			AddItem(v, ref vertices, index, ref numvertices);
			return v;
		}

		/// <summary>This creates a new linedef and returns it.</summary>
		public Linedef CreateLinedef(Vertex start, Vertex end)
		{
            if (start == null || end == null)
            {
                return null;
            }

			if(numlinedefs == General.Map.FormatInterface.MaxLinedefs)
			{
				General.Interface.DisplayStatus(StatusType.Warning, "Failed to complete operation: maximum number of linedefs reached.");
				return null;
			}

			// Make the linedef
			Linedef l = new Linedef(this, numlinedefs, start, end);
			AddItem(l, ref linedefs, numlinedefs, ref numlinedefs);
			return l;
		}

		/// <summary>This creates a new linedef and returns it.</summary>
		public Linedef CreateLinedef(int index, Vertex start, Vertex end)
		{
			if(numlinedefs == General.Map.FormatInterface.MaxLinedefs)
			{
				General.Interface.DisplayStatus(StatusType.Warning, "Failed to complete operation: maximum number of linedefs reached.");
				return null;
			}

			// Make the linedef
			Linedef l = new Linedef(this, index, start, end);
			AddItem(l, ref linedefs, index, ref numlinedefs);
			return l;
		}

		/// <summary>This creates a new sidedef and returns it.</summary>
		public Sidedef CreateSidedef(Linedef l, bool front, Sector s)
		{
			if(numsidedefs == int.MaxValue)
			{
				General.Interface.DisplayStatus(StatusType.Warning, "Failed to complete operation: maximum number of sidedefs reached.");
				return null;
			}
			
			// Make the sidedef
			Sidedef sd = new Sidedef(this, numsidedefs, l, front, s);
			AddItem(sd, ref sidedefs, numsidedefs, ref numsidedefs);
			return sd;
		}

		/// <summary>This creates a new sidedef and returns it.</summary>
		public Sidedef CreateSidedef(int index, Linedef l, bool front, Sector s)
		{
			if(numsidedefs == int.MaxValue)
			{
				General.Interface.DisplayStatus(StatusType.Warning, "Failed to complete operation: maximum number of sidedefs reached.");
				return null;
			}

			// Make the sidedef
			Sidedef sd = new Sidedef(this, index, l, front, s);
			AddItem(sd, ref sidedefs, index, ref numsidedefs);
			return sd;
		}

		/// <summary>This creates a new sector and returns it.</summary>
		public Sector CreateSector()
		{
			// Make the sector
			return CreateSector(numsectors);
		}

		/// <summary>This creates a new sector and returns it.</summary>
		public Sector CreateSector(int index)
		{
			int fixedindex;

			if(numsectors == General.Map.FormatInterface.MaxSectors)
			{
				General.Interface.DisplayStatus(StatusType.Warning, "Failed to complete operation: maximum number of sectors reached.");
				return null;
			}

			// Do we have any index holes we can use?
			if(indexholes.Count > 0)
			{
				// Take one of the index holes
				fixedindex = indexholes[indexholes.Count - 1];
				indexholes.RemoveAt(indexholes.Count - 1);
			}
			else
			{
				// Make a new index
				fixedindex = lastsectorindex++;
			}
			
			// Make the sector
			return CreateSectorEx(fixedindex, index);
		}

		// This creates a new sector with a specific fixed index
		private Sector CreateSectorEx(int fixedindex, int index)
		{
			if(numsectors == General.Map.FormatInterface.MaxSectors)
			{
				General.Interface.DisplayStatus(StatusType.Warning, "Failed to complete operation: maximum number of sectors reached.");
				return null;
			}

			// Make the sector
			Sector s = new Sector(this, index, fixedindex);
			AddItem(s, ref sectors, index, ref numsectors);
			return s;
		}

		/// <summary>This creates a new thing and returns it.</summary>
		public Thing CreateThing()
		{
			if(numthings == General.Map.FormatInterface.MaxThings)
			{
				General.Interface.DisplayStatus(StatusType.Warning, "Failed to complete operation: maximum number of things reached.");
				return null;
			}

			// Make the thing
			Thing t = new Thing(this, numthings);
			AddItem(t, ref things, numthings, ref numthings);
			return t;
		}

		/// <summary>This creates a new thing and returns it.</summary>
		public Thing CreateThing(int index)
		{
			if(numthings == General.Map.FormatInterface.MaxThings)
			{
				General.Interface.DisplayStatus(StatusType.Warning, "Failed to complete operation: maximum number of things reached.");
				return null;
			}

			// Make the thing
			Thing t = new Thing(this, index);
			AddItem(t, ref things, index, ref numthings);
			return t;
		}

		// This increases the size of the array to add an item
		private void AddItem<T>(T item, ref T[] array, int index, ref int counter) where T: MapElement
		{
			// Only resize when there are no more free entries
			if(counter == array.Length)
			{
				if(freezearrays == 0)
					Array.Resize(ref array, counter + 1);
				else
					Array.Resize(ref array, counter + 10);
			}
			
			// Move item at the given index if the new item is not added at the end
			if(index != counter)
			{
				array[counter] = array[index];
				array[counter].Index = counter;
			}

			// Add item
			array[index] = item;
			counter++;
		}
		
		// This adds a sector index hole
		internal void AddSectorIndexHole(int index)
		{
			indexholes.Add(index);
		}
		
		private void RemoveItem<T>(ref T[] array, int index, ref int counter) where T: MapElement
		{
			if(index == (counter - 1))
			{
				array[index] = null;
			}
			else
			{
				array[index] = array[counter - 1];
				array[index].Index = index;
				array[counter - 1] = null;
			}
			
			counter--;

			if(freezearrays == 0)
				Array.Resize(ref array, counter);
		}
		
		internal void RemoveVertex(int index)
		{
			RemoveItem(ref vertices, index, ref numvertices);
		}

		internal void RemoveLinedef(int index)
		{
			RemoveItem(ref linedefs, index, ref numlinedefs);
		}

		internal void RemoveSidedef(int index)
		{
			RemoveItem(ref sidedefs, index, ref numsidedefs);
		}

		internal void RemoveSector(int index)
		{
			RemoveItem(ref sectors, index, ref numsectors);
		}

		internal void RemoveThing(int index)
		{
			RemoveItem(ref things, index, ref numthings);
		}		
		
		#endregion

		#region ================== Serialization

		// This serializes the MapSet
		internal MemoryStream Serialize()
		{
			MemoryStream stream = new MemoryStream(20000000);	// Yes that is about 20 MB.
			SerializerStream serializer = new SerializerStream(stream);
			
			// Index the sidedefs
			int sidedefindex = 0;
			foreach(Sidedef sd in sidedefs)
				sd.SerializedIndex = sidedefindex++;

			serializer.Begin();

			// Write private data
			serializer.wInt(lastsectorindex);
			serializer.wInt(indexholes.Count);
			foreach(int i in indexholes) serializer.wInt(i);

			// Write map data
			WriteVertices(serializer);
			WriteSectors(serializer);
			WriteLinedefs(serializer);
			WriteSidedefs(serializer);
			WriteThings(serializer);

			serializer.End();

			// Reallocate to keep only the used memory
			stream.Capacity = (int)stream.Length;
			
			return stream;
		}

		// This serializes things
		private void WriteThings(SerializerStream stream)
		{
			stream.wInt(numthings);
			
			// Go for all things
			foreach(Thing t in things)
			{
				t.ReadWrite(stream);
			}
		}

		// This serializes vertices
		private void WriteVertices(SerializerStream stream)
		{
			stream.wInt(numvertices);

			// Go for all vertices
			int index = 0;
			foreach(Vertex v in vertices)
			{
				v.SerializedIndex = index++;
				
				v.ReadWrite(stream);
			}
		}

		// This serializes linedefs
		private void WriteLinedefs(SerializerStream stream)
		{
			stream.wInt(numlinedefs);

			// Go for all lines
			int index = 0;
			foreach(Linedef l in linedefs)
			{
				l.SerializedIndex = index++;
				
				stream.wInt(l.Start.SerializedIndex);
				
				stream.wInt(l.End.SerializedIndex);

				l.ReadWrite(stream);
			}
		}

		// This serializes sidedefs
		private void WriteSidedefs(SerializerStream stream)
		{
			stream.wInt(numsidedefs);

			// Go for all sidedefs
			foreach(Sidedef sd in sidedefs)
			{
				stream.wInt(sd.Line.SerializedIndex);
				
				stream.wInt(sd.Sector.SerializedIndex);

				stream.wBool(sd.IsFront);

				sd.ReadWrite(stream);
			}
		}

		// This serializes sectors
		private void WriteSectors(SerializerStream stream)
		{
			stream.wInt(numsectors);

			// Go for all sectors
			int index = 0;
			foreach(Sector s in sectors)
			{
				s.SerializedIndex = index++;

				s.ReadWrite(stream);
			}
		}

		#endregion

		#region ================== Deserialization

		// This serializes the MapSet
		private void Deserialize(MemoryStream stream)
		{
			stream.Seek(0, SeekOrigin.Begin);
			DeserializerStream deserializer = new DeserializerStream(stream);

			deserializer.Begin();

			// Read private data
			int c;
			deserializer.rInt(out lastsectorindex);
			deserializer.rInt(out c);
			indexholes = new List<int>(c);
			for(int i = 0; i < c; i++)
			{
				int index; deserializer.rInt(out index);
				indexholes.Add(index);
			}

			// Read map data
			Vertex[] verticesarray = ReadVertices(deserializer);
			Sector[] sectorsarray = ReadSectors(deserializer);
			Linedef[] linedefsarray = ReadLinedefs(deserializer, verticesarray);
			ReadSidedefs(deserializer, linedefsarray, sectorsarray);
			ReadThings(deserializer);

			deserializer.End();

			// Make table of sidedef indices
			sidedefindices = new Sidedef[numsidedefs];
			foreach(Sidedef sd in sidedefs)
				sidedefindices[sd.SerializedIndex] = sd;
				
			// Call PostDeserialize
			foreach(Sector s in sectors)
				s.PostDeserialize(this);
		}
		
		// This deserializes things
		private void ReadThings(DeserializerStream stream)
		{
			int c; stream.rInt(out c);

			// Go for all things
			for(int i = 0; i < c; i++)
			{
				Thing t = CreateThing();
				t.ReadWrite(stream);
			}
		}

		// This deserializes vertices
		private Vertex[] ReadVertices(DeserializerStream stream)
		{
			int c; stream.rInt(out c);

			Vertex[] array = new Vertex[c];

			// Go for all vertices
			for(int i = 0; i < c; i++)
			{
				array[i] = CreateVertex(new Vector2D());
				array[i].ReadWrite(stream);
			}

			return array;
		}

		// This deserializes linedefs
		private Linedef[] ReadLinedefs(DeserializerStream stream, Vertex[] verticesarray)
		{
			int c; stream.rInt(out c);

			Linedef[] array = new Linedef[c];

			// Go for all lines
			for(int i = 0; i < c; i++)
			{
				int start, end;
				
				stream.rInt(out start);

				stream.rInt(out end);

				array[i] = CreateLinedef(verticesarray[start], verticesarray[end]);
				array[i].ReadWrite(stream);
			}

			return array;
		}

		// This deserializes sidedefs
		private void ReadSidedefs(DeserializerStream stream, Linedef[] linedefsarray, Sector[] sectorsarray)
		{
			int c; stream.rInt(out c);

			// Go for all sidedefs
			for(int i = 0; i < c; i++)
			{
				int lineindex, sectorindex;
				bool front;
				
				stream.rInt(out lineindex);

				stream.rInt(out sectorindex);

				stream.rBool(out front);

				Sidedef sd = CreateSidedef(linedefsarray[lineindex], front, sectorsarray[sectorindex]);
				sd.ReadWrite(stream);
			}
		}

		// This deserializes sectors
		private Sector[] ReadSectors(DeserializerStream stream)
		{
			int c; stream.rInt(out c);

			Sector[] array = new Sector[c];

			// Go for all sectors
			for(int i = 0; i < c; i++)
			{
				array[i] = CreateSector();
				array[i].ReadWrite(stream);
			}

			return array;
		}

		#endregion

		#region ================== Updating

		/// <summary>
		/// This updates the cache of all elements where needed. You must call this after making changes to the map.
		/// </summary>
		public void Update()
		{
			// Update all!
			Update(true, true);
		}

		/// <summary>
		/// This updates the cache of all elements where needed. It is not recommended to use this version, please use Update() instead.
		/// </summary>
		public void Update(bool dolines, bool dosectors)
		{
			// Update all linedefs
			if(dolines) foreach(Linedef l in linedefs) l.UpdateCache();
			
			// Update all sectors
			if(dosectors)
			{
				foreach(Sector s in sectors) s.Triangulate();
				
				General.Map.CRenderer2D.Surfaces.AllocateBuffers();
				
				foreach(Sector s in sectors) s.CreateSurfaces();
				
				General.Map.CRenderer2D.Surfaces.UnlockBuffers();
			}
		}
		
		/// <summary>
		/// This updates the cache of all elements that is required after a configuration or settings change.
		/// </summary>
		public void UpdateConfiguration()
		{
			// Update all things
			foreach(Thing t in things) t.UpdateConfiguration();
		}
		
		#endregion

		#region ================== Selection
		
		// This checks a flag in a selection type
		private bool InSelectionType(SelectionType value, SelectionType bits)
		{
			return (value & bits) == bits;
		}

		/// <summary>This converts the current selection to a different type of selection as specified.
		/// Note that this function uses the markings to convert the selection.</summary>
		public void ConvertSelection(SelectionType target)
		{
			ConvertSelection(SelectionType.All, target);
		}

		/// <summary>This converts the current selection to a different type of selection as specified.
		/// Note that this function uses the markings to convert the selection.</summary>
		public void ConvertSelection(SelectionType source, SelectionType target)
		{
			ICollection<Linedef> lines;
			ICollection<Vertex> verts;
			
			ClearAllMarks(false);
			
			switch(target)
			{
				// Convert geometry selection to vertices only
				case SelectionType.Vertices:
					if(InSelectionType(source, SelectionType.Linedefs)) MarkSelectedLinedefs(true, true);
					if(InSelectionType(source, SelectionType.Sectors)) General.Map.Map.MarkSelectedSectors(true, true);
					verts = General.Map.Map.GetVerticesFromLinesMarks(true);
					foreach(Vertex v in verts) v.Selected = true;
					verts = General.Map.Map.GetVerticesFromSectorsMarks(true);
					foreach(Vertex v in verts) v.Selected = true;
					General.Map.Map.ClearSelectedSectors();
					General.Map.Map.ClearSelectedLinedefs();
					break;
					
				// Convert geometry selection to linedefs only
				case SelectionType.Linedefs:
					if(InSelectionType(source, SelectionType.Vertices)) MarkSelectedVertices(true, true);
					if(!InSelectionType(source, SelectionType.Linedefs)) ClearSelectedLinedefs();
					lines = General.Map.Map.LinedefsFromMarkedVertices(false, true, false);
					foreach(Linedef l in lines) l.Selected = true;
					if(InSelectionType(source, SelectionType.Sectors))
					{
						foreach(Sector s in General.Map.Map.Sectors)
						{
							if(s.Selected)
							{
								foreach(Sidedef sd in s.Sidedefs)
									sd.Line.Selected = true;
							}
						}
					}
					General.Map.Map.ClearSelectedSectors();
					General.Map.Map.ClearSelectedVertices();
					break;
					
				// Convert geometry selection to sectors only
				case SelectionType.Sectors:
					if(InSelectionType(source, SelectionType.Vertices)) MarkSelectedVertices(true, true);
					if(!InSelectionType(source, SelectionType.Linedefs)) ClearSelectedLinedefs();
					lines = LinedefsFromMarkedVertices(false, true, false);
					foreach(Linedef l in lines) l.Selected = true;
					ClearMarkedSectors(true);
					foreach(Linedef l in linedefs)
					{
						if(!l.Selected)
						{
							if(l.Front != null) l.Front.Sector.Marked = false;
							if(l.Back != null) l.Back.Sector.Marked = false;
						}
					}
					ClearSelectedLinedefs();
					ClearSelectedVertices();
					if(InSelectionType(source, SelectionType.Sectors))
					{
						foreach(Sector s in General.Map.Map.Sectors)
						{
							if(s.Marked || s.Selected)
							{
								s.Selected = true;
								foreach(Sidedef sd in s.Sidedefs)
									sd.Line.Selected = true;
							}
						}
					}
					else
					{
						foreach(Sector s in General.Map.Map.Sectors)
						{
							if(s.Marked)
							{
								s.Selected = true;
								foreach(Sidedef sd in s.Sidedefs)
									sd.Line.Selected = true;
							}
							else
							{
								s.Selected = false;
							}
						}
					}
					break;
					
				default:
					throw new ArgumentException("Unsupported selection target conversion");
					break;
			}
			
			// New selection type
			sel_type = target;
		}

		/// <summary>This clears all selected items</summary>
		public void ClearAllSelected()
		{
			ClearSelectedVertices();
			ClearSelectedThings();
			ClearSelectedLinedefs();
			ClearSelectedSectors();
		}

		/// <summary>This clears selected vertices.</summary>
		public void ClearSelectedVertices()
		{
			sel_vertices.Clear();
			foreach(Vertex v in vertices) v.Selected = false;
		}

		/// <summary>This clears selected things.</summary>
		public void ClearSelectedThings()
		{
			sel_things.Clear();
			foreach(Thing t in things) t.Selected = false;
		}

		/// <summary>This clears selected linedefs.</summary>
		public void ClearSelectedLinedefs()
		{
			sel_linedefs.Clear();
			foreach(Linedef l in linedefs) l.Selected = false;
		}

		/// <summary>This clears selected sectors.</summary>
		public void ClearSelectedSectors()
		{
			sel_sectors.Clear();
			foreach(Sector s in sectors) s.Selected = false;
		}

		/// <summary>Returns a collection of vertices that match a selected state.</summary>
		public ICollection<Vertex> GetSelectedVertices(bool selected)
		{
			if(selected)
			{
				return new List<Vertex>(sel_vertices);
			}
			else
			{
				List<Vertex> list = new List<Vertex>(numvertices - sel_vertices.Count);
				foreach(Vertex v in vertices) if(!v.Selected) list.Add(v);
				return list;
			}
		}

		/// <summary>Returns a collection of things that match a selected state.</summary>
		public ICollection<Thing> GetSelectedThings(bool selected)
		{
			if(selected)
			{
				return new List<Thing>(sel_things);
			}
			else
			{
				List<Thing> list = new List<Thing>(numthings - sel_things.Count);
				foreach(Thing t in things) if(!t.Selected) list.Add(t);
				return list;
			}
		}

		/// <summary>Returns a collection of linedefs that match a selected state.</summary>
		public ICollection<Linedef> GetSelectedLinedefs(bool selected)
		{
			if(selected)
			{
				return new List<Linedef>(sel_linedefs);
			}
			else
			{
				List<Linedef> list = new List<Linedef>(numlinedefs - sel_linedefs.Count);
				foreach(Linedef l in linedefs) if(!l.Selected) list.Add(l);
				return list;
			}
		}

		/// <summary>Returns a collection of sidedefs that match a selected linedefs state.</summary>
		public ICollection<Sidedef> GetSidedefsFromSelectedLinedefs(bool selected)
		{
			if(selected)
			{
				List<Sidedef> list = new List<Sidedef>(sel_linedefs.Count);
				foreach(Linedef ld in sel_linedefs)
				{
					if(ld.Front != null) list.Add(ld.Front);
					if(ld.Back != null) list.Add(ld.Back);
				}
				return list;
			}
			else
			{
				List<Sidedef> list = new List<Sidedef>(numlinedefs - sel_linedefs.Count);
				foreach(Linedef ld in linedefs)
				{
					if(!ld.Selected && (ld.Front != null)) list.Add(ld.Front);
					if(!ld.Selected && (ld.Back != null)) list.Add(ld.Back);
				}
				return list;
			}
		}

		/// <summary>Returns a collection of sectors that match a selected state.</summary>
		public ICollection<Sector> GetSelectedSectors(bool selected)
		{
			if(selected)
			{
				return new List<Sector>(sel_sectors);
			}
			else
			{
				List<Sector> list = new List<Sector>(numsectors - sel_sectors.Count);
				foreach(Sector s in sectors) if(!s.Selected) list.Add(s);
				return list;
			}
		}

		/// <summary>This selects or deselectes geometry based on marked elements.</summary>
		public void SelectMarkedGeometry(bool mark, bool select)
		{
			SelectMarkedVertices(mark, select);
			SelectMarkedLinedefs(mark, select);
			SelectMarkedSectors(mark, select);
			SelectMarkedThings(mark, select);
		}

		/// <summary>This selects or deselectes geometry based on marked elements.</summary>
		public void SelectMarkedVertices(bool mark, bool select)
		{
			foreach(Vertex v in vertices) if(v.Marked == mark) v.Selected = select;
		}

		/// <summary>This selects or deselectes geometry based on marked elements.</summary>
		public void SelectMarkedLinedefs(bool mark, bool select)
		{
			foreach(Linedef l in linedefs) if(l.Marked == mark) l.Selected = select;
		}

		/// <summary>This selects or deselectes geometry based on marked elements.</summary>
		public void SelectMarkedSectors(bool mark, bool select)
		{
			foreach(Sector s in sectors) if(s.Marked == mark) s.Selected = select;
		}

		/// <summary>This selects or deselectes geometry based on marked elements.</summary>
		public void SelectMarkedThings(bool mark, bool select)
		{
			foreach(Thing t in things) if(t.Marked == mark) t.Selected = select;
		}

		/// <summary>This selects geometry by selection group index.</summary>
		public void SelectVerticesByGroup(int groupmask)
		{
			foreach(SelectableElement e in vertices) e.SelectByGroup(groupmask);
		}

		/// <summary>This selects geometry by selection group index.</summary>
		public void SelectLinedefsByGroup(int groupmask)
		{
			foreach(SelectableElement e in linedefs) e.SelectByGroup(groupmask);
		}

		/// <summary>This selects geometry by selection group index.</summary>
		public void SelectSectorsByGroup(int groupmask)
		{
			foreach(SelectableElement e in sectors) e.SelectByGroup(groupmask);
		}

		/// <summary>This selects geometry by selection group index.</summary>
		public void SelectThingsByGroup(int groupmask)
		{
			foreach(SelectableElement e in things) e.SelectByGroup(groupmask);
		}

		/// <summary>This adds the current selection to the specified selection group.</summary>
		public void AddSelectionToGroup(int groupmask)
		{
			foreach(SelectableElement e in vertices)
				if(e.Selected) e.AddToGroup(groupmask);
			
			foreach(SelectableElement e in linedefs)
				if(e.Selected) e.AddToGroup(groupmask);
			
			foreach(SelectableElement e in sectors)
				if(e.Selected) e.AddToGroup(groupmask);
			
			foreach(SelectableElement e in things)
				if(e.Selected) e.AddToGroup(groupmask);
		}
		
		#endregion

		#region ================== Marking

		/// <summary>This clears all marks on all elements.</summary>
		public void ClearAllMarks(bool mark)
		{
			ClearMarkedVertices(mark);
			ClearMarkedThings(mark);
			ClearMarkedLinedefs(mark);
			ClearMarkedSectors(mark);
			ClearMarkedSidedefs(mark);
		}

		/// <summary>This clears all marks on all vertices.</summary>
		public void ClearMarkedVertices(bool mark)
		{
			foreach(Vertex v in vertices) v.Marked = mark;
		}

		/// <summary>This clears all marks on all things.</summary>
		public void ClearMarkedThings(bool mark)
		{
			foreach(Thing t in things) t.Marked = mark;
		}

		/// <summary>This clears all marks on all linedefs.</summary>
		public void ClearMarkedLinedefs(bool mark)
		{
			foreach(Linedef l in linedefs) l.Marked = mark;
		}

		/// <summary>This clears all marks on all sidedefs.</summary>
		public void ClearMarkedSidedefs(bool mark)
		{
			foreach(Sidedef s in sidedefs) s.Marked = mark;
		}

		/// <summary>This clears all marks on all sectors.</summary>
		public void ClearMarkedSectors(bool mark)
		{
			foreach(Sector s in sectors) s.Marked = mark;
		}

		/// <summary>This inverts all marks on all elements.</summary>
		public void InvertAllMarks()
		{
			InvertMarkedVertices();
			InvertMarkedThings();
			InvertMarkedLinedefs();
			InvertMarkedSectors();
			InvertMarkedSidedefs();
		}

		/// <summary>This inverts all marks on all vertices.</summary>
		public void InvertMarkedVertices()
		{
			foreach(Vertex v in vertices) v.Marked = !v.Marked;
		}

		/// <summary>This inverts all marks on all things.</summary>
		public void InvertMarkedThings()
		{
			foreach(Thing t in things) t.Marked = !t.Marked;
		}

		/// <summary>This inverts all marks on all linedefs.</summary>
		public void InvertMarkedLinedefs()
		{
			foreach(Linedef l in linedefs) l.Marked = !l.Marked;
		}

		/// <summary>This inverts all marks on all sidedefs.</summary>
		public void InvertMarkedSidedefs()
		{
			foreach(Sidedef s in sidedefs) s.Marked = !s.Marked;
		}

		/// <summary>This inverts all marks on all sectors.</summary>
		public void InvertMarkedSectors()
		{
			foreach(Sector s in sectors) s.Marked = !s.Marked;
		}

		/// <summary>Returns a collection of vertices that match a marked state.</summary>
		public List<Vertex> GetMarkedVertices(bool mark)
		{
			List<Vertex> list = new List<Vertex>(numvertices >> 1);
			foreach(Vertex v in vertices) if(v.Marked == mark) list.Add(v);
			return list;
		}

		/// <summary>Returns a collection of things that match a marked state.</summary>
		public List<Thing> GetMarkedThings(bool mark)
		{
			List<Thing> list = new List<Thing>(numthings >> 1);
			foreach(Thing t in things) if(t.Marked == mark) list.Add(t);
			return list;
		}

		/// <summary>Returns a collection of linedefs that match a marked state.</summary>
		public List<Linedef> GetMarkedLinedefs(bool mark)
		{
			List<Linedef> list = new List<Linedef>(numlinedefs >> 1);
			foreach(Linedef l in linedefs) if(l.Marked == mark) list.Add(l);
			return list;
		}

		/// <summary>Returns a collection of sidedefs that match a marked state.</summary>
		public List<Sidedef> GetMarkedSidedefs(bool mark)
		{
			List<Sidedef> list = new List<Sidedef>(numsidedefs >> 1);
			foreach(Sidedef s in sidedefs) if(s.Marked == mark) list.Add(s);
			return list;
		}

		/// <summary>Returns a collection of sectors that match a marked state.</summary>
		public List<Sector> GetMarkedSectors(bool mark)
		{
			List<Sector> list = new List<Sector>(numsectors >> 1);
			foreach(Sector s in sectors) if(s.Marked == mark) list.Add(s);
			return list;
		}

		/// <summary>This marks vertices based on selected vertices.</summary>
		public void MarkSelectedVertices(bool selected, bool mark)
		{
			foreach(Vertex v in sel_vertices) v.Marked = mark;
		}

		/// <summary>This marks linedefs based on selected linedefs.</summary>
		public void MarkSelectedLinedefs(bool selected, bool mark)
		{
			foreach(Linedef l in sel_linedefs) l.Marked = mark;
		}

		/// <summary>This marks sectors based on selected sectors.</summary>
		public void MarkSelectedSectors(bool selected, bool mark)
		{
			foreach(Sector s in sel_sectors) s.Marked = mark;
		}

		/// <summary>This marks things based on selected things.</summary>
		public void MarkSelectedThings(bool selected, bool mark)
		{
			foreach(Thing t in sel_things) t.Marked = mark;
		}

		/// <summary>
		/// This marks the front and back sidedefs on linedefs with the matching mark.
		/// </summary>
		public void MarkSidedefsFromLinedefs(bool matchmark, bool setmark)
		{
			foreach(Linedef l in linedefs)
			{
				if(l.Marked == matchmark)
				{
					if(l.Front != null) l.Front.Marked = setmark;
					if(l.Back != null) l.Back.Marked = setmark;
				}
			}
		}

		/// <summary>
		/// This marks the sidedefs that make up the sectors with the matching mark.
		/// </summary>
		public void MarkSidedefsFromSectors(bool matchmark, bool setmark)
		{
			foreach(Sidedef sd in sidedefs)
			{
				if(sd.Sector.Marked == matchmark) sd.Marked = setmark;
			}
		}

		/// <summary>
		/// Returns a collection of vertices that match a marked state on the linedefs.
		/// </summary>
		public ICollection<Vertex> GetVerticesFromLinesMarks(bool mark)
		{
			List<Vertex> list = new List<Vertex>(numvertices >> 1);
			foreach(Vertex v in vertices)
			{
				foreach(Linedef l in v.Linedefs)
				{
					if(l.Marked == mark)
					{
						list.Add(v);
						break;
					}
				}
			}
			return list;
		}

		/// <summary>
		/// Returns a collection of vertices that match a marked state on the linedefs.
		/// The difference with GetVerticesFromLinesMarks is that in this method
		/// ALL linedefs of a vertex must match the specified marked state.
		/// </summary>
		public ICollection<Vertex> GetVerticesFromAllLinesMarks(bool mark)
		{
			List<Vertex> list = new List<Vertex>(numvertices >> 1);
			foreach(Vertex v in vertices)
			{
				bool qualified = true;
				foreach(Linedef l in v.Linedefs)
				{
					if(l.Marked != mark)
					{
						qualified = false;
						break;
					}
				}
				if(qualified) list.Add(v);
			}
			return list;
		}

		/// <summary>
		/// Returns a collection of vertices that match a marked state on the linedefs.
		/// </summary>
		public ICollection<Vertex> GetVerticesFromSectorsMarks(bool mark)
		{
			List<Vertex> list = new List<Vertex>(numvertices >> 1);
			foreach(Vertex v in vertices)
			{
				foreach(Linedef l in v.Linedefs)
				{
					if(((l.Front != null) && (l.Front.Sector.Marked == mark)) ||
						((l.Back != null) && (l.Back.Sector.Marked == mark)))
					{
						list.Add(v);
						break;
					}
				}
			}
			return list;
		}

		/// <summary>
		/// This marks all selected geometry, including sidedefs from sectors.
		/// When sidedefsfromsectors is true, then the sidedefs are marked according to the
		/// marked sectors. Otherwise the sidedefs are marked according to the marked linedefs.
		/// </summary>
		public void MarkAllSelectedGeometry(bool mark, bool linedefsfromvertices, bool verticesfromlinedefs, bool sectorsfromlinedefs, bool sidedefsfromsectors)
		{
			General.Map.Map.ClearAllMarks(!mark);

			// Direct vertices
			General.Map.Map.MarkSelectedVertices(true, mark);

			// Direct linedefs
			General.Map.Map.MarkSelectedLinedefs(true, mark);

			// Linedefs from vertices
			// We do this before "vertices from lines" because otherwise we get lines marked that we didn't select
			if(linedefsfromvertices)
			{
				ICollection<Linedef> lines = General.Map.Map.LinedefsFromMarkedVertices(!mark, mark, !mark);
				foreach(Linedef l in lines) l.Marked = mark;
			}
			
			// Vertices from linedefs
			if(verticesfromlinedefs)
			{
				ICollection<Vertex> verts = General.Map.Map.GetVerticesFromLinesMarks(mark);
				foreach(Vertex v in verts) v.Marked = mark;
			}
			
			// Mark sectors from linedefs (note: this must be the first to mark
			// sectors, because this clears the sector marks!)
			if(sectorsfromlinedefs)
			{
				General.Map.Map.ClearMarkedSectors(mark);
				foreach(Linedef l in General.Map.Map.Linedefs)
				{
					if(!l.Selected)
					{
						if(l.Front != null) l.Front.Sector.Marked = !mark;
						if(l.Back != null) l.Back.Sector.Marked = !mark;
					}
				}
			}
			
			// Direct sectors
			General.Map.Map.MarkSelectedSectors(true, mark);

			// Direct things
			General.Map.Map.MarkSelectedThings(true, mark);

			// Sidedefs from linedefs or sectors
			if(sidedefsfromsectors)
				General.Map.Map.MarkSidedefsFromSectors(true, mark);
			else
				General.Map.Map.MarkSidedefsFromLinedefs(true, mark);
		}
		
		#endregion

		#region ================== Indexing
		
		/// <summary>
		/// Returns the vertex at the specified index. Returns null when index is out of range. This is an O(1) operation.
		/// </summary>
		public Vertex GetVertexByIndex(int index)
		{
			return index < numvertices ? vertices[index] : null;
		}

		/// <summary>
		/// Returns the linedef at the specified index. Returns null when index is out of range. This is an O(1) operation.
		/// </summary>
		public Linedef GetLinedefByIndex(int index)
		{
			return index < numlinedefs ? linedefs[index] : null;
		}

		/// <summary>
		/// Returns the sidedef at the specified index. Returns null when index is out of range. This is an O(1) operation.
		/// </summary>
		public Sidedef GetSidedefByIndex(int index)
		{
			return index < numsidedefs ? sidedefs[index] : null;
		}

		/// <summary>
		/// Returns the sector at the specified index. Returns null when index is out of range. This is an O(1) operation.
		/// </summary>
		public Sector GetSectorByIndex(int index)
		{
			return index < numsectors ? sectors[index] : null;
		}

		/// <summary>
		/// Returns the thing at the specified index. Returns null when index is out of range. This is an O(1) operation.
		/// </summary>
		public Thing GetThingByIndex(int index)
		{
			return index < numthings ? things[index] : null;
		}

		#endregion
		
		#region ================== Areas

		/// <summary>This creates an initial, undefined area.</summary>
		public static RectangleF CreateEmptyArea()
		{
			return new RectangleF(float.MaxValue / 2, float.MaxValue / 2, -float.MaxValue, -float.MaxValue);
		}

		/// <summary>This creates an area from vertices.</summary>
		public static RectangleF CreateArea(ICollection<Vertex> verts)
		{
			float l = float.MaxValue;
			float t = float.MaxValue;
			float r = float.MinValue;
			float b = float.MinValue;

			// Go for all vertices
			foreach(Vertex v in verts)
			{
				// Adjust boundaries by vertices
				if(v.Position.x < l) l = v.Position.x;
				if(v.Position.x > r) r = v.Position.x;
				if(v.Position.y < t) t = v.Position.y;
				if(v.Position.y > b) b = v.Position.y;
			}

			// Return a rect
			return new RectangleF(l, t, r - l, b - t);
		}

		/// <summary>This increases and existing area with the given vertices.</summary>
		public static RectangleF IncreaseArea(RectangleF area, ICollection<Vertex> verts)
		{
			float l = area.Left;
			float t = area.Top;
			float r = area.Right;
			float b = area.Bottom;
			
			// Go for all vertices
			foreach(Vertex v in verts)
			{
				// Adjust boundaries by vertices
				if(v.Position.x < l) l = v.Position.x;
				if(v.Position.x > r) r = v.Position.x;
				if(v.Position.y < t) t = v.Position.y;
				if(v.Position.y > b) b = v.Position.y;
			}
			
			// Return a rect
			return new RectangleF(l, t, r - l, b - t);
		}

		/// <summary>This increases and existing area with the given things.</summary>
		public static RectangleF IncreaseArea(RectangleF area, ICollection<Thing> things)
		{
			float l = area.Left;
			float t = area.Top;
			float r = area.Right;
			float b = area.Bottom;
			
			// Go for all vertices
			foreach(Thing th in things)
			{
				// Adjust boundaries by vertices
				if(th.Position.x < l) l = th.Position.x;
				if(th.Position.x > r) r = th.Position.x;
				if(th.Position.y < t) t = th.Position.y;
				if(th.Position.y > b) b = th.Position.y;
			}
			
			// Return a rect
			return new RectangleF(l, t, r - l, b - t);
		}

		/// <summary>This increases and existing area with the given vertices.</summary>
		public static RectangleF IncreaseArea(RectangleF area, ICollection<Vector2D> verts)
		{
			float l = area.Left;
			float t = area.Top;
			float r = area.Right;
			float b = area.Bottom;
			
			// Go for all vertices
			foreach(Vector2D v in verts)
			{
				// Adjust boundaries by vertices
				if(v.x < l) l = v.x;
				if(v.x > r) r = v.x;
				if(v.y < t) t = v.y;
				if(v.y > b) b = v.y;
			}
			
			// Return a rect
			return new RectangleF(l, t, r - l, b - t);
		}

		/// <summary>This increases and existing area with the given vertex.</summary>
		public static RectangleF IncreaseArea(RectangleF area, Vector2D vert)
		{
			float l = area.Left;
			float t = area.Top;
			float r = area.Right;
			float b = area.Bottom;
			
			// Adjust boundaries by vertices
			if(vert.x < l) l = vert.x;
			if(vert.x > r) r = vert.x;
			if(vert.y < t) t = vert.y;
			if(vert.y > b) b = vert.y;
			
			// Return a rect
			return new RectangleF(l, t, r - l, b - t);
		}

		/// <summary>This creates an area from linedefs.</summary>
		public static RectangleF CreateArea(ICollection<Linedef> lines)
		{
			float l = float.MaxValue;
			float t = float.MaxValue;
			float r = float.MinValue;
			float b = float.MinValue;

			// Go for all linedefs
			foreach(Linedef ld in lines)
			{
				// Adjust boundaries by vertices
				if(ld.Start.Position.x < l) l = ld.Start.Position.x;
				if(ld.Start.Position.x > r) r = ld.Start.Position.x;
				if(ld.Start.Position.y < t) t = ld.Start.Position.y;
				if(ld.Start.Position.y > b) b = ld.Start.Position.y;
				if(ld.End.Position.x < l) l = ld.End.Position.x;
				if(ld.End.Position.x > r) r = ld.End.Position.x;
				if(ld.End.Position.y < t) t = ld.End.Position.y;
				if(ld.End.Position.y > b) b = ld.End.Position.y;
			}

			// Return a rect
			return new RectangleF(l, t, r - l, b - t);
		}

		/// <summary>This filters lines by a rectangular area.</summary>
		public static ICollection<Linedef> FilterByArea(ICollection<Linedef> lines, ref RectangleF area)
		{
            if (lines == null)
            {
                return null;
            }

            ICollection<Linedef> newlines = new List<Linedef>(lines.Count);
            
			// Go for all lines
			foreach(Linedef l in lines)
			{
				// Check the cs field bits
				if((GetCSFieldBits(l.Start, ref area) & GetCSFieldBits(l.End, ref area)) == 0)
				{
					// The line could be in the area
					newlines.Add(l);
				}
			}
			
			// Return result
			return newlines;
		}

		// This returns the cohen-sutherland field bits for a vertex in a rectangle area
		private static int GetCSFieldBits(Vertex v, ref RectangleF area)
		{
			int bits = 0;
			if(v.Position.y < area.Top) bits |= 0x01;
			if(v.Position.y > area.Bottom) bits |= 0x02;
			if(v.Position.x < area.Left) bits |= 0x04;
			if(v.Position.x > area.Right) bits |= 0x08;
			return bits;
		}

		/// <summary>This filters vertices by a rectangular area.</summary>
		public static ICollection<Vertex> FilterByArea(ICollection<Vertex> verts, ref RectangleF area)
		{
            if (verts == null)
            {
                return null;
            }

			ICollection<Vertex> newverts = new List<Vertex>(verts.Count);

			// Go for all verts
			foreach(Vertex v in verts)
			{
				// Within rect?
				if((v.Position.x >= area.Left) &&
				   (v.Position.x <= area.Right) &&
				   (v.Position.y >= area.Top) &&
				   (v.Position.y <= area.Bottom))
				{
					// The vertex is in the area
					newverts.Add(v);
				}
			}

			// Return result
			return newverts;
		}

		#endregion

		#region ================== Stitching

		/// <summary>
		/// Stitches marked geometry with non-marked geometry. Returns false when the operation failed.
		/// </summary>
		public bool StitchGeometry()
		{
			ICollection<Linedef> movinglines;
			ICollection<Linedef> fixedlines;
			ICollection<Vertex> nearbyfixedverts;
			ICollection<Vertex> movingverts;
			ICollection<Vertex> fixedverts;
			RectangleF editarea;
			//int stitchundo;

			// Find vertices
			movingverts = General.Map.Map.GetMarkedVertices(true);
			fixedverts = General.Map.Map.GetMarkedVertices(false);
			
			// Find lines that moved during the drag
			movinglines = LinedefsFromMarkedVertices(false, true, true);
			
			// Find all non-moving lines
			fixedlines = LinedefsFromMarkedVertices(true, false, false);
			
			// Determine area in which we are editing
			editarea = MapSet.CreateArea(movinglines);
			editarea = MapSet.IncreaseArea(editarea, movingverts);
			editarea.Inflate(1.0f, 1.0f);
			
			// Join nearby vertices
			BeginAddRemove();
			MapSet.JoinVertices(fixedverts, movingverts, true, MapSet.STITCH_DISTANCE);
			EndAddRemove();
			
			// Update cached values of lines because we need their length/angle
			Update(true, false);

			BeginAddRemove();
			
			// Split moving lines with unselected vertices
			nearbyfixedverts = MapSet.FilterByArea(fixedverts, ref editarea);
			if(!MapSet.SplitLinesByVertices(movinglines, nearbyfixedverts, MapSet.STITCH_DISTANCE, movinglines))
				return false;
			
			// Split non-moving lines with selected vertices
			fixedlines = MapSet.FilterByArea(fixedlines, ref editarea);
			if(!MapSet.SplitLinesByVertices(fixedlines, movingverts, MapSet.STITCH_DISTANCE, movinglines))
				return false;
			
			// Remove looped linedefs
			MapSet.RemoveLoopedLinedefs(movinglines);
			
			// Join overlapping lines
			if(!MapSet.JoinOverlappingLines(movinglines))
				return false;
			
			EndAddRemove();
			
			return true;
		}
		
		#endregion
		
		#region ================== Geometry Tools

		/// <summary>This removes any virtual sectors in the map and returns the number of sectors removed.</summary>
		public int RemoveVirtualSectors()
		{
			int count = 0;
			int index = 0;
			
			// Go for all sectors
			while(index < numsectors)
			{
				// Remove when virtual
				if(sectors[index].Fields.ContainsKey(VIRTUAL_SECTOR_FIELD))
				{
					sectors[index].Dispose();
					count++;
				}
				else
				{
					index++;
				}
			}
			
			return count;
		}

		/// <summary>This removes unused sectors and returns the number of removed sectors.</summary>
		public int RemoveUnusedSectors(bool reportwarnings)
		{
			int count = 0;
			int index = numsectors - 1;
			
			// Go for all sectors
			while(index >= 0)
			{
				// Remove when unused
				if(sectors[index].Sidedefs.Count == 0)
				{
					if(reportwarnings)
						General.ErrorLogger.Add(ErrorType.Warning, "Sector " + index + " was unused and has been removed.");

					sectors[index].Dispose();
					count++;
				}

				index--;
			}
			
			return count;
		}

		/// <summary>This joins overlapping lines together. Returns false when the operation failed.</summary>
		public static bool JoinOverlappingLines(ICollection<Linedef> lines)
		{
			bool joined;
			
			do
			{
				// No joins yet
				joined = false;

				// Go for all the lines
				foreach(Linedef l1 in lines)
				{
					// Check if these vertices have lines that overlap
					foreach(Linedef l2 in l1.Start.Linedefs)
					{
						// Sharing vertices?
						if((l1.End == l2.End) ||
						   (l1.End == l2.Start))
						{
							// Not the same line?
							if(l1 != l2)
							{
								bool oppositedirection = (l1.End == l2.Start);
								bool l2marked = l2.Marked;
								
								// Merge these two linedefs
								while(lines.Remove(l2)) ;
								if(!l2.Join(l1)) return false;
								
								// If l2 was marked as new geometry, we have to make sure
								// that l1's FrontInterior is correct for the drawing procedure
								if(l2marked)
								{
									l1.FrontInterior = l2.FrontInterior ^ oppositedirection;
								}
								// If l1 is marked as new geometry, we may need to flip it to preserve
								// orientation of the original geometry, and update its FrontInterior
								else if(l1.Marked)
								{
									if(oppositedirection)
									{
										l1.FlipVertices();		// This also flips FrontInterior
										l1.FlipSidedefs();
									}
								}
								
								joined = true;
								break;
							}
						}
					}
					
					// Will have to restart when joined
					if(joined) break;
					
					// Check if these vertices have lines that overlap
					foreach(Linedef l2 in l1.End.Linedefs)
					{
						// Sharing vertices?
						if((l1.Start == l2.End) ||
						   (l1.Start == l2.Start))
						{
							// Not the same line?
							if(l1 != l2)
							{
								bool oppositedirection = (l1.Start == l2.End);
								bool l2marked = l2.Marked;
								
								// Merge these two linedefs
								while(lines.Remove(l2)) ;
								if(!l2.Join(l1)) return false;

								// If l2 was marked as new geometry, we have to make sure
								// that l1's FrontInterior is correct for the drawing procedure
								if(l2marked)
								{
									l1.FrontInterior = l2.FrontInterior ^ oppositedirection;
								}
								// If l1 is marked as new geometry, we may need to flip it to preserve
								// orientation of the original geometry, and update its FrontInterior
								else if(l1.Marked)
								{
									if(oppositedirection)
									{
										l1.FlipVertices();		// This also flips FrontInterior
										l1.FlipSidedefs();
									}
								}

								joined = true;
								break;
							}
						}
					}
					
					// Will have to restart when joined
					if(joined) break;
				}
			}
			while(joined);

			// Return result
			return true;
		}

		/// <summary>This removes looped linedefs (linedefs which reference the same vertex for
		/// start and end) and returns the number of linedefs removed.</summary>
		public static int RemoveLoopedLinedefs(ICollection<Linedef> lines)
		{
			int linesremoved = 0;
			bool removedline;

			do
			{
				// Nothing removed yet
				removedline = false;

				// Go for all the lines
				foreach(Linedef l in lines)
				{
					// Check if referencing the same vertex twice
					if(l.Start == l.End)
					{
						// Remove this line
						while(lines.Remove(l));
						l.Dispose();
						linesremoved++;
						removedline = true;
						break;
					}
				}
			}
			while(removedline);

			// Return result
			return linesremoved;
		}

		/// <summary>This joins nearby vertices from two collections. This does NOT join vertices
		/// within the same collection, only if they exist in both collections.
		/// The vertex from the second collection is moved to match the first vertex.
		/// When keepsecond is true, the vertex in the second collection is kept,
		/// otherwise the vertex in the first collection is kept.
		/// Returns the number of joins made.</summary>
		public static int JoinVertices(ICollection<Vertex> set1, ICollection<Vertex> set2, bool keepsecond, float joindist)
		{
			float joindist2 = joindist * joindist;
			int joinsdone = 0;
			bool joined;

			do
			{
				// No joins yet
				joined = false;

				// Go for all vertices in the first set
				foreach(Vertex v1 in set1)
				{
					// Go for all vertices in the second set
					foreach(Vertex v2 in set2)
					{
						// Check if vertices are close enough
						if(v1.DistanceToSq(v2.Position) <= joindist2)
						{
							// Check if not the same vertex
							if(v1 != v2)
							{
								// Move the second vertex to match the first
								v2.Move(v1.Position);
								
								// Check which one to keep
								if(keepsecond)
								{
									// Join the first into the second
									// Second is kept, first is removed
									v1.Join(v2);
									set1.Remove(v1);
									set2.Remove(v1);
								}
								else
								{
									// Join the second into the first
									// First is kept, second is removed
									v2.Join(v1);
									set1.Remove(v2);
									set2.Remove(v2);
								}
								
								// Count the join
								joinsdone++;
								joined = true;
								break;
							}
						}
					}

					// Will have to restart when joined
					if(joined) break;
				}
			}
			while(joined);

			// Return result
			return joinsdone;
		}

        // anotak - within a single list
        public static List<Vertex> JoinVerticesOneList(List<Vertex> vertices, float joindist)
        {
            float joindist2 = joindist * joindist;
            int count = vertices.Count;
            List<Vertex> output = new List<Vertex>(count);

            for (int i = 0; i < count - 1; i++)
            {
                bool bKeep = true;
                Vertex current = vertices[i];

                for (int j = i+1; j < count; j++)
                {
                    if (current.DistanceToSq(vertices[j].Position) <= joindist2)
                    {
                        bKeep = false;

                        if (current != vertices[j])
                        {
                            current.Join(vertices[j]);
                        }
                        break;
                    }
                }
                if (bKeep)
                {
                    output.Add(current);
                }
            }

            output.Add(vertices[count - 1]);

            return output;
        }

        // cleans out disposed vertices
        public static List<Vertex> CleanDisposed(List<Vertex> input)
        {
            int count = input.Count;
            List<Vertex> output = new List<Vertex>(count);
            for (int i = 0; i < count; i++)
            {
                Vertex current = input[i];
                if (!input[i].IsDisposed)
                {
                    output.Add(current);
                }
            }
            return output;
        }

        /// <summary>This corrects lines that have a back sidedef but no front sidedef by flipping them. Returns the number of flips made.</summary>
        public static int FlipBackwardLinedefs(ICollection<Linedef> lines)
		{
			int flipsdone = 0;
			
			// Examine all lines
			foreach(Linedef l in lines)
			{
				// Back side but no front side?
				if((l.Back != null) && (l.Front == null))
				{
					// Flip that linedef!
					l.FlipVertices();
					l.FlipSidedefs();
					flipsdone++;
				}
			}

			// Return result
			return flipsdone;
		}

		/// <summary>This splits the given lines with the given vertices. All affected lines
		/// will be added to changedlines. Returns false when the operation failed.</summary>
		public static bool SplitLinesByVertices(ICollection<Linedef> lines, ICollection<Vertex> verts, float splitdist, ICollection<Linedef> changedlines)
		{
			float splitdist2 = splitdist * splitdist;
			bool splitted;

			do
			{
				// No split yet
				splitted = false;
				
				// Go for all the lines
				foreach(Linedef l in lines)
				{
					// Go for all the vertices
					foreach(Vertex v in verts)
					{
                        if (v.IsDisposed) // anotak - fixes a crash
                        {
                            continue;
                        }
						// Check if v is close enough to l for splitting
						if(l.DistanceToSq(v.Position, true) <= splitdist2)
						{
							// Line is not already referencing v?
							Vector2D deltastart = l.Start.Position - v.Position;
							Vector2D deltaend = l.End.Position - v.Position;
							if(((Math.Abs(deltastart.x) > 0.001f) ||
							    (Math.Abs(deltastart.y) > 0.001f)) &&
							   ((Math.Abs(deltaend.x) > 0.001f) ||
							    (Math.Abs(deltaend.y) > 0.001f)))
							{
								// Split line l with vertex v
								Linedef nl = l.Split(v);
								if(nl == null) return false;

								// Add the new line to the list
								lines.Add(nl);

								// Both lines must be updated because their new length
								// is relevant for next iterations!
								l.UpdateCache();
								nl.UpdateCache();

								// Add both lines to changedlines
								if(changedlines != null)
								{
									changedlines.Add(l);
									changedlines.Add(nl);
								}

								// Count the split
								splitted = true;
								break;
							}
						}
					}

					// Will have to restart when splitted
					// TODO: If we make (linked) lists from the collections first,
					// we don't have to restart when splitted?
					if(splitted) break;
				}
			}
			while(splitted);
			
			return true;
		}

		/// <summary>This finds the side closest to the specified position.</summary>
		public static Sidedef NearestSidedef(ICollection<Sidedef> selection, Vector2D pos)
		{
			Sidedef closest = null;
			float distance = float.MaxValue;
			
			// Go for all sidedefs in selection
			foreach(Sidedef sd in selection)
			{
				// Calculate distance and check if closer than previous find
				float d = sd.Line.SafeDistanceToSq(pos, true);
				if(d == distance)
				{
					// Same distance, so only pick the one that is on the right side of the line
					float side = sd.Line.SideOfLine(pos);
					if(((side <= 0.0f) && sd.IsFront) || ((side > 0.0f) && !sd.IsFront))
					{
						closest = sd;
						distance = d;
					}
				}
				else if(d < distance)
				{
					// This one is closer
					closest = sd;
					distance = d;
				}
			}
			
			// Return result
			return closest;
		}

		/// <summary>This finds the line closest to the specified position.</summary>
		public static Linedef NearestLinedef(ICollection<Linedef> selection, Vector2D pos)
		{
			Linedef closest = null;
			float distance = float.MaxValue;

			// Go for all linedefs in selection
			foreach(Linedef l in selection)
			{
				// Calculate distance and check if closer than previous find
				float d = l.SafeDistanceToSq(pos, true);
				if(d < distance)
				{
					// This one is closer
					closest = l;
					distance = d;
				}
			}

			// Return result
			return closest;
		}

		/// <summary>This finds the line closest to the specified position.</summary>
		public static Linedef NearestLinedefRange(ICollection<Linedef> selection, Vector2D pos, float maxrange)
		{
			Linedef closest = null;
			float distance = float.MaxValue;
			float maxrangesq = maxrange * maxrange;
			float d;

			// Go for all linedefs in selection
			foreach(Linedef l in selection)
			{
				// Calculate distance and check if closer than previous find
				d = l.SafeDistanceToSq(pos, true);
				if((d <= maxrangesq) && (d < distance))
				{
					// This one is closer
					closest = l;
					distance = d;
				}
			}
			
			// Return result
			return closest;
		}

		/// <summary>This finds the vertex closest to the specified position.</summary>
		public static Vertex NearestVertex(ICollection<Vertex> selection, Vector2D pos)
		{
			Vertex closest = null;
			float distance = float.MaxValue;
			float d;
			
			// Go for all vertices in selection
			foreach(Vertex v in selection)
			{
				// Calculate distance and check if closer than previous find
				d = v.DistanceToSq(pos);
				if(d < distance)
				{
					// This one is closer
					closest = v;
					distance = d;
				}
			}

			// Return result
			return closest;
		}

		/// <summary>This finds the thing closest to the specified position.</summary>
		public static Thing NearestThing(ICollection<Thing> selection, Vector2D pos)
		{
			Thing closest = null;
			float distance = float.MaxValue;
			float d;

			// Go for all things in selection
			foreach(Thing t in selection)
			{
				// Calculate distance and check if closer than previous find
				d = t.DistanceToSq(pos);
				if(d < distance)
				{
					// This one is closer
					closest = t;
					distance = d;
				}
			}

			// Return result
			return closest;
		}

		/// <summary>This finds the vertex closest to the specified position.</summary>
		public static Vertex NearestVertexSquareRange(ICollection<Vertex> selection, Vector2D pos, float maxrange)
		{
			RectangleF range = RectangleF.FromLTRB(pos.x - maxrange, pos.y - maxrange, pos.x + maxrange, pos.y + maxrange);
			Vertex closest = null;
			float distance = float.MaxValue;
			float d;

			// Go for all vertices in selection
			foreach(Vertex v in selection)
			{
				// Within range?
				if((v.Position.x >= range.Left) && (v.Position.x <= range.Right))
				{
					if((v.Position.y >= range.Top) && (v.Position.y <= range.Bottom))
					{
						// Close than previous find?
						d = Math.Abs(v.Position.x - pos.x) + Math.Abs(v.Position.y - pos.y);
						if(d < distance)
						{
							// This one is closer
							closest = v;
							distance = d;
						}
					}
				}
			}

			// Return result
			return closest;
		}

		/// <summary>This finds the thing closest to the specified position.</summary>
		public static Thing NearestThingSquareRange(ICollection<Thing> selection, Vector2D pos, float maxrange)
		{
			RectangleF range = RectangleF.FromLTRB(pos.x - maxrange, pos.y - maxrange, pos.x + maxrange, pos.y + maxrange);
			Thing closest = null;
			float distance = float.MaxValue;
			float d;

			// Go for all vertices in selection
			foreach(Thing t in selection)
			{
				// Within range?
				if((t.Position.x >= (range.Left - t.Size)) && (t.Position.x <= (range.Right + t.Size)))
				{
					if((t.Position.y >= (range.Top - t.Size)) && (t.Position.y <= (range.Bottom + t.Size)))
					{
						// Close than previous find?
						d = Math.Abs(t.Position.x - pos.x) + Math.Abs(t.Position.y - pos.y);
						if(d < distance)
						{
							// This one is closer
							closest = t;
							distance = d;
						}
					}
				}
			}

			// Return result
			return closest;
		}
		
		#endregion

		#region ================== Tools

		/// <summary>This snaps all vertices to the map format accuracy. Call this to ensure the vertices are at valid coordinates.</summary>
		public void SnapAllToAccuracy()
		{
			foreach(Vertex v in vertices) v.SnapToAccuracy();
			foreach(Thing t in things) t.SnapToAccuracy();
		}

		/// <summary>This returns the next unused tag number.</summary>
		public int GetNewTag()
		{
			Dictionary<int, bool> usedtags = new Dictionary<int, bool>();
			ForAllTags(NewTagHandler, false, usedtags);
			ForAllTags(NewTagHandler, true, usedtags);
			
			// Now find the first unused index
			for(int i = 1; i <= General.Map.FormatInterface.MaxTag; i++)
				if(!usedtags.ContainsKey(i)) return i;
			
			// All tags used!
			return 0;
		}

		/// <summary>This returns the next unused tag number within the marked geometry.</summary>
		public int GetNewTag(bool marked)
		{
			Dictionary<int, bool> usedtags = new Dictionary<int, bool>();
			ForAllTags(NewTagHandler, marked, usedtags);

			// Now find the first unused index
			for(int i = 1; i <= General.Map.FormatInterface.MaxTag; i++)
				if(!usedtags.ContainsKey(i)) return i;

			// All tags used!
			return 0;
		}

		/// <summary>This returns the next unused tag number.</summary>
		public List<int> GetMultipleNewTags(int count)
		{
			List<int> newtags = new List<int>(count);
			if(count > 0)
			{
				Dictionary<int, bool> usedtags = new Dictionary<int, bool>();
				ForAllTags(NewTagHandler, false, usedtags);
				ForAllTags(NewTagHandler, true, usedtags);
				
				// Find unused tags and add them
				for(int i = 1; i <= General.Map.FormatInterface.MaxTag; i++)
				{
					if(!usedtags.ContainsKey(i))
					{
						newtags.Add(i);
						if(newtags.Count == count) break;
					}
				}
			}
			
			return newtags;
		}

		/// <summary>This returns the next unused tag number within the marked geometry.</summary>
		public List<int> GetMultipleNewTags(int count, bool marked)
		{
			List<int> newtags = new List<int>(count);
			if(count > 0)
			{
				Dictionary<int, bool> usedtags = new Dictionary<int, bool>();
				ForAllTags(NewTagHandler, marked, usedtags);

				// Find unused tags and add them
				for(int i = 1; i <= General.Map.FormatInterface.MaxTag; i++)
				{
					if(!usedtags.ContainsKey(i))
					{
						newtags.Add(i);
						if(newtags.Count == count) break;
					}
				}
			}
			
			return newtags;
		}

		// Handler for finding a new tag
		private void NewTagHandler(MapElement element, bool actionargument, UniversalType type, ref int value, Dictionary<int, bool> usedtags)
		{
			usedtags[value] = true;
		}

		/// <summary>This calls a function for all tag fields in the marked or unmarked geometry. The obj parameter can be anything you wish to pass on to your TagHandler function.</summary>
		public void ForAllTags<T>(TagHandler<T> handler, bool marked, T obj)
		{
			// Remove tags from sectors
			foreach(Sector s in sectors)
				if(s.Marked == marked)
				{
					int tag = s.Tag;
					handler(s, false, UniversalType.SectorTag, ref tag, obj);
					if(tag != s.Tag) s.Tag = tag;
				}
			
			// Remove tags from things
			if(General.Map.FormatInterface.HasThingTag)
			{
				foreach(Thing t in things)
					if(t.Marked == marked)
					{
						int tag = t.Tag;
						handler(t, false, UniversalType.ThingTag, ref tag, obj);
						if(tag != t.Tag) t.Tag = tag;
					}
			}

			// Remove tags from thing actions
			if(General.Map.FormatInterface.HasThingAction &&
			   General.Map.FormatInterface.HasActionArgs)
			{
				foreach(Thing t in things)
				{
					if(t.Marked == marked)
					{
						LinedefActionInfo info = General.Map.Config.GetLinedefActionInfo(t.Action);
						for(int i = 0; i < Thing.NUM_ARGS; i++)
							if(info.Args[i].Used && CheckIsTagType(info.Args[i].Type))
							{
								int tag = t.Args[i];
								handler(t, true, (UniversalType)(info.Args[i].Type), ref tag, obj);
								if(tag != t.Args[i]) t.Args[i] = tag;
							}
					}
				}
			}

			// Remove tags from linedefs
			if(General.Map.FormatInterface.HasLinedefTag)
			{
				foreach(Linedef l in linedefs)
					if(l.Marked == marked)
					{
						int tag = l.Tag;
						handler(l, false, UniversalType.LinedefTag, ref tag, obj);
						if(tag != l.Tag) l.Tag = tag;
					}
			}

			// Remove tags from linedef actions
			if(General.Map.FormatInterface.HasActionArgs)
			{
				foreach(Linedef l in linedefs)
				{
					if(l.Marked == marked)
					{
						LinedefActionInfo info = General.Map.Config.GetLinedefActionInfo(l.Action);
						for(int i = 0; i < Linedef.NUM_ARGS; i++)
							if(info.Args[i].Used && CheckIsTagType(info.Args[i].Type))
							{
								int tag = l.Args[i];
								handler(l, true, (UniversalType)(info.Args[i].Type), ref tag, obj);
								if(tag != l.Args[i]) l.Args[i] = tag;
							}
					}
				}
			}
		}
		
		// This checks if the given action argument type is a tag type
		private bool CheckIsTagType(int argtype)
		{
			return (argtype == (int)UniversalType.LinedefTag) ||
				   (argtype == (int)UniversalType.SectorTag) ||
				   (argtype == (int)UniversalType.ThingTag);
		}
		
		/// <summary>This makes a list of lines related to marked vertices.
		/// A line is unstable when one vertex is marked and the other isn't.</summary>
		public ICollection<Linedef> LinedefsFromMarkedVertices(bool includeunselected, bool includestable, bool includeunstable)
		{
			List<Linedef> list = new List<Linedef>((numlinedefs / 2) + 1);
			
			// Go for all lines
			foreach(Linedef l in linedefs)
			{
				// Check if this is to be included
				if((includestable && (l.Start.Marked && l.End.Marked)) ||
				   (includeunstable && (l.Start.Marked ^ l.End.Marked)) ||
				   (includeunselected && (!l.Start.Marked && !l.End.Marked)))
				{
					// Add to list
					list.Add(l);
				}
			}

			// Return result
			return list;
		}

		/// <summary>This makes a list of unstable lines from the given vertices.
		/// A line is unstable when one vertex is selected and the other isn't.</summary>
		public static ICollection<Linedef> UnstableLinedefsFromVertices(ICollection<Vertex> verts)
		{
			Dictionary<Linedef, Linedef> lines = new Dictionary<Linedef, Linedef>();

			// Go for all vertices
			foreach(Vertex v in verts)
			{
				// Go for all lines
				foreach(Linedef l in v.Linedefs)
				{
					// If the line exists in the list
					if(lines.ContainsKey(l))
					{
						// Remove it
						lines.Remove(l);
					}
					// Otherwise add it
					else
					{
						// Add the line
						lines.Add(l, l);
					}
				}
			}
			
			// Return result
			return new List<Linedef>(lines.Values);
		}

		/// <summary>This finds the line closest to the specified position.</summary>
		public Linedef NearestLinedef(Vector2D pos) { return MapSet.NearestLinedef(linedefs, pos); }

		/// <summary>This finds the line closest to the specified position.</summary>
		public Linedef NearestLinedefRange(Vector2D pos, float maxrange) { return MapSet.NearestLinedefRange(linedefs, pos, maxrange); }

		/// <summary>This finds the vertex closest to the specified position.</summary>
		public Vertex NearestVertex(Vector2D pos) { return MapSet.NearestVertex(vertices, pos); }

		/// <summary>This finds the vertex closest to the specified position.</summary>
		public Vertex NearestVertexSquareRange(Vector2D pos, float maxrange) { return MapSet.NearestVertexSquareRange(vertices, pos, maxrange); }

		/// <summary>This finds the thing closest to the specified position.</summary>
		public Thing NearestThingSquareRange(Vector2D pos, float maxrange) { return MapSet.NearestThingSquareRange(things, pos, maxrange); }

		/// <summary>This finds the closest unselected linedef that is not connected to the given vertex.</summary>
		public Linedef NearestUnselectedUnreferencedLinedef(Vector2D pos, float maxrange, Vertex v, out float distance)
		{
			Linedef closest = null;
			distance = float.MaxValue;
			float maxrangesq = maxrange * maxrange;
			float d;

			// Go for all linedefs in selection
			foreach(Linedef l in linedefs)
			{
				// Calculate distance and check if closer than previous find
				d = l.SafeDistanceToSq(pos, true);
				if((d <= maxrangesq) && (d < distance))
				{
					// Check if not selected

					// Check if linedef is not connected to v
					if((l.Start != v) && (l.End != v))
					{
						// This one is closer
						closest = l;
						distance = d;
					}
				}
			}

			// Return result
			return closest;
		}
		
		// This performs sidedefs compression
		// Note: Only use this for saving, because this messes up the expected data structure horribly.
		internal void CompressSidedefs()
		{
			Dictionary<uint, List<Sidedef>> storedsides = new Dictionary<uint, List<Sidedef>>(numsidedefs);
			int originalsidescount = numsidedefs;
			double starttime = General.stopwatch.Elapsed.TotalMilliseconds;

			BeginAddRemove();
			
			int sn = 0;
			while(sn < numsidedefs)
			{
				Sidedef stored = null;
				Sidedef snsd = sidedefs[sn];

				// Check if checksum is stored
				bool samesidedef = false;
				uint checksum = snsd.GetChecksum();
				bool checksumstored = storedsides.ContainsKey(checksum);
				if(checksumstored)
				{
					List<Sidedef> othersides = storedsides[checksum];
					foreach(Sidedef os in othersides)
					{
						// They must be in the same sector
						if (snsd.Sector == os.Sector)
						{
							// Check if sidedefs are really the same
							stored = os;
							MemoryStream sidemem = new MemoryStream(1024);
							SerializerStream sidedata = new SerializerStream(sidemem);
							MemoryStream othermem = new MemoryStream(1024);
							SerializerStream otherdata = new SerializerStream(othermem);
							snsd.ReadWrite(sidedata);
							os.ReadWrite(otherdata);
							if (sidemem.Length == othermem.Length)
							{
								samesidedef = true;
								sidemem.Seek(0, SeekOrigin.Begin);
								othermem.Seek(0, SeekOrigin.Begin);
								for (int i = 0; i < sidemem.Length; i++)
								{
									if (sidemem.ReadByte() != othermem.ReadByte())
									{
										samesidedef = false;
										break;
									}
								}
							}

							if (samesidedef) break;
						}
					}
				}

				// Same sidedef?
				if(samesidedef)
				{
					// Replace with stored sidedef
					bool isfront = snsd.IsFront;
					Linedef ld = snsd.Line;
					snsd.Line.DetachSidedefP(snsd);
					if(isfront)
						ld.AttachFront(stored);
					else
						ld.AttachBack(stored);
					
					// Remove the sidedef
					snsd.SetSector(null);
					RemoveSidedef(sn);
				}
				else
				{
					// Store this new one
					if(checksumstored)
					{
						storedsides[checksum].Add(snsd);
					}
					else
					{
						List<Sidedef> newlist = new List<Sidedef>(4);
						newlist.Add(snsd);
						storedsides.Add(checksum, newlist);
					}
					
					// Next
					sn++;
				}
			}

			EndAddRemove();

			// Output info
			double endtime = General.stopwatch.Elapsed.TotalMilliseconds;
			double deltatimesec = (endtime - starttime) / 1000.0d;
			float ratio = 100.0f - (((float)numsidedefs / (float)originalsidescount) * 100.0f);
			Logger.WriteLogLine("Sidedefs compressed: " + numsidedefs + " remaining out of " + originalsidescount + " (" + ratio.ToString("########0.00") + "%) in " + deltatimesec.ToString("########0.00") + " seconds");
		}

		// This converts flags and activations to UDMF fields
		internal void TranslateToUDMF()
		{
			foreach(Linedef l in linedefs) l.TranslateToUDMF();
			foreach(Thing t in things) t.TranslateToUDMF();
		}

		// This converts UDMF fields back into flags and activations
		// NOTE: Only converts the marked items
		internal void TranslateFromUDMF()
		{
			foreach(Linedef l in linedefs) if(l.Marked) l.TranslateFromUDMF();
			foreach(Thing t in things) if(t.Marked) t.TranslateFromUDMF();
		}

		/// <summary>This removes unused vertices.</summary>
		public void RemoveUnusedVertices()
		{
			// Go for all vertices
			int index = numvertices - 1;
			while(index >= 0)
			{
				if((vertices[index] != null) && (vertices[index].Linedefs.Count == 0))
					vertices[index].Dispose();
				else
					index--;
			}
		}
		
		#endregion
	}
}
