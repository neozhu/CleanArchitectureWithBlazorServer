namespace CleanArchitecture.Blazor.Application.Common.Interfaces.MultiTenant;

public interface ITenantProvider
{
    string TenantId { get; }
    string TenantName { get; }
    void Update();
    Guid Register(Action callback);
    void Clear();
    void Unregister(Guid id);
}