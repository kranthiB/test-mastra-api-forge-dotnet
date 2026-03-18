using Microsoft.AspNetCore.Mvc;

namespace ApiForge.Api.Middleware;

/// <summary>
/// Global exception handler that translates unhandled exceptions into
/// RFC 7807 Problem Details responses. Sits at the very top of the pipeline
/// so nothing leaks a raw 500 stack trace to the client.
/// </summary>
public sealed class ExceptionHandlingMiddleware(
    RequestDelegate next,
    ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Unhandled exception on {Method} {Path}",
                context.Request.Method,
                context.Request.Path);

            await HandleAsync(context, ex);
        }
    }

    private static Task HandleAsync(HttpContext context, Exception exception)
    {
        var (status, title) = exception switch
        {
            ArgumentException or InvalidOperationException
                => (StatusCodes.Status400BadRequest, "Bad Request"),
            KeyNotFoundException
                => (StatusCodes.Status404NotFound, "Not Found"),
            UnauthorizedAccessException
                => (StatusCodes.Status401Unauthorized, "Unauthorized"),
            OperationCanceledException
                => (StatusCodes.Status499ClientClosedRequest, "Client Closed Request"),
            _
                => (StatusCodes.Status500InternalServerError, "An unexpected error occurred."),
        };

        var problem = new ProblemDetails
        {
            Status   = status,
            Title    = title,
            Detail   = exception.Message,
            Instance = context.Request.Path,
        };

        context.Response.StatusCode  = status;
        context.Response.ContentType = "application/problem+json";
        return context.Response.WriteAsJsonAsync(problem);
    }
}
