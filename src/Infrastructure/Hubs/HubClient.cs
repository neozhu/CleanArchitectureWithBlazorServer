using CleanArchitecture.Blazor.Infrastructure.Constants;
using CleanArchitecture.Blazor.Infrastructure.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;

namespace CleanArchitecture.Blazor.Infrastructure.Hubs;
public class HubClient : IAsyncDisposable
{
    private HubConnection? _hubConnection;
    private string _hubUrl=String.Empty;
    private string _userId = String.Empty;
    private readonly NavigationManager _navigationManager;
    private readonly AuthenticationStateProvider _authenticationStateProvider;
    private bool _started = false;

    public HubClient(NavigationManager navigationManager,
        AuthenticationStateProvider authenticationStateProvider)
    {
        this._navigationManager =navigationManager;
        _authenticationStateProvider = authenticationStateProvider;
    }
    public async Task StartAsync()
    {
        _hubUrl = _navigationManager.BaseUri.TrimEnd('/') + SignalR.HubUrl;
        var state = await _authenticationStateProvider.GetAuthenticationStateAsync();
        _userId = state.User.GetUserId();
        if (!_started)
        {
            // create the connection using the .NET SignalR client
            _hubConnection = new HubConnectionBuilder()
                .WithUrl(_hubUrl)
                .Build();
            // add handler for receiving messages
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
            _hubConnection.On<string, string>(SignalR.SendMessage, (userId, message) =>
            {
                HandleReceiveMessage(userId, message);
            });
            _hubConnection.On<string>(SignalR.JobCompleted, (message) =>
            {
                JobCompleted?.Invoke(this,message);
            });
            _hubConnection.On<string>(SignalR.JobStart, (message) =>
            {
                JobStarted?.Invoke(this, message);
            });
            // start the connection
            await _hubConnection.StartAsync();


            // register user on hub to let other clients know they've joined
            await _hubConnection.SendAsync(SignalR.OnConnect, _userId);
            _started = true;
        }
     
    }


    private void HandleReceiveMessage(string userId, string message)
    {
        // raise an event to subscribers
        MessageReceived?.Invoke(this, new MessageReceivedEventArgs(userId, message));
    }
    public async Task StopAsync()
    {
        try
        {
            if (_started && _hubConnection is not null)
            {
                // disconnect the client
                await _hubConnection.StopAsync();
                await _hubConnection.DisposeAsync();
            }
        }
        finally
        {
            // There is a bug in the mono/SignalR client that does not
            // close connections even after stop/dispose
            // see https://github.com/mono/mono/issues/18628
            // this means the demo won't show "xxx left the chat" since 
            // the connections are left open
          
            _hubConnection = null;
            _started = false;
        }
    }
    public async Task SendAsync(string message)
    {
        await _hubConnection!.SendAsync(SignalR.SendMessage, _userId, message);
    }
    public async Task NotifyAsync(string message)
    {
        await _hubConnection!.SendAsync(SignalR.SendNotification,  message);
    }
    public async ValueTask DisposeAsync()
    {
        try
        {
            if (_hubConnection is not null)
            {
                await _hubConnection.StopAsync();
                await _hubConnection.DisposeAsync();
            }
        }
        finally
        {
            // There is a bug in the mono/SignalR client that does not
            // close connections even after stop/dispose
            // see https://github.com/mono/mono/issues/18628
            // this means the demo won't show "xxx left the chat" since 
            // the connections are left open

            _hubConnection = null;
            _started = false;
        }
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
