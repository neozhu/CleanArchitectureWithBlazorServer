using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using MudBlazor.Utilities;
using Blazor.Server.UI.Models.Charts;
using Blazor.Server.UI.Models.Charts.Chart;
using Blazor.Server.UI.Models.Charts.Legend;
using Blazor.Server.UI.Models.Charts.Series;
using Blazor.Server.UI.Models.Charts.XAxis;
using Blazor.Server.UI.Models.Charts.YAxis;

namespace Blazor.Server.UI.Components.Index;

public partial class AreaInstalled : MudComponentBase
{
    private static readonly Dictionary<int, List<SeriesModel<int>>> Series = new()
    {
        {
            2020, new List<SeriesModel<int>>
            {
                new()
                {
                    Name = "Asia",
                    Data = new List<int> {148, 91, 69, 62, 49, 51, 35, 41, 10}
                },
                new()
                {
                    Name = "America",
                    Data = new List<int> {45, 77, 99, 88, 77, 56, 13, 34, 10}
                }
            }
        },
        {
            2019, new List<SeriesModel<int>>
            {
                new()
                {
                    Name = "Asia",
                    Data = new List<int> {10, 41, 35, 51, 49, 62, 69, 91, 148}
                },
                new()
                {
                    Name = "America",
                    Data = new List<int> {10, 34, 13, 56, 77, 88, 99, 77, 45}
                }
            }
        }
    };

    private ChartOptionsModel<SeriesModel<int>, string>? _chartOptions;

    private int _selectedYear = Series.Keys.First();

    private string Classname =>
        new CssBuilder()
            .AddClass(Class)
            .Build();

    [Inject] private IJSRuntime JsRuntime { get; set; }
    
    protected override void OnInitialized()
    {
        _chartOptions = new ChartOptionsModel<SeriesModel<int>, string>
        {
            Chart = new ChartModel
            {
                Type = ChartTypes.Line,
                Toolbar = new ChartModel.ToolbarModel
                {
                    Show = false
                },
                Zoom = new ChartModel.ZoomModel
                {
                    Enabled = false
                },
                Width = "100%",
                Height = "350px",
                Id = "areaInstalled"
            },
            Series = Series[_selectedYear],
            XAxis = new XAxisModel<string>
            {
                Categories = new List<string> {"Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep"},
                AxisBorder = new XAxisModel<string>.AxisBorderModel
                {
                    Show = false
                },
                AxisTicks = new XAxisModel<string>.AxisTicksModel
                {
                    Show = false
                },
                Tooltip = new XAxisModel<string>.TooltipModel
                {
                    Enabled = false
                }
            },
            YAxis = new YAxisModel
            {
                AxisTicks = new YAxisModel.AxisTicksModel
                {
                    Show = false
                },
                AxisBorder = new YAxisModel.AxisBorderModel
                {
                    Show = false
                },
                Tooltip = new YAxisModel.TooltipModel
                {
                    Enabled = false
                },
                Max = 150
            },
            Legend = new LegendModel
            {
                Position = "top",
                HorizontalAlign = "right"
            },
            Colors = new List<string> {"var(--mud-palette-primary)", Colors.Yellow.Default},
        };
    }

    private async Task UpdateSeries(int yearSelected)
    {
        _selectedYear = yearSelected;
        await JsRuntime.InvokeVoidAsync("apex_wrapper.updateSeries", "areaInstalled", Series[_selectedYear]);
    }
}