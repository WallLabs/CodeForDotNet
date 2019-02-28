using System;
using System.Drawing;

namespace CodeForDotNet.UI.Drawing
{
    /// <summary>
    /// UI graphics paint event arguments.
    /// </summary>
    /// <remarks>
    /// Portable version of the "System.Windows.Forms.PaintEventArgs" class. To be replaced when
    /// available in the .NET Standard framework.
    /// </remarks>
    public class PaintEventArgs : EventArgs
    {
        #region Public Constructors

        /// <summary>
        /// Creates an instance with the specified values.
        /// </summary>
        public PaintEventArgs(Graphics graphics, Rectangle region)
        {
            Graphics = graphics;
            Region = region;
        }

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// Graphics interface to be used for drawing.
        /// </summary>
        public Graphics Graphics { get; set; }

        /// <summary>
        /// Region which has been invalidated and should be re-drawn.
        /// </summary>
        public Rectangle Region { get; set; }

        #endregion Public Properties
    }
}
