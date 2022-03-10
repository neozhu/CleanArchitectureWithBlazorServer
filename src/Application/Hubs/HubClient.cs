using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CleanArchitecture.Blazor.Application.Hubs.Constants;
using Microsoft.AspNetCore.SignalR.Client;

namespace CleanArchitecture.Blazor.Application.Hubs;
public class HubClient : IAsyncDisposable
{
    private HubConnection _hubConnection;
    private readonly string _hubUrl;
    public HubClient(string siteUrl)
    {
        if (string.IsNullOrWhiteSpace(siteUrl))
            throw new ArgumentNullException(nameof(siteUrl));
        // save username
        // set the hub URL
        _hubUrl = siteUrl.TrimEnd('/') + SignalR.HubUrl;
    }
    public async Task StartAsync(string userName)
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

            // start the connection
            await _hubConnection.StartAsync();


            // register user on hub to let other clients know they've joined
            await _hubConnection.SendAsync(SignalR.ConnectUser, userName);
     
    }
    public async Task StopAsync()
    {

        if (_hubConnection != null)
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

        }
    }

    public async ValueTask DisposeAsync()
    {
        await StopAsync();
    }

    public event EventHandler<string> LoggedIn;
    public event EventHandler<string> LoggedOut;
}
