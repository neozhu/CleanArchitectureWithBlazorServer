using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CleanArchitecture.Blazor.Infrastructure.Constants.LocalStorage;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace CleanArchitecture.Blazor.Infrastructure.Services.Tenant;
public sealed class TenantProvider
{
    private readonly ProtectedLocalStorage _protectedLocalStorage;
    private readonly IDictionary<Guid, Action> callbacks = new Dictionary<Guid, Action>();

    public TenantProvider(ProtectedLocalStorage protectedLocalStorage)
    {
        _protectedLocalStorage = protectedLocalStorage;
    }
    public async Task SetTenant(string tenant)
    {
        await _protectedLocalStorage.SetAsync(LocalStorage.TENANTID, tenant);
        foreach (var callback in callbacks.Values)
        {
            callback();
        }
    }
    public async Task<string> GetTenant()
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
        if (callbacks.ContainsKey(id))
        {
            callbacks.Remove(id);
        }
    }

    public Guid Register(Action callback)
    {
        var id = Guid.NewGuid();
        callbacks.Add(id, callback);
        return id;
    }
}
