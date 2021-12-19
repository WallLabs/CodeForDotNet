using CodeForDotNet.Data;
using CodeForDotNet.WindowsUniversal.Storage;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Storage;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

#nullable enable

namespace CodeForDotNet.WindowsUniversal.UI
{
    /// <summary>
    /// Windows Store application with common features including a strongly typed application model, session state persistence and error reporting.
    /// </summary>
    [CLSCompliant(false)]
    public abstract class ApplicationBase : Application
    {
        #region Protected Constructors

        /// <summary>
        /// Design-time constructor only.
        /// </summary>
        /// <param name="errorReportUri">URI where <see cref="ErrorReportData"/> will be posted.</param>
        protected ApplicationBase(Uri errorReportUri)
        {
            // Initialize members
            ErrorReportUri = errorReportUri;
        }

        /// <summary>
        /// Creates an instance.
        /// </summary>
        protected ApplicationBase(Type startPageType)
        {
            // Initialize members
            StartPageType = startPageType;
            Pages = new Collection<PageBase>();

            // Hook events
            Suspending += OnSuspending;
            Resuming += OnResuming;
            UnhandledException += OnError;
        }

        #endregion Protected Constructors

        #region Public Properties

        /// <summary>
        /// URI where <see cref="ErrorReportData"/> will be posted.
        /// </summary>
        public Uri? ErrorReportUri { get; private set; }

        /// <summary>
        /// Currently loaded pages, managed by this application, e.g. their state is saved and restored together with the application.
        /// </summary>
        public Collection<PageBase>? Pages { get; private set; }

        /// <summary>
        /// Start page type, used for navigation.
        /// </summary>
        public Type? StartPageType { get; protected set; }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Checks if any errors occurred then displays a prompt for the user to send an email.
        /// </summary>
        public async void CheckErrors()
        {
            // Check for errors (do nothing when none)
            var errors = LocalErrorStore.List();
            if (errors == null || errors.Length == 0)
            {
                return;
            }

            // Prompt user
            await ShowSendErrorsDialog().ConfigureAwait(false);
        }

        /// <summary>
        /// Loads any saved state of the entire application including any previously loaded pages.
        /// </summary>
        public virtual void LoadState()
        {
            // Open local storage container
            var localSettings = ApplicationData.Current.LocalSettings;

            // TODO: Detect which pages were loaded and re-create them

            // Load page data
            if (Pages is not null)
            {
                foreach (var page in Pages)
                    page.LoadState(localSettings);
            }
        }

        /// <summary>
        /// Saves state of the entire application including any loaded pages.
        /// </summary>
        public virtual void SaveState()
        {
            // Open local storage container
            var localSettings = ApplicationData.Current.LocalSettings;

            // TODO: Save application data (e.g. list of loaded page types and keys/names)

            // Save page data
            if (Pages is not null)
            {
                foreach (var page in Pages)
                    page.SaveState(localSettings);
            }
        }

        #endregion Public Methods

        #region Protected Methods

        /// <summary>
        /// Discards all errors.
        /// </summary>
        protected static void DiscardErrors()
        {
            // Check for errors (do nothing when none)
            var errors = LocalErrorStore.List();
            if (errors == null)
            {
                return;
            }

            // Delete all errors
            foreach (var errorFileName in errors)
            {
                LocalErrorStore.Remove(errorFileName);
            }
        }

        /// <summary>
        /// Logs unhandled errors.
        /// </summary>
        protected static void OnError(object sender, Windows.UI.Xaml.UnhandledExceptionEventArgs @event)
        {
            // Validate.
            if (@event is null) throw new ArgumentNullException(nameof(@event));

            // Store error to send later
            LocalErrorStore.Add(@event.Exception);
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user. Other entry points will be used when the application is launched to open a
        /// specific file, to display search results, and so forth.
        /// </summary>
        /// <param name="event">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs @event)
        {
            // Validate
            if (@event == null) throw new ArgumentNullException(nameof(@event));

            // Do not repeat initialization when the Window already has content, just ensure that the window is active
            if (Window.Current.Content is not Frame rootFrame)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();
                rootFrame.NavigationFailed += OnNavigationFailed;

                if (@event.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    // TODO: Load state from previously suspended application
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            // When the navigation stack isn't restored navigate to the first page, configuring the new page by passing required information as a navigation parameter
            if (rootFrame.Content == null)
            {
                if (!rootFrame.Navigate(StartPageType, @event.Arguments))
                    throw new InvalidOperationException(AssemblyResources.ApplicationBaseOnLaunchedErrorCreatePage);
            }

            // Ensure the current window is active
            Window.Current.Activate();

            // Check for pending errors
            CheckErrors();
        }

        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// Saves state when the application is suspended.
        /// </summary>
        protected void OnSuspending(object sender, SuspendingEventArgs @event)
        {
            // Validate.
            if (@event is null) throw new ArgumentNullException(nameof(@event));

            // Delay
            var deferral = @event.SuspendingOperation.GetDeferral();
            try
            {
                // Save state
                SaveState();
            }
            catch (Exception error)
            {
                // Log error saving state then continue
                LocalErrorStore.Add(error);
            }
            finally
            {
                // End delay
                deferral.Complete();
            }
        }

        /// <summary>
        /// Loads state when the application is resumed.
        /// </summary>
        protected void OnResuming(object sender, object @event)
        {
            try
            {
                // Load state
                LoadState();
            }
            catch (Exception error)
            {
                // Log error restoring state then create new model
                LocalErrorStore.Add(error);
            }
        }

        /// <summary>
        /// Reports outstanding errors to the web service.
        /// </summary>
        protected void SendErrors()
        {
            // Check for errors (do nothing when none)
            var errorFiles = LocalErrorStore.List();
            if (errorFiles == null || errorFiles.Length == 0)
            {
                return;
            }

            // Report all errors
            foreach (var errorFile in errorFiles)
            {
                // Get next error
                var error = LocalErrorStore.Get(errorFile);

                // Post error to service
                using var client = new HttpClient();
                using var buffer = new MemoryStream();
                var serializer = new DataContractJsonSerializer(typeof(ErrorReportData[]));
                serializer.WriteObject(buffer, error);
                var json = Encoding.UTF8.GetString(buffer.ToArray(), 0, (int)buffer.Length);
                using var content = new StringContent(json, Encoding.UTF8, "application/json");
                using var response = client.PostAsync(ErrorReportUri, content).Result;
                if (response.IsSuccessStatusCode || response.StatusCode == HttpStatusCode.BadRequest)
                {
                    // Remove when successful or invalid data
                    LocalErrorStore.Remove(errorFile);
                }
            }
        }

        /// <summary>
        /// Displays a dialog which prompts the user to send error reports.
        /// </summary>
        protected async Task ShowSendErrorsDialog()
        {
            var dialog = new MessageDialog((string)Resources["ErrorReportDialogText"],
                (string)Resources["ErrorReportDialogCaption"]);
            var now = new UICommand((string)Resources["ErrorReportDialogNowButtonText"], command => SendErrors());
            var later = new UICommand((string)Resources["ErrorReportDialogLaterButtonText"]);
            var discard = new UICommand((string)Resources["ErrorReportDialogDiscardButtonText"],
                command => DiscardErrors());
            dialog.Commands.Add(now);
            dialog.Commands.Add(later);
            dialog.Commands.Add(discard);
            dialog.DefaultCommandIndex = 0;
            await dialog.ShowAsync();
        }

        #endregion Protected Methods
    }
}
