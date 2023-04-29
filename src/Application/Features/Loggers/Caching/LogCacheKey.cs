// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CleanArchitecture.Blazor.Application.Features.Loggers.Caching;

public static class LogsCacheKey
{
    public const string GetAllCacheKey = "all-logs";
    private static readonly TimeSpan RefreshInterval = TimeSpan.FromSeconds(30);
    private static CancellationTokenSource _tokenSource;

    static LogsCacheKey()
    {
        _tokenSource = new CancellationTokenSource(RefreshInterval);
    }

    public static MemoryCacheEntryOptions MemoryCacheEntryOptions =>
        new MemoryCacheEntryOptions().AddExpirationToken(new CancellationChangeToken(SharedExpiryTokenSource().Token));

    public static string GetChartDataCacheKey(string parameters)
    {
        return $"GetChartDataCacheKey,{parameters}";
    }

    public static string GetPaginationCacheKey(string parameters)
    {
        return $"LogsTrailsWithPaginationQuery,{parameters}";
    }

    public static CancellationTokenSource SharedExpiryTokenSource()
    {
        if (_tokenSource.IsCancellationRequested)
        {
            _tokenSource = new CancellationTokenSource(RefreshInterval);
        }

        return _tokenSource;
    }
}