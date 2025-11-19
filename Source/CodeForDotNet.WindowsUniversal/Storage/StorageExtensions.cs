using System;
using System.Linq;
using Windows.Storage;

namespace CodeForDotNet.WindowsUniversal.Storage
{
    /// <summary>
    /// Extends the <see cref="StorageFolder"/> class.
    /// </summary>
    public static class StorageExtensions
    {
        #region Public Methods

        /// <summary>
        /// Creates or opens an existing file in the folder.
        /// </summary>
        /// <param name="parentFolder">
        /// Folder in which the file is located, usually specified via extension.
        /// </param>
        /// <param name="fileName">File name.</param>
        /// <returns>A <see cref="IStorageFile"/> representing the created or existing file.</returns>
        public static IStorageFile CreateFile(this IStorageFolder parentFolder, string fileName)
        {
            // Validate
            if (parentFolder == null)
            {
                throw new ArgumentNullException(nameof(parentFolder));
            }

            // Call overloaded method
            return parentFolder.CreateFileAsync(fileName, CreationCollisionOption.OpenIfExists).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Calls the <see cref="IStorageFolder.CreateFolderAsync(string)"/> method synchronously and
        /// without error if the folder already exists.
        /// </summary>
        /// <param name="parentFolder">Parent folder, usually specified via extension.</param>
        /// <param name="folderName">Name of the folder to create.</param>
        /// <returns>A <see cref="IStorageFolder"/> representing the created or existing folder.</returns>
        public static IStorageFolder CreateFolder(this IStorageFolder parentFolder, string folderName)
        {
            // Validate
            if (parentFolder == null)
            {
                throw new ArgumentNullException(nameof(parentFolder));
            }

            // Call overloaded method
            return parentFolder.CreateFolderAsync(folderName, CreationCollisionOption.OpenIfExists).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Deletes a file, moving it to the recycle bin when available.
        /// </summary>
        /// <param name="file">File to delete, usually specified via extension.</param>
        public static void Delete(this IStorageFile file)
        {
            Delete(file, false);
        }

        /// <summary>
        /// Moves a file to the recycle bin or deletes it permanently when specified or the recycle
        /// bin is not available.
        /// </summary>
        /// <param name="file">File to delete, usually specified via extension.</param>
        /// <param name="permanent">
        /// True to delete permanently, otherwise it is moved to the recycle bin when available.
        /// Default is false (recycle).
        /// </param>
        public static void Delete(this IStorageFile file, bool permanent)
        {
            // Validate
            if (file == null)
            {
                throw new ArgumentNullException(nameof(file));
            }

            // Call overloaded method with appropriate delete option
            file.DeleteAsync(permanent
                                 ? StorageDeleteOption.PermanentDelete
                                 : StorageDeleteOption.Default).GetAwaiter();
        }

        /// <summary>
        /// Gets a file if it exists or returns null when not found. A better alternative to
        /// <see cref="IStorageFolder.GetFileAsync"/> which throws an exception when not found.
        /// </summary>
        /// <param name="parentFolder">Folder to query, usually specified via extension.</param>
        /// <param name="fileName">Name of the file to query, case insensitive.</param>
        /// <returns>A <see cref="StorageFile"/> representing the file or null when not found.</returns>
        public static IStorageFile OpenFile(this IStorageFolder parentFolder, string fileName)
        {
            // Validate
            if (parentFolder == null)
            {
                throw new ArgumentNullException(nameof(parentFolder));
            }

            // Search for file and return result or null
            return (from file in parentFolder.GetFilesAsync().GetAwaiter().GetResult()
                    where file.Name.Equals(fileName, StringComparison.OrdinalIgnoreCase)
                    select file).FirstOrDefault();
        }

        /// <summary>
        /// Gets a subfolder if it exists or returns null when not found. A better alternative to
        /// <see cref="IStorageFolder.GetFolderAsync"/> which throws an exception when not found.
        /// </summary>
        /// <param name="parentFolder">Folder to query, usually specified via extension.</param>
        /// <param name="subfolderName">Name of the subfolder to query, case insensitive.</param>
        /// <returns>A <see cref="StorageFolder"/> representing the folder or null when not found.</returns>
        public static IStorageFolder OpenFolder(this IStorageFolder parentFolder, string subfolderName)
        {
            // Validate
            if (parentFolder == null)
            {
                throw new ArgumentNullException(nameof(parentFolder));
            }

            // Search for folder and return result or null
            return (from subfolder in parentFolder.GetFoldersAsync().GetAwaiter().GetResult()
                    where subfolder.Name.Equals(subfolderName, StringComparison.OrdinalIgnoreCase)
                    select subfolder).FirstOrDefault();
        }

        /// <summary>
        /// Reads the entire contents of a file as text.
        /// </summary>
        /// <param name="file">File to read, usually specified via extension.</param>
        /// <returns>Text from the file.</returns>
        public static string ReadAllText(this IStorageFile file)
        {
            // Validate
            if (file == null)
            {
                throw new ArgumentNullException(nameof(file));
            }

            // Call overloaded method
            return FileIO.ReadTextAsync(file).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Writes the entire contents of a file as text.
        /// </summary>
        /// <param name="file">File to write, usually specified via extension.</param>
        /// <param name="contents">Text content to write.</param>
        public static void WriteAllText(this IStorageFile file, string contents)
        {
            // Validate
            if (file == null)
            {
                throw new ArgumentNullException(nameof(file));
            }

            // Call overloaded method
            FileIO.WriteTextAsync(file, contents).GetAwaiter().GetResult();
        }

        #endregion Public Methods
    }
}
