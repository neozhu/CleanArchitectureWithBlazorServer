// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CleanArchitecture.Blazor.Application.Pipeline;

public class FusionCacheBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : ICacheableRequest<TResponse>
{
    private readonly IAppCache _cache;
    private readonly ILogger<FusionCacheBehaviour<TRequest, TResponse>> _logger;

    public FusionCacheBehaviour(
        IAppCache cache,
        ILogger<FusionCacheBehaviour<TRequest, TResponse>> logger
    )
    {
        _cache = cache;
        _logger = logger;
    }

    public async ValueTask<TResponse> Handle(TRequest request, MessageHandlerDelegate<TRequest, TResponse> next,
        CancellationToken cancellationToken)
    {
        _logger.LogTrace("Handling request of type {RequestType} with cache key {CacheKey}", nameof(request), request.CacheKey);
        var response = await _cache.GetOrSetAsync(
            request.CacheKey,
            _ => next(request, cancellationToken).AsTask(),
            request.Tags,
            cancellationToken
            ).ConfigureAwait(false);

        return response;
    }
}
