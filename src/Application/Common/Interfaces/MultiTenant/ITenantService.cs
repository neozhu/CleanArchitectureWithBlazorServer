using CleanArchitecture.Blazor.Application.Features.Identity.DTOs;
using CleanArchitecture.Blazor.Application.Features.Tenants.DTOs;

namespace CleanArchitecture.Blazor.Application.Common.Interfaces.MultiTenant;

public interface ITenantService
{
    List<TenantDto> DataSource { get; }
    List<TenantDto> GetAllowedTenants(ApplicationUserDto userDto);
    event Action? OnChange;
    Task InitializeAsync();
    void Initialize();
    Task Refresh();
}