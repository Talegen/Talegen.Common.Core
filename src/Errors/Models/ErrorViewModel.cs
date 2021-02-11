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