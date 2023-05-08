// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CleanArchitecture.Blazor.Infrastructure.Hubs;

public static class SignalR
{
    public const string HubUrl = "/signalRHub";
    public const string SendUpdateDashboard = "UpdateDashboard";
    public const string SendNotification = "SendNotification";
    public const string SendMessage = "SendMessage";
    public const string SendPrivateMessage = "SendPrivateMessage";
    public const string OnConnect = "Connect";
    public const string OnDisconnect = "Disconnect";
    public const string OnChangeRolePermissions = "OnChangeRolePermissions";
    public const string PingRequest = "PingRequestAsync";
    public const string PingResponse = "PingResponseAsync";
    public const string UpdateOnlineUsers = "UpdateOnlineUsers";
    public const string JobCompleted = "Completed";
    public const string JobStart = "Start";

}
