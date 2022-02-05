using System.Text.Json.Serialization;

namespace Blazor.Server.UI.Models.Charts.Legend;

public class LegendModel
{
    [JsonPropertyName("show")] public bool Show { get; set; } = true;

    [JsonPropertyName("showForSingleSeries")]
    public bool ShowForSingleSeries { get; set; } = false;

    [JsonPropertyName("showForNullSeries")]
    public bool ShowForNullSeries { get; set; } = true;

    [JsonPropertyName("showForZeroSeries")]
    public bool ShowForZeroSeries { get; set; } = true;

    [JsonPropertyName("position")] public string Position { get; set; } = "bottom";
    [JsonPropertyName("horizontalAlign")] public string HorizontalAlign { get; set; } = "center";
    [JsonPropertyName("floating")] public bool Floating { get; set; } = false;
    [JsonPropertyName("fontSize")] public string FontSize { get; set; } = "14px";
    [JsonPropertyName("fontFamily")] public string FontFamily { get; set; } = "Helvetica, Arial";

    [JsonPropertyName("fontWeight")] public int FontWeight { get; set; } = 400;

    // TODO: Add formatter.
    [JsonPropertyName("inverseOrder")] public bool InverseOrder { get; set; } = false;
    [JsonPropertyName("width")] public int Width { get; set; }

    [JsonPropertyName("height")] public int Height { get; set; }

    // TODO: Add tooltip formatter.
    // TODO: Add custom legend.
    [JsonPropertyName("offsetX")] public int OffsetX { get; set; } = 0;
    [JsonPropertyName("offsetY")] public int OffsetY { get; set; } = 0;
    [JsonPropertyName("labels")] public LabelsModel Labels { get; set; } = new();
    [JsonPropertyName("markers")] public MarkersModel Markers { get; set; } = new();
    [JsonPropertyName("itemMargin")] public ItemMarginModel ItemMargin { get; set; } = new();
    [JsonPropertyName("onItemClick")] public OnItemClickModel OnItemClick { get; set; } = new();
    [JsonPropertyName("onItemHover")] public OnItemHoverModel OnItemHover { get; set; } = new();

    public class LabelsModel
    {
        [JsonPropertyName("colors")] public List<string>? Colors { get; set; } // TODO...
        [JsonPropertyName("useSeriesColors")] public bool UseSeriesColors { get; set; } = false;
    }

    public class MarkersModel
    {
        [JsonPropertyName("width")] public int Width { get; set; } = 12;
        [JsonPropertyName("height")] public int Height { get; set; } = 12;
        [JsonPropertyName("strokeWidth")] public int StrokeWidth { get; set; } = 0;
        [JsonPropertyName("strokeColor")] public string StrokeColor { get; set; } = "#fff";
        [JsonPropertyName("fillColors")] public List<string> FillColors { get; set; } = new(); // TODO...

        [JsonPropertyName("radius")] public int Radius { get; set; } = 12;

        // TODO: Add custom HTML.
        // TODO: Add onclick.
        [JsonPropertyName("offsetX")] public int OffsetX { get; set; } = 0;
        [JsonPropertyName("offsetY")] public int OffsetY { get; set; } = 0;
    }

    public class ItemMarginModel
    {
        [JsonPropertyName("horizontal")] public int Horizontal { get; set; } = 5;
        [JsonPropertyName("vertical")] public int Vertical { get; set; } = 0;
    }

    public class OnItemClickModel
    {
        [JsonPropertyName("toggleDataSeries")] public bool ToggleDataSeries { get; set; } = true;
    }

    public class OnItemHoverModel
    {
        [JsonPropertyName("highlightDataSeries")]
        public bool HighlightDataSeries { get; set; } = true;
    }
}