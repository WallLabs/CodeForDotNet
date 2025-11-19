using System;
using System.Globalization;
using Windows.UI.Xaml.Data;

namespace CodeForDotNet.WindowsUniversal.UI.Converters
{
    /// <summary>
    /// Byte to hexadecimal string two-way value converter.
    /// </summary>
    /// <remarks>
    /// Converts an unsigned byte to a fixed width format uppercase hexadecimal string without any
    /// prefix, i.e. exactly two characters "00" to "FF". Converts a string of any supported (
    /// <see cref="NumberStyles.HexNumber"/>) format to an unsigned byte.
    /// </remarks>
    public partial class ByteToHexStringValueConverter : IValueConverter
    {
        #region Public Methods

        /// <summary>
        /// Modifies the source data before passing it to the target for display in the UI.
        /// </summary>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            // Validate
            if (value is not byte byteValue) throw new ArgumentOutOfRangeException(nameof(value));

            // Convert to hexadecimal byte string
            return string.Format(CultureInfo.InvariantCulture, "{0:X2}", byteValue);
        }

        /// <summary>
        /// Modifies the target data before passing it to the source object. This method is called
        /// only in TwoWay bindings.
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            // Validate
            if (value is not string stringValue || stringValue is null)
                throw new ArgumentNullException(nameof(value));

            // Convert hexadecimal string to byte
            return byte.Parse(stringValue, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
        }

        #endregion Public Methods
    }
}
