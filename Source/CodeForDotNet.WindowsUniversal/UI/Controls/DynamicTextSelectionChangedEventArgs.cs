using System;
using Windows.UI.Xaml.Controls;

namespace CodeForDotNet.WindowsUniversal.UI.Controls
{
    /// <summary>
    /// Provides a better alternative to <see cref="SelectionChangedEventArgs"/> to track overrides to
    /// selection position and length during change events, throughout multiple event consumers,
    /// with additional support for the concept of focus shifting to another control.
    /// Enables the sender to efficiently set the final changes once and avoid looping or conflicting change events.
    /// </summary>
    [CLSCompliant(false)]
    public class DynamicTextSelectionChangedEventArgs : EventArgs
    {
        #region Lifetime

        /// <summary>
        /// Creates an instance with the required properties.
        /// </summary>
        public DynamicTextSelectionChangedEventArgs(Control focus, int selectionStart, int selectionLength)
        {
            // Validate
            if (focus == null) throw new ArgumentNullException(nameof(focus));
            if (selectionStart < 0) throw new ArgumentOutOfRangeException(nameof(selectionStart));
            if (selectionLength < 0) throw new ArgumentOutOfRangeException(nameof(selectionLength));

            // Initialize members
            Focus = OriginalFocus = focus;
            SelectionStart = OriginalSelectionStart = selectionStart;
            SelectionLength = OriginalSelectionLength = selectionLength;
        }

        #endregion Lifetime

        #region Properties

        /// <summary>
        /// Original control which has focus (where selection is changing).
        /// </summary>
        public Control OriginalFocus { get; private set; }

        /// <summary>
        /// Original selection start index at the time the change occurred (not before).
        /// </summary>
        /// <returns>
        /// May be different to the current control value if called during another change event.
        /// </returns>
        public int OriginalSelectionStart { get; private set; }

        /// <summary>
        /// Original selection length at the time the change occurred (not before).
        /// </summary>
        /// <returns>
        /// May be different to the current control value if called during another change event.
        /// </returns>
        public int OriginalSelectionLength { get; private set; }

        /// <summary>
        /// Control which should have focus after the event is processed,
        /// which may have been changed by event handlers.
        /// </summary>
        public Control Focus { get; set; }

        /// <summary>
        /// Selection start after the event is processed,
        /// which may have been changed by event handlers.
        /// </summary>
        public int SelectionStart { get; set; }

        /// <summary>
        /// Selection end after the event is processed,
        /// which may have been changed by event handlers.
        /// </summary>
        public int SelectionLength { get; set; }

        #endregion Properties
    }
}
