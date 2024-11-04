// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Common.Interfaces.Identity;

namespace CleanArchitecture.Blazor.Application.Pipeline.PreProcessors;

public class LoggingPreProcessor<TRequest> : IRequestPreProcessor<TRequest> where TRequest : notnull
{
    private readonly ICurrentUserAccessor _currentUserAccessor;
    private readonly ILogger _logger;


    public LoggingPreProcessor(ILogger<TRequest> logger, ICurrentUserAccessor currentUserAccessor)
    {
        _logger = logger;
        _currentUserAccessor = currentUserAccessor;
    }

    public Task Process(TRequest request, CancellationToken cancellationToken)
    {
        var requestName = nameof(TRequest);
        var userName = _currentUserAccessor.SessionInfo?.UserName;
        _logger.LogTrace("Processing request of type {RequestName} with details {@Request} by user {UserName}",
         requestName, request, userName);
        return Task.CompletedTask;
    }
}