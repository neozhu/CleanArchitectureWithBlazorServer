using System.Text.Json.Serialization;

namespace MudDemo.Server.Models.Charts.Fill;

public class FillModel
{
    // [JsonPropertyName("colors")] public List<string> Colors { get; set; } = new(); // TODO...
    [JsonPropertyName("opacity")] public double Opacity { get; set; } = 0.9;
    [JsonPropertyName("type")] public string Type { get; set; } = "solid";
    [JsonPropertyName("gradient")] public GradientModel Gradient { get; set; } = new();
    [JsonPropertyName("image")] public ImageModel Image { get; set; } = new();
    [JsonPropertyName("pattern")] public PatternModel Pattern { get; set; } = new();

    public class GradientModel
    {
        [JsonPropertyName("shade")] public string Shade { get; set; } = "dark";
        [JsonPropertyName("type")] public string Type { get; set; } = "horizontal";

        [JsonPropertyName("shadeIntensity")] public double ShadeIntensity { get; set; } = 0.5;

        // [JsonPropertyName("gradientToColors")] public List<string> GradientToColors { get; set; } = new(); // TODO...
        [JsonPropertyName("inverseColors")] public bool InverseColors { get; set; } = true;
        [JsonPropertyName("opacityFrom")] public double OpacityFrom { get; set; } = 1;
        [JsonPropertyName("opacityTo")] public double OpacityTo { get; set; } = 1;

        [JsonPropertyName("stops")] public List<int> Stops { get; set; } = new() {0, 50, 100};
        // [JsonPropertyName("colorStops")] public List<ColorStopModel> ColorStops { get; set; } = new(); // TODO...

        public class ColorStopModel
        {
            [JsonPropertyName("color")] public string Color { get; set; } = "";
            [JsonPropertyName("offset")] public int Offset { get; set; } = 0;
            [JsonPropertyName("opacity")] public double Opacity { get; set; } = 1;
        }
    }

    public class ImageModel
    {
        [JsonPropertyName("src")] public List<string> Src { get; set; } = new();
        [JsonPropertyName("width")] public int Width { get; set; }
        [JsonPropertyName("height")] public int Height { get; set; }
    }

    public class PatternModel
    {
        [JsonPropertyName("style")] public string Style { get; set; } = "verticalLines";
        [JsonPropertyName("width")] public int Width { get; set; } = 6;
        [JsonPropertyName("height")] public int Height { get; set; } = 6;
        [JsonPropertyName("strokeWidth")] public int StrokeWidth { get; set; } = 2;
    }
}