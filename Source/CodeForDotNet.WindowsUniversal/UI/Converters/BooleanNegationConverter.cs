using System;
using Windows.UI.Xaml.Data;

namespace CodeForDotNet.WindowsUniversal.UI.Converters
{
    /// <summary>
    /// Value converter that translates true to false and vice versa.
    /// </summary>
    public sealed partial class BooleanNegationConverter : IValueConverter
    {
        /// <summary>
        /// Converts a <see cref="bool"/> by negating it.
        /// </summary>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return !(value is bool result && result);
        }

        /// <summary>
        /// Converts a <see cref="bool"/> back by negating it again.
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return !(value is bool result && result);
        }
    }
}
