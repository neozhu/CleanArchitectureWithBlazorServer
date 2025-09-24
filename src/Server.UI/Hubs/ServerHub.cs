// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Concurrent;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using CleanArchitecture.Blazor.Application.Common.Interfaces.Identity;
using CleanArchitecture.Blazor.Domain.Identity;
using Microsoft.AspNetCore.Identity;

namespace CleanArchitecture.Blazor.Server.UI.Hubs;

[Authorize(AuthenticationSchemes = "Identity.Application")]
public class ServerHub : Hub<ISignalRHub>
{
    private sealed record ConnectionUser(string UserId, string UserName);
    private static readonly ConcurrentDictionary<string, ConnectionUser> OnlineUsers = new(StringComparer.Ordinal);
    private static readonly ConcurrentDictionary<string, ConcurrentDictionary<string, string>> ComponentUsers = new(StringComparer.Ordinal);
    private readonly IServiceScopeFactory _scopeFactory;
    public ServerHub(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }
    public override async Task OnConnectedAsync()
    {
        var connectionId = Context.ConnectionId;
        var username = Context.User?.Identity?.Name ?? string.Empty;
        var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
                     ?? Context.User?.FindFirst("sub")?.Value
                     ?? username;
        // Notify all clients if this is a new user connecting.
        if (!OnlineUsers.Any(x => string.Equals(x.Value.UserId, userId, StringComparison.Ordinal)))
        {
            await Clients.All.Connect(connectionId, username).ConfigureAwait(false);
        }
        if (!OnlineUsers.ContainsKey(connectionId))
        {
            OnlineUsers.TryAdd(connectionId, new ConnectionUser(userId, username));
        }
        await base.OnConnectedAsync().ConfigureAwait(false); 
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var connectionId = Context.ConnectionId;
        // Remove the connection and check if it was the last one for this user.
        if (OnlineUsers.TryRemove(connectionId, out var connectionUser))
        {
            if (!OnlineUsers.Any(x => string.Equals(x.Value.UserId, connectionUser.UserId, StringComparison.Ordinal)))
            {
                await Clients.All.Disconnect(connectionId, connectionUser.UserName).ConfigureAwait(false);
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

    // Client -> Server: returns a snapshot of distinct online users with profile data
    public async Task<List<UserContext>> GetOnlineUsers()
    {
        var distinctUsers = OnlineUsers.Values
            .GroupBy(v => v.UserId, StringComparer.Ordinal)
            .Select(g => g.First())
            .ToList();

        using var scope = _scopeFactory.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        var result = new List<UserContext>(distinctUsers.Count);
        foreach (var cu in distinctUsers.OrderBy(u => u.UserName, StringComparer.Ordinal))
        {
            var appUser = await userManager.FindByIdAsync(cu.UserId).ConfigureAwait(false);
            result.Add(new UserContext(
                UserId: cu.UserId,
                UserName: cu.UserName,
                DisplayName: appUser?.DisplayName,
                TenantId: appUser?.TenantId,
                Email: appUser?.Email,
                Roles: null,
                ProfilePictureDataUrl: appUser?.ProfilePictureDataUrl,
                SuperiorId: appUser?.SuperiorId
            ));
        }
        return result;
    }
}
