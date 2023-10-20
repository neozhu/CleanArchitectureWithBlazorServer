using ApexCharts;
using CleanArchitecture.Blazor.Application.Features.Loggers.DTOs;
using CleanArchitecture.Blazor.Server.UI.Services.Layout;

namespace CleanArchitecture.Blazor.Server.UI.Pages.SystemManagement.Components;
public partial class LogsLineCharts
{
    private ApexChart<LogTimeLineDto>? Chart { get; set; }
    private ApexChartOptions<LogTimeLineDto> Options { get; set; } = new();
    [EditorRequired][Parameter] public List<LogTimeLineDto> Data { get; set; } = new();
    [Inject]
    private LayoutService LayoutService { get; set; } = null!;
    public string BackgroundColor = "background:#fff";
    protected override void OnInitialized()
    {
        var isDarkMode = LayoutService.IsDarkMode;
        var colors = isDarkMode ? new List<string>() { "#0277BD", "#039BE5" } : new List<string>() { "#1976D2", "#90CAF9" };
        BackgroundColor = isDarkMode ? "background:rgb(66, 66, 66)" : "background:#fff";

        Options.Theme = new ApexCharts.Theme()
        {
            Mode = isDarkMode ? Mode.Dark : Mode.Light,
        };

        Options.Fill = new Fill
        {
            Type = new List<FillType> { FillType.Gradient },
            Gradient = new FillGradient
            {
                Type = GradientType.Vertical,
                ShadeIntensity = 1,
                OpacityFrom = 1,
                OpacityTo = 0.7,
                GradientToColors = colors,
                Stops = new List<double>() { 0, 100 }
            },

        };
        Options.Yaxis = new List<YAxis>();
        Options.Yaxis.Add(new YAxis
        {
            Labels = new YAxisLabels
            {
                Formatter = @"function (value) {return Number(value).toLocaleString();}"
            }
        }
        );
        Options.Xaxis = new XAxis
        {
            Labels = new XAxisLabels
            {
                Formatter = @"function (value) {
                    if (value === undefined) {return '';}
                    return '';}"
            }
        };

        Options.DataLabels = new DataLabels
        {
            Formatter = @"function(value, opts) {
                   return  Number(value).toLocaleString();}"
        };

        Options.Tooltip = new ApexCharts.Tooltip
        {
            X = new TooltipX
            {
                Formatter = @"function(value, opts) {
                    if (value === undefined) {return '';}
                    return  value}"
            }
        };

        LayoutService.MajorUpdateOccured += LayoutServiceOnMajorUpdateOccured;
    }
    private async void LayoutServiceOnMajorUpdateOccured(object? sender, EventArgs e)
    {
        var isDarkMode = LayoutService.IsDarkMode;
        var colors = isDarkMode ? new List<string>() { "#0277BD", "#039BE5" } : new List<string>() { "#1976D2", "#90CAF9" };
        BackgroundColor = isDarkMode ? "background:rgb(66, 66, 66)" : "background:#fff";
        Options.Theme.Mode = isDarkMode ? Mode.Dark : Mode.Light;
        Options.Fill.Gradient.GradientToColors = colors;
        await Chart!.UpdateOptionsAsync(true, true, false);
        await Chart!.RenderAsync();
        StateHasChanged();
    }
    public async Task RenderChart()
    {
        await Chart!.UpdateSeriesAsync(true);
        await Chart!.RenderAsync();
    }
    public void Dispose()
    {
        LayoutService.MajorUpdateOccured -= LayoutServiceOnMajorUpdateOccured;
        GC.SuppressFinalize(this);
    }

}