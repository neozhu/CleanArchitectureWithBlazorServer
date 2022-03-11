// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CleanArchitecture.Blazor.Infrastructure.Constants;

public static class SignalR
{
    public const string HubUrl = "/signalRHub";
    public const string SendUpdateDashboard = "UpdateDashboardAsync";
    public const string ReceiveUpdateDashboard = "UpdateDashboard";
    public const string ReceiveChatNotification = "ReceiveChatNotification";
    public const string SendNotification = "SendNotification";
    public const string ReceiveMessage = "ReceiveMessage";
    public const string SendMessage = "SendMessage";
    public const string OnConnect = "OnConnectAsync";
    public const string ConnectUser = "OnConnectUser";
    public const string OnDisconnect = "OnDisconnectAsync";
    public const string DisconnectUser = "OnDisconnectUser";
    public const string OnChangeRolePermissions = "OnChangeRolePermissions";
    public const string LogoutUsersByRole = "LogoutUsersByRole";
    public const string PingRequest = "PingRequestAsync";
    public const string PingResponse = "PingResponseAsync";
    public const string UpdateOnlineUsers = "UpdateOnlineUsers";
    public const string OCRTaskCompleted = "OCRTaskCompleted";

}
