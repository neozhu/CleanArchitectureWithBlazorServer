using MudBlazor.Utilities;

namespace CleanArchitecture.Blazor.Server.UI.Components.Shared;
public partial class LandingSection
{
    [Parameter] public RenderFragment Stripes { get; set; } = null!;
    [Parameter] public RenderFragment ChildContent { get; set; } = null!;
    [Parameter] public bool Straight { get; set; }
    [Parameter] public bool StraightEnd { get; set; }
    [Parameter] public string SectionClass { get; set; } = null!;
    [Parameter] public string BackgroundClass { get; set; } = null!;
    [Parameter] public string Class { get; set; } = null!;

    protected string SectionClassnames =>
        new CssBuilder("mud-landingpage-section")
          .AddClass(SectionClass)
        .Build();

    protected string BackgroundClassnames =>
        new CssBuilder("section-background d-flex flex-column justify-end")
          .AddClass("straight", Straight)
          .AddClass("skew", !Straight)
          .AddClass(BackgroundClass)
        .Build();

    protected string EndBackgroundClassnames =>
        new CssBuilder("section-background straight-end")
          .AddClass(BackgroundClass)
        .Build();

    protected string ContainerClassnames =>
        new CssBuilder("section-container")
          .AddClass("padding-straight", Straight)
          .AddClass("padding-skew", !Straight)
          .AddClass(Class)
        .Build();
}