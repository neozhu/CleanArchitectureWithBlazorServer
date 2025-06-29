using System.Globalization;
using CleanArchitecture.Blazor.Server.UI.Services.UserPreferences;

namespace CleanArchitecture.Blazor.Server.UI.Services.Layout;

public class LayoutService
{
    private readonly IUserPreferencesService _userPreferencesService;
    private bool _systemPreferences;

    public DarkLightMode DarkModeToggle { get; private set; } = DarkLightMode.System;

    public LayoutService(IUserPreferencesService userPreferencesService)
    {
        _userPreferencesService = userPreferencesService;
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
    }

    /// <summary>
    /// Updates the user preferences, saves them, and updates the theme accordingly.
    /// </summary>
    public async Task UpdateUserPreferences(UserPreference preferences)
    {
        UserPreferences = preferences;
        UpdateLayoutSettings(_systemPreferences);
        UpdateCurrentTheme();
        await _userPreferencesService.SaveUserPreferences(UserPreferences).ConfigureAwait(false);
        OnMajorUpdateOccurred();
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
        CurrentTheme.PaletteDark.PrimaryDarken = UserPreferences.PrimaryDarken;
        CurrentTheme.PaletteDark.PrimaryLighten = UserPreferences.PrimaryLighten;

        // Update layout properties
        CurrentTheme.LayoutProperties.DefaultBorderRadius = UserPreferences.BorderRadius + "px";

        // Update typography settings
        string defaultFontSize = FormatFontSize(UserPreferences.DefaultFontSize);
        CurrentTheme.Typography.Default.FontSize = defaultFontSize;

        CurrentTheme.Typography.Button.FontSize = FormatFontSize(UserPreferences.ButtonFontSize);
        CurrentTheme.Typography.Button.LineHeight = UserPreferences.ButtonLineHeight.ToString();

        // Heading fonts
        CurrentTheme.Typography.H1.FontSize = FormatFontSize(UserPreferences.H1FontSize);
        CurrentTheme.Typography.H2.FontSize = FormatFontSize(UserPreferences.H2FontSize);
        CurrentTheme.Typography.H3.FontSize = FormatFontSize(UserPreferences.H3FontSize);
        CurrentTheme.Typography.H4.FontSize = FormatFontSize(UserPreferences.H4FontSize);
        CurrentTheme.Typography.H5.FontSize = FormatFontSize(UserPreferences.H5FontSize);
        CurrentTheme.Typography.H6.FontSize = FormatFontSize(UserPreferences.H6FontSize);

        // Body text
        CurrentTheme.Typography.Body1.FontSize = FormatFontSize(UserPreferences.Body1FontSize);
        CurrentTheme.Typography.Body1.LineHeight = UserPreferences.Body1LineHeight.ToString();
        //CurrentTheme.Typography.Body1.LetterSpacing = FormatFontSize(UserPreferences.Body1LetterSpacing);

        CurrentTheme.Typography.Body2.FontSize = FormatFontSize(UserPreferences.Body2FontSize);
        CurrentTheme.Typography.Body2.LineHeight = UserPreferences.Body1LineHeight.ToString();
        //CurrentTheme.Typography.Body2.LetterSpacing = FormatFontSize(UserPreferences.Body1LetterSpacing);

        // Caption and overline texts
        CurrentTheme.Typography.Caption.FontSize = FormatFontSize(UserPreferences.CaptionFontSize);
        CurrentTheme.Typography.Caption.LineHeight = UserPreferences.CaptionLineHeight.ToString();

        CurrentTheme.Typography.Overline.FontSize = FormatFontSize(UserPreferences.OverlineFontSize);
        CurrentTheme.Typography.Overline.LineHeight = UserPreferences.OverlineLineHeight.ToString();

        // Subtitles
        CurrentTheme.Typography.Subtitle1.FontSize = FormatFontSize(UserPreferences.Subtitle1FontSize);
        CurrentTheme.Typography.Subtitle2.FontSize = FormatFontSize(UserPreferences.Subtitle2FontSize); ;
    }

    /// <summary>
    /// Formats a font size value as a string with the "rem" unit.
    /// </summary>
    private string FormatFontSize(double size) =>
        size.ToString("0.0000", CultureInfo.InvariantCulture) + "rem";

    #region Events and System Preferences

    public event EventHandler? MajorUpdateOccurred;

    /// <summary>
    /// Raises the MajorUpdateOccurred event.
    /// </summary>
    private void OnMajorUpdateOccurred() =>
        MajorUpdateOccurred?.Invoke(this, EventArgs.Empty);

    /// <summary>
    /// Handles system preference changes (e.g., system dark mode).
    /// Updates IsDarkMode based on the new value and notifies subscribers.
    /// </summary>
    public Task OnSystemPreferenceChanged(bool newValue)
    {
        _systemPreferences = newValue;
        if (DarkModeToggle == DarkLightMode.System)
        {
            IsDarkMode = newValue;
            OnMajorUpdateOccurred();
        }
        return Task.CompletedTask;
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
        OnMajorUpdateOccurred();
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
        OnMajorUpdateOccurred();
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
    public void SetBaseTheme(MudTheme theme)
    {
        CurrentTheme = theme;
        OnMajorUpdateOccurred();
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
        OnMajorUpdateOccurred();
    }

    /// <summary>
    /// Sets the border radius in the theme, saves the updated preference, and notifies subscribers.
    /// </summary>
    public async Task SetBorderRadius(double size)
    {
        CurrentTheme.LayoutProperties.DefaultBorderRadius = size + "px";
        UserPreferences.BorderRadius = size;
        await _userPreferencesService.SaveUserPreferences(UserPreferences).ConfigureAwait(false);
        OnMajorUpdateOccurred();
    }

    #endregion
}