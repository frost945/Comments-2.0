using System.Diagnostics;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Comments.Api.Middleware
{
    public sealed class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;

        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
        {
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {
            var traceId = Activity.Current?.Id ?? httpContext.TraceIdentifier;

            var problem = exception switch
            {
                ArgumentException ex => LogAndCreateProblem(
                    ex,
                    StatusCodes.Status400BadRequest,
                    "Invalid request data",
                    traceId,
                    LogLevel.Warning),

                UnauthorizedAccessException ex => LogAndCreateProblem(
                    ex,
                    StatusCodes.Status401Unauthorized,
                    "Unauthorized access",
                    traceId,
                    LogLevel.Warning),

                KeyNotFoundException ex => LogAndCreateProblem(
                    ex,
                    StatusCodes.Status404NotFound,
                    "Resource not found",
                    traceId,
                    LogLevel.Information),

                _ => LogAndCreateProblem(
                    exception,
                    StatusCodes.Status500InternalServerError,
                    "An unexpected error occurred",
                    traceId,
                    LogLevel.Error)
            };

            httpContext.Response.StatusCode = problem.Status!.Value;
            httpContext.Response.ContentType = "application/problem+json";

            await httpContext.Response.WriteAsJsonAsync(problem, cancellationToken);

            return true;
        }

        private ProblemDetails LogAndCreateProblem(
            Exception exception,
            int statusCode,
            string title,
            string traceId,
            LogLevel logLevel)
        {
            _logger.Log(
                logLevel,
                exception,
                "Handled exception. StatusCode: {StatusCode}, TraceId: {TraceId}",
                statusCode,
                traceId);

            return new ProblemDetails
            {
                Status = statusCode,
                Title = title,
                Instance = traceId,
                Extensions =
                {
                    ["traceId"] = traceId
                }
            };
        }
    }
}


/*using Microsoft.AspNetCore.Diagnostics;

namespace Comments.Api.Middleware
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;
        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
        {
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            _logger.LogError(exception, "An unhandled exception occurred during request processing");

            httpContext.Response.ContentType = "application/json";

            object responseObj;

            switch (exception)
            {
                case ArgumentException:
                    httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                    responseObj = new { error = "Invalid request data" };
                    break;

                case UnauthorizedAccessException:
                    httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    responseObj = new { error = "Unauthorized access" };
                    break;
                
                case KeyNotFoundException:
                    httpContext.Response.StatusCode = StatusCodes.Status404NotFound;
                    responseObj = new { error = "Resource not found" };
                    break;

                default:
                    httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    responseObj = new { error = "An unexpected error occurred" };
                    break;
            }

            var json = System.Text.Json.JsonSerializer.Serialize(responseObj);
            await httpContext.Response.WriteAsync(json, cancellationToken);

            return true;
        }
    }
}*/
