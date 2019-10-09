using System;
using System.Globalization;

namespace CodeForDotNet.UI.Input
{
	/// <summary>
	/// Converts virtual keys to strings.
	/// </summary>
	public static class KeyboardKeyConverter
	{
		#region Public Methods

		/// <summary>
		/// Translates a virtual key to an input character when relevant.
		/// </summary>
		/// <param name="key">Key to convert.</param>
		/// <returns>String (normally one character) or null when no mapping, e.g. function key.</returns>
		public static string? ConvertToString(KeyboardKey key)
		{
			return ConvertToString(key, false, CultureInfo.InvariantCulture);
		}

		/// <summary>
		/// Translates a virtual key to an input character when relevant.
		/// </summary>
		/// <param name="key">Key to convert.</param>
		/// <param name="uppercase">True to return uppercase characters, e.g. when shift is also pressed.</param>
		/// <returns>String (normally one character) or null when no mapping, e.g. function key.</returns>
		public static string? ConvertToString(KeyboardKey key, bool uppercase)
		{
			return ConvertToString(key, uppercase, null);
		}

		/// <summary>
		/// Translates a virtual key to an input character when relevant.
		/// </summary>
		/// <param name="key">Key to convert.</param>
		/// <param name="uppercase">True to return uppercase characters, e.g. when shift is also pressed.</param>
		/// <param name="culture">Culture used for the <see cref="KeyboardKey.NumberPadDecimal"/>.</param>
		/// <returns>String (normally one character) or null when no mapping, e.g. function key.</returns>
		public static string? ConvertToString(KeyboardKey key, bool uppercase, CultureInfo? culture)
		{
			switch (key)
			{
				case KeyboardKey.Tab:
					return "\t";

				case KeyboardKey.Space:
					return " ";

				case KeyboardKey.Enter:
					return Environment.NewLine;

				case KeyboardKey.Number0:
				case KeyboardKey.NumberPad0:
					return "0";

				case KeyboardKey.Number1:
				case KeyboardKey.NumberPad1:
					return "1";

				case KeyboardKey.Number2:
				case KeyboardKey.NumberPad2:
					return "2";

				case KeyboardKey.Number3:
				case KeyboardKey.NumberPad3:
					return "3";

				case KeyboardKey.Number4:
				case KeyboardKey.NumberPad4:
					return "4";

				case KeyboardKey.Number5:
				case KeyboardKey.NumberPad5:
					return "5";

				case KeyboardKey.Number6:
				case KeyboardKey.NumberPad6:
					return "6";

				case KeyboardKey.Number7:
				case KeyboardKey.NumberPad7:
					return "7";

				case KeyboardKey.Number8:
				case KeyboardKey.NumberPad8:
					return "8";

				case KeyboardKey.Number9:
				case KeyboardKey.NumberPad9:
					return "9";

				case KeyboardKey.A:
					return uppercase ? "A" : "a";

				case KeyboardKey.B:
					return uppercase ? "B" : "b";

				case KeyboardKey.C:
					return uppercase ? "C" : "c";

				case KeyboardKey.D:
					return uppercase ? "D" : "d";

				case KeyboardKey.E:
					return uppercase ? "E" : "e";

				case KeyboardKey.F:
					return uppercase ? "F" : "f";

				case KeyboardKey.G:
					return uppercase ? "G" : "g";

				case KeyboardKey.H:
					return uppercase ? "H" : "h";

				case KeyboardKey.I:
					return uppercase ? "I" : "i";

				case KeyboardKey.J:
					return uppercase ? "J" : "j";

				case KeyboardKey.K:
					return uppercase ? "K" : "k";

				case KeyboardKey.L:
					return uppercase ? "L" : "l";

				case KeyboardKey.M:
					return uppercase ? "M" : "m";

				case KeyboardKey.N:
					return uppercase ? "N" : "n";

				case KeyboardKey.O:
					return uppercase ? "O" : "o";

				case KeyboardKey.P:
					return uppercase ? "P" : "p";

				case KeyboardKey.Q:
					return uppercase ? "Q" : "q";

				case KeyboardKey.R:
					return uppercase ? "R" : "r";

				case KeyboardKey.S:
					return uppercase ? "S" : "s";

				case KeyboardKey.T:
					return uppercase ? "T" : "t";

				case KeyboardKey.U:
					return uppercase ? "U" : "u";

				case KeyboardKey.V:
					return uppercase ? "V" : "v";

				case KeyboardKey.W:
					return uppercase ? "W" : "w";

				case KeyboardKey.X:
					return uppercase ? "X" : "x";

				case KeyboardKey.Y:
					return uppercase ? "Y" : "y";

				case KeyboardKey.Z:
					return uppercase ? "Z" : "z";

				case KeyboardKey.NumberPadMultiply:
					return "*";

				case KeyboardKey.NumberPadAdd:
				case KeyboardKey.Plus:
					return "+";

				case KeyboardKey.NumberPadSubtract:
				case KeyboardKey.Minus:
					return "-";

				case KeyboardKey.NumberPadDecimal:
					return culture != null
							   ? culture.NumberFormat.NumberDecimalSeparator
							   : CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;

				case KeyboardKey.NumberPadDivide:
					return "/";

				default:

					// Control key or non-visible character
					return null;
			}
		}

		#endregion Public Methods
	}
}
