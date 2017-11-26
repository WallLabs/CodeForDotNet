using System;
using System.Diagnostics;
using System.IO;

namespace CodeForDotNet.Diagnostics
{
    /// <summary>
    /// Writes trace output to a file, supporting environment variables in the filename and lazy open of the file.
    /// </summary>
    public class FileWriterTraceListener : TraceListener
    {
        /// <summary>
        /// Filename to write to.
        /// </summary>
        string _fileName;

        /// <summary>
        /// Output file stream.
        /// </summary>
        StreamWriter _stream;

        /// <summary>
        /// Creates the object.
        /// </summary>
        public FileWriterTraceListener(string fileName)
        {
            // Expand any variables in the filename
            _fileName = Environment.ExpandEnvironmentVariables(fileName);
        }

        /// <summary>
        /// Closes the output stream (if open).
        /// </summary>
        public override void Close()
        {
            if (_stream != null)
            {
                _stream.Dispose();
                _stream = null;
            }
        }

        /// <summary>
        /// Flushes any buffered data to the file (if open).
        /// </summary>
        public override void Flush()
        {
            if (_stream != null)
                _stream.Flush();
        }

        /// <summary>
        /// Cleans-up resources.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            try
            {
                // Dispose unmanaged resources
                if (_stream != null)
                {
                    _stream.Dispose();
                    _stream = null;
                }
            }
            finally
            {
                // Dispose base class
                base.Dispose(disposing);
            }
        }

        /// <summary>
        /// Writes to the output stream.
        /// </summary>
        public override void Write(string message)
        {
            // Lazy create/open file
            OpenFile();

            // Write to file
            _stream.Write(message);
        }

        /// <summary>
        /// Writes to the output stream followed by a new line.
        /// </summary>
        public override void WriteLine(string message)
        {
            // Lazy create/open file
            OpenFile();

            // Write to file
            _stream.WriteLine(message);
        }

        /// <summary>
        /// Opens the stream when it is needed (lazy open).
        /// </summary>
        void OpenFile()
        {
            // Create directory if not exists
            string directory = Path.GetDirectoryName(_fileName);
            if (!String.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            // Create or open file
            if (_stream == null)
                _stream = new StreamWriter(_fileName, true);
        }
    }
}
