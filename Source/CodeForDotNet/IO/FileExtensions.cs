using CodeForDotNet.Properties;
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;

namespace CodeForDotNet.IO
{
    /// <summary>
    /// Provides helper functions for working with files and directories.
    /// </summary>
    public static class FileExtensions
    {
        #region Constants

        /// <summary>
        /// Minimum <see cref="DateTime"/> reported by the file system.
        /// </summary>
        public static readonly DateTime FileTimeMinValue = new DateTime(1980, 1, 1);

        #endregion

        #region Public Methods

        /// <summary>
        /// Counts the size of all files in all subdirectories of the specified path.
        /// </summary>
        /// <param name="sourcePath">Directory to scan.</param>
        /// <param name="excludeFiles">List of filenames to exclude.</param>
        /// <returns>Total size in bytes.</returns>
        public static long GetDirectorySize(string sourcePath, string[] excludeFiles)
        {
            // Recurse subdirectories
            var size = Directory.GetDirectories(sourcePath).Sum(directory => GetDirectorySize(directory, excludeFiles));

            // Scan files in this directory
            foreach (var fileName in Directory.GetFiles(sourcePath))
            {
                // Ignore excluded files
                if (excludeFiles != null)
                {
                    if (excludeFiles.Any(excludeFile =>
                        string.Compare(fileName, excludeFile, StringComparison.OrdinalIgnoreCase) == 0))
                        continue;
                }

                // Count file size
                var fileInfo = new FileInfo(fileName);
                size += fileInfo.Length;
            }

            // Return total size for this subdirectory
            return size;
        }

        /// <summary>
        /// Forcibly deletes a file; removing any attributes (i.e. read-only) which may prevent deletion.
        /// </summary>
        public static void DeleteFileForce(string path)
        {
            // Do nothing when file does not exist
            if (!File.Exists(path))
                return;

            // Remove attributes then delete
            File.SetAttributes(path, FileAttributes.Normal);
            File.Delete(path);
        }

        /// <summary>
        /// Forcibly deletes a directory; removing any attributes (i.e. read-only) from files within.
        /// </summary>
        public static void DeleteDirectoryForce(string path)
        {
            // Do nothing when directory does not exist
            if (!Directory.Exists(path))
                return;

            // Recursively delete subdirectories
            foreach (string subdirectory in Directory.GetDirectories(path))
                DeleteDirectoryForce(subdirectory);

            // Delete files in current directory
            foreach (string file in Directory.GetFiles(path))
                DeleteFileForce(file);

            // Delete the directory itself (now it is empty)
            new DirectoryInfo(path).Attributes = FileAttributes.Normal;
            Directory.Delete(path, true);
        }

        /// <summary>
        /// Recursively counts files in a directory.
        /// </summary>
        public static int CountFiles(string path)
        {
            // Do nothing when directory does not exist
            if (!Directory.Exists(path))
                return 0;

            // Count files in current directory
            var count = Directory.GetFiles(path).Length;

            // Recurse subdirectories
            foreach (var subdirectory in Directory.GetDirectories(path))
                count += CountFiles(subdirectory);

            // Return result
            return count;
        }

        /// <summary>
        /// Copies all the files and subdirectories from one path to another.
        /// Creates the target path and overwrites any existing files as necessary.
        /// Provides progress feedback for each directory copied.
        /// </summary>
        /// <param name="sourcePath">Source directory.</param>
        /// <param name="targetPath">Target directory.</param>
        /// <param name="progressHandler">Optional progress delegate.</param>
        public static void CopyDirectory(string sourcePath, string targetPath, EventHandler<FileExtensionsProgressEventArgs> progressHandler)
        {
            int current = 0, total = 1;
            CopyDirectoryRecurse(sourcePath, targetPath, progressHandler, ref current, ref total);
        }

        /// <summary>
        /// Recursion method for <see cref="CopyDirectory"/>.
        /// </summary>
        /// <param name="sourcePath">Source directory.</param>
        /// <param name="targetPath">Target directory.</param>
        /// <param name="progressHandler">Optional progress delegate.</param>
        /// <param name="current">Current file count, used for progress feedback. Set to 0 at start.</param>
        /// <param name="total">Total file count, used for progress feedback. Set to 1 at start.</param>
        static void CopyDirectoryRecurse(string sourcePath, string targetPath, EventHandler<FileExtensionsProgressEventArgs> progressHandler, ref int current, ref int total)
        {
            // Create the target path if it doesn't exist
            progressHandler?.Invoke(null, new FileExtensionsProgressEventArgs
            {
                Message = string.Format(CultureInfo.CurrentCulture,
                Resources.FileExtensionsCopyDirectoryCreateDirectory,
                Path.GetFileName(Path.GetFileName(targetPath))),
                Position = ++current,
                Range = total
            });
            if (!Directory.Exists(targetPath))
                Directory.CreateDirectory(targetPath);

            // Copy files in this directory
            var files = Directory.GetFiles(sourcePath);
            total += files.Length;
            foreach (var filePath in files)
            {
                // Get target filename
                var fileName = Path.GetFileName(filePath);
                Debug.Assert(fileName != null);
                var targetFilePath = Path.Combine(targetPath, fileName);

                // Provide feedback
                progressHandler?.Invoke(null, new FileExtensionsProgressEventArgs
                {
                    Message = string.Format(CultureInfo.CurrentCulture,
                    Resources.FileExtensionsCopyDirectoryFile,
                    Path.GetFileName(filePath)),
                    Position = ++current,
                    Range = total
                });

                // Copy file
                File.Copy(filePath, targetFilePath, true);
            }

            // Recurse subdirectories
            var subdirectories = Directory.GetDirectories(sourcePath);
            total += subdirectories.Length;
            foreach (var directory in subdirectories)
            {
                var fileName = Path.GetFileName(directory);
                Debug.Assert(fileName != null);
                var targetFilePath = Path.Combine(targetPath, fileName);
                CopyDirectoryRecurse(directory, targetFilePath, progressHandler, ref current, ref total);
            }
        }

        #endregion
    }
}
