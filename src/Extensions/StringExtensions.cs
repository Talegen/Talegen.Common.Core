namespace Talegen.Common.Core.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using Talegen.Common.Core.Properties;

    /// <summary>
    /// This class contains methods to aid in manipulating strings.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// This extension method is used to write a string to the specified stream/
        /// </summary>
        /// <param name="stream">Contains the stream that will be written to.</param>
        /// <param name="content">Contains the string content to write to the stream.</param>
        /// <param name="encoder">Contains the optional encoder used to define the string encoding.</param>
        public static void WriteString(this Stream stream, string content, Encoding encoder = null)
        {
            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            byte[] outputBytes = content.ToByteArray(encoder);
            stream.Write(outputBytes, 0, outputBytes.Length);
        }

        /// <summary>
        /// This method is used to read and return a string of data from a specified stream.
        /// </summary>
        /// <param name="stream">Contains the stream to read string data from.</param>
        /// <param name="encoder">Contains an optional encoder to use for reading the byte data.</param>
        /// <param name="detectEncodingFromByteOrderMarks">Contains an optional value indicating that byte order marks should be used to determine encoding.</param>
        /// <param name="bufferSize">Contains an optional byte buffer read size.</param>
        /// <returns>Returns stream data as a string.</returns>
        public static string ReadString(this Stream stream, Encoding encoder = null, bool detectEncodingFromByteOrderMarks = false, int bufferSize = 4096)
        {
            string result;

            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            using (TextReader reader = new StreamReader(stream, encoder ?? Encoding.Default, detectEncodingFromByteOrderMarks, bufferSize))
            {
                result = reader.ReadToEnd();
            }

            return result;
        }

        /// <summary>
        /// This extension method is used to convert a string to an array of bytes using the specified optional encoder.
        /// </summary>
        /// <param name="content">Contains the content string to convert to bytes.</param>
        /// <param name="encoder">Contains the optional encoder to use during the conversion. A default Unicode encoder is used if not specified.</param>
        /// <returns>Returns the specified string as a byte array.</returns>
        public static byte[] ToByteArray(this string content, Encoding encoder = null)
        {
            content = content ?? string.Empty;
            Encoding encoderUsed = encoder ?? Encoding.Default;
            return encoderUsed.GetBytes(content);
        }

        /// <summary>
        /// This extension method is used to determine if a string contains Base-64 encoded text.
        /// </summary>
        /// <param name="content">Contains the content to analyze.</param>
        /// <returns>Returns a value indicating whether the content string contains Base-64 encoded text.</returns>
        public static bool IsBase64String(this string content)
        {
            bool result = false;

            if (!string.IsNullOrEmpty(content))
            {
                content = content.Trim();
                result = (content.Length % 4 == 0) && Regex.IsMatch(content, @"^[a-zA-Z0-9\+/]*={0,3}$", RegexOptions.None);
            }

            return result;
        }

        /// <summary>
        /// This extension method is used to convert a byte array into Base-64 encoded string.
        /// </summary>
        /// <param name="content">Contains the byte array to convert to a Base-64 encoded string.</param>
        /// <returns>Returns the Base-64 encoded string.</returns>
        public static string Base64Encode(this byte[] content)
        {
            return content != null ? Convert.ToBase64String(content) : string.Empty;
        }

        /// <summary>
        /// This extension method is used to convert a Base-64 encoded string into a binary byte array.
        /// </summary>
        /// <param name="content">Contains the Base-64 encoded string to convert.</param>
        /// <returns>Returns the decoded string content or the content if not a Base-64 string as a UTF8 byte array.</returns>
        public static byte[] Base64Decode(this string content)
        {
            if (content.IsBase64String())
            {
                return Convert.FromBase64String(content);
            }
            else
            {
                return Encoding.UTF8.GetBytes(content);
            }
        }

        /// <summary>
        /// This extension method is used to split a large string into an enumerated list of smaller strings of the a length no larger than the specified
        /// maximum length.
        /// </summary>
        /// <param name="text">Contains the text that is to be split into an enumerable list of strings of the specified maximum length.</param>
        /// <param name="maxLength">Contains the maximum size of each individual split string value returned.</param>
        /// <returns>Contains an enumerated list of strings that have been split apart from the source string.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException"><c>maxLength</c> must be greater than 0.</exception>
        public static IEnumerable<string> Chop(this string text, int maxLength)
        {
            if (maxLength <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(maxLength), string.Format(CultureInfo.InvariantCulture, Resources.ArgumentMustBeGreaterThanZeroErrorFormat, nameof(maxLength)));
            }

            if (text == null)
            {
                text = string.Empty;
            }

            return text.Select((x, i) => i)
                       .Where(i => i % maxLength == 0)
                       .Select(i => text.Substring(i, text.Length - i >= maxLength ? maxLength : text.Length - i));
        }

        /// <summary>
        /// This method is used to determine if a specified string matches a regular expression.
        /// </summary>
        /// <param name="text">Contains the text to match with the regular expression.</param>
        /// <param name="regularExpression">Contains the regular expression text.</param>
        /// <returns>Returns a value indicating whether the text successfully matches the regular expression.</returns>
        public static bool Match(this string text, string regularExpression)
        {
            Regex reg = new Regex(regularExpression);
            return reg.Match(text).Success;
        }

        /// <summary>
        /// This method is used to determine if a specified string contains a valid e-mail address.
        /// </summary>
        /// <param name="emailAddress">Contains the string to evaluate.</param>
        /// <returns>Returns a value indicating whether the string contains a valid e-mail address.</returns>
        public static bool IsEmail(this string emailAddress)
        {
            return Regex.IsMatch(emailAddress, @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// This method is used to get a substring before a specified character is found within the string.
        /// </summary>
        /// <param name="source">Contains the source string to search within.</param>
        /// <param name="characterToFind">Contains the character to find within the specified source string.</param>
        /// <returns>
        /// Returns all characters before the specified character within the source string. If the character is not found, the entire source string is returned.
        /// If the character is the only character within the source string, an empty string is returned.
        /// </returns>
        public static string Before(this string source, char characterToFind)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            int index = source.IndexOf(characterToFind);
            return index > -1 ? (index > 0 ? source.Substring(0, index) : string.Empty) : source;
        }

        /// <summary>
        /// This method is used to get a substring after a specified character is found within the string.
        /// </summary>
        /// <param name="source">Contains the source string to search within.</param>
        /// <param name="characterToFind">Contains the character to find within the specified source string.</param>
        /// <returns>
        /// Returns all characters after the specified character within a source string. If the character is not found, the entire source string is returned.
        /// </returns>
        public static string After(this string source, char characterToFind)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            int index = source.IndexOf(characterToFind) + 1;
            return index > 0 && index < source.Length ? source.Substring(index) : source;
        }
    }
}