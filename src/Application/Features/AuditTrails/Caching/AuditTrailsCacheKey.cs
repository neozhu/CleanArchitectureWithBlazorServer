// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CleanArchitecture.Blazor.Application.Features.AuditTrails.Caching;

public static class AuditTrailsCacheKey
{
    private static readonly TimeSpan RefreshInterval = TimeSpan.FromMilliseconds(30);
    public const string GetAllCacheKey = "all-audittrails";
    public static string GetPaginationCacheKey(string parameters)
    {
        return $"AuditTrailsWithPaginationQuery,{parameters}";
    }
    static AuditTrailsCacheKey()
    {
        _tokenSource = new CancellationTokenSource(RefreshInterval);
    }
    private static CancellationTokenSource _tokenSource;
    public static CancellationTokenSource SharedExpiryTokenSource()
    {
        if (_tokenSource.IsCancellationRequested)
        {
            _tokenSource = new CancellationTokenSource(RefreshInterval);
        }
        return _tokenSource;
    }
    public static void Refresh() => SharedExpiryTokenSource().Cancel();
    public static MemoryCacheEntryOptions MemoryCacheEntryOptions => new MemoryCacheEntryOptions().AddExpirationToken(new CancellationChangeToken(SharedExpiryTokenSource().Token));
}

