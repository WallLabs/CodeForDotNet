using CodeForDotNet.WindowsUniversal.UI;

namespace CodeForDotNet.WindowsUniversal.TestApp;

/// <summary>
/// Generic wrapper for <see cref="PageApplicationBase{ApplicationBase}"/> for this <see cref="App"/> type.
/// </summary>
/// <remarks>
/// Necessary workaround as Visual Studio does not support (and/or has designer issues) with
/// generic XAML pages in Windows Store applications (it only works properly in WPF).
/// </remarks>
public partial class AppPageBase : PageApplicationBase<App>
{
}
