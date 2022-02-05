using System.Text.Json.Serialization;

namespace MudDemo.Server.Models.Charts.Theme;

public class ThemeModel
{
    [JsonPropertyName("mode")] public string Mode { get; set; } = "light";
    [JsonPropertyName("palette")] public string Palette { get; set; } = "palette1";
    [JsonPropertyName("monochrome")] public MonochromeModel Monochrome { get; set; } = new();

    public class MonochromeModel
    {
        [JsonPropertyName("enabled")] public bool Enabled { get; set; } = false;
        [JsonPropertyName("color")] public string Color { get; set; } = "#255aee";
        [JsonPropertyName("shadeTo")] public string ShadeTo { get; set; } = "light";
        [JsonPropertyName("shadeIntensity")] public double ShadeIntensity { get; set; } = 0.65;
    }
}