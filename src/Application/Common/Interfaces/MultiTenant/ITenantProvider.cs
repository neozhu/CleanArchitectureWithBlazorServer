namespace CleanArchitecture.Blazor.Application.Common.Interfaces.MultiTenant;

public interface ITenantProvider
{
    Task<string> GetTenantId();
    Guid Register(Action callback);
    Task SetTenant(string tenantId,string tenantName);
    Task Clear();
    void Unregister(Guid id);
}