using System.Net;
using CleanArchitecture.Blazor.Application.Common.ExceptionHandlers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;

namespace CleanArchitecture.Blazor.Infrastructure.Middlewares;

internal class ExceptionHandlingMiddleware : IMiddleware
{
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    private readonly IStringLocalizer<ExceptionHandlingMiddleware> _localizer;

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
            var responseModel = await Result.FailureAsync(new string[] { exception.Message });
            var response = context.Response;
            response.ContentType = "application/json";
            if (exception is not ServerException && exception.InnerException != null)
            {
                while (exception.InnerException != null)
                {
                    exception = exception.InnerException;
                }
            }
            if (!string.IsNullOrEmpty(exception.Message))
            {
                responseModel.Errors=new string[] { exception.Message };
            }
            switch (exception)
            {
                case ServerException e:
                    response.StatusCode = (int)e.StatusCode;
                    if (e.ErrorMessages is not null)
                    {
                        responseModel.Errors = e.ErrorMessages.ToArray();
                    }
                    break;
                case KeyNotFoundException:
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    break;

                default:
                    response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    break;
            }
            //_logger.LogError(exception, $"{exception}. Request failed with Status Code {response.StatusCode}");
            await response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(responseModel));
        }
    }
}
