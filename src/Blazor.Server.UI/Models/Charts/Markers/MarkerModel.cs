using System.Text.Json.Serialization;

namespace MudDemo.Server.Models.Charts.Markers;

public class MarkerModel
{
    [JsonPropertyName("size")] public int Size { get; set; } = 0;
    [JsonPropertyName("colors")] public List<string> Colors { get; set; } = new(); // TODO...
    [JsonPropertyName("strokeColors")] public string StrokeColors { get; set; } = "#fff";
    [JsonPropertyName("strokeWidth")] public int StrokeWidth { get; set; } = 2;
    [JsonPropertyName("strokeOpacity")] public double StrokeOpacity { get; set; } = 0.9;
    [JsonPropertyName("strokeDashArray")] public int StrokeDashArray { get; set; } = 0;

    [JsonPropertyName("fillOpacity")] public double FillOpacity { get; set; } = 1;

    // TODO: Add discrete.
    [JsonPropertyName("shape")] public string Shape { get; set; } = "circle";
    [JsonPropertyName("radius")] public int Radius { get; set; } = 2;
    [JsonPropertyName("offsetX")] public int OffsetX { get; set; } = 0;
    [JsonPropertyName("offsetY")] public int OffsetY { get; set; } = 0;

    // TODO: Add onclick.
    // TODO: Add onDblClick.

    [JsonPropertyName("showNullDataPoints")]
    public bool ShowNullDataPoints { get; set; } = true;

    [JsonPropertyName("hover")] public HoverModel Hover { get; set; } = new();

    public class HoverModel
    {
        [JsonPropertyName("size")] public int Size { get; set; }
        [JsonPropertyName("sizeOffset")] public int SizeOffset { get; set; } = 3;
    }
}