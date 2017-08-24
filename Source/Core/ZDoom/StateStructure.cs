
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
	public sealed class StateStructure
	{
		#region ================== Constants
		
		// Some odd thing in ZDoom
		private const string IGNORE_SPRITE = "TNT1A0";
		
		#endregion

		#region ================== Variables
		
		// All we care about is the first sprite in the sequence
		private List<string> sprites;
		private StateGoto gotostate;
		private DecorateParser parser;
		
		#endregion

		#region ================== Properties

		public int SpritesCount { get { return sprites.Count; } }

		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		internal StateStructure(ActorStructure actor, DecorateParser parser, string statename)
		{
			string lasttoken = "";
			
			this.gotostate = null;
			this.parser = parser;
			this.sprites = new List<string>();
			
			// Skip whitespace
			while(parser.SkipWhitespace(true))
			{
				// Read first token
				string token = parser.ReadToken();
				token = token.ToLowerInvariant();
				
				// One of the flow control statements?
				if((token == "loop") || (token == "stop") || (token == "wait") || (token == "fail"))
				{
					// Ignore flow control
				}
				// Goto?
				else if(token == "goto")
				{
					gotostate = new StateGoto(actor, parser);
					if(parser.HasError) return;
				}
				// Label?
				else if(token == ":")
				{
					// Rewind so that this label can be read again
					parser.DataStream.Seek(-(lasttoken.Length + 1), SeekOrigin.Current);
					
					// Done here
					return;
				}
				// End of scope?
				else if(token == "}")
				{
					// Rewind so that this scope end can be read again
					parser.DataStream.Seek(-1, SeekOrigin.Current);

					// Done here
					return;
				}
				else
				{
					// First part of the sprite name
					if(token == null)
					{
						parser.ReportError("Unexpected end of structure");
						return;
					}
					
					// Frames of the sprite name
					parser.SkipWhitespace(true);
					string spriteframes = parser.ReadToken();
					if(spriteframes == null)
					{
						parser.ReportError("Unexpected end of structure");
						return;
					}
					// Label?
					else if(spriteframes == ":")
					{
						// Rewind so that this label can be read again
						parser.DataStream.Seek(-(token.Length + 1), SeekOrigin.Current);

						// Done here
						return;
					}
					
					// No first sprite yet?
					if(spriteframes.Length > 0)
					{
						// Make the sprite name
						string spritename = token + spriteframes[0];
						spritename = spritename.ToUpperInvariant();
						
						// Ignore some odd ZDoom thing
						if(!IGNORE_SPRITE.StartsWith(spritename))
							sprites.Add(spritename);
					}
					
					// Continue until the end of the line
					string t = "";
					while((t != "\n") && (t != null))
					{
						parser.SkipWhitespace(false);
						t = parser.ReadToken();
					}
				}
				
				lasttoken = token;
			}
		}

		#endregion

		#region ================== Methods
		
		// This finds the first valid sprite and returns it
		public string GetSprite(int index)
		{
			List<StateStructure> callstack = new List<StateStructure>();
			return GetSprite(index, callstack);
		}
		
		// This version of GetSprite uses a callstack to check if it isn't going into an endless loop
		private string GetSprite(int index, List<StateStructure> prevstates)
		{
			// If we have sprite of our own, see if we can return this index
			if(index < sprites.Count)
			{
				return sprites[index];
			}
			
			// Otherwise, continue searching where goto tells us to go
			if(gotostate != null)
			{
				// Find the class
				ActorStructure a = parser.GetArchivedActorByName(gotostate.ClassName);
				if(a != null)
				{
					StateStructure s = a.GetState(gotostate.StateName);
					if((s != null) && !prevstates.Contains(s))
					{
						prevstates.Add(this);
						return s.GetSprite(gotostate.SpriteOffset, prevstates);
					}
				}
			}
			
			// If there is no goto keyword used, just give us one of our sprites if we can
			if(sprites.Count > 0)
			{
				// The following behavior should really depend on the flow control keyword (loop or stop) but who cares.
				return sprites[0];
			}
			
			return "";
		}
		
		#endregion
	}
}
