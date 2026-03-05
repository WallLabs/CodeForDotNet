using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace CodeForDotNet.Xml;

/// <summary>
/// Extended <see cref="DateTimeOffset"/> to better support XML serialization to XML standard
/// date and times which include offset and a specific string format as default.
/// </summary>
/// <remarks>
/// <para>
/// Use at the public/serialized property level in place of <see cref="DateTimeOffset"/>. This
/// structure fully implements assignment and comparison operators so can be used
/// interchangeably with the original type. Occasionally you may find it necessary to cast
/// variables to either type in order to avoid compiler ambiguity.
/// </para>
/// <para>For background information why this is necessary and not fixed by Microsoft, <see href="https://connect.microsoft.com/VisualStudio/feedback/details/288349/datetimeoffset-is-not-serialized-by-a-xmlserializer"/>.</para>
/// </remarks>
[Serializable]
[TypeConverter(typeof(XmlDateTimeConverter))]
public struct XmlDateTime :
    IXmlSerializable, IComparable, IComparable<DateTimeOffset>,
    IEquatable<XmlDateTime>, IEquatable<DateTimeOffset>, IConvertible,
    IFormattable
{
    /// <summary>
    /// ISO standard string format which preserves the entire value (supports round-tripping).
    /// </summary>
    private const string StringFormat = "yyyy-MM-ddTHH:mm:ss.FFFFFFFK";

    /// <summary>
    /// Creates an instance based on an existing value.
    /// </summary>
    public XmlDateTime(DateTimeOffset value)
        : this()
    {
        Value = value;
    }

    /// <summary>
    /// Creates an instance based on an existing value.
    /// </summary>
    public XmlDateTime(DateTime value)
        : this()
    {
        Value = new DateTimeOffset(value);
    }

    /// <summary>
    /// Underlying <see cref="DateTimeOffset"/> value.
    /// </summary>
    public DateTimeOffset Value { get; private set; }

    /// <summary>
    /// Assignment operator overload from <see cref="DateTime"/>.
    /// </summary>
    public static implicit operator XmlDateTime(DateTime value)
    {
        return new XmlDateTime(value);
    }

    /// <summary>
    /// Assignment operator overload from <see cref="DateTimeOffset"/>.
    /// </summary>
    public static implicit operator XmlDateTime(DateTimeOffset value)
    {
        return new XmlDateTime(value);
    }

    /// <summary>
    /// Tests two instances of this type for inequality by value.
    /// </summary>
    public static bool operator !=(XmlDateTime left, XmlDateTime right)
    {
        return left.Value != right.Value;
    }

    /// <summary>
    /// Tests if an instance of this type is less than another by value.
    /// </summary>
    public static bool operator <(XmlDateTime left, XmlDateTime right)
    {
        return left.Value < right.Value;
    }

    /// <summary>
    /// Tests if an instance of this type is less than or equal to another by value.
    /// </summary>
    public static bool operator <=(XmlDateTime left, XmlDateTime right)
    {
        return left.Value <= right.Value;
    }

    /// <summary>
    /// Tests two instances of this type for equality by value.
    /// </summary>
    public static bool operator ==(XmlDateTime left, XmlDateTime right)
    {
        return left.Value == right.Value;
    }

    /// <summary>
    /// Tests if an instance of this type is greater than another by value.
    /// </summary>
    public static bool operator >(XmlDateTime left, XmlDateTime right)
    {
        return left.Value > right.Value;
    }

    /// <summary>
    /// Tests if an instance of this type is greater than or equal to another by value.
    /// </summary>
    public static bool operator >=(XmlDateTime left, XmlDateTime right)
    {
        return left.Value >= right.Value;
    }

    /// <summary>
    /// Converts to a value of this type from a string.
    /// </summary>
    /// <param name="input">String value to convert.</param>
    /// <returns>Converted value.</returns>
    public static XmlDateTime Parse(string input)
    {
        // Call standard method
        return new XmlDateTime(XmlConvert.ToDateTimeOffset(input));
    }

    /// <summary>
    /// Converts to a value of this type from a string using a specific style.
    /// </summary>
    /// <param name="input">String value to convert.</param>
    /// <param name="formatProvider">Format settings.</param>
    /// <returns>Converted value.</returns>
    public static XmlDateTime Parse(string input, IFormatProvider? formatProvider)
    {
        // Call overloaded method
        return Parse(input, formatProvider, DateTimeStyles.RoundtripKind);
    }

    /// <summary>
    /// Converts to a value of this type from a string using a specific culture and style.
    /// </summary>
    /// <param name="input">String value to convert.</param>
    /// <param name="formatProvider">Format settings.</param>
    /// <param name="styles">Supported format styles.</param>
    /// <returns>Converted value.</returns>
    public static XmlDateTime Parse(string input, IFormatProvider? formatProvider, DateTimeStyles styles)
    {
        // Call contained method
        return new XmlDateTime(DateTimeOffset.Parse(input, formatProvider, styles));
    }

    /// <summary>
    /// Attempts conversion to a value of this type from a string.
    /// </summary>
    /// <param name="input">String value to convert.</param>
    /// <param name="result">Resulting value when successful.</param>
    /// <returns>True when successful.</returns>
    public static bool TryParse(string input, out XmlDateTime result)
    {
        // Call overloaded method
        return TryParse(input, CultureInfo.CurrentCulture, DateTimeStyles.RoundtripKind, out result);
    }

    /// <summary>
    /// Attempts conversion to a value of this type from a string using a specific culture and style.
    /// </summary>
    /// <param name="input">String value to convert.</param>
    /// <param name="formatProvider">Format settings.</param>
    /// <param name="styles">Supported format styles.</param>
    /// <param name="result">Resulting value when successful.</param>
    /// <returns>True when successful.</returns>
    public static bool TryParse(string input, IFormatProvider? formatProvider,
        DateTimeStyles styles, out XmlDateTime result)
    {
        // Call overloaded method
        return TryParseExact(input, [StringFormat, "O", "R"], formatProvider, styles, out result);
    }

    /// <summary>
    /// Attempts conversion to a value of this type from a string using a specific format,
    /// culture and style.
    /// </summary>
    /// <param name="input">String value to convert.</param>
    /// <param name="format">Format string.</param>
    /// <param name="formatProvider">Format settings.</param>
    /// <param name="styles">Supported format styles.</param>
    /// <param name="result">Resulting value when successful.</param>
    /// <returns>True when successful.</returns>
    public static bool TryParseExact(string input, string format, IFormatProvider? formatProvider,
        DateTimeStyles styles, out XmlDateTime result)
    {
        // Call overloaded method
        return TryParseExact(input, [format], formatProvider, styles, out result);
    }

    /// <summary>
    /// Attempts conversion to a value of this type from a string using a specific formats,
    /// culture and styles.
    /// </summary>
    /// <param name="input">String value to convert.</param>
    /// <param name="formats">Supported format strings.</param>
    /// <param name="formatProvider">Format settings.</param>
    /// <param name="styles">Supported format styles.</param>
    /// <param name="result">Resulting value when successful.</param>
    /// <returns>True when successful.</returns>
    public static bool TryParseExact(string input, string[] formats, IFormatProvider? formatProvider,
        DateTimeStyles styles, out XmlDateTime result)
    {
        // Parse using original method
        if (DateTimeOffset.TryParseExact(input, formats, formatProvider, styles, out var value))
        {
            // Return valid value
            result = new XmlDateTime(value);
            return true;
        }

        // Return invalid
        result = new XmlDateTime(DateTimeOffset.MinValue);
        return false;
    }

    /// <summary>
    /// Creates an instance from a <see cref="DateTimeOffset"/>.
    /// </summary>
    public static XmlDateTime FromDateTimeOffset(DateTimeOffset value)
    {
        return new XmlDateTime(value);
    }

    /// <summary>
    /// Creates an instance from a <see cref="DateTime"/>.
    /// </summary>
    public static XmlDateTime FromDateTime(DateTime value)
    {
        return new XmlDateTime(value);
    }

    /// <summary>
    /// Compares the current instance with another object of the same type and returns an
    /// integer that indicates whether the current instance precedes, follows, or occurs in the
    /// same position in the sort order as the other object.
    /// </summary>
    /// <returns>
    /// A value that indicates the relative order of the objects being compared. The return
    /// value has these meanings: Value Meaning Less than zero This instance precedes
    /// <paramref name="other"/> in the sort order. Zero This instance occurs in the same
    /// position in the sort order as <paramref name="other"/>. Greater than zero This instance
    /// follows <paramref name="other"/> in the sort order.
    /// </returns>
    /// <param name="other">An object to compare with this instance.</param>
    /// <exception cref="ArgumentException">
    /// <paramref name="other"/> is not the same type as this instance.
    /// </exception>
    public readonly int CompareTo(object? other)
    {
        // Call overloaded method when compatible type
        if (other is XmlDateTime or DateTimeOffset)
        {
            return Compare(this, (XmlDateTime)other);
        }

        // Return less than when not same type
        return -1;
    }

    /// <summary>
    /// Compares this value with another of the same type.
    /// </summary>
    public readonly int CompareTo(XmlDateTime other)
    {
        return Compare(this, other);
    }

    /// <summary>
    /// Compares this value with a <see cref="DateTimeOffset"/>.
    /// </summary>
    public readonly int CompareTo(DateTimeOffset other)
    {
        return Value.CompareTo(other);
    }

    /// <summary>
    /// Indicates whether this instance and a specified object are equal.
    /// </summary>
    /// <param name="other">The object to compare with the current instance.</param>
    /// <returns>
    /// true if <paramref name="other"/> and this instance are the same type and represent the
    /// same value; otherwise, false.
    /// </returns>
    [SuppressMessage("Naming", "CA1725:Parameter names should match base declaration", Justification = "Readability.")]
    public override readonly bool Equals(object? other)
    {
        // Call overloaded methods for supported types.
        if (other is XmlDateTime xml)
            return Equals(xml.Value);
        if (other is DateTimeOffset offset)
            return Equals(offset);
        if (other is DateTime dateTime)
            return Equals(dateTime);

        // Unsupported type is never the same.
        return false;
    }

    /// <summary>
    /// Compares this value to another of the same type.
    /// </summary>
    public readonly bool Equals(XmlDateTime other)
    {
        return Value.Equals(other.Value);
    }

    /// <summary>
    /// Compares this value to a <see cref="DateTimeOffset"/>.
    /// </summary>
    public readonly bool Equals(DateTimeOffset other)
    {
        return Value.Equals(other);
    }

    /// <summary>
    /// Compares this value to a <see cref="DateTime"/>.
    /// </summary>
    public readonly bool Equals(DateTime other)
    {
        return Value.UtcDateTime.Equals(other.ToUniversalTime());
    }

    /// <summary>
    /// Returns the hash code for this instance.
    /// </summary>
    /// <returns>A 32-bit signed integer that is the hash code for this instance.</returns>
    public override readonly int GetHashCode()
    {
        return Value.GetHashCode();
    }

    /// <summary>
    /// This method is reserved and should not be used. When implementing the IXmlSerializable
    /// interface, you should return null (Nothing in Visual Basic) from this method, and
    /// instead, if specifying a custom schema is required, apply the
    /// <see cref="XmlSchemaProviderAttribute"/> to the class.
    /// </summary>
    /// <returns>
    /// An <see cref="XmlSchema"/> that describes the XML representation of the object that is
    /// produced by the <see cref="IXmlSerializable.WriteXml(XmlWriter)"/> method and consumed
    /// by the <see cref="IXmlSerializable.ReadXml(XmlReader)"/> method.
    /// </returns>
    public readonly XmlSchema? GetSchema()
    {
        return null;
    }

    /// <summary>
    /// Generates an object from its XML representation.
    /// </summary>
    /// <param name="reader">The <see cref="XmlReader"/> stream from which the object is deserialized.</param>
    public void ReadXml(XmlReader reader)
    {
        // Validate.
        ArgumentNullException.ThrowIfNull(reader);

        // Read start element
        var empty = reader.IsEmptyElement;
        reader.ReadStartElement();
        if (empty)
        {
            // Set default and return when empty
            Value = DateTimeOffset.MinValue;
            return;
        }

        // Parse value from element content and read end
        Value = XmlConvert.ToDateTimeOffset(reader.ReadContentAsString().Trim());

        // Read end element
        reader.ReadEndElement();
    }

    /// <summary>
    /// Converts the current value to a <see cref="DateTime"/>.
    /// </summary>
    public readonly DateTime ToDateTime()
    {
        return Value.LocalDateTime;
    }

    /// <summary>
    /// Converts the current value to a <see cref="DateTimeOffset"/>.
    /// </summary>
    public readonly DateTimeOffset ToDateTimeOffset()
    {
        return Value;
    }

    /// <summary>
    /// Returns the value in W3C recommended ISO 8601 string format.
    /// </summary>
    public override readonly string ToString()
    {
        return XmlConvert.ToString(Value, StringFormat);
    }

    /// <summary>
    /// Returns the value in a specific string format.
    /// </summary>
    public readonly string ToString(string format)
    {
        // Call overloaded method
        return ToString(format, CultureInfo.CurrentCulture);
    }

    /// <summary>
    /// Formats the value of the current instance using the specified format.
    /// </summary>
    /// <param name="format">The format to use or null to use the default <see cref="StringFormat"/>.</param>
    /// <param name="formatProvider">
    /// The provider to use to format the value.-or- A null reference (Nothing in Visual Basic)
    /// to obtain the numeric format information from the current locale setting of the
    /// operating system.
    /// </param>
    /// <returns>The value of the current instance in the specified format.</returns>
    public readonly string ToString(string? format, IFormatProvider? formatProvider)
    {
        return Value.ToString(
            format ?? StringFormat,     // Use our own default format when null
            formatProvider);
    }

    /// <summary>
    /// <see cref="IConvertible"/> implementation which converts to a string.
    /// </summary>
    public readonly string ToString(IFormatProvider? provider)
    {
        return ToString(null, provider);
    }

    /// <summary>
    /// <see cref="IConvertible"/> implementation which uses the
    /// <see cref="XmlDateTimeConverter"/> to attempt conversion to the requested type.
    /// </summary>
    public readonly object ToType(Type conversionType, IFormatProvider? provider)
    {
        var converter = new XmlDateTimeConverter();
        return converter.ConvertTo(this, conversionType)!;
    }

    /// <summary>
    /// Converts an object into its XML representation.
    /// </summary>
    /// <param name="writer">The <see cref="XmlWriter"/> stream to which the object is serialized.</param>
    public readonly void WriteXml(XmlWriter writer)
    {
        // Validate.
        ArgumentNullException.ThrowIfNull(writer);

        // Write.
        writer.WriteString(XmlConvert.ToString(Value, StringFormat));
    }

    /// <summary>
    /// <see cref="IConvertible"/> implementation which returns <see cref="TypeCode.DateTime"/>
    /// that this type is most compatible with.
    /// </summary>
    readonly TypeCode IConvertible.GetTypeCode()
    {
        return TypeCode.DateTime;
    }

    /// <summary>
    /// <see cref="IConvertible"/> implementation which is not supported.
    /// </summary>
    /// <exception cref="NotImplementedException">
    /// Thrown always because this method is not supported.
    /// </exception>
    bool IConvertible.ToBoolean(IFormatProvider? provider)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="IConvertible"/> implementation which is not supported.
    /// </summary>
    /// <exception cref="NotImplementedException">
    /// Thrown always because this method is not supported.
    /// </exception>
    byte IConvertible.ToByte(IFormatProvider? provider)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="IConvertible"/> implementation which is not supported.
    /// </summary>
    /// <exception cref="NotImplementedException">
    /// Thrown always because this method is not supported.
    /// </exception>
    char IConvertible.ToChar(IFormatProvider? provider)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Converts the current value to a <see cref="DateTime"/>.
    /// </summary>
    readonly DateTime IConvertible.ToDateTime(IFormatProvider? provider)
    {
        return ToDateTime();
    }

    /// <summary>
    /// <see cref="IConvertible"/> implementation which is not supported.
    /// </summary>
    /// <exception cref="NotImplementedException">
    /// Thrown always because this method is not supported.
    /// </exception>
    decimal IConvertible.ToDecimal(IFormatProvider? provider)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="IConvertible"/> implementation which is not supported.
    /// </summary>
    /// <exception cref="NotImplementedException">
    /// Thrown always because this method is not supported.
    /// </exception>
    double IConvertible.ToDouble(IFormatProvider? provider)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="IConvertible"/> implementation which is not supported.
    /// </summary>
    /// <exception cref="NotImplementedException">
    /// Thrown always because this method is not supported.
    /// </exception>
    short IConvertible.ToInt16(IFormatProvider? provider)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="IConvertible"/> implementation which is not supported.
    /// </summary>
    /// <exception cref="NotImplementedException">
    /// Thrown always because this method is not supported.
    /// </exception>
    int IConvertible.ToInt32(IFormatProvider? provider)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="IConvertible"/> implementation which is not supported.
    /// </summary>
    /// <exception cref="NotImplementedException">
    /// Thrown always because this method is not supported.
    /// </exception>
    long IConvertible.ToInt64(IFormatProvider? provider)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="IConvertible"/> implementation which is not supported.
    /// </summary>
    /// <exception cref="NotImplementedException">
    /// Thrown always because this method is not supported.
    /// </exception>
    sbyte IConvertible.ToSByte(IFormatProvider? provider)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="IConvertible"/> implementation which is not supported.
    /// </summary>
    /// <exception cref="NotImplementedException">
    /// Thrown always because this method is not supported.
    /// </exception>
    float IConvertible.ToSingle(IFormatProvider? provider)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="IConvertible"/> implementation which is not supported.
    /// </summary>
    /// <exception cref="NotImplementedException">
    /// Thrown always because this method is not supported.
    /// </exception>
    ushort IConvertible.ToUInt16(IFormatProvider? provider)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="IConvertible"/> implementation which is not supported.
    /// </summary>
    /// <exception cref="NotImplementedException">
    /// Thrown always because this method is not supported.
    /// </exception>
    uint IConvertible.ToUInt32(IFormatProvider? provider)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="IConvertible"/> implementation which is not supported.
    /// </summary>
    /// <exception cref="NotImplementedException">
    /// Thrown always because this method is not supported.
    /// </exception>
    ulong IConvertible.ToUInt64(IFormatProvider? provider)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Compares two values of this type.
    /// </summary>
    /// <returns>Zero when same, 1 when first value greater than second or -1 when less.</returns>
    private static int Compare(XmlDateTime left, XmlDateTime right)
    {
        return left == right ? 0 : left > right ? 1 : -1;
    }
}
