using System.Text.Json.Serialization;

namespace Blazor.Server.UI.Models.Charts.DataLabels;

public class DataLabelsModel
{
    [JsonPropertyName("enabled")] public bool Enabled { get; set; } = true;

    [JsonPropertyName("enabledOnSeries")] public List<int> EnabledOnSeries { get; set; } = new(); // TODO...

    // TODO: Add formatter.
    [JsonPropertyName("textAnchor")] public string TextAnchor { get; set; } = "middle";
    [JsonPropertyName("distributed")] public bool Distributed { get; set; } = false;
    [JsonPropertyName("offsetX")] public int OffsetX { get; set; } = 0;
    [JsonPropertyName("offsetY")] public int OffsetY { get; set; } = 0;
    [JsonPropertyName("style")] public StyleModel Style { get; set; } = new();
    [JsonPropertyName("background")] public BackgroundModel Background { get; set; } = new();
    [JsonPropertyName("dropShadow")] public DropShadowModel DropShadow { get; set; } = new();


    public class StyleModel
    {
        [JsonPropertyName("fontSize")] public string FontSize { get; set; } = "14px";
        [JsonPropertyName("fontFamily")] public string FontFamily { get; set; } = "Helvetica, Arial, sans-serif";
        [JsonPropertyName("fontWeight")] public string FontWeight { get; set; } = "bold";
        [JsonPropertyName("colors")] public List<string> Colors { get; set; } = new(); // TODO...
    }

    public class BackgroundModel
    {
        [JsonPropertyName("enabled")] public bool Enabled { get; set; } = true;
        [JsonPropertyName("foreColor")] public string ForeColor { get; set; } = "#fff";
        [JsonPropertyName("padding")] public int Padding { get; set; } = 4;
        [JsonPropertyName("borderRadius")] public int BorderRadius { get; set; } = 2;
        [JsonPropertyName("borderWidth")] public int BorderWidth { get; set; } = 1;
        [JsonPropertyName("borderColor")] public string BorderColor { get; set; } = "#fff";
        [JsonPropertyName("opacity")] public double Opacity { get; set; } = 0.9;
        [JsonPropertyName("dropShadow")] public DropShadowModel DropShadow { get; set; } = new();

        public class DropShadowModel
        {
            [JsonPropertyName("enabled")] public bool Enabled { get; set; } = false;
            [JsonPropertyName("top")] public int Top { get; set; } = 1;
            [JsonPropertyName("left")] public int Left { get; set; } = 1;
            [JsonPropertyName("blur")] public double Blur { get; set; } = 1;
            [JsonPropertyName("color")] public string Color { get; set; } = "#000";
            [JsonPropertyName("opacity")] public double Opacity { get; set; } = 0.45;
        }
    }

    public class DropShadowModel
    {
        [JsonPropertyName("enabled")] public bool Enabled { get; set; } = false;
        [JsonPropertyName("top")] public int Top { get; set; } = 1;
        [JsonPropertyName("left")] public int Left { get; set; } = 1;
        [JsonPropertyName("blur")] public double Blur { get; set; } = 1;
        [JsonPropertyName("color")] public string Color { get; set; } = "#000";
        [JsonPropertyName("opacity")] public double Opacity { get; set; } = 0.45;
    }
}