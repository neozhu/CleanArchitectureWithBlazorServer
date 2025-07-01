// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Features.LoginAudits.Queries.GetMyLoginHistory;
using CleanArchitecture.Blazor.Domain.Identity;

namespace CleanArchitecture.Blazor.Application.Features.LoginAudits.Specifications;

public class MyLoginHistorySpecification : Specification<LoginAudit>
{
    public MyLoginHistorySpecification(GetMyLoginHistoryQuery query)
    {
        Query.Where(p => p.UserId == query.UserId)
             .Where(p => p.IpAddress != null && p.IpAddress.Contains(query.Keyword!) ||
                        p.BrowserInfo != null && p.BrowserInfo.Contains(query.Keyword!) ||
                        p.Region != null && p.Region.Contains(query.Keyword!) ||
                        p.Provider != null && p.Provider.Contains(query.Keyword!), 
                        !string.IsNullOrEmpty(query.Keyword));
    }
}
