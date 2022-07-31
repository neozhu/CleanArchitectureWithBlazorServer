using CleanArchitecture.Blazor.$safeprojectname$.Constants;
using CleanArchitecture.Blazor.$safeprojectname$.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;

namespace CleanArchitecture.Blazor.$safeprojectname$.Hubs;
public class HubClient : IAsyncDisposable
{
    private HubConnection _hubConnection;
    private string _hubUrl=String.Empty;
    private string _userId = String.Empty;
    private readonly NavigationManager _navigationManager;
    private readonly AuthenticationStateProvider _authenticationStateProvider;
    private bool _started = false;
    //public HubClient(string siteUrl,string userId)
    //{
    //    if (string.IsNullOrWhiteSpace(siteUrl))
    //        throw new ArgumentNullException(nameof(siteUrl));
    //    if (string.IsNullOrWhiteSpace(userId))
    //        throw new ArgumentNullException(nameof(userId));
    //    // set the hub URL
    //    _hubUrl = siteUrl.TrimEnd('/') + SignalR.HubUrl;
    //    // save username
    //    _userId = userId;
    //}
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
            _hubConnection.On<string>(SignalR.ConnectUser, (userId) =>
            {
                LoggedIn?.Invoke(this, userId);
            });
            _hubConnection.On<string>(SignalR.DisconnectUser, (userId) =>
            {
                LoggedOut?.Invoke(this, userId);
            });
            _hubConnection.On<string>(SignalR.SendNotification, (message) =>
            {
                NotificationReceived?.Invoke(this, message);
            });
            _hubConnection.On<string, string>(SignalR.SendMessage, (userId, message) =>
            {
                HandleReceiveMessage(userId, message);
            });
            // start the connection
            await _hubConnection.StartAsync();


            // register user on hub to let other clients know they've joined
            await _hubConnection.SendAsync(SignalR.ConnectUser, _userId);
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

        if (_started)
        {
            // disconnect the client
            await _hubConnection.StopAsync();
            // There is a bug in the mono/SignalR client that does not
            // close connections even after stop/dispose
            // see https://github.com/mono/mono/issues/18628
            // this means the demo won't show "xxx left the chat" since 
            // the connections are left open
            await _hubConnection.DisposeAsync();
            _hubConnection = null;
            _started = false;
        }
    }
    public async Task SendAsync(string message)
    {
        // check we are connected
        if (!_started)
            throw new InvalidOperationException("Client not started");
        // send the message
        await _hubConnection.SendAsync(SignalR.SendMessage, _userId, message);
    }
    public async Task NotifyAsync(string message)
    {
        // check we are connected
        if (!_started)
            throw new InvalidOperationException("Client not started");
        // send the message
        await _hubConnection.SendAsync(SignalR.SendNotification,  message);
    }
    public async ValueTask DisposeAsync()
    {
        await StopAsync();
    }

    public event EventHandler<string>? LoggedIn;
    public event EventHandler<string>? LoggedOut;
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
