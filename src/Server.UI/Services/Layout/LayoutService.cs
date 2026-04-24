using System.Globalization;
using CleanArchitecture.Blazor.Server.UI.Services.UserPreferences;
using Microsoft.JSInterop;

namespace CleanArchitecture.Blazor.Server.UI.Services.Layout;

public class LayoutService : IAsyncDisposable
{
    private readonly IUserPreferencesService _userPreferencesService;
    private readonly IJSRuntime _jsRuntime;
    private IJSObjectReference? _themeModule;
    private bool _systemPreferences;

    public DarkLightMode DarkModeToggle { get; private set; } = DarkLightMode.System;

    public LayoutService(IUserPreferencesService userPreferencesService, IJSRuntime jsRuntime)
    {
        _userPreferencesService = userPreferencesService;
        _jsRuntime = jsRuntime;
    }

    public UserPreferences.UserPreference UserPreferences { get; private set; } = new();
    public bool IsRTL { get; private set; }
    public bool IsDarkMode { get; private set; }
    public MudTheme CurrentTheme { get; private set; } = new();

    /// <summary>
    /// Loads and applies the user preferences.
    /// The isDarkModeDefaultTheme parameter indicates whether dark mode is enabled by default when using system settings.
    /// </summary>
    public async Task ApplyUserPreferences(bool isDarkModeDefaultTheme)
    {
        _systemPreferences = isDarkModeDefaultTheme;
        UserPreferences = await _userPreferencesService.LoadUserPreferences(isDarkModeDefaultTheme);
        DarkModeToggle = UserPreferences.DarkLightTheme; // Fix #1: sync toggle state
        await UpdateDarkMode(isDarkModeDefaultTheme);
        IsRTL = UserPreferences.RightToLeft;
        UpdateCurrentTheme();
        await ApplyBaseFontSize();
    }

    /// <summary>
    /// Updates the user preferences, saves them, and updates the theme accordingly.
    /// </summary>
    public async Task UpdateUserPreferences(UserPreference preferences)
    {
        UserPreferences = preferences;
        DarkModeToggle = preferences.DarkLightTheme;
        await UpdateDarkMode(_systemPreferences); // Fix #2: single path for dark mode changes
        IsRTL = preferences.RightToLeft;
        UpdateCurrentTheme();
        await ApplyBaseFontSize();
        await _userPreferencesService.SaveUserPreferences(UserPreferences);
        await OnMajorUpdateOccurred();
    }

    /// <summary>
    /// Resolves and applies the effective dark mode state from preferences and system setting.
    /// Fires <see cref="DarkModeChanged"/> only when the value actually changes.
    /// </summary>
    private async Task UpdateDarkMode(bool isDarkModeDefault)
    {
        var isDarkMode = UserPreferences.DarkLightTheme switch // Fix #3: renamed from isDarkModel
        {
            DarkLightMode.Dark => true,
            DarkLightMode.Light => false,
            DarkLightMode.System => isDarkModeDefault,
            _ => IsDarkMode
        };

        if (IsDarkMode != isDarkMode)
        {
            IsDarkMode = isDarkMode;
            await OnDarkModeChanged();
        }
    }

    /// <summary>
    /// Updates the CurrentTheme properties including palette, layout, and typography.
    /// </summary>
    private void UpdateCurrentTheme()
    {
        CurrentTheme.PaletteLight.Primary = UserPreferences.PrimaryColor;
        CurrentTheme.PaletteDark.Primary = UserPreferences.DarkPrimaryColor;
        if (IsDarkMode)
        {
            CurrentTheme.PaletteDark.PrimaryDarken = UserPreferences.PrimaryDarken;
            CurrentTheme.PaletteDark.PrimaryLighten = UserPreferences.PrimaryLighten;
            CurrentTheme.PaletteDark.PrimaryContrastText = UserPreferences.PrimaryContrastText;
        }
        else
        {
            CurrentTheme.PaletteLight.PrimaryDarken = UserPreferences.PrimaryDarken;
            CurrentTheme.PaletteLight.PrimaryLighten = UserPreferences.PrimaryLighten;
            CurrentTheme.PaletteLight.PrimaryContrastText = UserPreferences.PrimaryContrastText;
        }

        CurrentTheme.LayoutProperties.DefaultBorderRadius = UserPreferences.BorderRadius + "px";
    }

    private async Task EnsureThemeModule()
    {
        _themeModule ??= await _jsRuntime.InvokeAsync<IJSObjectReference>("import", "/js/theme.js");
    }

    private async Task ApplyBaseFontSize()
    {
        try
        {
            await EnsureThemeModule();
            var size = UserPreferences?.DefaultFontSize > 0 ? UserPreferences.DefaultFontSize : 15;
            await _themeModule!.InvokeVoidAsync("setRootFontSize", size);
        }
        catch (JSDisconnectedException) { } // Fix #6: only catch expected JS interop failures
        catch (InvalidOperationException) { } // prerendering not yet connected
    }

    #region Events and System Preferences

    public event Func<Task>? MajorUpdateOccurred;
    public event Func<Task>? DarkModeChanged;

    private async Task OnDarkModeChanged()
    {
        if (DarkModeChanged is not null)
        {
            foreach (Func<Task> handler in DarkModeChanged.GetInvocationList())
            {
                await handler();
            }
        }
    }

    private async Task OnMajorUpdateOccurred()
    {
        if (MajorUpdateOccurred is not null)
        {
            foreach (Func<Task> handler in MajorUpdateOccurred.GetInvocationList())
            {
                await handler();
            }
        }
    }

    /// <summary>
    /// Handles system preference changes (e.g., system dark mode).
    /// Updates IsDarkMode based on the new value and notifies subscribers.
    /// </summary>
    public async Task OnSystemPreferenceChanged(bool newValue)
    {
        _systemPreferences = newValue;
        if (DarkModeToggle == DarkLightMode.System)
        {
            IsDarkMode = newValue;
            await OnDarkModeChanged(); // Fix #5: also notify dark mode subscribers
            await OnMajorUpdateOccurred();
        }
    }

    #endregion

    #region Mode Switching and Other Settings

    /// <summary>
    /// Toggles dark mode between System, Light, and Dark modes.
    /// Saves the updated user preference and notifies subscribers.
    /// </summary>
    public async Task ToggleDarkMode()
    {
        switch (DarkModeToggle)
        {
            case DarkLightMode.System:
                DarkModeToggle = DarkLightMode.Light;
                IsDarkMode = false;
                break;
            case DarkLightMode.Light:
                DarkModeToggle = DarkLightMode.Dark;
                IsDarkMode = true;
                break;
            case DarkLightMode.Dark:
                DarkModeToggle = DarkLightMode.System;
                IsDarkMode = _systemPreferences;
                break;
        }
        UserPreferences.DarkLightTheme = DarkModeToggle;
        await _userPreferencesService.SaveUserPreferences(UserPreferences);
        await OnDarkModeChanged();
        await OnMajorUpdateOccurred();
    }

    /// <summary>
    /// Toggles the layout direction between right-to-left and left-to-right.
    /// </summary>
    public async Task ToggleRightToLeft()
    {
        IsRTL = !IsRTL;
        UserPreferences.RightToLeft = IsRTL;
        await _userPreferencesService.SaveUserPreferences(UserPreferences);
        await OnMajorUpdateOccurred();
    }

    public async Task SetRightToLeft()
    {
        if (!IsRTL)
            await ToggleRightToLeft();
    }

    public async Task SetLeftToRight()
    {
        if (IsRTL)
            await ToggleRightToLeft();
    }

    public async Task SetBaseTheme(MudTheme theme)
    {
        CurrentTheme = theme;
        await OnMajorUpdateOccurred();
    }

    public async Task SetBorderRadius(double size)
    {
        CurrentTheme.LayoutProperties.DefaultBorderRadius = size + "px";
        UserPreferences.BorderRadius = size;
        await _userPreferencesService.SaveUserPreferences(UserPreferences);
        await OnMajorUpdateOccurred();
    }

    #endregion

    // Fix #4: dispose JS module reference
    public async ValueTask DisposeAsync()
    {
        if (_themeModule is not null)
        {
            try
            {
                await _themeModule.DisposeAsync();
            }
            catch (JSDisconnectedException) { }
        }
        GC.SuppressFinalize(this);
    }
}
