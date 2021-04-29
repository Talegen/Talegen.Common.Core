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
    using System.Xml;
    using System.Xml.Serialization;

    /// <summary>
    /// This method is used to enhance objects and strings adding additional serialization and deserialization properties to any type.
    /// </summary>
    public static class SerializeExtensions
    {
        /// <summary>
        /// This method is used to serialize an object to an XML file.
        /// </summary>
        /// <typeparam name="T">Contains the object type to serialize.</typeparam>
        /// <param name="value">Contains the object that is to be serialized.</param>
        /// <param name="serializeFilePath">Contains the file path where the serialized content will be stored.</param>
        /// <param name="xmlWriteSettings">Contains an instance of the XML writer settings object used to define settings for the serialized XML output.</param>
        /// <exception cref="ArgumentNullException">Exception thrown if no file path is specified</exception>
        public static void Serialize<T>(this T value, string serializeFilePath, XmlWriterSettings xmlWriteSettings = null)
        {
            if (string.IsNullOrWhiteSpace(serializeFilePath))
            {
                throw new ArgumentNullException(nameof(serializeFilePath), Properties.Resources.InvalidFileArgumentErrorText);
            }

            XmlSerializer objectSerializer = new XmlSerializer(typeof(T));

            // initialize write settings if none specified.
            if (xmlWriteSettings == null)
            {
                xmlWriteSettings = new XmlWriterSettings
                {
                    Indent = true,
                    IndentChars = " ",
                    Encoding = Encoding.UTF8,
                    NewLineChars = "\r\n",
                    NewLineOnAttributes = false,
                    NewLineHandling = NewLineHandling.Entitize
                };
            }

            using (XmlWriter xmlWriter = XmlWriter.Create(serializeFilePath, xmlWriteSettings))
            {
                objectSerializer.Serialize(xmlWriter, value);
            }
        }

        /// <summary>
        /// This method is used to serialize an object to an UTF-8 encoded file.
        /// </summary>
        /// <typeparam name="T">Contains the object type to serialize.</typeparam>
        /// <param name="value">Contains the object that is to be serialized.</param>
        /// <returns>Returns the XML content of the serialized object.</returns>
        /// <exception cref="ArgumentNullException">Exception thrown if no file path is specified.</exception>
        public static string Serialize<T>(this T value)
        {
            string result;

            XmlSerializer objectSerializer = new XmlSerializer(typeof(T));

            using (MemoryStream ms = new MemoryStream())
            {
                objectSerializer.Serialize(ms, value);
                result = Encoding.UTF8.GetString(ms.ToArray());
            }

            return result;
        }

        /// <summary>
        /// This method is used to deserialize an object from an XML file.
        /// </summary>
        /// <typeparam name="T">Contains the object type to deserialize.</typeparam>
        /// <param name="fileInfo">Contains an <see cref="FileInfo" /> object representing the file to deserialize.</param>
        /// <returns>Returns an object of the specified type <typeparamref name="T" />.</returns>
        /// <exception cref="FileNotFoundException">This exception is thrown if the file is not found.</exception>
        public static T Deserialize<T>(this FileInfo fileInfo)
        {
            if (!fileInfo.Exists)
            {
                throw new FileNotFoundException(Properties.Resources.DeserializeFileNotFoundErrorText, fileInfo.FullName);
            }

            return File.ReadAllText(fileInfo.FullName).Deserialize<T>();
        }

        /// <summary>
        /// This method is used to deserialize an object from an XML string.
        /// </summary>
        /// <typeparam name="T">Contains the object type to deserialize.</typeparam>
        /// <param name="value">Contains the string value to deserialize.</param>
        /// <returns>Returns an object of the specified type <typeparamref name="T" />.</returns>
        public static T Deserialize<T>(this string value)
        {
            object result = null;

            if (!string.IsNullOrWhiteSpace(value))
            {
                XmlSerializer objectSerializer = new XmlSerializer(typeof(T));
                using (StringReader contentReader = new StringReader(value))
                {
                    result = (T)objectSerializer.Deserialize(contentReader);
                }
            }

            return (T)result;
        }
    }
}