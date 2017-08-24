
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
using CodeImp.DoomBuilder.IO;
using CodeImp.DoomBuilder.Data;
using System.IO;
using System.Diagnostics;
using CodeImp.DoomBuilder.Compilers;

#endregion

namespace CodeImp.DoomBuilder.ZDoom
{
	public sealed class DecorateParser : ZDTextParser
	{
		#region ================== Delegates

		public delegate void IncludeDelegate(DecorateParser parser, string includefile);
		
		public IncludeDelegate OnInclude;
		
		#endregion
		
		#region ================== Constants
		
		#endregion
		
		#region ================== Variables
		
		// These are actors we want to keep
		private Dictionary<string, ActorStructure> actors;
		
		// These are all parsed actors, also those from other games
		private Dictionary<string, ActorStructure> archivedactors;
		
		#endregion
		
		#region ================== Properties
		
		/// <summary>
		/// All actors that are supported by the current game.
		/// </summary>
		public ICollection<ActorStructure> Actors { get { return actors.Values; } }

		/// <summary>
		/// All actors defined in the loaded DECORATE structures. This includes actors not supported in the current game.
		/// </summary>
		public ICollection<ActorStructure> AllActors { get { return archivedactors.Values; } }
		
		#endregion
		
		#region ================== Constructor / Disposer
		
		// Constructor
		public DecorateParser()
		{
			// Syntax
			whitespace = "\n \t\r";
			specialtokens = ":{}+-\n;,";
			
			// Initialize
			actors = new Dictionary<string, ActorStructure>();
			archivedactors = new Dictionary<string, ActorStructure>();
		}
		
		// Disposer
		public void Dispose()
		{
			foreach(KeyValuePair<string, ActorStructure> a in archivedactors)
				a.Value.Dispose();
			
			actors = null;
			archivedactors = null;
		}
		
		#endregion

		#region ================== Parsing

		// This parses the given decorate stream
		// Returns false on errors
		public override bool Parse(Stream stream, string sourcefilename)
		{
			base.Parse(stream, sourcefilename);
			
			// Keep local data
			Stream localstream = datastream;
			string localsourcename = sourcename;
			BinaryReader localreader = datareader;
			
			// Continue until at the end of the stream
			while(SkipWhitespace(true))
			{
				// Read a token
				string objdeclaration = ReadToken();
				if(objdeclaration != null)
				{
					objdeclaration = objdeclaration.ToLowerInvariant();
					if(objdeclaration == "actor")
					{
						// Read actor structure
						ActorStructure actor = new ActorStructure(this);
						if(this.HasError) break;
						
						// Add the actor
						archivedactors[actor.ClassName.ToLowerInvariant()] = actor;
						if(actor.CheckActorSupported())
							actors[actor.ClassName.ToLowerInvariant()] = actor;
						
						// Replace an actor?
						if(actor.ReplacesClass != null)
						{
							if(GetArchivedActorByName(actor.ReplacesClass) != null)
								archivedactors[actor.ReplacesClass.ToLowerInvariant()] = actor;
							else
								General.ErrorLogger.Add(ErrorType.Warning, "Unable to find the DECORATE class '" + actor.ReplacesClass + "' to replace, while parsing '" + actor.ClassName + "'");
							
							if(actor.CheckActorSupported())
							{
								if(GetActorByName(actor.ReplacesClass) != null)
									actors[actor.ReplacesClass.ToLowerInvariant()] = actor;
							}
						}
					}
					else if(objdeclaration == "#include")
					{
						// Include a file
						SkipWhitespace(true);
						string filename = ReadToken();
						if(!string.IsNullOrEmpty(filename))
						{
							// Strip the quotes
							filename = filename.Replace("\"", "");

							// Callback to parse this file now
							if(OnInclude != null) OnInclude(this, filename);

							// Set our buffers back to continue parsing
							datastream = localstream;
							datareader = localreader;
							sourcename = localsourcename;
							if(HasError) break;
						}
						else
						{
							ReportError("Expected file name to include");
							break;
						}
					}
					else if((objdeclaration == "const") || (objdeclaration == "native"))
					{
						// We don't need this, ignore up to the first next ;
						while(SkipWhitespace(true))
						{
							string t = ReadToken();
							if((t == ";") || (t == null)) break;
						}
					}
					else
					{
						// Unknown structure!
						// Best we can do now is just find the first { and then
						// follow the scopes until the matching } is found
						string token2;
						do
						{
							if(!SkipWhitespace(true)) break;
							token2 = ReadToken();
							if(token2 == null) break;
						}
						while(token2 != "{");
						int scopelevel = 1;
						do
						{
							if(!SkipWhitespace(true)) break;
							token2 = ReadToken();
							if(token2 == null) break;
							if(token2 == "{") scopelevel++;
							if(token2 == "}") scopelevel--;
						}
						while(scopelevel > 0);
					}
				}
			}
			
			// Return true when no errors occurred
			return (ErrorDescription == null);
		}
		
		#endregion
		
		#region ================== Methods
		
		/// <summary>
		/// This returns a supported actor by name. Returns null when no supported actor with the specified name can be found. This operation is of O(1) complexity.
		/// </summary>
		public ActorStructure GetActorByName(string name)
		{
			name = name.ToLowerInvariant();
			if(actors.ContainsKey(name))
				return actors[name];
			else
				return null;
		}

		/// <summary>
		/// This returns a supported actor by DoomEdNum. Returns null when no supported actor with the specified name can be found. Please note that this operation is of O(n) complexity!
		/// </summary>
		public ActorStructure GetActorByDoomEdNum(int doomednum)
		{
			ICollection<ActorStructure> collection = actors.Values;
			foreach(ActorStructure a in collection)
			{
				if(a.DoomEdNum == doomednum)
					return a;
			}
			return null;
		}

		// This returns an actor by name
		// Returns null when actor cannot be found
		internal ActorStructure GetArchivedActorByName(string name)
		{
			name = name.ToLowerInvariant();
			if(archivedactors.ContainsKey(name))
				return archivedactors[name];
			else
				return null;
		}
		
		#endregion
	}
}
