// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CleanArchitecture.Blazor.Application.Features.Customers.Caching;

public static class CustomerCacheKey
{
    private static readonly TimeSpan refreshInterval = TimeSpan.FromHours(3);
    public const string GetAllCacheKey = "all-Customers";
    public static string GetPaginationCacheKey(string parameters) {
        return $"CustomerCacheKey:CustomersWithPaginationQuery,{parameters}";
    }
    public static string GetByNameCacheKey(string parameters) {
        return $"CustomerCacheKey:GetByNameCacheKey,{parameters}";
    }
    public static string GetByIdCacheKey(string parameters) {
        return $"CustomerCacheKey:GetByIdCacheKey,{parameters}";
    }
    static CustomerCacheKey()
    {
        _tokensource = new CancellationTokenSource(refreshInterval);
    }
    private static CancellationTokenSource _tokensource;
    public static CancellationTokenSource SharedExpiryTokenSource()
    {
        if (_tokensource.IsCancellationRequested)
        {
            _tokensource = new CancellationTokenSource(refreshInterval);
        }
        return _tokensource;
    }
    public static void Refresh() => SharedExpiryTokenSource().Cancel();
    public static MemoryCacheEntryOptions MemoryCacheEntryOptions => new MemoryCacheEntryOptions().AddExpirationToken(new CancellationChangeToken(SharedExpiryTokenSource().Token));
}

