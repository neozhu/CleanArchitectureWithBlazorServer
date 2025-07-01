// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Domain.Identity;

namespace CleanArchitecture.Blazor.Application.Features.LoginAudits.Specifications;

public class LoginAuditAdvancedSpecification : Specification<LoginAudit>
{
    public LoginAuditAdvancedSpecification(LoginAuditAdvancedFilter filter)
    {
        var today = DateTime.UtcNow.Date;
        var todayStart = today;
        var todayEnd = today.AddDays(1);
        var last30Days = today.AddDays(-30);

        Query.Where(p => p.LoginTimeUtc != default, filter.Keyword is null)
             .Where(p => p.UserName!.Contains(filter.Keyword!) || p.IpAddress!.Contains(filter.Keyword!), filter.Keyword is not null);

        // Apply list view filter
        switch (filter.ListView)
        {
            case LoginAuditListView.All:
                // No additional filter for "All"
                break;
            case LoginAuditListView.My:
                if (filter.CurrentUser != null)
                {
                    Query.Where(x => x.UserId == filter.CurrentUser.UserId);
                }
                break;
            case LoginAuditListView.TODAY:
                Query.Where(x => x.LoginTimeUtc >= todayStart && x.LoginTimeUtc < todayEnd);
                break;
            case LoginAuditListView.LAST_30_DAYS:
                Query.Where(x => x.LoginTimeUtc >= last30Days);
                break;
            case LoginAuditListView.Successful:
                Query.Where(x => x.Success == true);
                break;
            case LoginAuditListView.Failed:
                Query.Where(x => x.Success == false);
                break;
        }

        // Apply success filter if specified
        if (filter.Success.HasValue)
        {
            Query.Where(x => x.Success == filter.Success.Value);
        }

        // Apply provider filter if specified
        if (!string.IsNullOrEmpty(filter.Provider))
        {
            Query.Where(x => x.Provider == filter.Provider);
        }

        // Apply user-specific filter for non-admin users
        if (filter.CurrentUser != null && !IsAdmin(filter.CurrentUser))
        {
            Query.Where(x => x.UserId == filter.CurrentUser.UserId);
        }
    }

    private static bool IsAdmin(UserProfile userProfile)
    {
        return userProfile.SuperiorId == null || 
               userProfile.AssignedRoles?.Any(role => role.Equals("Administrator", StringComparison.OrdinalIgnoreCase) || 
                                                       role.Equals("Admin", StringComparison.OrdinalIgnoreCase)) == true;
    }
}
