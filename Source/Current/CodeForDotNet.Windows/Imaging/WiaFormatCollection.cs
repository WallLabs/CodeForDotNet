using System;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using Wia = Interop.Wia;

namespace CodeForDotNet.Windows.Imaging
{
    /// <summary>
    /// Managed <see cref="Wia.Formats"/>.
    /// </summary>
    public class WiaFormatCollection : Collection<object>, IDisposable
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
        [CLSCompliant(false)]
        public WiaFormatCollection(Wia.Formats interopCollection)
        {
            // Validate
            if (interopCollection == null) throw new ArgumentNullException("interopCollection");

            // Add unmanaged collection items with managed wrappers
            foreach (object interopItem in interopCollection)
                Add(interopItem);
        }

        #region IDisposable

        /// <summary>
        /// Calls dispose during finalization (if it has not been called already).
        /// </summary>
        ~WiaFormatCollection()
        {
            Dispose(false);
        }

        /// <summary>
        /// Proactively frees resources.
        /// </summary>
        public void Dispose()
        {
            // Dispose
            Dispose(true);

            // Suppress finalization (it is no longer necessary)
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Frees resources.
        /// </summary>
        /// <param name="disposing">
        /// True when called from <see cref="Dispose()"/>,
        /// false when called during finalization.</param>
        void Dispose(bool disposing)
        {
            // Dispose unmanaged resources
            foreach (var item in Items)
                Marshal.ReleaseComObject(item);
            Clear();
        }

        #endregion

        #endregion
    }
}
