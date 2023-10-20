using CleanArchitecture.Blazor.Server.UI.Services.Layout;

namespace CleanArchitecture.Blazor.Server.UI.Components.Shared;
public partial class LoadingScreen
{

    [Parameter]
    public RenderFragment ChildContent { get; set; } = null!;

    [Parameter]
    public LayoutService LayoutService { get; set; } = null!;

    [Parameter]
    public bool IsLoaded { get; set; }

    private string Gradient => $"background-image: linear-gradient(-120deg, {(LayoutService.IsDarkMode ? LayoutService.CurrentTheme.PaletteDark.Background : LayoutService.CurrentTheme.Palette.Background)} 50%, {(LayoutService.IsDarkMode ? LayoutService.CurrentTheme.PaletteDark.Surface : LayoutService.CurrentTheme.Palette.Surface)} 50%);";

}