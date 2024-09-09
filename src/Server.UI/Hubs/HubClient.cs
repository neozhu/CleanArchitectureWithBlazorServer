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
        _hubConnection.On<string, string>(nameof(ISignalRHub.Connect),
            async (connectionId, userName) => await OnLoginEventAsync(this, new UserStateChangeEventArgs(connectionId, userName)).ConfigureAwait(false));

        _hubConnection.On<string, string>(nameof(ISignalRHub.Disconnect),
            async (connectionId, userName) => await OnLogoutEventAsync(this, new UserStateChangeEventArgs(connectionId, userName)).ConfigureAwait(false));

        _hubConnection.On<string>(nameof(ISignalRHub.Start),
            async message => await OnJobStartedEventAsync(this, message).ConfigureAwait(false));

        _hubConnection.On<string>(nameof(ISignalRHub.Completed),
            async message => await OnJobCompletedEventAsync(this, message).ConfigureAwait(false));

        _hubConnection.On<string>(nameof(ISignalRHub.SendNotification),
            async message => await OnNotificationReceivedEventAsync(this, message).ConfigureAwait(false));

        _hubConnection.On<string, string>(nameof(ISignalRHub.SendMessage),
            async (from, message) => await OnMessageReceivedEventAsync(this, new MessageReceivedEventArgs(from, message)).ConfigureAwait(false));

        _hubConnection.On<string, string, string>(nameof(ISignalRHub.SendPrivateMessage),
            async (from, to, message) => await OnMessageReceivedEventAsync(this, new MessageReceivedEventArgs(from, message)).ConfigureAwait(false));
    }

    // Handle the result of async event invocations
    private async Task OnLoginEventAsync(object sender, UserStateChangeEventArgs e)
    {
        if (LoginEvent != null)
        {
            await Task.Run(() => LoginEvent?.Invoke(sender, e)).ConfigureAwait(false);
        }
    }

    private async Task OnLogoutEventAsync(object sender, UserStateChangeEventArgs e)
    {
        if (LogoutEvent != null)
        {
            await Task.Run(() => LogoutEvent?.Invoke(sender, e)).ConfigureAwait(false);
        }
    }

    private async Task OnJobStartedEventAsync(object sender, string e)
    {
        if (JobStartedEvent != null)
        {
            await Task.Run(() => JobStartedEvent?.Invoke(sender, new JobStartedEventArgs(e))).ConfigureAwait(false);
        }
    }

    private async Task OnJobCompletedEventAsync(object sender, string e)
    {
        if (JobCompletedEvent != null)
        {
            await Task.Run(() => JobCompletedEvent?.Invoke(sender, new JobCompletedEventArgs(e))).ConfigureAwait(false);
        }
    }

    private async Task OnNotificationReceivedEventAsync(object sender, string e)
    {
        if (NotificationReceivedEvent != null)
        {
            await Task.Run(() => NotificationReceivedEvent?.Invoke(sender, new NotificationReceivedEventArgs(e))).ConfigureAwait(false);
        }
    }

    private async Task OnMessageReceivedEventAsync(object sender, MessageReceivedEventArgs e)
    {
        if (MessageReceivedEvent != null)
        {
            await Task.Run(() => MessageReceivedEvent?.Invoke(sender, new MessageReceivedEventArgs(e.UserId, e.Message))).ConfigureAwait(false);
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
    public JobStartedEventArgs(string message)
    {
        Message = message;
    }
    public string Message { get; set; }
}
public class JobCompletedEventArgs : EventArgs
{
    public JobCompletedEventArgs(string message)
    {
        Message = message;
    }
    public string Message { get; set; }
}
public class NotificationReceivedEventArgs : EventArgs
{
    public NotificationReceivedEventArgs(string message)
    {
        Message = message;
    }
    public string Message { get; set; }
}