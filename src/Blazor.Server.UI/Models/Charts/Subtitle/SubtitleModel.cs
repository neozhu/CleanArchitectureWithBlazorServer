using System.Text.Json.Serialization;

namespace Blazor.Server.UI.Models.Charts.Subtitle;

public class SubtitleModel
{
    [JsonPropertyName("text")] public string Text { get; set; } = string.Empty;
    [JsonPropertyName("align")] public string Align { get; set; } = "left";
    [JsonPropertyName("margin")] public int Margin { get; set; } = 10;
    [JsonPropertyName("offsetX")] public int OffsetX { get; set; } = 0;
    [JsonPropertyName("offsetY")] public int OffsetY { get; set; } = 0;
    [JsonPropertyName("floating")] public bool Floating { get; set; } = false;
    [JsonPropertyName("style")] public StyleModel Style { get; set; } = new();

    public class StyleModel
    {
        [JsonPropertyName("fontSize")] public string FontSize { get; set; } = "12px";
        [JsonPropertyName("fontWeight")] public string FontWeight { get; set; } = "normal";
        [JsonPropertyName("fontFamily")] public string FontFamily { get; set; } = string.Empty;
        [JsonPropertyName("color")] public string Color { get; set; } = "#9699a2";
    }
}