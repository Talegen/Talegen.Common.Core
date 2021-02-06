namespace Talegen.Common.Core.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;

    /// <summary>
    /// Contains an enumerated list of hash methods available.
    /// </summary>
    public enum HashMethod
    {
        /// <summary>
        /// SHA2-256 bit (32 byte) hashing algorithm.
        /// </summary>
        SHA256,

        /// <summary>
        /// SHA2-512 (64 byte) bit hashing algorithm.
        /// </summary>
        /// <remarks>The best hashing algorithm for security purposes.</remarks>
        SHA512,
    }

    /// <summary>
    /// This class contains extension methods for cryptographic hashing of string data.
    /// </summary>
    public static class HashExtensions
    {
        /// <summary>
        /// This method is used to hash binary data using the specified hashing algorithm.
        /// </summary>
        /// <param name="textData">Contains the text to hash.</param>
        /// <param name="hashMethod">Contains the hashing algorithm to use to hash the specified data.</param>
        /// <param name="maximumLength">Contains the maximum number of hash bytes to return in the hash result.</param>
        /// <returns>Returns a hex-encoded string representing the bytes of the hash result.</returns>
        public static string ToHashString(this string textData, HashMethod hashMethod = HashMethod.SHA256, int maximumLength = 0)
        {
            return textData.ToHash(hashMethod, maximumLength).ToHexString();
        }

        /// <summary>
        /// This method is used to hash XML data using the specified hashing algorithm.
        /// </summary>
        /// <param name="textData">Contains the text to hash.</param>
        /// <param name="hashMethod">Contains the hashing algorithm to use to hash the specified data.</param>
        /// <param name="maximumLength">Contains the maximum number of hash bytes to return in the hash result.</param>
        /// <returns>Returns a hex-encoded string representing the bytes of the hash result.</returns>
        public static string UnicodeToHashString(this string textData, HashMethod hashMethod = HashMethod.SHA256, int maximumLength = 0)
        {
            return Encoding.Unicode.GetBytes(textData).ToHash(hashMethod, maximumLength).ToHexString();
        }

        /// <summary>
        /// This method is used to hash binary data using the specified hashing algorithm.
        /// </summary>
        /// <param name="binaryData">Contains the binary data array to hash.</param>
        /// <param name="hashMethod">Contains the hashing algorithm to use to hash the specified data.</param>
        /// <param name="maximumLength">Contains the maximum number of hash bytes to return in the hash result.</param>
        /// <returns>Returns a hex-encoded string representing the bytes of the hash result.</returns>
        public static string ToHashString(this byte[] binaryData, HashMethod hashMethod = HashMethod.SHA256, int maximumLength = 0)
        {
            return binaryData.ToHash(hashMethod, maximumLength).ToHexString();
        }

        /// <summary>
        /// This method is used to hash binary data using the specified hashing algorithm.
        /// </summary>
        /// <param name="fileInfo">Contains a <c>FileInfo</c> containing the file to hash.</param>
        /// <param name="hashMethod">Contains the hashing algorithm to use to hash the specified data.</param>
        /// <returns>Returns a hex-encoded string representing the bytes of the file hash result.</returns>
        public static string ToHashString(this FileInfo fileInfo, HashMethod hashMethod = HashMethod.SHA256)
        {
            if (fileInfo == null)
            {
                throw new ArgumentNullException(nameof(fileInfo));
            }

            return File.ReadAllBytes(fileInfo.FullName).ToHash(hashMethod).ToHexString();
        }

        /// <summary>
        /// This method is used to hash binary data using the specified hashing algorithm.
        /// </summary>
        /// <param name="textData">Contains the text to hash.</param>
        /// <param name="hashMethod">Contains the hashing algorithm to use to hash the specified data.</param>
        /// <param name="maximumLength">Contains the maximum number of hash bytes to return in the hash result.</param>
        /// <returns>Returns a byte array of hashed bytes</returns>
        public static byte[] ToHash(this string textData, HashMethod hashMethod = HashMethod.SHA256, int maximumLength = 0)
        {
            return Encoding.UTF8.GetBytes(textData).ToHash(hashMethod, maximumLength);
        }

        /// <summary>
        /// This method is used to hash binary data using the specified hashing algorithm.
        /// </summary>
        /// <param name="binaryData">Contains the binary data array to hash.</param>
        /// <param name="hashMethod">Contains the hashing algorithm to use to hash the specified data.</param>
        /// <param name="maximumLength">Contains the maximum number of hash bytes to return in the hash result.</param>
        /// <returns>Returns a byte array of hashed bytes</returns>
        public static byte[] ToHash(this byte[] binaryData, HashMethod hashMethod = HashMethod.SHA256, int maximumLength = 0)
        {
            byte[] result = Array.Empty<byte>();
            HashAlgorithm hashTransform = null;

            switch (hashMethod)
            {
                case HashMethod.SHA256:
                    hashTransform = SHA256.Create();
                    break;

                case HashMethod.SHA512:
                    hashTransform = SHA512.Create();
                    break;
            }

            if (hashTransform != null)
            {
                result = hashTransform.ComputeHash(binaryData);

                if (maximumLength > 0)
                {
                    result = result.Take(maximumLength).ToArray();
                }

                hashTransform.Dispose();
            }

            return result;
        }

        /// <summary>
        /// Converts a binary byte array to a hex encoded string.
        /// </summary>
        /// <param name="bytesIn">byte array of binary data.</param>
        /// <returns>Returns a hex encoded string.</returns>
        public static string ToHexString(this IEnumerable<byte> bytesIn)
        {
            return bytesIn.Aggregate(string.Empty, (current, t) => current + $"{t:x2}");
        }
    }
}