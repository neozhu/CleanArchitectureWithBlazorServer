using System.Text.Json.Serialization;

namespace MudDemo.Server.Models.Charts.Stroke;

public class StrokeModel
{
    [JsonPropertyName("show")] public bool Show { get; set; } = true;
    [JsonPropertyName("curve")] public string Curve { get; set; } = "smooth";
    [JsonPropertyName("lineCap")] public string LineCap { get; set; } = "butt";
    //[JsonPropertyName("colors")] public List<string> Colors { get; set; } // TODO...
    [JsonPropertyName("width")] public int Width { get; set; } = 2;
    [JsonPropertyName("dashArray")] public int DashArray { get; set; } = 0;
}