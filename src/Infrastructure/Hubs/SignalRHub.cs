// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Concurrent;
using CleanArchitecture.Blazor.Infrastructure.Constants;
using Microsoft.AspNetCore.SignalR;


namespace CleanArchitecture.Blazor.Infrastructure.Hubs;

public class SignalRHub : Hub
{
    private static readonly ConcurrentDictionary<string, string> _onlineUsers = new();

    public SignalRHub()
    {

    }
    public override async Task OnConnectedAsync()
    {

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        string id = Context.ConnectionId;
        //try to remove key from dictionary
        if (_onlineUsers.TryRemove(id, out string? userId))
        {
          await Clients.All.SendAsync(SignalR.DisconnectUser, userId);
        }
        await base.OnDisconnectedAsync(exception);
    }

    public async Task OnConnectUser(string userId)
    {
        var id = Context.ConnectionId;
        if (!_onlineUsers.ContainsKey(id))
        {
            // maintain a lookup of connectionId-to-username
            if (_onlineUsers.TryAdd(id, userId))
            {
                // re-use existing message for now
                await Clients.All.SendAsync(SignalR.ConnectUser, userId);
            }
        }
    }
    public async Task SendMessage(string userId, string message)
    {
        await Clients.All.SendAsync(SignalR.SendMessage, userId, message);
    }
    public async Task SendNotification(string message)
    {
        await Clients.All.SendAsync(SignalR.SendNotification, message);
    }
}
