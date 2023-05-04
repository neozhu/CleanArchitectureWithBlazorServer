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
        Colors.BlueGrey.Default,
        "#6f42c1",
        Colors.Orange.Default,
        Colors.Red.Default
    };
    private readonly List<string> _primaryDarkColors = new()
    {
        "#8292b4",
        "#689F38",
        "#0A81AB",
        "#546E7A",
        "#8E24AA",
        "#F5B400",
        "#9C2727",
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
    private async Task ToggleDarkLightMode(bool isDarkMode)
    {
        UserPreferences.IsDarkMode = isDarkMode;
        await UserPreferencesChanged.InvokeAsync(UserPreferences);
    }
    public async Task ToggleDarkLightMode(DarkLightMode mode )
    {
        UserPreferences.DarkLightTheme = mode;
        await UserPreferencesChanged.InvokeAsync(UserPreferences);
    }
    private async Task ChangedFontSize(ChangeEventArgs args)
    {
        UserPreferences.DefaultFontSize = double.Parse(args?.Value?.ToString() ?? "0");
        await UserPreferencesChanged.InvokeAsync(UserPreferences);
    }
    
}