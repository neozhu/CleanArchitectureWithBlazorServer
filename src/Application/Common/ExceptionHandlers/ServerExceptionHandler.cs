﻿namespace CleanArchitecture.Blazor.Application.Common.ExceptionHandlers;

public class
    ServerExceptionHandler<TRequest, TResponse, TException> : IRequestExceptionHandler<TRequest, TResponse, TException>
    where TRequest : IRequest<Result>
    where TResponse : Result
    where TException : ServerException
{
    private readonly ILogger<ServerExceptionHandler<TRequest, TResponse, TException>> _logger;

    public ServerExceptionHandler(ILogger<ServerExceptionHandler<TRequest, TResponse, TException>> logger)
    {
        _logger = logger;
    }

    public Task Handle(TRequest request, TException exception, RequestExceptionHandlerState<TResponse> state,
        CancellationToken cancellationToken)
    {
        state.SetHandled((TResponse)Result.Failure(exception.Message));
        _logger.LogError(exception, exception.Message);
        return Task.CompletedTask;
    }
}