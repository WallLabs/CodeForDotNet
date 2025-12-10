using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using CodeForDotNet.Numerics;
using CodeForDotNet.UI.Input;
using Windows.UI.Xaml;

namespace CodeForDotNet.WindowsUniversal.UI.Controls;

/// <summary>
/// Text box with masking and key filters to make it easier for the user to edit numbers. Supports multiple bases, e.g. decimal, hexadecimal, binary.
/// </summary>
public partial class NumericTextBox : DynamicTextBox
{
    #region Public Fields

    /// <summary>
    /// <see cref="NumberBase"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty NumberBaseProperty =
        DependencyProperty.Register("NumberBase", typeof(NumericTextBoxNumberBase), typeof(NumericTextBox),
                                    new PropertyMetadata(NumericTextBoxNumberBase.Decimal));

    /// <summary>
    /// <see cref="NumberMax"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty NumberMaxProperty =
        DependencyProperty.Register("MaxSignedValue", typeof(double), typeof(NumericTextBox),
                                    new PropertyMetadata(double.MaxValue));

    /// <summary>
    /// <see cref="NumberMin"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty NumberMinProperty =
        DependencyProperty.Register("MinSignedValue", typeof(double), typeof(NumericTextBox),
                                    new PropertyMetadata(double.MinValue));

    /// <summary>
    /// <see cref="NumberSigned"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty NumberSignedProperty =
        DependencyProperty.Register("NumberSigned", typeof(bool), typeof(NumericTextBox),
                                    new PropertyMetadata(true));

    #endregion Public Fields

    #region Public Constructors

    /// <summary>
    /// Creates an instance with default values.
    /// </summary>
    public NumericTextBox()
    {
        MinLength = 1;
        PadChar = '0';
    }

    #endregion Public Constructors

    #region Public Properties

    /// <summary>
    /// Sets the number base edited with this text box.
    /// </summary>
    public NumericTextBoxNumberBase NumberBase
    {
        get => (NumericTextBoxNumberBase)GetValue(NumberBaseProperty);
        set => SetValue(NumberBaseProperty, value);
    }

    /// <summary>
    /// Maximum value.
    /// </summary>
    public double NumberMax
    {
        get => (double)GetValue(NumberMaxProperty);
        set => SetValue(NumberMaxProperty, value);
    }

    /// <summary>
    /// Minimum value.
    /// </summary>
    public double NumberMin
    {
        get => (double)GetValue(NumberMinProperty);
        set => SetValue(NumberMinProperty, value);
    }

    /// <summary>
    /// Enables editing signed values. Only applicable to decimal.
    /// </summary>
    public bool NumberSigned
    {
        get => (bool)GetValue(NumberSignedProperty);
        set => SetValue(NumberSignedProperty, value);
    }

    #endregion Public Properties

    #region Protected Methods

    /// <summary>
    /// Filters invalid keys.
    /// </summary>
    [SuppressMessage("Naming", "CA1725:Parameter names should match base declaration", Justification = "Readability.")]
    protected override void OnBeforeKeyDown(global::Windows.UI.Xaml.Input.KeyRoutedEventArgs args)
    {
        // Validate.
        ArgumentNullException.ThrowIfNull(args);

        // Call base class method to process cursor keys, do nothing more when handled
        base.OnBeforeKeyDown(args);
        if (args.Handled)
        {
            return;
        }

        var key = (KeyboardKey)args.Key;

        // Allow minus to switch sign when signed
        if ((key == KeyboardKey.NumberPadSubtract || key == KeyboardKey.Minus) && NumberSigned)
        {
            Negate();
            args.Handled = true;
            return;
        }

        // Filter keys according to current number settings
        if (!IsKeyValidNumber(key, (int)NumberBase))
        {
            args.Handled = true;
        }
    }

    /// <summary>
    /// Enforces selection rules based on current/maximum length and trying over to next control in a group.
    /// </summary>
    [SuppressMessage("Naming", "CA1725:Parameter names should match base declaration", Justification = "Readability.")]
    protected override void OnBeforeSelectionChanged(object sender, DynamicTextSelectionChangedEventArgs args)
    {
        // Validate.
        ArgumentNullException.ThrowIfNull(args);

        // Never select the sign
        bool? sign = null;
        var text = Text;
        if (NumberBase == NumericTextBoxNumberBase.Decimal)
        {
            if (text.StartsWith("+", StringComparison.OrdinalIgnoreCase))
            {
                sign = true;
            }
            else if (text.StartsWith("-", StringComparison.OrdinalIgnoreCase))
            {
                sign = false;
            }
        }
        if (sign.HasValue && args.SelectionStart == 0)
        {
            args.SelectionStart = 1;
        }

        // Apply base class rules
        base.OnBeforeSelectionChanged(sender, args);
    }

    /// <summary>
    /// Strips non-numeric characters according to <see cref="NumberBase"/> before processing text changes.
    /// </summary>
    [SuppressMessage("Naming", "CA1725:Parameter names should match base declaration", Justification = "Readability.")]
    protected override void OnBeforeTextChanged(object sender, DynamicTextChangedEventArgs args)
    {
        // Validate.
        ArgumentNullException.ThrowIfNull(args);

        // Apply decimal sign rules
        var originalText = args.OriginalText;
        var textValue = args.Text;
        bool? sign = null;
        if (NumberBase == NumericTextBoxNumberBase.Decimal && NumberSigned)
        {
            // Detect sign
            if (textValue.StartsWith("+", StringComparison.OrdinalIgnoreCase))
            {
                sign = true;
            }
            else if (textValue.StartsWith("-", StringComparison.OrdinalIgnoreCase))
            {
                sign = false;
            }

            // Apply sign rules
            if (sign.HasValue)
            {
                // Remove sign when only remaining character
                if (textValue.Length == 1)
                {
                    sign = null;
                }

                // Prevent selection of sign itself
                else if (SelectionStart == 0)
                {
                    SelectionStart = 1;
                }
            }
        }

        // Strip invalid characters.
        var validChars = GetValidNumberChars((int)NumberBase);
        var text = "";
        foreach (var textChar in args.Text)
        {
            // Allow sign as first character.
            if (sign.HasValue && text.Length == 0 && (textChar == '+' || textChar == '-'))
            {
                text += textChar;
            }
            else
            {
                // Allow only valid number characters for rest of string.
                if (validChars.Contains(textChar.ToString(CultureInfo.InvariantCulture), StringComparison.OrdinalIgnoreCase))
                {
                    // Add valid char with uppercase correction if necessary.
                    if (char.IsLetter(textChar) && !char.IsUpper(textChar))
                    {
                        // Correct lower to uppercase.
                        text += char.ToUpperInvariant(textChar);
                    }
                    else
                    {
                        // No change necessary.
                        text += textChar;
                    }
                }
                else
                {
                    // Skip or pad invalid characters.
                    if (PadChar.HasValue)
                    {
                        text += PadChar.Value;
                    }
                }
            }
        }

        // Validate number when set (format and sign, minimum and maximum)
        if (!string.IsNullOrEmpty(text))
        {
            var numberBase = (int)NumberBase;
            if (!Number.TryParse(text, numberBase, out var value) || value < NumberMin || value > NumberMax)
            {
                // Discard changes when invalid
                text = originalText;
            }
        }

        // Call base class to trim or pad remaining text
        args.Text = text;
        base.OnBeforeTextChanged(sender, args);
    }

    #endregion Protected Methods

    #region Private Methods

    /// <summary>
    /// Gets the valid characters for a number base system.
    /// </summary>
    private static string GetValidNumberChars(int numberBase)
    {
        var allowedChars = numberBase switch {
            2 => "01",
            10 => "0123456789",
            16 => "0123456789ABCDEF",
            _ => throw new ArgumentOutOfRangeException(nameof(numberBase)),
        };
        return allowedChars;
    }

    /// <summary>
    /// Tests if a key is a valid number entry for the specified base.
    /// </summary>
    private static bool IsKeyValidNumber(KeyboardKey key, int numberBase)
    {
        switch (key)
        {
            // Allow binary keys for base 2+
            case KeyboardKey.Number0:
            case KeyboardKey.Number1:
            case KeyboardKey.NumberPad0:
            case KeyboardKey.NumberPad1:
                if (numberBase < 2)
                {
                    return false;
                }

                break;

            // Allow decimal keys for base 10+
            case KeyboardKey.Number2:
            case KeyboardKey.Number3:
            case KeyboardKey.Number4:
            case KeyboardKey.Number5:
            case KeyboardKey.Number6:
            case KeyboardKey.Number7:
            case KeyboardKey.Number8:
            case KeyboardKey.Number9:
            case KeyboardKey.NumberPad2:
            case KeyboardKey.NumberPad3:
            case KeyboardKey.NumberPad4:
            case KeyboardKey.NumberPad5:
            case KeyboardKey.NumberPad6:
            case KeyboardKey.NumberPad7:
            case KeyboardKey.NumberPad8:
            case KeyboardKey.NumberPad9:
                if (numberBase < 10)
                {
                    return false;
                }

                break;

            // Allow hex keys for base 16
            case KeyboardKey.A:
            case KeyboardKey.B:
            case KeyboardKey.C:
            case KeyboardKey.D:
            case KeyboardKey.E:
            case KeyboardKey.F:
                if (numberBase < 16)
                {
                    return false;
                }

                break;

            // All other keys invalid
            default:
                return false;
        }
        return true;
    }

    /// <summary>
    /// Gets the current numeric value.
    /// </summary>
    private Number GetValue()
    {
        // Return zero when empty
        if (string.IsNullOrWhiteSpace(Text))
        {
            return Number.Zero;
        }

        // Parse and return value
        _ = Number.TryParse(Text, (int)NumberBase, out var value, NumberSigned);
        return value;
    }

    /// <summary>
    /// Negates the current value.
    /// </summary>
    private void Negate()
    {
        // Validate
        if (!NumberSigned)
        {
            throw new InvalidOperationException();
        }

        // Get value
        var value = GetValue();

        // Negate
        value = -value;

        // Set text to negated value
        Text = value.ToString((int)NumberBase, MinLength);
    }

    #endregion Private Methods
}
