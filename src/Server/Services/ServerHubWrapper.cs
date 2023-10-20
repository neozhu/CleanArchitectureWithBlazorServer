using CleanArchitecture.Blazor.Server.Common.Interfaces;
using CleanArchitecture.Blazor.Server.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace CleanArchitecture.Blazor.Server.Services;
public class ServerHubWrapper : IApplicationHubWrapper
{
    private readonly IHubContext<ServerHub, ISignalRHub> _hubContext;

    public ServerHubWrapper(IHubContext<ServerHub, ISignalRHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task JobStarted(string message)
    {
        await _hubContext.Clients.All.Start(message);
    }

    public async Task JobCompleted(string message)
    {
        await _hubContext.Clients.All.Completed(message);
    }
}
