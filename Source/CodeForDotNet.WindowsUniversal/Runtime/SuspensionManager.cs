using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace CodeForDotNet.WindowsUniversal.Runtime
{
   /// <summary>
   /// SuspensionManager captures global session state to simplify process lifetime management for an
   /// application. Note that session state will be automatically cleared under a variety of
   /// conditions and should only be used to store information that would be convenient to carry
   /// across sessions, but that should be discarded when an application crashes or is upgraded.
   /// </summary>
   public static class SuspensionManager
   {
      #region Private Fields

      private const string SessionStateFilename = "_sessionState.xml";

      private static readonly List<WeakReference<Frame>> _registeredFrames = new List<WeakReference<Frame>>();

      private static readonly DependencyProperty FrameSessionStateKeyProperty =
                        DependencyProperty.RegisterAttached("_FrameSessionStateKey", typeof(string), typeof(SuspensionManager), null);

      private static readonly DependencyProperty FrameSessionStateProperty =
                  DependencyProperty.RegisterAttached("_FrameSessionState", typeof(Dictionary<string, object>), typeof(SuspensionManager), null);

      #endregion Private Fields

      #region Public Properties

      /// <summary>
      /// List of custom types provided to the <see cref="DataContractSerializer"/> when reading and
      /// writing session state. Initially empty, additional types may be added to customize the
      /// serialization process.
      /// </summary>
      public static List<Type> KnownTypes { get; } = new List<Type>();

      /// <summary>
      /// Provides access to global session state for the current session. This state is serialized
      /// by <see cref="SaveAsync"/> and restored by <see cref="RestoreAsync"/>, so values must be
      /// serializable by <see cref="DataContractSerializer"/> and should be as compact as possible.
      /// Strings and other self-contained data types are strongly recommended.
      /// </summary>
      public static Dictionary<string, object> SessionState { get; private set; } = new Dictionary<string, object>();

      #endregion Public Properties

      #region Public Methods

      /// <summary>
      /// Registers a <see cref="Frame"/> instance to allow its navigation history to be saved to and
      /// restored from <see cref="SessionState"/>. Frames should be registered once immediately
      /// after creation if they will participate in session state management. Upon registration if
      /// state has already been restored for the specified key the navigation history will
      /// immediately be restored. Subsequent invocations of <see cref="RestoreAsync"/> will also
      /// restore navigation history.
      /// </summary>
      /// <param name="frame">An instance whose navigation history should be managed by <see cref="SuspensionManager"/></param>
      /// <param name="sessionStateKey">
      /// A unique key into <see cref="SessionState"/> used to store navigation-related information.
      /// </param>
      [CLSCompliant(false)]
      public static void RegisterFrame(Frame frame, string sessionStateKey)
      {
         // Validate
         if (frame == null) throw new ArgumentNullException(nameof(frame));
         if (sessionStateKey == null) throw new ArgumentNullException(nameof(sessionStateKey));

         if (frame.GetValue(FrameSessionStateKeyProperty) != null)
         {
            throw new InvalidOperationException("Frames can only be registered to one session state key");
         }

         if (frame.GetValue(FrameSessionStateProperty) != null)
         {
            throw new InvalidOperationException("Frames must be either be registered before accessing frame session state, or not registered at all");
         }

         // Use a dependency property to associate the session key with a frame, and keep a list of
         // frames whose navigation state should be managed
         frame.SetValue(FrameSessionStateKeyProperty, sessionStateKey);
         _registeredFrames.Add(new WeakReference<Frame>(frame));

         // Check to see if navigation state can be restored
         RestoreFrameNavigationState(frame);
      }

      /// <summary>
      /// Restores previously saved <see cref="SessionState"/>. Any <see cref="Frame"/> instances
      /// registered with <see cref="RegisterFrame"/> will also restore their prior navigation state,
      /// which in turn gives their active <see cref="Page"/> an opportunity restore its state.
      /// </summary>
      /// <returns>
      /// An asynchronous task that reflects when session state has been read. The content of
      /// <see cref="SessionState"/> should not be relied upon until this task completes.
      /// </returns>
      [SuppressMessage("Microsoft.Design", "CA1031", Justification = "Errors are re-thrown with context.")]
      public static async Task RestoreAsync()
      {
         SessionState = new Dictionary<string, object>();

         try
         {
            // Get the input stream for the SessionState file
            var file = await ApplicationData.Current.LocalFolder.GetFileAsync(SessionStateFilename);
            using (var inStream = await file.OpenSequentialReadAsync())
            {
               // Deserialize the Session State
               var serializer = new DataContractSerializer(typeof(Dictionary<string, object>), KnownTypes);
               SessionState = (Dictionary<string, object>)serializer.ReadObject(inStream.AsStreamForRead());
            }

            // Restore any registered frames to their saved state
            foreach (var weakFrameReference in _registeredFrames)
            {
               if (weakFrameReference.TryGetTarget(out var frame))
               {
                  frame.ClearValue(FrameSessionStateProperty);
                  RestoreFrameNavigationState(frame);
               }
            }
         }
         catch (Exception error)
         {
            throw new SuspensionManagerException("Failed to restore state.", error);
         }
      }

      /// <summary>
      /// Save the current <see cref="SessionState"/>. Any <see cref="Frame"/> instances registered
      /// with <see cref="RegisterFrame"/> will also preserve their current navigation stack, which
      /// in turn gives their active <see cref="Page"/> an opportunity to save its state.
      /// </summary>
      /// <returns>An asynchronous task that reflects when session state has been saved.</returns>
      [SuppressMessage("Microsoft.Design", "CA1031", Justification = "Errors are re-thrown with context.")]
      public static async Task SaveAsync()
      {
         try
         {
            // Save the navigation state for all registered frames
            foreach (var weakFrameReference in _registeredFrames)
            {
               if (weakFrameReference.TryGetTarget(out var frame))
                  SaveFrameNavigationState(frame);
            }

            // Serialize the session state synchronously to avoid asynchronous access to shared state
            var sessionData = new MemoryStream();
            var serializer = new DataContractSerializer(typeof(Dictionary<string, object>), KnownTypes);
            serializer.WriteObject(sessionData, SessionState);

            // Get an output stream for the SessionState file and write the state asynchronously
            var file = await ApplicationData.Current.LocalFolder.CreateFileAsync(SessionStateFilename, CreationCollisionOption.ReplaceExisting);
            using (var fileStream = await file.OpenStreamForWriteAsync().ConfigureAwait(false))
            {
               sessionData.Seek(0, SeekOrigin.Begin);
               await sessionData.CopyToAsync(fileStream).ConfigureAwait(false);
               await fileStream.FlushAsync().ConfigureAwait(false);
            }
         }
         catch (Exception error)
         {
            throw new SuspensionManagerException("Failed to save state.", error);
         }
      }

      /// <summary>
      /// Provides storage for session state associated with the specified <see cref="Frame"/>.
      /// Frames that have been previously registered with <see cref="RegisterFrame"/> have their
      /// session state saved and restored automatically as a part of the global
      /// <see cref="SessionState"/>. Frames that are not registered have transient state that can
      /// still be useful when restoring pages that have been discarded from the navigation cache.
      /// </summary>
      /// <param name="frame">The instance for which session state is desired.</param>
      /// <returns>A collection of state subject to the same serialization mechanism as <see cref="SessionState"/>.</returns>
      [CLSCompliant(false)]
      public static Dictionary<string, object> SessionStateForFrame(Frame frame)
      {
         // Validate
         if (frame == null) throw new ArgumentNullException(nameof(frame));

         var frameState = (Dictionary<string, object>)frame.GetValue(FrameSessionStateProperty);

         if (frameState == null)
         {
            var frameSessionKey = (string)frame.GetValue(FrameSessionStateKeyProperty);
            if (frameSessionKey != null)
            {
               // Registered frames reflect the corresponding session state
               if (!SessionState.ContainsKey(frameSessionKey))
               {
                  SessionState[frameSessionKey] = new Dictionary<string, object>();
               }
               frameState = (Dictionary<string, object>)SessionState[frameSessionKey];
            }
            else
            {
               // Frames that aren't registered have transient state
               frameState = new Dictionary<string, object>();
            }
            frame.SetValue(FrameSessionStateProperty, frameState);
         }
         return frameState;
      }

      /// <summary>
      /// Disassociates a <see cref="Frame"/> previously registered by <see cref="RegisterFrame"/>
      /// from <see cref="SessionState"/>. Any navigation state previously captured will be removed.
      /// </summary>
      /// <param name="frame">An instance whose navigation history should no longer be managed.</param>
      [CLSCompliant(false)]
      public static void UnregisterFrame(Frame frame)
      {
         // Remove session state and remove the frame from the list of frames whose navigation state
         // will be saved (along with any weak references that are no longer reachable)
         SessionState.Remove((string)frame.GetValue(FrameSessionStateKeyProperty));
         _registeredFrames.RemoveAll(weakFrameReference =>
         {
            return !weakFrameReference.TryGetTarget(out var testFrame) || testFrame == frame;
         });
      }

      #endregion Public Methods

      #region Private Methods

      private static void RestoreFrameNavigationState(Frame frame)
      {
         var frameState = SessionStateForFrame(frame);
         if (frameState.ContainsKey("Navigation"))
         {
            frame.SetNavigationState((string)frameState["Navigation"]);
         }
      }

      private static void SaveFrameNavigationState(Frame frame)
      {
         var frameState = SessionStateForFrame(frame);
         frameState["Navigation"] = frame.GetNavigationState();
      }

      #endregion Private Methods
   }
}
