// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CleanArchitecture.Blazor.Application.Features.Tenants.Caching;

public static class TenantCacheKey
{
    private static readonly TimeSpan refreshInterval = TimeSpan.FromHours(1);
    public const string GetAllCacheKey = "all-Tenants";
    public const string TenantsCacheKey = "all-TenantsCacheKey";
    public static string GetPaginationCacheKey(string parameters) {
        return $"TenantsWithPaginationQuery,{parameters}";
    }
    static TenantCacheKey()
    {
        _tokensource = new CancellationTokenSource(refreshInterval);
    }
    private static CancellationTokenSource _tokensource;
    public static CancellationTokenSource SharedExpiryTokenSource()
    {
        if (_tokensource.IsCancellationRequested)
        {
            _tokensource= new CancellationTokenSource(refreshInterval);
        }
        return _tokensource;
    }
    public static void Refresh() => SharedExpiryTokenSource().Cancel();
    public static MemoryCacheEntryOptions MemoryCacheEntryOptions => new MemoryCacheEntryOptions().AddExpirationToken(new CancellationChangeToken(SharedExpiryTokenSource().Token));
}

