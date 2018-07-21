using System;

namespace CodeForDotNet
{
    /// <summary>
    /// Event arguments containing a typed value.
    /// </summary>
    public class ValueEventArgs<T> : EventArgs
    {
        #region Lifetime

        /// <summary>
        /// Initializes a new instance with the specified value.
        /// </summary>
        public ValueEventArgs(T value)
        {
            Value = value;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the value passed with this event.
        /// </summary>
        public T Value { get; set; }

        #endregion
    }
}
