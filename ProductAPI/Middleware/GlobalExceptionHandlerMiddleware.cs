using System.Net;
using System.Text.Json;
using ProductAPI.Application.Common.Errors;
using ProductAPI.Domain.Exceptions;

namespace ProductAPI.Middleware
{
    /// <summary>
    /// Middleware for handling exceptions globally and returning a standardized error response.
    /// </summary>
    public class GlobalExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="GlobalExceptionHandlerMiddleware"/> class.
        /// </summary>
        /// <param name="next">The next middleware in the request pipeline.</param>
        /// <param name="logger">The logger for logging exceptions.</param>
        public GlobalExceptionHandlerMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlerMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        /// <summary>
        /// Processes the HTTP request and catches any unhandled exceptions.
        /// </summary>
        /// <param name="context">The current HTTP context.</param>
        /// <returns>A task that represents the completion of request processing.</returns>
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            HttpStatusCode statusCode = HttpStatusCode.InternalServerError;
            ApiError errorResponse; // Use your ApiError class

            switch (exception)
            {
                case NotFoundException notFoundException:
                    statusCode = HttpStatusCode.NotFound;
                    // Create your ApiError for a 404
                    errorResponse = new ApiError
                    {
                        Code = statusCode.ToString(),
                        Message = notFoundException.Message
                    };
                    _logger.LogWarning(exception, "Resource not found: {Message}", exception.Message);
                    break;

                case ValidationException validationException:
                    statusCode = HttpStatusCode.BadRequest;
                    // Create your ApiError for a 400
                    errorResponse = new ApiError
                    {
                        Code = statusCode.ToString(),
                        Message = validationException.Message
                    };
                    _logger.LogWarning(exception, "Validation error: {Message}", exception.Message);
                    break;

                default:
                    // Create your ApiError for a 500
                    errorResponse = new ApiError
                    {
                        Code = statusCode.ToString(),
                        Message = "An unexpected internal server error has occurred."
                        // In development, you might want to add exception.ToString() to the Details property
                        // Details = exception.ToString()
                    };
                    _logger.LogError(exception, "An unhandled exception has occurred: {Message}", exception.Message);
                    break;
            }

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            var jsonResponse = JsonSerializer.Serialize(errorResponse);

            await context.Response.WriteAsync(jsonResponse);
        }
    }
}