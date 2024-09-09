// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CleanArchitecture.Blazor.Application.Common.Interfaces.Caching;

public interface ICacheableRequest<TResponse> : IRequest<TResponse>
{
    string CacheKey => string.Empty;
    MemoryCacheEntryOptions? Options { get; }
}

public interface IFusionCacheRequest<TResponse> : IRequest<TResponse>
{
    string CacheKey => string.Empty;
}