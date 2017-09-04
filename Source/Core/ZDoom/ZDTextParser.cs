
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
	public abstract class ZDTextParser
	{
		#region ================== Constants
		
		#endregion
		
		#region ================== Variables
		
		// Parsing
		protected string whitespace = "\n \t\r";
		protected string specialtokens = ":{}+-\n;";
		
		// Input data stream
		protected Stream datastream;
		protected BinaryReader datareader;
		protected string sourcename;
		
		// Error report
		private int errorline;
		private string errordesc;
		private string errorsource;
		
		#endregion
		
		#region ================== Properties
		
		internal Stream DataStream { get { return datastream; } }
		internal BinaryReader DataReader { get { return datareader; } }
		public int ErrorLine { get { return errorline; } }
		public string ErrorDescription { get { return errordesc; } }
		public string ErrorSource { get { return errorsource; } }
		public bool HasError { get { return (errordesc != null); } }
		
		#endregion
		
		#region ================== Constructor / Disposer
		
		// Constructor
		protected ZDTextParser()
		{
			// Initialize
			errordesc = null;
		}
		
		#endregion

		#region ================== Parsing

		// This parses the given decorate stream
		// Returns false on errors
		public virtual bool Parse(Stream stream, string sourcefilename)
		{
            if (stream == null)
            {
                General.ErrorLogger.Add(ErrorType.Error, "unable to open zdtext " + sourcefilename + " due to null ptr");
                return false;
            }
            if (!stream.CanRead)
            {
                General.ErrorLogger.Add(ErrorType.Error, "unable to open zdtext " + sourcefilename + " due to unreadable stream");
                return false;
            }
            datastream = stream;
			datareader = new BinaryReader(stream, Encoding.ASCII);
			sourcename = sourcefilename;
			datastream.Seek(0, SeekOrigin.Begin);
			return true;
		}
		
		// This returns true if the given character is whitespace
		protected internal bool IsWhitespace(char c)
		{
			return (whitespace.IndexOf(c) > -1);
		}

		// This returns true if the given character is a special token
		protected internal bool IsSpecialToken(char c)
		{
			return (specialtokens.IndexOf(c) > -1);
		}

		// This returns true if the given character is a special token
		protected internal bool IsSpecialToken(string s)
		{
			if(s.Length > 0)
				return (specialtokens.IndexOf(s[0]) > -1);
			else
				return false;
		}
		
		// This removes beginning and ending quotes from a token
		protected internal string StripTokenQuotes(string token)
		{
			// Remove first character, if it is a quote
			if(!string.IsNullOrEmpty(token) && (token[0] == '"'))
				token = token.Substring(1);
			
			// Remove last character, if it is a quote
			if(!string.IsNullOrEmpty(token) && (token[token.Length - 1] == '"'))
				token = token.Substring(0, token.Length - 1);
			
			return token;
		}
		
		// This skips whitespace on the stream, placing the read
		// position right before the first non-whitespace character
		// Returns false when the end of the stream is reached
		protected internal bool SkipWhitespace(bool skipnewline)
		{
			int offset = skipnewline ? 0 : 1;
			char c;
			
			do
			{
				if(datastream.Position == datastream.Length) return false;
				c = (char)datareader.ReadByte();

				// Check if this is comment
				if(c == '/')
				{
					if(datastream.Position == datastream.Length) return false;
					char c2 = (char)datareader.ReadByte();
					if(c2 == '/')
					{
						// Check if not a special comment with a token
						if(datastream.Position == datastream.Length) return false;
						char c3 = (char)datareader.ReadByte();
						if(c3 != '$')
						{
							// Skip entire line
							char c4 = ' ';
							while((c4 != '\n') && (datastream.Position < datastream.Length)) { c4 = (char)datareader.ReadByte(); }
							c = c4;
						}
						else
						{
							// Not a comment
							c = c3;
						}
					}
					else if(c2 == '*')
					{
						// Skip until */
						char c4, c3 = '\0';
						do
						{
							c4 = c3;
							c3 = (char)datareader.ReadByte();
						}
						while((c4 != '*') || (c3 != '/'));
						c = ' ';
					}
					else
					{
						// Not a comment, rewind from reading c2
						datastream.Seek(-1, SeekOrigin.Current);
					}
				}
			}
			while(whitespace.IndexOf(c, offset) > -1);
			
			// Go one character back so we can read this non-whitespace character again
			datastream.Seek(-1, SeekOrigin.Current);
			return true;
		}
		
		// This reads a token (all sequential non-whitespace characters or a single character)
		// Returns null when the end of the stream has been reached
		protected internal string ReadToken()
		{
			string token = "";
			bool quotedstring = false;
			
			// Return null when the end of the stream has been reached
			if(datastream.Position == datastream.Length) return null;
			
			// Start reading
			char c = (char)datareader.ReadByte();
			while(!IsWhitespace(c) || quotedstring || IsSpecialToken(c))
			{
				// Special token?
				if(!quotedstring && IsSpecialToken(c))
				{
					// Not reading a token yet?
					if(token.Length == 0)
					{
						// This is our whole token
						token += c;
						break;
					}
					else
					{
						// This is a new token and shouldn't be read now
						// Go one character back so we can read this token again
						datastream.Seek(-1, SeekOrigin.Current);
						break;
					}
				}
				else
				{
					// Quote?
					if(c == '"')
					{
						// Quote to end the string?
						if(quotedstring) quotedstring = false;
						
						// First character is a quote?
						if(token.Length == 0) quotedstring = true;
						
						token += c;
					}
					// Potential comment?
					else if((c == '/') && !quotedstring)
					{
						// Check the next byte
						if(datastream.Position == datastream.Length) return token;
						char c2 = (char)datareader.ReadByte();
						if((c2 == '/') || (c2 == '*'))
						{
							// This is a comment start, so the token ends here
							// Go two characters back so we can read this comment again
							datastream.Seek(-2, SeekOrigin.Current);
							break;
						}
						else
						{
							// Not a comment
							// Go one character back so we can read this char again
							datastream.Seek(-1, SeekOrigin.Current);
							token += c;
						}
					}
					else
					{
						token += c;
					}
				}
				
				// Next character
				if(datastream.Position < datastream.Length)
					c = (char)datareader.Read();
				else
					break;
			}
			
			return token;
		}

		// This reads the rest of the line
		// Returns null when the end of the stream has been reached
		protected internal string ReadLine()
		{
			string token = "";

			// Return null when the end of the stream has been reached
			if(datastream.Position == datastream.Length) return null;

			// Start reading
			char c = (char)datareader.ReadByte();
			while(c != '\n')
			{
				token += c;
				
				// Next character
				if(datastream.Position < datastream.Length)
					c = (char)datareader.Read();
				else
					break;
			}

			return token.Trim();
		}
		
		// This reports an error
		protected internal void ReportError(string message)
		{
			long position = datastream.Position;
			long readpos = 0;
			int linenumber = 1;
			
			// Find the line on which we found this error
			datastream.Seek(0, SeekOrigin.Begin);
			StreamReader textreader = new StreamReader(datastream, Encoding.ASCII);
			while(readpos < position)
			{
				string line = textreader.ReadLine();
				if(line == null) break;
				readpos += line.Length + 2;
				linenumber++;
			}
			
			// Return to original position
			datastream.Seek(position, SeekOrigin.Begin);
			
			// Set error information
			errordesc = message;
			errorline = linenumber;
			errorsource = sourcename;
		}
		
		#endregion
	}
}
