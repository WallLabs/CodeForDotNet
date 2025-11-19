using System;
using Windows.UI.Xaml.Controls;

namespace CodeForDotNet.WindowsUniversal.UI.Controls
{
    /// <summary>
    /// Extensions for work with the XAML <see cref="TextBox"/> control.
    /// </summary>
    public static class TextBoxExtensions
    {
        #region Public Methods

        /// <summary>
        /// Sets the <see cref="TextBox.SelectionStart"/> and <see cref="TextBox.SelectionLength"/> as
        /// close a possible to the desired index and length, automatically selecting at least one
        /// character when overwrite mode is necessary, i.e. when the text box is already filled to maximum.
        /// </summary>
        public static void SelectText(this TextBox textBox, int selectIndex, int selectLength)
        {
            // Validate
            if (textBox == null) throw new ArgumentNullException(nameof(textBox));

            // Select first position when invalid
            if (selectIndex < 0)
                selectIndex = 0;

            // Check selection...
            var text = textBox.Text;
            var textLength = text.Length;
            var maxLength = textBox.MaxLength;
            if (maxLength > 0 && textLength >= maxLength)
            {
                // Select the last and at least one character in overwrite mode
                selectIndex = selectIndex <= textLength - 1 ? selectIndex : textLength - 1;
                if (selectIndex < 1)
                    selectLength = 1;
            }
            else
            {
                // Select the same or nearest character in insert mode
                selectIndex = selectIndex <= textLength ? selectIndex : textLength;
            }

            // Ensure selection length does not exceed content
            if (selectIndex + selectLength > textLength)
                selectLength = textLength - selectIndex;

            // Set new values
            textBox.SelectionStart = selectIndex;
            textBox.SelectionLength = selectLength;
        }

        /// <summary>
        /// Types into a text box, overwriting, inserting or adding text according to the current selection.
        /// </summary>
        public static void SendKeys(this TextBox textBox, string value)
        {
            // Validate
            if (textBox == null) throw new ArgumentNullException(nameof(textBox));
            if (value == null) value = "";

            // Calculate change
            var text = textBox.Text;
            var selectIndex = textBox.SelectionStart;
            var selectLength = textBox.SelectionLength;
            if (selectLength > 0)
            {
                // Over-type text
                text = text.Remove(selectIndex, selectLength);
                text = text.Insert(selectIndex, value);
            }
            else
            {
                // Insert or add text
                if (selectIndex < text.Length - 1)
                    text = text.Insert(selectIndex, value);
                else
                    text += value;
            }

            // Trim any padding characters at start when variable length
            var trimmedChars = 0;
            var dynamicTextBox = textBox as DynamicTextBox;
            if (dynamicTextBox != null)
            {
                var minLength = dynamicTextBox.MinLength;
                var maxLength = dynamicTextBox.MaxLength;
                var padChar = dynamicTextBox.PadChar;
                if (minLength != maxLength && text.Length > minLength && padChar.HasValue)
                {
                    var textLengthBeforeTrim = text.Length;
                    text = text.PadLeft(minLength, padChar.Value);
                    trimmedChars = textLengthBeforeTrim - text.Length;
                }
            }

            // Set text
            textBox.Text = text;
            var textLength = text.Length;

            // Select next logical character (depending if trimmed or over-typed)
            selectIndex += value.Length - trimmedChars;
            if (selectIndex > textLength)
                selectIndex = textLength;
            else if (selectIndex < 0)
                selectIndex = 0;

            // Select next character or move to next text box at end
            if (selectIndex < textLength || dynamicTextBox == null || !dynamicTextBox.MoveRightInGroup())
                SelectText(textBox, selectIndex, 0);
        }

        #endregion Public Methods
    }
}
