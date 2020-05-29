using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Wia = Interop.Wia;

namespace CodeForDotNet.Windows.Imaging
{
    /// <summary>
    /// Managed enumerator for <see cref="Wia.Vector" /> items.
    /// </summary>
    public class WiaVectorEnumerator : DisposableObject, IEnumerator<WiaVector>
    {
        #region Lifetime

        /// <summary>
        /// Creates the object.
        /// </summary>
        public WiaVectorEnumerator(IEnumerator vectorEnumerator)
        {
            _wiaVectorEnumerator = vectorEnumerator;
        }

        /// <summary>
        /// Frees resources owned by this instance.
        /// </summary>
        /// <param name="disposing">
        /// True when called from <see cref="IDisposable.Dispose()"/>,
        /// false when called during finalization.
        /// </param>
        protected override void Dispose(bool disposing)
        {
            try
            {
                // Dispose unmanaged resources.
                Marshal.ReleaseComObject(_wiaVectorEnumerator);
            }
            finally
            {
                // Call base class method to fire events and set status properties.
                base.Dispose(disposing);
            }
        }

        #endregion Lifetime

        #region Private Fields

        /// <summary>
        /// WIA vector enumerator.
        /// </summary>
        private readonly IEnumerator _wiaVectorEnumerator;

        #endregion Private Fields

        #region IEnumerator Members

        /// <summary>
        /// Item at the current position.
        /// </summary>
        object IEnumerator.Current { get { return Current; } }

        /// <summary>
        /// Item at the current position.
        /// </summary>
        public WiaVector Current
        {
            get
            {
                var vector = (Wia.Vector?)_wiaVectorEnumerator.Current;
                return new WiaVector(vector!);
            }
        }

        /// <summary>
        /// Moves to the next item if available.
        /// </summary>
        /// <returns>True when moved, false when no more items exist.</returns>
        public bool MoveNext()
        {
            return _wiaVectorEnumerator.MoveNext();
        }

        /// <summary>
        /// Moves to the beginning of the collection.
        /// </summary>
        public void Reset()
        {
            _wiaVectorEnumerator.Reset();
        }

        #endregion IEnumerator Members
    }
}
