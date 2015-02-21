using System.Linq;

namespace CodeForDotNet.Windows.Imaging
{
    /// <summary>
    /// Encapsulates a <see cref="Interop.Wia.DeviceInfo"/> in managed code.
    /// </summary>
    public class WiaDeviceInfo
    {
        #region Lifetime

        /// <summary>
        /// Creates an instance to wrap the specified unmanaged object.
        /// </summary>
        internal WiaDeviceInfo(Interop.Wia.DeviceInfo deviceInfo)
        {
            _wiaDeviceInfo = deviceInfo;
        }

        #endregion

        #region Private Fields

        /// <summary>
        /// Unmanaged <see cref="Interop.Wia.DeviceInfo"/>.
        /// </summary>
        readonly Interop.Wia.DeviceInfo _wiaDeviceInfo;

        #endregion

        #region Public Properties

        /// <summary>
        /// Thread synchronization object.
        /// </summary>
        public object SyncRoot { get { return _syncRoot; } }
        static readonly object _syncRoot = new object();

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
        WiaPropertyCollection _properties;

        #endregion

        #region Public Methods

        /// <summary>
        /// Creates a connection to this device.
        /// </summary>
        public WiaDevice Connect()
        {
            var wiaDevice = _wiaDeviceInfo.Connect();
            return new WiaDevice(wiaDevice);
        }

        #endregion
    }
}
