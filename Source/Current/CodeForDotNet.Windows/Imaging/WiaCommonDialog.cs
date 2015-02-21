﻿using System;
using System.Runtime.InteropServices;

namespace CodeForDotNet.Windows.Imaging
{
    /// <summary>
    /// Encapsulates a <see cref="Interop.Wia.CommonDialog"/> in managed code.
    /// </summary>
    public class WiaCommonDialog : IDisposable
    {
        #region Lifetime

        /// <summary>
        /// Creates an instance to wrap the specified unmanaged object.
        /// </summary>
        public WiaCommonDialog()
        {
            _wiaCommonDialog = new Interop.Wia.CommonDialog();
        }

        #region IDisposable

        /// <summary>
        /// Calls dispose during finalization (if it has not been called already).
        /// </summary>
        ~WiaCommonDialog()
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
            Marshal.ReleaseComObject(_wiaCommonDialog);
        }

        #endregion

        #endregion

        #region Private Fields

        /// <summary>
        /// Unmanaged <see cref="Interop.Wia.CommonDialog"/>.
        /// </summary>
        readonly Interop.Wia.CommonDialog _wiaCommonDialog;

        #endregion

        #region Public Methods

        /// <summary>
        /// Shows the common "Acquire Image" dialog to acquire an image.
        /// </summary>
        /// <returns>Image file or null when cancelled.</returns>
        public WiaImageFile ShowAcquireImage()
        {
            // Call overloaded method with defaults
            return ShowAcquireImage(WiaDeviceType.Unspecified, WiaImageIntent.Color, WiaImageBias.MaximizeQuality, WiaConstants.DefaultFormatId, false, true, false);
        }

        /// <summary>
        /// Shows the common "Acquire Image" dialog to acquire an image.
        /// </summary>
        /// <param name="deviceType">Device type.</param>
        /// <param name="intent">Image intent.</param>
        /// <param name="bias">Image bias.</param>
        /// <param name="formatId">Image format ID.</param>
        /// <param name="alwaysSelectDevice">Always prompt to select device.</param>
        /// <param name="useCommonUI">Use common UI.</param>
        /// <param name="cancelError">Generate error if cancelled.</param>
        /// <returns>Image file or null when cancelled.</returns>
        public WiaImageFile ShowAcquireImage(WiaDeviceType deviceType, WiaImageIntent intent, WiaImageBias bias, string formatId, bool alwaysSelectDevice, bool useCommonUI, bool cancelError)
        {
            var wiaImageFile = _wiaCommonDialog.ShowAcquireImage(
                (Interop.Wia.WiaDeviceType)(int)deviceType,
                (Interop.Wia.WiaImageIntent)(int)intent,
                (Interop.Wia.WiaImageBias)(int)bias,
                formatId, alwaysSelectDevice, useCommonUI, cancelError);
            return wiaImageFile != null ? new WiaImageFile(wiaImageFile) : null;
        }

        #endregion
    }
}
