// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CleanArchitecture.Blazor.Application.Pipeline;

public class MemoryCacheBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : ICacheableRequest<TResponse>
{
    private readonly IAppCache _cache;
    private readonly ILogger<MemoryCacheBehaviour<TRequest, TResponse>> _logger;

    public MemoryCacheBehaviour(
        IAppCache cache,
        ILogger<MemoryCacheBehaviour<TRequest, TResponse>> logger
    )
    {
        _cache = cache;
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        _logger.LogTrace("Handling request of type {RequestType} with cache key {CacheKey}", nameof(request), request.CacheKey);
        var response = await _cache.GetOrAddAsync(
            request.CacheKey,
            async () =>
                await next(),
            request.Options).ConfigureAwait(false);

        return response;
    }
}