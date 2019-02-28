using System;
using System.Drawing;

namespace CodeForDotNet.UI.Input
{
    /// <summary>
    /// UI mouse input event arguments.
    /// </summary>
    /// <remarks>
    /// Portable version of the "System.Windows.Forms.MouseEventArgs" class. To be replaced when
    /// available in the .NET Standard framework.
    /// </remarks>
    public class MouseEventArgs : EventArgs
    {
        #region Public Constructors

        /// <summary>
        /// Creates an instance with the specified value.
        /// </summary>
        public MouseEventArgs(Point location, MouseButtons buttons)
        {
            Location = location;
            Buttons = buttons;
        }

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// Mouse buttons.
        /// </summary>
        public MouseButtons Buttons { get; set; }

        /// <summary>
        /// Mouse location.
        /// </summary>
        public Point Location { get; set; }

        #endregion Public Properties
    }
}
