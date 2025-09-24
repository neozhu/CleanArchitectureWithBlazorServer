// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CleanArchitecture.Blazor.Application.Features.AuditTrails.Caching;

public static class AuditTrailsCacheKey
{
    public const string GetAllCacheKey = "all-audittrails";

    public static string GetPaginationCacheKey(string parameters)
    {
        return $"AuditTrailsWithPaginationQuery,{parameters}";
    }

    public static IEnumerable<string>? Tags => new string[] { "audittrail" };
    public static void Refresh()
    {
        FusionCacheFactory.RemoveByTags(Tags);
    }

}
