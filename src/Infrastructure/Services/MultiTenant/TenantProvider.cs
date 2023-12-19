using CleanArchitecture.Blazor.Application.Common.Interfaces.MultiTenant;

namespace CleanArchitecture.Blazor.Infrastructure.Services.MultiTenant;

public sealed class TenantProvider : ITenantProvider
{
    private readonly IDictionary<Guid, Action> _callbacks = new Dictionary<Guid, Action>();
    public string? TenantId { get; set; }
    public string? TenantName { get; set; }

    public void Unregister(Guid id)
    {
        if (_callbacks.ContainsKey(id)) _callbacks.Remove(id);
    }

    public void Clear()
    {
        _callbacks.Clear();
    }

    public void Update()
    {
        foreach (var callback in _callbacks.Values) callback();
    }

    public Guid Register(Action callback)
    {
        var id = Guid.NewGuid();
        _callbacks.Add(id, callback);
        return id;
    }
}