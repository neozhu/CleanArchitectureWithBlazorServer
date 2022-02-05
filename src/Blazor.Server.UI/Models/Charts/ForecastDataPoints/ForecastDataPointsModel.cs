using System.Text.Json.Serialization;

namespace Blazor.Server.UI.Models.Charts.ForecastDataPoints;

public class ForecastDataPointsModel
{
    [JsonPropertyName("count")] public int Count { get; set; } = 0;
    [JsonPropertyName("fillOpacity")] public double FillOpacity { get; set; } = 0.5;
    [JsonPropertyName("strokeWidth")] public int StrokeWidth { get; set; } = 0;
    [JsonPropertyName("dashArray")] public int DashArray { get; set; } = 4;
}