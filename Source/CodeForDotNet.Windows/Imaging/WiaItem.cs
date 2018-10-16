using System;
using System.Runtime.InteropServices;
using Wia = Interop.Wia;

namespace CodeForDotNet.Windows.Imaging
{
    /// <summary>
    /// Encapsulates a <see cref="Item"/> in managed code.
    /// </summary>
    public class WiaItem : DisposableObject
    {
        #region Lifetime

        /// <summary>
        /// Creates an instance to wrap the specified unmanaged object.
        /// </summary>
        internal WiaItem(Wia.Item item)
        {
            _wiaItem = item;
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
                    if (_formats != null) _formats.Dispose();
                    if (_commands != null) _commands.Dispose();
                }

                // Dispose unmanaged resources.
                if (_wiaItem != null)
                    Marshal.ReleaseComObject(_wiaItem);
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
        /// Unmanaged <see cref="Wia.Item"/>.
        /// </summary>
        private readonly Wia.Item _wiaItem;

        #endregion Private Fields

        #region Public Properties

        /// <summary>
        /// Thread synchronization object.
        /// </summary>
        public static object SyncRoot { get; } = new object();

        /// <summary>
        /// Item identifier.
        /// </summary>
        public string Id { get { return _wiaItem.ItemID; } }

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
                            _commands = new WiaDeviceCommandCollection(_wiaItem.Commands);
                    }
                }

                // Return wrapped value
                return _commands;
            }
        }

        private WiaDeviceCommandCollection _commands;

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
                            _properties = new WiaPropertyCollection(_wiaItem.Properties);
                    }
                }

                // Return wrapped value
                return _properties;
            }
        }

        private WiaPropertyCollection _properties;

        /// <summary>
        /// Formats.
        /// </summary>
        public WiaFormatCollection Formats
        {
            get
            {
                // Create wrapper first time
                if (_formats == null)
                {
                    lock (SyncRoot)
                    {
                        if (_formats == null)
                            _formats = new WiaFormatCollection(_wiaItem.Formats);
                    }
                }

                // Return wrapped value
                return _formats;
            }
        }

        private WiaFormatCollection _formats;

        /// <summary>
        /// Item.
        /// </summary>
        public object Item { get { return _wiaItem.WiaItem; } }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Executes a command.
        /// </summary>
        public WiaItem ExecuteCommand(string commandId)
        {
            var wiaItem = _wiaItem.ExecuteCommand(commandId);
            return new WiaItem(wiaItem);
        }

        /// <summary>
        /// Transfers using the default format ID.
        /// </summary>
        /// <returns></returns>
        public object Transfer()
        {
            // Call overloaded method
            return Transfer(WiaConstants.DefaultFormatId);
        }

        /// <summary>
        /// Transfers using a specific format ID.
        /// </summary>
        public object Transfer(string formatId)
        {
            return _wiaItem.Transfer(formatId);
        }

        #endregion Public Methods
    }
}