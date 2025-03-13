﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CleanArchitecture.Blazor.Application.Features.SystemLogs.Caching;

public static class SystemLogsCacheKey
{
    public const string GetAllCacheKey = "all-logs";
    public static string GetChartDataCacheKey(string parameters)
    {
        return $"GetChartDataCacheKey,{parameters}";
    }
    public static string GetPaginationCacheKey(string parameters)
    {
        return $"LogsTrailsWithPaginationQuery,{parameters}";
    }
    public static IEnumerable<string>? Tags => new string[] { "logs" };
    public static void Refresh()
    {
        FusionCacheFactory.RemoveByTags(Tags);
    }
}