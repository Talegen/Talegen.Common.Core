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
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;
    using Talegen.Common.Core.Properties;

    /// <summary>
    /// This class contains a number of extensions for encrypting and decrypting data using the AES cryptography algorithm.
    /// </summary>
    public static class CryptoExtensions
    {
        #region Private Constants

        /// <summary>
        /// Contains the block bit size for encryption
        /// </summary>
        private const int BlockBitSize = 128;

        /// <summary>
        /// Contains the key bit size
        /// </summary>
        private const int KeyBitSize = 256;

        /// <summary>
        /// Contains the salt bit size
        /// </summary>
        private const int SaltBitSize = 64;

        /// <summary>
        /// Contains the number of iterations
        /// </summary>
        private const int Iterations = 10000;

        /// <summary>
        /// Contains alpha-characters for password generation.
        /// </summary>
        private const string AlphanumericCharacters =
            "ABCDEFGHIJKLMNOPQRSTUVWXYZ" +
            "abcdefghijklmnopqrstuvwxyz" +
            "0123456789";

        #endregion Private Constants

        #region Public Static Methods

        /// <summary>
        /// This method is used to generate a random alpha-numeric string.
        /// </summary>
        /// <param name="length">Contains the length of the string.</param>
        /// <param name="characterSet">Optionally, contains the set of characters used for character selection.</param>
        /// <returns>Returns a string containing random characters.</returns>
        public static string RandomAlphaString(int length, IEnumerable<char> characterSet = null)
        {
            char[] result;

            if (length < 0 || length >= int.MaxValue)
            {
                length = 10;
            }

            if (characterSet == null)
            {
                characterSet = AlphanumericCharacters;
            }

            var characterArray = characterSet.Distinct().ToArray();
            var bytes = new byte[length * 8];

            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(bytes);
                result = new char[length];

                for (int i = 0; i < length; i++)
                {
                    ulong value = BitConverter.ToUInt64(bytes, i * 8);
                    result[i] = characterArray[value % (uint)characterArray.Length];
                }
            }

            return new string(result);
        }

        /// <summary>
        /// This method is used to generate random byte data encoded as a hex string.
        /// </summary>
        /// <param name="length">Contains the length of the random bytes.</param>
        /// <returns>Returns an hex-encoded string of random characters of the specified length.</returns>
        public static string RandomString(int length = 10)
        {
            return RandomBytes(length).ToHexString();
        }

        /// <summary>
        /// This method is used to generate random byte data.
        /// </summary>
        /// <param name="length">Contains the length of the random bytes.</param>
        /// <returns>Returns an array of random bytes of the specified length.</returns>
        public static byte[] RandomBytes(int length = 10)
        {
            if (length <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(length));
            }

            byte[] randomBytes = new byte[length];

            // generate cryptographically secure random bytes
            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomBytes);
            }

            return randomBytes;
        }

        /// <summary>
        /// This method is used to encrypt the specified plain data.
        /// </summary>
        /// <param name="plainData">Contains the plain text data to encrypt.</param>
        /// <param name="password">Contains the password.</param>
        /// <returns>Returns a string containing the base-64 encoded encrypted data stream.</returns>
        public static string Encrypt(this string plainData, string password)
        {
            return !string.IsNullOrEmpty(plainData) ? Convert.ToBase64String(Encoding.UTF8.GetBytes(plainData).Encrypt(password)) : string.Empty;
        }

        /// <summary>
        /// This method is used to encrypt the specified plain data.
        /// </summary>
        /// <param name="plainData">Contains the plain text data to encrypt.</param>
        /// <param name="password">Contains the password.</param>
        /// <returns>Returns a byte array containing the encrypted data stream.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Security", "CA5379:Do Not Use Weak Key Derivation Function Algorithm", Justification = "implementing RSA RFC2898 for encrypting password key.")]
        public static byte[] Encrypt(this byte[] plainData, string password)
        {
            string passwordKey = password.Base64Decode().ToHashString();

            if (plainData == null || plainData.Length == 0)
            {
                throw new ArgumentNullException(nameof(plainData));
            }

            // build the non-secret payload array
            byte[] payload = new byte[(SaltBitSize / 8) * 2];

            // non-secret payload is formatted as such [cryptKey-Salt (8-bytes)][authKey-Salt (8-bytes)]
            int payloadIndex = 0;

            byte[] cryptKey;
            byte[] authKey;

            // Use Random Salt to prevent pre-generated weak password attacks.
            using (var generator = new Rfc2898DeriveBytes(passwordKey, SaltBitSize / 8, Iterations))
            {
                // generate a salt
                byte[] salt = generator.Salt;

                // Generate Keys
                cryptKey = generator.GetBytes(KeyBitSize / 8);

                // Create first part of Non-Secret Payload
                Array.Copy(salt, 0, payload, payloadIndex, salt.Length);
                payloadIndex += salt.Length;
            }

            // Deriving separate key, might be less efficient than using HKDF, but now compatible with RNG crypto which had a very similar wire format and
            // requires less code than HKDF.
            using (var generator = new Rfc2898DeriveBytes(passwordKey, SaltBitSize / 8, Iterations))
            {
                byte[] salt = generator.Salt;

                // Generate Keys
                authKey = generator.GetBytes(KeyBitSize / 8);

                // Create second part of the Non-Secret Payload
                Array.Copy(salt, 0, payload, payloadIndex, salt.Length);
            }

            // generate encrypted results using crypt and authorization with payload
            return plainData.Encrypt(cryptKey, authKey, payload);
        }

        /// <summary>
        /// This method is used to encrypt the specified plain data
        /// </summary>
        /// <param name="plainData">Contains the plain text data to encrypt.</param>
        /// <param name="cryptKey">Contains the crypto key data.</param>
        /// <param name="authKey">Contains the authorize key data.</param>
        /// <param name="nonSecretPayload">Contains additional salt non-secret payload data.</param>
        /// <returns>Returns a byte array containing the encrypted data stream.</returns>
        public static byte[] Encrypt(this byte[] plainData, byte[] cryptKey, byte[] authKey, byte[] nonSecretPayload = null)
        {
            // if no crypt key or the length is invalid
            if (cryptKey == null || cryptKey.Length != KeyBitSize / 8)
            {
                throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, Resources.CryptoKeyLengthErrorText, KeyBitSize), nameof(cryptKey));
            }

            if (authKey == null || authKey.Length != KeyBitSize / 8)
            {
                throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, Resources.CryptoAuthKeyLengthErrorText, KeyBitSize), nameof(authKey));
            }

            if (plainData == null || plainData.Length == 0)
            {
                throw new ArgumentNullException(nameof(plainData), Resources.CryptoMustSpecifyTextToEncryptErrorText);
            }

            // non-secret payload
            nonSecretPayload = nonSecretPayload ?? Array.Empty<byte>();

            byte[] cipherText;
            byte[] iv;
            byte[] results;

            using (var aes = new AesCryptoServiceProvider
            {
                KeySize = KeyBitSize,
                BlockSize = BlockBitSize,
                Mode = CipherMode.CBC,
                Padding = PaddingMode.PKCS7
            })
            {
                // Use a random IV
                aes.GenerateIV();
                iv = aes.IV;

                // create a new AES encryption engine...
                using (ICryptoTransform crypto = aes.CreateEncryptor(cryptKey, iv))
                {
                    // create a new memory stream to write output to...
                    using (var cipherStream = new MemoryStream())
                    {
                        // create a new crypto stream for encryption
                        using (var cryptoStream = new CryptoStream(cipherStream, crypto, CryptoStreamMode.Write))
                        {
                            // create a new binary writer...
                            using (var binaryWriter = new BinaryWriter(cryptoStream))
                            {
                                // Encrypt Data
                                binaryWriter.Write(plainData);
                            }
                        }

                        // return encrypted results from stream
                        cipherText = cipherStream.ToArray();
                    }
                }
            }

            // Assemble encrypted message and add authentication
            using (var hmac = new HMACSHA256(authKey))
            {
                // create a new memory stream...
                using (var encryptedStream = new MemoryStream())
                {
                    // create a new binary writer...
                    using (var binaryWriter = new BinaryWriter(encryptedStream))
                    {
                        // Prefix non-secret payload if any
                        binaryWriter.Write(nonSecretPayload);

                        // Prefix IV
                        binaryWriter.Write(iv);

                        // Write encrypted data
                        binaryWriter.Write(cipherText);
                        binaryWriter.Flush();

                        // Authenticate all data
                        var hashTag = hmac.ComputeHash(encryptedStream.ToArray());

                        // Append hashed tag
                        binaryWriter.Write(hashTag);
                    }

                    // the final format is as follows: [nonSecretPayload:{crypt-salt}{auth-salt}][IV][CipherText][HMAC-256]
                    results = encryptedStream.ToArray();
                }
            }

            return results;
        }

        /// <summary>
        /// This method is used to decrypt the specified encrypted data
        /// </summary>
        /// <param name="cryptoData">Contains the encrypted data to decrypt.</param>
        /// <param name="password">Contains the password.</param>
        /// <param name="base64Encoded">Contains a value indicating whether the encrypted string is base-64 encoded and should be decoded first.</param>
        /// <returns>Returns the original decrypted text.</returns>
        public static string Decrypt(this string cryptoData, string password, bool base64Encoded = true)
        {
            if (string.IsNullOrWhiteSpace(cryptoData))
            {
                throw new ArgumentNullException(nameof(cryptoData));
            }

            byte[] cipherText = base64Encoded ? Convert.FromBase64String(cryptoData) : Encoding.UTF8.GetBytes(cryptoData);
            return Encoding.UTF8.GetString(cipherText.Decrypt(password));
        }

        /// <summary>
        /// This method is used to decrypt the specified encrypted data.
        /// </summary>
        /// <param name="cryptoData">Contains the encrypted data to decrypt.</param>
        /// <param name="password">Contains the password.</param>
        /// <returns>Returns the original decrypted text.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Security", "CA5379:Do Not Use Weak Key Derivation Function Algorithm", Justification = "implementing RSA RFC2898 for encrypting password key.")]
        public static byte[] Decrypt(this byte[] cryptoData, string password)
        {
            string passwordKey = password.Base64Decode().ToHashString();

            if (cryptoData == null || cryptoData.Length == 0)
            {
                throw new ArgumentNullException(nameof(cryptoData), Resources.CryptoMustSpecifyCryptoTextToDecryptErrorText);
            }

            var cryptSalt = new byte[SaltBitSize / 8];
            var authSalt = new byte[SaltBitSize / 8];

            // Grab Salt from Non-Secret Payload
            Array.Copy(cryptoData, 0, cryptSalt, 0, cryptSalt.Length);
            Array.Copy(cryptoData, cryptSalt.Length, authSalt, 0, authSalt.Length);

            byte[] cryptKey;
            byte[] authKey;

            // Generate crypt key
            using (var generator = new Rfc2898DeriveBytes(passwordKey, cryptSalt, Iterations))
            {
                cryptKey = generator.GetBytes(KeyBitSize / 8);
            }

            // Generate auth key
            using (var generator = new Rfc2898DeriveBytes(passwordKey, authSalt, Iterations))
            {
                authKey = generator.GetBytes(KeyBitSize / 8);
            }

            return cryptoData.Decrypt(cryptKey, authKey, cryptSalt.Length + authSalt.Length);
        }

        /// <summary>
        /// This method is used to decrypt the specified encrypted data.
        /// </summary>
        /// <param name="cryptoData">Contains the encrypted data to decrypt.</param>
        /// <param name="cryptKey">Contains the crypto key data.</param>
        /// <param name="authKey">Contains the authorize key data.</param>
        /// <param name="nonSecretPayloadLength">Contains the length of the additional payload used in encryption.</param>
        /// <returns>Returns the original decrypted text.</returns>
        public static byte[] Decrypt(this byte[] cryptoData, byte[] cryptKey, byte[] authKey, int nonSecretPayloadLength = 0)
        {
            byte[] result = Array.Empty<byte>();

            // if no crypt key or the length is invalid
            if (cryptKey == null || cryptKey.Length != KeyBitSize / 8)
            {
                throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, Resources.CryptoKeyLengthErrorText, KeyBitSize), nameof(cryptKey));
            }

            if (authKey == null || authKey.Length != KeyBitSize / 8)
            {
                throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, Resources.CryptoAuthKeyLengthErrorText, KeyBitSize), nameof(authKey));
            }

            if (cryptoData == null || cryptoData.Length == 0)
            {
                throw new ArgumentNullException(nameof(cryptoData), Resources.CryptoMustSpecifyCryptoTextToDecryptErrorText);
            }

            // generate a hash first to check validation
            using (var hmac = new HMACSHA256(authKey))
            {
                byte[] sentTag = new byte[hmac.HashSize / 8];

                // Calculate Tag sans the hash tag suffix
                var calcTag = hmac.ComputeHash(cryptoData, 0, cryptoData.Length - sentTag.Length);
                var vectorLength = BlockBitSize / 8;

                // if message length is to small just return null
                if (cryptoData.Length >= sentTag.Length + nonSecretPayloadLength + vectorLength)
                {
                    // Grab Sent hash Tag
                    Array.Copy(cryptoData, cryptoData.Length - sentTag.Length, sentTag, 0, sentTag.Length);

                    // Compare Tag with constant time comparison
                    var compare = 0;
                    for (var i = 0; i < sentTag.Length; i++)
                    {
                        compare |= sentTag[i] ^ calcTag[i];
                    }

                    // if hash authenticated OK, continue...
                    if (compare == 0)
                    {
                        // create a new AES crypto engine...
                        using (var aes = new AesCryptoServiceProvider
                        {
                            KeySize = KeyBitSize,
                            BlockSize = BlockBitSize,
                            Mode = CipherMode.CBC,
                            Padding = PaddingMode.PKCS7
                        })
                        {
                            // Grab IV from message
                            var iv = new byte[vectorLength];
                            Array.Copy(cryptoData, nonSecretPayloadLength, iv, 0, iv.Length);

                            // create a new decryption engine...
                            using (var crypto = aes.CreateDecryptor(cryptKey, iv))
                            using (MemoryStream plainTextStream = new MemoryStream())
                            {
                                // create a new crypto stream...
                                using (var cryptoStream = new CryptoStream(plainTextStream, crypto, CryptoStreamMode.Write))
                                using (var binaryWriter = new BinaryWriter(cryptoStream))
                                {
                                    // Decrypt Cipher Text from Message
                                    binaryWriter.Write(cryptoData,
                                        nonSecretPayloadLength + iv.Length,
                                        cryptoData.Length - nonSecretPayloadLength - iv.Length - sentTag.Length);
                                }

                                // Return Plain Text as secure string
                                result = plainTextStream.ToArray();
                            }
                        }
                    }
                }
            }

            return result;
        }

        #endregion Public Static Methods
    }
}