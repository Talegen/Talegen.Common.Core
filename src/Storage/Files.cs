/*
 *
 * (c) Copyright Talegen, LLC.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * http://www.apache.org/licenses/LICENSE-2.0
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *
*/

namespace Talegen.Common.Core.Storage
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;
    using Extensions;

    /// <summary>
    /// This class provides file related helper methods for the developer.
    /// </summary>
    public static class Files
    {
        #region Private Static Fields

        /// <summary>
        /// Contains a dictionary of known image content types.
        /// </summary>
        private static readonly Dictionary<string, string> ImageLookupMap = new Dictionary<string, string>
        {
            { "apng", "image/png" },
            { "bmp", "image/bmp" },
            { "cgm", "image/cgm" },
            { "djv", "image/vnd.djvu" },
            { "djvu", "image/vnd.djvu" },
            { "gif", "image/gif" },
            { "ico", "image/x-icon" },
            { "ief", "image/ief" },
            { "jfif", "image/jpeg" },
            { "jp2", "image/jp2" },
            { "jpe", "image/jpeg" },
            { "jpeg", "image/jpeg" },
            { "jpg", "image/jpeg" },
            { "mac", "image/x-macpaint" },
            { "pbm", "image/x-portable-bitmap" },
            { "pct", "image/pict" },
            { "pgm", "image/x-portable-graymap" },
            { "pic", "image/pict" },
            { "pict", "image/pict" },
            { "png", "image/png" },
            { "pnm", "image/x-portable-anymap" },
            { "pnt", "image/x-macpaint" },
            { "pntg", "image/x-macpaint" },
            { "ppm", "image/x-portable-pixmap" },
            { "qti", "image/x-quicktime" },
            { "qtif", "image/x-quicktime" },
            { "ras", "image/x-cmu-raster" },
            { "rgb", "image/x-rgb" },
            { "svg", "image/svg+xml" },
            { "svgz", "image/svg+xml" },
            { "tif", "image/tiff" },
            { "tiff", "image/tiff" },
            { "wbmp", "image/vnd.wap.wbmp" },
            { "xbm", "image/x-xbitmap" },
            { "xpm", "image/x-xpixmap" },
            { "xwd", "image/x-xwindowdump" }
        };

        /// <summary>
        /// Contains a pre-populated dictionary of file extensions to MIME content types mapping.
        /// </summary>
        private static readonly Dictionary<string, string> LookupMap = new Dictionary<string, string>(ImageLookupMap)
        {
            { "ai", "application/postscript" },
            { "aif", "audio/x-aiff" },
            { "aifc", "audio/x-aiff" },
            { "aiff", "audio/x-aiff" },
            { "asc", "text/plain" },
            { "asp", "text/html" },
            { "aspx", "text/html" },
            { "atom", "application/atom+xml" },
            { "au", "audio/basic" },
            { "avi", "video/x-msvideo" },
            { "bcpio", "application/x-bcpio" },
            { "bin", "application/octet-stream" },
            { "cdf", "application/x-netcdf" },
            { "chtml", "text/html" },
            { "class", "application/octet-stream" },
            { "cpio", "application/x-cpio" },
            { "cpt", "application/mac-compactpro" },
            { "csh", "application/x-csh" },
            { "css", "text/css" },
            { "csv", "application/csv" },
            { "dcr", "application/x-director" },
            { "dif", "video/x-dv" },
            { "dir", "application/x-director" },
            { "ditamap", "text/xml" }, // RK: Maybe application/dita+xml someday?
            { "dll", "application/octet-stream" },
            { "dmg", "application/octet-stream" },
            { "dms", "application/octet-stream" },
            { "doc", "application/msword" },
            { "docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document" },
            { "dotx", "application/vnd.openxmlformats-officedocument.wordprocessingml.template" },
            { "docm", "application/vnd.ms-word.document.macroEnabled.12" },
            { "dotm", "application/vnd.ms-word.template.macroEnabled.12" },
            { "dtd", "application/xml-dtd" },
            { "dv", "video/x-dv" },
            { "dvi", "application/x-dvi" },
            { "dxr", "application/x-director" },
            { "eps", "application/postscript" },
            { "etx", "text/x-setext" },
            { "exe", "application/octet-stream" },
            { "ez", "application/andrew-inset" },
            { "gram", "application/srgs" },
            { "grxml", "application/srgs+xml" },
            { "gtar", "application/x-gtar" },
            { "hdf", "application/x-hdf" },
            { "hqx", "application/mac-binhex40" },
            { "htm", "text/html" },
            { "html", "text/html" },
            { "ice", "x-conference/x-cooltalk" },
            { "ics", "text/calendar" },
            { "ifb", "text/calendar" },
            { "iges", "model/iges" },
            { "igs", "model/iges" },
            { "jnlp", "application/x-java-jnlp-file" },
            { "js", "application/x-javascript" },
            { "kar", "audio/midi" },
            { "latex", "application/x-latex" },
            { "lha", "application/octet-stream" },
            { "lzh", "application/octet-stream" },
            { "m3u", "audio/x-mpegurl" },
            { "m4a", "audio/mp4a-latm" },
            { "m4b", "audio/mp4a-latm" },
            { "m4p", "audio/mp4a-latm" },
            { "m4u", "video/vnd.mpegurl" },
            { "m4v", "video/x-m4v" },
            { "man", "application/x-troff-man" },
            { "mathml", "application/mathml+xml" },
            { "me", "application/x-troff-me" },
            { "mesh", "model/mesh" },
            { "mid", "audio/midi" },
            { "midi", "audio/midi" },
            { "mif", "application/vnd.mif" },
            { "mov", "video/quicktime" },
            { "movie", "video/x-sgi-movie" },
            { "mp2", "audio/mpeg" },
            { "mp3", "audio/mpeg" },
            { "mp4", "video/mp4" },
            { "mpe", "video/mpeg" },
            { "mpeg", "video/mpeg" },
            { "mpg", "video/mpeg" },
            { "mpga", "audio/mpeg" },
            { "ms", "application/x-troff-ms" },
            { "msh", "model/mesh" },
            { "mxu", "video/vnd.mpegurl" },
            { "nc", "application/x-netcdf" },
            { "oda", "application/oda" },
            { "ogg", "application/ogg" },
            { "pdb", "chemical/x-pdb" },
            { "pdf", "application/pdf" },
            { "pgn", "application/x-chess-pgn" },
            { "ppt", "application/vnd.ms-powerpoint" },
            { "pptx", "application/vnd.openxmlformats-officedocument.presentationml.presentation" },
            { "potx", "application/vnd.openxmlformats-officedocument.presentationml.template" },
            { "ppsx", "application/vnd.openxmlformats-officedocument.presentationml.slideshow" },
            { "ppam", "application/vnd.ms-powerpoint.addin.macroEnabled.12" },
            { "pptm", "application/vnd.ms-powerpoint.presentation.macroEnabled.12" },
            { "potm", "application/vnd.ms-powerpoint.template.macroEnabled.12" },
            { "ppsm", "application/vnd.ms-powerpoint.slideshow.macroEnabled.12" },
            { "ps", "application/postscript" },
            { "qt", "video/quicktime" },
            { "ra", "audio/x-pn-realaudio" },
            { "ram", "audio/x-pn-realaudio" },
            { "rdf", "application/rdf+xml" },
            { "rm", "application/vnd.rn-realmedia" },
            { "roff", "application/x-troff" },
            { "rtf", "text/rtf" },
            { "rtx", "text/richtext" },
            { "sgm", "text/sgml" },
            { "sgml", "text/sgml" },
            { "sh", "application/x-sh" },
            { "shar", "application/x-shar" },
            { "silo", "model/mesh" },
            { "sit", "application/x-stuffit" },
            { "skd", "application/x-koan" },
            { "skm", "application/x-koan" },
            { "skp", "application/x-koan" },
            { "skt", "application/x-koan" },
            { "smi", "application/smil" },
            { "smil", "application/smil" },
            { "snd", "audio/basic" },
            { "so", "application/octet-stream" },
            { "spl", "application/x-futuresplash" },
            { "src", "application/x-wais-source" },
            { "sv4cpio", "application/x-sv4cpio" },
            { "sv4crc", "application/x-sv4crc" },
            { "swf", "application/x-shockwave-flash" },
            { "t", "application/x-troff" },
            { "tar", "application/x-tar" },
            { "tcl", "application/x-tcl" },
            { "tex", "application/x-tex" },
            { "texi", "application/x-texinfo" },
            { "texinfo", "application/x-texinfo" },
            { "tr", "application/x-troff" },
            { "tsv", "text/tab-separated-values" },
            { "txt", "text/plain" },
            { "ustar", "application/x-ustar" },
            { "vcd", "application/x-cdlink" },
            { "vrml", "model/vrml" },
            { "vxml", "application/voicexml+xml" },
            { "wav", "audio/x-wav" },
            { "wbmxl", "application/vnd.wap.wbxml" },
            { "wml", "text/vnd.wap.wml" },
            { "wmlc", "application/vnd.wap.wmlc" },
            { "wmls", "text/vnd.wap.wmlscript" },
            { "wmlsc", "application/vnd.wap.wmlscriptc" },
            { "wrl", "model/vrml" },
            { "xht", "application/xhtml+xml" },
            { "xhtml", "application/xhtml+xml" },
            { "xls", "application/vnd.ms-excel" },
            { "xml", "application/xml" },
            { "xsl", "application/xml" },
            { "xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" },
            { "xltx", "application/vnd.openxmlformats-officedocument.spreadsheetml.template" },
            { "xlsm", "application/vnd.ms-excel.sheet.macroEnabled.12" },
            { "xltm", "application/vnd.ms-excel.template.macroEnabled.12" },
            { "xlam", "application/vnd.ms-excel.addin.macroEnabled.12" },
            { "xlsb", "application/vnd.ms-excel.sheet.binary.macroEnabled.12" },
            { "xslt", "application/xslt+xml" },
            { "xul", "application/vnd.mozilla.xul+xml" },
            { "xyz", "chemical/x-xyz" },
            { "zip", "application/zip" }
        };

        #endregion Private Static Fields

        #region Public Static Methods

        /// <summary>
        /// This extension method is used to add a specified GUID to the file name.
        /// </summary>
        /// <param name="fileInfo">Contains the File Info for the file to add a GUID into the file name.</param>
        /// <param name="guidValue">Contains the GUID to add to the file name.</param>
        /// <returns>Returns the a new file name containing the GUID value.</returns>
        public static string AddGuid(this FileInfo fileInfo, Guid guidValue)
        {
            if (fileInfo == null)
            {
                throw new ArgumentNullException(nameof(fileInfo));
            }

            return AddGuidToName(fileInfo.Name, guidValue);
        }

        /// <summary>
        /// This extension method is used to read error lines from a text file.
        /// </summary>
        /// <param name="filePathName">Contains the file and path name of the text file to read.</param>
        /// <param name="prefix">Contains the text of the line prefix that will identify the line as an error.</param>
        /// <returns>Returns a list of errors from the text file.</returns>
        public static List<string> FindFileErrorLines(this string filePathName, string prefix = "Error: ")
        {
            List<string> errorLines = new List<string>();

            if (File.Exists(filePathName))
            {
                try
                {
                    errorLines = File.ReadLines(filePathName).Where(line => line.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                                                             .Select(line => line.Substring(prefix.Length)).ToList();
                }
                catch
                {
                    errorLines = new List<string>();
                }
            }

            return errorLines;
        }

        /// <summary>
        /// This method is used to clean up invalid characters from a file name.
        /// </summary>
        /// <param name="fileName">Contains the file name string to parse.</param>
        /// <param name="convertSpaceCharacters">Contains a value indicating whether space characters are converted to the specified character string.</param>
        /// <param name="convertSpaceTo">Contains an optional character string that space characters shall be converted to.</param>
        /// <param name="invalidCharacters">Contains an array of characters to removed from the specified file name.</param>
        /// <returns>Returns the file name with the invalid characters removed.</returns>
        public static string CleanFileName(string fileName, bool convertSpaceCharacters = true, string convertSpaceTo = "_", List<char> invalidCharacters = null)
        {
            string result = string.Empty;

            if (!string.IsNullOrEmpty(fileName))
            {
                // Get an array of all invalid characters. We're adding ' and # to the list for "safety"
                invalidCharacters = invalidCharacters ?? new List<char> { '\'', '#', '&', ';', '%', '!', '"', ':', '@', '{', '}', '$' };
                invalidCharacters.AddRange(Path.GetInvalidFileNameChars());

                if (convertSpaceCharacters)
                {
                    fileName = Regex.Replace(fileName, @"\s+", convertSpaceTo);
                }

                result = new string(fileName.Where(x => !invalidCharacters.Contains(x)).ToArray());
            }

            return result;
        }

        /// <summary>
        /// This method is used to return a MIME content type string for a specified file by using the file extension as a dictionary lookup for the appropriate type.
        /// </summary>
        /// <param name="fileName">Contains the file name to retrieve a MIME type for.</param>
        /// <param name="defaultMimeType">Contains the default MIME type to return if a match is not found.</param>
        /// <returns>Returns the MIME content type string for the specified file extension.</returns>
        public static string FindMimeContentTypeByExtension(string fileName, string defaultMimeType = "application/octet-stream")
        {
            string result = defaultMimeType;
            string extension = Path.GetExtension(fileName);

            if (!string.IsNullOrWhiteSpace(extension))
            {
                string key = extension.Replace(".", string.Empty).ToLowerInvariant();

                if (LookupMap.ContainsKey(key))
                {
                    result = LookupMap[key];
                }
            }

            return result;
        }

        /// <summary>
        /// This method is used to determine if the MIME content type specified matches that for a known image type.
        /// </summary>
        /// <param name="contentType">Contains the content type to lookup.</param>
        /// <returns>Returns a value indicating whether the content type is a known image type.</returns>
        public static bool IsImageContent(string contentType)
        {
            if (string.IsNullOrEmpty(contentType))
            {
                throw new ArgumentNullException(nameof(contentType));
            }

            return ImageLookupMap.ContainsValue(contentType.ToLowerInvariant());
        }

        /// <summary>
        /// This method is used to determine if the file name is a supported image.
        /// </summary>
        /// <param name="fileName">Contains the file name to determine if it is a supported browser image.</param>
        /// <returns>Returns a value indicating whether the file name is an image.</returns>
        public static bool IsImageFile(string fileName)
        {
            return IsImageContent(FindMimeContentTypeByExtension(fileName));
        }

        /// <summary>
        /// This method is used to determine if a file name contains an extension recognized as a supported browser image type.
        /// </summary>
        /// <param name="fileName">Contains the file name to analyze.</param>
        /// <returns>Returns a value indicating whether the file name is recognized as a supported browser image.</returns>
        public static bool IsBrowserImage(string fileName)
        {
            string[] supportedExtensions = { "apng", "bmp", "gif", "ico", "jfif", "jpeg", "jpe", "jpg", "png", "svg" };
            return supportedExtensions.Contains(Path.GetExtension(fileName)?.Replace(".", string.Empty).ToLowerInvariant() ?? string.Empty, StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// This method is used to determine if the image file name contains an extension that is able to be rendered as a thumbnail within the application.
        /// </summary>
        /// <param name="fileName">Contains the file name of the image in question.</param>
        /// <returns>Returns true if the image can be rendered as a thumbnail within the application.</returns>
        public static bool CanCreateThumbnail(string fileName)
        {
            string[] supportedThumbnailImageTypes = { "apng", "bmp", "gif", "ico", "jfif", "jpeg", "jpe", "jpg", "png", "tif", "tiff" };
            return supportedThumbnailImageTypes.Contains(Path.GetExtension(fileName)?.Replace(".", string.Empty).ToLowerInvariant() ?? string.Empty, StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// This method is used to remove invalid file system characters from a file name.
        /// </summary>
        /// <param name="fileName">Contains the file name to parse.</param>
        /// <returns>Returns a valid clean file name.</returns>
        /// <remarks>Could probably just use regular expressions but this will suffice for now</remarks>
        public static string FormatFileName(string fileName)
        {
            return fileName.Replace('<', '-').Replace('>', '-').Replace(':', '-').Replace(':', '-').Replace('"', '-').Replace('/', '-').Replace('\\', '-').Replace('|', '-').Replace('?', '-').Replace('*', '-');
        }

        /// <summary>
        /// This method is used to move a file and overwrite (remove) the target if it already exists to avoid errors.
        /// </summary>
        /// <param name="sourcePath">Contains the full path to the source file.</param>
        /// <param name="targetPath">Contains the full path to the target file.</param>
        public static void OverwriteMove(string sourcePath, string targetPath)
        {
            if (File.Exists(targetPath))
            {
                File.Delete(targetPath);
            }

            File.Move(sourcePath, targetPath);
        }

        /// <summary>
        /// This method does a copy before attempting to remove a previous source file. If the delete fails, the copy will succeed.
        /// </summary>
        /// <param name="sourcePath">Contains the full path to the source file.</param>
        /// <param name="targetPath">Contains the full path to the target file.</param>
        /// <returns>Returns a value indicating whether the file move was successful.</returns>
        public static bool SafeMove(string sourcePath, string targetPath)
        {
            bool result;

            try
            {
                File.Copy(sourcePath, targetPath, true);

                result = File.Exists(targetPath);

                if (result)
                {
                    File.Delete(sourcePath);
                }
            }
            catch (Exception)
            {
                result = File.Exists(targetPath);
            }

            return result;
        }

        /// <summary>
        /// This method is used to determine if a file name contains a GUID.
        /// </summary>
        /// <param name="fileName">Contains the file name text that will be searched.</param>
        /// <returns>Returns a value indicating whether the file name contains a GUID value.</returns>
        public static bool NameContainsGuid(string fileName)
        {
            bool result = false;

            if (!string.IsNullOrWhiteSpace(fileName) && fileName.Length >= 36)
            {
                // remove any contents after #
                int anchorCharLocation = fileName.LastIndexOf('#');
                if (anchorCharLocation >= 0)
                {
                    fileName = fileName.Substring(0, anchorCharLocation);
                }

                // its long enough to contain a GUID, so continue...
                string fileNameOnly = Path.GetFileNameWithoutExtension(fileName);

                // check to ensure that the string is long enough to contain a guid... potential guid string example: 7C2D53DF-C77E-4E99-B739-6877DDAF165A
                if (!string.IsNullOrWhiteSpace(fileNameOnly) && fileNameOnly.Length >= 36)
                {
                    int delimiterPosition = fileNameOnly.LastIndexOf('_');

                    if (delimiterPosition > 0)
                    {
                        fileNameOnly = fileNameOnly.Substring(delimiterPosition + 1);
                    }

                    result = Guid.TryParse(fileNameOnly, out Guid guidResult);
                }
            }

            return result;
        }

        /// <summary>
        /// This method is used to remove a GUID from a file name.
        /// </summary>
        /// <param name="fileName">Contains the file name that does not include an extension and ends with a GUID value.</param>
        /// <returns>Returns the value of the file name with the GUID value removed.</returns>
        public static string RemoveGuidFromName(string fileName)
        {
            var newName = fileName;
            var anchorContents = string.Empty;

            if (!string.IsNullOrWhiteSpace(fileName))
            {
                // store contents after #
                int anchorCharLocation = newName.LastIndexOf('#');
                if (anchorCharLocation >= 0)
                {
                    anchorContents = newName.Substring(anchorCharLocation);
                    newName = newName.Substring(0, anchorCharLocation);
                }

                var path = Path.GetDirectoryName(newName) ?? string.Empty;
                var extension = Path.GetExtension(newName);
                newName = Path.GetFileNameWithoutExtension(newName);

                if (newName.Length > 36)
                {
                    var prefixIndex = newName.LastIndexOf('_');

                    if (prefixIndex >= 0)
                    {
                        newName = newName.Substring(0, prefixIndex);
                    }

                    newName = Path.Combine(path, newName + extension + anchorContents);
                }
                else
                {
                    newName = fileName;
                }
            }

            return newName;
        }

        /// <summary>
        /// This extension method is used to add a specified GUID to the component file name.
        /// </summary>
        /// <param name="fileName">Contains the file name to modify.</param>
        /// <param name="guidValue">Contains the GUID to include in the file name.</param>
        /// <returns>Returns a GUID with {fileName}_{guidValue}.extension filename</returns>
        public static string AddGuidToName(string fileName, Guid guidValue)
        {
            if (fileName == null)
            {
                throw new ArgumentNullException(nameof(fileName));
            }

            string justFileName = Path.GetFileName(fileName);

            // add the GUID to the end of the file name
            if (!string.IsNullOrWhiteSpace(justFileName))
            {
                string originalFileNameNoExtension = Path.GetFileNameWithoutExtension(justFileName);
                string originalExtension = Path.GetExtension(justFileName);

                // if the original has a GUID...
                if (NameContainsGuid(originalFileNameNoExtension))
                {
                    // remove the GUID
                    originalFileNameNoExtension = RemoveGuidFromName(originalFileNameNoExtension);
                }

                // get the original extension
                justFileName = $"{originalFileNameNoExtension}_{guidValue}{originalExtension}";
            }
            else
            {
                // default to the GUID and no extension
                justFileName = guidValue.ToString();
            }

            return justFileName;
        }

        /// <summary>
        /// This method is used to retrieve the GUID from a specified file name.
        /// </summary>
        /// <param name="fileName">Contains the filename from which a GUID shall be retrieved.</param>
        /// <returns>Returns the Guid object of the GUID found within the filename.</returns>
        public static Guid ParseGuidFromName(string fileName)
        {
            Guid result = Guid.Empty;

            if (!string.IsNullOrWhiteSpace(fileName) && fileName.Length >= 36)
            {
                // its long enough to contain a GUID, so continue...
                string fileNameOnly = Path.GetFileNameWithoutExtension(fileName);

                // check to ensure that the string is long enough to contain a guid... potential guid string example: 7C2D53DF-C77E-4E99-B739-6877DDAF165A
                if (!string.IsNullOrWhiteSpace(fileNameOnly) && fileNameOnly.Length >= 36)
                {
                    int delimiterPosition = fileNameOnly.LastIndexOf('_');

                    if (delimiterPosition > 0)
                    {
                        fileNameOnly = fileNameOnly.Substring(delimiterPosition + 1);
                    }

                    Guid.TryParse(fileNameOnly, out result);
                }
            }

            return result;
        }

        /// <summary>
        /// This method is used to strip a GUID from a filename in the format text_{GUID}.ext
        /// </summary>
        /// <param name="fileName">Contains the file name to strip a guid from.</param>
        /// <returns>Returns the filename without a GUID value.</returns>
        public static string StripGuidFromName(string fileName)
        {
            string originalFileNameNoExtension = Path.GetFileNameWithoutExtension(fileName);
            string originalExtension = Path.GetExtension(fileName);

            // if the original has a GUID...
            if (NameContainsGuid(originalFileNameNoExtension))
            {
                // remove the GUID
                originalFileNameNoExtension = RemoveGuidFromName(originalFileNameNoExtension);
            }

            return originalFileNameNoExtension + originalExtension;
        }

        #endregion Public Static Methods
    }
}