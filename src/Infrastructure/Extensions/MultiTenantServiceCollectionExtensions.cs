 
using CleanArchitecture.Blazor.Infrastructure.Services.Tenant;

namespace CleanArchitecture.Blazor.Infrastructure.Extensions;
public static class MultiTenantServiceCollectionExtensions
{
    public static IServiceCollection AddMultiTenantService(this IServiceCollection services)
        => services.AddScoped<TenantProvider>();
}
