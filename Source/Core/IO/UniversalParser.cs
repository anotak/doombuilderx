
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
using System.IO;
using System.Text;
using System.Globalization;
using System.Collections;
using System.Collections.Specialized;
using System.Collections.Generic;

#endregion

namespace CodeImp.DoomBuilder.IO
{
	public sealed class UniversalParser
	{
		#region ================== Constants
		
		// Path seperator
		public const string DEFAULT_SEPERATOR = ".";
		
		// Allowed characters in a key
		public const string KEY_CHARACTERS = "abcdefghijklmnopqrstuvwxyz0123456789_";
		
		// Parse mode constants
		private const int PM_NOTHING = 0;
		private const int PM_ASSIGNMENT = 1;
		private const int PM_NUMBER = 2;
		private const int PM_STRING = 3;
		private const int PM_KEYWORD = 4;
		
		// Error strings
		private const string ERROR_KEYMISSING = "Missing key name in assignment or scope.";
		private const string ERROR_KEYCHARACTERS = "Invalid characters in key name.";
		private const string ERROR_ASSIGNINVALID = "Invalid assignment. Missing a previous terminator symbol?";
		private const string ERROR_VALUEINVALID = "Invalid value in assignment. Missing a previous terminator symbol?";
		private const string ERROR_VALUETOOBIG = "Value too big.";
		private const string ERROR_KEYWITHOUTVALUE = "Key has no value assigned.";
		private const string ERROR_KEYWORDUNKNOWN = "Unknown keyword in assignment. Missing a previous terminator symbol?";
		
		#endregion
		
		#region ================== Variables
		
		// Error result
		private int cpErrorResult = 0;
		private string cpErrorDescription = "";
		private int cpErrorLine = 0;
		
		// Configuration root
		private UniversalCollection root = null;

		// Settings
		private bool strictchecking = true;
		
		#endregion
		
		#region ================== Properties
		
		// Properties
		public int ErrorResult { get { return cpErrorResult; } }
		public string ErrorDescription { get { return cpErrorDescription; } }
		public int ErrorLine { get { return cpErrorLine; } }
		public UniversalCollection Root { get { return root; } }
		public bool StrictChecking { get { return strictchecking; } set { strictchecking = value; } }
		
		#endregion
		
		#region ================== Constructor / Destructor
		
		// Constructor
		public UniversalParser()
		{
			// Standard new configuration
			NewConfiguration();
			
			// We have no destructor
			GC.SuppressFinalize(this);
		}
		
		// Constructor to load a file immediately
		public UniversalParser(string filename)
		{
			// Load configuration from file
			LoadConfiguration(filename);

			// We have no destructor
			GC.SuppressFinalize(this);
		}
		
		#endregion
		
		#region ================== Private Methods

		// This returns a string added with escape characters
		private string EscapedString(string str)
		{
			// Replace the \ with \\ first!
			str = str.Replace("\\", "\\\\");
			str = str.Replace("\n", "\\n");
			str = str.Replace("\r", "\\r");
			str = str.Replace("\t", "\\t");
			str = str.Replace("\"", "\\\"");
			
			// Return result
			return str;
		}
		
		
		// This raises an error
		private void RaiseError(int line, string description)
		{
			// Raise error
			cpErrorResult = 1;
			cpErrorDescription = description;
			cpErrorLine = line;
		}
		
		
		// This validates a given key and sets
		// error properties if key is invalid and errorline > -1
		private bool ValidateKey(string key, int errorline)
		{
			bool validateresult = true;
			
			// Check if key is an empty string
			if(key.Length == 0)
			{
				// ERROR: Missing key name in statement
				if(errorline > -1) RaiseError(errorline, ERROR_KEYMISSING);
				validateresult = false;
			}
			else
			{
				// Only when strict checking
				if(strictchecking)
				{
					// Check if all characters are valid
					foreach(char c in key)
					{
						if(KEY_CHARACTERS.IndexOf(c) == -1)
						{
							// ERROR: Invalid characters in key name
							if(errorline > -1) RaiseError(errorline, ERROR_KEYCHARACTERS);
							validateresult = false;
							break;
						}
					}
				}
			}
			
			// Return result
			return validateresult;
		}
		
		
		// This parses a structure in the given data starting
		// from the given pos and line and updates pos and line.
		private UniversalCollection InputStructure(ref string data, ref int pos, ref int line)
		{
			char c = '\0';					// current data character
			int pm = PM_NOTHING;			// current parse mode
			//string key = "", val = "";		// current key and value beign built
			bool escape = false;			// escape sequence?
			bool endofstruct = false;		// true as soon as this level struct ends
			UniversalCollection cs = new UniversalCollection();
            StringBuilder keysb = new StringBuilder(32);
            StringBuilder valsb = new StringBuilder(32);

            // Go through all of the data until
            // the end or until the struct closes
            // or when an arror occurred
            while ((pos < data.Length) && (cpErrorResult == 0) && (endofstruct == false))
			{
				// Get current character
				c = data[pos];
				
				// ================ What parse mode are we at?
				if(pm == PM_NOTHING)
				{
					// Now check what character this is
					switch(c)
					{
						case '{': // Begin of new struct

                            // Validate key
                            {
                                string key = keysb.ToString().Trim();
                                if (ValidateKey(key, line))
                                {
                                    // Next character
                                    pos++;

                                    // Parse this struct and add it
                                    cs.Add(new UniversalEntry(key, InputStructure(ref data, ref pos, ref line)));

                                    // Check the last character
                                    pos--;

                                    // Reset the key
                                    keysb.Length = 0;
                                }
                            }
							
							// Leave switch
							break;
							
						case '}': // End of this struct
							
							// Stop parsing in this struct
							endofstruct = true;
							
							// Leave the loop
							break;
							
						case '=': // Assignment
							
							// Validate key
							if(ValidateKey(keysb.ToString().Trim(), line))
							{
								// Now parsing assignment
								pm = PM_ASSIGNMENT;
							}
							
							// Leave switch
							break;
							
						case ';': // Terminator
							
							// Validate key
							if(ValidateKey(keysb.ToString().Trim(), line))
							{
								// Error: No value
								RaiseError(line, ERROR_KEYWITHOUTVALUE);
							}
							
							// Leave switch
							break;
							
						case '\n': // New line
							
							// Count the line
							line++;
							
							// Add this to the key as a space.
							// Spaces are not allowed, but it will be trimmed
							// when its the first or last character.
							keysb.Append(' ');
							
							// Leave switch
							break;
							
						case '\\': // Possible comment
						case '/':
							
							// Check for the line comment //
							if(data.Substring(pos, 2) == "//")
							{
								// Find the next line
								int np = data.IndexOf("\n", pos);
								
								// Next line found?
								if(np > -1)
								{
									// Count the line
									line++;
									
									// Skip everything on this line
									pos = np;
								}
								else
								{
									// No end of line
									// Skip everything else
									pos = data.Length;
								}
							}
								// Check for the block comment /* */
							else if(data.Substring(pos, 2) == "/*")
							{
								// Find the next closing block comment
								int np = data.IndexOf("*/", pos);
								
								// Closing block comment found?
								if(np > -1)
								{
									// Count the lines in the block comment
									string blockdata = data.Substring(pos, np - pos + 2);
									line += (blockdata.Split("\n".ToCharArray()).Length - 1);
									
									// Skip everything in this block
									pos = np + 1;
								}
								else
								{
									// No end of line
									// Skip everything else
									pos = data.Length;
								}
							}
							
							// Leave switch
							break;
							
						default: // Everything else
							
							// Add character to key
							keysb.Append(char.ToLowerInvariant(c));
							
							// Leave switch
							break;
					}
				}
				// ================ Parsing an assignment
				else if(pm == PM_ASSIGNMENT)
				{
					// Check for string opening
					if(c == '\"')
					{
						// Now parsing string
						pm = PM_STRING;
					}
                    // Check for numeric character
                    //else if("0123456789-.&".IndexOf(c.ToString(CultureInfo.InvariantCulture)) > -1)
                    else if ((c >= '0' && c <= '9') || c == '&' || c == '-' || c == '.')
                    {
						// Now parsing number
						pm = PM_NUMBER;
						
						// Go one byte back, because this
						// byte is part of the number!
						pos--;
					}
					// Check for new line
					else if(c == '\n')
					{
						// Count the new line
						line++;
					}
					// Check if assignment ends
					else if(c == ';')
					{
						// End of assignment
						pm = PM_NOTHING;

                        // Remove this if it causes problems
                        keysb.Length = 0;
                        valsb.Length = 0;
					}
					// Otherwise (if not whitespace) it will be a keyword
					else if((c != ' ') && (c != '\t'))
					{
						// Now parsing a keyword
						pm = PM_KEYWORD;
						
						// Go one byte back, because this
						// byte is part of the keyword!
						pos--;
					}
				}
				// ================ Parsing a number
				else if(pm == PM_NUMBER)
				{
					// Check if number ends here
					if(c == ';')
					{
                        string val = valsb.ToString().Trim();
                        string key = keysb.ToString().Trim();
                        // Hexadecimal?
                        if ((val.Length > 2) && val.StartsWith("0x", StringComparison.InvariantCultureIgnoreCase))
						{
							int ival = 0;
							long lval = 0;

							// Convert to int
							try
							{
								// Convert to value
								ival = System.Convert.ToInt32(val.Substring(2), 16);

								// Add it to struct
								cs.Add(new UniversalEntry(key, ival));
							}
							catch(System.OverflowException)
							{
								// Too large for Int32, try Int64
								try
								{
									// Convert to value
									lval = System.Convert.ToInt64(val.Substring(2), 16);

									// Add it to struct
									cs.Add(new UniversalEntry(key, lval));
								}
								catch(System.OverflowException)
								{
									// Too large for Int64, return error
									RaiseError(line, ERROR_VALUETOOBIG);
								}
								catch(System.FormatException)
								{
									// ERROR: Invalid value in assignment
									RaiseError(line, ERROR_VALUEINVALID);
								}
							}
							catch(System.FormatException)
							{
								// ERROR: Invalid value in assignment
								RaiseError(line, ERROR_VALUEINVALID);
							}
						}
						// Floating point?
						else if(val.IndexOf(".") > -1)
						{
							float fval = 0;
							
							// Convert to float (remove the f first)
							try { fval = System.Convert.ToSingle(val, CultureInfo.InvariantCulture); }
							catch(System.FormatException)
							{ 
								// ERROR: Invalid value in assignment
								RaiseError(line, ERROR_VALUEINVALID);
							}
							
							// Add it to struct
							cs.Add(new UniversalEntry(key, fval));
						}
						else
						{
							int ival = 0;
							long lval = 0;
							
							// Convert to int
							try
							{
								// Convert to value
								ival = System.Convert.ToInt32(val, CultureInfo.InvariantCulture);
								
								// Add it to struct
								cs.Add(new UniversalEntry(key, ival));
							}
							catch(System.OverflowException)
							{
								// Too large for Int32, try Int64
								try
								{
									// Convert to value
									lval = System.Convert.ToInt64(val, CultureInfo.InvariantCulture);
									
									// Add it to struct
									cs.Add(new UniversalEntry(key, lval));
								}
								catch(System.OverflowException)
								{
									// Too large for Int64, return error
									RaiseError(line, ERROR_VALUETOOBIG);
								}
								catch(System.FormatException)
								{ 
									// ERROR: Invalid value in assignment
									RaiseError(line, ERROR_VALUEINVALID);
								}
							}
							catch(System.FormatException)
							{ 
								// ERROR: Invalid value in assignment
								RaiseError(line, ERROR_VALUEINVALID);
							}
						}
						
						// Reset key and value
						keysb.Length = 0;
						valsb.Length = 0;
						
						// End of assignment
						pm = PM_NOTHING;
					}
					// Check for new line
					else if(c == '\n')
					{
						// Count the new line
						line++;
					}
					// Everything else is part of the value
					else
					{
						valsb.Append(char.ToLowerInvariant(c));
					}
				}
				// ================ Parsing a string
				else if(pm == PM_STRING)
				{
					// Check if in an escape sequence
					if(escape)
					{
						// What character?
						switch(c)
						{
							case '\\': valsb.Append('\\'); break;
							case 'n': valsb.Append('\n'); break;

                            case '\"': valsb.Append('\"'); break;
							case 'r': valsb.Append('\r'); break;
							case 't': valsb.Append('\t'); break;
							default:
								
								// Is it a number?
								//if("0123456789".IndexOf(c.ToString(CultureInfo.InvariantCulture)) > -1)
                                if(c >= '0' && c <= '9')
                                {
									int vv = 0;
									char vc = '0';
									
									// Convert the next 3 characters to a number
									string v = data.Substring(pos, 3);
									try { vv = System.Convert.ToInt32(v.Trim(), CultureInfo.InvariantCulture); }
									catch(System.FormatException)
									{ 
										// ERROR: Invalid value in assignment
										RaiseError(line, ERROR_VALUEINVALID);
									}
									
									// Convert the number to a char
									try { vc = System.Convert.ToChar(vv, CultureInfo.InvariantCulture); }
									catch(System.FormatException)
									{ 
										// ERROR: Invalid value in assignment
										RaiseError(line, ERROR_VALUEINVALID);
									}
									
									// Add the char
									valsb.Append(Char.ToLowerInvariant(vc));
								}
								else
								{
                                    // Add the character as it is
                                    valsb.Append(c);
								}
								
								// Leave switch
								break;
						}
						
						// End of escape sequence
						escape = false;
					}
					else
					{
						// Check for sequence start
						if(c == '\\')
						{
							// Next character is of escape sequence
							escape = true;
						}
						// Check if string ends
						else if(c == '\"')
						{
							// Add string to struct
							cs.Add(new UniversalEntry(keysb.ToString().Trim(), valsb.ToString()));
							
							// End of assignment
							pm = PM_ASSIGNMENT;
							
							// Reset key and value
							keysb.Length = 0;
                            valsb.Length = 0;

                        }
						// Check for new line
						else if(c == '\n')
						{
							// Count the new line
							line++;
						}
						// Everything else is just part of string
						else
						{
							// Add to value
							valsb.Append(c);
						}
					}
				}
				// ================ Parsing a keyword
				else if(pm == PM_KEYWORD)
				{
					// Check if keyword ends
					if(c == ';')
					{
						// Add to the struct depending on the keyword
						switch(valsb.ToString().Trim().ToLowerInvariant())
						{
							case "true":
								
								// Add boolean true
								cs.Add(new UniversalEntry(keysb.ToString().Trim(), true));
								break;
								
							case "false":
								
								// Add boolean false
								cs.Add(new UniversalEntry(keysb.ToString().Trim(), false));
								break;
								
							default:
								
								// Unknown keyword
								RaiseError(line, ERROR_KEYWORDUNKNOWN);
								break;
						}
						
						// End of assignment
						pm = PM_NOTHING;
						
						// Reset key and value
						keysb.Length = 0;
                        valsb.Length = 0;
					}
					// Check for new line
					else if(c == '\n')
					{
						// Count the new line
						line++;
					}
					// Everything else is just part of keyword
					else
					{
						// Add to value
						valsb.Append(c);
					}
				}
				
				// Next character
				pos++;
			}
			
			// Return the parsed result
			return cs;
		}
		
		
		// This will create a data structure from the given object
		private string OutputStructure(UniversalCollection cs, int level, string newline, bool whitespace)
		{
			string leveltabs = "";
			string spacing = "";
			StringBuilder db = new StringBuilder("");
			
			// Check if this ConfigStruct is not empty
			if(cs.Count > 0)
			{
				// Create whitespace
				if(whitespace)
				{
					for(int i = 0; i < level; i++) leveltabs += "\t";
					spacing = " ";
				}
				
				// Get enumerator
				IEnumerator<UniversalEntry> de = cs.GetEnumerator();
				
				// Go for each item
				for(int i = 0; i < cs.Count; i++)
				{
					// Go to next item
					de.MoveNext();
					
					// Check if the value if of collection type
					if(de.Current.Value is UniversalCollection)
					{
						UniversalCollection c = (UniversalCollection)de.Current.Value;
						
						// Output recursive structure
						if(whitespace) { db.Append(leveltabs); db.Append(newline); }
						db.Append(leveltabs); db.Append(de.Current.Key);
						if(!string.IsNullOrEmpty(c.Comment))
						{
							if(whitespace) db.Append("\t");
							db.Append("//");
                            db.Append(c.Comment);
                        }
						db.Append(newline);
						db.Append(leveltabs); db.Append("{"); db.Append(newline);
						db.Append(OutputStructure(c, level + 1, newline, whitespace));
						db.Append(leveltabs); db.Append("}"); db.Append(newline);
						if(whitespace) { db.Append(leveltabs); db.Append(newline); }
					}
					// Check if the value is of boolean type
					else if(de.Current.Value is bool)
					{
						// Check value
						if((bool)de.Current.Value == true)
						{
							// Output the keyword "true"
							db.Append(leveltabs); db.Append(de.Current.Key); db.Append(spacing);
							db.Append("="); db.Append(spacing); db.Append("true;"); db.Append(newline);
						}
						else
						{
							// Output the keyword "false"
							db.Append(leveltabs); db.Append(de.Current.Key); db.Append(spacing);
							db.Append("="); db.Append(spacing); db.Append("false;"); db.Append(newline);
						}
					}
					// Check if value is of float type
					else if(de.Current.Value is float)
					{
						// Output the value as float (3 decimals)
						float f = (float)de.Current.Value;
						db.Append(leveltabs); db.Append(de.Current.Key); db.Append(spacing); db.Append("=");
						db.Append(spacing); db.Append(f.ToString("0.000", CultureInfo.InvariantCulture)); db.Append(";"); db.Append(newline);
					}
					// Check if value is of other numeric type
					else if(de.Current.Value.GetType().IsPrimitive)
					{
						// Output the value unquoted
						db.Append(leveltabs); db.Append(de.Current.Key); db.Append(spacing); db.Append("=");
						db.Append(spacing); db.Append(String.Format(CultureInfo.InvariantCulture, "{0}", de.Current.Value)); db.Append(";"); db.Append(newline);
					}
					else
					{
						// Output the value with quotes and escape characters
						db.Append(leveltabs); db.Append(de.Current.Key); db.Append(spacing); db.Append("=");
						db.Append(spacing); db.Append("\""); db.Append(EscapedString(de.Current.Value.ToString())); db.Append("\";"); db.Append(newline);
					}
				}
			}
			
			// Return the structure
			return db.ToString();
		}
		
		#endregion
		
		#region ================== Public Methods
		
		// This clears the last error
		public void ClearError()
		{
			// Clear error
			cpErrorResult = 0;
			cpErrorDescription = "";
			cpErrorLine = 0;
		}
		
		
		// This creates a new configuration
		public void NewConfiguration()
		{
			// Create new configuration
			root = new UniversalCollection();
		}
		
		
		// This will save the current configuration to the specified file
		public bool SaveConfiguration(string filename) { return SaveConfiguration(filename, "\r\n", true); }
		public bool SaveConfiguration(string filename, string newline) { return SaveConfiguration(filename, newline, true); }
		public bool SaveConfiguration(string filename, string newline, bool whitespace)
		{
			// Kill the file if it exists
			if(File.Exists(filename) == true) File.Delete(filename);
			
			// Open file stream for writing
			FileStream fstream = File.OpenWrite(filename);
			
			// Create output structure and write to file
			string data = OutputConfiguration(newline, whitespace);
			byte[] baData= Encoding.ASCII.GetBytes(data);
			fstream.Write(baData, 0, baData.Length);
			fstream.Flush();
			fstream.Close();
			
			// Return true when done, false when errors occurred
			if(cpErrorResult == 0) return true; else return false;
		}
		
		
		// This will output the current configuration as a string
		public string OutputConfiguration() { return OutputConfiguration("\n", false); }
		public string OutputConfiguration(string newline) { return OutputConfiguration(newline, false); }
		public string OutputConfiguration(string newline, bool whitespace)
		{
			// Simply return the configuration structure as string
			return OutputStructure(root, 0, newline, whitespace);
		}
		
		
		// This will load a configuration from file
		public bool LoadConfiguration(string filename)
		{
			// Check if the file is missing
			if(File.Exists(filename) == false)
			{
				throw(new FileNotFoundException("File not found \"" + filename + "\"", filename));
			}
			else
			{
				// Load the file contents
				FileStream fstream = File.OpenRead(filename);
				byte[] fbuffer = new byte[fstream.Length];
				fstream.Read(fbuffer, 0, fbuffer.Length);
				fstream.Close();
				
				// Convert byte array to string
				string data = Encoding.ASCII.GetString(fbuffer);
				
				// Load the configuration from this data
				return InputConfiguration(data);
			}
		}
		
		
		// This will load a configuration from string
		public bool InputConfiguration(string data)
		{
			// Remove returns and tabs because the
			// parser only uses newline for new lines.
			data = data.Replace("\r", "");
			data = data.Replace("\t", "");
			
			// Clear errors
			ClearError();
			
			// Parse the data to the root structure
			int pos = 0;
			int line = 1;
			root = InputStructure(ref data, ref pos, ref line);
			
			// Return true when done, false when errors occurred
			if(cpErrorResult == 0) return true; else return false;
		}
		
		#endregion
	}
}
