using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using System.Net.Http;
using System.Net.Http.Json;
using CleanArchitecture.Blazor.Infrastructure.Configurations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.Web.Virtualization;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Blazored.LocalStorage;
using Microsoft.JSInterop;
using Blazor.Server.UI;
using Blazor.Server.UI.Shared;
using MudBlazor;
using MediatR;
using Blazor.Server.UI.Components.Shared;
using Blazor.Server.UI.Components.Shared.Themes;
using Blazor.Server.UI.Components.Index;
using Blazor.Server.UI.Components.Charts;
using Blazor.Server.UI.Models.Localization;
using FluentValidation;
using CleanArchitecture.Blazor.Infrastructure.Identity;
using CleanArchitecture.Blazor.Application.Constants.Permission;
using CleanArchitecture.Blazor.Application.Common.Interfaces;
using CleanArchitecture.Blazor.Application.Common.Models;
using CleanArchitecture.Blazor.Domain.Enums;
using CleanArchitecture.Blazor.Infrastructure.Extensions;
using Blazor.Server.UI.Components.Common;
using Blazor.Server.UI.Components.Dialogs;
using CleanArchitecture.Blazor.Infrastructure.Hubs;
using CleanArchitecture.Blazor.Application.Common.Interfaces.Identity;

namespace Blazor.Server.UI.Shared;

public partial class UserLoginState : IAsyncDisposable
{
    [CascadingParameter]
    protected Task<AuthenticationState> _authState { get; set; } = default!;
    [Inject]
    private NavigationManager _navigationManager { get; set; } = default!;
    [Inject]
    private AuthenticationStateProvider _authenticationStateProvider { get; set; } = default!;

    private IIdentityService _identityService { get; set; }=default!;
    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
    }

    public async ValueTask DisposeAsync()
    {
        var state = await _authState;
        if (state.User.Identity is not null)
        {
            await _identityService.UpdateLiveStatus(state.User.GetUserId(), false);
        }
        if (_client is not null)
        {
            await _client.StopAsync();
            _client.LoggedOut -= _client_LoggedOut;
            _client.LoggedIn -= _client_LoggedIn;
        }
        _authenticationStateProvider.AuthenticationStateChanged -= _authenticationStateProvider_AuthenticationStateChanged;
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private HubClient? _client = null;
    protected override async Task OnInitializedAsync()
    {
        _identityService = ScopedServices.GetRequiredService<IIdentityService>();
        var state = await _authState;
        if (state.User.Identity != null && state.User.Identity.IsAuthenticated)
        {
          
            _client = new HubClient(_navigationManager.BaseUri, state.User.GetUserId());
            _client.LoggedOut += _client_LoggedOut;
            _client.LoggedIn += _client_LoggedIn;
            await _client.StartAsync();
            await _identityService.UpdateLiveStatus(state.User.GetUserId(), true);
        }
        _authenticationStateProvider.AuthenticationStateChanged += _authenticationStateProvider_AuthenticationStateChanged;
    }
    private void _authenticationStateProvider_AuthenticationStateChanged(Task<AuthenticationState> authenticationState)
    {
        InvokeAsync(async () =>
        {
            var state = await authenticationState;
            if (state.User.Identity != null && state.User.Identity.IsAuthenticated)
            {
                if(_client is not null)
                {
                    await _client.StopAsync();
                    _client.LoggedOut -= _client_LoggedOut;
                    _client.LoggedIn -= _client_LoggedIn;
                }
                _client = new HubClient(_navigationManager.BaseUri, state.User.GetUserId());
                _client.LoggedOut += _client_LoggedOut;
                _client.LoggedIn += _client_LoggedIn;
                await _client.StartAsync();
                await _identityService.UpdateLiveStatus(state.User.GetUserId(), true);

            }
        });
    }
    private void _client_LoggedIn(object? sender, string e)
    {
        InvokeAsync(async () =>
        {
            var username = await _identityService.GetUserNameAsync(e);
            Snackbar.Add($"{username} login.", MudBlazor.Severity.Info);
            StateHasChanged();
        });
    }

    private void _client_LoggedOut(object? sender, string e)
    {
        InvokeAsync(async () =>
        {
            var username = await _identityService.GetUserNameAsync(e);
            Snackbar.Add($"{username} logout.", MudBlazor.Severity.Normal);
            StateHasChanged();
        });
    }
}
