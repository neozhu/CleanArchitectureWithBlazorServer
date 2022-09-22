// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Concurrent;
using CleanArchitecture.Blazor.Infrastructure.Constants;
using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.AspNetCore.SignalR;


namespace CleanArchitecture.Blazor.Infrastructure.Hubs;

public interface ISignalRHub
{
    Task Start(string message);
    Task Completed(string message);
    Task SendMessage(string message);
    Task Disconnect(string userId);
    Task Connect(string userId);
    Task SendNotification(string message);
}

public class SignalRHub : Hub<ISignalRHub>
{
    private static readonly ConcurrentDictionary<string, string> _onlineUsers = new();

    public SignalRHub()
    {

    }
    public override async Task OnConnectedAsync()
    {
        var id = Context.ConnectionId;
        if (!_onlineUsers.ContainsKey(id))
        {
            _onlineUsers.TryAdd(id, String.Empty);
        }
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var id = Context.ConnectionId;
        //try to remove key from dictionary
        if (_onlineUsers.TryRemove(id, out string? userId))
        {
          await Disconnect(userId);
        }
        await base.OnDisconnectedAsync(exception);
    }


    public async Task SendMessage(string message)
    {
        await Clients.All.SendMessage(message);
    }
    public async Task SendNotification(string message)
    {
        await Clients.All.SendNotification(message);
    }
    public async Task Completed(string message)
    {
        await Clients.All.Completed(message);
    }
    public async Task Connect(string userId)
    {
        var id = Context.ConnectionId;
        if (!_onlineUsers.ContainsKey(id))
        {
            // maintain a lookup of connectionId-to-username
            if (_onlineUsers.TryAdd(id, userId))
            {
                // re-use existing message for now
                await Clients.All.Connect(userId);
            }
        }
        else
        {
            _onlineUsers.TryUpdate(id, userId, String.Empty);
            await Clients.All.Connect(userId);
        }
      
    }
    public async Task Disconnect(string userId)
    {
        await Clients.All.Disconnect(userId);
    }
}
