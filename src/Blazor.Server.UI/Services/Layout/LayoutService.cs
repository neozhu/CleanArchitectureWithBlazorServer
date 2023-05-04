using Blazor.Server.UI.Services.UserPreferences;

namespace Blazor.Server.UI.Services.Layout;

public class LayoutService
{
    private bool _systemPreferences;
    private readonly IUserPreferencesService _userPreferencesService;
    private UserPreferences.UserPreferences _userPreferences=new();
    public DarkLightMode DarkModeToggle = DarkLightMode.System;
    public bool IsRTL { get; private set; } = false;
    public bool IsDarkMode { get; private set; } = false;
    public string PrimaryColor { get; set; } = "#2d4275";
    public string SecondaryColor { get; set; } = "#ff4081ff";
    public double BorderRadius { get; set; } = 4;
    public double DefaultFontSize { get; set; } = 0.8125;
    public MudTheme CurrentTheme { get; private set; } = new();


    public LayoutService(IUserPreferencesService userPreferencesService)
    {
        _userPreferencesService = userPreferencesService;
    }

    public void SetDarkMode(bool value)
    {
        IsDarkMode = value;
    }

    public async Task<UserPreferences.UserPreferences> ApplyUserPreferences(bool isDarkModeDefaultTheme)
    {
        _userPreferences = await _userPreferencesService.LoadUserPreferences();
        if (_userPreferences != null)
        {
            IsDarkMode =  _userPreferences.DarkLightTheme switch
            {
                DarkLightMode.Dark => true,
                DarkLightMode.Light => false,
                DarkLightMode.System => isDarkModeDefaultTheme,
                _ => IsDarkMode
            };
            IsRTL = _userPreferences.RightToLeft;
            PrimaryColor = _userPreferences.PrimaryColor;
            SecondaryColor = _userPreferences.SecondaryColor;
            BorderRadius = _userPreferences.BorderRadius;
            DefaultFontSize = _userPreferences.DefaultFontSize;
            CurrentTheme.Palette.Primary = PrimaryColor;
            CurrentTheme.PaletteDark.Primary = PrimaryColor;
            CurrentTheme.Palette.Secondary = SecondaryColor;
            CurrentTheme.PaletteDark.Secondary = SecondaryColor;
            CurrentTheme.LayoutProperties.DefaultBorderRadius = BorderRadius + "px";
            CurrentTheme.Typography.Default.FontSize = DefaultFontSize + "rem";
        }
        else
        {
            IsDarkMode = isDarkModeDefaultTheme;
            _userPreferences = new UserPreferences.UserPreferences { IsDarkMode = IsDarkMode };
            await _userPreferencesService.SaveUserPreferences(_userPreferences);
        }
        return _userPreferences;
    }

    public event EventHandler? MajorUpdateOccured;

    private void OnMajorUpdateOccured() => MajorUpdateOccured?.Invoke(this, EventArgs.Empty);

    public  Task OnSystemPreferenceChanged(bool newValue)
    {
        _systemPreferences = newValue;
        if (DarkModeToggle == DarkLightMode.System)
        {
            IsDarkMode = newValue;
            OnMajorUpdateOccured();
        }
        return Task.CompletedTask;
    }
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

        _userPreferences.DarkLightTheme = DarkModeToggle;
        await _userPreferencesService.SaveUserPreferences(_userPreferences);
        OnMajorUpdateOccured();
    }
    public async Task ToggleRightToLeft()
    {
        IsRTL = !IsRTL;
        _userPreferences.RightToLeft = IsRTL;
        await _userPreferencesService.SaveUserPreferences(_userPreferences);
        OnMajorUpdateOccured();
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

    public void SetBaseTheme(MudTheme theme)
    {
        CurrentTheme = theme;
        CurrentTheme.Palette.Primary = PrimaryColor;
        CurrentTheme.PaletteDark.Primary = PrimaryColor;
        CurrentTheme.LayoutProperties.DefaultBorderRadius = BorderRadius + "px";
        OnMajorUpdateOccured();
    }

    public async Task SetPrimaryColor(string color)
    {
        PrimaryColor = color;
        CurrentTheme.Palette.Primary = PrimaryColor;
        _userPreferences.PrimaryColor = PrimaryColor;
        await _userPreferencesService.SaveUserPreferences(_userPreferences);
        OnMajorUpdateOccured();
    }
    public async Task SetSecondaryColor(string color)
    {
        SecondaryColor = color;
        CurrentTheme.Palette.Secondary = SecondaryColor;
        _userPreferences.SecondaryColor = SecondaryColor;
        await _userPreferencesService.SaveUserPreferences(_userPreferences);
        OnMajorUpdateOccured();
    }
    public async Task SetBorderRadius(double size)
    {
        BorderRadius = size;
        CurrentTheme.LayoutProperties.DefaultBorderRadius = BorderRadius + "px";
        _userPreferences.BorderRadius = BorderRadius;
        await _userPreferencesService.SaveUserPreferences(_userPreferences);
        OnMajorUpdateOccured();
    }
    public async Task UpdateUserPreferences(UserPreferences.UserPreferences preferences)
    {
        _userPreferences = preferences;
        IsDarkMode = _userPreferences.DarkLightTheme switch
        {
            DarkLightMode.Dark => true,
            DarkLightMode.Light => false,
            DarkLightMode.System => _systemPreferences=true,
            _ => IsDarkMode
        };
        IsRTL = _userPreferences.RightToLeft;
        PrimaryColor = _userPreferences.PrimaryColor;
        SecondaryColor = _userPreferences.SecondaryColor;
        BorderRadius = _userPreferences.BorderRadius;
        DefaultFontSize = _userPreferences.DefaultFontSize;
        DarkModeToggle = _userPreferences.DarkLightTheme;
        CurrentTheme.Palette.Primary = PrimaryColor;
        CurrentTheme.PaletteDark.Primary = PrimaryColor;
        CurrentTheme.Palette.Secondary = SecondaryColor;
        CurrentTheme.PaletteDark.Secondary = SecondaryColor;
        CurrentTheme.LayoutProperties.DefaultBorderRadius = BorderRadius + "px";
        CurrentTheme.Typography.Default.FontSize = DefaultFontSize+"rem";
        await _userPreferencesService.SaveUserPreferences(_userPreferences);
        OnMajorUpdateOccured();
    }
}
