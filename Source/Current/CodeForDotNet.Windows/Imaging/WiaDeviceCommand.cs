using System;
using System.Runtime.InteropServices;
using Wia = Interop.Wia;

namespace CodeForDotNet.Windows.Imaging
{
    /// <summary>
    /// Managed <see cref="Wia.DeviceCommand"/>.
    /// </summary>
    public class WiaDeviceCommand : IDisposable
    {
        #region Lifetime

        /// <summary>
        /// Creates an instance to wrap the specified unmanaged object.
        /// </summary>
        internal WiaDeviceCommand(Wia.DeviceCommand deviceCommand)
        {
            _wiaDeviceCommand = deviceCommand;
        }

        #region IDisposable

        /// <summary>
        /// Calls dispose during finalization (if it has not been called already).
        /// </summary>
        ~WiaDeviceCommand()
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
            Marshal.ReleaseComObject(_wiaDeviceCommand);
        }

        #endregion

        #endregion

        #region Private Fields

        /// <summary>
        /// Unmanaged <see cref="Wia.DeviceCommand"/>.
        /// </summary>
        readonly Wia.DeviceCommand _wiaDeviceCommand;

        #endregion

        #region Public Properties

        /// <summary>
        /// ID.
        /// </summary>
        public string Id { get { return _wiaDeviceCommand.CommandID; } }

        /// <summary>
        /// Name.
        /// </summary>
        public string Name { get { return _wiaDeviceCommand.Name; } }

        /// <summary>
        /// Description.
        /// </summary>
        public string Description { get { return _wiaDeviceCommand.Description; } }

        #endregion
    }
}
