
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
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32;
using System.Drawing.Drawing2D;
using CodeImp.DoomBuilder.Config;
using System.IO;
using System.Reflection;
using CodeImp.DoomBuilder.IO;
using System.Collections;
using System.Globalization;
using CodeImp.DoomBuilder.Rendering;
using CodeImp.DoomBuilder.Properties;

#endregion

namespace CodeImp.DoomBuilder.Controls
{
	internal partial class ScriptEditorControl : UserControl
	{
		#region ================== Constants
		
		private const string LEXERS_RESOURCE = "Lexers.cfg";
		private const int DEFAULT_STYLE = (int)ScriptStylesCommon.Default;
		private const int MAX_BACKTRACK_LENGTH = 200;

		// Index for registered images
		private enum ImageIndex : int
		{
			ScriptConstant = 0,
			ScriptKeyword = 1,
			ScriptError = 2
		}
		
		#endregion

		#region ================== Delegates / Events

		public delegate void ExplicitSaveTabDelegate();
		public delegate void OpenScriptBrowserDelegate();
		public delegate void OpenFindReplaceDelegate();
		public delegate void FindNextDelegate();

		public event ExplicitSaveTabDelegate OnExplicitSaveTab;
		public event OpenScriptBrowserDelegate OnOpenScriptBrowser;
		public event OpenFindReplaceDelegate OnOpenFindAndReplace;
		public event FindNextDelegate OnFindNext;

		#endregion

		#region ================== Variables
		
		// Script configuration
		private ScriptConfiguration scriptconfig;
		
		// List of keywords and constants, sorted as uppercase
		private string autocompletestring;

		// Style translation from Scintilla style to ScriptStyleType
		private Dictionary<int, ScriptStyleType> stylelookup;
		
		// Current position information
		private string curfunctionname = "";
		private int curargumentindex = 0;
		private int curfunctionstartpos = 0;
		
		// Status
		private bool changed;
		
		#endregion

		#region ================== Properties

		public bool IsChanged { get { return changed; } set { changed = value; } }
		public int Position { get { return scriptedit.CurrentPos; } set { scriptedit.CurrentPos = value; } }
		public int SelectionStart { get { return scriptedit.SelectionStart; } set { scriptedit.SelectionStart = value; } }
		public int SelectionEnd { get { return scriptedit.SelectionEnd; } set { scriptedit.SelectionEnd = value; } }
		
		#endregion

		#region ================== Contructor / Disposer

		// Constructor
		public ScriptEditorControl()
		{
			// Initialize
			InitializeComponent();
			
			// Script editor properties
			// Unfortunately, these cannot be set using the designer
			// because the control is not really loaded in design mode
			scriptedit.AutoCMaximumHeight = 8;
			scriptedit.AutoCSeparator = ' ';
			scriptedit.AutoCTypeSeparator = '?';
			scriptedit.AutoCSetFillUps("\r\n();[]");	// I should put this in the script configs
			scriptedit.CaretWidth = 2;
			scriptedit.EndAtLastLine = 1;
			scriptedit.EndOfLineMode = ScriptEndOfLine.CRLF;
			scriptedit.IsAutoCGetChooseSingle = true;
			scriptedit.IsAutoCGetIgnoreCase = true;
			scriptedit.IsBackSpaceUnIndents = true;
			scriptedit.IsBufferedDraw = true;
			scriptedit.IsCaretLineVisible = false;
			scriptedit.IsHScrollBar = true;
			scriptedit.IndentationGuides = (int)ScriptIdentGuides.None;
			scriptedit.IsMouseDownCaptures = true;
			scriptedit.IsTabIndents = true;
			scriptedit.IsUndoCollection = true;
			scriptedit.IsUseTabs = true;
			scriptedit.IsViewEOL = false;
			scriptedit.IsVScrollBar = true;
			scriptedit.SetFoldFlags((int)ScriptFoldFlag.Box);
			scriptedit.TabWidth = 4;
			scriptedit.Indent = 4;
			scriptedit.ExtraAscent = 1;
			scriptedit.ExtraDescent = 1;
			scriptedit.CursorType = -1;

			// Symbol margin
			scriptedit.SetMarginTypeN(0, (int)ScriptMarginType.Symbol);
			scriptedit.SetMarginWidthN(0, 20);
			scriptedit.SetMarginMaskN(0, -1);	// all
			
			// Line numbers margin
			scriptedit.SetMarginTypeN(1, (int)ScriptMarginType.Number);
			scriptedit.SetMarginWidthN(1, 40);
			scriptedit.SetMarginMaskN(1, 0);	// none

			// Spacing margin
			scriptedit.SetMarginTypeN(2, (int)ScriptMarginType.Symbol);
			scriptedit.SetMarginWidthN(2, 5);
			scriptedit.SetMarginMaskN(2, 0);	// none
			
			// Setup with default script config
			// Disabled, the form designer doesn't like this
			//SetupStyles(new ScriptConfiguration());

			// Images
			RegisterAutoCompleteImage(ImageIndex.ScriptConstant, Resources.ScriptConstant);
			RegisterAutoCompleteImage(ImageIndex.ScriptKeyword, Resources.ScriptKeyword);
			RegisterMarkerImage(ImageIndex.ScriptError, Resources.ScriptError);

			// Events
			scriptedit.ModEventMask = 0x7FFFF;	// Which events to receive (see also ScriptModificationFlags)
			scriptedit.Modified += new ScintillaControl.ModifiedHandler(scriptedit_Modified);
		}
		
		#endregion
		
		#region ================== Methods
		
		// This launches keyword help website
		public void LaunchKeywordHelp()
		{
			string helpsite = scriptconfig.KeywordHelp;
			string currentword = GetCurrentWord();
			if(!string.IsNullOrEmpty(currentword) && (currentword.Length > 1) && !string.IsNullOrEmpty(helpsite))
			{
				currentword = scriptconfig.GetKeywordCase(currentword);
				helpsite = helpsite.Replace("%K", currentword);
				General.OpenWebsite(helpsite);
			}
		}
		
		// This replaces the selection with the given text
		public void ReplaceSelection(string replacement)
		{
			Encoding encoder = Encoding.GetEncoding(scriptedit.CodePage);
			string text = encoder.GetString(GetText());
			int selectionstart = scriptedit.SelectionStart;
			
			// Make new text
			StringBuilder newtext = new StringBuilder(text.Length + replacement.Length);
			newtext.Append(text.Substring(0, scriptedit.SelectionStart));
			newtext.Append(replacement);
			newtext.Append(text.Substring(scriptedit.SelectionEnd));
			
			SetText(encoder.GetBytes(newtext.ToString()));
			
			// Adjust selection
			scriptedit.SelectionStart = selectionstart;
			scriptedit.SelectionEnd = selectionstart + replacement.Length;
		}
		
		// This moves the caret to a given line and ensures the line is visible
		public void MoveToLine(int linenumber)
		{
			scriptedit.GotoLine(linenumber);
			EnsureLineVisible(linenumber);
		}

		// This makes sure a line is visible
		public void EnsureLineVisible(int linenumber)
		{
			scriptedit.EnsureVisibleEnforcePolicy(linenumber);
		}

		// This returns the line for a position
		public int LineFromPosition(int position)
		{
			return scriptedit.LineFromPosition(position);
		}
		
		// This clears all marks
		public void ClearMarks()
		{
			scriptedit.MarkerDeleteAll((int)ImageIndex.ScriptError);
		}

		// This adds a mark on the given line
		public void AddMark(int linenumber)
		{
			scriptedit.MarkerAdd(linenumber, (int)ImageIndex.ScriptError);
		}

		// This refreshes the style setup
		public void RefreshStyle()
		{
			// Re-setup with the same config
			SetupStyles(scriptconfig);
		}

		// This sets up the script editor with a script configuration
		public void SetupStyles(ScriptConfiguration config)
		{
			Stream lexersdata;
			StreamReader lexersreader;
			Configuration lexercfg = new Configuration();
			SortedList<string, string> autocompletelist;
			string[] resnames;
			int imageindex;
			
			// Make collections
			stylelookup = new Dictionary<int, ScriptStyleType>();
			autocompletelist = new SortedList<string, string>(StringComparer.Ordinal);
			
			// Keep script configuration
			if(scriptconfig != config) scriptconfig = config;
			
			// Find a resource named Lexers.cfg
			resnames = General.ThisAssembly.GetManifestResourceNames();
			foreach(string rn in resnames)
			{
				// Found one?
				if(rn.EndsWith(LEXERS_RESOURCE, StringComparison.InvariantCultureIgnoreCase))
				{
					// Get a stream from the resource
					lexersdata = General.ThisAssembly.GetManifestResourceStream(rn);
					lexersreader = new StreamReader(lexersdata, Encoding.ASCII);

					// Load configuration from stream
					lexercfg.InputConfiguration(lexersreader.ReadToEnd());

					// Done with the resource
					lexersreader.Dispose();
					lexersdata.Dispose();
				}
			}
			
			// Check if specified lexer exists and set the lexer to use
			string lexername = "lexer" + scriptconfig.Lexer.ToString(CultureInfo.InvariantCulture);
			if(!lexercfg.SettingExists(lexername)) throw new InvalidOperationException("Unknown lexer " + scriptconfig.Lexer + " specified in script configuration!");
			scriptedit.Lexer = scriptconfig.Lexer;
			
			// Set the default style and settings
			scriptedit.StyleSetFont(DEFAULT_STYLE, General.Settings.ScriptFontName);
			scriptedit.StyleSetSize(DEFAULT_STYLE, General.Settings.ScriptFontSize);
			scriptedit.StyleSetBold(DEFAULT_STYLE, General.Settings.ScriptFontBold);
			scriptedit.StyleSetItalic(DEFAULT_STYLE, false);
			scriptedit.StyleSetUnderline(DEFAULT_STYLE, false);
			scriptedit.StyleSetCase(DEFAULT_STYLE, ScriptCaseVisible.Mixed);
			scriptedit.StyleSetFore(DEFAULT_STYLE, General.Colors.PlainText.ToColorRef());
			scriptedit.StyleSetBack(DEFAULT_STYLE, General.Colors.ScriptBackground.ToColorRef());
			scriptedit.CaretPeriod = SystemInformation.CaretBlinkTime;
			scriptedit.CaretFore = General.Colors.ScriptBackground.Inverse().ToColorRef();
			scriptedit.StyleBits = 7;
			
			// These don't work?
			scriptedit.TabWidth = General.Settings.ScriptTabWidth;
			scriptedit.IsUseTabs = false;
			scriptedit.IsTabIndents = true;
			scriptedit.Indent = General.Settings.ScriptTabWidth;
			scriptedit.IsBackSpaceUnIndents = true;
			
			// This applies the default style to all styles
			scriptedit.StyleClearAll();

			// Set the code page to use
			scriptedit.CodePage = scriptconfig.CodePage;

			// Set the default to something normal (this is used by the autocomplete list)
			scriptedit.StyleSetFont(DEFAULT_STYLE, this.Font.Name);
			scriptedit.StyleSetBold(DEFAULT_STYLE, this.Font.Bold);
			scriptedit.StyleSetItalic(DEFAULT_STYLE, this.Font.Italic);
			scriptedit.StyleSetUnderline(DEFAULT_STYLE, this.Font.Underline);
			scriptedit.StyleSetSize(DEFAULT_STYLE, (int)Math.Round(this.Font.SizeInPoints));
			
			// Set style for linenumbers and margins
			scriptedit.StyleSetBack((int)ScriptStylesCommon.LineNumber, General.Colors.ScriptBackground.ToColorRef());
			
			// Clear all keywords
			for(int i = 0; i < 9; i++) scriptedit.KeyWords(i, null);
			
			// Now go for all elements in the lexer configuration
			// We are looking for the numeric keys, because these are the
			// style index to set and the value is our ScriptStyleType
			IDictionary dic = lexercfg.ReadSetting(lexername, new Hashtable());
			foreach(DictionaryEntry de in dic)
			{
				// Check if this is a numeric key
				int stylenum = -1;
				if(int.TryParse(de.Key.ToString(), out stylenum))
				{
					// Add style to lookup table
					stylelookup.Add(stylenum, (ScriptStyleType)(int)de.Value);
					
					// Apply color to style
					int colorindex = 0;
					switch((ScriptStyleType)(int)de.Value)
					{
						case ScriptStyleType.PlainText: colorindex = ColorCollection.PLAINTEXT; break;
						case ScriptStyleType.Comment: colorindex = ColorCollection.COMMENTS; break;
						case ScriptStyleType.Constant: colorindex = ColorCollection.CONSTANTS; break;
						case ScriptStyleType.Keyword: colorindex = ColorCollection.KEYWORDS; break;
						case ScriptStyleType.LineNumber: colorindex = ColorCollection.LINENUMBERS; break;
						case ScriptStyleType.Literal: colorindex = ColorCollection.LITERALS; break;
						default: colorindex = ColorCollection.PLAINTEXT; break;
					}
					scriptedit.StyleSetFore(stylenum, General.Colors.Colors[colorindex].ToColorRef());
				}
			}
			
			// Create the keywords list and apply it
			imageindex = (int)ImageIndex.ScriptKeyword;
			int keywordsindex = lexercfg.ReadSetting(lexername + ".keywordsindex", -1);
			if(keywordsindex > -1)
			{
				StringBuilder keywordslist = new StringBuilder("");
				foreach(string k in scriptconfig.Keywords)
				{
					if(keywordslist.Length > 0) keywordslist.Append(" ");
					keywordslist.Append(k);
					autocompletelist.Add(k.ToUpperInvariant(), k + "?" + imageindex.ToString(CultureInfo.InvariantCulture));
				}
				string words = keywordslist.ToString();
				if(scriptconfig.CaseSensitive)
					scriptedit.KeyWords(keywordsindex, words);
				else
					scriptedit.KeyWords(keywordsindex, words.ToLowerInvariant());
			}

			// Create the constants list and apply it
			imageindex = (int)ImageIndex.ScriptConstant;
			int constantsindex = lexercfg.ReadSetting(lexername + ".constantsindex", -1);
			if(constantsindex > -1)
			{
				StringBuilder constantslist = new StringBuilder("");
				foreach(string c in scriptconfig.Constants)
				{
					if(constantslist.Length > 0) constantslist.Append(" ");
					constantslist.Append(c);
					autocompletelist.Add(c.ToUpperInvariant(), c + "?" + imageindex.ToString(CultureInfo.InvariantCulture));
				}
				string words = constantslist.ToString();
				if(scriptconfig.CaseSensitive)
					scriptedit.KeyWords(constantsindex, words);
				else
					scriptedit.KeyWords(constantsindex, words.ToLowerInvariant());
			}
			
			// Sort the autocomplete list
			List<string> autocompleteplainlist = new List<string>(autocompletelist.Values);
			autocompletestring = string.Join(" ", autocompleteplainlist.ToArray());
			
			// Show/hide the functions bar
			functionbar.Visible = (scriptconfig.FunctionRegEx.Length > 0);

			// Rearrange the layout
			scriptedit.ClearDocumentStyle();
			scriptedit.SetText(scriptedit.GetText(scriptedit.TextSize));
			this.PerformLayout();
		}
		
		
		// This returns the current word (where the caret is at)
		public string GetCurrentWord()
		{
			return GetWordAt(scriptedit.CurrentPos);
		}


		// This returns the word at the given position
		public string GetWordAt(int position)
		{
			int wordstart = scriptedit.WordStartPosition(position, true);
			int wordend = scriptedit.WordEndPosition(position, true);

			// Decode the text
			byte[] scripttextdata = scriptedit.GetText(scriptedit.TextSize);
			Encoding encoder = Encoding.GetEncoding(scriptedit.CodePage);
			string scripttext = encoder.GetString(scripttextdata);

			if(wordstart < wordend)
				return scripttext.Substring(wordstart, wordend - wordstart);
			else
				return "";
		}
		
		
		// This returns the ScriptStyleType for a given Scintilla style
		private ScriptStyleType GetScriptStyle(int scintillastyle)
		{
			if(stylelookup.ContainsKey(scintillastyle))
				return stylelookup[scintillastyle];
			else
				return ScriptStyleType.PlainText;
		}
		
		
		// This gathers information about the current caret position
		private void UpdatePositionInfo()
		{
			int bracketlevel = 0;			// bracket level counting
			int argindex = 0;				// function argument counting
			int limitpos;					// lowest position we'll backtrack to
			int pos = scriptedit.CurrentPos;
			
			// Decode the text
			byte[] scripttextdata = scriptedit.GetText(scriptedit.TextSize);
			Encoding encoder = Encoding.GetEncoding(scriptedit.CodePage);
			string scripttext = encoder.GetString(scripttextdata);
			
			// Reset position info
			curfunctionname = "";
			curargumentindex = 0;
			curfunctionstartpos = 0;
			
			// Determine lowest backtrack position
			limitpos = scriptedit.CurrentPos - MAX_BACKTRACK_LENGTH;
			if(limitpos < 0) limitpos = 0;
			
			// We can only do this when we have function syntax information
			if((scriptconfig.ArgumentDelimiter.Length == 0) || (scriptconfig.FunctionClose.Length == 0) ||
			   (scriptconfig.FunctionOpen.Length == 0) || (scriptconfig.Terminator.Length == 0)) return;
			
			// Get int versions of the function syntax informantion
			int argumentdelimiter = scriptconfig.ArgumentDelimiter[0];
			int functionclose = scriptconfig.FunctionClose[0];
			int functionopen = scriptconfig.FunctionOpen[0];
			int terminator = scriptconfig.Terminator[0];
			
			// Continue backtracking until we reached the limitpos
			while(pos >= limitpos)
			{
				// Backtrack 1 character
				pos--;
				
				// Get the style and character at this position
				ScriptStyleType curstyle = GetScriptStyle(scriptedit.StyleAt(pos));
				int curchar = scriptedit.CharAt(pos);
				
				// Then meeting ) then increase bracket level
				// When meeting ( then decrease bracket level
				// When bracket level goes -1, then the next word should be the function name
				// Only when at bracket level 0, count the comma's for argument index
				
				// TODO:
				// Original code checked for scope character here and breaks if found
				
				// Check if in plain text or keyword
				if((curstyle == ScriptStyleType.PlainText) || (curstyle == ScriptStyleType.Keyword))
				{
					// Closing bracket
					if(curchar == functionclose)
					{
						bracketlevel++;
					}
					// Opening bracket
					else if(curchar == functionopen)
					{
						bracketlevel--;
						
						// Out of the brackets?
						if(bracketlevel < 0)
						{
							// Skip any whitespace before this bracket
							do
							{
								// Backtrack 1 character
								curchar = scriptedit.CharAt(--pos);
							}
							while((pos >= limitpos) && ((curchar == ' ') || (curchar == '\t') ||
														(curchar == '\r') || (curchar == '\n')));
							
							// NOTE: We may need to set onlyWordCharacters argument in the
							// following calls to false to get any argument delimiter included,
							// but this may also cause a valid keyword to be combined with other
							// surrounding characters that do not belong to the keyword.
							
							// Find the word before this bracket
							int wordstart = scriptedit.WordStartPosition(pos, true);
							int wordend = scriptedit.WordEndPosition(pos, true);
							string word = scripttext.Substring(wordstart, wordend - wordstart);
							if(word.Length > 0)
							{
								// Check if this is an argument delimiter
								// I can't remember why I did this, but I'll probably stumble
								// upon the problem if this doesn't work right (see note above)
								if(word[0] == argumentdelimiter)
								{
									// We are now in the parent function
									bracketlevel++;
									argindex = 0;
								}
								// Now check if this is a keyword
								else if(scriptconfig.IsKeyword(word))
								{
									// Found it!
									curfunctionname = scriptconfig.GetKeywordCase(word);
									curargumentindex = argindex;
									curfunctionstartpos = wordstart;
									break;
								}
								else
								{
									// Don't know this word
									break;
								}
							}
						}
					}
					// Argument delimiter
					else if(curchar == argumentdelimiter)
					{
						// Only count these at brackt level 0
						if(bracketlevel == 0) argindex++;
					}
					// Terminator
					else if(curchar == terminator)
					{
						// Can't find anything, break now
						break;
					}
				}
			}
		}
		
		// This clears all undo levels
		public void ClearUndoRedo()
		{
			scriptedit.EmptyUndoBuffer();
		}
		
		// This registers an XPM image for the autocomplete list
		private unsafe void RegisterAutoCompleteImage(ImageIndex index, byte[] imagedata)
		{
			// Convert to string
			string bigstring = Encoding.UTF8.GetString(imagedata);
			
			// Register image
			scriptedit.RegisterImage((int)index, bigstring);
		}

		// This registers an XPM image for the markes list
		private unsafe void RegisterMarkerImage(ImageIndex index, byte[] imagedata)
		{
			// Convert to string
			string bigstring = Encoding.UTF8.GetString(imagedata);

			// Register image
			scriptedit.MarkerDefinePixmap((int)index, bigstring);
		}

		// Perform undo
		public void Undo()
		{
			scriptedit.Undo();
		}

		// Perform redo
		public void Redo()
		{
			scriptedit.Redo();
		}

		// Perform cut
		public void Cut()
		{
			scriptedit.Cut();
		}

		// Perform copy
		public void Copy()
		{
			scriptedit.Copy();
		}

		// Perform paste
		public void Paste()
		{
			scriptedit.Paste();
		}
		
		// This steals the focus (use with care!)
		public void GrabFocus()
		{
			scriptedit.GrabFocus();
		}

		public byte[] GetText()
		{
			return scriptedit.GetText(scriptedit.TextSize);
		}

		public void SetText(byte[] text)
		{
			scriptedit.SetText(text);
		}

		#endregion
		
		#region ================== Events
		
		// Layout needs to be re-organized
		protected override void OnLayout(LayoutEventArgs e)
		{
			base.OnLayout(e);

			// With or without functions bar?
			if(functionbar.Visible)
			{
				scriptpanel.Top = functionbar.Bottom + 6;
				scriptpanel.Height = this.ClientSize.Height - scriptpanel.Top;
			}
			else
			{
				scriptpanel.Top = 0;
				scriptpanel.Height = this.ClientSize.Height;
			}
		}

		// Script modified
		private void scriptedit_Modified(ScintillaControl pSender, int position, int modificationType, string text, int length, int linesAdded, int line, int foldLevelNow, int foldLevelPrev)
		{
			changed = true;
		}
		
		// Key pressed down
		private void scriptedit_KeyDown(object sender, KeyEventArgs e)
		{
			// These key combinations put odd characters in the script, so I disabled them
			if((e.KeyCode == Keys.Q) && ((e.Modifiers & Keys.Control) == Keys.Control)) e.Handled = true;
			if((e.KeyCode == Keys.W) && ((e.Modifiers & Keys.Control) == Keys.Control)) e.Handled = true;
			if((e.KeyCode == Keys.E) && ((e.Modifiers & Keys.Control) == Keys.Control)) e.Handled = true;
			if((e.KeyCode == Keys.R) && ((e.Modifiers & Keys.Control) == Keys.Control)) e.Handled = true;
			if((e.KeyCode == Keys.Y) && ((e.Modifiers & Keys.Control) == Keys.Control)) e.Handled = true;
			if((e.KeyCode == Keys.U) && ((e.Modifiers & Keys.Control) == Keys.Control)) e.Handled = true;
			if((e.KeyCode == Keys.I) && ((e.Modifiers & Keys.Control) == Keys.Control)) e.Handled = true;
			if((e.KeyCode == Keys.P) && ((e.Modifiers & Keys.Control) == Keys.Control)) e.Handled = true;
			if((e.KeyCode == Keys.A) && ((e.Modifiers & Keys.Control) == Keys.Control) && ((e.Modifiers & Keys.Shift) == Keys.Shift)) e.Handled = true;
			if((e.KeyCode == Keys.D) && ((e.Modifiers & Keys.Control) == Keys.Control)) e.Handled = true;
			if((e.KeyCode == Keys.G) && ((e.Modifiers & Keys.Control) == Keys.Control)) e.Handled = true;
			if((e.KeyCode == Keys.H) && ((e.Modifiers & Keys.Control) == Keys.Control)) e.Handled = true;
			if((e.KeyCode == Keys.J) && ((e.Modifiers & Keys.Control) == Keys.Control)) e.Handled = true;
			if((e.KeyCode == Keys.K) && ((e.Modifiers & Keys.Control) == Keys.Control)) e.Handled = true;
			if((e.KeyCode == Keys.L) && ((e.Modifiers & Keys.Control) == Keys.Control)) e.Handled = true;
			if((e.KeyCode == Keys.Z) && ((e.Modifiers & Keys.Control) == Keys.Control) && ((e.Modifiers & Keys.Shift) == Keys.Shift)) e.Handled = true;
			if((e.KeyCode == Keys.X) && ((e.Modifiers & Keys.Control) == Keys.Control) && ((e.Modifiers & Keys.Shift) == Keys.Shift)) e.Handled = true;
			if((e.KeyCode == Keys.C) && ((e.Modifiers & Keys.Control) == Keys.Control) && ((e.Modifiers & Keys.Shift) == Keys.Shift)) e.Handled = true;
			if((e.KeyCode == Keys.V) && ((e.Modifiers & Keys.Control) == Keys.Control) && ((e.Modifiers & Keys.Shift) == Keys.Shift)) e.Handled = true;
			if((e.KeyCode == Keys.B) && ((e.Modifiers & Keys.Control) == Keys.Control)) e.Handled = true;
			if((e.KeyCode == Keys.N) && ((e.Modifiers & Keys.Control) == Keys.Control)) e.Handled = true;
			if((e.KeyCode == Keys.M) && ((e.Modifiers & Keys.Control) == Keys.Control)) e.Handled = true;

			// F3 for Find Next
			if((e.KeyCode == Keys.F3) && (e.Modifiers == Keys.None))
			{
				if(OnFindNext != null) OnFindNext();
				e.Handled = true;
			}

			// F2 for Keyword Help
			if((e.KeyCode == Keys.F2) && (e.Modifiers == Keys.None))
			{
				LaunchKeywordHelp();
				e.Handled = true;
			}

			// CTRL+F for find & replace
			if((e.KeyCode == Keys.F) && ((e.Modifiers & Keys.Control) == Keys.Control))
			{
				if(OnOpenFindAndReplace != null) OnOpenFindAndReplace();
				e.Handled = true;
			}

			// CTRL+S for save
			if((e.KeyCode == Keys.S) && ((e.Modifiers & Keys.Control) == Keys.Control))
			{
				if(OnExplicitSaveTab != null) OnExplicitSaveTab();
				e.Handled = true;
			}

			// CTRL+O for open
			if((e.KeyCode == Keys.O) && ((e.Modifiers & Keys.Control) == Keys.Control))
			{
				if(OnOpenScriptBrowser != null) OnOpenScriptBrowser();
				e.Handled = true;
			}

			// CTRL+Space to autocomplete
			if((e.KeyCode == Keys.Space) && (e.Modifiers == Keys.Control))
			{
				// Hide call tip if any
				scriptedit.CallTipCancel();
				
				// Show autocomplete
				int currentpos = scriptedit.CurrentPos;
				int wordstartpos = scriptedit.WordStartPosition(currentpos, true);
				scriptedit.AutoCShow(currentpos - wordstartpos, autocompletestring);
				
				e.Handled = true;
			}
		}
		
		// Key released
		private void scriptedit_KeyUp(object sender, KeyEventArgs e)
		{
			bool showcalltip = false;
			int highlightstart = 0;
			int highlightend = 0;
			
			// Enter pressed?
			if((e.KeyCode == Keys.Enter) && (e.Modifiers == Keys.None))
			{
				// Do we want auto-indent?
				if(General.Settings.ScriptAutoIndent)
				{
					// Get the current line index and check if its not the first line
					int curline = scriptedit.LineFromPosition(scriptedit.CurrentPos);
					if(curline > 0)
					{
						// Apply identation of the previous line to this line
						int ident = scriptedit.GetLineIndentation(curline - 1);
						int tabs = ident ;// / scriptedit.Indent;
						if(scriptedit.GetLineIndentation(curline) == 0)
						{
							scriptedit.SetLineIndentation(curline, ident);
							scriptedit.SetSel(scriptedit.SelectionStart + tabs, scriptedit.SelectionStart + tabs);
						}
					}
				}
			}
			
			UpdatePositionInfo();
			
			// Call tip shown
			if(scriptedit.IsCallTipActive)
			{
				// Should we hide the call tip?
				if(curfunctionname.Length == 0)
				{
					// Hide the call tip
					scriptedit.CallTipCancel();
				}
				else
				{
					// Update the call tip
					showcalltip = true;
				}
			}
			// No call tip
			else
			{
				// Should we show a call tip?
				showcalltip = (curfunctionname.Length > 0) && !scriptedit.IsAutoCActive;
			}
			
			// Show or update call tip
			if(showcalltip)
			{
				string functiondef = scriptconfig.GetFunctionDefinition(curfunctionname);
				if(functiondef != null)
				{
					// Determine the range to highlight
					int argsopenpos = functiondef.IndexOf(scriptconfig.FunctionOpen);
					int argsclosepos = functiondef.LastIndexOf(scriptconfig.FunctionClose);
					if((argsopenpos > -1) && (argsclosepos > -1))
					{
						string argsstr = functiondef.Substring(argsopenpos + 1, argsclosepos - argsopenpos - 1);
						string[] args = argsstr.Split(scriptconfig.ArgumentDelimiter[0]);
						if((curargumentindex >= 0) && (curargumentindex < args.Length))
						{
							int argoffset = 0;
							for(int i = 0; i < curargumentindex; i++) argoffset += args[i].Length + 1;
							highlightstart = argsopenpos + argoffset + 1;
							highlightend = highlightstart + args[curargumentindex].Length;
						}
					}
					
					// Show tip
					scriptedit.CallTipShow(curfunctionstartpos, functiondef);
					scriptedit.CallTipSetHlt(highlightstart, highlightend);
				}
			}
		}
		
		#endregion
	}
}
