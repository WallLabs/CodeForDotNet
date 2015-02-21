using System;
using System.Runtime.InteropServices;

namespace CodeForDotNet.Windows.Imaging
{
    /// <summary>
    /// Encapsulates a <see cref="Interop.Wia.Item"/> in managed code.
    /// </summary>
    public class WiaItem : IDisposable
    {
        #region Lifetime

        /// <summary>
        /// Creates an instance to wrap the specified unmanaged object.
        /// </summary>
        internal WiaItem(Interop.Wia.Item item)
        {
            _wiaItem = item;
        }

        #region IDisposable

        /// <summary>
        /// Calls dispose during finalization (if it has not been called already).
        /// </summary>
        ~WiaItem()
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
            Marshal.ReleaseComObject(_wiaItem);
        }

        #endregion

        #endregion

        #region Private Fields

        /// <summary>
        /// Unmanaged <see cref="Interop.Wia.Item"/>.
        /// </summary>
        readonly Interop.Wia.Item _wiaItem;

        #endregion

        #region Public Properties

        /// <summary>
        /// Thread synchronization object.
        /// </summary>
        public object SyncRoot { get { return _syncRoot; } }
        static readonly object _syncRoot = new object();

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
                    lock (_syncRoot)
                    {
                        if (_commands == null)
                            _commands = new WiaDeviceCommandCollection(_wiaItem.Commands);
                    }
                }

                // Return wrapped value
                return _commands;
            }
        }
        WiaDeviceCommandCollection _commands;

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
                            _properties = new WiaPropertyCollection(_wiaItem.Properties);
                    }
                }

                // Return wrapped value
                return _properties;
            }
        }
        WiaPropertyCollection _properties;

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
                    lock (_syncRoot)
                    {
                        if (_formats == null)
                            _formats = new WiaFormatCollection(_wiaItem.Formats);
                    }
                }

                // Return wrapped value
                return _formats;
            }
        }
        WiaFormatCollection _formats;

        /// <summary>
        /// Item.
        /// </summary>
        public object Item { get { return _wiaItem.WiaItem; } }

        #endregion

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

        #endregion
    }
}
