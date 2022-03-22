using MudBlazor;

namespace Blazor.Server.UI.Services;

public class LayoutService
{
    private readonly IUserPreferencesService _userPreferencesService;
    private UserPreferences _userPreferences=new();

    public bool IsRTL { get; private set; } = false;
    public bool IsDarkMode { get; private set; } = false;
    public string PrimaryColor { get; set; } = "#2d4275";
    public string SecondaryColor { get; set; } = "#ff4081ff";
    public double BorderRadius { get; set; } = 4;

    public MudTheme CurrentTheme { get; private set; }


    public LayoutService(IUserPreferencesService userPreferencesService)
    {
        _userPreferencesService = userPreferencesService;
    }

    public void SetDarkMode(bool value)
    {
        IsDarkMode = value;
    }

    public async Task<UserPreferences> ApplyUserPreferences(bool isDarkModeDefaultTheme)
    {
        _userPreferences = await _userPreferencesService.LoadUserPreferences();
        if (_userPreferences != null)
        {
            IsDarkMode = _userPreferences.IsDarkMode;
            IsRTL = _userPreferences.RightToLeft;
            PrimaryColor = _userPreferences.PrimaryColor;
            SecondaryColor = _userPreferences.SecondaryColor;
            BorderRadius = _userPreferences.BorderRadius;
            CurrentTheme.Palette.Primary = PrimaryColor;
            CurrentTheme.PaletteDark.Primary = PrimaryColor;
            CurrentTheme.Palette.Secondary = SecondaryColor;
            CurrentTheme.PaletteDark.Secondary = SecondaryColor;
            CurrentTheme.LayoutProperties.DefaultBorderRadius = BorderRadius + "px";
        }
        else
        {
            IsDarkMode = isDarkModeDefaultTheme;
            _userPreferences = new UserPreferences { IsDarkMode = IsDarkMode };
            await _userPreferencesService.SaveUserPreferences(_userPreferences);
        }
        return _userPreferences;
    }

    public event EventHandler MajorUpdateOccured;

    private void OnMajorUpdateOccured() => MajorUpdateOccured?.Invoke(this, EventArgs.Empty);

    public async Task ToggleDarkMode()
    {
        IsDarkMode = !IsDarkMode;
        _userPreferences.IsDarkMode = IsDarkMode;
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
    public async Task UpdateUserPreferences(UserPreferences preferences)
    {
        _userPreferences = preferences;
        IsDarkMode = _userPreferences.IsDarkMode;
        IsRTL = _userPreferences.RightToLeft;
        PrimaryColor = _userPreferences.PrimaryColor;
        SecondaryColor = _userPreferences.SecondaryColor;
        BorderRadius = _userPreferences.BorderRadius;
        CurrentTheme.Palette.Primary = PrimaryColor;
        CurrentTheme.PaletteDark.Primary = PrimaryColor;
        CurrentTheme.Palette.Secondary = SecondaryColor;
        CurrentTheme.PaletteDark.Secondary = SecondaryColor;
        CurrentTheme.LayoutProperties.DefaultBorderRadius = BorderRadius + "px";
        await _userPreferencesService.SaveUserPreferences(_userPreferences);
        OnMajorUpdateOccured();
    }
}
