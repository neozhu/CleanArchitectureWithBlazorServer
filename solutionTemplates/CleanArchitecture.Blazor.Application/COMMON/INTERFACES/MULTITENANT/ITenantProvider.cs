namespace CleanArchitecture.Blazor.$safeprojectname$.Common.Interfaces.MultiTenant;

public interface ITenantProvider
{
    Task<string> GetTenant();
    Guid Register(Action callback);
    Task SetTenant(string tenant);
    void Unregister(Guid id);
}