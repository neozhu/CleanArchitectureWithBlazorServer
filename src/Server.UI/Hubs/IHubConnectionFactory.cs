using System.Net;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.SignalR.Client;

namespace CleanArchitecture.Blazor.Server.UI.Hubs;

public interface IHubConnectionFactory
{
    HubConnection CreateForCurrentUser(string relativeHubUrl);
}
public class HubConnectionFactory : IHubConnectionFactory
{
    private readonly NavigationManager _navigationManager;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public HubConnectionFactory(
        NavigationManager navigationManager,
        IHttpContextAccessor httpContextAccessor)
    {
        _navigationManager = navigationManager;
        _httpContextAccessor = httpContextAccessor;
    }

    public HubConnection CreateForCurrentUser(string relativeHubUrl)
    {
        var uri = new UriBuilder(_navigationManager.Uri);
        var container = new CookieContainer();

        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext != null &&
            httpContext.Request.Cookies.TryGetValue(".AspNetCore.Identity.Application", out var authCookie))
        {
            container.Add(new Cookie(".AspNetCore.Identity.Application", authCookie)
            {
                Domain = uri.Host,
                Path = "/"
            });
        }

        var hubUrl = _navigationManager.BaseUri.TrimEnd('/') + relativeHubUrl;

        return new HubConnectionBuilder()
            .WithUrl(hubUrl, options =>
            {
                options.Transports = HttpTransportType.WebSockets;
                options.Cookies = container;
            })
            .WithAutomaticReconnect()
            .Build();
    }
}