// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CleanArchitecture.Blazor.Application.Features.DocumentTypes.Caching;

public static class DocumentTypeCacheKey
{
    public const string GetAllCacheKey = "all-documenttypes";
    static DocumentTypeCacheKey()
    {
        SharedExpiryTokenSource = new CancellationTokenSource(new TimeSpan(12,0,0));
    }
    public static CancellationTokenSource SharedExpiryTokenSource { get; private set; }
    public static MemoryCacheEntryOptions MemoryCacheEntryOptions => new MemoryCacheEntryOptions().AddExpirationToken(new CancellationChangeToken(SharedExpiryTokenSource.Token));
}
