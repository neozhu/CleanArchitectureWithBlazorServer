namespace CleanArchitecture.Blazor.Application.Common.Interfaces.MultiTenant;

public interface ITenantProvider
{
    Task<string> GetTenant();
    Guid Register(Action callback);
    Task SetTenant(string tenant);
    void Unregister(Guid id);
}