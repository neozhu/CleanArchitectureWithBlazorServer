using Microsoft.AspNetCore.Components;
using MudBlazor;
using Blazor.Server.UI.Components.Shared;
using Toolbelt.Blazor.HotKeys;
using Microsoft.AspNetCore.Components.Authorization;
using CleanArchitecture.Blazor.Infrastructure.Services.Authentication;
using Blazor.Server.UI.Services;

namespace Blazor.Server.UI.Shared;

public partial class MainLayout: IDisposable
{
    private bool _commandPaletteOpen;
    private HotKeysContext? _hotKeysContext;
    private bool _sideMenuDrawerOpen = true;
    private UserPreferences UserPreferences = new();
    [Inject] 
    private LayoutService _layoutService { get; set; } = null!;
    private MudThemeProvider? _mudThemeProvider;
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
        _profileService.OnChange -= _profileService_OnChange;
        _layoutService.MajorUpdateOccured -= LayoutServiceOnMajorUpdateOccured;
        _authenticationStateProvider.AuthenticationStateChanged -= _authenticationStateProvider_AuthenticationStateChanged;
        _hotKeysContext?.Dispose();
        GC.SuppressFinalize(this);
    }
  
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await ApplyUserPreferences();
            StateHasChanged();
        }
       
    }
    private async Task ApplyUserPreferences()
    {
        if (_mudThemeProvider is not null)
        {
            var defaultDarkMode = await _mudThemeProvider.GetSystemPreference();
            UserPreferences = await _layoutService.ApplyUserPreferences(defaultDarkMode);
        }
    }
    protected override async Task OnInitializedAsync()
    {
        _layoutService.MajorUpdateOccured += LayoutServiceOnMajorUpdateOccured;
        _profileService.OnChange += _profileService_OnChange;
        _layoutService.SetBaseTheme(Theme.ApplicationTheme());
        _hotKeysContext = _hotKeys.CreateContext()
            .Add(ModKeys.Meta, Keys.K, OpenCommandPalette, "Open command palette.");
        _authenticationStateProvider.AuthenticationStateChanged += _authenticationStateProvider_AuthenticationStateChanged;
        var state = await _authState;
        if (state.User.Identity != null && state.User.Identity.IsAuthenticated)
        {
              await _profileService.Set(state.User);
        }
       await base.OnInitializedAsync();

    }

    private void _profileService_OnChange()
    {
        InvokeAsync(() => StateHasChanged());
    }

    private void LayoutServiceOnMajorUpdateOccured(object? sender, EventArgs e) => StateHasChanged();
    private void _authenticationStateProvider_AuthenticationStateChanged(Task<AuthenticationState> authenticationState)
    {
        InvokeAsync(async () =>
        {
            var state = await authenticationState;
            if (state.User.Identity != null && state.User.Identity.IsAuthenticated)
            {
               await _profileService.Set(state.User);
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