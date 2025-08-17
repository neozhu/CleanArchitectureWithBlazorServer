﻿using CleanArchitecture.Blazor.Application.Common.Interfaces.Identity;

namespace CleanArchitecture.Blazor.Server.UI.Hubs;

public interface ISignalRHub
{
    public const string Url = "/signalRHub";

    Task Connect(string connectionId, string userName);
    Task Disconnect(string connectionId, string userName);

    Task Start(int id, string message);
    Task Completed(int id, string message);

    Task SendMessage(string from, string message);
    Task SendPrivateMessage(string from, string to, string message);
    Task SendNotification(string message);

    // Active page-component session signaling
    Task PageComponentOpened(string pageComponent, string userId, string userName);
    Task PageComponentClosed(string pageComponent, string userId, string userName);

    // Snapshot method: fetch current online users with profile data
    // Note: invoked via HubConnection.InvokeAsync from clients
    Task<List<UserContext>> GetOnlineUsers();
}