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

    /// <summary>
    /// This class contains Extension methods for exceptions.
    /// </summary>
    public static class ExceptionExtensions
    {
        /// <summary>
        /// This method is used to recurse through an exception's inner exceptions and return a combined message string containing all error messages.
        /// </summary>
        /// <param name="ex">Contains the exception object to recurse.</param>
        /// <param name="recursionLevel">Contains the indentation level of the recursive messages.</param>
        /// <returns>Returns a string containing all related exception messages.</returns>
        public static string RecurseMessages(this Exception ex, int recursionLevel = 0)
        {
            string message = ex?.Message + Environment.NewLine;

            if (recursionLevel > 0)
            {
                message = new string('-', recursionLevel) + ">" + message;
            }

            if (ex?.InnerException != null)
            {
                message += ex.InnerException.RecurseMessages(++recursionLevel);
            }

            return message;
        }
    }
}