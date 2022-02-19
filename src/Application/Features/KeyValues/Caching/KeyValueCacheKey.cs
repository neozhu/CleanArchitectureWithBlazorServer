// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CleanArchitecture.Blazor.Application.Features.KeyValues.Caching;

public static class KeyValueCacheKey
{
    public const string GetAllCacheKey = "all-keyvalues";
    public static string GetCacheKey(string name)
    {
        return $"{name}-keyvalues";
    }
    static KeyValueCacheKey()
    {
        SharedExpiryTokenSource = new CancellationTokenSource(new TimeSpan(3,0,0));
        
    }
    public static CancellationTokenSource? SharedExpiryTokenSource { get; private set; }
    public static MemoryCacheEntryOptions MemoryCacheEntryOptions  => new MemoryCacheEntryOptions().AddExpirationToken(new CancellationChangeToken(SharedExpiryTokenSource.Token));
   
    

}
