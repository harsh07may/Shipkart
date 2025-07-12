using Shipkart.Application.Common;
using Shipkart.Application.Exceptions;

namespace Shipkart.Api.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (AppException ex)
            {
                context.Response.StatusCode = ex.StatusCode;
                context.Response.ContentType = "application/json";

                var response = ApiResponse<string>.Failure(ex.Message);
                await context.Response.WriteAsJsonAsync(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception");
                context.Response.StatusCode = 500;
                context.Response.ContentType = "application/json";

                var response = ApiResponse<string>.Failure("An unexpected error occurred.");
                await context.Response.WriteAsJsonAsync(response);
            }
        }
    }
}
