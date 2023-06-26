// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CleanArchitecture.Blazor.Application.Features.Documents.Caching;

public static class DocumentCacheKey
{
    public const string GetAllCacheKey = "all-documents";
    private static readonly TimeSpan RefreshInterval = TimeSpan.FromHours(1);
    private static CancellationTokenSource _tokenSource;

    static DocumentCacheKey()
    {
        _tokenSource = new CancellationTokenSource(RefreshInterval);
    }

    public static MemoryCacheEntryOptions MemoryCacheEntryOptions =>
        new MemoryCacheEntryOptions().AddExpirationToken(new CancellationChangeToken(SharedExpiryTokenSource().Token));

    public static string GetStreamByIdKey(int id)
    {
        return $"GetStreamByIdKey:{id}";
    }

    public static string GetPaginationCacheKey(string parameters)
    {
        return $"DocumentsWithPaginationQuery,{parameters}";
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