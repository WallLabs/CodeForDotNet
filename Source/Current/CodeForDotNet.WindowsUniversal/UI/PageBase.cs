using System;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace CodeForDotNet.WindowsUniversal.UI
{
    /// <summary>
    /// Windows Store <see cref="Page"/> with common features including a
    /// standard model property and session state persistence when used in
    /// a <see cref="Application"/> based application.
    /// </summary>
    [CLSCompliant(false)]
    public class PageBase : Page
    {
        #region Lifetime

        /// <summary>
        /// Creates an instance.
        /// </summary>
        protected PageBase()
        {
            // Initialize members
            Application = Application.Current;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Application.
        /// </summary>
        public Application Application { get; private set; }

        /// <summary>
        /// <see cref="Model"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ModelProperty =
            DependencyProperty.Register("Model", typeof(object), typeof(PageBase), null);

        /// <summary>
        /// View model.
        /// </summary>
        public object Model
        {
            get { return GetValue(ModelProperty); }
            protected set { SetValue(ModelProperty, value); }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Saves state to the application data container, e.g. during application suspension.
        /// </summary>
        public void SaveState(ApplicationDataContainer container)
        {
            // TODO: Use model key from UI model based class
            var modelKey = Model.GetType().FullName + "." + Name;
            container.Values[modelKey] = Model;
        }

        /// <summary>
        /// Loads state from the application data container, e.g. during launch/resume.
        /// </summary>
        public void LoadState(ApplicationDataContainer container)
        {
            // TODO: Use model key from UI model based class
            var modelKey = Model.GetType().FullName + "." + Name;
            Model = container.Values[modelKey];
        }

        #endregion
    }
}
