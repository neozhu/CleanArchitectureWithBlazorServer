using System.Diagnostics;
using System.Security;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Diagnostics;

namespace CleanArchitecture.Blazor.Server.UI.Middlewares;

internal sealed class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception,
        CancellationToken cancellationToken)
    {
        var traceId = Activity.Current?.Id ?? httpContext.TraceIdentifier;

        // Handle antiforgery validation failures by redirecting to the login page.
        // The redirect ensures the antiforgery cookie is set via App.razor's GetAndStoreTokens(),
        // so the next login attempt will succeed.
        if (IsAntiforgeryException(exception))
        {
            logger.LogWarning(
                "Antiforgery token validation failed on machine {MachineName}. Redirecting to login. TraceId: {TraceId}",
                Environment.MachineName, traceId);

            httpContext.Response.Redirect("/account/login");
            return true;
        }

        logger.LogError(exception,
            "Could not process a request on machine {MachineName}. TraceId: {TraceId}",
            Environment.MachineName, traceId);

        var rootException = GetRootException(exception);
        var (statusCode, title) = MapExceptionToStatusCode(rootException);

        httpContext.Response.StatusCode = statusCode;

        await Results.Problem(
            title: title,
            statusCode: statusCode,
            extensions: new Dictionary<string, object?>
            {
                { "traceId", traceId }
            }).ExecuteAsync(httpContext).ConfigureAwait(false);

        return true;
    }

    private static bool IsAntiforgeryException(Exception exception)
    {
        // Check the exception chain for AntiforgeryValidationException
        var current = exception;
        while (current is not null)
        {
            if (current is AntiforgeryValidationException)
                return true;
            current = current.InnerException;
        }
        return false;
    }

    private static Exception GetRootException(Exception exception)
    {
        while (exception.InnerException is not null)
        {
            exception = exception.InnerException;
        }
        return exception;
    }

    private static (int statusCode, string title) MapExceptionToStatusCode(Exception exception)
    {
        return exception switch
        {
            ArgumentOutOfRangeException or ArgumentNullException or ArgumentException
                => (StatusCodes.Status400BadRequest, "Bad request."),
            KeyNotFoundException or FileNotFoundException
                => (StatusCodes.Status404NotFound, "Resource not found."),
            UnauthorizedAccessException or SecurityException
                => (StatusCodes.Status403Forbidden, "Access denied."),
            TimeoutException or TaskCanceledException
                => (StatusCodes.Status504GatewayTimeout, "The request timed out."),
            NotImplementedException
                => (StatusCodes.Status501NotImplemented, "This feature is not implemented."),
            _ => (StatusCodes.Status500InternalServerError, "An unexpected error occurred. Please try again later.")
        };
    }
}
