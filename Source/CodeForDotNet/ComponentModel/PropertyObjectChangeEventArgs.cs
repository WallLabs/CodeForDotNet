using System;
using System.Collections.ObjectModel;

namespace CodeForDotNet.ComponentModel
{
    /// <summary>
    /// Event arguments for the <see cref="IPropertyObject.PropertyObjectChanged"/> event.
    /// </summary>
    public class PropertyObjectChangeEventArgs : EventArgs
    {
        #region Lifetime

        /// <summary>
        /// Creates an empty instance.
        /// </summary>
        public PropertyObjectChangeEventArgs()
        {
            Keys = new Collection<Guid>();
        }

        /// <summary>
        /// Creates an instance with the specified values.
        /// </summary>
        public PropertyObjectChangeEventArgs(Guid[] keys)
        {
            Keys = new Collection<Guid>(keys);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Names of the properties or relations which changed.
        /// </summary>
        public Collection<Guid> Keys { get; private set; }

        #endregion
    }
}