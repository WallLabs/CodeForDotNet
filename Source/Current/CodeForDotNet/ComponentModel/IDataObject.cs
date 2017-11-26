using System;

namespace CodeForDotNet.ComponentModel
{
    /// <summary>
    /// CRUD data object with event caching.
    /// </summary>
    public interface IDataObject : IPropertyObject
    {
        #region Properties

        /// <summary>
        /// Indicates the current state of the data represented by this instance.
        /// </summary>
        DataObjectState DataState { get; }

        #endregion

        #region Methods

        #region Store Access

        /// <summary>
        /// Creates the object in storage.
        /// </summary>
        void Create();

        /// <summary>
        /// Reads the object from storage.
        /// </summary>
        void Read();

        /// <summary>
        /// Updates the object in storage.
        /// </summary>
        void Update();

        /// <summary>
        /// Deletes the object from storage.
        /// </summary>
        void Delete();

        #endregion

        #endregion

        #region Events

        /// <summary>
        /// Fired when data for this object has changed.
        /// </summary>
        event EventHandler<DataObjectChangeEventArgs> DataChanged;

        #endregion
    }
}
