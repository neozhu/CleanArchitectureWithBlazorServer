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


    public ConcurrentDictionary<string, Circuit> Circuits { get; set; }
    public event EventHandler<bool> CircuitsChanged;

    protected virtual void OnCircuitsChanged(bool connected)
    => CircuitsChanged?.Invoke(this, connected);

    public CircuitHandlerService( )
    {
        Circuits = new ConcurrentDictionary<string, Circuit>();

    }

    public override async Task OnCircuitOpenedAsync(Circuit circuit, CancellationToken cancellationToken)
    {
        Circuits[circuit.Id] = circuit;
        OnCircuitsChanged(true);
        await base.OnCircuitOpenedAsync(circuit, cancellationToken);
    }

    public override async Task OnCircuitClosedAsync(Circuit circuit, CancellationToken cancellationToken)
    {
        
        Circuit circuitRemoved;
        Circuits.TryRemove(circuit.Id, out circuitRemoved);
        OnCircuitsChanged(false);
        await base.OnCircuitClosedAsync(circuit, cancellationToken);
    }

    public override async Task OnConnectionDownAsync(Circuit circuit, CancellationToken cancellationToken)
    {
        await base.OnConnectionDownAsync(circuit, cancellationToken);
    }

    public override Task OnConnectionUpAsync(Circuit circuit, CancellationToken cancellationToken)
    {
        return base.OnConnectionUpAsync(circuit, cancellationToken);
    }

}