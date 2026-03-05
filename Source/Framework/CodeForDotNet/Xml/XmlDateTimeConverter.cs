using System;
using System.ComponentModel;
using System.Globalization;

namespace CodeForDotNet.Xml;

/// <summary>
/// Type converter for the <see cref="XmlDateTime"/> value type.
/// </summary>
public class XmlDateTimeConverter : DateTimeOffsetConverter
{
    /// <summary>
    /// Returns whether this converter can convert an object of the given type to the type of
    /// this converter, using the specified context.
    /// </summary>
    /// <returns>true if this converter can perform the conversion; otherwise, false.</returns>
    /// <param name="context">
    /// An <see cref="ITypeDescriptorContext"/> that provides a format context.
    /// </param>
    /// <param name="sourceType">
    /// A <see cref="Type"/> that represents the type you want to convert from.
    /// </param>
    public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
    {
        return
            sourceType == typeof(XmlDateTime) ||
            sourceType == typeof(DateTimeOffset) ||
            sourceType == typeof(DateTime) ||
            sourceType == typeof(string);
    }

    /// <summary>
    /// Returns whether this converter can convert the object to the specified type, using the
    /// specified context.
    /// </summary>
    /// <returns>true if this converter can perform the conversion; otherwise, false.</returns>
    /// <param name="context">
    /// An <see cref="ITypeDescriptorContext"/> that provides a format context.
    /// </param>
    /// <param name="destinationType">
    /// A <see cref="Type"/> that represents the type you want to convert to.
    /// </param>
    public override bool CanConvertTo(ITypeDescriptorContext? context, Type? destinationType)
    {
        return
            destinationType == typeof(XmlDateTime) ||
            destinationType == typeof(DateTimeOffset) ||
            destinationType == typeof(DateTime) ||
            destinationType == typeof(string);
    }

    /// <summary>
    /// Converts the given object to the type of this converter, using the specified context and
    /// culture information.
    /// </summary>
    /// <returns>
    /// An <see cref="object"/> that represents the converted value or null when value is not specified.
    /// </returns>
    /// <param name="context">
    /// An <see cref="ITypeDescriptorContext"/> that provides a format context.
    /// </param>
    /// <param name="culture">The <see cref="CultureInfo"/> to use as the current culture.</param>
    /// <param name="value">The <see cref="object"/> to convert.</param>
    /// <exception cref="NotSupportedException">The conversion cannot be performed.</exception>
    public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
    {
        // Do nothing when source is null
        if (value is null)
            return null;

        // Get types
        var targetType = value.GetType();
        var ourType = typeof(XmlDateTime);

        // Convert source value
        if (targetType == ourType)
            return (XmlDateTime)value;
        if (targetType == typeof(DateTimeOffset))
            return new XmlDateTime((DateTimeOffset)value);
        if (targetType == typeof(DateTime))
            return new XmlDateTime((DateTime)value);
        if (targetType == typeof(string))
            return XmlDateTime.Parse((string)value, culture);

        // Unsupported type
        throw new ArgumentOutOfRangeException(nameof(value));
    }

    /// <summary>
    /// Converts the given value object to the specified type, using the specified context and
    /// culture information.
    /// </summary>
    /// <returns>An <see cref="object"/> that represents the converted value.</returns>
    /// <param name="context">
    /// An <see cref="ITypeDescriptorContext"/> that provides a format context.
    /// </param>
    /// <param name="culture">
    /// A <see cref="CultureInfo"/>. If null is passed, the current culture is assumed.
    /// </param>
    /// <param name="value">The <see cref="object"/> to convert.</param>
    /// <param name="destinationType">
    /// The <see cref="Type"/> to convert the <paramref name="value"/> parameter to.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// The <paramref name="destinationType"/> parameter is null.
    /// </exception>
    /// <exception cref="NotSupportedException">The conversion cannot be performed.</exception>
    public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type? destinationType)
    {
        // Do nothing when source is null
        if (value is not XmlDateTime source)
            return null;

        // Convert target value
        if (destinationType == typeof(XmlDateTime))
            return source;
        if (destinationType == typeof(DateTimeOffset))
            return source.ToDateTimeOffset();
        if (destinationType == typeof(DateTime))
            return source.ToDateTime();
        if (destinationType == typeof(string))
            return source.ToString(null, culture);

        // Unsupported type
        throw new ArgumentOutOfRangeException(nameof(destinationType));
    }
}
