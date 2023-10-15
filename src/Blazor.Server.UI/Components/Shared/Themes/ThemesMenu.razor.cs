using System.Globalization;
using Blazor.Server.UI.Services;
using Blazor.Server.UI.Services.Layout;
using Blazor.Server.UI.Services.UserPreferences;

namespace Blazor.Server.UI.Components.Shared.Themes;

public partial class ThemesMenu
{
    // TODO: The razor file could probably be cleaner..
    // TODO: Find why MudOverlay does not work..
    // TODO: Better CSS..

    private readonly List<string> _primaryColors = new()
    {
        "#2d4275",
        Colors.Green.Default,
        "#0576b9",
        "#8421d1",
        "#FF7F00",
        Colors.Red.Default
    };
    private readonly List<string> _primaryDarkColors = new()
    {
        "#8b9ac6",
        "#6c9f77",
        "#79a5d1",
        "#b194d7",
        "#ffc27f",
        "#f88989",
    };
    private List<string> GetColorDefinition()
    {
        return UserPreferences.DarkLightTheme switch
        {
            DarkLightMode.Dark => _primaryDarkColors,
            DarkLightMode.Light => _primaryColors,
            DarkLightMode.System => UserPreferences.IsDarkMode ? _primaryDarkColors : _primaryColors,
            _ => UserPreferences.IsDarkMode ? _primaryDarkColors : _primaryColors
        };
    }

    [EditorRequired] [Parameter] public bool ThemingDrawerOpen { get; set; }
    [EditorRequired] [Parameter] public EventCallback<bool> ThemingDrawerOpenChanged { get; set; }
    [EditorRequired] [Parameter] public UserPreferences UserPreferences { get; set; }=new();
    [EditorRequired] [Parameter] public EventCallback<UserPreferences> UserPreferencesChanged { get; set; }
    
    [Inject] private LayoutService LayoutService { get; set; } = default!;

    [Parameter]
    public double Radius { get; set; }

    [Parameter]
    public double MaxValue { get; set; } = 32;

    private async Task UpdateThemePrimaryColor(string color)
    {
        UserPreferences.PrimaryColor = color;
        await UserPreferencesChanged.InvokeAsync(UserPreferences);
    }
    private async Task ChangedSelection(ChangeEventArgs args)
    {
        UserPreferences.BorderRadius = double.Parse(args?.Value?.ToString() ?? "0");
        await UserPreferencesChanged.InvokeAsync(UserPreferences);
    }
    public async Task ToggleDarkLightMode(DarkLightMode mode)
    {
        UserPreferences.DarkLightTheme = mode;
        if (mode == DarkLightMode.Dark)
        {
            if (_primaryColors.IndexOf(UserPreferences.PrimaryColor) >= 0)
            {
                UserPreferences.PrimaryColor = _primaryDarkColors[_primaryColors.IndexOf(UserPreferences.PrimaryColor)];
            }
        }
        else if (mode == DarkLightMode.Light)
        {
            if (_primaryDarkColors.IndexOf(UserPreferences.PrimaryColor) >= 0)
            {
                UserPreferences.PrimaryColor = _primaryColors[_primaryDarkColors.IndexOf(UserPreferences.PrimaryColor)];
            }
        }
        await UserPreferencesChanged.InvokeAsync(UserPreferences);
    }
    private async Task ChangedFontSize(ChangeEventArgs args)
    {
        UserPreferences.DefaultFontSize = double.Parse(args?.Value?.ToString() ?? "0", NumberStyles.Float, CultureInfo.InvariantCulture);
        await UserPreferencesChanged.InvokeAsync(UserPreferences);
    }
}
