using Windows.UI.Xaml.Controls;

namespace CodeForDotNet.WindowsUniversal.UI;

/// <summary>
/// Windows Store <see cref="Page"/> with common features including a
/// strongly typed model and session state persistence when used in
/// a <see cref="ApplicationBase"/> based application.
/// </summary>
public partial class PageApplicationBase<TApplication> : PageBase
    where TApplication : ApplicationBase
{
    #region Public Properties

    /// <summary>
    /// Application.
    /// </summary>
    public new TApplication Application => (TApplication)base.Application;

    #endregion
}
