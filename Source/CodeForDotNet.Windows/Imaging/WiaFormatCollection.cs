using CodeForDotNet.Collections;
using System;
using Wia = Interop.Wia;

namespace CodeForDotNet.Windows.Imaging
{
    /// <summary>
    /// Managed <see cref="Wia.Formats"/>.
    /// </summary>
    public class WiaFormatCollection : DisposableCollection<object>
    {
        #region Lifetime

        /// <summary>
        /// Creates an empty instance.
        /// </summary>
        public WiaFormatCollection()
        {
        }

        /// <summary>
        /// Creates an instance to wrap the specified unmanaged object.
        /// </summary>
        public WiaFormatCollection(Wia.Formats interopCollection)
        {
            // Validate
            if (interopCollection == null) throw new ArgumentNullException(nameof(interopCollection));

            // Add unmanaged collection items with managed wrappers
            foreach (object? interopItem in interopCollection)
                Add(interopItem!);
        }

        #endregion Lifetime
    }
}
