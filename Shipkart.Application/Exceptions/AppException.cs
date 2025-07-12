using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shipkart.Application.Exceptions
{
    /// <summary>
    /// Represents application-specific exceptions with an associated HTTP status code.
    /// </summary>
    /// <remarks>
    /// Use <see cref="AppException"/> to throw errors that should be returned to the client with a specific status code.
    /// </remarks>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="statusCode">The HTTP status code to associate with the exception. Defaults to 400.</param>
    public class AppException : Exception
    {
        public int StatusCode { get; }

        public AppException(string message, int statusCode = 400) : base(message)
        {
            StatusCode = statusCode;
        }
    }
}
