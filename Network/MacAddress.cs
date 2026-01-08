using CodeForDotNet;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace CodeChief.Net
{
    /// <summary>
    /// Media Access Control (MAC) address. A 48bit unique identifier for LAN stations, e.g. a network adapter or router.
    /// See IEEE RFC2469 (http://tools.ietf.org/html/rfc2469) for specifications.
    /// </summary>
    public struct MacAddress : IFormattable, IComparable, IComparable<MacAddress>, IEquatable<MacAddress>
    {
        #region Constants

        /// <summary>
        /// Size of a <see cref="MacAddress"/> in bytes.
        /// </summary>
        public const int Size = 6;

        /// <summary>
        /// Broadcast MAC address.
        /// </summary>
        public const long Broadcast = 0xFFFFFFFFFFFF;

        /// <summary>
        /// Regular expression which matches any valid string value.
        /// </summary>
        public const string StringValueExpression =
            @"^(([0-9a-fA-F]{12})|" +
            @"(([0-9a-fA-F]{2}:){5}[0-9a-fA-F]{2})|" +
            @"(([0-9a-fA-F]{2}-){5}[0-9a-fA-F]{2})|" +
            @"(([0-9a-fA-F]{2}\s){5}[0-9a-fA-F]{2})|" +
            @"(([0-9a-fA-F]{4}:){2}[0-9a-fA-F]{4})|" +
            @"(([0-9a-fA-F]{4}-){2}[0-9a-fA-F]{4})|" +
            @"(([0-9a-fA-F]{4}\s){2}[0-9a-fA-F]{4}))$";

        /// <summary>
        /// Regular expression which matches any valid string format name.
        /// </summary>
        public const string StringFormatExpression = @"^[NnCcHhBb]{1}[24]{0,1}$";

        #endregion

        #region Lifetime

        /// <summary>
        /// The constructor expecting an <see cref="long"/> represented MAC address.
        /// </summary>
        public MacAddress(long mac)
        {
            _value = mac;
        }

        /// <summary>
        /// The constructor expecting an 12 character hexadecimal string representing a MAC address. The bytes or words might be separated by ':' or '-'.
        /// </summary>
        public MacAddress(string mac)
        {
            long? value = TryParseInt64(mac, true);
            Debug.Assert(value != null);
            _value = value.Value;
        }

        /// <summary>
        /// Teh constructor expecting an <see cref="byte"/>[6] represented MAC address.
        /// </summary>
        public MacAddress(byte[] mac)
        {
            // Validate
            if (mac == null || mac.Length != 6)
                throw new ArgumentOutOfRangeException(nameof(mac));

            // Reverse and resize array without changing source
            byte[]? valueBytes = new byte[8];
            for (int sourceIndex = mac.Length - 1, targetIndex = 0; sourceIndex >= 0; sourceIndex--, targetIndex++)
                valueBytes[targetIndex] = mac[sourceIndex];

            // Convert to internal 64bit representation
            _value = BitConverter.ToInt64(valueBytes, 0);
        }

        #endregion

        #region Operators

        /// <summary>
        /// Tests two objects of this type for equality by value.
        /// </summary>
        public static bool operator ==(MacAddress mac1, MacAddress mac2)
        {
            return mac1.Equals(mac2);
        }

        /// <summary>
        /// Tests two objects of this type for inequality by value.
        /// </summary>
        public static bool operator !=(MacAddress mac1, MacAddress mac2)
        {
            return !mac1.Equals(mac2);
        }

        /// <summary>
        /// Test two objects of this type if the left value is lower than the right value.
        /// </summary>
        public static bool operator <(MacAddress mac1, MacAddress mac2)
        {
            return mac1.CompareTo(mac2) < 0;
        }

        /// <summary>
        /// Test two objects of this type if the left value is greater than the right value.
        /// </summary>
        public static bool operator >(MacAddress mac1, MacAddress mac2)
        {
            return mac1.CompareTo(mac2) > 0;
        }

        #endregion

        #region Private Fields

        /// <summary>
        /// The internal representation of the MAC address.
        /// </summary>
        readonly long _value;

        #endregion

        #region Public Properties

        /// <summary>
        /// Returns the MAC address in the default string format for serialization.
        /// </summary>
        [XmlText]
        public string ValueString
        {
            get
            {
                return ToString();
            }
        }

        /// <summary>
        /// Indicates if the address is currently empty (all zeros).
        /// </summary>
        [XmlIgnore]
        public bool IsEmpty { get { return _value == 0; } }

        #endregion

        #region Public Methods

        /// <summary>
        /// <para>Returns the MAC address as a string in the default numeric format "N", e.g. 0123456789AB.</para>
        /// </summary>
        public override string ToString()
        {
            return ToString(null, null);
        }

        /// <summary>
        /// <para>Returns the MAC address as a string formatted like specified in <paramref name="format"/>.</para>
        /// </summary>
        /// <param name="format">
        /// <para>N: The numeric format like 0123456789AB. (default)</para>
        /// <para>n: The numeric format like 0123456789AB in Bit-reversed order.</para>
        /// <para>C[2|4]: The numeric format but the bytes(2), words(4) are separated by colons like 01:23:45:67:89:AB or 0123:4567:89AB.</para>
        /// <para>c[2|4]: The numeric format but the bytes(2), words(4) are separated by colons like 01:23:45:67:89:AB or 0123:4567:89AB in Bit-reversed order.</para>
        /// <para>H[2|4]: The numeric format but the bytes(2), words(4) are separated by hyphens like 01-23-45-67-89-AB or 0123-4567-89AB.</para>
        /// <para>h[2|4]: The numeric format but the bytes(2), words(4) are separated by hyphens like 01-23-45-67-89-AB or 0123-4567-89AB in Bit-reversed order.</para>
        /// <para>B[2|4]: The numeric format but the bytes(2), words(4) are separated by spaces like 01 23 45 67 89 AB or 0123 4567 89AB.</para>
        /// <para>b[2|4]: The numeric format but the bytes(2), words(4) are separated by spaces like 01 23 45 67 89 AB or 0123 4567 89AB in Bit-reversed order.</para>
        /// </param>
        public string ToString(string format)
        {
            return ToString(format, null);
        }

        /// <summary>
        /// <para>Returns the MAC address as a string formatted like specified in <paramref name="format"/>.</para>
        /// </summary>
        /// <param name="format">
        /// <para>N: The numeric format like 0123456789AB. (default)</para>
        /// <para>n: The numeric format like 0123456789AB in Bit-reversed order.</para>
        /// <para>C[2|4]: The numeric format but the bytes(2), words(4) are separated by colons like 01:23:45:67:89:AB or 0123:4567:89AB.</para>
        /// <para>c[2|4]: The numeric format but the bytes(2), words(4) are separated by colons like 01:23:45:67:89:AB or 0123:4567:89AB in Bit-reversed order.</para>
        /// <para>H[2|4]: The numeric format but the bytes(2), words(4) are separated by hyphens like 01-23-45-67-89-AB or 0123-4567-89AB.</para>
        /// <para>h[2|4]: The numeric format but the bytes(2), words(4) are separated by hyphens like 01-23-45-67-89-AB or 0123-4567-89AB in Bit-reversed order.</para>
        /// <para>B[2|4]: The numeric format but the bytes(2), words(4) are separated by spaces like 01 23 45 67 89 AB or 0123 4567 89AB.</para>
        /// <para>b[2|4]: The numeric format but the bytes(2), words(4) are separated by spaces like 01 23 45 67 89 AB or 0123 4567 89AB in Bit-reversed order.</para>
        /// </param>
        /// <param name="formatProvider">Not used, required by IFormattable. MAC strings are always invariant culture.</param>
        public string ToString(string? format, IFormatProvider? formatProvider)
        {
            // Set default format when null
            if (string.IsNullOrEmpty(format))
                format = "N";

            // Validate format
            if (!Regex.IsMatch(format, StringFormatExpression))
                throw new ArgumentOutOfRangeException(nameof(format));

            // Get format style
            char style = format.ToUpperInvariant()[0];
            bool reverse = char.IsLower(style);
            int group = format.Length == 2 ? int.Parse(format[1].ToString(CultureInfo.InvariantCulture), CultureInfo.InvariantCulture) : 0;

            // Get byte value to format, in reversed order when lowercase style
            byte[]? bytes = ToByteArray(reverse);

            // Decide which separator to use (if any)
            string? separator = "";
            switch (style)
            {
                case 'C':
                    separator = ":";
                    break;

                case 'H':
                    separator = "-";
                    break;

                case 'B':
                    separator = " ";
                    break;
            }

            // Format string based on specified style
            return group switch
            {
                0 => string.Format(CultureInfo.InvariantCulture, "{0:X2}{1:X2}{2:X2}{3:X2}{4:X2}{5:X2}",
                                       bytes[0], bytes[1], bytes[2], bytes[3], bytes[4], bytes[5]),// Format without separators
                2 => string.Format(CultureInfo.InvariantCulture, "{0:X2}{6}{1:X2}{6}{2:X2}{6}{3:X2}{6}{4:X2}{6}{5:X2}",
                                        bytes[0], bytes[1], bytes[2], bytes[3], bytes[4], bytes[5], separator),// Format with separators at every byte
                4 => string.Format(CultureInfo.InvariantCulture, "{0:X2}{1:X2}{6}{2:X2}{3:X2}{6}{4:X2}{5:X2}",
                                        bytes[0], bytes[1], bytes[2], bytes[3], bytes[4], bytes[5], separator),// Format with separators at every 2nd byte
                _ => throw new InvalidOperationException(),// Future proof
            };
        }

        /// <summary>
        /// Converts a <see cref="string"/> into a MacAddress.
        /// </summary>
        public static MacAddress Parse(string mac)
        {
            long? value = TryParseInt64(mac, true);
            Debug.Assert(value != null);
            return new MacAddress(value.Value);
        }

        /// <summary>
        /// Tries to convert a <see cref="string"/> into a MacAddress.
        /// </summary>
        /// <returns>True when conversion was possible, otherwise false.</returns>
        public static bool TryParse(string mac, out MacAddress macAddress)
        {
            long? macAsInt64 = TryParseInt64(mac, false);

            if (macAsInt64.HasValue)
            {
                macAddress = new MacAddress(macAsInt64.Value);
                return true;
            }

            macAddress = new MacAddress();
            return false;
        }

        /// <summary>
        /// Compares the current object <see cref="MacAddress"/>, <see cref="long"/> or <see cref="byte"/>[].
        /// </summary>
        public int CompareTo(object other)
        {
            if (other is MacAddress address)
                return CompareTo(address.ToInt64());

            if (other is long longValue)
                return ToInt64().CompareTo(longValue);

            if (other is byte[] bytes)
                return CompareTo(new MacAddress(bytes));

            throw new NotSupportedException();
        }

        /// <summary>
        /// Compares the current object with another object of the same type.
        /// </summary>
        public int CompareTo(MacAddress other)
        {
            return ToInt64().CompareTo(other.ToInt64());
        }

        /// <summary>
        /// Returns a value that indicates whether this instance is equal to a specified object.
        /// </summary>
        public override bool Equals(object other)
        {
            if (other is MacAddress address)
                return Equals(address);

            if (other is long longValue)
                return ToInt64().Equals(longValue);

            if (other is byte[] bytes)
                return Equals(new MacAddress(bytes));

            return false;
        }

        /// <summary>
        /// Returns a value that indicates whether this instance is equal to a specified object.
        /// </summary>
        public bool Equals(MacAddress other)
        {
            return ToInt64().Equals(other.ToInt64());
        }

        /// <summary>
        /// Returns a hash code based on the current value of this object.
        /// </summary>
        public override int GetHashCode()
        {
            return HashCode.Combine(_value);
        }

        /// <summary>
        /// Returns the MAC address as an <see cref="long"/>.
        /// </summary>
        public long ToInt64()
        {
            return _value;
        }

        /// <summary>
        /// Returns the MAC address as a 6 byte array in transmission order (MSB first).
        /// </summary>
        public byte[] ToByteArray()
        {
            return ToByteArray(false);
        }

        /// <summary>
        /// Returns the MAC address as a 6 byte array in transmission order (MSB first), optionally in bit-reversed order.
        /// </summary>
        /// <param name="reversed">Optionally true to reverse all bits (not bytes). Default is false.</param>
        public byte[] ToByteArray(bool reversed)
        {
            // Get bytes for MAC address (48 bits = 6 in total)
            byte[]? bytes = BitConverter.GetBytes(_value);
            Array.Resize(ref bytes, 6);
            Array.Reverse(bytes);

            // Reverse bit order (not bytes) when specified
            if (reversed)
            {
                for (int i = 0; i < bytes.Length; i++)
                    bytes[i] = bytes[i].Reverse();
            }

            // Return result
            return bytes;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Tries to parse a long from a MAC address string.
        /// Returns null or throws an exception when the value has an invalid format,
        /// depending on the <paramref name="throwExceptions"/> parameter.
        /// </summary>
        private static long? TryParseInt64(string mac, bool throwExceptions)
        {
            // Validate
            if (string.IsNullOrEmpty(mac))
                if (throwExceptions)
                    throw new ArgumentNullException(nameof(mac));
                else
                    return null;

            // Check format
            if (!Regex.IsMatch(mac, StringValueExpression))
                if (throwExceptions)
                    throw new FormatException();
                else
                    return null;

            // Remove separators
            mac = mac
                .Replace(":", "", StringComparison.OrdinalIgnoreCase)
                .Replace("-", "", StringComparison.OrdinalIgnoreCase)
                .Replace(" ", "", StringComparison.OrdinalIgnoreCase);

            // Try to parse MAC as number
            if (!long.TryParse(mac, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out long result))
                if (throwExceptions)
                    throw new FormatException();
                else
                    return null;

            // Return parsed result when sucessfull
            return result;
        }

        /// <summary>
        /// Comparison operator for less than or equal to.
        /// </summary>
        public static bool operator <=(MacAddress left, MacAddress right)
        {
            return left.CompareTo(right) <= 0;
        }

        /// <summary>
        /// Comparison operator for greater or equal to.
        /// </summary>
        public static bool operator >=(MacAddress left, MacAddress right)
        {
            return left.CompareTo(right) >= 0;
        }

        #endregion
    }
}
