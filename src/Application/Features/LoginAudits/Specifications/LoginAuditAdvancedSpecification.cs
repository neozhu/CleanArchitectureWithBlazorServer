// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Domain.Identity;

namespace CleanArchitecture.Blazor.Application.Features.LoginAudits.Specifications;

public class LoginAuditAdvancedSpecification : Specification<LoginAudit>
{
    public LoginAuditAdvancedSpecification(LoginAuditAdvancedFilter filter)
    {
        DateTime today = DateTime.UtcNow;
        var todayrange = today.GetDateRange(LoginAuditListView.TODAY.ToString(), filter.CurrentUser?.LocalTimeOffset ?? TimeSpan.Zero);
        var last30daysrange = today.GetDateRange(LoginAuditListView.LAST_30_DAYS.ToString(), filter.CurrentUser?.LocalTimeOffset ?? TimeSpan.Zero);

        Query.Where(p => p.LoginTimeUtc != default, filter.Keyword is null)
             .Where(p => p.UserName!.Contains(filter.Keyword!) || p.IpAddress!.Contains(filter.Keyword!), filter.Keyword is not null)
             .Where(p =>p.UserId == filter.CurrentUser!.UserId, filter.CurrentUser != null && filter.ListView == LoginAuditListView.My)
             .Where(p=> p.UserId == filter.CurrentUser!.UserId, filter.CurrentUser != null && !IsAdmin(filter.CurrentUser))
             .Where(p =>p.Success == filter.Success, filter.Success.HasValue)
             .Where(x => x.LoginTimeUtc >= todayrange.Start && x.LoginTimeUtc < todayrange.End.AddDays(1), filter.ListView == LoginAuditListView.TODAY)
             .Where(x => x.LoginTimeUtc >= last30daysrange.Start, filter.ListView == LoginAuditListView.LAST_30_DAYS);
       

         
    }

    private static bool IsAdmin(UserProfile userProfile)
    {
        return userProfile.SuperiorId == null || 
               userProfile.AssignedRoles?.Any(role => role.Equals("Administrator", StringComparison.OrdinalIgnoreCase) || 
                                                       role.Equals("Admin", StringComparison.OrdinalIgnoreCase)) == true;
    }
}
