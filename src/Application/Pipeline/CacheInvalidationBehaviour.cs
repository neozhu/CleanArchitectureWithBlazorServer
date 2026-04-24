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

    public async ValueTask<TResponse> Handle(TRequest request, MessageHandlerDelegate<TRequest, TResponse> next,
        CancellationToken cancellationToken)
    {
        _logger.LogTrace("Handling request of type {RequestType}", typeof(TRequest).Name);
        var response = await next(request, cancellationToken).ConfigureAwait(false);
        if (!string.IsNullOrEmpty(request.CacheKey))
        {
            _cache.Remove(request.CacheKey);
            _logger.LogTrace("Cache key {CacheKey} removed from cache", request.CacheKey);
        }
        if (request.Tags != null && request.Tags.Any())
        {
            await _cache.RemoveByTagsAsync(request.Tags, cancellationToken);
            foreach (var tag in request.Tags)
            {
                _logger.LogTrace("Cache tag {CacheTag} removed from cache", tag);
            }
        }
        return response;
    }
}
