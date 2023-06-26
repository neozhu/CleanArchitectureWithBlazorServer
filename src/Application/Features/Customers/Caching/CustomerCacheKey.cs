// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CleanArchitecture.Blazor.Application.Features.Customers.Caching;

public static class CustomerCacheKey
{
    public const string GetAllCacheKey = "all-Customers";
    private static readonly TimeSpan RefreshInterval = TimeSpan.FromHours(3);
    private static CancellationTokenSource _tokenSource;

    static CustomerCacheKey()
    {
        _tokenSource = new CancellationTokenSource(RefreshInterval);
    }

    public static MemoryCacheEntryOptions MemoryCacheEntryOptions =>
        new MemoryCacheEntryOptions().AddExpirationToken(new CancellationChangeToken(SharedExpiryTokenSource().Token));

    public static string GetPaginationCacheKey(string parameters)
    {
        return $"CustomerCacheKey:CustomersWithPaginationQuery,{parameters}";
    }

    public static string GetByNameCacheKey(string parameters)
    {
        return $"CustomerCacheKey:GetByNameCacheKey,{parameters}";
    }

    public static string GetByIdCacheKey(string parameters)
    {
        return $"CustomerCacheKey:GetByIdCacheKey,{parameters}";
    }

    public static CancellationTokenSource SharedExpiryTokenSource()
    {
        if (_tokenSource.IsCancellationRequested) _tokenSource = new CancellationTokenSource(RefreshInterval);
        return _tokenSource;
    }

    public static void Refresh()
    {
        SharedExpiryTokenSource().Cancel();
    }
}