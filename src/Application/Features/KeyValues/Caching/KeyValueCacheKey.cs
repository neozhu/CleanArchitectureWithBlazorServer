// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CleanArchitecture.Blazor.Application.Features.KeyValues.Caching;

public static class KeyValueCacheKey
{
    private static readonly TimeSpan refreshInterval = TimeSpan.FromHours(1);
    public const string GetAllCacheKey = "all-keyvalues";
    public const string PicklistCacheKey = "all-picklistcachekey";
    public static string GetCacheKey(string name)
    {
        return $"{name}-keyvalues";
    }
    static KeyValueCacheKey()
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
