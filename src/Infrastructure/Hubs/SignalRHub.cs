// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Concurrent;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace CleanArchitecture.Blazor.Infrastructure.Hubs;

public interface ISignalRHub
{
    Task Start(string message);
    Task Completed(string message);
    Task SendMessage(string from, string message);
    Task SendPrivateMessage(string from, string to, string message);
    Task Disconnect(string userId);
    Task Connect(string userId);
    Task SendNotification(string message);
}

[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class SignalRHub : Hub<ISignalRHub>
{
    private static readonly ConcurrentDictionary<string, string> OnlineUsers = new();

    public override async Task OnConnectedAsync()
    {
        var id = Context.ConnectionId;
        var username = Context.User?.Identity?.Name ?? string.Empty;
        if (!OnlineUsers.ContainsKey(id)) OnlineUsers.TryAdd(id, username);

        await Clients.All.Connect(username);
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var id = Context.ConnectionId;
        //try to remove key from dictionary
        if (OnlineUsers.TryRemove(id, out var username)) await Clients.All.Disconnect(username);

        await base.OnConnectedAsync();
    }

    public async Task SendMessage(string message)
    {
        var username = Context.User?.Identity?.Name ?? string.Empty;
        await Clients.All.SendMessage(username, message);
    }

    public async Task SendPrivateMessage(string to, string message)
    {
        var username = Context.User?.Identity?.Name ?? string.Empty;
        await Clients.User(to).SendPrivateMessage(username, to, message);
    }

    public async Task SendNotification(string message)
    {
        await Clients.All.SendNotification(message);
    }

    public async Task Completed(string message)
    {
        await Clients.All.Completed(message);
    }
}