using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

#nullable enable

namespace CodeForDotNet.WindowsUniversal.UI
{
    /// <summary>
    /// Windows Store <see cref="Page"/> with common features including a strongly typed model and session state persistence when used in a
    /// <see cref="ApplicationBase"/> based application.
    /// </summary>
    public partial class PageModelBase<TApplication, TModel> : PageApplicationBase<TApplication>
        where TApplication : ApplicationBase
        where TModel : class
    {
        #region Public Properties

        /// <summary>
        /// View model.
        /// </summary>
        public new TModel? Model { get { return (TModel?)base.Model; } }

        #endregion Public Properties

        #region Protected Methods

        /// <summary>
        /// Creates the <see cref="Model"/>.
        /// </summary>
        protected virtual object? CreateModel()
        {
            return null;
        }

        /// <summary>
        /// Performs page specific setup when navigated to.
        /// </summary>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // Call base class method
            base.OnNavigatedTo(e);

            // Set model from application
            DataContext = base.Model = CreateModel();
        }

        #endregion Protected Methods
    }
}
