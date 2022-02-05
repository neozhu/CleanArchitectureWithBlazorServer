using System.Text.Json.Serialization;
using Blazor.Server.UI.Models.Charts.Annotations;
using Blazor.Server.UI.Models.Charts.Chart;
using Blazor.Server.UI.Models.Charts.DataLabels;
using Blazor.Server.UI.Models.Charts.Fill;
using Blazor.Server.UI.Models.Charts.ForecastDataPoints;
using Blazor.Server.UI.Models.Charts.Grid;
using Blazor.Server.UI.Models.Charts.Legend;
using Blazor.Server.UI.Models.Charts.NoData;
using Blazor.Server.UI.Models.Charts.PlotOptions;
using Blazor.Server.UI.Models.Charts.Responsive;
using Blazor.Server.UI.Models.Charts.Series;
using Blazor.Server.UI.Models.Charts.States;
using Blazor.Server.UI.Models.Charts.Stroke;
using Blazor.Server.UI.Models.Charts.Subtitle;
using Blazor.Server.UI.Models.Charts.Theme;
using Blazor.Server.UI.Models.Charts.Title;
using Blazor.Server.UI.Models.Charts.Tooltip;
using Blazor.Server.UI.Models.Charts.XAxis;
using Blazor.Server.UI.Models.Charts.YAxis;

namespace Blazor.Server.UI.Models.Charts;

public class ChartOptionsModel<TSeries, TXAxis>
{
    [JsonPropertyName("annotations")] public AnnotationsModel Annotations { get; set; } = new();
    [JsonPropertyName("chart")] public ChartModel Chart { get; set; } = new();
    //
    [JsonPropertyName("colors")]
    public List<string> Colors { get; set; } = new() {"var(--mud-palette-primary)", "var(--mud-palette-secondary)"};
    
    [JsonPropertyName("dataLabels")] public DataLabelsModel DataLabels { get; set; } = new();
    [JsonPropertyName("fill")] public FillModel Fill { get; set; } = new();
    
    [JsonPropertyName("forecastDataPoints")]
    public ForecastDataPointsModel ForecastDataPoints { get; set; } = new();

    // [JsonPropertyName("grid")] public GridModel Grid { get; set; } = new(); // TODO: Yaxis issue...
    [JsonPropertyName("labels")] public List<string> Labels { get; set; } = new();
    [JsonPropertyName("legend")] public LegendModel Legend { get; set; } = new();
    [JsonPropertyName("markers")] public LegendModel.MarkersModel Markers { get; set; } = new();
    [JsonPropertyName("noData")] public NoDataModel NoData { get; set; } = new();
    [JsonPropertyName("plotOptions")] public PlotOptionsModel PlotOptions { get; set; } = new();
    [JsonPropertyName("responsive")] public List<ResponsiveModel> Responsive { get; set; } = new();
    [JsonPropertyName("series")] public List<TSeries> Series { get; set; } = new();
    [JsonPropertyName("states")] public StatesModel States { get; set; } = new();
    [JsonPropertyName("stroke")] public StrokeModel Stroke { get; set; } = new();
    [JsonPropertyName("subtitle")] public SubtitleModel Subtitle { get; set; } = new();
    [JsonPropertyName("theme")] public ThemeModel Theme { get; set; } = new();
    [JsonPropertyName("title")] public TitleModel Title { get; set; } = new();
    [JsonPropertyName("tooltip")] public TooltipModel Tooltip { get; set; } = new();
    [JsonPropertyName("xaxis")] public XAxisModel<TXAxis> XAxis { get; set; } = new();
    [JsonPropertyName("yaxis")] public YAxisModel YAxis { get; set; } = new();
}