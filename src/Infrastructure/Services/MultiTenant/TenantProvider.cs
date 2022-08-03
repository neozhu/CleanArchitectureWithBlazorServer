using CleanArchitecture.Blazor.Application.Common.Interfaces.MultiTenant;
using CleanArchitecture.Blazor.Infrastructure.Constants.LocalStorage;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace CleanArchitecture.Blazor.Infrastructure.Services.MultiTenant;
public sealed class TenantProvider : ITenantProvider
{
    private readonly ProtectedLocalStorage _protectedLocalStorage;
    private readonly IDictionary<Guid, Action> _callbacks = new Dictionary<Guid, Action>();
    public TenantProvider(ProtectedLocalStorage protectedLocalStorage)
    {
        _protectedLocalStorage = protectedLocalStorage;
    }
    public async Task SetTenant(string tenantId,string tenantName)
    {
        await _protectedLocalStorage.SetAsync(LocalStorage.TENANTID, tenantId);
        await _protectedLocalStorage.SetAsync(LocalStorage.TENANTNAME, tenantName);
        foreach (var callback in _callbacks.Values)
        {
            callback();
        }
    }
    public async Task Clear()
    {
        await _protectedLocalStorage.DeleteAsync(LocalStorage.TENANTID);
        await _protectedLocalStorage.DeleteAsync(LocalStorage.TENANTNAME);
        foreach (var callback in _callbacks.Values)
        {
            callback();
        }
    }
    public async Task<string> GetTenantId()
    {
        try
        {
            var tenant = string.Empty;
            var storedPrincipal = await _protectedLocalStorage.GetAsync<string>(LocalStorage.TENANTID);
            if (storedPrincipal.Success && storedPrincipal.Value is not null)
            {
                tenant = storedPrincipal.Value;
            }

            return tenant;
        }
        catch
        {
            return String.Empty;
        }
    }
    public void Unregister(Guid id)
    {
        if (_callbacks.ContainsKey(id))
        {
            _callbacks.Remove(id);
        }
    }

    public Guid Register(Action callback)
    {
        var id = Guid.NewGuid();
        _callbacks.Add(id, callback);
        return id;
    }
}
