﻿using System;
using Windows.UI.Xaml.Data;

namespace CodeForDotNet.WindowsUniversal.UI.Converters
{
    /// <summary>
    /// Null test to boolean one-way value converter.
    /// </summary>
    /// <remarks>
    /// Converts null to false and non-null to true, any object type.
    /// </remarks>
    public class IsNullToBooleanValueConverter : IValueConverter
    {
        /// <summary>
        /// Modifies the source data before passing it to the target for display in the UI.
        /// </summary>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            // Return false when null
            return ReferenceEquals(value, null);
        }

        /// <summary>
        /// Two-way bindings are not supported.
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotSupportedException();
        }
    }
}