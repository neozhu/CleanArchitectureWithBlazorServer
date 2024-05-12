using Hangfire.Dashboard;

namespace CleanArchitecture.Blazor.Server.UI.Middlewares;

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