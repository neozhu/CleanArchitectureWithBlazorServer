using CleanArchitecture.Blazor.$safeprojectname$.Features.Tenants.DTOs;

namespace CleanArchitecture.Blazor.$safeprojectname$.Common.Interfaces.MultiTenant;
public interface ITenantsService
{
    List<TenantDto> DataSource { get; }
    event Action? OnChange;
    Task Initialize();
    Task Refresh();
}
