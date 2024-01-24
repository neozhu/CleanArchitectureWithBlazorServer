using System.Net;
using System.Text.Json;
using CleanArchitecture.Blazor.Application.Common.ExceptionHandlers;
using Microsoft.Extensions.Localization;

namespace CleanArchitecture.Blazor.Server.Middlewares;

public class ExceptionHandlingMiddleware : IMiddleware
{
    private readonly IStringLocalizer<ExceptionHandlingMiddleware> _localizer;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(
        ILogger<ExceptionHandlingMiddleware> logger,
        IStringLocalizer<ExceptionHandlingMiddleware> localizer)
    {
        _logger = logger;
        _localizer = localizer;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception exception)
        {
            var responseModel = await Result.FailureAsync(exception.Message);
            var response = context.Response;
            response.ContentType = "application/json";
            if (exception is not ServerException && exception.InnerException != null)
                while (exception.InnerException != null)
                    exception = exception.InnerException;
            if (!string.IsNullOrEmpty(exception.Message)) responseModel = await Result.FailureAsync(exception.Message);
            switch (exception)
            {
                case ServerException e:
                    response.StatusCode = (int)e.StatusCode;
                    if (e.ErrorMessages is not null)
                        responseModel = await Result.FailureAsync(e.ErrorMessages.ToArray());
                    break;
                case KeyNotFoundException:
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    break;

                default:
                    response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    break;
            }

            //_logger.LogError(exception, $"{exception}. Request failed with Status Code {response.StatusCode}");
            await response.WriteAsync(JsonSerializer.Serialize(responseModel));
        }
    }
}