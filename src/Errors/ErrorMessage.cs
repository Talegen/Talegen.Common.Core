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

namespace Talegen.Common.Core.Errors
{
    using System;

    /// <summary>
    /// This class represents an error message.
    /// </summary>
    public class ErrorMessage : IErrorMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorMessage" /> class.
        /// </summary>
        public ErrorMessage()
            : this(string.Empty, ErrorType.Warning, string.Empty)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorMessage" /> class.
        /// </summary>
        /// <param name="message">Contains the error message text.</param>
        /// <param name="category">Contains the error message category.</param>
        public ErrorMessage(string message, ErrorCategory category = ErrorCategory.General)
            : this(message, ErrorType.Warning, string.Empty, category)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorMessage" /> class.
        /// </summary>
        /// <param name="message">Contains the error message text.</param>
        /// <param name="type">Contains the error message type.</param>
        /// <param name="category">Contains the error message category.</param>
        public ErrorMessage(string message, ErrorType type, ErrorCategory category = ErrorCategory.General)
            : this(message, type, string.Empty, category)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorMessage" /> class.
        /// </summary>
        /// <param name="message">Contains the error message text.</param>
        /// <param name="type">Contains the error message type.</param>
        /// <param name="stackTrace">Contains the error message stack trace text.</param>
        /// <param name="category">Contains the error message category.</param>
        public ErrorMessage(string message, ErrorType type, string stackTrace, ErrorCategory category = ErrorCategory.General)
        {
            this.Message = message;
            this.ErrorType = type;
            this.StackTrace = stackTrace;
            this.EventDate = DateTime.UtcNow;
            this.ErrorCategory = category;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorMessage" /> class.
        /// </summary>
        /// <param name="propertyName">Contains the related property name.</param>
        /// <param name="message">Contains the error message text.</param>
        /// <param name="category">Contains the error message category.</param>
        public ErrorMessage(string propertyName, string message, ErrorCategory category = ErrorCategory.General)
        {
            this.PropertyName = propertyName;
            this.ErrorType = ErrorType.Validation;
            this.Message = message;
            this.EventDate = DateTime.UtcNow;
            this.ErrorCategory = category;
        }

        /// <summary>
        /// Gets or sets the error message type.
        /// </summary>
        public ErrorType ErrorType { get; set; }

        /// <summary>
        /// Gets or sets the error message category.
        /// </summary>
        public ErrorCategory ErrorCategory { get; set; }

        /// <summary>
        /// Gets or sets the error message text.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the error message stack trace text.
        /// </summary>
        public string StackTrace { get; set; }

        /// <summary>
        /// Gets or sets a related property name.
        /// </summary>
        public string PropertyName { get; set; }

        /// <summary>
        /// Gets or sets a suggested return error code to the client.
        /// </summary>
        public int SuggestedErrorCode { get; set; }

        /// <summary>
        /// Gets or sets the date time when the error occurred.
        /// </summary>
        public DateTime EventDate { get; set; }
    }
}