using System.Text.Json.Serialization;

namespace MudDemo.Server.Models.Charts.NoData;

public class NoDataModel
{
    [JsonPropertyName("text")] public string Text { get; set; } = string.Empty;
    [JsonPropertyName("align")] public string Align { get; set; } = "center";
    [JsonPropertyName("verticalAlign")] public string VerticalAlign { get; set; } = "middle";
    [JsonPropertyName("offsetX")] public int OffsetX { get; set; } = 0;
    [JsonPropertyName("offsetY")] public int OffsetY { get; set; } = 0;
    [JsonPropertyName("style")] public StyleModel Style { get; set; } = new();

    public class StyleModel
    {
        [JsonPropertyName("color")] public string Color { get; set; } = string.Empty;
        [JsonPropertyName("fontSize")] public string FontSize { get; set; } = "14px";
        [JsonPropertyName("fontFamily")] public string FontFamily { get; set; } = string.Empty;
    }
}