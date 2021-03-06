using CodeForDotNet.Collections;
using System;
using System.Runtime.Versioning;
using Wia = Interop.Wia;

namespace CodeForDotNet.Windows.Imaging
{
    /// <summary>
    /// Managed <see cref="Properties"/>.
    /// </summary>
    [SupportedOSPlatform("windows")]
    [CLSCompliant(false)]
    public class WiaPropertyCollection : DisposableCollection<WiaProperty>
    {
        #region Lifetime

        /// <summary>
        /// Creates an empty instance.
        /// </summary>
        public WiaPropertyCollection()
        {
        }

        /// <summary>
        /// Creates an instance to wrap the specified unmanaged object.
        /// </summary>
        public WiaPropertyCollection(Wia.Properties interopCollection)
        {
            // Validate
            if (interopCollection == null) throw new ArgumentNullException(nameof(interopCollection));

            // Add unmanaged collection items with managed wrappers
            foreach (Wia.Property? interopProperty in interopCollection)
                Add(new WiaProperty(interopProperty!));
        }

        #endregion Lifetime
    }
}
