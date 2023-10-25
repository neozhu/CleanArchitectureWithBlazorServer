// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CleanArchitecture.Blazor.Application.Features.Products.Caching;

public static class ProductCacheKey
{
    public const string GetAllCacheKey = "all-Products";
    private static readonly TimeSpan RefreshInterval = TimeSpan.FromHours(1);
    private static CancellationTokenSource _tokenSource;
    private static readonly object _tokenLock = new object();
    public static string GetProductByIdCacheKey(int id)
    {
        return $"GetProductById,{id}";
    }

    static ProductCacheKey()
    {
        _tokenSource = new CancellationTokenSource(RefreshInterval);
    }

    public static MemoryCacheEntryOptions MemoryCacheEntryOptions =>
        new MemoryCacheEntryOptions().AddExpirationToken(new CancellationChangeToken(SharedExpiryTokenSource().Token));

    public static string GetPaginationCacheKey(string parameters)
    {
        return $"ProductsWithPaginationQuery,{parameters}";
    }

    public static CancellationTokenSource SharedExpiryTokenSource()
    {
        lock (_tokenLock)
        {
            if (_tokenSource.IsCancellationRequested)
            {
                _tokenSource = new CancellationTokenSource(RefreshInterval);
            }

            return _tokenSource;
        }
    }

    public static void Refresh()
    {
        lock (_tokenLock)
        {
            if (!_tokenSource.IsCancellationRequested)
            {
                _tokenSource.Cancel();
                _tokenSource = new CancellationTokenSource(RefreshInterval);
            }
        }
    }
}