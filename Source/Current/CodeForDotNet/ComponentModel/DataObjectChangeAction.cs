namespace CodeForDotNet.ComponentModel
{
    /// <summary>
    /// Describes the type of change made to a <see cref="IDataObject"/>.
    /// </summary>
    public enum DataObjectChangeAction
    {
        /// <summary>
        /// The data record was created.
        /// </summary>
        Create,

        /// <summary>
        /// The data record was read.
        /// </summary>
        /// <remarks>
        /// All properties and relations should be refreshed.
        /// </remarks>
        Read,

        /// <summary>
        /// The data record was updated with current properties and relations from the object.
        /// </summary>
        Update,

        /// <summary>
        /// The data record was deleted.
        /// </summary>
        /// <remarks>
        /// Related objects and data should be deleted or disposed.
        /// </remarks>
        Delete,

        /// <summary>
        /// The instance properties have been changed, but not updated at the data source.
        /// </summary>
        Property
    }
}