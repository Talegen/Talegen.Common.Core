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

namespace Talegen.Common.Core.Errors.Models
{
    /// <summary>
    /// This class implements a minimum view model for errors.
    /// </summary>
    /// <typeparam name="TErrorModel">The type of the error model.</typeparam>
    public class ErrorViewModel<TErrorModel> where TErrorModel : class, IErrorMessage
    {
        /// <summary>
        /// Gets or sets the request identifier.
        /// </summary>
        /// <value>The request identifier.</value>
        public string RequestId { get; set; }

        /// <summary>
        /// Gets a value indicating whether [show request identifier].
        /// </summary>
        /// <value><c>true</c> if [show request identifier]; otherwise, <c>false</c>.</value>
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ErrorViewModel{TErrorModel}" /> hosted application is development.
        /// </summary>
        /// <value><c>true</c> if development; otherwise, <c>false</c>.</value>
        public bool Development { get; set; }

        /// <summary>
        /// Gets or sets the error.
        /// </summary>
        /// <value>The error.</value>
        public TErrorModel Error { get; set; }
    }
}