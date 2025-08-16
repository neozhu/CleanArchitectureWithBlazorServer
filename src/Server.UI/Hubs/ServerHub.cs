// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Concurrent;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace CleanArchitecture.Blazor.Server.UI.Hubs;

[Authorize(AuthenticationSchemes = "Identity.Application")]
public class ServerHub : Hub<ISignalRHub>
{
    private static readonly ConcurrentDictionary<string, string> OnlineUsers = new(StringComparer.Ordinal);
    private static readonly ConcurrentDictionary<string, ConcurrentDictionary<string, string>> ComponentUsers = new(StringComparer.Ordinal);
    public override async Task OnConnectedAsync()
    {
        var connectionId = Context.ConnectionId;
        var username =Context.User?.Identity?.Name ?? string.Empty;
        // Notify all clients if this is a new user connecting.
        if (!OnlineUsers.Any(x => x.Value.Equals(username)))
        {
            await Clients.All.Connect(connectionId, username).ConfigureAwait(false);
        }
        if (!OnlineUsers.ContainsKey(connectionId))
        {
            OnlineUsers.TryAdd(connectionId, username);
        }
        await base.OnConnectedAsync().ConfigureAwait(false); 
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var connectionId = Context.ConnectionId;
        // Remove the connection and check if it was the last one for this user.
        if (OnlineUsers.TryRemove(connectionId, out var username))
        {
            if (!OnlineUsers.Any(x => x.Value.Equals(username)))
            {
                await Clients.All.Disconnect(connectionId, username).ConfigureAwait(false);
            }    
        }
        await base.OnConnectedAsync().ConfigureAwait(false);
    }

    public async Task SendMessage(string message)
    {
        var username = Context.User?.Identity?.Name ?? string.Empty;
        await Clients.All.SendMessage(username, message).ConfigureAwait(false);
    }

    public async Task SendPrivateMessage(string to, string message)
    {
        var username = Context.User?.Identity?.Name ?? string.Empty;
        await Clients.User(to).SendPrivateMessage(username, to, message).ConfigureAwait(false);
    }

    public async Task SendNotification(string message)
    {
        await Clients.All.SendNotification(message).ConfigureAwait(false);
    }

    public async Task Completed(int id,string message)
    {
        await Clients.All.Completed(id,message).ConfigureAwait(false);
    }

    // Client -> Server: notify open/close of a specific page component
    public async Task NotifyPageComponentOpen(string pageComponent)
    {
        var username = Context.User?.Identity?.Name ?? string.Empty;
        var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
                     ?? Context.User?.FindFirst("sub")?.Value
                     ?? username;
        var users = ComponentUsers.GetOrAdd(pageComponent, _ => new ConcurrentDictionary<string, string>(StringComparer.Ordinal));
        // Send existing users of this component to the caller first
        foreach (var kvp in users)
        {
            await Clients.Caller.PageComponentOpened(pageComponent, kvp.Key, kvp.Value).ConfigureAwait(false);
        }
        users[userId] = username;
        // Notify all clients that this user opened the component
        await Clients.All.PageComponentOpened(pageComponent, userId, username).ConfigureAwait(false);
    }

    public async Task NotifyPageComponentClose(string pageComponent)
    {
        var username = Context.User?.Identity?.Name ?? string.Empty;
        var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
                     ?? Context.User?.FindFirst("sub")?.Value
                     ?? username;
        if (ComponentUsers.TryGetValue(pageComponent, out var users))
        {
            users.TryRemove(userId, out _);
            if (users.IsEmpty)
            {
                ComponentUsers.TryRemove(pageComponent, out _);
            }
        }
        await Clients.All.PageComponentClosed(pageComponent, userId, username).ConfigureAwait(false);
    }
}