// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CleanArchitecture.Razor.Application.Hubs.Constants;

public static class SignalR
{
    public const string HubUrl = "/signalRHub";
    public const string SendUpdateDashboard = "UpdateDashboardAsync";
    public const string ReceiveUpdateDashboard = "UpdateDashboard";
    public const string ReceiveChatNotification = "ReceiveChatNotification";
    public const string SendChatNotification = "ChatNotificationAsync";
    public const string ReceiveMessage = "ReceiveMessage";
    public const string SendMessage = "SendMessageAsync";
    public const string OnConnect = "OnConnectAsync";
    public const string ConnectUser = "ConnectUser";
    public const string OnDisconnect = "OnDisconnectAsync";
    public const string DisconnectUser = "DisconnectUser";
    public const string OnChangeRolePermissions = "OnChangeRolePermissions";
    public const string LogoutUsersByRole = "LogoutUsersByRole";
    public const string PingRequest = "PingRequestAsync";
    public const string PingResponse = "PingResponseAsync";

    public const string OCRTaskCompleted = "OCRTaskCompleted";

}
