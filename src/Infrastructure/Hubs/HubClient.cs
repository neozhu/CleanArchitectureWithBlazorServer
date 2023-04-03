using CleanArchitecture.Blazor.Infrastructure.Constants;
using CleanArchitecture.Blazor.Infrastructure.Extensions;
using CleanArchitecture.Blazor.Infrastructure.Services.JWT;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;

namespace CleanArchitecture.Blazor.Infrastructure.Hubs;
public class HubClient : IAsyncDisposable
{
    private HubConnection _hubConnection;
    private string _hubUrl = String.Empty;
    private string? _userName=null;
    private readonly NavigationManager _navigationManager;
    private readonly TokenAuthProvider _authProvider;
    private readonly ICurrentUserService _currentUserService;
    private bool _started = false;
    private bool _isDisposed;
    public HubClient(NavigationManager navigationManager,
        TokenAuthProvider  authProvider,
        ICurrentUserService currentUserService
)
    {
        
        _navigationManager = navigationManager;
        _authProvider = authProvider;
        _currentUserService = currentUserService;
        _userName = currentUserService.UserName;
        var token = _authProvider.AccessToken;
        _hubUrl = _navigationManager.BaseUri.TrimEnd('/') + SignalR.HubUrl;
        _hubConnection = new HubConnectionBuilder()
              .WithUrl(_hubUrl, options => {
                  options.Headers.Add("Authorization", $"Bearer {token}");
                  options.AccessTokenProvider =()=> Task.FromResult(token);
                  options.Transports = HttpTransportType.WebSockets;
                  })
              .Build();
        _hubConnection.ServerTimeout = TimeSpan.FromSeconds(30);
        _hubConnection.On<string>(SignalR.OnConnect, (userId) =>
        {
            Login?.Invoke(this, userId);
        });

        _hubConnection.On<string>(SignalR.OnDisconnect, (userId) =>
        {
            Logout?.Invoke(this, userId);
        });
        _hubConnection.On<string>(SignalR.SendNotification, (message) =>
        {
            NotificationReceived?.Invoke(this, message);
        });
        _hubConnection.On<string, string>(SignalR.SendMessage, (from,message) =>
        {
            HandleReceiveMessage(from,message);
        });
        _hubConnection.On<string>(SignalR.JobCompleted, (message) =>
        {
            JobCompleted?.Invoke(this, message);
        });
        _hubConnection.On<string>(SignalR.JobStart, (message) =>
        {
            JobStarted?.Invoke(this, message);
        });
    }
    public async Task StartAsync(CancellationToken cancellation = default)
    {
        if (_hubConnection.State != HubConnectionState.Disconnected)
            throw new ApplicationException("Connection is in progress or has already been established");
        if (_started) return;
        _started = true;
        await _hubConnection.StartAsync(cancellation);
    }


    private void HandleReceiveMessage(string from, string message)
    {
        // raise an event to subscribers
        MessageReceived?.Invoke(this, new MessageReceivedEventArgs(from, message));
    }
    public async Task StopAsync()
    {

        if (_started && _hubConnection is not null)
        {
            // disconnect the client
            await _hubConnection.StopAsync();
            // There is a bug in the mono/SignalR client that does not
            // close connections even after stop/dispose
            // see https://github.com/mono/mono/issues/18628
            // this means the demo won't show "xxx left the chat" since 
            // the connections are left open
            await _hubConnection.DisposeAsync();
            _started = false;
        }
    }
    public async Task SendAsync(string message)
    {
        await _hubConnection.SendAsync(SignalR.SendMessage, _userName, message);
    }
    public async Task NotifyAsync(string message)
    {
        await _hubConnection.SendAsync(SignalR.SendNotification, message);
    }
    public async ValueTask DisposeAsync()
    {
        if (_isDisposed) return;
        _isDisposed = true;
        await StopAsync();
    }



    public event EventHandler<string>? Login;
    public event EventHandler<string>? JobStarted;
    public event EventHandler<string>? JobCompleted;
    public event EventHandler<string>? Logout;
    public event EventHandler<string>? NotificationReceived;
    public event MessageReceivedEventHandler? MessageReceived;
    public delegate Task MessageReceivedEventHandler(object sender, MessageReceivedEventArgs e);

    public class MessageReceivedEventArgs : EventArgs
    {
        public MessageReceivedEventArgs(string userId, string message)
        {
            UserId = userId;
            Message = message;
        }
        public string UserId { get; set; }
        public string Message { get; set; }
    }
}
