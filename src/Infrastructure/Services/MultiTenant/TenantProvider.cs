using CleanArchitecture.Blazor.Application.Common.Interfaces.MultiTenant;
using CleanArchitecture.Blazor.Infrastructure.Extensions;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Http;

namespace CleanArchitecture.Blazor.Infrastructure.Services.MultiTenant;
public sealed class TenantProvider : ITenantProvider
{

    private readonly IDictionary<Guid, Action> _callbacks = new Dictionary<Guid, Action>();
    private readonly AuthenticationStateProvider _stateProvider;

    //public TenantProvider(AuthenticationStateProvider stateProvider)
    //{
    //    _stateProvider = stateProvider;
    //}


    public async Task<string?> TenantId()
    {
        var state = await _stateProvider.GetAuthenticationStateAsync();
        return state.User?.GetTenantId();
    }

    public async Task<string?> TenantName()
    {
        var state = await _stateProvider.GetAuthenticationStateAsync();
        return state.User?.GetTenantName();
    }
    public void Unregister(Guid id)
    {
        if (_callbacks.ContainsKey(id))
        {
            _callbacks.Remove(id);
        }
    }
    public void Clear()
    {
        _callbacks.Clear();
    }
    public void Update()
    {
        foreach (var callback in _callbacks.Values)
        {
            callback();
        }
    }

    public Guid Register(Action callback)
    {
        var id = Guid.NewGuid();
        _callbacks.Add(id, callback);
        return id;
    }
}
