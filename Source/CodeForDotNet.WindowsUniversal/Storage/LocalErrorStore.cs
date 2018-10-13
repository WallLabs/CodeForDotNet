using CodeForDotNet.Data;
using CodeForDotNet.Xml;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Windows.Storage;
using Windows.UI.Xaml;

namespace CodeForDotNet.WindowsUniversal.Storage
{
    /// <summary>
    /// Stores errors in local storage as serialized exceptions.
    /// </summary>
    public static class LocalErrorStore
    {
        #region Constants

        /// <summary>
        /// Name of the file which stores a unique source identifier,
        /// used to correlate files sent from the same installation.
        /// </summary>
        public const string SourceIdFileName = "Source ID.txt";

        /// <summary>
        /// Name of the folder in local storage where files are stored.
        /// </summary>
        public const string FolderName = "Errors";

        /// <summary>
        /// Error file name format.
        /// </summary>
        public const string FileNameFormat = "{0:yyyyMMddHHmmssfff} Error.txt";

        #endregion Constants

        #region Public Methods

        /// <summary>
        /// Extracts information from an error to use as the contents of an error file.
        /// </summary>
        /// <param name="error">Error to extract.</param>
        /// <returns>File contents.</returns>
        public static ErrorReportData GenerateReport(Exception error)
        {
            // Validate
            if (error == null) throw new ArgumentNullException(nameof(error));

            // Write application and version information at top
            var application = Application.Current;
            var applicationType = application.GetType();
            var report = new ErrorReportData
            {
                Id = Guid.NewGuid(),
                SourceId = LoadSourceId(),
                SourceAssemblyName = applicationType.AssemblyQualifiedName,
                EventDate = DateTime.UtcNow,
                Message = error.GetFullMessage(),
                ErrorTypeFullName = error.GetType().Name,
                StackTrace = error.StackTrace
            };

            // Return result
            return report;
        }

        /// <summary>
        /// Retrieves or creates the error ID file.
        /// </summary>
        /// <returns>Unique ID stored as a file locally so it remains the same for the installation lifetime of the application.</returns>
        public static Guid LoadSourceId()
        {
            // Open local storage folder
            var storage = ApplicationData.Current.LocalFolder;

            // Open errors folder (return null when not found)
            var errorsFolder = storage.CreateFolder(FolderName);

            // Open ID file if exists
            var file = errorsFolder.OpenFile(SourceIdFileName);
            Guid id;
            if (file != null)
            {
                // Read existing ID
                if (!Guid.TryParse(file.ReadAllText(), out id))
                    return id;
            }
            else
            {
                // Create new file when doesn't exist
                file = errorsFolder.CreateFile(SourceIdFileName);
            }

            // Write new ID when invalid or new file
            id = Guid.NewGuid();
            file.WriteAllText(id.ToString());

            // Return new ID
            return id;
        }

        /// <summary>
        /// Adds an error to the store.
        /// </summary>
        /// <param name="error">Error to write the contents of.</param>
        /// <returns>File name.</returns>
        public static string Add(Exception error)
        {
            // Extract error contents
            var contents = GenerateReport(error);

            // Call overloaded method
            return Add(contents);
        }

        /// <summary>
        /// Adds an error to the store.
        /// </summary>
        /// <param name="contents">Contents to write.</param>
        /// <returns>File name.</returns>
        public static string Add(ErrorReportData contents)
        {
            // Open local storage folder
            var storage = ApplicationData.Current.LocalFolder;

            // Open or create errors folder
            var errorsFolder = storage.CreateFolder(FolderName);

            // Generate a unique file name
            var fileName = string.Format(CultureInfo.InvariantCulture, FileNameFormat, DateTime.UtcNow);

            // Write error to file
            var file = errorsFolder.CreateFile(fileName);
            file.WriteAllText(contents.SerializeXml(true));

            // Return file name
            return fileName;
        }

        /// <summary>
        /// Lists all errors currently held in the store.
        /// </summary>
        public static string[] List()
        {
            // Open local storage folder
            var storage = ApplicationData.Current.LocalFolder;

            // Open errors folder (return null when not found)
            var errorsFolder = storage.OpenFolder(FolderName);
            if (errorsFolder == null)
                return null;

            // List files
            var files = new List<string>();
            files.AddRange(from file in errorsFolder.GetFilesAsync().GetAwaiter().GetResult()
                           where !file.Name.Equals(SourceIdFileName, StringComparison.OrdinalIgnoreCase)
                           select file.Name);

            // Return result
            return files.ToArray();
        }

        /// <summary>
        /// Gets an error.
        /// </summary>
        public static ErrorReportData Get(string fileName)
        {
            // Open local storage folder
            var storage = ApplicationData.Current.LocalFolder;

            // Open errors folder (return null when not found)
            var errorsFolder = storage.OpenFolder(FolderName);
            if (errorsFolder == null)
                return null;

            // Open file (return null when not found)
            var file = errorsFolder.OpenFile(fileName);
            if (file == null)
                return null;

            // Return contents
            return XmlSerializerExtensions.DeserializeXml<ErrorReportData>(file.ReadAllText());
        }

        /// <summary>
        /// Removes an error from the store.
        /// </summary>
        /// <param name="fileName">File name.</param>
        public static void Remove(string fileName)
        {
            // Open local storage folder
            var storage = ApplicationData.Current.LocalFolder;

            // Open errors folder (return when not found)
            var errorsFolder = storage.OpenFolder(FolderName);
            if (errorsFolder == null)
                return;

            // Open file (return when not found)
            var file = errorsFolder.OpenFile(fileName);
            if (file == null)
                return;

            // Delete file
            file.Delete(true);
        }

        #endregion Public Methods
    }
}