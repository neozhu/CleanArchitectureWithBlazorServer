// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CleanArchitecture.Blazor.Application.Features.Documents.Caching;

public static class DocumentCacheKey
{
    public const string GetAllCacheKey = "all-documents";
    static DocumentCacheKey()
    {
        SharedExpiryTokenSource = new CancellationTokenSource(new TimeSpan(12,0,0));
    }
    public static CancellationTokenSource SharedExpiryTokenSource { get; private set; }
    public static MemoryCacheEntryOptions MemoryCacheEntryOptions => new MemoryCacheEntryOptions().AddExpirationToken(new CancellationChangeToken(SharedExpiryTokenSource.Token));
}
