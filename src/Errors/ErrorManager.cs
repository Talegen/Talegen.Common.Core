namespace Talegen.Common.Core.Errors
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Linq;
    using System.Net;
    using System.Resources;
    using Microsoft.Extensions.Logging;
    using Talegen.Common.Core.Extensions;

    /// <summary>
    /// This error logging management class will track errors but also log to the data repository event log.
    /// </summary>
    public class ErrorManager : IErrorManager
    {
        #region Private Fields

        /// <summary>
        /// Contains an instance of a logger implementation.
        /// </summary>
        private readonly ILogger logger;

        /// <summary>
        /// Contains a list of <see cref="IErrorMessage" /> objects.
        /// </summary>
        private readonly List<IErrorMessage> messages;

        /// <summary>
        /// The resource manager
        /// </summary>
        private readonly ResourceManager resourceManager;

        /// <summary>
        /// The culture
        /// </summary>
        private readonly CultureInfo culture;

        #endregion

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="IdentityErrorManager" /> class.
        /// </summary>
        /// <param name="logger">Contains an instance of the <see cref="ILoggerService" /> interface object.</param>
        /// <param name="enableLogging">Contains a value indicating whether event logging is enabled by default.</param>
        /// <param name="minimumEventLogLevel">Contains the minimum <see cref="EventLogLevels" /> that will be sent to the event log database.</param>
        public ErrorManager(ILogger logger = null, bool enableLogging = false, LogLevel minimumEventLogLevel = LogLevel.Warning, ResourceManager resourceManager = null, CultureInfo cultureInfo = null)
        {
            this.messages = new List<IErrorMessage>();
            this.logger = logger;
            this.LogMessages = enableLogging;
            this.EventLoggingLevel = minimumEventLogLevel;
            this.resourceManager = resourceManager ?? Properties.Resources.ResourceManager;
            this.culture = cultureInfo ?? CultureInfo.CurrentCulture;
        }

        #endregion

        #region Public Interface Properties

        /// <summary>
        /// Gets a value indicating whether there are any critical error messages.
        /// </summary>
        public bool HasCriticalErrors => this.messages.Any(e => e.ErrorType == ErrorType.Critical);

        /// <summary>
        /// Gets a value indicating whether there are any error messages.
        /// </summary>
        public bool HasErrors => this.messages.Any();

        /// <summary>
        /// Gets a value indicating whether there are any validation error messages.
        /// </summary>
        public bool HasValidationErrors => this.messages.Any(v => v.ErrorType == ErrorType.Validation);

        /// <summary>
        /// Gets a value indicating whether there are any forbidden error messages reported.
        /// </summary>
        public bool HasForbiddenErrors => this.messages.Any(v => v.ErrorType == ErrorType.Critical && v.SuggestedErrorCode == 403);

        /// <summary>
        /// Gets or sets a value indicating whether any error messages are logged in real-time.
        /// </summary>
        public bool LogMessages { get; set; }

        /// <summary>
        /// Gets or sets the minimum event level where event logging will occur.
        /// </summary>
        public LogLevel EventLoggingLevel { get; set; }

        /// <summary>
        /// Gets a list of error message objects.
        /// </summary>
        public List<IErrorMessage> Messages => this.messages;

        #endregion

        #region Public Interface Methods

        /// <summary>
        /// This method is used to generate a new error message object.
        /// </summary>
        /// <returns>Returns an error message that implements IErrorMessage.</returns>
        public IErrorMessage CreateErrorMessage()
        {
            return this.CreateErrorMessage(string.Empty, ErrorType.Warning);
        }

        /// <summary>
        /// This method is used to create a new empty <see cref="IErrorMessage" /> object.
        /// </summary>
        /// <param name="message">Contains the error message text.</param>
        /// <param name="type">Contains the error message type.</param>
        /// <param name="category">Contains the error message category.</param>
        /// <returns>Returns a new instance of the <see cref="IErrorMessage" /> object.</returns>
        public IErrorMessage CreateErrorMessage(string message, ErrorType type, ErrorCategory category = ErrorCategory.General)
        {
            return new ErrorMessage(message, type, category);
        }

        /// <summary>
        /// This method is used to create a new empty <see cref="IErrorMessage" /> object.
        /// </summary>
        /// <param name="message">Contains the error message text.</param>
        /// <param name="type">Contains the error message type.</param>
        /// <param name="stackTrace">Contains the stack trace text.</param>
        /// <param name="category">Contains the error message category.</param>
        /// <returns>Returns a new instance of the <see cref="IErrorMessage" /> object.</returns>
        public IErrorMessage CreateErrorMessage(string message, ErrorType type, string stackTrace, ErrorCategory category = ErrorCategory.General)
        {
            return new ErrorMessage(message, type, stackTrace, category);
        }

        /// <summary>
        /// This method adds a validation error message.
        /// </summary>
        /// <param name="propertyName">Contains the property name error occurred.</param>
        /// <param name="message">Contains the error message text.</param>
        /// <param name="category">Contains the error message category.</param>
        /// <returns>Returns a new error message.</returns>
        public IErrorMessage CreateValidationMessage(string propertyName, string message, ErrorCategory category = ErrorCategory.General)
        {
            return new ErrorMessage(propertyName, message, category);
        }

        /// <summary>
        /// This method is used to generate a critical error with a suggested error code of 404 Not Found.
        /// </summary>
        /// <param name="message">Contains the resource key that will be used to look up the resource message value.</param>
        /// <param name="category">Contains the error message category.</param>
        public void CriticalNotFound(string message, ErrorCategory category = ErrorCategory.General)
        {
            var error = this.CreateErrorMessage(this.resourceManager.GetString(message, this.culture) ?? message, ErrorType.Critical, category);
            error.SuggestedErrorCode = (int)HttpStatusCode.NotFound;
            this.messages.Add(error);
        }

        /// <summary>
        /// This method is used to generate and add a critical error with a suggested error code of 404 Not Found to the messages list with a formatted message.
        /// </summary>
        /// <param name="message">Contains the resource key that will be used to look up the resource message value.</param>
        /// <param name="category">Contains the error message category.</param>
        /// <param name="parameters">Contains one or more parameter values to insert into the formatted message.</param>
        public void CriticalNotFoundFormat(string message, ErrorCategory category, params object[] parameters)
        {
            var error = this.CreateErrorMessage(string.Format(CultureInfo.InvariantCulture, this.resourceManager.GetString(message, this.culture) ?? message, parameters), ErrorType.Critical, category);
            error.SuggestedErrorCode = (int)HttpStatusCode.NotFound;
            this.messages.Add(error);
        }

        /// <summary>
        /// This method is used to generate a critical error with a suggested error code of 403 Forbidden.
        /// </summary>
        /// <param name="message">Contains the resource key that will be used to look up the resource message value.</param>
        /// <param name="category">Contains the error message category.</param>
        public void CriticalForbidden(string message, ErrorCategory category = ErrorCategory.General)
        {
            var error = this.CreateErrorMessage(this.resourceManager.GetString(message, this.culture) ?? message, ErrorType.Critical, category);
            error.SuggestedErrorCode = (int)HttpStatusCode.Forbidden;
            this.messages.Add(error);
        }

        /// <summary>
        /// This method is used to generate and add a critical error with a suggested error code of 403 Forbidden to the messages list with a formatted message.
        /// </summary>
        /// <param name="message">Contains the resource key that will be used to look up the resource message value.</param>
        /// <param name="category">Contains the error message category.</param>
        /// <param name="parameters">Contains one or more parameter values to insert into the formatted message.</param>
        public void CriticalForbiddenFormat(string message, ErrorCategory category, params object[] parameters)
        {
            var error = this.CreateErrorMessage(string.Format(CultureInfo.InvariantCulture, this.resourceManager.GetString(message, this.culture) ?? message, parameters), ErrorType.Critical, category);
            error.SuggestedErrorCode = (int)HttpStatusCode.Forbidden;
            this.messages.Add(error);
        }

        // None, Critical, Error, Warning, Information, Debug, Trace

        /// <summary>
        /// This method is used to generate and add a fatal error message to the messages list.
        /// </summary>
        /// <param name="ex">Contains the exception to add as a fatal error message.</param>
        /// <param name="category">Contains the error message category.</param>
        public void Fatal(Exception ex, ErrorCategory category = ErrorCategory.General)
        {
            if (this.logger != null && this.LogMessages && this.EventLoggingLevel <= LogLevel.Critical)
            {
                this.logger.LogCritical(ex, ex.RecurseMessages(), category);
            }

            this.messages.Add(this.CreateErrorMessage(ex.RecurseMessages(), ErrorType.Fatal, ex.StackTrace));
        }

        /// <summary>
        /// This method is used to generate and add a fatal error message to the messages list.
        /// </summary>
        /// <param name="message">Contains the resource key that will be used to look up the resource message value.</param>
        /// <param name="category">Contains the error message category.</param>
        /// <param name="ex">Contains the exception to add as a fatal error message.</param>
        public void Fatal(string message, ErrorCategory category = ErrorCategory.General, Exception ex = null)
        {
            if (this.logger != null && this.LogMessages && this.EventLoggingLevel <= LogLevel.Critical)
            {
                this.logger.LogError(ex, this.resourceManager.GetString(message, this.culture) ?? message, category, ex != null);
            }

            this.messages.Add(this.CreateErrorMessage(this.resourceManager.GetString(message, this.culture) ?? message + Environment.NewLine + ex.RecurseMessages(), ErrorType.Fatal, ex != null ? ex.StackTrace : string.Empty));
        }

        /// <summary>
        /// This method is used to generate and add a critical error message to the messages list.
        /// </summary>
        /// <param name="ex">Contains the exception to add as a critical error message.</param>
        /// <param name="category">Contains the error message category.</param>
        public void Critical(Exception ex, ErrorCategory category = ErrorCategory.General)
        {
            if (this.logger != null && this.LogMessages && this.EventLoggingLevel <= LogLevel.Critical)
            {
                this.logger.LogCritical(ex, ex.RecurseMessages(), category);
            }

            this.messages.Add(this.CreateErrorMessage(ex.RecurseMessages(), ErrorType.Critical, ex.StackTrace));
        }

        /// <summary>
        /// This method is used to generate and add a critical error message to the messages list.
        /// </summary>
        /// <param name="message">Contains the resource key that will be used to look up the resource message value.</param>
        /// <param name="category">Contains the error message category.</param>
        /// <param name="ex">Contains the exception to add as a critical error message.</param>
        public void Critical(string message, ErrorCategory category = ErrorCategory.General, Exception ex = null)
        {
            if (this.logger != null && this.LogMessages && this.EventLoggingLevel <= LogLevel.Critical)
            {
                this.logger.LogCritical(ex, this.resourceManager.GetString(message, this.culture) ?? message, category, ex != null);
            }

            this.messages.Add(this.CreateErrorMessage(this.resourceManager.GetString(message, this.culture) ?? message + Environment.NewLine + ex.RecurseMessages(), ErrorType.Critical, ex != null ? ex.StackTrace : string.Empty));
        }

        /// <summary>
        /// This method is used to generate and add a critical error message to the messages list with a formatted message.
        /// </summary>
        /// <param name="message">Contains the resource key that will be used to look up the resource message value.</param>
        /// <param name="category">Contains the error message category.</param>
        /// <param name="parameters">Contains one or more parameter values to insert into the formatted message.</param>
        public void CriticalFormat(string message, ErrorCategory category, params object[] parameters)
        {
            if (this.logger != null && this.LogMessages && this.EventLoggingLevel <= LogLevel.Critical)
            {
                this.logger.LogCritical(string.Format(CultureInfo.InvariantCulture, this.resourceManager.GetString(message, this.culture) ?? message, parameters), category);
            }

            this.messages.Add(this.CreateErrorMessage(string.Format(CultureInfo.InvariantCulture, this.resourceManager.GetString(message, this.culture) ?? message, parameters), ErrorType.Critical));
        }

        /// <summary>
        /// This method is used to generate and add a critical error message to the messages list with a formatted message.
        /// </summary>
        /// <param name="message">Contains the resource key that will be used to look up the resource message value.</param>
        /// <param name="ex">Contains the exception to add as a critical error message.</param>
        /// <param name="category">Contains the error message category.</param>
        /// <param name="parameters">Contains one or more parameter values to insert into the formatted message.</param>
        public void CriticalFormat(string message, Exception ex, ErrorCategory category, params object[] parameters)
        {
            if (this.logger != null && this.LogMessages && this.EventLoggingLevel <= LogLevel.Critical)
            {
                this.logger.LogCritical(ex, string.Format(CultureInfo.InvariantCulture, this.resourceManager.GetString(message, this.culture) ?? message, parameters), category, true);
            }

            this.messages.Add(this.CreateErrorMessage(string.Format(CultureInfo.InvariantCulture, this.resourceManager.GetString(message, this.culture) ?? message, parameters) + Environment.NewLine + ex.RecurseMessages(), ErrorType.Critical, ex != null ? ex.StackTrace : string.Empty));
        }

        /// <summary>
        /// This method is used to generate and add a validation error message to the messages list.
        /// </summary>
        /// <param name="propertyName">Contains the property name error occurred.</param>
        /// <param name="message">Contains the resource key that will be used to look up the resource message value.</param>
        /// <param name="category">Contains the error message category.</param>
        public void Validation(string propertyName, string message, ErrorCategory category = ErrorCategory.General)
        {
            this.messages.Add(this.CreateValidationMessage(propertyName, this.resourceManager.GetString(message, this.culture) ?? message));
        }

        /// <summary>
        /// This method is used to generate and add a validation error message to the messages list with a formatted message.
        /// </summary>
        /// <param name="propertyName">Contains the property name error occurred.</param>
        /// <param name="format">Contains the resource key that will be used to look up the resource message value.</param>
        /// <param name="category">Contains the error message category.</param>
        /// <param name="parameters">Contains one or more parameter values to insert into the formatted message.</param>
        public void ValidationFormat(string propertyName, string format, ErrorCategory category, params object[] parameters)
        {
            this.messages.Add(this.CreateValidationMessage(propertyName, string.Format(CultureInfo.InvariantCulture, this.resourceManager.GetString(format, this.culture) ?? format, parameters)));
        }

        /// <summary>
        /// This method is used to generate and add a warning error message to the messages list.
        /// </summary>
        /// <param name="message">Contains the resource key that will be used to look up the resource message value.</param>
        /// <param name="category">Contains the error message category.</param>
        /// <param name="ex">Contains the exception to add as a critical error message.</param>
        public void Warning(string message, ErrorCategory category = ErrorCategory.General, Exception ex = null)
        {
            if (this.logger != null && this.LogMessages && this.EventLoggingLevel <= LogLevel.Warning)
            {
                this.logger.LogWarning(ex, this.resourceManager.GetString(message, this.culture) ?? message, category, ex != null);
            }

            this.messages.Add(this.CreateErrorMessage(this.resourceManager.GetString(message, this.culture) ?? message + Environment.NewLine + ex.RecurseMessages(), ErrorType.Warning, ex != null ? ex.StackTrace : string.Empty));
        }

        /// <summary>
        /// This method is used to generate and add a warning error message to the messages list with a formatted message.
        /// </summary>
        /// <param name="message">Contains the resource key that will be used to look up the resource message value.</param>
        /// <param name="category">Contains the error message category.</param>
        /// <param name="parameters">Contains one or more parameter values to insert into the formatted message.</param>
        public void WarningFormat(string message, ErrorCategory category, params object[] parameters)
        {
            if (this.logger != null && this.LogMessages && this.EventLoggingLevel <= LogLevel.Warning)
            {
                this.logger.LogWarning(string.Format(CultureInfo.InvariantCulture, this.resourceManager.GetString(message, this.culture) ?? message, parameters), category);
            }

            this.messages.Add(this.CreateErrorMessage(string.Format(CultureInfo.InvariantCulture, this.resourceManager.GetString(message, this.culture) ?? message, parameters), ErrorType.Warning));
        }

        /// <summary>
        /// This method is used to generate and add a warning error message to the messages list with a formatted message.
        /// </summary>
        /// <param name="message">Contains the resource key that will be used to look up the resource message value.</param>
        /// <param name="ex">Contains the exception to add as a critical error message.</param>
        /// <param name="category">Contains the error message category.</param>
        /// <param name="parameters">Contains one or more parameter values to insert into the formatted message.</param>
        public void WarningFormat(string message, Exception ex, ErrorCategory category, params object[] parameters)
        {
            if (this.logger != null && this.LogMessages && this.EventLoggingLevel <= LogLevel.Warning)
            {
                this.logger.LogWarning(ex, string.Format(CultureInfo.InvariantCulture, this.resourceManager.GetString(message, this.culture) ?? message, parameters), category, true);
            }

            this.messages.Add(this.CreateErrorMessage(string.Format(CultureInfo.InvariantCulture, this.resourceManager.GetString(message, this.culture) ?? message, parameters) + Environment.NewLine + ex.RecurseMessages(), ErrorType.Warning, ex != null ? ex.StackTrace : string.Empty));
        }

        #endregion
    }
}