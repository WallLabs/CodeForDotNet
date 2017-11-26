using System;
using Windows.UI.Xaml.Data;

namespace CodeForDotNet.WindowsUniversal.UI.Converters
{
    /// <summary>
    /// Value converter that translates true to false and vice versa.
    /// </summary>
    public sealed class BooleanNegationConverter : IValueConverter
    {
        /// <summary>
        /// Converts a <see cref="Boolean"/> by negating it.
        /// </summary>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return !(value is bool && (bool)value);
        }

        /// <summary>
        /// Converts a <see cref="Boolean"/> back by negating it again.
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return !(value is bool && (bool)value);
        }
    }
}
