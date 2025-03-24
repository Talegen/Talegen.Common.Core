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
    using System.ComponentModel;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.Serialization;
    using Properties;
    using Talegen.Common.Core.Attributes;

    /// <summary>
    /// This class contains a number of extension methods for conversion of data values.
    /// </summary>
    public static class TypeExtensions
    {
        /// <summary>
        /// This method is used to convert a string representation of a GUID into a Guid structure.
        /// </summary>
        /// <param name="value">Contains the string value containing a GUID value.</param>
        /// <returns>Returns the parsed string as a Guid structure.</returns>
        public static Guid ToGuid(this string value)
        {
            Guid.TryParse(value, out var result);
            return result;
        }

        /// <summary>
        /// This method is used to convert a potentially local date-time to a UTC kind date. This is used in scenarios where all dates are expected to be
        /// handled as UTC instead of allowing localization to manipulate dates.
        /// </summary>
        /// <param name="value">Contains the potentially local date-time to convert to UTC kind.</param>
        /// <returns>Returns a new date with the same values as the original date, only formatted as UTC localization.</returns>
        public static DateTime ToUtcKindDate(this DateTime value)
        {
            return new DateTime(value.Year, value.Month, value.Day, value.Hour, value.Minute, value.Second, DateTimeKind.Utc);
        }

        /// <summary>
        /// This method is used to provide a short date conversion value for a null date time object. By default null date time will not offer this method, so
        /// this extension method handles the necessary logic to provide this shorthand.
        /// </summary>
        /// <param name="value">Contains the null date time object to convert to a short date string.</param>
        /// <returns>Returns a short date string if the null date time has a value.</returns>
        public static string ToShortDateString(this DateTime? value)
        {
            return value?.ToShortDateString() ?? string.Empty;
        }

        /// <summary>
        /// This method is used to convert a password string to a password hint string.
        /// </summary>
        /// <param name="passwordText">Contains the password text to convert to a password hint text.</param>
        /// <param name="passwordReplaceCharacter">Contains the character to use as the hidden password character.</param>
        /// <param name="showBeginningCharacter">Contains a value indicating whether the first password character is shown.</param>
        /// <returns>Returns a password hint value of the specified password string. E.g. "*******" or "P*******" for "Password".</returns>
        public static string ToPasswordHint(this string passwordText, char passwordReplaceCharacter = '*', bool showBeginningCharacter = false)
        {
            passwordText = passwordText.ConvertToString();
            string buffer = new string(passwordReplaceCharacter, passwordText.Length);
            return showBeginningCharacter ? passwordText[0] + buffer.Substring(0, buffer.Length - 2) : buffer;
        }

        /// <summary>
        /// Truncates a string to a maximum length.
        /// </summary>
        /// <param name="value">Contains the value to truncate.</param>
        /// <param name="maximumLength">Contains the maximum number of bytes to truncate.</param>
        /// <returns>Returns the truncated string.</returns>
        public static string Truncate(this string value, int maximumLength)
        {
            if (!string.IsNullOrWhiteSpace(value) && value.Length > maximumLength)
            {
                value = value.Substring(0, maximumLength);
            }

            return value;
        }

        /// <summary>
        /// Converts an object to an integer value returning default value on null or <see cref="DBNull" /> types.
        /// </summary>
        /// <param name="value">Contains the object value to convert.</param>
        /// <param name="defaultValue">Contains the default value to return if value cannot be converted.</param>
        /// <returns>Returns the converted or default integer value.</returns>
        public static int ToInt(this object value, int defaultValue = 0)
        {
            return value != null && value != DBNull.Value ? value.ToString().ToInt(defaultValue) : defaultValue;
        }

        /// <summary>
        /// Converts a string to an integer value returning default value for null or <see cref="DBNull" /> types.
        /// </summary>
        /// <param name="value">Contains the string value to convert.</param>
        /// <param name="defaultValue">Contains the default value to return if value cannot be converted.</param>
        /// <returns>Returns the converted or default integer value.</returns>
        public static int ToInt(this string value, int defaultValue = 0)
        {
            if (string.IsNullOrEmpty(value) || !int.TryParse(value, out var result))
            {
                result = defaultValue;
            }

            return result;
        }

        /// <summary>
        /// Converts an object to a long integer value returning default value on null or <see cref="DBNull" /> types.
        /// </summary>
        /// <param name="value">Contains the object value to convert.</param>
        /// <param name="defaultValue">Contains the default value to return if value cannot be converted.</param>
        /// <returns>Returns the converted or default long integer value.</returns>
        public static long ToLong(this object value, long defaultValue = 0)
        {
            return value != null && value != DBNull.Value ? value.ToString().ToLong(defaultValue) : defaultValue;
        }

        /// <summary>
        /// Converts a string to a long integer value returning default value for null or <see cref="DBNull" /> types.
        /// </summary>
        /// <param name="value">Contains the string value to convert.</param>
        /// <param name="defaultValue">Contains the default value to return if value cannot be converted.</param>
        /// <returns>Returns the converted or default long integer value.</returns>
        public static long ToLong(this string value, long defaultValue = 0)
        {
            if (string.IsNullOrEmpty(value) || !long.TryParse(value, out var result))
            {
                result = defaultValue;
            }

            return result;
        }

        /// <summary>
        /// Converts a string to an decimal value returning default value for null or <see cref="DBNull" /> types.
        /// </summary>
        /// <param name="value">Contains the string value to convert.</param>
        /// <param name="defaultValue">Contains the default value to return if value cannot be converted.</param>
        /// <returns>Returns the converted or default decimal value.</returns>
        public static decimal ToDecimal(this string value, decimal defaultValue = 0)
        {
            if (string.IsNullOrEmpty(value) || !decimal.TryParse(value, out var result))
            {
                result = defaultValue;
            }

            return result;
        }

        /// <summary>
        /// Converts an object to an decimal value returning default value on null or <see cref="DBNull" /> types.
        /// </summary>
        /// <param name="value">Contains the object value to convert.</param>
        /// <param name="defaultValue">Contains the default value to return if value cannot be converted.</param>
        /// <returns>Returns the converted or default decimal value.</returns>
        public static decimal ToDecimal(this object value, decimal defaultValue = 0)
        {
            return value != null && value != DBNull.Value ? value.ToString().ToDecimal() : defaultValue;
        }

        /// <summary>
        /// Converts an object to a string value returning default value on null or <see cref="DBNull" /> types.
        /// </summary>
        /// <param name="value">Contains the object value to convert.</param>
        /// <param name="defaultValue">Contains the default value to return if value cannot be converted.</param>
        /// <returns>Returns the converted or default string value.</returns>
        public static string ConvertToString(this object value, string defaultValue = "")
        {
            string result = defaultValue;

            if (value != null && value != DBNull.Value)
            {
                result = value.ToString();

                if (string.IsNullOrEmpty(result))
                {
                    result = defaultValue;
                }
            }

            return result;
        }

        /// <summary>
        /// Converts an object to a DateTime value returning default value on null or <see cref="DBNull" /> types.
        /// </summary>
        /// <param name="value">Contains the object value to convert.</param>
        /// <param name="defaultValue">Contains the default value to return if value cannot be converted.</param>
        /// <param name="dateFormatter">Contains the globalization formatter for converting a date.</param>
        /// <param name="dateStyle">Contains the date style the string is conformed to.</param>
        /// <returns>Returns the converted or default DateTime value.</returns>
        public static DateTime ToDateTime(this object value, DateTime? defaultValue = null, IFormatProvider dateFormatter = null, DateTimeStyles dateStyle = DateTimeStyles.None)
        {
            DateTime result = defaultValue ?? DateTime.MinValue;

            if (value != null && value != DBNull.Value)
            {
                result = value.ToString().ToDateTime(defaultValue, dateFormatter, dateStyle);
            }

            return result;
        }

        /// <summary>
        /// Converts a string to an DateTime value returning default value for null or <see cref="DBNull" /> types.
        /// </summary>
        /// <param name="value">Contains the string value to convert.</param>
        /// <param name="defaultValue">Contains the default value to return if value cannot be converted.</param>
        /// <param name="dateFormatter">Contains the globalization formatter for converting a date.</param>
        /// <param name="dateStyle">Contains the date style the string is conformed to.</param>
        /// <returns>Returns the converted or default DateTime value.</returns>
        public static DateTime ToDateTime(this string value, DateTime? defaultValue = null, IFormatProvider dateFormatter = null, DateTimeStyles dateStyle = DateTimeStyles.None)
        {
            DateTime result = defaultValue ?? DateTime.MinValue;

            if (!string.IsNullOrWhiteSpace(value))
            {
                if (dateFormatter == null)
                {
                    dateFormatter = CultureInfo.InvariantCulture;
                }

                if (!DateTime.TryParse(value, dateFormatter, dateStyle, out result))
                {
                    result = defaultValue ?? DateTime.MinValue;
                }
            }

            return result;
        }

        /// <summary>
        /// Converts a string matching an exact string format to a DateTime value returning default value for null or <see cref="DBNull" /> types.
        /// </summary>
        /// <param name="value">Contains the object value to convert.</param>
        /// <param name="stringFormat">Contains the exact date format that the value to convert contains.</param>
        /// <param name="defaultValue">Contains the default value to return if value cannot be converted.</param>
        /// <param name="dateFormatter">Contains the globalization formatter for converting a date.</param>
        /// <returns>Returns the converted or default DateTime value.</returns>
        public static DateTime ToExactDateTime(this object value, string stringFormat, DateTime? defaultValue = null, IFormatProvider dateFormatter = null)
        {
            DateTime result = defaultValue ?? DateTime.MinValue;

            if (value != null && value != DBNull.Value)
            {
                result = value.ToString().ToExactDateTime(stringFormat, defaultValue, dateFormatter);
            }

            return result;
        }

        /// <summary>
        /// Converts a string matching an exact string format to a DateTime value returning default value for null or <see cref="DBNull" /> types.
        /// </summary>
        /// <param name="value">Contains the string value to convert.</param>
        /// <param name="stringFormat">Contains the exact date format that the value to convert contains.</param>
        /// <param name="defaultValue">Contains the default value to return if value cannot be converted.</param>
        /// <param name="dateFormatter">Contains the globalization formatter for converting a date.</param>
        /// <returns>Returns the converted or default DateTime value.</returns>
        public static DateTime ToExactDateTime(this string value, string stringFormat, DateTime? defaultValue = null, IFormatProvider dateFormatter = null)
        {
            DateTime result = defaultValue ?? DateTime.MinValue;

            if (string.IsNullOrWhiteSpace(stringFormat))
            {
                throw new ArgumentNullException(nameof(stringFormat));
            }

            if (!string.IsNullOrWhiteSpace(value))
            {
                try
                {
                    if (dateFormatter == null)
                    {
                        dateFormatter = CultureInfo.InvariantCulture;
                    }

                    result = DateTime.ParseExact(value, stringFormat, dateFormatter);
                }
                catch
                {
                    result = defaultValue ?? DateTime.MinValue;
                }
            }

            return result;
        }

        /// <summary>
        /// This method is used to convert a string value to an enumerated value of a specified type.
        /// </summary>
        /// <typeparam name="T">Contains the enumeration type.</typeparam>
        /// <param name="value">Contains the string value to convert.</param>
        /// <param name="ignoreCase"><c>true</c> to perform a case-insensitive search; otherwise <c>false</c>.</param>
        /// <param name="converter">Contains an optional function used for specialized string conversion to a given enumeration value.</param>
        /// <returns>Returns the enumerated value on success.</returns>
        public static T ToEnum<T>(this object value, bool ignoreCase = true, Func<string, T> converter = null)
        {
            return value.ConvertToString().ToEnum<T>(ignoreCase, converter);
        }

        /// <summary>
        /// This method is used to convert a string value to an enumerated value of a specified type.
        /// </summary>
        /// <typeparam name="T">Contains the enumeration type.</typeparam>
        /// <param name="value">Contains the string value to convert.</param>
        /// <param name="ignoreCase"><c>true</c> to perform a case-insensitive search; otherwise <c>false</c>.</param>
        /// <param name="converter">Contains an optional function used for specialized string conversion to a given enumeration value.</param>
        /// <returns>Returns the enumerated value on success.</returns>
        public static T ToEnum<T>(this string value, bool ignoreCase = true, Func<string, T> converter = null)
        {
            T result;

            if (converter == null)
            {
                result = (T)Enum.Parse(typeof(T), value, ignoreCase);
            }
            else
            {
                result = converter.Invoke(value);
            }

            return result;
        }

        /// <summary>
        /// This method is used to convert a character value into a string representing an enumeration value and then converting to a matching enumeration list selection.
        /// </summary>
        /// <typeparam name="T">Contains the enumeration type.</typeparam>
        /// <param name="value">Contains the string value to convert.</param>
        /// <param name="castDictionary">Contains a character string casting map for a character value and enumeration name value.</param>
        /// <returns>Returns the enumerated value on success.</returns>
        public static T ToEnum<T>(this char value, Dictionary<char, string> castDictionary)
        {
            return (T)Enum.Parse(typeof(T), castDictionary[value], true);
        }

        /// <summary>
        /// This method is used to convert an object value to a boolean.
        /// </summary>
        /// <param name="value">Contains the value to convert.</param>
        /// <param name="defaultValue">Contains a default value to return when the value is invalid.</param>
        /// <returns>Returns the converted value or default value.</returns>
        public static bool ToBoolean(this object value, bool defaultValue = false)
        {
            return ToBoolean(value.ConvertToString(), defaultValue);
        }

        /// <summary>
        /// This method is used to convert a string value to a boolean.
        /// </summary>
        /// <param name="value">Contains the value to convert.</param>
        /// <param name="defaultValue">Contains a default value to return when the value is invalid.</param>
        /// <returns>Returns the converted value or default value.</returns>
        public static bool ToBoolean(this string value, bool defaultValue = false)
        {
            bool result = defaultValue;
            string[] allowedPositives = { "T", "TRUE", "1", "Y", "YES", "O" };

            if (!string.IsNullOrWhiteSpace(value))
            {
                result = allowedPositives.Contains(value.ToUpperInvariant());
            }

            return result;
        }

        /// <summary>
        /// This method is used to find a <see cref="System.ComponentModel.DescriptionAttribute" /> value specified on an enumeration value.
        /// </summary>
        /// <typeparam name="T">Contains the type of enumeration to retrieve the description value for.</typeparam>
        /// <param name="enumerationValue">Contains the enumeration to search for a description.</param>
        /// <param name="resourceCulture">
        /// Contains an optional culture info object used to override the current threads culture info. Typically used when an enumerated description must be
        /// rendered in a different language than the user's profile preference.
        /// </param>
        /// <returns>Returns the enumeration description or the default ToString value.</returns>
        public static string ToDescription<T>(this T enumerationValue, CultureInfo resourceCulture = null) where T : struct
        {
            Type type = enumerationValue.GetType();

            if (!type.IsEnum)
            {
                throw new ArgumentException(Resources.InvalidArgumentMustBeEnumerationText, nameof(enumerationValue));
            }

            string result = enumerationValue.ToString();
            MemberInfo[] memberInfo = type.GetMember(enumerationValue.ToString());

            // if there were any members found...
            if (memberInfo.Length > 0)
            {
                // get the first member that has any custom attributes of the type we're looking for
                var member = memberInfo.FirstOrDefault(mi => mi.CustomAttributes.Any(ca => ca.AttributeType == typeof(DescriptionAttribute) ||
                                        ca.AttributeType == typeof(LocalizedDescriptionAttribute) ||
                                        ca.AttributeType == typeof(EnumMemberAttribute)));

                // if a member was found with either type, get custom attributes
                if (member != null)
                {
                    var localizedDescriptionAttribute = member.GetCustomAttributes<LocalizedDescriptionAttribute>(false).FirstOrDefault();

                    if (localizedDescriptionAttribute != null)
                    {
                        // get the previously set culture if any.
                        var previousCulture = Resources.Culture;

                        // if an override culture was specified...
                        if (resourceCulture != null)
                        {
                            // use the override culture for this instance
                            Resources.Culture = resourceCulture;
                        }
                        
                        // retrieve the resource stored description from the attribute.
                        result = localizedDescriptionAttribute.Description;

                        // reset the previous culture override when done
                        Resources.Culture = previousCulture;
                    }
                    else
                    {
                        var descriptionAttribute = member.GetCustomAttributes<DescriptionAttribute>(false).FirstOrDefault();

                        if (descriptionAttribute != null)
                        {
                            result = descriptionAttribute.Description;
                        }
                        else
                        {
                            var valueAttribute = member.GetCustomAttributes<EnumMemberAttribute>(false).FirstOrDefault();

                            if (valueAttribute != null)
                            {
                                result = valueAttribute.Value;
                            }
                            else
                            {
                                // fail out to just serializing the name.
                                result = enumerationValue.ToString();
                            }
                        }
                    }
                }
            }

            // If we have no description attribute, just return the ToString of the enumeration.
            return result;
        }

        /// <summary>
        /// This method is used to create a dictionary of description to enumeration value mappings for reverse enumeration value lookup.
        /// </summary>
        /// <typeparam name="T">Contains the type of enumeration to retrieve the description mapping for.</typeparam>
        /// <returns>Returns the enumeration character to enumeration mapping dictionary.</returns>
        public static Dictionary<string, T> ToCharMapDictionary<T>() where T : struct
        {
            Dictionary<string, T> resultDictionary = new Dictionary<string, T>();

            Type type = typeof(T);

            if (!type.IsEnum)
            {
                throw new ArgumentException(nameof(T));
            }

            string[] enumerationNames = type.GetEnumNames();

            if (enumerationNames.Length > 0)
            {
                foreach (string name in enumerationNames)
                {
                    // get all members of the enumeration
                    MemberInfo[] memberInfo = type.GetMember(name);

                    // if there were any members found...
                    if (memberInfo.Length > 0)
                    {
                        // get the first member that has any custom attributes of the type we're looking for
                        var member = memberInfo.FirstOrDefault(mi => mi.CustomAttributes.Any(ca => ca.AttributeType == typeof(DescriptionAttribute)));

                        // if a member was found with either type, get custom attribtes
                        if (member != null)
                        {
                            var descriptionAttribute = member.GetCustomAttributes<DescriptionAttribute>(false).FirstOrDefault();
                            if (descriptionAttribute != null && !resultDictionary.ContainsKey(descriptionAttribute.Description))
                            {
                                resultDictionary.Add(descriptionAttribute.Description, name.ToEnum<T>());
                            }
                        }
                    }
                }
            }

            return resultDictionary;
        }

        /// <summary>
        /// This extension method is used to convert a zero value to null.
        /// </summary>
        /// <param name="value">Contains the value to evaluate.</param>
        /// <returns>Returns null if the value is zero.</returns>
        public static long? ZeroToNull(this long? value)
        {
            return value.HasValue ? (value.Value != 0 ? value.Value : (long?)null) : null;
        }

        /// <summary>
        /// This extension method is used to convert a zero value to null.
        /// </summary>
        /// <param name="value">Contains the value to evaluate.</param>
        /// <returns>Returns null if the value is zero.</returns>
        public static int? ZeroToNull(this int? value)
        {
            return value.HasValue ? (value.Value != 0 ? value.Value : (int?)null) : null;
        }

        /// <summary>
        /// This extension method is used to convert a zero value to null.
        /// </summary>
        /// <param name="value">Contains the value to evaluate.</param>
        /// <returns>Returns null if the value is zero.</returns>
        public static long? ZeroToNull(this long value)
        {
            return value != 0 ? value : (long?)null;
        }

        /// <summary>
        /// This extension method is used to convert a zero value to null.
        /// </summary>
        /// <param name="value">Contains the value to evaluate.</param>
        /// <returns>Returns null if the value is zero.</returns>
        public static int? ZeroToNull(this int value)
        {
            return value != 0 ? value : (int?)null;
        }

        /// <summary>
        /// This extension method is used to convert an empty GUID value to null.
        /// </summary>
        /// <param name="value">Contains the value to evaluate.</param>
        /// <returns>Returns null if the Guid is empty, otherwise returns the guid.</returns>
        public static Guid? EmptyToNull(this Guid value)
        {
            return value != Guid.Empty ? value : (Guid?)null;
        }
    }
}