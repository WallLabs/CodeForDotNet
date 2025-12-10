using System;
using System.ComponentModel;
using System.Numerics;

namespace CodeForDotNet.ComponentModel;

/// <summary>
/// Type converter which supports use of <see cref="BigInteger"/> types in dependency properties.
/// </summary>
public class BigIntegerConverter : TypeConverter
{
    #region Public Methods

    /// <summary>
    /// Tests if conversion is possible from a source type.
    /// </summary>
    public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
    {
        return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
    }

    /// <summary>
    /// Tests if conversion is possible to a destination type.
    /// </summary>
    public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
    {
        return destinationType == typeof(string) || base.CanConvertTo(context, destinationType);
    }

    /// <summary>
    /// Converts a value from a source type.
    /// </summary>
    public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
    {
        // Convert from string
        if (value is string stringValue)
            return BigInteger.Parse(stringValue, culture);

        // Convert from other types
        return base.ConvertFrom(context, culture, value);
    }

    /// <summary>
    /// Converts the current value to the target type.
    /// </summary>
    public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
    {
        // Get value
        var bigValue = (BigInteger)value;

        // Convert to string
        if (destinationType == typeof(string))
            return bigValue.ToString(culture);

        // Convert to other types
        return base.ConvertTo(context, culture, value, destinationType);
    }

    #endregion Public Methods
}
