
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
using System.Globalization;
using System.Windows.Forms;
using CodeImp.DoomBuilder.Actions;
using CodeImp.DoomBuilder.Geometry;
using CodeImp.DoomBuilder.Rendering;
using CodeImp.DoomBuilder.Editing;

#endregion

namespace CodeImp.DoomBuilder.Controls
{
	public class NumericTextbox : AutoSelectTextbox
	{
		#region ================== Constants

		#endregion

		#region ================== Variables

		private bool allownegative = false;		// Allow negative numbers
		private bool allowrelative = false;		// Allow ++ and -- prefix for relative changes
		private bool allowdecimal = false;		// Allow decimal (float) numbers
		private bool controlpressed = false;
		
		#endregion

		#region ================== Properties

		public bool AllowNegative { get { return allownegative; } set { allownegative = value; } }
		public bool AllowRelative { get { return allowrelative; } set { allowrelative = value; } }
		public bool AllowDecimal { get { return allowdecimal; } set { allowdecimal = value; } }

		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		public NumericTextbox()
		{
			this.ImeMode = ImeMode.Off;
		}

		#endregion

		#region ================== Methods

		// Key pressed
		protected override void OnKeyDown(KeyEventArgs e)
		{
			controlpressed = e.Control;
			base.OnKeyDown(e);
		}

		// Key released
		protected override void OnKeyUp(KeyEventArgs e)
		{
			controlpressed = e.Control;
			base.OnKeyUp(e);
		}
		
		// When a key is pressed
		protected override void OnKeyPress(KeyPressEventArgs e)
		{
			string allowedchars = "0123456789\b";
			string nonselectedtext;
			string textpart;
			int selectionpos;
			int numprefixes;
			char otherprefix;
			
			// Determine allowed chars
			if(allownegative) allowedchars += CultureInfo.CurrentUICulture.NumberFormat.NegativeSign;
			if(allowrelative) allowedchars += "+-";
			if(controlpressed) allowedchars += "\u0018\u0003\u0016";
			if(allowdecimal) allowedchars += CultureInfo.CurrentUICulture.NumberFormat.CurrencyDecimalSeparator;
			
			// Check if key is not allowed
			if(allowedchars.IndexOf(e.KeyChar) == -1)
			{
				// Cancel this
				e.Handled = true;
			}
			else
			{
				// Check if + or - is pressed
				if((e.KeyChar == '+') || (e.KeyChar == '-'))
				{
					// Determine non-selected text
					if(this.SelectionLength > 0)
					{
						nonselectedtext = this.Text.Substring(0, this.SelectionStart) +
							this.Text.Substring(this.SelectionStart + this.SelectionLength);
					}
					else if(this.SelectionLength < 0)
					{
						nonselectedtext = this.Text.Substring(0, this.SelectionStart + this.SelectionLength) +
							this.Text.Substring(this.SelectionStart);
					}
					else
					{
						nonselectedtext = this.Text;
					}
					
					// Not at the start?
					selectionpos = this.SelectionStart - 1;
					if(this.SelectionLength < 0) selectionpos = (this.SelectionStart + this.SelectionLength) - 1;
					if(selectionpos > -1)
					{
						// Find any other characters before the insert position
						textpart = this.Text.Substring(0, selectionpos + 1);
						textpart = textpart.Replace("+", "");
						textpart = textpart.Replace("-", "");
						if(textpart.Length > 0)
						{
							// Cancel this
							e.Handled = true;
						}
					}

					// Determine other prefix
					if(e.KeyChar == '+') otherprefix = '-'; else otherprefix = '+';
					
					// Limit the number of + and - allowed
					numprefixes = nonselectedtext.Split(e.KeyChar, otherprefix).Length;
					if(numprefixes > 2)
					{
						// Can't have more than 2 prefixes
						e.Handled = true;
					}
					else if(numprefixes > 1)
					{
						// Must have 2 the same prefixes
						if(this.Text.IndexOf(e.KeyChar) == -1) e.Handled = true;

						// Double prefix must be allowed
						if(!allowrelative) e.Handled = true;
					}
				}
			}
			
			// Call base
			base.OnKeyPress(e);
		}

		// Validate contents
		protected override void OnValidating(CancelEventArgs e)
		{
			string textpart = this.Text;

			// Strip prefixes
			textpart = textpart.Replace("+", "");
			if(!allownegative) textpart = textpart.Replace("-", "");
			
			// No numbers left?
			if(textpart.Length == 0)
			{
				// Make the textbox empty
				this.Text = "";
			}
			
			// Call base
			base.OnValidating(e);
		}
		
		// This checks if the number is relative
		public bool CheckIsRelative()
		{
			// Prefixed with ++ or --?
			return (this.Text.StartsWith("++") || this.Text.StartsWith("--"));
		}
		
		// This determines the result value
		public int GetResult(int original)
		{
			string textpart = this.Text;
			int result;
			
			// Strip prefixes
			textpart = textpart.Replace("+", "");
			textpart = textpart.Replace("-", "");
			
			// Any numbers left?
			if(textpart.Length > 0)
			{
				// Prefixed with ++?
				if(this.Text.StartsWith("++"))
				{
					// Add number to original
					if(!int.TryParse(textpart, out result)) result = 0;
					return original + result;
				}
				// Prefixed with --?
				else if(this.Text.StartsWith("--"))
				{
					// Subtract number from original
					if(!int.TryParse(textpart, out result)) result = 0;
					int newvalue = original - result;
					if(!allownegative && (newvalue < 0)) newvalue = 0;
					return newvalue;
				}
				else
				{
					// Return the new value
					return int.TryParse(this.Text, out result) ? result : original;
				}
			}
			else
			{
				// Nothing given, keep original value
				return original;
			}
		}

		// This determines the result value
		public float GetResultFloat(float original)
		{
			string textpart = this.Text;
			float result;

			// Strip prefixes
			textpart = textpart.Replace("+", "");
			textpart = textpart.Replace("-", "");

			// Any numbers left?
			if(textpart.Length > 0)
			{
				// Prefixed with ++?
				if(this.Text.StartsWith("++"))
				{
					// Add number to original
					if(!float.TryParse(textpart, out result)) result = 0;
					return original + result;
				}
				// Prefixed with --?
				else if(this.Text.StartsWith("--"))
				{
					// Subtract number from original
					if(!float.TryParse(textpart, out result)) result = 0;
					float newvalue = original - result;
					if(!allownegative && (newvalue < 0)) newvalue = 0;
					return newvalue;
				}
				else
				{
					// Return the new value
					return float.TryParse(this.Text, out result) ? result : original;
				}
			}
			else
			{
				// Nothing given, keep original value
				return original;
			}
		}
		
		#endregion
	}
}
