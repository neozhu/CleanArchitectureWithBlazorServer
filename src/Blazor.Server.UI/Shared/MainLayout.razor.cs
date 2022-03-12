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

namespace Blazor.Server.UI.Shared;

public partial class MainLayout : IDisposable
{
    private readonly Palette _darkPalette = new()
    {
        Black = "#27272f",
        Background = "rgb(21,27,34)",
        BackgroundGrey = "#27272f",
        Surface = "#212B36",
        DrawerBackground = "rgb(21,27,34)",
        DrawerText = "rgba(255,255,255, 0.50)",
        DrawerIcon = "rgba(255,255,255, 0.50)",
        AppbarBackground = "rgba(21,27,34,0.7)",
        AppbarText = "rgba(255,255,255, 0.70)",
        TextPrimary = "rgba(255,255,255, 0.70)",
        TextSecondary = "rgba(255,255,255, 0.50)",
        ActionDefault = "#adadb1",
        ActionDisabled = "rgba(255,255,255, 0.26)",
        ActionDisabledBackground = "rgba(255,255,255, 0.12)",
        DarkDarken= "rgba(21,27,34,0.7)",
        Divider = "rgba(255,255,255, 0.12)",
        DividerLight = "rgba(255,255,255, 0.06)",
        TableLines = "rgba(255,255,255, 0.12)",
        LinesDefault = "rgba(255,255,255, 0.12)",
        LinesInputs = "rgba(255,255,255, 0.3)",
        TextDisabled = "rgba(255,255,255, 0.2)"
    };

    private readonly Palette _lightPalette = new()
    {
        Primary = "#2d4275",
        Black = "#0A0E19",
        Success = "#64A70B",
        Secondary = "#ff4081ff",
        AppbarBackground = "rgba(255,255,255,0.8)",
    };
    private readonly MudTheme _theme = new()
    {
 
        Palette = new Palette
        {
            Primary = "#2d4275",
            Black = "#0A0E19",
            Success = "#64A70B",
            Secondary = "#ff4081ff",
            AppbarBackground = "rgba(255,255,255,0.8)",
        },
        PaletteDark =new Palette
        {
            Black = "#27272f",
            Background = "rgb(21,27,34)",
            BackgroundGrey = "#27272f",
            Surface = "#212B36",
            DrawerBackground = "rgb(21,27,34)",
            DrawerText = "rgba(255,255,255, 0.50)",
            DrawerIcon = "rgba(255,255,255, 0.50)",
            AppbarBackground = "rgba(21,27,34,0.7)",
            AppbarText = "rgba(255,255,255, 0.70)",
            TextPrimary = "rgba(255,255,255, 0.70)",
            TextSecondary = "rgba(255,255,255, 0.50)",
            ActionDefault = "#adadb1",
            ActionDisabled = "rgba(255,255,255, 0.26)",
            ActionDisabledBackground = "rgba(255,255,255, 0.12)",
            DarkDarken = "rgba(21,27,34,0.7)",
            Divider = "rgba(255,255,255, 0.12)",
            DividerLight = "rgba(255,255,255, 0.06)",
            TableLines = "rgba(255,255,255, 0.12)",
            LinesDefault = "rgba(255,255,255, 0.12)",
            LinesInputs = "rgba(255,255,255, 0.3)",
            TextDisabled = "rgba(255,255,255, 0.2)"
        },
        LayoutProperties = new LayoutProperties
        {
            AppbarHeight = "80px",
            DefaultBorderRadius = "6px",
        },
        Typography = new Typography
        {
            Default = new Default
            {
                FontSize = ".825rem",
                FontWeight = 400,
                LineHeight = 1.43,
                LetterSpacing = "normal",
                FontFamily = new string[] { "Public Sans", "Roboto", "Arial", "sans-serif" }
            },
            H1 = new H1
            {
                FontSize = "4rem",
                FontWeight = 700,
                LineHeight = 1.167,
                LetterSpacing = "-.01562em"
            },
            H2 = new H2
            {
                FontSize = "3.75rem",
                FontWeight = 300,
                LineHeight = 1.2,
                LetterSpacing = "-.00833em"
            },
            H3 = new H3
            {
                FontSize = "3rem",
                FontWeight = 600,
                LineHeight = 1.167,
                LetterSpacing = "0"
            },
            H4 = new H4
            {
                FontSize = "1.8rem",
                FontWeight = 400,
                LineHeight = 1.235,
                LetterSpacing = ".00735em"
            },
            H5 = new H5
            {
                FontSize = "1.5rem",
                FontWeight = 400,
                LineHeight = 1.334,
                LetterSpacing = "0"
            },
            H6 = new H6
            {
                FontSize = "1.125rem",
                FontWeight = 600,
                LineHeight = 1.6,
                LetterSpacing = ".0075em"
            },
            Button = new Button
            {
                FontSize = ".825rem",
                FontWeight = 500,
                LineHeight = 1.75,
                LetterSpacing = ".02857em",
                TextTransform = "none"


            },
            Subtitle1 = new Subtitle1
            {
                FontSize = "1rem",
                FontWeight = 400,
                LineHeight = 1.75,
                LetterSpacing = ".00938em"
            },
            Subtitle2 = new Subtitle2
            {
                FontSize = ".875rem",
                FontWeight = 500,
                LineHeight = 1.57,
                LetterSpacing = ".00714em"
            },
            Body1 = new Body1
            {
                FontSize = "0.875rem",
                FontWeight = 400,
                LineHeight = 1.5,
                LetterSpacing = ".00938em"
            },
            Body2 = new Body2
            {
                FontSize = ".825rem",
                FontWeight = 400,
                LineHeight = 1.43,
                LetterSpacing = ".01071em"
            },
            Caption = new Caption
            {
                FontSize = ".75rem",
                FontWeight = 400,
                LineHeight = 1.66,
                LetterSpacing = ".03333em"
            },
            Overline = new Overline
            {
                FontSize = ".75rem",
                FontWeight = 400,
                LineHeight = 2.66,
                LetterSpacing = ".08333em"
            }
        }
    };

    private UserModel _user = new()
    {
     
    };

    private bool _commandPaletteOpen;

    private HotKeysContext? _hotKeysContext;
    private bool _sideMenuDrawerOpen = true;
    private bool _rightToLeft = false;
    private bool _isDark = false;
    private ThemeManagerModel _themeManager = new();


    private bool _themingDrawerOpen;
    [Inject] private IDialogService _dialogService { get; set; } = default!;
    [Inject] private NavigationManager _navigationManager { get; set; } = default!;
    [Inject] private HotKeys _hotKeys { get; set; } = default!;
    [Inject] private ILocalStorageService _localStorage { get; set; } = default!;
    [CascadingParameter]
    protected Task<AuthenticationState> _authState { get; set; } = default!;
    [Inject]
    private ProfileService _profileService { get; set; } = default!;

    private HubClient _client  { get; set; } = default!;

    [Inject]
    private AuthenticationStateProvider _authenticationStateProvider { get; set; } = default!;
    public  void Dispose()
    {
        
        if (_client is not null)
        {
            _client.StopAsync().Wait();
            _client.LoggedOut -= _client_LoggedOut;
            _client.LoggedIn -= _client_LoggedIn;
        }
        _authenticationStateProvider.AuthenticationStateChanged -= _authenticationStateProvider_AuthenticationStateChanged;
        _hotKeysContext?.Dispose();
    }
    
    
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            if (await _localStorage.ContainKeyAsync("themeManager"))
                _themeManager = await _localStorage.GetItemAsync<ThemeManagerModel>("themeManager");
            await ThemeManagerChanged(_themeManager);
            StateHasChanged();
        }
       
    }

    private void _client_LoggedIn(object? sender, string e)
    {
        InvokeAsync(() =>
        {
            Snackbar.Add($"{e} login.", MudBlazor.Severity.Info);
            StateHasChanged();
        });
    }

    private void _client_LoggedOut(object? sender, string e)
    {
        InvokeAsync(() =>
        {
            Snackbar.Add($"{e} logout.", MudBlazor.Severity.Normal);
            StateHasChanged();
        });
    }

    protected override async Task OnInitializedAsync()
    {
        _profileService.OnChange = (s) =>
        {
            return InvokeAsync(() =>
            {
                _user = s;
                StateHasChanged();
            });
        };
        
        _hotKeysContext = _hotKeys.CreateContext()
            .Add(ModKeys.Meta, Keys.K, OpenCommandPalette, "Open command palette.");
        _authenticationStateProvider.AuthenticationStateChanged += _authenticationStateProvider_AuthenticationStateChanged;

        var state = await _authState;
        if (state.User.Identity != null && state.User.Identity.IsAuthenticated)
        {
            _user = await _profileService.Get(state.User);
            _client = new HubClient(_navigationManager.BaseUri, state.User.GetUserId());
            _client.LoggedOut += _client_LoggedOut;
            _client.LoggedIn += _client_LoggedIn;
            await _client.StartAsync();
        }
    }

    private async void _authenticationStateProvider_AuthenticationStateChanged(Task<AuthenticationState> authenticationState)
    {
        var state = await authenticationState;
        if (state.User.Identity != null && state.User.Identity.IsAuthenticated)
        {
            _user = await _profileService.Get(state.User);
            _client = new HubClient(_navigationManager.BaseUri, state.User.GetUserId());
            _client.LoggedOut += _client_LoggedOut;
            _client.LoggedIn += _client_LoggedIn;
            await _client.StartAsync();
            StateHasChanged();
        }
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
    protected void RightToLeftToggleHandler()
    {
        _rightToLeft = !_rightToLeft;
    }

    protected async Task ThemeManagerChanged(ThemeManagerModel themeManager)
    {
        _themeManager = themeManager;
        //_theme.Palette = _themeManager.IsDarkMode ? _darkPalette : _lightPalette;
        _isDark = _themeManager.IsDarkMode;
        _theme.Palette.Primary = _themeManager.PrimaryColor;
        _theme.PaletteDark.Primary = _themeManager.PrimaryColor;
        _theme.LayoutProperties.DefaultBorderRadius = _themeManager.BorderRadius + "px";
        await UpdateThemeManagerLocalStorage();
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

    private async Task UpdateThemeManagerLocalStorage()
    {
        await _localStorage.SetItemAsync("themeManager", _themeManager);
    }
}