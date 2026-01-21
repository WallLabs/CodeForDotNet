using System;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Text;

namespace CodeForDotNet.Numerics;

/// <summary>
/// <see cref="BigInteger"/> extensions.
/// </summary>
[SuppressMessage("Microsoft.Naming", "CA1720", Justification = "Use of the word \"signed\" is preferable for an intuitive interface.")]
public static class BigIntegerExtensions
{
    #region Private Fields

    /// <summary>
    /// Valid number digits in order of value starting at zero.
    /// </summary>
    private const string NumberDigits = "0123456789ABCDEF";

    #endregion Private Fields

    #region Public Methods

    /// <summary>
    /// Converts the number to a string of the specified base.
    /// </summary>
    public static string ToString(this BigInteger value, int numberBase)
    {
        return ToString(value, numberBase, true, 0);
    }

    /// <summary>
    /// Converts the number to a string of the specified base.
    /// </summary>
    public static string ToString(this BigInteger value, int numberBase, bool signed)
    {
        return ToString(value, numberBase, signed, 0);
    }

    /// <summary>
    /// Converts the number to a string of the specified base.
    /// </summary>
    public static string ToString(this BigInteger value, int numberBase, bool signed, int minWidth)
    {
        // Validate
        if (numberBase is < 2 or > 16) throw new ArgumentOutOfRangeException(nameof(numberBase));
        ArgumentOutOfRangeException.ThrowIfNegative(minWidth);
        var negative = value < 0;
        if (negative && !signed) throw new ArgumentOutOfRangeException(nameof(value));

        // Decide how to handle sign according to base
        var bitBased = true;
        var bitsPerDigit = 1;
        if (numberBase > 2)
        {
            var baseRoot = Math.Sqrt(numberBase);
            bitsPerDigit = (int)Math.Floor(baseRoot);
            bitBased = !(baseRoot - bitsPerDigit > 0);
        }

        // Detect sign and prepare negative conversion
        var result = new StringBuilder(minWidth);
        var negate = false;
        if (negative)
        {
            // Make value positive for conversion
            value = BigInteger.Abs(value);

            // Make negative
            if (bitBased)
            {
                // Setup two's complement for bit based conversion
                value--;
                negate = true;
            }
            else
            {
                // Just prepend negative sign to other number bases
                _ = result.Append('-');
            }
        }

        // Divide into string of digits...
        var firstDigit = result.Length;
        var quotient = value;
        do
        {
            // Divide quotient by base
            var dividend = BigInteger.Abs(quotient / numberBase);
            var remainder = quotient - (numberBase * dividend);
            quotient = dividend;

            // Use remainder as digit from right to left
            var digit = (int)(negate ? numberBase - 1 - remainder : remainder);
            _ = result.Insert(firstDigit, [NumberDigits[digit]]);

            // Add padding at last position of bit based values when necessary
            if (quotient < 1 && signed && bitBased)
            {
                if (negative)
                {
                    // Ensure negative binary values have at least 2 bits (one for sign)
                    if (bitsPerDigit == 1 && result.Length == 1 && minWidth < 2)
                        minWidth = 2;
                }
                else
                {
                    // Pad with zero when non-negative but MSB set in value
                    var negativeBitValue = 1 << (bitsPerDigit - 1);
                    var negativeBitSet = (negativeBitValue & digit) != 0;
                    if (negativeBitSet)
                    {
                        var positiveWidth = result.Length - firstDigit + 1;
                        if (minWidth < positiveWidth)
                            minWidth = positiveWidth;
                    }
                }
            }
        } while (quotient > 0);

        // Pad string to desired width
        while (result.Length - firstDigit < minWidth)
            _ = result.Insert(firstDigit, [negate ? NumberDigits[numberBase - 1] : NumberDigits[0]]);

        // Return result
        return result.ToString();
    }

    /// <summary>
    /// Converts a string to a number of the specified base.
    /// </summary>
    public static bool TryParse(this string value, int numberBase, out BigInteger result)
    {
        return TryParse(value, numberBase, out result, true);
    }

    /// <summary>
    /// Converts a string to a number of the specified base.
    /// </summary>
    public static bool TryParse(this string value, int numberBase, out BigInteger result, bool signed)
    {
        // Validate
        if (string.IsNullOrEmpty(value)) throw new ArgumentNullException(nameof(value));
        if (numberBase is < 2 or > 16) throw new ArgumentOutOfRangeException(nameof(numberBase));

        // Prepare result
        result = 0;
        BigInteger parsedValue = 0;

        // Decide how to handle sign according to base
        var bitBased = true;
        var bitsPerDigit = 1;
        if (numberBase > 2)
        {
            var baseRoot = Math.Sqrt(numberBase);
            bitsPerDigit = (int)Math.Floor(baseRoot);
            bitBased = !(baseRoot - bitsPerDigit > 0);
        }

        // Detect negative when signed
        var valueLength = value.Length;
        var negative = false;
        var firstDigit = 0;
        if (signed && !bitBased && value[0] == '-')
        {
            negative = true;
            firstDigit = 1;
        }

        // Parse value
        for (var index = firstDigit; index < valueLength; index++)
        {
            var position = valueLength - index - 1 - firstDigit;

            // Get next character value (return false when invalid)
            var indexChar = value[index];
            BigInteger indexValue = NumberDigits.IndexOf(indexChar, StringComparison.OrdinalIgnoreCase);
            if (indexValue < 0)
                return false;

            // Detect sign bit at first character when relevant
            if (index == firstDigit && signed && bitBased)
            {
                var negativeBitValue = 1 << (bitsPerDigit - 1);
                negative = (negativeBitValue & (long)indexValue) != 0;
                if (negative)
                {
                    indexValue -= negativeBitValue;
                    parsedValue = -negativeBitValue * BigInteger.Pow(numberBase, position);
                }
            }

            // Multiply to the power of base value at current position
            if (position > 0)
                indexValue *= BigInteger.Pow(numberBase, position);

            // Add value to result
            if (negative && !bitBased)
                parsedValue -= indexValue;
            else
                parsedValue += indexValue;
        }

        // Return result
        result = parsedValue;
        return true;
    }

    #endregion Public Methods
}
