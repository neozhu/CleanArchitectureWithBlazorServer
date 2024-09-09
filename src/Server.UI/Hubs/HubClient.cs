using System.Net;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.SignalR.Client;

namespace CleanArchitecture.Blazor.Server.UI.Hubs;

public sealed class HubClient : IAsyncDisposable
{
   

    private readonly HubConnection _hubConnection;
    private bool _started;

    public HubClient(NavigationManager navigationManager, IHttpContextAccessor httpContextAccessor)
    {
        var uri = new UriBuilder(navigationManager.Uri);
        var container = new CookieContainer();
        if (httpContextAccessor.HttpContext != null)
        {
            foreach (var c in httpContextAccessor.HttpContext.Request.Cookies)
            {
                container.Add(new Cookie(c.Key, c.Value)
                {
                    Domain = uri.Host,
                    Path = "/"
                });
            }
        }

        var hubUrl = navigationManager.BaseUri.TrimEnd('/') + ISignalRHub.Url;
        _hubConnection = new HubConnectionBuilder()
            .WithUrl(hubUrl, options =>
            {
                options.Transports = HttpTransportType.WebSockets;
                options.Cookies = container;
            }).WithAutomaticReconnect().Build();

        _hubConnection.ServerTimeout = TimeSpan.FromSeconds(30);

        // Observe and await the async result of the event invocation
        _hubConnection.On<string, string>(nameof(ISignalRHub.Connect), OnLoginEventAsync);

        _hubConnection.On<string, string>(nameof(ISignalRHub.Disconnect), OnLogoutEventAsync);

        _hubConnection.On<int, string>(nameof(ISignalRHub.Start), OnJobStartedEventAsync);

        _hubConnection.On<int, string>(nameof(ISignalRHub.Completed), OnJobCompletedEventAsync);

        _hubConnection.On<string>(nameof(ISignalRHub.SendNotification), OnNotificationReceivedEventAsync);

        _hubConnection.On<string, string>(nameof(ISignalRHub.SendMessage),OnMessageReceivedEventAsync);

        _hubConnection.On<string, string, string>(nameof(ISignalRHub.SendPrivateMessage),
            async (from, to, message) => await OnMessageReceivedEventAsync(from, message).ConfigureAwait(false));
    }

    // Handle the result of async event invocations
    private async Task OnLoginEventAsync(string connectionId, string userName)
    {
        if (LoginEvent != null)
        {
            await Task.Run(() => LoginEvent?.Invoke(this, new UserStateChangeEventArgs(connectionId, userName))).ConfigureAwait(false);
        }
    }

    private async Task OnLogoutEventAsync(string connectionId, string userName)
    {
        if (LogoutEvent != null)
        {
            await Task.Run(() => LogoutEvent?.Invoke(this, new UserStateChangeEventArgs(connectionId, userName))).ConfigureAwait(false);
        }
    }

    private async Task OnJobStartedEventAsync( int id, string message)
    {
        if (JobStartedEvent != null)
        {
            await Task.Run(() => JobStartedEvent?.Invoke(this, new JobStartedEventArgs(id, message))).ConfigureAwait(false);
        }
    }

    private async Task OnJobCompletedEventAsync( int id, string message)
    {
        if (JobCompletedEvent != null)
        {
            await Task.Run(() => JobCompletedEvent?.Invoke(this, new JobCompletedEventArgs(id, message))).ConfigureAwait(false);
        }
    }

    private async Task OnNotificationReceivedEventAsync( string message)
    {
        if (NotificationReceivedEvent != null)
        {
            await Task.Run(() => NotificationReceivedEvent?.Invoke(this, new NotificationReceivedEventArgs(message))).ConfigureAwait(false);
        }
    }

    private async Task OnMessageReceivedEventAsync( string from,string message)
    {
        if (MessageReceivedEvent != null)
        {
            await Task.Run(() => MessageReceivedEvent?.Invoke(this, new MessageReceivedEventArgs(from, message))).ConfigureAwait(false);
        }
    }

    public async ValueTask DisposeAsync()
    {
        try
        {
            await _hubConnection.StopAsync().ConfigureAwait(false);
        }
        finally
        {
            await _hubConnection.DisposeAsync().ConfigureAwait(false);
        }
    }

    // Event handlers
    public event EventHandler<UserStateChangeEventArgs>? LoginEvent;
    public event EventHandler<UserStateChangeEventArgs>? LogoutEvent;
    public event EventHandler<JobStartedEventArgs>? JobStartedEvent;
    public event EventHandler<JobCompletedEventArgs>? JobCompletedEvent;
    public event EventHandler<NotificationReceivedEventArgs>? NotificationReceivedEvent;
    public event EventHandler<MessageReceivedEventArgs>? MessageReceivedEvent;

    public async Task StartAsync(CancellationToken cancellation = default)
    {
        if (_started) return;
        _started = true;
        await _hubConnection.StartAsync(cancellation).ConfigureAwait(false);
    }

    public async Task SendAsync(string message)
    {
        await _hubConnection.SendAsync(nameof(ISignalRHub.SendMessage), message).ConfigureAwait(false);
    }

    public async Task NotifyAsync(string message)
    {
        await _hubConnection.SendAsync(nameof(ISignalRHub.SendNotification), message).ConfigureAwait(false);
    }
}

public class MessageReceivedEventArgs : EventArgs
{
    public MessageReceivedEventArgs(string userName, string message)
    {
        UserId = userName;
        Message = message;
    }

    public string UserId { get; set; }
    public string Message { get; set; }
}

public class UserStateChangeEventArgs : EventArgs
{
    public UserStateChangeEventArgs(string connectionId, string userName)
    {
        ConnectionId = connectionId;
        UserName = userName;
    }

    public string ConnectionId { get; set; }
    public string UserName { get; set; }
}

public class JobStartedEventArgs : EventArgs
{
    public JobStartedEventArgs(int id,string message)
    {
        Message = message;
        Id=id;
    }
    public string Message { get; }
    public int Id { get; }
}
public class JobCompletedEventArgs : EventArgs
{
    public JobCompletedEventArgs(int id,string message)
    {
        Message = message;
        Id = id;
    }
    public string Message { get; }
    public int Id { get; }
}
public class NotificationReceivedEventArgs : EventArgs
{
    public NotificationReceivedEventArgs(string message)
    {
        Message = message;
    }
    public string Message { get; set; }
}