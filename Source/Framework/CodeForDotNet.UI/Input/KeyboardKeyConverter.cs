using System;
using System.Globalization;

namespace CodeForDotNet.UI.Input;

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
        return key switch {
            KeyboardKey.Tab => "\t",
            KeyboardKey.Space => " ",
            KeyboardKey.Enter => Environment.NewLine,
            KeyboardKey.Number0 or KeyboardKey.NumberPad0 => "0",
            KeyboardKey.Number1 or KeyboardKey.NumberPad1 => "1",
            KeyboardKey.Number2 or KeyboardKey.NumberPad2 => "2",
            KeyboardKey.Number3 or KeyboardKey.NumberPad3 => "3",
            KeyboardKey.Number4 or KeyboardKey.NumberPad4 => "4",
            KeyboardKey.Number5 or KeyboardKey.NumberPad5 => "5",
            KeyboardKey.Number6 or KeyboardKey.NumberPad6 => "6",
            KeyboardKey.Number7 or KeyboardKey.NumberPad7 => "7",
            KeyboardKey.Number8 or KeyboardKey.NumberPad8 => "8",
            KeyboardKey.Number9 or KeyboardKey.NumberPad9 => "9",
            KeyboardKey.A => uppercase ? "A" : "a",
            KeyboardKey.B => uppercase ? "B" : "b",
            KeyboardKey.C => uppercase ? "C" : "c",
            KeyboardKey.D => uppercase ? "D" : "d",
            KeyboardKey.E => uppercase ? "E" : "e",
            KeyboardKey.F => uppercase ? "F" : "f",
            KeyboardKey.G => uppercase ? "G" : "g",
            KeyboardKey.H => uppercase ? "H" : "h",
            KeyboardKey.I => uppercase ? "I" : "i",
            KeyboardKey.J => uppercase ? "J" : "j",
            KeyboardKey.K => uppercase ? "K" : "k",
            KeyboardKey.L => uppercase ? "L" : "l",
            KeyboardKey.M => uppercase ? "M" : "m",
            KeyboardKey.N => uppercase ? "N" : "n",
            KeyboardKey.O => uppercase ? "O" : "o",
            KeyboardKey.P => uppercase ? "P" : "p",
            KeyboardKey.Q => uppercase ? "Q" : "q",
            KeyboardKey.R => uppercase ? "R" : "r",
            KeyboardKey.S => uppercase ? "S" : "s",
            KeyboardKey.T => uppercase ? "T" : "t",
            KeyboardKey.U => uppercase ? "U" : "u",
            KeyboardKey.V => uppercase ? "V" : "v",
            KeyboardKey.W => uppercase ? "W" : "w",
            KeyboardKey.X => uppercase ? "X" : "x",
            KeyboardKey.Y => uppercase ? "Y" : "y",
            KeyboardKey.Z => uppercase ? "Z" : "z",
            KeyboardKey.NumberPadMultiply => "*",
            KeyboardKey.NumberPadAdd or KeyboardKey.Plus => "+",
            KeyboardKey.NumberPadSubtract or KeyboardKey.Minus => "-",
            KeyboardKey.NumberPadDecimal => culture != null
                                           ? culture.NumberFormat.NumberDecimalSeparator
                                           : CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator,
            KeyboardKey.NumberPadDivide => "/",
            _ => null,// Control key or non-visible character
        };
    }

    #endregion Public Methods
}
