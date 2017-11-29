using System;

namespace CodeForDotNet.IO
{
    /// <summary>
    /// Event arguments for a progress update.
    /// </summary>
    public class FileExtensionsProgressEventArgs : EventArgs
    {
        /// <summary>
        /// Progress message.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Current position.
        /// </summary>
        public int Position { get; set; }

        /// <summary>
        /// Range/maximum/end position.
        /// </summary>
        public int Range { get; set; }
    }
}