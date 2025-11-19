using CodeForDotNet.WindowsUniversal.Tests.Views;
using Microsoft.VisualStudio.TestPlatform.TestExecutor;
using System;
using Windows.ApplicationModel.Activation;

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
            this.Suspending += OnSuspending;
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user. Other entry points will be used when the application is launched to open a
        /// specific file, to display search results, and so forth.
        /// </summary>
        /// <param name="event">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs @event)
        {

            UnitTestClient.CreateDefaultUI();
            base.OnLaunched(@event);
            UnitTestClient.Run(@event.Arguments);
        }

        #endregion
    }
}
