// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CleanArchitecture.Blazor.Application.Features.AuditTrails.Caching;

public static class AuditTrailsCacheKey
{
    public const string GetAllCacheKey = "all-audittrails";
    private static readonly TimeSpan RefreshInterval = TimeSpan.FromMilliseconds(30);
    private static CancellationTokenSource _tokenSource;

    static AuditTrailsCacheKey()
    {
        _tokenSource = new CancellationTokenSource(RefreshInterval);
    }

    public static MemoryCacheEntryOptions MemoryCacheEntryOptions =>
        new MemoryCacheEntryOptions().AddExpirationToken(new CancellationChangeToken(SharedExpiryTokenSource().Token));

    public static string GetPaginationCacheKey(string parameters)
    {
        return $"AuditTrailsWithPaginationQuery,{parameters}";
    }

    public static CancellationTokenSource SharedExpiryTokenSource()
    {
        if (_tokenSource.IsCancellationRequested) _tokenSource = new CancellationTokenSource(RefreshInterval);
        return _tokenSource;
    }

    public static void Refresh()
    {
        SharedExpiryTokenSource().Cancel();
    }
}