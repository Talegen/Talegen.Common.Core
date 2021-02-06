namespace Talegen.Common.Core.Storage
{
    using System;
    using System.IO;
    using System.Linq;
    using Extensions;

    /// <summary>
    /// This class provides developers with directory path related helper methods.
    /// </summary>
    public static class Paths
    {
        /// <summary>
        /// This method is used to clean up invalid characters from a path.
        /// </summary>
        /// <param name="folderPath">Contains the file name string to parse.</param>
        /// <param name="invalidCharacters">Contains an array of characters to removed from the specified path.</param>
        /// <returns>Returns the file name with the invalid characters removed.</returns>
        public static string CleanPath(string folderPath, char[] invalidCharacters = null)
        {
            // Get an array of all invalid characters
            invalidCharacters ??= Path.GetInvalidPathChars();

            // Use value.ConvertToString() to account for empty or null values
            return new string(folderPath.ConvertToString().Where(x => !invalidCharacters.Contains(x)).ToArray());
        }

        /// <summary>
        /// This method is used to get a file name from either a local path or URL.
        /// </summary>
        /// <param name="filePathUri">Contains the file path Uri.</param>
        /// <returns>Returns the file name within the full Uri path.</returns>
        public static string GetFileName(string filePathUri)
        {
            string result;

            try
            {
                Uri fileUri = new Uri(filePathUri);
                result = Path.GetFileName(fileUri.LocalPath);
            }
            catch
            {
                result = Path.GetFileName(filePathUri);
            }

            return result;
        }

        /// <summary>
        /// This method returns the directory path string specified and appends a backward slash to the path if one did not exist.
        /// </summary>
        /// <param name="fullDirectoryPath">Contains the directory path to ensure ends with a backward slash.</param>
        /// <param name="separator">Contains the separator to append to the path. The default is \.</param>
        /// <returns>Returns the directory path string specified ending with a backward slash.</returns>
        public static string AddPathSeparator(string fullDirectoryPath, string separator = "\\")
        {
            string result = fullDirectoryPath;

            if (!string.IsNullOrWhiteSpace(result) && !result.EndsWith(separator, StringComparison.Ordinal))
            {
                result = result + separator;
            }

            return result;
        }

        /// <summary>
        /// This method returns the directory path string without any ending separator.
        /// </summary>
        /// <param name="fullDirectoryPath">Contains the directory path to ensure does not end with the separator.</param>
        /// <param name="separator">Contains the separator to remove from the path. The default is \.</param>
        /// <returns>Returns the directory path string specified with any separator removed.</returns>
        public static string RemovePathSeparator(string fullDirectoryPath, char separator = '\\')
        {
            string result = (fullDirectoryPath ?? string.Empty).TrimEnd(new[] { ' ', separator });
            return result.Contains(separator) ? result : fullDirectoryPath;
        }

        /// <summary>
        /// This method is used to return the parent directory path of a specified path.
        /// </summary>
        /// <param name="fullDirectoryPath">Contains the directory path to the child sub-folder.</param>
        /// <returns>Returns the parent folder path of the specified child sub-folder. If no parent exists, the child sub-folder is returned.</returns>
        public static string ParentDirectory(string fullDirectoryPath)
        {
            string result = Path.GetDirectoryName(RemovePathSeparator(fullDirectoryPath));
            return !string.IsNullOrEmpty(result) ? result : fullDirectoryPath;
        }

        /// <summary>
        /// This method is used to move a folder and overwrite (remove) the target if it already exists to avoid errors.
        /// </summary>
        /// <param name="sourcePath">Contains the full path to the source folder.</param>
        /// <param name="targetPath">Contains the full path to the target folder.</param>
        public static void OverwriteMove(string sourcePath, string targetPath)
        {
            if (Directory.Exists(targetPath))
            {
                Directory.Delete(targetPath, true);
            }

            Directory.Move(sourcePath, targetPath);
        }

        /// <summary>
        /// This method is used to determine if a directory path specified is valid and well-formed.
        /// </summary>
        /// <param name="directoryPath">Contains the directory path to validate.</param>
        /// <returns>Returns a value indicating whether the path is determined to be well-formed.</returns>
        public static bool ValidatePath(string directoryPath)
        {
            char[] invalidCharacters = Path.GetInvalidPathChars();
            bool result = !string.IsNullOrWhiteSpace(directoryPath) && Path.IsPathRooted(directoryPath) && directoryPath.Any(invalidCharacters.Contains);

            if (result)
            {
                try
                {
                    // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
                    Path.GetDirectoryName(directoryPath);
                }
                catch
                {
                    result = false;
                }
            }

            return result;
        }
    }
}