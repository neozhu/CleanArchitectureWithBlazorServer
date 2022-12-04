// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CleanArchitecture.Blazor.Application.Features.Loggers.Caching;

public static class LogsCacheKey
{
    private static readonly TimeSpan refreshInterval = TimeSpan.FromSeconds(30);
    public const string GetAllCacheKey = "all-logs";
    public static string GetChartDataCacheKey(string parameters) {
        return $"GetChartDataCacheKey,{parameters}";
    }
    public static string GetPaginationCacheKey(string parameters)
    {
        return $"LogsTrailsWithPaginationQuery,{parameters}";
    }
    static LogsCacheKey()
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
    public static MemoryCacheEntryOptions MemoryCacheEntryOptions => new MemoryCacheEntryOptions().AddExpirationToken(new CancellationChangeToken(SharedExpiryTokenSource().Token));
}

