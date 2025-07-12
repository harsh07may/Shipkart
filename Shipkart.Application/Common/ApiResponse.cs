using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shipkart.Application.Common
{
    /// <summary>
    /// Represents a standardized API response wrapper containing success status, data, message, and errors.
    /// </summary>
    /// <typeparam name="T">The type of the data returned in the response.</typeparam>
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public T? Data { get; set; }
        public string? Message { get; set; }
        public List<string>? Errors { get; set; }

        public static ApiResponse<T> SuccessResponse(T data, string? message = null)
        {
            return new() { Success = true, Data = data, Message = message };
        }

        public static ApiResponse<T> Failure(List<string> errors, string? message = null)
        {
            return new() { Success = false, Errors = errors, Message = message };
        }

        public static ApiResponse<T> Failure(string error, string? message = null)
        {
            return Failure([error], message);
        }
    }
}
