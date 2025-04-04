﻿/*
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
    using System.Collections.Generic;
    using System.Net;



    /// <summary>
    /// Contains an enumerated list of event types.
    /// </summary>

    public enum EventType
    {
        /// <summary>
        /// An event for debugging purposes only.
        /// </summary>
        Debug,

        /// <summary>
        /// A validation event.
        /// </summary>
        Validation,

        /// <summary>
        /// A warning event.
        /// </summary>
        Warning,

        /// <summary>
        /// An error event that is recoverable.
        /// </summary>
        Error,

        /// <summary>
        /// A critical event that is not recoverable.
        /// </summary>
        Critical
    }

    /// <summary>
    /// This interface represents an error message handling object.
    /// </summary>
    public interface IErrorManager
    {
        /// <summary>
        /// Gets a list of error message objects.
        /// </summary>
        List<IErrorMessage> Messages { get; }

        /// <summary>
        /// Gets a value indicating whether there are any error messages.
        /// </summary>
        bool HasErrors { get; }

        /// <summary>
        /// Gets a value indicating whether there are any critical error messages.
        /// </summary>
        bool HasCriticalErrors { get; }

        /// <summary>
        /// Gets a value indicating whether there are any validation error messages.
        /// </summary>
        bool HasValidationErrors { get; }

        /// <summary>
        /// Gets a value indicating whether there are any forbidden error messages reported.
        /// </summary>
        bool HasForbiddenErrors { get; }

        /// <summary>
        /// Gets or sets a value indicating whether any error messages are logged in real-time.
        /// </summary>
        bool LogMessages { get; set; }

        /// <summary>
        /// This method is used to generate a new error message object.
        /// </summary>
        /// <returns>Returns an error message that implements IErrorMessage.</returns>
        IErrorMessage CreateErrorMessage();

        /// <summary>
        /// This method is used to generate a new error message object.
        /// </summary>
        /// <param name="message">Contains the error message text.</param>
        /// <param name="type">Contains the error message type.</param>
        /// <param name="category">Contains the error message category.</param>
        /// <returns>Returns an error message that implements IErrorMessage.</returns>
        IErrorMessage CreateErrorMessage(string message, ErrorType type, ErrorCategory category = ErrorCategory.General);

        /// <summary>
        /// This method is used to create a new empty <see cref="IErrorMessage" /> object.
        /// </summary>
        /// <param name="message">Contains the error message text.</param>
        /// <param name="type">Contains the error message type.</param>
        /// <param name="stackTrace">Contains the stack trace text.</param>
        /// <param name="category">Contains the error message category.</param>
        /// <returns>Returns a new instance of the <see cref="IErrorMessage" /> object.</returns>
        IErrorMessage CreateErrorMessage(string message, ErrorType type, string stackTrace, ErrorCategory category = ErrorCategory.General);

        /// <summary>
        /// This method is used to create a property validation error message.
        /// </summary>
        /// <param name="propertyName">Contains the property name error occurred.</param>
        /// <param name="message">Contains the error message text.</param>
        /// <param name="category">Contains the error message category.</param>
        /// <returns>Returns a new instance of the <see cref="IErrorMessage" /> object.</returns>
        IErrorMessage CreateValidationMessage(string propertyName, string message, ErrorCategory category = ErrorCategory.General);

        /// <summary>
        /// This method is used to generate a critical error with a suggested error code of 404 Not Found.
        /// </summary>
        /// <param name="message">Contains an optional message to apply to the critical error.</param>
        /// <param name="category">Contains the error message category.</param>
        void CriticalNotFound(string message, ErrorCategory category = ErrorCategory.General);

        /// <summary>
        /// This method is used to generate and add a critical error with a suggested error code of 404 Not Found to the messages list with a formatted message.
        /// </summary>
        /// <param name="message">Contains the message and any format related token values for inserting parameter values.</param>
        /// <param name="category">Contains the error message category.</param>
        /// <param name="parameters">Contains one or more parameter values to insert into the formatted message.</param>
        void CriticalNotFoundFormat(string message, ErrorCategory category, params object[] parameters);

        /// <summary>
        /// This method is used to generate a critical error with a suggested error code of 403 Forbidden.
        /// </summary>
        /// <param name="message">Contains an optional message to apply to the critical error.</param>
        /// <param name="category">Contains the error message category.</param>
        void CriticalForbidden(string message, ErrorCategory category = ErrorCategory.General);

        /// <summary>
        /// This method is used to generate and add a critical error with a suggested error code of 403 Forbidden to the messages list with a formatted message.
        /// </summary>
        /// <param name="message">Contains the message and any format related token values for inserting parameter values.</param>
        /// <param name="category">Contains the error message category.</param>
        /// <param name="parameters">Contains one or more parameter values to insert into the formatted message.</param>
        void CriticalForbiddenFormat(string message, ErrorCategory category, params object[] parameters);


        /// <summary>
        /// This method is used to generate a critical error with a suggested error code of 500 Internal Server Error.
        /// </summary>
        /// <param name="message">Contains the resource key that will be used to look up the resource message value.</param>
        /// <param name="category">Contains the error message category.</param>
        /// <param name="statusCode">Contains the HTTP status code to suggest to the client.</param>
        /// <param name="parameters">Contains one or more parameter values to insert into the formatted message.</param>
        void WarningStatusCodeFormat(string message, ErrorCategory category, HttpStatusCode statusCode, params object[] parameters);
        
        /// <summary>
        /// This method is used to generate a warning error with a suggested error code of 500 Internal Server Error.
        /// </summary>
        /// <param name="message">Contains the resource key that will be used to look up the resource message value.</param>
        /// <param name="category">Contains the error message category.</param>
        /// <param name="statusCode">Contains the HTTP status code to suggest to the client.</param>
        void WarningStatusCode(string message, ErrorCategory category, HttpStatusCode statusCode);
        

        /// <summary>
        /// This method is used to generate a critical error with a suggested error code of 500 Internal Server Error.
        /// </summary>
        /// <param name="message">Contains the resource key that will be used to look up the resource message value.</param>
        /// <param name="category">Contains the error message category.</param>
        /// <param name="statusCode">Contains the HTTP status code to suggest to the client.</param>
        void CriticalStatusCode(string message, ErrorCategory category, HttpStatusCode statusCode);
        

        /// <summary>
        /// This method is used to generate and add a critical error with a suggested error code of 500 Internal Server Error to the messages list with a formatted message.
        /// </summary>
        /// <param name="message">Contains the resource key that will be used to look up the resource message value.</param>
        /// <param name="category">Contains the error message category.</param>
        /// <param name="statusCode">Contains the HTTP status code to suggest to the client.</param>
        /// <param name="parameters">Contains one or more parameter values to insert into the formatted message.</param>
        void CriticalStatusCodeFormat(string message, ErrorCategory category, HttpStatusCode statusCode, params object[] parameters);

        /// <summary>
        /// This method is used to generate and add a fatal error message to the messages list.
        /// </summary>
        /// <param name="ex">Contains the exception to add as a fatal error message.</param>
        /// <param name="category">Contains the error message category.</param>
        void Fatal(Exception ex, ErrorCategory category = ErrorCategory.General);

        /// <summary>
        /// This method is used to generate and add a fatal error message to the messages list.
        /// </summary>
        /// <param name="message">Contains the message to add.</param>
        /// <param name="category">Contains the error message category.</param>
        /// <param name="ex">Contains the exception to add as a fatal error message.</param>
        void Fatal(string message, ErrorCategory category = ErrorCategory.General, Exception ex = null);

        /// <summary>
        /// This method is used to generate and add a critical error message to the messages list.
        /// </summary>
        /// <param name="ex">Contains the exception to add as a critical error message.</param>
        /// <param name="category">Contains the error message category.</param>
        void Critical(Exception ex, ErrorCategory category = ErrorCategory.General);

        /// <summary>
        /// This method is used to generate and add a critical error message to the messages list.
        /// </summary>
        /// <param name="message">Contains the message to add.</param>
        /// <param name="category">Contains the error message category.</param>
        /// <param name="ex">Contains the exception to add as a critical error message.</param>
        void Critical(string message, ErrorCategory category = ErrorCategory.General, Exception ex = null);

        /// <summary>
        /// This method is used to generate and add a warning error message to the messages list.
        /// </summary>
        /// <param name="message">Contains the message to add.</param>
        /// <param name="category">Contains the error message category.</param>
        /// <param name="ex">Contains the exception to add as a critical error message.</param>
        void Warning(string message, ErrorCategory category = ErrorCategory.General, Exception ex = null);

        /// <summary>
        /// This method is used to generate and add a validation error message to the messages list.
        /// </summary>
        /// <param name="propertyName">Contains the property name error occurred.</param>
        /// <param name="message">Contains the message to add.</param>
        /// <param name="category">Contains the error message category.</param>
        void Validation(string propertyName, string message, ErrorCategory category = ErrorCategory.General);

        /// <summary>
        /// This method is used to generate and add a critical error message to the messages list with a formatted message.
        /// </summary>
        /// <param name="message">Contains the message and any format related token values for inserting parameter values.</param>
        /// <param name="category">Contains the error message category.</param>
        /// <param name="parameters">Contains one or more parameter values to insert into the formatted message.</param>
        void CriticalFormat(string message, ErrorCategory category, params object[] parameters);

        /// <summary>
        /// This method is used to generate and add a critical error message to the messages list with a formatted message.
        /// </summary>
        /// <param name="message">Contains the message and any format related token values for inserting parameter values.</param>
        /// <param name="ex">Contains the exception to add as a critical error message.</param>
        /// <param name="category">Contains the error message category.</param>
        /// <param name="parameters">Contains one or more parameter values to insert into the formatted message.</param>
        void CriticalFormat(string message, Exception ex, ErrorCategory category, params object[] parameters);

        /// <summary>
        /// This method is used to generate and add a warning error message to the messages list with a formatted message.
        /// </summary>
        /// <param name="message">Contains the message and any format related token values for inserting parameter values.</param>
        /// <param name="category">Contains the error message category.</param>
        /// <param name="parameters">Contains one or more parameter values to insert into the formatted message.</param>
        void WarningFormat(string message, ErrorCategory category, params object[] parameters);

        /// <summary>
        /// This method is used to generate and add a warning error message to the messages list with a formatted message.
        /// </summary>
        /// <param name="message">Contains the message and any format related token values for inserting parameter values.</param>
        /// <param name="ex">Contains the exception to add as a critical error message.</param>
        /// <param name="category">Contains the error message category.</param>
        /// <param name="parameters">Contains one or more parameter values to insert into the formatted message.</param>
        void WarningFormat(string message, Exception ex, ErrorCategory category, params object[] parameters);

        /// <summary>
        /// This method is used to generate and add a validation error message to the messages list with a formatted message.
        /// </summary>
        /// <param name="propertyName">Contains the property name error occurred.</param>
        /// <param name="format">Contains the message and any format related token values for inserting parameter values.</param>
        /// <param name="category">Contains the error message category.</param>
        /// <param name="parameters">Contains one or more parameter values to insert into the formatted message.</param>
        void ValidationFormat(string propertyName, string format, ErrorCategory category, params object[] parameters);
    }
}