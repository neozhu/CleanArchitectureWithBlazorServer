using System.Text.Json.Serialization;

namespace MudDemo.Server.Models.Charts.YAxis;

public class YAxisModel
{
    [JsonPropertyName("show")] public bool Show { get; set; } = true;
    [JsonPropertyName("showAlways")] public bool ShowAlways { get; set; } = true;

    [JsonPropertyName("showForNullSeries")]
    public bool ShowForNullSeries { get; set; } = true;

    [JsonPropertyName("seriesName")] public string SeriesName { get; set; } = string.Empty;
    [JsonPropertyName("opposite")] public bool Opposite { get; set; } = false;
    [JsonPropertyName("reversed")] public bool Reversed { get; set; } = false;
    [JsonPropertyName("logarithmic")] public bool Logarithmic { get; set; } = false;
    [JsonPropertyName("tickAmount")] public int TickAmount { get; set; } = 6;

    [JsonPropertyName("min")] public int Min { get; set; } = 0;

    [JsonPropertyName("max")] public int Max { get; set; } = 0; // TODO...
    [JsonPropertyName("forceNiceScale")] public bool ForceNiceScale { get; set; } = false;
    [JsonPropertyName("floating")] public bool Floating { get; set; } = false;
    [JsonPropertyName("decimalsInFloat")] public int DecimalsInFloat { get; set; }
    [JsonPropertyName("labels")] public LabelsModel Labels { get; set; } = new();
    [JsonPropertyName("axisBorder")] public AxisBorderModel AxisBorder { get; set; } = new();
    [JsonPropertyName("axisTicks")] public AxisTicksModel AxisTicks { get; set; } = new();
    [JsonPropertyName("title")] public TitleModel Title { get; set; } = new();
    [JsonPropertyName("crosshairs")] public CrosshairsModel Crosshairs { get; set; } = new();
    [JsonPropertyName("tooltip")] public TooltipModel Tooltip { get; set; } = new();

    public class LabelsModel
    {
        [JsonPropertyName("show")] public bool Show { get; set; } = true;
        // [JsonPropertyName("align")] public string Align { get; set; } = "right"; // TODO: This property is causing issue (chart is pushed on the right x-axis)
        [JsonPropertyName("minWidth")] public int MinWidth { get; set; } = 0;
        [JsonPropertyName("maxWidth")] public int MaxWidth { get; set; } = 160;
        [JsonPropertyName("style")] public StyleModel Style { get; set; } = new();
        [JsonPropertyName("offsetX")] public int OffsetX { get; set; } = 0;
        [JsonPropertyName("offsetY")] public int OffsetY { get; set; } = 0;
        [JsonPropertyName("rotate")] public int Rotate { get; set; } = 0;
        // TODO: Add formatter

        public class StyleModel
        {
            [JsonPropertyName("colors")] public string Colors { get; set; } = "var(--mud-palette-text-primary)"; // TODO...
            [JsonPropertyName("fontSize")] public string FontSize { get; set; } = "12px";
            [JsonPropertyName("fontFamily")] public string FontFamily { get; set; } = "Helvetica, Arial, sans-serif";
            [JsonPropertyName("fontWeight")] public string FontWeight { get; set; } = "400";
            [JsonPropertyName("cssClass")] public string CssClass { get; set; } = "apexcharts-yaxis-label";
        }
    }

    public class AxisBorderModel
    {
        [JsonPropertyName("show")] public bool Show { get; set; } = true;
        [JsonPropertyName("color")] public string Color { get; set; } = "#78909C";
        [JsonPropertyName("offsetX")] public int OffsetX { get; set; } = 0;
        [JsonPropertyName("offsetY")] public int OffsetY { get; set; } = 0;
    }

    public class AxisTicksModel
    {
        [JsonPropertyName("show")] public bool Show { get; set; } = true;
        [JsonPropertyName("borderType")] public string BorderType { get; set; } = "solid";
        [JsonPropertyName("color")] public string Color { get; set; } = "#78909C";
        [JsonPropertyName("width")] public int Width { get; set; } = 6;
        [JsonPropertyName("offsetX")] public int OffsetX { get; set; } = 0;
        [JsonPropertyName("offsetY")] public int OffsetY { get; set; } = 0;
    }

    public class TitleModel
    {
        [JsonPropertyName("text")] public string Text { get; set; } = string.Empty;
        [JsonPropertyName("rotate")] public int Rotate { get; set; } = -90;
        [JsonPropertyName("offsetX")] public int OffsetX { get; set; } = 0;
        [JsonPropertyName("offsetY")] public int OffsetY { get; set; } = 0;
        [JsonPropertyName("style")] public StyleModel Style { get; set; } = new();

        public class StyleModel
        {
            [JsonPropertyName("color")] public string Color { get; set; } = string.Empty;
            [JsonPropertyName("fontSize")] public string FontSize { get; set; } = "12px";
            [JsonPropertyName("fontFamily")] public string FontFamily { get; set; } = "Helvetica, Arial, sans-serif";
            [JsonPropertyName("fontWeight")] public string FontWeight { get; set; } = "600";
            [JsonPropertyName("cssClass")] public string CssClass { get; set; } = "apexcharts-yaxis-title";
        }
    }

    public class CrosshairsModel
    {
        [JsonPropertyName("show")] public bool Show { get; set; } = true;
        [JsonPropertyName("position")] public string Position { get; set; } = "back";
        [JsonPropertyName("stroke")] public StrokeModel Stroke { get; set; } = new();

        public class StrokeModel
        {
            [JsonPropertyName("color")] public string Color { get; set; } = "#b6b6b6";
            [JsonPropertyName("width")] public int Width { get; set; } = 1;
            [JsonPropertyName("dashArray")] public int DashArray { get; set; } = 0;
        }
    }

    public class TooltipModel
    {
        [JsonPropertyName("enabled")] public bool Enabled { get; set; } = true;
        [JsonPropertyName("offsetX")] public int OffsetX { get; set; } = 0;
    }
}