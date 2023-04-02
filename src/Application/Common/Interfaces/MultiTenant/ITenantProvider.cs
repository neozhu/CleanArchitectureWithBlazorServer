namespace CleanArchitecture.Blazor.Application.Common.Interfaces.MultiTenant;

public interface ITenantProvider
{
    Task<string?> TenantId();
    Task<string?> TenantName();
    void Update();
    Guid Register(Action callback);
    void Clear();
    void Unregister(Guid id);
}