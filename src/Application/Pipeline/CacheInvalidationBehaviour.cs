// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CleanArchitecture.Blazor.Application.Pipeline;

public class CacheInvalidationBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : ICacheInvalidatorRequest<TResponse>
{
    private readonly IAppCache _cache;
    private readonly ILogger<CacheInvalidationBehaviour<TRequest, TResponse>> _logger;

    public CacheInvalidationBehaviour(
        IAppCache cache,
        ILogger<CacheInvalidationBehaviour<TRequest, TResponse>> logger
    )
    {
        _cache = cache;
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        _logger.LogTrace("Handling request of type {RequestType} with details {@Request}", nameof(request), request);
        var response = await next().ConfigureAwait(false);
        if (!string.IsNullOrEmpty(request.CacheKey))
        {
            _cache.Remove(request.CacheKey);
            _logger.LogTrace("Cache key {CacheKey} removed from cache", request.CacheKey);
        }
        request.SharedExpiryTokenSource?.Cancel();
        return response;
    }
}