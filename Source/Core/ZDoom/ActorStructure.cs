
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
	public sealed class ActorStructure
	{
		#region ================== Constants
		
		private readonly string[] SPRITE_POSTFIXES = new string[] {"2C8", "2D8", "2A8", "2B8", "1C1", "1D1", "1A1", "1B1", "A2", "A1", "A0", "2", "1", "0" };
		
		#endregion
		
		#region ================== Variables
		
		// Declaration
		private string classname;
		private string inheritclass;
		private string replaceclass;
		private int doomednum = -1;
		
		// Inheriting
		private ActorStructure baseclass;
		private bool skipsuper;
		
		// Flags
		private Dictionary<string, bool> flags;
		
		// Properties
		private Dictionary<string, List<string>> props;
		
		// States
		private Dictionary<string, StateStructure> states;
		
		#endregion
		
		#region ================== Properties
		
		public Dictionary<string, bool> Flags { get { return flags; } }
		public Dictionary<string, List<string>> Properties { get { return props; } }
		public string ClassName { get { return classname; } }
		public string InheritsClass { get { return inheritclass; } }
		public string ReplacesClass { get { return replaceclass; } }
		public ActorStructure BaseClass { get { return baseclass; } }
		public int DoomEdNum { get { return doomednum; } }
		
		#endregion
		
		#region ================== Constructor / Disposer
		
		// Constructor
		internal ActorStructure(DecorateParser parser)
		{
			// Initialize
			flags = new Dictionary<string, bool>();
			props = new Dictionary<string, List<string>>();
			states = new Dictionary<string, StateStructure>();
			
			// Always define a game property, but default to 0 values
			props["game"] = new List<string>();
			
			inheritclass = "actor";
			replaceclass = null;
			baseclass = null;
			skipsuper = false;
			
			// First next token is the class name
			parser.SkipWhitespace(true);
			classname = parser.StripTokenQuotes(parser.ReadToken());
			if(string.IsNullOrEmpty(classname))
			{
				parser.ReportError("Expected actor class name");
				return;
			}

			// Parse tokens before entering the actor scope
			while(parser.SkipWhitespace(true))
			{
				string token = parser.ReadToken();
				if(!string.IsNullOrEmpty(token))
				{
					token = token.ToLowerInvariant();
					if(token == ":")
					{
						// The next token must be the class to inherit from
						parser.SkipWhitespace(true);
						inheritclass = parser.StripTokenQuotes(parser.ReadToken());
						if(string.IsNullOrEmpty(inheritclass) || parser.IsSpecialToken(inheritclass))
						{
							parser.ReportError("Expected class name to inherit from");
							return;
						}
						else
						{
							// Find the actor to inherit from
							baseclass = parser.GetArchivedActorByName(inheritclass);
							if(baseclass == null)
								General.ErrorLogger.Add(ErrorType.Warning, "Unable to find the DECORATE class '" + inheritclass + "' to inherit from, while parsing '" + classname + "'");
						}
					}
					else if(token == "replaces")
					{
						// The next token must be the class to replace
						parser.SkipWhitespace(true);
						replaceclass = parser.StripTokenQuotes(parser.ReadToken());
						if(string.IsNullOrEmpty(replaceclass) || parser.IsSpecialToken(replaceclass))
						{
							parser.ReportError("Expected class name to replace");
							return;
						}
					}
					else if(token == "native")
					{
						// Igore this token
					}
					else if(token == "{")
					{
						// Actor scope begins here,
						// break out of this parse loop
						break;
					}
					else if(token == "-")
					{
						// This could be a negative doomednum (but our parser sees the - as separate token)
						// So read whatever is after this token and ignore it (negative doomednum indicates no doomednum)
						parser.ReadToken();
					}
					else
					{
						// Check if numeric
						if(!int.TryParse(token, NumberStyles.Integer, CultureInfo.InvariantCulture, out doomednum))
						{
							// Not numeric!
							parser.ReportError("Expected numeric editor thing number or start of actor scope");
							return;
						}
					}
				}
				else
				{
					parser.ReportError("Unexpected end of structure");
					return;
				}
			}
			
			// Now parse the contents of actor structure
			string previoustoken = "";
			while(parser.SkipWhitespace(true))
			{
				string token = parser.ReadToken();
				token = token.ToLowerInvariant();

				if((token == "+") || (token == "-"))
				{
					// Next token is a flag (option) to set or remove
					bool flagvalue = (token == "+");
					parser.SkipWhitespace(true);
					string flagname = parser.ReadToken();
					if(!string.IsNullOrEmpty(flagname))
					{
						// Add the flag with its value
						flagname = flagname.ToLowerInvariant();
						flags[flagname] = flagvalue;
					}
					else
					{
						parser.ReportError("Expected flag name");
						return;
					}
				}
				else if((token == "action") || (token == "native"))
				{
					// We don't need this, ignore up to the first next ;
					while(parser.SkipWhitespace(true))
					{
						string t = parser.ReadToken();
						if((t == ";") || (t == null)) break;
					}
				}
				else if(token == "skip_super")
				{
					skipsuper = true;
				}
				else if(token == "states")
				{
					// Now parse actor states until we reach the end of the states structure
					while(parser.SkipWhitespace(true))
					{
						string statetoken = parser.ReadToken();
						if(!string.IsNullOrEmpty(statetoken))
						{
							// Start of scope?
							if(statetoken == "{")
							{
								// This is fine
							}
							// End of scope?
							else if(statetoken == "}")
							{
								// Done with the states,
								// break out of this parse loop
								break;
							}
							// State label?
							else if(statetoken == ":")
							{
								if(!string.IsNullOrEmpty(previoustoken))
								{
									// Parse actor state
									StateStructure st = new StateStructure(this, parser, previoustoken);
									if(parser.HasError) return;
									states[previoustoken.ToLowerInvariant()] = st;
								}
								else
								{
									parser.ReportError("Unexpected end of structure");
									return;
								}
							}
							else
							{
								// Keep token
								previoustoken = statetoken;
							}
						}
						else
						{
							parser.ReportError("Unexpected end of structure");
							return;
						}
					}
				}
				else if(token == "}")
				{
					// Actor scope ends here,
					// break out of this parse loop
					break;
				}
				// Monster property?
				else if(token == "monster")
				{
					// This sets certain flags we are interested in
					flags["shootable"] = true;
					flags["countkill"] = true;
					flags["solid"] = true;
					flags["canpushwalls"] = true;
					flags["canusewalls"] = true;
					flags["activatemcross"] = true;
					flags["canpass"] = true;
					flags["ismonster"] = true;
				}
				// Projectile property?
				else if(token == "projectile")
				{
					// This sets certain flags we are interested in
					flags["noblockmap"] = true;
					flags["nogravity"] = true;
					flags["dropoff"] = true;
					flags["missile"] = true;
					flags["activateimpact"] = true;
					flags["activatepcross"] = true;
					flags["noteleport"] = true;
				}
				// Clearflags property?
				else if(token == "clearflags")
				{
					// Clear all flags
					flags.Clear();
				}
				// Game property?
				else if(token == "game")
				{
					// Include all tokens on the same line
					List<string> games = new List<string>();
					while(parser.SkipWhitespace(false))
					{
						string v = parser.ReadToken();
						if(v == null)
						{
							parser.ReportError("Unexpected end of structure");
							return;
						}
						if(v == "\n") break;
						if(v != ",")
							games.Add(v.ToLowerInvariant());
					}
					props[token] = games;
				}
				// Property
				else
				{
					// Property begins with $? Then the whole line is a single value
					if(token.StartsWith("$"))
					{
						// This is for editor-only properties such as $sprite and $category
						List<string> values = new List<string>();
						if(parser.SkipWhitespace(false))
							values.Add(parser.ReadLine());
						else
							values.Add("");
						props[token] = values;
					}
					else
					{
						// Next tokens up until the next newline are values
						List<string> values = new List<string>();
						while(parser.SkipWhitespace(false))
						{
							string v = parser.ReadToken();
							if(v == null)
							{
								parser.ReportError("Unexpected end of structure");
								return;
							}
							if(v == "\n") break;
							if(v != ",")
								values.Add(v);
						}
						props[token] = values;
					}
				}
				
				// Keep token
				previoustoken = token;
			}
		}
		
		// Disposer
		public void Dispose()
		{
			baseclass = null;
			flags = null;
			props = null;
			states = null;
		}
		
		#endregion
		
		#region ================== Methods
		
		/// <summary>
		/// This checks if the actor has a specific property.
		/// </summary>
		public bool HasProperty(string propname)
		{
			if(props.ContainsKey(propname))
				return true;
			else if(!skipsuper && (baseclass != null))
				return baseclass.HasProperty(propname);
			else
				return false;
		}
		
		/// <summary>
		/// This checks if the actor has a specific property with at least one value.
		/// </summary>
		public bool HasPropertyWithValue(string propname)
		{
			if(props.ContainsKey(propname) && (props[propname].Count > 0))
				return true;
			else if(!skipsuper && (baseclass != null))
				return baseclass.HasPropertyWithValue(propname);
			else
				return false;
		}
		
		/// <summary>
		/// This returns values of a specific property as a complete string. Returns an empty string when the propery has no values.
		/// </summary>
		public string GetPropertyAllValues(string propname)
		{
			if(props.ContainsKey(propname) && (props[propname].Count > 0))
				return string.Join(" ", props[propname].ToArray());
			else if(!skipsuper && (baseclass != null))
				return baseclass.GetPropertyAllValues(propname);
			else
				return "";
		}
		
		/// <summary>
		/// This returns a specific value of a specific property as a string. Returns an empty string when the propery does not have the specified value.
		/// </summary>
		public string GetPropertyValueString(string propname, int valueindex)
		{
			if(props.ContainsKey(propname) && (props[propname].Count > valueindex))
				return props[propname][valueindex];
			else if(!skipsuper && (baseclass != null))
				return baseclass.GetPropertyValueString(propname, valueindex);
			else
				return "";
		}
		
		/// <summary>
		/// This returns a specific value of a specific property as an integer. Returns 0 when the propery does not have the specified value.
		/// </summary>
		public int GetPropertyValueInt(string propname, int valueindex)
		{
			string str = GetPropertyValueString(propname, valueindex);
			
			int intvalue;
			if(int.TryParse(str, NumberStyles.Integer, CultureInfo.InvariantCulture, out intvalue))
				return intvalue;
			else
				return 0;
		}
		
		/// <summary>
		/// This returns a specific value of a specific property as a float. Returns 0.0f when the propery does not have the specified value.
		/// </summary>
		public float GetPropertyValueFloat(string propname, int valueindex)
		{
			string str = GetPropertyValueString(propname, valueindex);
			
			float fvalue;
			if(float.TryParse(str, NumberStyles.Float, CultureInfo.InvariantCulture, out fvalue))
				return fvalue;
			else
				return 0.0f;
		}
		
		/// <summary>
		/// This returns the status of a flag.
		/// </summary>
		public bool HasFlagValue(string flag)
		{
			if(flags.ContainsKey(flag))
				return true;
			else if(!skipsuper && (baseclass != null))
				return baseclass.HasFlagValue(flag);
			else
				return false;
		}
		
		/// <summary>
		/// This returns the status of a flag.
		/// </summary>
		public bool GetFlagValue(string flag, bool defaultvalue)
		{
			if(flags.ContainsKey(flag))
				return flags[flag];
			else if(!skipsuper && (baseclass != null))
				return baseclass.GetFlagValue(flag, defaultvalue);
			else
				return defaultvalue;
		}
		
		/// <summary>
		/// This checks if a state has been defined.
		/// </summary>
		public bool HasState(string statename)
		{
			if(states.ContainsKey(statename))
				return true;
			else if(!skipsuper && (baseclass != null))
				return baseclass.HasState(statename);
			else
				return false;
		}
		
		/// <summary>
		/// This returns a specific state, or null when the state can't be found.
		/// </summary>
		public StateStructure GetState(string statename)
		{
			if(states.ContainsKey(statename))
				return states[statename];
			else if(!skipsuper && (baseclass != null))
				return baseclass.GetState(statename);
			else
				return null;
		}
		
		/// <summary>
		/// This creates a list of all states, also those inherited from the base class.
		/// </summary>
		public Dictionary<string, StateStructure> GetAllStates()
		{
			Dictionary<string, StateStructure> list = new Dictionary<string, StateStructure>(states);
			
			if(!skipsuper && (baseclass != null))
			{
				Dictionary<string, StateStructure> baselist = baseclass.GetAllStates();
				foreach(KeyValuePair<string, StateStructure> s in baselist)
					if(!list.ContainsKey(s.Key)) list.Add(s.Key, s.Value);
			}
			
			return list;
		}
		
		/// <summary>
		/// This checks if this actor is meant for the current decorate game support
		/// </summary>
		public bool CheckActorSupported()
		{
			// Check if we want to include this actor
			string includegames = General.Map.Config.DecorateGames.ToLowerInvariant();
			bool includeactor = (props["game"].Count == 0);
			foreach(string g in props["game"])
				includeactor |= includegames.Contains(g);
			
			return includeactor;
		}
		
		/// <summary>
		/// This finds the best suitable sprite to use when presenting this actor to the user.
		/// </summary>
		public string FindSuitableSprite()
		{
			string result = "";
			
			// Sprite forced?
			if(HasPropertyWithValue("$sprite"))
			{
				return GetPropertyValueString("$sprite", 0);
			}
			else
			{
				// Try the idle state
				if(HasState("idle"))
				{
					StateStructure s = GetState("idle");
					string spritename = s.GetSprite(0);
					if(!string.IsNullOrEmpty(spritename))
						result = spritename;
				}
				
				// Try the see state
				if(string.IsNullOrEmpty(result) && HasState("see"))
				{
					StateStructure s = GetState("see");
					string spritename = s.GetSprite(0);
					if(!string.IsNullOrEmpty(spritename))
						result = spritename;
				}
				
				// Try the inactive state
				if(string.IsNullOrEmpty(result) && HasState("inactive"))
				{
					StateStructure s = GetState("inactive");
					string spritename = s.GetSprite(0);
					if(!string.IsNullOrEmpty(spritename))
						result = spritename;
				}
				
				// Try the spawn state
				if(string.IsNullOrEmpty(result) && HasState("spawn"))
				{
					StateStructure s = GetState("spawn");
					string spritename = s.GetSprite(0);
					if(!string.IsNullOrEmpty(spritename))
						result = spritename;
				}
				
				// Still no sprite found? then just pick the first we can find
				if(string.IsNullOrEmpty(result))
				{
					Dictionary<string, StateStructure> list = GetAllStates();
					foreach(StateStructure s in list.Values)
					{
						string spritename = s.GetSprite(0);
						if(!string.IsNullOrEmpty(spritename))
						{
							result = spritename;
							break;
						}
					}
				}
				
				if(!string.IsNullOrEmpty(result))
				{
					// The sprite name is not actually complete, we still have to append
					// the direction characters to it. Find an existing sprite with direction.
					foreach(string postfix in SPRITE_POSTFIXES)
					{
						if(General.Map.Data.GetSpriteExists(result + postfix))
							return result + postfix;
					}
				}
			}
			
			// No sprite found
			return "";
		}
		
		#endregion
	}
}
