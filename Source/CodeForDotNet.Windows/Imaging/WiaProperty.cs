using System;
using System.Runtime.InteropServices;
using Wia = Interop.Wia;

namespace CodeForDotNet.Windows.Imaging
{
    /// <summary>
    /// Managed <see cref="Wia.Property"/>.
    /// </summary>
    public class WiaProperty : DisposableObject
    {
        #region Lifetime

        /// <summary>
        /// Creates an instance to wrap the specified unmanaged object.
        /// </summary>
        internal WiaProperty(Wia.Property property)
        {
            _wiaProperty = property;
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
                Marshal.ReleaseComObject(_wiaProperty);
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
        /// Unmanaged <see cref="Wia.Property"/>.
        /// </summary>
        private readonly Wia.Property _wiaProperty;

        #endregion Private Fields

        #region Public Properties

        /// <summary>
        /// Thread synchronization object.
        /// </summary>
        public object SyncRoot { get { return _syncRoot; } }

        private static readonly object _syncRoot = new object();

        /// <summary>
        /// Indicates the property is read-only.
        /// </summary>
        public bool IsReadOnly { get { return _wiaProperty.IsReadOnly; } }

        /// <summary>
        /// Indicates the property is a vector.
        /// </summary>
        public bool IsVector { get { return _wiaProperty.IsVector; } }

        /// <summary>
        /// Name.
        /// </summary>
        public string Name { get { return _wiaProperty.Name; } }

        /// <summary>
        /// ID.
        /// </summary>
        public int Id { get { return _wiaProperty.PropertyID; } }

        /// <summary>
        /// WIA sub-type.
        /// </summary>
        public WiaSubtype Subtype { get { return (WiaSubtype)(int)_wiaProperty.SubType; } }

        /// <summary>
        /// WIA sub-type default value.
        /// </summary>
        public object SubtypeDefault { get { return _wiaProperty.SubTypeDefault; } }

        /// <summary>
        /// WIA sub-type maximum value.
        /// </summary>
        public int SubtypeMax { get { return _wiaProperty.SubTypeMax; } }

        /// <summary>
        /// WIA sub-type minimum.
        /// </summary>
        public int SubtypeMin { get { return _wiaProperty.SubTypeMin; } }

        /// <summary>
        /// WIA sub-type increment or decrement step.
        /// </summary>
        public int SubtypeStep { get { return _wiaProperty.SubTypeStep; } }

        /// <summary>
        /// WIA sub-type array values.
        /// </summary>
        public WiaVector WiaSubtypeValues
        {
            get
            {
                // Create wrapper first time
                if (_wiaSubtypeValues == null)
                {
                    lock (_syncRoot)
                    {
                        if (_wiaSubtypeValues == null)
                            _wiaSubtypeValues = new WiaVector(_wiaProperty.SubTypeValues);
                    }
                }

                // Return wrapped value
                return _wiaSubtypeValues;
            }
        }

        private WiaVector _wiaSubtypeValues;

        /// <summary>
        /// Property type.
        /// </summary>
        public int PropertyType { get { return _wiaProperty.Type; } }

        /// <summary>
        /// Property value.
        /// </summary>
        public object Value
        {
            get { return _wiaProperty.get_Value(); }
            set { _wiaProperty.set_Value(ref value); }
        }

        #endregion Public Properties
    }
}