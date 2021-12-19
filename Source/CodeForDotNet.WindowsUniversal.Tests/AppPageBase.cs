using CodeForDotNet.WindowsUniversal.UI;
using System;

namespace CodeForDotNet.WindowsUniversal.Tests
{
    /// <summary>
    /// Generic wrapper for <see cref="PageApplicationBase{ApplicationBase}"/> for this <see cref="App"/> type.
    /// </summary>
    /// <remarks>
    /// Necessary workaround as Visual Studio does not support (and/or has designer issues) with
    /// generic XAML pages in Windows Store applications (it only works properly in WPF).
    /// </remarks>
    [CLSCompliant(false)]
    public class AppPageBase : PageApplicationBase<App>
    {
    }
}
