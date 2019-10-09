using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;

namespace CodeForDotNet.UI.Models
{
	/// <summary>
	/// Base class for all UI models
	/// </summary>
	public abstract class UIModel : DisposableObject, INotifyPropertyChanged
	{
		#region Public Fields

		/// <summary>
		/// Maximum time (in milliseconds) allowed for the UI to process updates before continuing with other updates.
		/// </summary>
		/// <remarks>
		/// When too short and events are generated too quickly, the UI has no chance to refresh. When too long and processor intensive operations are triggered,
		/// the UI could appear to hang. Deadlocks could occur when background threads with call <see cref="DoPropertyChanged(string)"/> to update the UI thread
		/// and another UI action has been performed which also requires the same lock.
		/// </remarks>
		public const int UpdateTimeout = 500;

		#endregion Public Fields

		#region Protected Constructors

		/// <summary>
		/// Creates an instance.
		/// </summary>
		protected UIModel(TaskFactory uiTaskFactory)
		{
			// Initialize members
			UITaskFactory = uiTaskFactory;
		}

		#endregion Protected Constructors

		#region Public Events

		/// <summary>
		/// Fired when the model data has changed and the view should be refreshed.
		/// </summary>
		public event PropertyChangedEventHandler? PropertyChanged;

		#endregion Public Events

		#region Public Properties

		/// <summary>
		/// Task factory for the UI thread.
		/// </summary>
		public TaskFactory UITaskFactory { get; private set; }

		#endregion Public Properties

		#region Protected Methods

		/// <summary>
		/// Frees resources owned by this instance.
		/// </summary>
		/// <param name="disposing">True when called via <see cref="IDisposable.Dispose()"/>, false when called during finalization.</param>
		protected override void Dispose(bool disposing)
		{
			// Implemented to allow inheritors to reduce code when not necessary
		}

		/// <summary>
		/// Fires the <see cref="PropertyChanged"/> event.
		/// </summary>
		/// <param name="name">Name of the property which changed.</param>
		protected virtual void DoPropertyChanged(string name)
		{
			// Do nothing when disposed
			if (IsDisposed) return;

			// Run event handler on UI thread
			if (PropertyChanged != null)
			{
				UITaskFactory.StartNew(() =>
				{
					// Do nothing when disposed (may occur whilst scheduling call to UI thread)
					if (IsDisposed) return;

					// Fire event causing UI to update
					PropertyChanged(this, new PropertyChangedEventArgs(name));
				}, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default)
				.Wait(UpdateTimeout);
			}
		}

		#endregion Protected Methods
	}
}
