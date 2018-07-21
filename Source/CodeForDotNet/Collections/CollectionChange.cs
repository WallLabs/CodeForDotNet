namespace CodeForDotNet.Collections
{
    /// <summary>
    /// Describes the action that causes a change to a collection.
    /// </summary>
    /// <remarks>
    /// Portable version of the Windows.Foundation.Collections.CollectionChange enumeration only
    /// available to non-portable Windows Store application.
    /// </remarks>
    public enum CollectionChange
    {
        /// <summary>
        /// All entries cleared.
        /// </summary>
        Reset,

        /// <summary>
        /// Entry added.
        /// </summary>
        Add,

        /// <summary>
        /// Entry removed.
        /// </summary>
        Remove,

        /// <summary>
        /// Entry replaced.
        /// </summary>
        Replace,
    }
}
