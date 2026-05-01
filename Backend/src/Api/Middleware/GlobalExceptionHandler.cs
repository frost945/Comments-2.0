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