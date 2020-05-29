using CodeForDotNet.WindowsUniversal.Tests.Views;
using System;

namespace CodeForDotNet.WindowsUniversal.Tests
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public sealed partial class App
    {
        #region Lifetime

        /// <summary>
        /// Creates an instance.
        /// </summary>
        public App() : base(typeof(ControlTestPage))
        {
            InitializeComponent();
        }

        #endregion
    }
}
