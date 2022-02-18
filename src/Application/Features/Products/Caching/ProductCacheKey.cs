// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.


namespace CleanArchitecture.Blazor.Application.Features.Products.Caching;

public static class ProductCacheKey
{
    public const string GetAllCacheKey = "all-Products";
    public static string GetPagtionCacheKey(string parameters)
    {
        return "ProductsWithPaginationQuery,{parameters}";
    }
    static ProductCacheKey()
    {
        SharedExpiryTokenSource = new CancellationTokenSource(new TimeSpan(1,0,0));
    }
    public static CancellationTokenSource SharedExpiryTokenSource { get; private set; }
    public static MemoryCacheEntryOptions MemoryCacheEntryOptions => new MemoryCacheEntryOptions().AddExpirationToken(new CancellationChangeToken(SharedExpiryTokenSource.Token));
}

