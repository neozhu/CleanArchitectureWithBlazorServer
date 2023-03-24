using CleanArchitecture.Blazor.Application.Features.Tenants.DTOs;

namespace CleanArchitecture.Blazor.Application.Common.Interfaces.MultiTenant;
public interface ITenantService
{
    List<TenantDto> DataSource { get; }
    event Action? OnChange;
    Task InitializeAsync();
    void Initialize();
    Task Refresh();
}
