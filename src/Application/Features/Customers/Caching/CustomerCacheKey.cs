// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CleanArchitecture.Razor.Application.Features.Customers.Caching;

public static class CustomerCacheKey
{
    public const string GetAllCacheKey = "all-customers";
    public static string GetPaginationCacheKey(string parameters)
    {
        return $"CustomersWithPaginationQuery,{parameters}";
    }
    static CustomerCacheKey()
    {
        ResetCacheToken = new CancellationTokenSource();
    }
    public static CancellationTokenSource ResetCacheToken { get; private set; }
    public static MemoryCacheEntryOptions MemoryCacheEntryOptions => new MemoryCacheEntryOptions().AddExpirationToken(new CancellationChangeToken(ResetCacheToken.Token));
}
