// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CleanArchitecture.Blazor.Application.Features.PicklistSets.Caching;

public static class PicklistSetCacheKey
{
    public const string GetAllCacheKey = "all-PicklistSet";
    public const string PicklistCacheKey = "all-PicklistSetcachekey";
    public static string GetCacheKey(string name)
    {
        return $"{name}-PicklistSet";
    }
    public static IEnumerable<string>? Tags => new string[] { "picklistset" };
    public static void Refresh()
    {
        FusionCacheFactory.RemoveByTags(Tags);
    }
}