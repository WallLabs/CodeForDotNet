using CodeForDotNet.UI.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using Windows.System;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

#nullable enable

namespace CodeForDotNet.WindowsUniversal.UI.Controls
{
    /// <summary>
    /// Enhanced <see cref="TextBox"/> control supporting multiple key-down tracking and group navigational functionality.
    /// </summary>
    [CLSCompliant(false)]
    public class DynamicTextBox : TextBox
    {
        #region Public Fields

        /// <summary>
        /// Name of the element added to draw the cursor highlight over the selected character(s).
        /// </summary>
        public const string CursorElementName = "Cursor";

        /// <summary>
        /// Name of the resource used as the fill brush for the cursor.
        /// </summary>
        public const string CursorFillBrushResourceName = "ComboBoxItemSelectedBackgroundThemeBrush";

        /// <summary>
        /// <see cref="DirectInput"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DirectInputProperty =
            DependencyProperty.Register("DirectInput", typeof(bool), typeof(DynamicTextBox), new PropertyMetadata(false));

        /// <summary>
        /// <see cref="GroupDown"/> property.
        /// </summary>
        public static readonly DependencyProperty GroupDownProperty =
            DependencyProperty.Register("GroupDown", typeof(Control), typeof(DynamicTextBox), new PropertyMetadata(null));

        /// <summary>
        /// <see cref="GroupNext"/> property.
        /// </summary>
        public static readonly DependencyProperty GroupNextProperty =
            DependencyProperty.Register("GroupNext", typeof(Control), typeof(DynamicTextBox), new PropertyMetadata(null));

        /// <summary>
        /// <see cref="GroupPrevious"/> property.
        /// </summary>
        public static readonly DependencyProperty GroupPreviousProperty =
            DependencyProperty.Register("GroupPrevious", typeof(Control), typeof(DynamicTextBox), new PropertyMetadata(null));

        /// <summary>
        /// <see cref="GroupUp"/> property.
        /// </summary>
        public static readonly DependencyProperty GroupUpProperty =
            DependencyProperty.Register("GroupUp", typeof(Control), typeof(DynamicTextBox), new PropertyMetadata(null));

        /// <summary>
        /// <see cref="MinLength"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MinLengthProperty =
            DependencyProperty.Register("MinLength", typeof(int), typeof(DynamicTextBox), new PropertyMetadata(0));

        /// <summary>
        /// <see cref="PadChar"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PadCharProperty =
            DependencyProperty.Register("PadChar", typeof(char?), typeof(DynamicTextBox), new PropertyMetadata(null));

        /// <summary>
        /// <see cref="Uppercase"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty UppercaseProperty =
            DependencyProperty.Register("Uppercase", typeof(bool), typeof(DynamicTextBox), new PropertyMetadata(false));

        #endregion Public Fields

        #region Private Fields

        private readonly List<VirtualKey> _keysDown;

        /// <summary>
        /// Cursor rectangle.
        /// </summary>
        private Rectangle? _cursor;

        /// <summary>
        /// <see cref="TextBox.SelectionStart"/> value before it was changed.
        /// </summary>
        private int _originalSelectionStart;

        /// <summary>
        /// <see cref="TextBox.Text"/> value before it was changed.
        /// </summary>
        private string _originalText;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Creates an instance.
        /// </summary>
        public DynamicTextBox()
        {
            // Set default style
            DefaultStyleKey = typeof(DynamicTextBox);

            // Initialize members
            _keysDown = new List<VirtualKey>();
            _originalText = Text;
            _originalSelectionStart = SelectionStart;

            // Hook events
            TextChanged += OnTextChanged;
            SelectionChanged += OnSelectionChanged;
            GotFocus += OnGotFocus;
        }

        #endregion Public Constructors

        #region Public Events

        /// <summary>
        /// Fired before the key down event is processed.
        /// </summary>
        public event EventHandler<KeyRoutedEventArgs>? BeforeKeyDown;

        #endregion Public Events

        #region Public Properties

        /// <summary>
        /// Enables direct handling of input from key press events, allowing input to be accepted even when the control has <see cref="TextBox.IsReadOnly"/> set,
        /// e.g. to blocking any soft keyboards from popping up.
        /// </summary>
        public bool DirectInput
        {
            get => (bool)GetValue(DirectInputProperty);
            set => SetValue(DirectInputProperty, value);
        }

        /// <summary>
        /// Sets the next control downwards in an input group, to which the focus will be switched when moving down in this text box.
        /// </summary>
        public Control GroupDown
        {
            get => (Control)GetValue(GroupDownProperty);
            set => SetValue(GroupDownProperty, value);
        }

        /// <summary>
        /// Sets the next control in an input group, to which the focus will be switched when moving right at the edge of this text box or typing a character
        /// beyond the maximum length.
        /// </summary>
        public Control GroupNext
        {
            get => (Control)GetValue(GroupNextProperty);
            set => SetValue(GroupNextProperty, value);
        }

        /// <summary>
        /// Sets the previous control in an input group, to which the focus will be switched when moving left or typing at the edge of this text box.
        /// </summary>
        public Control GroupPrevious
        {
            get => (Control)GetValue(GroupPreviousProperty);
            set => SetValue(GroupPreviousProperty, value);
        }

        /// <summary>
        /// Sets the previous control upwards an input group, to which the focus will be switched when moving up in this text box.
        /// </summary>
        public Control GroupUp
        {
            get => (Control)GetValue(GroupUpProperty);
            set => SetValue(GroupUpProperty, value);
        }

        /// <summary>
        /// Minimum text length.
        /// </summary>
        public int MinLength
        {
            get => (int)GetValue(MinLengthProperty);
            set => SetValue(MinLengthProperty, value);
        }

        /// <summary>
        /// Padding character used when the current text is less than <see cref="MinLength"/>.
        /// </summary>
        public char? PadChar
        {
            get => (char?)GetValue(PadCharProperty);
            set => SetValue(PadCharProperty, value);
        }

        /// <summary>
        /// Forces all alphabetical keys to upper case, not shift. Requires <see cref="DirectInput"/> to be enabled.
        /// </summary>
        public bool Uppercase
        {
            get => (bool)GetValue(UppercaseProperty);
            set => SetValue(UppercaseProperty, value);
        }

        #endregion Public Properties

        #region Protected Properties

        /// <summary>
        /// Currently pressed keys. Set during the <see cref="OnKeyDown"/> even handler, after <see cref="OnBeforeKeyDown"/> but before
        /// <see cref="OnAfterKeyDown"/> to supporting repeat detection.
        /// </summary>
        protected ReadOnlyCollection<VirtualKey> KeysDown => new ReadOnlyCollection<VirtualKey>(_keysDown);

        #endregion Protected Properties

        #region Public Methods

        /// <summary>
        /// Deletes the current selection or one character to the left (backspace rather than delete).
        /// </summary>
        public void DoBackspace()
        {
            // Do nothing when empty or at start
            var text = Text;
            var selectLength = SelectionLength;
            var selectIndex = SelectionStart;
            if (text.Length == 0 || selectIndex < 0 || (selectIndex == 0 && selectLength == 0))
            {
                return;
            }

            // Delete selection or at least the current character
            if (selectLength > 0)
            {
                // Delete current selection and stay at current position
                text = text.Remove(selectIndex, selectLength);
            }
            else
            {
                // Delete previous character and move backwards
                text = text.Remove(selectIndex - 1, 1);
                if (selectIndex > 0)
                {
                    selectIndex--;
                }
            }

            // Pad if necessary
            var minLength = MinLength;
            if (minLength > 0 && PadChar.HasValue && text.Length < minLength)
            {
                text = text.PadLeft(minLength, PadChar.Value);
            }

            // Set new value
            Text = text;

            // Keep selection
            this.SelectText(selectIndex, 0);
        }

        /// <summary>
        /// Deletes the current selection or one character to the right.
        /// </summary>
        public void DoDelete()
        {
            // Do nothing when empty or at end
            var text = Text;
            var selectLength = SelectionLength;
            var selectIndex = SelectionStart;
            if (text.Length == 0 || selectIndex >= text.Length)
            {
                return;
            }

            // Delete selection or at least the current character
            text = text.Remove(selectIndex, selectLength > 0 ? selectLength : 1);

            // Pad if necessary
            var minLength = MinLength;
            if (minLength > 0 && PadChar.HasValue && text.Length < minLength)
            {
                text = text.PadRight(minLength, PadChar.Value);
            }

            // Set new value
            Text = text;

            // Keep selection
            this.SelectText(selectIndex, 0);
        }

        /// <summary>
        /// Handles the key down event.
        /// </summary>
        public void HandleKeyDown(KeyRoutedEventArgs args)
        {
            OnKeyDown(args);
        }

        /// <summary>
        /// Moves to the <see cref="GroupDown"/> control if specified. When the next control is a <see cref="TextBox"/> also selects the next logical character
        /// from which to continue input.
        /// </summary>
        /// <returns>True when a move occurred.</returns>
        public bool MoveDownInGroup()
        {
            // Check if control specified, return negative when not
            var control = GroupDown;
            if (control is null)
                return false;

            // Focus new control
            control.Focus(FocusState.Keyboard);
            if (control is TextBox textBox)
            {
                // Select first character
                textBox.SelectText(0, 0);
            }

            // Return positive
            return true;
        }

        /// <summary>
        /// Moves to the <see cref="GroupPrevious"/> control if specified. When the next control is a <see cref="TextBox"/> also selects the next logical
        /// character from which to continue input.
        /// </summary>
        /// <returns>True when a move occurred.</returns>
        public bool MoveLeftInGroup()
        {
            // Check if control specified, return negative when not
            var control = GroupPrevious;
            if (control == null)
            {
                return false;
            }

            // Focus new control
            control.Focus(FocusState.Keyboard);
            if (control is TextBox textBox)
            {
                // Select last character of previous text box
                var textMax = textBox.MaxLength;
                var textLength = textBox.Text.Length;
                var selectStart = textLength > 0 && textMax > 0 && textLength >= textMax ? textLength - 1 : textLength;
                var selectLength = SelectionLength;
                if (selectLength > textLength - selectStart)
                {
                    selectLength = textLength - selectStart;
                }

                this.SelectText(selectStart, selectLength);
            }

            // Return positive
            return true;
        }

        /// <summary>
        /// Moves to the <see cref="GroupNext"/> control if specified. When the next control is a <see cref="TextBox"/> also selects the next logical character
        /// from which to continue input.
        /// </summary>
        /// <returns>True when a move occurred.</returns>
        public bool MoveRightInGroup()
        {
            // Check if control specified, return negative when not
            var control = GroupNext;
            if (control is null)
                return false;

            // Focus new control
            control.Focus(FocusState.Keyboard);
            if (control is TextBox textBox)
            {
                // Select first character of next text box
                textBox.SelectText(0, 0);
            }

            // Return positive
            return true;
        }

        /// <summary>
        /// Moves to the <see cref="GroupUp"/> control if specified. When the next control is a <see cref="TextBox"/> also selects the next logical character
        /// from which to continue input.
        /// </summary>
        /// <returns>True when a move occurred.</returns>
        public bool MoveUpInGroup()
        {
            // Check if control specified, return negative when not
            var control = GroupUp;
            if (control is null)
                return false;

            // Focus new control
            control.Focus(FocusState.Keyboard);
            if (control is TextBox textBox)
            {
                // Select first character
                textBox.SelectText(0, 0);
            }

            // Return positive
            return true;
        }

        #endregion Public Methods

        #region Protected Methods

        /// <summary>
        /// Called after the <see cref="UIElement.KeyDown"/> event is processed. The key press was blocked when <see cref="KeyRoutedEventArgs.Handled"/> is true.
        /// </summary>
        protected virtual void OnAfterKeyDown(KeyRoutedEventArgs args)
        {
        }

        /// <summary>
        /// Called after the <see cref="UIElement.KeyUp"/> event is processed. The key release processing was blocked when
        /// <see cref="KeyRoutedEventArgs.Handled"/> is true.
        /// </summary>
        protected virtual void OnAfterKeyUp(KeyRoutedEventArgs args)
        {
        }

        /// <summary>
        /// Called after the base text box changed event has been processed.
        /// </summary>
        protected virtual void OnAfterTextChanged(object sender, DynamicTextChangedEventArgs args)
        {
        }

        /// <summary>
        /// Prepares the control for display.
        /// </summary>
        protected override void OnApplyTemplate()
        {
            // Apply base class template
            base.OnApplyTemplate();

            // Locate our control additions
            _cursor = (Rectangle)FindName(CursorElementName);

            // Create when doesn't exist
            if (_cursor == null)
            {
                var grid = (Grid)VisualTreeHelper.GetChild(this, 0);
                var canvas = new Canvas { Background = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0)) };
                canvas.SetValue(Grid.ColumnProperty, 0);
                var cursor = new Rectangle {
                    Name = CursorElementName,
                    Visibility = Visibility.Collapsed,
                    Fill = (Brush)Resources[CursorFillBrushResourceName]
                };
                canvas.Children.Add(cursor);
                grid.Children.Add(canvas);
                _cursor = cursor;
            }
        }

        /// <summary>
        /// Called before the <see cref="UIElement.KeyDown"/> event is processed. Set <see cref="KeyRoutedEventArgs.Handled"/> true to block the key press from
        /// being processed by the base control. Use the <see cref="KeyRoutedEventArgs.Key"/> to examine the current key press, the <see cref="KeysDown"/>
        /// collection does not contain the current key unless it was previously pressed to support repeat detection. This class implements grouped control
        /// navigation in this method. If you override this method and do not call it this functionality will be disabled.
        /// </summary>
        protected virtual void OnBeforeKeyDown(KeyRoutedEventArgs args)
        {
            // Validate.
            if (args is null) throw new ArgumentNullException(nameof(args));

            // Handle specific key presses
            var selectIndex = SelectionStart;
            var selectLength = SelectionLength;
            var text = Text;
            var textLength = text.Length;
            var overtype = MaxLength > 0 && textLength >= MaxLength;
            var selectionMax = overtype ? textLength - 1 : textLength;
            var shift = KeysDown.Contains(VirtualKey.Shift) ||
                        KeysDown.Contains(VirtualKey.LeftShift) ||
                        KeysDown.Contains(VirtualKey.RightShift);
            switch (args.Key)
            {
                case VirtualKey.Delete:

                    // Delete
                    args.Handled = true;
                    DoDelete();
                    break;

                case VirtualKey.Back:

                    // Backspace
                    args.Handled = true;
                    DoBackspace();
                    break;

                case VirtualKey.Left:
                    {
                        // Move or extend selection left within text box, or previous group
                        args.Handled = true;
                        if (shift)
                        {
                            // Extend selection when shift is held
                            if (selectIndex > 0)
                            {
                                selectIndex--;
                                selectLength++;
                                this.SelectText(selectIndex, selectLength);
                            }
                        }
                        else if (selectIndex > 0 || selectLength > (overtype ? 1 : 0))
                        {
                            // Move cursor and cancel any extended selection when shift is not held
                            if (selectLength <= (overtype ? 1 : 0))
                            {
                                selectIndex--;
                            }

                            selectLength = 0;
                            this.SelectText(selectIndex, selectLength);
                        }
                        else
                        {
                            // Shift not held and no selection...

                            // Moving left at edge moves to previous in group when specified
                            MoveLeftInGroup();
                        }
                        break;
                    }

                case VirtualKey.Right:
                    {
                        // Move or extend selection right within text box, or move to next group
                        args.Handled = true;
                        if (shift)
                        {
                            // Extend selection when shift is held
                            if (selectIndex + selectLength < textLength)
                            {
                                selectLength++;
                                this.SelectText(selectIndex, selectLength);
                            }
                        }
                        else if (selectIndex < selectionMax)
                        {
                            // Move cursor and cancel any extended selection when shift is not held
                            selectIndex += selectLength > (overtype ? 1 : 0) ? selectLength : 1;
                            selectLength = 0;
                            this.SelectText(selectIndex, selectLength);
                        }
                        else
                        {
                            // Shift not held...

                            // Moving right at edge moves to next in group when specified
                            MoveRightInGroup();
                        }
                        break;
                    }

                case VirtualKey.Up:
                    {
                        // Move up within group if specified
                        args.Handled = true;
                        MoveUpInGroup();
                        break;
                    }

                case VirtualKey.Down:
                    {
                        // Move down within group if specified
                        args.Handled = true;
                        MoveDownInGroup();
                        break;
                    }
            }
        }

        /// <summary>
        /// Called before the <see cref="UIElement.KeyUp"/> event is processed. Set <see cref="KeyRoutedEventArgs.Handled"/> true to block the key release from
        /// being processed by the base control. Use the <see cref="KeyRoutedEventArgs.Key"/> to examine the released key, the <see cref="KeysDown"/> collection
        /// is the state before processing this event so still contains the released key.
        /// </summary>
        protected virtual void OnBeforeKeyUp(KeyRoutedEventArgs args)
        {
        }

        /// <summary>
        /// Enforces selection rules based on current/maximum length and trying over to next control in a group.
        /// </summary>
        protected virtual void OnBeforeSelectionChanged(object sender, DynamicTextSelectionChangedEventArgs args)
        {
            // Validate.
            if (args is null) throw new ArgumentNullException(nameof(args));

            // Move to next control (if any) when at edge...
            var nextControl = GroupNext;
            var textLength = Text.Length;
            var selectedIndex = args.SelectionStart;
            var maxLength = MaxLength;
            if (maxLength > 0 && selectedIndex >= maxLength && nextControl != null)
            {
                // Set new control with cursor at start
                args.Focus = nextControl;
                args.SelectionStart = 0;
                args.SelectionLength = 0;
                return;
            }

            // Select one character (over-type) when within text but is at maximum length
            if (maxLength > 0 && textLength >= maxLength)
            {
                // Move back one character at end
                if (selectedIndex >= maxLength)
                {
                    args.SelectionStart = selectedIndex - 1;
                }

                // Select one character
                args.SelectionLength = 1;
            }

            // Over-type when only remaining character is pad character
            if (textLength == 1 && PadChar.HasValue && Text == PadChar.Value.ToString(CultureInfo.InvariantCulture))
            {
                args.SelectionStart = 0;
                args.SelectionLength = 1;
            }
        }

        /// <summary>
        /// Called before the base text box changed event is processed but this class, but after the core text box has already processed a change (because it
        /// provides no text change handler itself to override). This class performs trimming or padding here, so must be called when overridden to retain this functionality.
        /// </summary>
        protected virtual void OnBeforeTextChanged(object sender, DynamicTextChangedEventArgs args)
        {
            // Validate.
            if (args is null) throw new ArgumentNullException(nameof(args));

            // Trim text when too long
            var text = args.Text;
            var maxLength = MaxLength;
            if (maxLength > 0 && text.Length > maxLength)
            {
                // Trim text
                text = text.Substring(0, maxLength);
            }

            // Pad text when enabled and too short
            var minLength = MinLength;
            if (minLength > 0 && text.Length < MinLength && PadChar.HasValue)
            {
                text = text.PadLeft(minLength, PadChar.Value);
            }

            // Return result
            args.Text = text;
        }

        /// <summary>
        /// Tracks key presses, filters keys and updates data bindings.
        /// </summary>
        protected override void OnKeyDown(KeyRoutedEventArgs args)
        {
            // Fire before event
            BeforeKeyDown?.Invoke(this, args);

            // Call before event handler
            OnBeforeKeyDown(args);

            // Process key press unless blocked
            if (!args.Handled)
            {
                if (DirectInput)
                {
                    // Attempt to translate and enter key directly when read-only
                    var upperCase = Uppercase ||
                                    KeysDown.Contains(VirtualKey.Shift) ||
                                    KeysDown.Contains(VirtualKey.LeftShift) ||
                                    KeysDown.Contains(VirtualKey.RightShift) ||
                                    KeysDown.Contains(VirtualKey.CapitalLock);
                    var inputText = KeyboardKeyConverter.ConvertToString((KeyboardKey)args.Key, upperCase, CultureInfo.CurrentCulture);
                    if (inputText != null)
                    {
                        // Update text directly...
                        this.SendKeys(inputText);
                        args.Handled = true;
                    }
                }
                else
                {
                    // Let system handle key press
                    base.OnKeyDown(args);
                }
            }

            // Track pressed keys (add to keys down if not already present)
            if (!_keysDown.Contains(args.Key))
            {
                _keysDown.Add(args.Key);
            }

            // Call after event handler
            OnAfterKeyDown(args);
        }

        /// <summary>
        /// Tracks key presses.
        /// </summary>
        protected override void OnKeyUp(KeyRoutedEventArgs args)
        {
            // Validate.
            if (args is null) throw new ArgumentNullException(nameof(args));

            // Call before event handler
            OnBeforeKeyUp(args);

            // Call base class method to process key release unless blocked
            if (!args.Handled)
            {
                base.OnKeyUp(args);
            }

            // Track pressed keys (remove from keys down)
            if (_keysDown.Contains(args.Key))
            {
                _keysDown.Remove(args.Key);
            }

            // Call after event handler
            OnAfterKeyUp(args);
        }

        #endregion Protected Methods

        #region Private Methods

        /// <summary>
        /// Positions the cursor based on current selection state.
        /// </summary>
        private void LayoutCursor()
        {
            // Do nothing before cursor is initialized
            if (_cursor == null)
            {
                throw new InvalidOperationException(AssemblyResources.DynamicTextBoxStyleMissingCursor);
            }

            // Do nothing when no text.
            var textLength = Text.Length;
            if (textLength == 0)
            {
                return;
            }

            // Get area of selected character
            var cursorIndex = SelectionStart;
            var trailing = false;
            if (cursorIndex >= textLength)
            {
                trailing = true;
                cursorIndex = textLength - 1;
            }
            var cursorRect = GetRectFromCharacterIndex(cursorIndex, trailing);

            // Move cursor to selected character
            var padding = Padding;
            Canvas.SetLeft(_cursor, cursorRect.Left + padding.Left);
            Canvas.SetTop(_cursor, cursorRect.Top + padding.Top);
            _cursor.Width = 1;
            _cursor.Height = RenderSize.Height - padding.Top - padding.Bottom;
        }

        /// <summary>
        /// Checks text selection when focus is received.
        /// </summary>
        private void OnGotFocus(object sender, RoutedEventArgs e)
        {
            this.SelectText(SelectionStart, SelectionLength);
        }

        /// <summary>
        /// Handles the selection changed event.
        /// </summary>
        private void OnSelectionChanged(object sender, RoutedEventArgs args)
        {
            // Apply rules
            var change = new DynamicTextSelectionChangedEventArgs(this, SelectionStart, SelectionLength);
            OnBeforeSelectionChanged(this, change);

            // Change focus when necessary
            var newFocus = change.Focus;
            if (newFocus != this)
            {
                newFocus.Focus(FocusState.Keyboard);
                return;
            }

            // Set new selection index and length
            SelectionStart = change.SelectionStart;
            SelectionLength = change.SelectionLength;

            // Update cursor
            LayoutCursor();
        }

        /// <summary>
        /// Applies format and range rules as the text changes.
        /// </summary>
        private void OnTextChanged(object sender, TextChangedEventArgs args)
        {
            // Save selection position relative to right edge
            var originalTextLength = _originalText.Length;
            var selectionOffset = originalTextLength - _originalSelectionStart;

            // Call before event handler
            var change = new DynamicTextChangedEventArgs(Text, _originalText);
            OnBeforeTextChanged(sender, change);

            // Adjust text value when changed by event handler
            if (Text != change.Text)
            {
                Text = _originalText = change.Text;
            }

            _originalText = Text;

            // Restore selection
            var textLength = change.Text.Length;
            var selectionStart = textLength - selectionOffset;
            if (selectionStart < 0)
            {
                selectionStart = 0;
            }
            else if (selectionStart > textLength)
            {
                selectionStart = textLength;
            }

            if (SelectionStart != selectionStart)
            {
                SelectionStart = selectionStart;
            }

            _originalSelectionStart = selectionStart;

            // Update cursor
            LayoutCursor();

            // Call after event handlers
            OnAfterTextChanged(sender, change);
        }

        #endregion Private Methods
    }
}
