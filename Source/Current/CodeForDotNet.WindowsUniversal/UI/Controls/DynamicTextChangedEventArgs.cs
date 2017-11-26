using System;
using Windows.UI.Xaml.Controls;

namespace CodeForDotNet.WindowsUniversal.UI.Controls
{
    /// <summary>
    /// Provides a better alternative to <see cref="TextChangedEventArgs"/> to track overrides to text content through multiple event consumers.
    /// Enables the sender to efficiently set the final changes once, avoiding loops or conflicting change events.
    /// </summary>
    public class DynamicTextChangedEventArgs : EventArgs
    {
        #region Lifetime

        /// <summary>
        /// Creates an instance with the required properties.
        /// </summary>
        public DynamicTextChangedEventArgs(string text, string originalText)
        {
            // Validate
            if (text == null) throw new ArgumentNullException("text");
            if (originalText == null) throw new ArgumentNullException("originalText");

            // Initialize member
            Text = text;
            OriginalText = originalText;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Original text content at the time the change occurred (not before).
        /// </summary>
        /// <returns>
        /// May be different to the current control value if called during another change event.
        /// </returns>
        public string OriginalText { get; private set; }

        /// <summary>
        /// New or adjusted text value.
        /// </summary>
        public string Text { get; set; }

        #endregion
    }
}
