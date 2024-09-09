// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using ZiggyCreatures.Caching.Fusion;

namespace CleanArchitecture.Blazor.Application.Pipeline;

public class FusionCacheBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IFusionCacheRequest<TResponse>
{
    private readonly IFusionCache _fusionCache;
    private readonly ILogger<FusionCacheBehaviour<TRequest, TResponse>> _logger;

    public FusionCacheBehaviour(
        IFusionCache fusionCache,
        ILogger<FusionCacheBehaviour<TRequest, TResponse>> logger
    )
    {
        _fusionCache = fusionCache;
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        _logger.LogTrace("Handling request of type {RequestType} with cache key {CacheKey}", nameof(request), request.CacheKey);
        var response = await _fusionCache.GetOrSetAsync(
            request.CacheKey,
            _ => next()
            ).ConfigureAwait(false);

        return response;
    }
}