using System.Globalization;
using CleanArchitecture.Blazor.Server.UI.Services.UserPreferences;
using Microsoft.JSInterop;

namespace CleanArchitecture.Blazor.Server.UI.Services.Layout;

public class LayoutService
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
    /// Sets the dark mode flag in the user preferences.
    /// </summary>
    public void SetDarkMode(bool value)
    {
        UserPreferences.IsDarkMode = value;
    }

    /// <summary>
    /// Loads and applies the user preferences.
    /// The isDarkModeDefaultTheme parameter indicates whether dark mode is enabled by default when using system settings.
    /// </summary>
    public async Task ApplyUserPreferences(bool isDarkModeDefaultTheme)
    {
        UserPreferences = await _userPreferencesService.LoadUserPreferences().ConfigureAwait(false);
        UpdateLayoutSettings(isDarkModeDefaultTheme);
        UpdateCurrentTheme();
        await ApplyBaseFontSize().ConfigureAwait(false);
    }

    /// <summary>
    /// Updates the user preferences, saves them, and updates the theme accordingly.
    /// </summary>
    public async Task UpdateUserPreferences(UserPreference preferences)
    {
        UserPreferences = preferences;
        UpdateLayoutSettings(_systemPreferences);
        UpdateCurrentTheme();
        await ApplyBaseFontSize().ConfigureAwait(false);
        await _userPreferencesService.SaveUserPreferences(UserPreferences).ConfigureAwait(false);
        await OnMajorUpdateOccurred();
    }

    /// <summary>
    /// Updates layout settings (IsDarkMode and IsRTL) based on user preferences.
    /// The isDarkModeDefault parameter indicates whether dark mode should be enabled when in system mode.
    /// </summary>
    private void UpdateLayoutSettings(bool isDarkModeDefault)
    {
        IsDarkMode = UserPreferences.DarkLightTheme switch
        {
            DarkLightMode.Dark => true,
            DarkLightMode.Light => false,
            DarkLightMode.System => isDarkModeDefault,
            _ => IsDarkMode
        };
        IsRTL = UserPreferences.RightToLeft;
    }

    /// <summary>
    /// Updates the CurrentTheme properties including palette, layout, and typography.
    /// </summary>
    private void UpdateCurrentTheme()
    {
        // Update palette settings
        CurrentTheme.PaletteLight.Primary = UserPreferences.PrimaryColor;
        CurrentTheme.PaletteDark.Primary = UserPreferences.DarkPrimaryColor;
        CurrentTheme.PaletteLight.PrimaryDarken = UserPreferences.PrimaryDarken;
        CurrentTheme.PaletteLight.PrimaryLighten = UserPreferences.PrimaryLighten;

        // Update layout properties
        CurrentTheme.LayoutProperties.DefaultBorderRadius = UserPreferences.BorderRadius + "px";

         
    }

    private async Task EnsureThemeModule()
    {
        if (_themeModule is null)
        {
            _themeModule = await _jsRuntime.InvokeAsync<IJSObjectReference>("import", "/js/theme.js").ConfigureAwait(false);
        }
    }

    private async Task ApplyBaseFontSize()
    {
        try
        {
            await EnsureThemeModule().ConfigureAwait(false);
            var size = UserPreferences?.DefaultFontSize > 0 ? UserPreferences.DefaultFontSize : 14;
            await _themeModule!.InvokeVoidAsync("setRootFontSize", size).ConfigureAwait(false);
        }
        catch
        {
            // ignore JS errors on early startup
        }
    }



    #region Events and System Preferences

    public event Func<Task>? MajorUpdateOccurred;

    /// <summary>
    /// Raises the MajorUpdateOccurred event.
    /// </summary>
    private async Task OnMajorUpdateOccurred()
    {
        if (MajorUpdateOccurred is not null)
        {
            var handlers = MajorUpdateOccurred.GetInvocationList();
            foreach (Func<Task> handler in handlers)
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
        await _userPreferencesService.SaveUserPreferences(UserPreferences).ConfigureAwait(false);
        await  OnMajorUpdateOccurred();
    }

    /// <summary>
    /// Toggles the layout direction between right-to-left and left-to-right.
    /// Saves the updated user preference and notifies subscribers.
    /// </summary>
    public async Task ToggleRightToLeft()
    {
        IsRTL = !IsRTL;
        UserPreferences.RightToLeft = IsRTL;
        await _userPreferencesService.SaveUserPreferences(UserPreferences).ConfigureAwait(false);
        await  OnMajorUpdateOccurred();
    }

    /// <summary>
    /// Sets the layout to right-to-left if it is not already.
    /// </summary>
    public async Task SetRightToLeft()
    {
        if (!IsRTL)
            await ToggleRightToLeft().ConfigureAwait(false);
    }

    /// <summary>
    /// Sets the layout to left-to-right if it is currently right-to-left.
    /// </summary>
    public async Task SetLeftToRight()
    {
        if (IsRTL)
            await ToggleRightToLeft().ConfigureAwait(false);
    }

    /// <summary>
    /// Sets the base theme and notifies subscribers.
    /// </summary>
    public async Task SetBaseTheme(MudTheme theme)
    {
        CurrentTheme = theme;
        await OnMajorUpdateOccurred();
    }

    /// <summary>
    /// Sets the secondary color in the theme, saves the updated preference, and notifies subscribers.
    /// </summary>
    public async Task SetSecondaryColor(string color)
    {
        CurrentTheme.PaletteLight.Secondary = color;
        CurrentTheme.PaletteDark.Secondary = color;
        UserPreferences.SecondaryColor = color;
        await _userPreferencesService.SaveUserPreferences(UserPreferences).ConfigureAwait(false);
        await OnMajorUpdateOccurred();
    }

    /// <summary>
    /// Sets the border radius in the theme, saves the updated preference, and notifies subscribers.
    /// </summary>
    public async Task SetBorderRadius(double size)
    {
        CurrentTheme.LayoutProperties.DefaultBorderRadius = size + "px";
        UserPreferences.BorderRadius = size;
        await _userPreferencesService.SaveUserPreferences(UserPreferences).ConfigureAwait(false);
       await  OnMajorUpdateOccurred();
    }

    #endregion
}
