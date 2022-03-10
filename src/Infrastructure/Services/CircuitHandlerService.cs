using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Server.Circuits;

namespace CleanArchitecture.Blazor.Infrastructure.Services;
public class CircuitHandlerService : CircuitHandler
{
    private readonly IIdentityService _identityService;
    private readonly ICurrentUserService _currentUserService;

    public ConcurrentDictionary<string, Circuit> Circuits { get; set; }
    public event EventHandler CircuitsChanged;

    protected virtual void OnCircuitsChanged()
    => CircuitsChanged?.Invoke(this, EventArgs.Empty);

    public CircuitHandlerService(
        IIdentityService identityService,
        ICurrentUserService currentUserService
        )
    {
        Circuits = new ConcurrentDictionary<string, Circuit>();
        _identityService = identityService;
        _currentUserService = currentUserService;
    }

    public override async Task OnCircuitOpenedAsync(Circuit circuit, CancellationToken cancellationToken)
    {
        var userId =await _currentUserService.UserId();
        await _identityService.UpdateLiveStatus(userId, true);
        Circuits[circuit.Id] = circuit;
        OnCircuitsChanged();
        await base.OnCircuitOpenedAsync(circuit, cancellationToken);
    }

    public override async Task OnCircuitClosedAsync(Circuit circuit, CancellationToken cancellationToken)
    {
        
        Circuit circuitRemoved;
        Circuits.TryRemove(circuit.Id, out circuitRemoved);
        OnCircuitsChanged();
        await base.OnCircuitClosedAsync(circuit, cancellationToken);
    }

    public override async Task OnConnectionDownAsync(Circuit circuit, CancellationToken cancellationToken)
    {
        var userId = await _currentUserService.UserId();
        await _identityService.UpdateLiveStatus(userId, false);
        await base.OnConnectionDownAsync(circuit, cancellationToken);
    }

    public override Task OnConnectionUpAsync(Circuit circuit, CancellationToken cancellationToken)
    {
        return base.OnConnectionUpAsync(circuit, cancellationToken);
    }

}