// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using ZiggyCreatures.Caching.Fusion;

namespace CleanArchitecture.Blazor.Application.Pipeline;

public class CacheInvalidationBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : ICacheInvalidatorRequest<TResponse>
{
    private readonly IFusionCache _cache;
    private readonly ILogger<CacheInvalidationBehaviour<TRequest, TResponse>> _logger;

    public CacheInvalidationBehaviour(
        IFusionCache cache,
        ILogger<CacheInvalidationBehaviour<TRequest, TResponse>> logger
    )
    {
        _cache = cache;
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        _logger.LogTrace("Handling request of type {RequestType}", typeof(TRequest).Name);
        var response = await next().ConfigureAwait(false);
        if (!string.IsNullOrEmpty(request.CacheKey))
        {
            _cache.Remove(request.CacheKey);
            _logger.LogTrace("Cache key {CacheKey} removed from cache", request.CacheKey);
        }
        if(request.Tags!=null && request.Tags.Any())
        {
            foreach (var tag in request.Tags)
            {
               await _cache.RemoveByTagAsync(tag);
               _logger.LogTrace("Cache tag {CacheTag} removed from cache", tag);
            }
        }
        return response;
    }
}
