using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeForDotNet.Windows.Imaging
{
    /// <summary>
    /// Managed <see cref="Interop.Wia.WiaEventFlag"/>.
    /// </summary>
    public enum WiaDeviceEventType
    {
        /// <summary>
        /// Notification event.
        /// </summary>
        Notification = 1,

        /// <summary>
        /// Action event.
        /// </summary>
        ActionEvent = 2
    }
}
