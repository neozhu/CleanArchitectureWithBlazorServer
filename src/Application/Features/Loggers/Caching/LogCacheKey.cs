// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CleanArchitecture.Blazor.Application.Features.Logs.Caching;

public static class LogsCacheKey
{

    public static string GetChartDataCacheKey(string parameters) {
        return $"GetChartDataCacheKey,{parameters}";
    }
    static LogsCacheKey()
    {
        _tokensource = new CancellationTokenSource(new TimeSpan(0,15,0));
    }
    private static CancellationTokenSource _tokensource;
    public static CancellationTokenSource SharedExpiryTokenSource()
    {
        if (_tokensource.IsCancellationRequested)
        {
            _tokensource = new CancellationTokenSource(new TimeSpan(0,15,0));
        }
        return _tokensource;
    }
    public static MemoryCacheEntryOptions MemoryCacheEntryOptions => new MemoryCacheEntryOptions().AddExpirationToken(new CancellationChangeToken(SharedExpiryTokenSource().Token));
}

