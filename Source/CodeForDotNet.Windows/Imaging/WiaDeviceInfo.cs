using System;
using System.Linq;
using System.Runtime.InteropServices;
using Wia = Interop.Wia;

namespace CodeForDotNet.Windows.Imaging
{
    /// <summary>
    /// Encapsulates a <see cref="Wia.DeviceInfo"/> in managed code.
    /// </summary>
    public class WiaDeviceInfo : DisposableObject
    {
        #region Lifetime

        /// <summary>
        /// Creates an instance to wrap the specified unmanaged object.
        /// </summary>
        internal WiaDeviceInfo(Wia.DeviceInfo deviceInfo)
        {
            _wiaDeviceInfo = deviceInfo;
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
                // Dispose managed resources.
                if (disposing)
                {
                    if (_properties != null) _properties.Dispose();
                }

                // Dispose unmanaged resources.
                if (_wiaDeviceInfo != null)
                    Marshal.ReleaseComObject(_wiaDeviceInfo);
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
        /// Unmanaged <see cref="Wia.DeviceInfo"/>.
        /// </summary>
        private readonly Wia.DeviceInfo _wiaDeviceInfo;

        #endregion Private Fields

        #region Public Properties

        /// <summary>
        /// Thread synchronization object.
        /// </summary>
        public object SyncRoot { get { return _syncRoot; } }

        private static readonly object _syncRoot = new object();

        /// <summary>
        /// Device identifier.
        /// </summary>
        public string Id { get { return _wiaDeviceInfo.DeviceID; } }

        /// <summary>
        /// Name.
        /// </summary>
        public string Name
        {
            get
            {
                return (from property in Properties
                        where property.Name == "Name"
                        select (string)property.Value).FirstOrDefault();
            }
        }

        /// <summary>
        /// Description.
        /// </summary>
        public string Description
        {
            get
            {
                return (from property in Properties
                        where property.Name == "Description"
                        select (string)property.Value).FirstOrDefault();
            }
        }

        /// <summary>
        /// Device type.
        /// </summary>
        public WiaDeviceType DeviceType { get { return (WiaDeviceType)(int)_wiaDeviceInfo.Type; } }

        /// <summary>
        /// Properties.
        /// </summary>
        public WiaPropertyCollection Properties
        {
            get
            {
                // Create wrapper first time
                if (_properties == null)
                {
                    lock (_syncRoot)
                    {
                        if (_properties == null)
                            _properties = new WiaPropertyCollection(_wiaDeviceInfo.Properties);
                    }
                }

                // Return wrapped value
                return _properties;
            }
        }

        private WiaPropertyCollection _properties;

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Creates a connection to this device.
        /// </summary>
        public WiaDevice Connect()
        {
            var wiaDevice = _wiaDeviceInfo.Connect();
            return new WiaDevice(wiaDevice);
        }

        #endregion Public Methods
    }
}