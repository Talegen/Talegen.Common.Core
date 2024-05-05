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
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;

    /// <summary>
    /// This class contains extension methods for working with URI.
    /// </summary>
    public static class UriBuilderExtensions
    {
        /// <summary>
        /// This method appends the object as a query string to the target URI object.
        /// </summary>
        /// <typeparam name="T">Contains the type of the source object.</typeparam>
        /// <param name="target">Contains the target URI to append to.</param>
        /// <param name="source">Contains the source object.</param>
        /// <returns>Returns the target URI with the appended query string.</returns>
        public static Uri AppendAsQueryString<T>(this Uri target, T source)
        {
            UriBuilder builder = new UriBuilder(target);
            
            if (string.IsNullOrEmpty(builder.Query))
            {
                builder.Query = source.AsQueryString();
            }
            else
            {
                builder.Query = builder.Query.TrimStart('?') + "&" + source.AsQueryString();
            }
            
            return builder.Uri;
        }

        /// <summary>
        /// This method is used to convert an object to a query string formatted string.
        /// </summary>
        /// <typeparam name="T">Contains the type of the source object.</typeparam>
        /// <param name="sourceObject">Contains the source object to convert.</param>
        /// <returns>Returns the converted query string.</returns>
        public static string AsQueryString<T>(this T sourceObject)
        {
            var result = new List<string>();

            var props = sourceObject.GetType().GetProperties().Where(p => p.GetValue(sourceObject, null) != null);

            foreach (var p in props)
            {
                var value = p.GetValue(sourceObject, null);
                var enumerable = value as ICollection;

                if (enumerable != null)
                {
                    result.AddRange(from object v in enumerable select string.Format("{0}={1}", p.Name, HttpUtility.UrlEncode(v.ToString())));
                }
                else
                {
                    result.Add(string.Format("{0}={1}", p.Name, HttpUtility.UrlEncode(value.ToString())));
                }
            }

            return string.Join("&", result.ToArray());
        }
    }
}
