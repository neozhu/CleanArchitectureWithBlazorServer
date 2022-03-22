using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Blazor.Server.UI.Components.Shared;
using Blazor.Server.UI.Models;
using Toolbelt.Blazor.HotKeys;
using Microsoft.AspNetCore.Components.Authorization;
using CleanArchitecture.Blazor.Application.Common.Models;
using CleanArchitecture.Blazor.Infrastructure.Services.Authentication;
using CleanArchitecture.Blazor.Application.Common.Interfaces.Identity;
using CleanArchitecture.Blazor.Infrastructure.Hubs;
using CleanArchitecture.Blazor.Infrastructure.Extensions;
using Blazor.Server.UI.Services;

namespace Blazor.Server.UI.Shared;

public partial class MainLayout: IDisposable
{
    private UserModel _user = new();
    private bool _commandPaletteOpen;
    private HotKeysContext? _hotKeysContext;
    private bool _sideMenuDrawerOpen = true;
    private UserPreferences UserPreferences = new();
    [Inject] private LayoutService LayoutService { get; set; } = default!;
    private MudThemeProvider _mudThemeProvider=default!;
    private bool _themingDrawerOpen;
    [Inject] private IDialogService _dialogService { get; set; } = default!;
    [Inject] private HotKeys _hotKeys { get; set; } = default!;
     [CascadingParameter]
    protected Task<AuthenticationState> _authState { get; set; } = default!;
    [Inject]
    private ProfileService _profileService { get; set; } = default!;
    [Inject]
    private AuthenticationStateProvider _authenticationStateProvider { get; set; } = default!;
    public void Dispose()
    {
        LayoutService.MajorUpdateOccured -= LayoutServiceOnMajorUpdateOccured;
        _authenticationStateProvider.AuthenticationStateChanged -= _authenticationStateProvider_AuthenticationStateChanged;
        _hotKeysContext?.Dispose();
        GC.SuppressFinalize(this);
    }
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);
        if (firstRender)
        {
            await ApplyUserPreferences();
            StateHasChanged();
        }
       
    }
    private async Task ApplyUserPreferences()
    {
        var defaultDarkMode = await _mudThemeProvider.GetSystemPreference();
        UserPreferences= await LayoutService.ApplyUserPreferences(defaultDarkMode);
    }
    protected override async Task OnInitializedAsync()
    {
        LayoutService.MajorUpdateOccured += LayoutServiceOnMajorUpdateOccured;
        _profileService.OnChange = (s) =>
        {
            return InvokeAsync(() =>
            {
                _user = s;
                StateHasChanged();
            });
        };
        LayoutService.SetBaseTheme(Theme.ApplicationTheme());
        _hotKeysContext = _hotKeys.CreateContext()
            .Add(ModKeys.Meta, Keys.K, OpenCommandPalette, "Open command palette.");
        _authenticationStateProvider.AuthenticationStateChanged += _authenticationStateProvider_AuthenticationStateChanged;
        var state = await _authState;
        if (state.User.Identity != null && state.User.Identity.IsAuthenticated)
        {
            _user = await _profileService.Get(state.User);
            StateHasChanged();
        }
       await base.OnInitializedAsync();

    }
    private void LayoutServiceOnMajorUpdateOccured(object? sender, EventArgs e) => StateHasChanged();
    private void _authenticationStateProvider_AuthenticationStateChanged(Task<AuthenticationState> authenticationState)
    {
        InvokeAsync(async () =>
        {
            var state = await authenticationState;
            if (state.User.Identity != null && state.User.Identity.IsAuthenticated)
            {
                _user = await _profileService.Get(state.User);
                StateHasChanged();
            }
        });
    }


    protected void SideMenuDrawerOpenChangedHandler(bool state)
    {
        _sideMenuDrawerOpen = state;
    }
    protected void ThemingDrawerOpenChangedHandler(bool state)
    {
        _themingDrawerOpen = state;
    }
    protected void ToggleSideMenuDrawer()
    {
        _sideMenuDrawerOpen = !_sideMenuDrawerOpen;
    }
    private async Task OpenCommandPalette()
    {
        if (!_commandPaletteOpen)
        {
            var options = new DialogOptions
            {
                NoHeader = true,
                MaxWidth = MaxWidth.Medium,
                FullWidth = true
            };

            var commandPalette = _dialogService.Show<CommandPalette>("", options);
            _commandPaletteOpen = true;

            await commandPalette.Result;
            _commandPaletteOpen = false;
        }
    }

    
}