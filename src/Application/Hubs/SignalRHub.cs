// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Concurrent;
using CleanArchitecture.Blazor.Application.Common.Interfaces.Identity;
using CleanArchitecture.Blazor.Application.Hubs.Constants;
using Microsoft.AspNetCore.SignalR;


namespace CleanArchitecture.Blazor.Application.Hubs;

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

    public override async Task OnDisconnectedAsync(Exception exception)
    {
        string id = Context.ConnectionId;
        //try to remove key from dictionary
        if (_onlineUsers.TryRemove(id, out string userName))
        {
            await Clients.All.SendAsync(SignalR.DisconnectUser, userName );
        }

        await base.OnDisconnectedAsync(exception);
    }

    public async Task ConnectUser(string userName)
    {
        var id = Context.ConnectionId;
        if (!_onlineUsers.ContainsKey(id))
        {
            // maintain a lookup of connectionId-to-username
            if (_onlineUsers.TryAdd(id, userName))
            {
                // re-use existing message for now
                await Clients.All.SendAsync(SignalR.ConnectUser,  userName );
            }
        }
       
    }
}
