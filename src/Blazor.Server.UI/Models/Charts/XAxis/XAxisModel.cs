using System.Text.Json.Serialization;

namespace Blazor.Server.UI.Models.Charts.XAxis;

public class XAxisModel<TCategory>
{
    [JsonPropertyName("type")] public string Type { get; set; } = "category";

    [JsonPropertyName("categories")] public List<TCategory> Categories { get; set; } = new();

    //[JsonPropertyName("min")] public double Min { get; set; } // TODO...
    //[JsonPropertyName("max")] public double Max { get; set; } // TODO...
    //[JsonPropertyName("range")] public double Range { get; set; } // TODO...
    [JsonPropertyName("floating")] public bool Floating { get; set; } = false;
    //[JsonPropertyName("decimalsInFloat")] public int DecimalsInFloat { get; set; } // TODO...

    [JsonPropertyName("overwriteCategories")]
    public bool OverwriteCategories { get; set; }

    [JsonPropertyName("labels")] public LabelsModel Labels { get; set; } = new();
    [JsonPropertyName("axisBorder")] public AxisBorderModel AxisBorder { get; set; } = new();
    [JsonPropertyName("axisTicks")] public AxisTicksModel AxisTicks { get; set; } = new();
    [JsonPropertyName("title")] public TitleModel Title { get; set; } = new();
    [JsonPropertyName("crosshairs")] public CrosshairsModel Crosshairs { get; set; } = new();
    [JsonPropertyName("tooltip")] public TooltipModel Tooltip { get; set; } = new();


    public class LabelsModel
    {
        [JsonPropertyName("show")] public bool Show { get; set; } = true;
        [JsonPropertyName("rotate")] public int Rotate { get; set; } = 45;
        [JsonPropertyName("rotateAlways")] public bool RotateAlways { get; set; } = false;

        [JsonPropertyName("hideOverlappingLabels")]
        public bool HideOverlappingLabels { get; set; } = true;

        [JsonPropertyName("showDuplicates")] public bool ShowDuplicates { get; set; } = false;

        [JsonPropertyName("trim")] public bool Trim { get; set; } = false;

        // [JsonPropertyName("minHeight")] public double MinHeight { get; set; }
        //[JsonPropertyName("maxHeight")] public double MaxHeight { get; set; } = 120;
        [JsonPropertyName("style")] public StyleModel Style { get; set; } = new();
        [JsonPropertyName("offsetX")] public int OffsetX { get; set; } = 0;
        [JsonPropertyName("offsetY")] public int OffsetY { get; set; } = 0;

        [JsonPropertyName("format")] public string Format { get; set; } = string.Empty;

        // TODO: Add formatter.
        [JsonPropertyName("datetimeUTC")] public bool DatetimeUTC { get; set; } = true;

        [JsonPropertyName("datetimeFormatter")]
        public DatetimeFormatterModel DatetimeFormatter { get; set; } = new();

        public class StyleModel
        {
            [JsonPropertyName("colors")] public string Colors { get; set; } = "var(--mud-palette-text-primary)"; // TODO...
            [JsonPropertyName("fontSize")] public string FontSize { get; set; } = "12px";
            [JsonPropertyName("fontFamily")] public string FontFamily { get; set; } = "Helvetica, Arial, sans-serif";
            [JsonPropertyName("fontWeight")] public string FontWeight { get; set; } = "400";
            [JsonPropertyName("cssClass")] public string CssClass { get; set; } = "apexcharts-xaxis-label";
        }

        public class DatetimeFormatterModel
        {
            [JsonPropertyName("year")] public string Year { get; set; } = "yyyy";
            [JsonPropertyName("month")] public string Month { get; set; } = "MMM 'yy";
            [JsonPropertyName("day")] public string Day { get; set; } = "dd MMM";
            [JsonPropertyName("hour")] public string Hour { get; set; } = "HH:mm";
        }
    }

    public class AxisBorderModel
    {
        [JsonPropertyName("show")] public bool Show { get; set; } = true;
        [JsonPropertyName("color")] public string Color { get; set; } = "#78909C";
        [JsonPropertyName("height")] public int Height { get; set; } = 1;
        [JsonPropertyName("width")] public string Width { get; set; } = "100%";
        [JsonPropertyName("offsetX")] public int OffsetX { get; set; } = 0;
        [JsonPropertyName("offsetY")] public int OffsetY { get; set; } = 0;
    }

    public class AxisTicksModel
    {
        [JsonPropertyName("show")] public bool Show { get; set; } = true;
        [JsonPropertyName("borderType")] public string BorderType { get; set; } = "solid";
        [JsonPropertyName("color")] public string Color { get; set; } = "#78909C";
        [JsonPropertyName("height")] public int Height { get; set; } = 6;
        [JsonPropertyName("offsetX")] public int OffsetX { get; set; } = 0;
        [JsonPropertyName("offsetY")] public int OffsetY { get; set; } = 0;
    }


    public class TitleModel
    {
        [JsonPropertyName("text")] public string Text { get; set; } = string.Empty;
        [JsonPropertyName("offsetX")] public int OffsetX { get; set; } = 0;
        [JsonPropertyName("offsetY")] public int OffsetY { get; set; } = 0;
        [JsonPropertyName("style")] public StyleModel Style { get; set; } = new();

        public class StyleModel
        {
            [JsonPropertyName("color")] public string Color { get; set; } = string.Empty;
            [JsonPropertyName("fontSize")] public string FontSize { get; set; } = "12px";
            [JsonPropertyName("fontFamily")] public string FontFamily { get; set; } = "Helvetica, Arial, sans-serif";
            [JsonPropertyName("fontWeight")] public string FontWeight { get; set; } = "600";
            [JsonPropertyName("cssClass")] public string CssClass { get; set; } = "apexcharts-xaxis-title";
        }
    }

    public class CrosshairsModel
    {
        [JsonPropertyName("show")] public bool Show { get; set; } = true;
        [JsonPropertyName("width")] public int Width { get; set; } = 1;
        [JsonPropertyName("position")] public string Position { get; set; } = "back";
        [JsonPropertyName("opacity")] public double Opacity { get; set; } = 0.9;
        [JsonPropertyName("stroke")] public StrokeModel Stroke { get; set; } = new();
        [JsonPropertyName("fill")] public FillModel Fill { get; set; } = new();
        [JsonPropertyName("dropShadow")] public DropShadowModel DropShadow { get; set; } = new();

        public class StrokeModel
        {
            [JsonPropertyName("color")] public string Color { get; set; } = "#b6b6b6";
            [JsonPropertyName("width")] public int Width { get; set; } = 0;
            [JsonPropertyName("dashArray")] public int DashArray { get; set; } = 0;
        }

        public class FillModel
        {
            [JsonPropertyName("type")] public string Type { get; set; } = "solid";
            [JsonPropertyName("color")] public string Color { get; set; } = "#B1B9C4";
            [JsonPropertyName("gradient")] public GradientModel Gradient { get; set; } = new();

            public class GradientModel
            {
                [JsonPropertyName("colorFrom")] public string ColorFrom { get; set; } = "#D8E3F0";
                [JsonPropertyName("colorTo")] public string ColorTo { get; set; } = "#BED1E6";
                [JsonPropertyName("stops")] public List<int> Stops { get; set; } = new() {0, 100};
                [JsonPropertyName("opacityFrom")] public double OpacityFrom { get; set; } = 0.4;
                [JsonPropertyName("opacityTo")] public double OpacityTo { get; set; } = 0.5;
            }
        }

        public class DropShadowModel
        {
            [JsonPropertyName("enabled")] public bool Enabled { get; set; } = false;
            [JsonPropertyName("top")] public int Top { get; set; } = 0;
            [JsonPropertyName("left")] public int Left { get; set; } = 0;
            [JsonPropertyName("blur")] public double Blur { get; set; } = 1;
            [JsonPropertyName("opacity")] public double Opacity { get; set; } = 0.4;
        }
    }

    public class TooltipModel
    {
        [JsonPropertyName("enabled")] public bool Enabled { get; set; } = true;

        // TODO: Formatter.
        [JsonPropertyName("offsetY")] public int OffsetY { get; set; } = 0;
        [JsonPropertyName("style")] public StyleModel Style { get; set; } = new();

        public class StyleModel
        {
            [JsonPropertyName("fontSize")] public int FontSize { get; set; } = 0;
            [JsonPropertyName("fontFamily")] public int FontFamily { get; set; } = 0;
        }
    }
}