// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CleanArchitecture.Razor.Application.Features.KeyValues.Caching;

public static class KeyValueCacheKey
{
    public const string GetAllCacheKey = "all-keyvalues";
    public static string GetCacheKey(string name)
    {
        return $"{name}-keyvalues";
    }
    static KeyValueCacheKey()
    {
        ResetCacheToken = new CancellationTokenSource();
    }
    public static CancellationTokenSource ResetCacheToken { get; private set; }
    public static MemoryCacheEntryOptions MemoryCacheEntryOptions => new MemoryCacheEntryOptions().AddExpirationToken(new CancellationChangeToken(ResetCacheToken.Token));
}
