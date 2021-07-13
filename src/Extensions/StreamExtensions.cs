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

namespace Talegen.Common.Core.Extensions
{
    using System;
    using System.IO;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// This class contains extension methods for helping with reading and writing from Stream objects.
    /// </summary>
    public static class StreamExtensions
    {
        /// <summary>
        /// This method is used to read and return a string of data from a specified stream.
        /// </summary>
        /// <param name="stream">Contains the stream to read string data from.</param>
        /// <param name="bufferSize">Contains an optional byte buffer read size.</param>
        /// <returns>Returns stream data as a byte array.</returns>
        public static byte[] ReadAllBytes(this Stream stream, int bufferSize = 4096)
        {
            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            if (bufferSize <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(bufferSize));
            }

            using (MemoryStream ms = new MemoryStream((int)stream.Length))
            {
                byte[] buffer = new byte[bufferSize];
                int bytesRead;

                while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) != 0)
                {
                    ms.Write(buffer, 0, bytesRead);
                }

                return ms.ToArray();
            }
        }

        /// <summary>
        /// Reads all bytes asynchronous.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="bufferSize">Size of the buffer.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Returns stream data as a byte array.</returns>
        /// <exception cref="ArgumentNullException">stream</exception>
        /// <exception cref="ArgumentOutOfRangeException">bufferSize</exception>
        public static async Task<byte[]> ReadAllBytesAsync(this Stream stream, int bufferSize = 4096, CancellationToken cancellationToken = default)
        {
            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            if (bufferSize <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(bufferSize));
            }

            using (MemoryStream ms = new MemoryStream((int)stream.Length))
            {
                byte[] buffer = new byte[bufferSize];
                int bytesRead;

                while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length, cancellationToken)) != 0)
                {
                    await ms.WriteAsync(buffer, 0, bytesRead, cancellationToken);
                }

                return ms.ToArray();
            }
        }

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
        /// This extension method is used to write a string to the specified stream asynchronously.
        /// </summary>
        /// <param name="stream">Contains the stream that will be written to.</param>
        /// <param name="content">Contains the string content to write to the stream.</param>
        /// <param name="encoder">Contains the optional encoder used to define the string encoding.</param>
        /// <param name="cancellationToken">Contains an optional cancellation token.</param>
        /// <exception cref="ArgumentNullException">stream</exception>
        public static async Task WriteStringAsync(this Stream stream, string content, Encoding encoder = null, CancellationToken cancellationToken = default)
        {
            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            byte[] outputBytes = content.ToByteArray(encoder);
            await stream.WriteAsync(outputBytes, 0, outputBytes.Length, cancellationToken);
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
        /// This method is used to read and return a string of data from a specified stream.
        /// </summary>
        /// <param name="stream">Contains the stream to read string data from.</param>
        /// <param name="encoder">Contains an optional encoder to use for reading the byte data.</param>
        /// <param name="detectEncodingFromByteOrderMarks">Contains an optional value indicating that byte order marks should be used to determine encoding.</param>
        /// <param name="bufferSize">Contains an optional byte buffer read size.</param>
        /// <returns>Returns stream data as a string.</returns>
        /// <exception cref="ArgumentNullException">stream</exception>
        public static async Task<string> ReadStringAsync(this Stream stream, Encoding encoder = null, bool detectEncodingFromByteOrderMarks = false, int bufferSize = 4096)
        {
            string result;

            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            using (TextReader reader = new StreamReader(stream, encoder ?? Encoding.Default, detectEncodingFromByteOrderMarks, bufferSize))
            {
                result = await reader.ReadToEndAsync();
            }

            return result;
        }
    }
}