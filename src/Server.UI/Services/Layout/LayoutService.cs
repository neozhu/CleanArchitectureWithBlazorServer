using System.Globalization;
using CleanArchitecture.Blazor.Server.UI.Services.UserPreferences;
using Microsoft.IdentityModel.Tokens;

namespace CleanArchitecture.Blazor.Server.UI.Services.Layout;

public class LayoutService
{
    private readonly IUserPreferencesService UserPreferencesService;
    private bool _systemPreferences;
    public DarkLightMode DarkModeToggle = DarkLightMode.System;

    /// <summary>
    /// Initializes a new instance of the <see cref="LayoutService"/> class.
    /// </summary>
    /// <param name="userPreferencesService">The user preferences service.</param>
    public LayoutService(IUserPreferencesService userPreferencesService)
    {
        UserPreferencesService = userPreferencesService;
    }

    /// <summary>
    /// Gets or sets the user preferences.
    /// </summary>
    public UserPreferences.UserPreference UserPreferences { get; private set; } = new();

    /// <summary>
    /// Gets or sets a value indicating whether the layout is right-to-left.
    /// </summary>
    public bool IsRTL { get; private set; }

    /// <summary>
    /// Gets or sets a value indicating whether the layout is in dark mode.
    /// </summary>
    public bool IsDarkMode { get; private set; }

     

    /// <summary>
    /// Gets or sets the current theme.
    /// </summary>
    public MudTheme CurrentTheme { get; private set; } = new();

    /// <summary>
    /// Sets the dark mode.
    /// </summary>
    /// <param name="value">The value indicating whether dark mode is enabled.</param>
    public void SetDarkMode(bool value)
    {
        UserPreferences.IsDarkMode = value;
    }

    /// <summary>
    /// Applies the user preferences.
    /// </summary>
    /// <param name="isDarkModeDefaultTheme">The value indicating whether dark mode is the default theme.</param>
    public async Task ApplyUserPreferences(bool isDarkModeDefaultTheme)
    {
        UserPreferences = await UserPreferencesService.LoadUserPreferences();

        IsDarkMode = UserPreferences.DarkLightTheme switch
        {
            DarkLightMode.Dark => true,
            DarkLightMode.Light => false,
            DarkLightMode.System => isDarkModeDefaultTheme,
            _ => IsDarkMode
        };
        IsRTL = UserPreferences.RightToLeft;
 
        CurrentTheme.PaletteLight.Primary = UserPreferences.PrimaryColor;
        CurrentTheme.PaletteDark.Primary = UserPreferences.DarkPrimaryColor;
        CurrentTheme.PaletteLight.PrimaryDarken = UserPreferences.PrimaryDarken;
        CurrentTheme.PaletteLight.PrimaryLighten = UserPreferences.PrimaryLighten;
        CurrentTheme.PaletteDark.PrimaryDarken = UserPreferences.PrimaryDarken;
        CurrentTheme.PaletteDark.PrimaryLighten = UserPreferences.PrimaryLighten;
        CurrentTheme.LayoutProperties.DefaultBorderRadius = UserPreferences.BorderRadius + "px";
        CurrentTheme.Typography.Default.FontSize =
            UserPreferences.DefaultFontSize.ToString("0.0000", CultureInfo.InvariantCulture) + "rem";
        CurrentTheme.Typography.Button.FontSize =
            UserPreferences.ButtonFontSize.ToString("0.0000", CultureInfo.InvariantCulture) + "rem";
        CurrentTheme.Typography.Button.LineHeight = UserPreferences.ButtonLineHeight;
        CurrentTheme.Typography.Body1.FontSize =
            UserPreferences.Body1FontSize.ToString("0.0000", CultureInfo.InvariantCulture) + "rem";

        CurrentTheme.Typography.Body1.LineHeight = UserPreferences.Body1LineHeight;
        CurrentTheme.Typography.Body1.LetterSpacing = UserPreferences.Body1LetterSpacing.ToString("0.0000", CultureInfo.InvariantCulture) + "rem";
        CurrentTheme.Typography.Body2.FontSize =
            UserPreferences.Body2FontSize.ToString("0.0000", CultureInfo.InvariantCulture) + "rem";
        CurrentTheme.Typography.Body2.LineHeight = UserPreferences.Body1LineHeight;
        CurrentTheme.Typography.Body2.LetterSpacing = UserPreferences.Body1LetterSpacing.ToString("0.0000", CultureInfo.InvariantCulture) + "rem";

        CurrentTheme.Typography.Caption.FontSize =
            UserPreferences.CaptionFontSize.ToString("0.0000", CultureInfo.InvariantCulture) + "rem";
        CurrentTheme.Typography.Caption.LineHeight = UserPreferences.CaptionLineHeight;
        CurrentTheme.Typography.Overline.FontSize =
            UserPreferences.OverlineFontSize.ToString("0.0000", CultureInfo.InvariantCulture) + "rem";
        CurrentTheme.Typography.Subtitle1.FontSize =
            UserPreferences.Subtitle1FontSize.ToString("0.0000", CultureInfo.InvariantCulture) + "rem";
        CurrentTheme.Typography.Subtitle2.FontSize =
            UserPreferences.Subtitle1FontSize.ToString("0.0000", CultureInfo.InvariantCulture) + "rem";
    }

    /// <summary>
    /// Event that is raised when a major update occurs.
    /// </summary>
    public event EventHandler? MajorUpdateOccured;

    /// <summary>
    /// Raises the MajorUpdateOccured event.
    /// </summary>
    private void OnMajorUpdateOccured()
    {
        MajorUpdateOccured?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Handles the system preference changed event.
    /// </summary>
    /// <param name="newValue">The new value of the system preference.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public Task OnSystemPreferenceChanged(bool newValue)
    {
        _systemPreferences = newValue;
        if (DarkModeToggle == DarkLightMode.System)
        {
            IsDarkMode = newValue;
            OnMajorUpdateOccured();
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// Toggles the dark mode.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
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
        await UserPreferencesService.SaveUserPreferences(UserPreferences).ConfigureAwait(false);
        OnMajorUpdateOccured();
    }

    /// <summary>
    /// Toggles the right-to-left layout.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task ToggleRightToLeft()
    {
        IsRTL = !IsRTL;
        UserPreferences.RightToLeft = IsRTL;
        await UserPreferencesService.SaveUserPreferences(UserPreferences).ConfigureAwait(false);
        OnMajorUpdateOccured();
    }

    /// <summary>
    /// Sets the layout to right-to-left.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task SetRightToLeft()
    {
        if (!IsRTL)
            await ToggleRightToLeft().ConfigureAwait(false);
    }

    /// <summary>
    /// Sets the layout to left-to-right.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task SetLeftToRight()
    {
        if (IsRTL)
            await ToggleRightToLeft().ConfigureAwait(false);
    }

    /// <summary>
    /// Sets the base theme.
    /// </summary>
    /// <param name="theme">The theme to set.</param>
    public void SetBaseTheme(MudTheme theme)
    {
        CurrentTheme = theme;
        OnMajorUpdateOccured();
    }

    /// <summary>
    /// Sets the secondary color.
    /// </summary>
    /// <param name="color">The secondary color to set.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task SetSecondaryColor(string color)
    {
        CurrentTheme.PaletteLight.Secondary = color;
        CurrentTheme.PaletteDark.Secondary = color;
        UserPreferences.SecondaryColor = color;
        await UserPreferencesService.SaveUserPreferences(UserPreferences).ConfigureAwait(false);
        OnMajorUpdateOccured();
    }

    /// <summary>
    /// Sets the border radius.
    /// </summary>
    /// <param name="size">The size of the border radius.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task SetBorderRadius(double size)
    {
        CurrentTheme.LayoutProperties.DefaultBorderRadius = size + "px";
        UserPreferences.BorderRadius = size;
        await UserPreferencesService.SaveUserPreferences(UserPreferences).ConfigureAwait(false);
        OnMajorUpdateOccured();
    }

    /// <summary>
    /// Updates the user preferences.
    /// </summary>
    /// <param name="preferences">The updated user preferences.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task UpdateUserPreferences(UserPreferences.UserPreference preferences)
    {
        UserPreferences = preferences;
        IsDarkMode = UserPreferences.DarkLightTheme switch
        {
            DarkLightMode.Dark => true,
            DarkLightMode.Light => false,
            DarkLightMode.System => _systemPreferences = true,
            _ => IsDarkMode
        };
        IsRTL = UserPreferences.RightToLeft;
        CurrentTheme.PaletteLight.Primary = UserPreferences.PrimaryColor;
        CurrentTheme.PaletteDark.Primary = UserPreferences.DarkPrimaryColor;
        CurrentTheme.PaletteLight.PrimaryDarken = UserPreferences.PrimaryDarken;
        CurrentTheme.PaletteLight.PrimaryLighten = UserPreferences.PrimaryLighten;
        CurrentTheme.PaletteDark.PrimaryDarken = UserPreferences.PrimaryDarken;
        CurrentTheme.PaletteDark.PrimaryLighten = UserPreferences.PrimaryLighten;
        CurrentTheme.LayoutProperties.DefaultBorderRadius = UserPreferences.BorderRadius + "px";
        CurrentTheme.Typography.Default.FontSize =
            UserPreferences.DefaultFontSize.ToString("0.0000", CultureInfo.InvariantCulture) + "rem";
        CurrentTheme.Typography.Button.FontSize =
            UserPreferences.ButtonFontSize.ToString("0.0000", CultureInfo.InvariantCulture) + "rem";
        CurrentTheme.Typography.Button.LineHeight = UserPreferences.ButtonLineHeight;
        CurrentTheme.Typography.Body1.FontSize =
            UserPreferences.Body1FontSize.ToString("0.0000", CultureInfo.InvariantCulture) + "rem";

        CurrentTheme.Typography.Body1.LineHeight = UserPreferences.Body1LineHeight;
        CurrentTheme.Typography.Body1.LetterSpacing = UserPreferences.Body1LetterSpacing.ToString("0.0000", CultureInfo.InvariantCulture) + "rem";
        CurrentTheme.Typography.Body2.FontSize =
            UserPreferences.Body2FontSize.ToString("0.0000", CultureInfo.InvariantCulture) + "rem";
        CurrentTheme.Typography.Body2.LineHeight = UserPreferences.Body1LineHeight;
        CurrentTheme.Typography.Body2.LetterSpacing = UserPreferences.Body1LetterSpacing.ToString("0.0000", CultureInfo.InvariantCulture) + "rem";

        CurrentTheme.Typography.Caption.FontSize =
            UserPreferences.CaptionFontSize.ToString("0.0000", CultureInfo.InvariantCulture) + "rem";
        CurrentTheme.Typography.Caption.LineHeight = UserPreferences.CaptionLineHeight;
        CurrentTheme.Typography.Overline.FontSize =
            UserPreferences.OverlineFontSize.ToString("0.0000", CultureInfo.InvariantCulture) + "rem";
        CurrentTheme.Typography.Subtitle1.FontSize =
            UserPreferences.Subtitle1FontSize.ToString("0.0000", CultureInfo.InvariantCulture) + "rem";
        CurrentTheme.Typography.Subtitle2.FontSize =
            UserPreferences.Subtitle1FontSize.ToString("0.0000", CultureInfo.InvariantCulture) + "rem";


        await UserPreferencesService.SaveUserPreferences(UserPreferences).ConfigureAwait(false);
        OnMajorUpdateOccured();
    }
}
