using CodeForDotNet.WindowsUniversal.Tests.Models;
using CodeForDotNet.WindowsUniversal.UI;
using System;

namespace CodeForDotNet.WindowsUniversal.Tests.Views
{
    /// <summary>
    /// Generic base class of the <see cref="ControlTestPage"/>.
    /// </summary>
    /// <remarks>
    /// Necessary workaround as Visual Studio does not support (and/or has designer issues) with
    /// generic XAML pages in Windows Store applications (it only works properly in WPF).
    /// </remarks>
    public partial class ControlTestPageBase : PageModelBase<App, ControlTestUIModel>
    {
        /// <summary>
        /// Creates the <see cref="PageModelBase{App,Page}.Model"/>.
        /// </summary>
        protected override object CreateModel()
        {
            return new ControlTestUIModel();
        }
    }
}
