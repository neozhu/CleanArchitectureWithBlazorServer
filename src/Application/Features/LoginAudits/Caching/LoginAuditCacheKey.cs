// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.


namespace CleanArchitecture.Blazor.Application.Features.LoginAudits.Caching;

public static class LoginAuditCacheKey
{
    public const string GetAllCacheKey = "all-loginaudits";
    public static string GetPaginationCacheKey(string parameters)
    {
        return $"LoginAuditCacheKey:LoginAuditsWithPaginationQuery,{parameters}";
    }
    public static IEnumerable<string> Tags => new string[] { "loginaudits","userloginrisksummary", "statistics" };
    public static void Refresh()
    {
        FusionCacheFactory.RemoveByTags(Tags);
    }
}
