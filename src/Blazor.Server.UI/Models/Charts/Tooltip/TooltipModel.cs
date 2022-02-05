using System.Text.Json.Serialization;

namespace MudDemo.Server.Models.Charts.Tooltip;

public class TooltipModel
{
    [JsonPropertyName("enabled")] public bool Enabled { get; set; } = true;

    // TODO: EnabledOnSeries.
    [JsonPropertyName("shared")] public bool Shared { get; set; } = true;
    [JsonPropertyName("followCursor")] public bool FollowCursor { get; set; } = false;
    [JsonPropertyName("intersect")] public bool Intersect { get; set; } = false;

    [JsonPropertyName("inverseOrder")] public bool InverseOrder { get; set; } = false;

    // TODO: Custom.
    [JsonPropertyName("fillSeriesColor")] public bool FillSeriesColor { get; set; } = false;
    [JsonPropertyName("theme")] public bool Theme { get; set; } = false;
    [JsonPropertyName("style")] public StyleModel Style { get; set; } = new();
    [JsonPropertyName("onDatasetHover")] public OnDatasetHoverModel OnDatasetHover { get; set; } = new();
    [JsonPropertyName("x")] public XModel X { get; set; } = new();
    [JsonPropertyName("y")] public YModel Y { get; set; } = new();
    [JsonPropertyName("z")] public ZModel Z { get; set; } = new();
    [JsonPropertyName("marker")] public MarkerModel Marker { get; set; } = new();
    [JsonPropertyName("items")] public ItemsModel Items { get; set; } = new();
    [JsonPropertyName("fixed")] public FixedModel Fixed { get; set; } = new();

    public class StyleModel
    {
        [JsonPropertyName("fontSize")] public string FontSize { get; set; } = "12px";
        [JsonPropertyName("fontFamily")] public string FontFamily { get; set; } = string.Empty;
    }

    public class OnDatasetHoverModel
    {
        [JsonPropertyName("highlightDataSeries")]
        public bool HighlightDataSeries { get; set; } = false;
    }

    public class XModel
    {
        [JsonPropertyName("show")] public bool Show { get; set; } = true;

        [JsonPropertyName("format")] public string Format { get; set; } = "dd MMM";
        // TODO: Formatter.
    }

    public class YModel
    {
        // TODO: Formatter.
        [JsonPropertyName("title")] public TitleModel Title { get; set; } = new();

        public class TitleModel
        {
            // TODO: Formatter.
        }
    }

    public class ZModel
    {
        // TODO: Formatter.
        [JsonPropertyName("title")] public string Title { get; set; } = "Size: ";
    }

    public class MarkerModel
    {
        [JsonPropertyName("show")] public bool Show { get; set; } = true;
    }

    public class ItemsModel
    {
        [JsonPropertyName("display")] public string Display { get; set; } = "flex";
    }

    public class FixedModel
    {
        [JsonPropertyName("enabled")] public bool Enabled { get; set; } = false;
        [JsonPropertyName("position")] public string Position { get; set; } = "topRight";
        [JsonPropertyName("offsetX")] public double OffsetX { get; set; } = 0;
        [JsonPropertyName("offsetY")] public double OffsetY { get; set; } = 0;
    }
}