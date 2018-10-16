using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using Wia = Interop.Wia;

namespace CodeForDotNet.Windows.Imaging
{
    /// <summary>
    /// Encapsulates a <see cref="Wia.Device"/> in managed code.
    /// </summary>
    public class WiaDevice : DisposableObject
    {
        #region Lifetime

        /// <summary>
        /// Creates an instance to wrap the specified unmanaged object.
        /// </summary>
        internal WiaDevice(Wia.Device device)
        {
            _wiaDevice = device;
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
                    if (_item != null) _item.Dispose();
                    if (_items != null) _items.Dispose();
                    if (_properties != null) _properties.Dispose();
                    if (_events != null) _events.Dispose();
                    if (_commands != null) _commands.Dispose();
                }

                // Dispose unmanaged resources.
                Marshal.ReleaseComObject(_wiaDevice);
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
        /// Unmanaged <see cref="Wia.Device"/>.
        /// </summary>
        private readonly Wia.Device _wiaDevice;

        #endregion Private Fields

        #region Public Properties

        /// <summary>
        /// Thread synchronization object.
        /// </summary>
        public static object SyncRoot { get; } = new object();

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
                    lock (SyncRoot)
                    {
                        if (_properties == null)
                            _properties = new WiaPropertyCollection(_wiaDevice.Properties);
                    }
                }

                // Return wrapped value
                return _properties;
            }
        }

        private WiaPropertyCollection _properties;

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
                    lock (SyncRoot)
                    {
                        if (_commands == null)
                            _commands = new WiaDeviceCommandCollection(_wiaDevice.Commands);
                    }
                }

                // Return wrapped value
                return _commands;
            }
        }

        private WiaDeviceCommandCollection _commands;

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
                    lock (SyncRoot)
                    {
                        if (_events == null)
                            _events = new WiaDeviceEventCollection(_wiaDevice.Events);
                    }
                }

                // Return wrapped value
                return _events;
            }
        }

        private WiaDeviceEventCollection _events;

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
                    lock (SyncRoot)
                    {
                        if (_items == null)
                            _items = new WiaItemCollection(_wiaDevice.Items);
                    }
                }

                // Return wrapped value
                return _items;
            }
        }

        private WiaItemCollection _items;

        /// <summary>
        /// Item.
        /// </summary>
        public WiaItem Item
        {
            get
            {
                // Create wrapper first time
                if (_item == null)
                {
                    lock (SyncRoot)
                    {
                        if (_item == null)
                            _item = new WiaItem((Wia.Item)_wiaDevice.WiaItem);
                    }
                }

                // Return wrapped value
                return _item;
            }
        }

        private WiaItem _item;

        #endregion Public Properties

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

        #endregion Public Methods
    }
}