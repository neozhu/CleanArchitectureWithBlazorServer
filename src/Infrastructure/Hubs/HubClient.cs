using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CleanArchitecture.Blazor.Infrastructure.Constants;
using Microsoft.AspNetCore.SignalR.Client;

namespace CleanArchitecture.Blazor.Infrastructure.Hubs;
public class HubClient : IAsyncDisposable
{
    private HubConnection _hubConnection;
    private readonly string _hubUrl;
    private readonly string _username;
    private bool _started = false;
    public HubClient(string siteUrl,string userName)
    {
        if (string.IsNullOrWhiteSpace(siteUrl))
            throw new ArgumentNullException(nameof(siteUrl));
        if (string.IsNullOrWhiteSpace(userName))
            throw new ArgumentNullException(nameof(userName));
        // set the hub URL
        _hubUrl = siteUrl.TrimEnd('/') + SignalR.HubUrl;
        // save username
        _username = userName;
    }
    public async Task StartAsync()
    {
        if (!_started)
        {
            // create the connection using the .NET SignalR client
            _hubConnection = new HubConnectionBuilder()
                .WithUrl(_hubUrl)
                .Build();
            // add handler for receiving messages
            _hubConnection.On<string>(SignalR.ConnectUser, (user) =>
            {
                LoggedIn?.Invoke(this, user);
            });
            _hubConnection.On<string>(SignalR.DisconnectUser, (user) =>
            {
                LoggedOut?.Invoke(this, user);
            });
            _hubConnection.On<string>(SignalR.SendNotification, (message) =>
            {
                NotificationReceived?.Invoke(this, message);
            });
            _hubConnection.On<string, string>(SignalR.SendMessage, (user, message) =>
            {
                HandleReceiveMessage(user, message);
            });
            // start the connection
            await _hubConnection.StartAsync();


            // register user on hub to let other clients know they've joined
            await _hubConnection.SendAsync(SignalR.ConnectUser, _username);
            _started = true;
        }
     
    }


    private void HandleReceiveMessage(string username, string message)
    {
        // raise an event to subscribers
        MessageReceived?.Invoke(this, new MessageReceivedEventArgs(username, message));
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
        await _hubConnection.SendAsync(SignalR.SendMessage, _username, message);
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

    public event EventHandler<string> LoggedIn;
    public event EventHandler<string> LoggedOut;
    public event EventHandler<string> NotificationReceived;
    public event MessageReceivedEventHandler MessageReceived;
    public delegate void MessageReceivedEventHandler(object sender, MessageReceivedEventArgs e);

    public class MessageReceivedEventArgs : EventArgs
    {
        public MessageReceivedEventArgs(string username, string message)
        {
            Username = username;
            Message = message;
        }
        public string Username { get; set; }
        public string Message { get; set; }

    }
}
