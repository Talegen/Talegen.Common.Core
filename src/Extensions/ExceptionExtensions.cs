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