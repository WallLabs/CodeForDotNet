using System;
using System.Runtime.InteropServices;

namespace CodeForDotNet.Windows.Imaging
{
    /// <summary>
    /// Encapsulates a <see cref="Interop.Wia.Device"/> in managed code.
    /// </summary>
    public class WiaDevice : IDisposable
    {
        #region Lifetime

        /// <summary>
        /// Creates an instance to wrap the specified unmanaged object.
        /// </summary>
        internal WiaDevice(Interop.Wia.Device device)
        {
            _wiaDevice = device;
        }

        #region IDisposable

        /// <summary>
        /// Calls dispose during finalization (if it has not been called already).
        /// </summary>
        ~WiaDevice()
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
        private void Dispose(bool disposing)
        {
            // Dispose unmanaged resources
            Marshal.ReleaseComObject(_wiaDevice);
        }

        #endregion

        #endregion

        #region Private Fields

        /// <summary>
        /// Unmanaged <see cref="Interop.Wia.Device"/>.
        /// </summary>
        readonly Interop.Wia.Device _wiaDevice;

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
        public string Id { get { return _wiaDevice.DeviceID; } }

        /// <summary>
        /// Device type.
        /// </summary>
        public WiaDeviceType DeviceType { get { return (WiaDeviceType)(int)_wiaDevice.Type; } }

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
                            _properties = new WiaPropertyCollection(_wiaDevice.Properties);
                    }
                }

                // Return wrapped value
                return _properties;
            }
        }
        WiaPropertyCollection _properties;

        /// <summary>
        /// Commands.
        /// </summary>
        public WiaDeviceCommandCollection Commands
        {
            get
            {
                // Create wrapper first time
                if (_commands == null)
                {
                    lock (_syncRoot)
                    {
                        if (_commands == null)
                            _commands = new WiaDeviceCommandCollection(_wiaDevice.Commands);
                    }
                }

                // Return wrapped value
                return _commands;
            }
        }
        WiaDeviceCommandCollection _commands;

        /// <summary>
        /// Events.
        /// </summary>
        public WiaDeviceEventCollection Events
        {
            get
            {
                // Create wrapper first time
                if (_events == null)
                {
                    lock (_syncRoot)
                    {
                        if (_events == null)
                            _events = new WiaDeviceEventCollection(_wiaDevice.Events);
                    }
                }

                // Return wrapped value
                return _events;
            }
        }
        WiaDeviceEventCollection _events;

        /// <summary>
        /// Items.
        /// </summary>
        public WiaItemCollection Items
        {
            get
            {
                // Create wrapper first time
                if (_items == null)
                {
                    lock (_syncRoot)
                    {
                        if (_items == null)
                            _items = new WiaItemCollection(_wiaDevice.Items);
                    }
                }

                // Return wrapped value
                return _items;
            }
        }
        WiaItemCollection _items;

        /// <summary>
        /// Item.
        /// </summary>
        public object Item { get { return _wiaDevice.WiaItem; } }

        #endregion

        #region Public Methods

        /// <summary>
        /// Executes a command.
        /// </summary>
        public WiaItem ExecuteCommand(string commandId)
        {
            var wiaItem = _wiaDevice.ExecuteCommand(commandId);
            return new WiaItem(wiaItem);
        }

        /// <summary>
        /// Gets the item with the specified ID.
        /// </summary>
        public WiaItem GetItemById(string itemId)
        {
            var wiaItem = _wiaDevice.GetItem(itemId);
            return new WiaItem(wiaItem);
        }

        #endregion
    }
}
