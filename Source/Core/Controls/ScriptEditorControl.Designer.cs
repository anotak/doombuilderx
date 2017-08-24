namespace CodeImp.DoomBuilder.Controls
{
	partial class ScriptEditorControl
	{
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if(disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.functionbar = new System.Windows.Forms.ComboBox();
			this.scriptedit = new CodeImp.DoomBuilder.Controls.ScintillaControl();
			this.scriptpanel = new System.Windows.Forms.Panel();
			this.scriptpanel.SuspendLayout();
			this.SuspendLayout();
			// 
			// functionbar
			// 
			this.functionbar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.functionbar.FormattingEnabled = true;
			this.functionbar.Items.AddRange(new object[] {
            "Function1",
            "Function2",
            "Function3"});
			this.functionbar.Location = new System.Drawing.Point(0, 0);
			this.functionbar.Name = "functionbar";
			this.functionbar.Size = new System.Drawing.Size(474, 21);
			this.functionbar.TabIndex = 1;
			this.functionbar.TabStop = false;
			// 
			// scriptedit
			// 
			this.scriptedit.AnchorPosition = 0;
			this.scriptedit.AutoCMaximumHeight = 0;
			this.scriptedit.AutoCMaximumWidth = 0;
			this.scriptedit.AutoCSeparator = 0;
			this.scriptedit.AutoCTypeSeparator = 0;
			this.scriptedit.BackColor = System.Drawing.SystemColors.Window;
			this.scriptedit.CaretFore = 0;
			this.scriptedit.CaretLineBack = 0;
			this.scriptedit.CaretPeriod = 0;
			this.scriptedit.CaretWidth = 0;
			this.scriptedit.CodePage = 0;
			this.scriptedit.ControlCharSymbol = 0;
			this.scriptedit.CurrentPos = 0;
			this.scriptedit.CursorType = 0;
			this.scriptedit.Dock = System.Windows.Forms.DockStyle.Fill;
			this.scriptedit.DocPointer = 0;
			this.scriptedit.EdgeColour = 0;
			this.scriptedit.EdgeColumn = 0;
			this.scriptedit.EdgeMode = 0;
			this.scriptedit.EndAtLastLine = 0;
			this.scriptedit.EndOfLineMode = CodeImp.DoomBuilder.Controls.ScriptEndOfLine.CRLF;
			this.scriptedit.EOLMode = 0;
			this.scriptedit.ExtraAscent = 0;
			this.scriptedit.ExtraDescent = 0;
			this.scriptedit.HighlightGuide = 0;
			this.scriptedit.Indent = 0;
			this.scriptedit.IndentationGuides = 0;
			this.scriptedit.IsAutoCGetAutoHide = false;
			this.scriptedit.IsAutoCGetCancelAtStart = false;
			this.scriptedit.IsAutoCGetChooseSingle = false;
			this.scriptedit.IsAutoCGetDropRestOfWord = false;
			this.scriptedit.IsAutoCGetIgnoreCase = false;
			this.scriptedit.IsBackSpaceUnIndents = false;
			this.scriptedit.IsBufferedDraw = false;
			this.scriptedit.IsCaretLineVisible = false;
			this.scriptedit.IsFocus = false;
			this.scriptedit.IsHScrollBar = false;
			this.scriptedit.IsMouseDownCaptures = false;
			this.scriptedit.IsOvertype = false;
			this.scriptedit.IsReadOnly = false;
			this.scriptedit.IsTabIndents = false;
			this.scriptedit.IsTwoPhaseDraw = false;
			this.scriptedit.IsUndoCollection = false;
			this.scriptedit.IsUsePalette = false;
			this.scriptedit.IsUseTabs = false;
			this.scriptedit.IsViewEOL = false;
			this.scriptedit.IsVScrollBar = false;
			this.scriptedit.LayoutCache = 0;
			this.scriptedit.Lexer = 0;
			this.scriptedit.Location = new System.Drawing.Point(0, 0);
			this.scriptedit.MarginLeft = 0;
			this.scriptedit.MarginRight = 0;
			this.scriptedit.ModEventMask = 0;
			this.scriptedit.MouseDwellTime = 0;
			this.scriptedit.Name = "scriptedit";
			this.scriptedit.PrintColourMode = 0;
			this.scriptedit.PrintMagnification = 0;
			this.scriptedit.PrintWrapMode = 0;
			this.scriptedit.ScrollWidth = 0;
			this.scriptedit.SearchFlags = 0;
			this.scriptedit.SelectionEnd = 0;
			this.scriptedit.SelectionMode = 0;
			this.scriptedit.SelectionStart = 0;
			this.scriptedit.Size = new System.Drawing.Size(470, 377);
			this.scriptedit.Status = 0;
			this.scriptedit.StyleBits = 0;
			this.scriptedit.TabIndex = 0;
			this.scriptedit.TabStop = false;
			this.scriptedit.TabWidth = 0;
			this.scriptedit.TargetEnd = 0;
			this.scriptedit.TargetStart = 0;
			this.scriptedit.ViewWhitespace = CodeImp.DoomBuilder.Controls.ScriptWhiteSpace.Invisible;
			this.scriptedit.ViewWS = 0;
			this.scriptedit.WrapMode = 0;
			this.scriptedit.WrapStartIndent = 0;
			this.scriptedit.WrapVisualFlags = 0;
			this.scriptedit.WrapVisualFlagsLocation = 0;
			this.scriptedit.XOffset = 0;
			this.scriptedit.ZoomLevel = 0;
			this.scriptedit.KeyUp += new System.Windows.Forms.KeyEventHandler(this.scriptedit_KeyUp);
			this.scriptedit.KeyDown += new System.Windows.Forms.KeyEventHandler(this.scriptedit_KeyDown);
			// 
			// scriptpanel
			// 
			this.scriptpanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.scriptpanel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.scriptpanel.Controls.Add(this.scriptedit);
			this.scriptpanel.Location = new System.Drawing.Point(0, 27);
			this.scriptpanel.Name = "scriptpanel";
			this.scriptpanel.Size = new System.Drawing.Size(474, 381);
			this.scriptpanel.TabIndex = 2;
			// 
			// ScriptEditorControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.Controls.Add(this.scriptpanel);
			this.Controls.Add(this.functionbar);
			this.Name = "ScriptEditorControl";
			this.Size = new System.Drawing.Size(474, 408);
			this.scriptpanel.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private ScintillaControl scriptedit;
		private System.Windows.Forms.ComboBox functionbar;
		private System.Windows.Forms.Panel scriptpanel;
	}
}
