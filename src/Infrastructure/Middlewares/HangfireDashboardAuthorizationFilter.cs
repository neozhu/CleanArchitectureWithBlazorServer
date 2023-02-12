using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hangfire.Dashboard;

namespace CleanArchitecture.Blazor.Infrastructure.Middlewares;
public class HangfireDashboardAsyncAuthorizationFilter : IDashboardAsyncAuthorizationFilter
{

    public Task<bool> AuthorizeAsync(DashboardContext context)
    {
        return Task.FromResult(true);
    }
}

public class HangfireDashboardAuthorizationFilter : IDashboardAuthorizationFilter
{
    public bool Authorize(DashboardContext context)
    {

        return true;
    }
}