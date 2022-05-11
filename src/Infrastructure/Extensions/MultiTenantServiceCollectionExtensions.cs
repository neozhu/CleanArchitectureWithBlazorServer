
using CleanArchitecture.Blazor.Application.Common.Interfaces.MultiTenant;
using CleanArchitecture.Blazor.Infrastructure.Services.MultiTenant;

namespace CleanArchitecture.Blazor.Infrastructure.Extensions;
public static class MultiTenantServiceCollectionExtensions
{
    public static IServiceCollection AddMultiTenantService(this IServiceCollection services)
        => services.AddScoped<ITenantProvider, TenantProvider>();
}
