﻿namespace CleanArchitecture.Blazor.Application.Common.ExceptionHandlers;

public class
    ValidationExceptionHandler<TRequest, TResponse, TException> : IRequestExceptionHandler<TRequest, TResponse,
    TException>
    where TRequest : IRequest<Result<int>>
    where TResponse : Result<int>
    where TException : ValidationException
{

    public Task Handle(TRequest request, TException exception, RequestExceptionHandlerState<TResponse> state,
        CancellationToken cancellationToken)
    {
        state.SetHandled(
            (TResponse)Result<int>.Failure(exception.Errors.Select(x => x.ErrorMessage).Distinct().ToArray()));
        return Task.CompletedTask;
    }
}