
using CleanArchitecture.Blazor.Application.Common.Interfaces.MultiTenant;
using CleanArchitecture.Blazor.$safeprojectname$.Services.MultiTenant;

namespace CleanArchitecture.Blazor.$safeprojectname$.Extensions;
public static class MultiTenantServiceCollectionExtensions
{
    public static IServiceCollection AddMultiTenantService(this IServiceCollection services)
        => services.AddScoped<ITenantProvider, TenantProvider>();
}
