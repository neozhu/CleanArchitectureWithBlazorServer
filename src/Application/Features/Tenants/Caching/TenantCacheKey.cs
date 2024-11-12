// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CleanArchitecture.Blazor.Application.Features.Tenants.Caching;

public static class TenantCacheKey
{
    public const string GetAllCacheKey = "all-Tenants";
    public const string TenantsCacheKey = "all-TenantsCacheKey";
    public static string GetPaginationCacheKey(string parameters)
    {
        return $"TenantsWithPaginationQuery,{parameters}";
    }
    public static IEnumerable<string>? Tags => new string[] { "tenant" };
    public static void Refresh()
    {
        FusionCacheFactory.RemoveByTags(Tags);
    }

}