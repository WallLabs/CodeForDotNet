using CodeForDotNet.Collections;
using System;
using System.Runtime.Versioning;
using Wia = Interop.Wia;

namespace CodeForDotNet.Windows.Imaging
{
    /// <summary>
    /// Managed <see cref="Wia.Items"/>.
    /// </summary>
    [SupportedOSPlatform("windows")]
    [CLSCompliant(false)]
    public class WiaItemCollection : DisposableCollection<WiaItem>
    {
        #region Lifetime

        /// <summary>
        /// Creates an empty instance.
        /// </summary>
        public WiaItemCollection()
        {
        }

        /// <summary>
        /// Creates an instance to wrap the specified unmanaged object.
        /// </summary>
        public WiaItemCollection(Wia.Items interopCollection)
        {
            // Validate
            if (interopCollection == null) throw new ArgumentNullException(nameof(interopCollection));

            // Add unmanaged collection items with managed wrappers
            foreach (Wia.Item? interopItem in interopCollection)
                Add(new WiaItem(interopItem!));
        }

        #endregion Lifetime
    }
}
