using System;

namespace CodeForDotNet.ComponentModel
{
    /// <summary>
    /// CRUD data object with event caching.
    /// </summary>
    public interface IDataObject : IPropertyStore
    {
        #region Public Events

        /// <summary>
        /// Fired when data for this object has changed.
        /// </summary>
        event EventHandler<DataObjectChangeEventArgs> DataChanged;

        #endregion Public Events

        #region Public Properties

        /// <summary>
        /// Indicates the current state of the data represented by this instance.
        /// </summary>
        DataObjectState DataState { get; }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Creates the object in storage.
        /// </summary>
        void Create();

        /// <summary>
        /// Deletes the object from storage.
        /// </summary>
        void Delete();

        /// <summary>
        /// Reads the object from storage.
        /// </summary>
        void Read();

        /// <summary>
        /// Updates the object in storage.
        /// </summary>
        void Update();

        #endregion Public Methods
    }
}
