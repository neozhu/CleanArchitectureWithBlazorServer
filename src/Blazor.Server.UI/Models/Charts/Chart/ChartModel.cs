using System.Text.Json.Serialization;

namespace MudDemo.Server.Models.Charts.Chart;

public class ChartModel
{
    [JsonPropertyName("animations")] public AnimationsModel Animations { get; set; } = new();
    [JsonPropertyName("background")] public string Background { get; set; } = "var(--mud-palette-surface)";
    [JsonPropertyName("brush")] public BrushModel Brush { get; set; } = new();
    [JsonPropertyName("defaultLocale")] public string DefaultLocale { get; set; } = "en";
    [JsonPropertyName("dropShadow")] public DropShadowModel DropShadow { get; set; } = new();

    [JsonPropertyName("fontFamily")]
    public string FontFamily { get; set; } = "Helvetica, Arial, sans-serif"; // TODO: use MudBlazor 

    [JsonPropertyName("foreColor")]
    public string ForeColor { get; set; } = "var(--mud-palette-on-surface)"; // TODO: use MudBlazor 

    [JsonPropertyName("group")] public string Group { get; set; } = string.Empty;
    [JsonPropertyName("events")] public EventsModel Events { get; set; } = new();
    [JsonPropertyName("height")] public string Height { get; set; } = "auto";

    [JsonPropertyName("id")] public string Id { get; set; } = string.Empty;

    //[JsonPropertyName("locales")] public LocalesModel Locales { get; set; } = new(); // TODO...
    [JsonPropertyName("offsetX")] public int OffsetX { get; set; } = 0;
    [JsonPropertyName("offsetY")] public int OffsetY { get; set; } = 0;

    [JsonPropertyName("parentHeightOffset")]
    public int ParentHeightOffset { get; set; } = 15;

    [JsonPropertyName("redrawOnParentResize")]
    public bool RedrawOnParentResize { get; set; } = true;

    [JsonPropertyName("redrawOnWindowResize")]
    public bool RedrawOnWindowResize { get; set; } = true;

    [JsonPropertyName("selection")] public SelectionModel Selection { get; set; } = new();
    [JsonPropertyName("sparkline")] public SparklineModel Sparkline { get; set; } = new();
    [JsonPropertyName("stacked")] public bool Stacked { get; set; }
    [JsonPropertyName("stackType")] public bool StackType { get; set; }
    [JsonPropertyName("toolbar")] public ToolbarModel Toolbar { get; set; } = new();
    [JsonPropertyName("type")] public string Type { get; set; } = ChartTypes.Line;
    [JsonPropertyName("width")] public string Width { get; set; } = "100%";
    [JsonPropertyName("zoom")] public ZoomModel Zoom { get; set; } = new();

    public class AnimationsModel
    {
        [JsonPropertyName("enabled")] public bool Enabled { get; set; } = true;
        [JsonPropertyName("easing")] public string Easing { get; set; } = "easeinout";
        [JsonPropertyName("duration")] public int Duration { get; set; } = 1000;
        [JsonPropertyName("animateGradually")] public AnimateGraduallyModel AnimateGradually { get; set; } = new();
        [JsonPropertyName("dynamicAnimation")] public DynamicAnimationModel DynamicAnimation { get; set; } = new();

        public class AnimateGraduallyModel
        {
            [JsonPropertyName("enabled")] public bool Enabled { get; set; } = true;
            [JsonPropertyName("delay")] public int Delay { get; set; } = 150;
        }

        public class DynamicAnimationModel
        {
            [JsonPropertyName("enabled")] public bool Enabled { get; set; } = true;
            [JsonPropertyName("speed")] public int Speed { get; set; } = 350;
        }
    }

    public class BrushModel
    {
        [JsonPropertyName("enabled")] public bool Enabled { get; set; } = false;
        [JsonPropertyName("target")] public string Target { get; set; } = string.Empty;
        [JsonPropertyName("autoScaleYaxis")] public bool AutoScaleYaxis { get; set; } = false;
    }

    public class DropShadowModel
    {
        [JsonPropertyName("enabled")] public bool Enabled { get; set; } = false;
        [JsonPropertyName("enabledOnSeries")] public List<int> EnabledOnSeries { get; set; } = new(); // TODO...
        [JsonPropertyName("top")] public int Top { get; set; } = 0;
        [JsonPropertyName("left")] public int Left { get; set; } = 0;
        [JsonPropertyName("blur")] public int Blur { get; set; } = 3;
        [JsonPropertyName("color")] public string Color { get; set; } = "#000";
        [JsonPropertyName("opacity")] public double Opacity { get; set; } = 0.35;
    }

    public class EventsModel
    {
        // TODO: Add events.
    }

    public class LocalesModel
    {
        [JsonPropertyName("defaultLocale")] public string DefaultLocale { get; set; } = "en";
        [JsonPropertyName("locales")] public List<LocalesModelItem> Locales { get; set; } = new();

        public class LocalesModelItem
        {
            [JsonPropertyName("name")] public string Name { get; set; } = "en";
            [JsonPropertyName("options")] public OptionsModel Options { get; set; } = new();

            public class OptionsModel
            {
                [JsonPropertyName("months")]
                public List<string> Months { get; set; } = new()
                {
                    "January", "February", "March", "April", "May", "June", "July", "August", "September", "October",
                    "November", "December"
                };

                [JsonPropertyName("shortMonths")]
                public List<string> ShortMonths { get; set; } = new()
                    {"Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"};

                [JsonPropertyName("days")]
                public List<string> Days { get; set; } = new()
                    {"Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday"};

                [JsonPropertyName("shortDays")]
                public List<string> ShortDays { get; set; } = new() {"Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat"};

                [JsonPropertyName("toolbar")] public ToolbarModel Toolbar { get; set; } = new();

                public class ToolbarModel
                {
                    [JsonPropertyName("download")] public string Download { get; set; } = "Download SVG";
                    [JsonPropertyName("selection")] public string Selection { get; set; } = "Selection";
                    [JsonPropertyName("selectionZoom")] public string SelectionZoom { get; set; } = "Selection Zoom";
                    [JsonPropertyName("zoomIn")] public string ZoomIn { get; set; } = "Zoom In";
                    [JsonPropertyName("zoomOut")] public string ZoomOut { get; set; } = "Zoom Out";
                    [JsonPropertyName("pan")] public string Pan { get; set; } = "Panning";
                    [JsonPropertyName("reset")] public string Reset { get; set; } = "Reset Zoom";
                }
            }
        }
    }

    public class SelectionModel
    {
        [JsonPropertyName("enabled")] public bool Enabled { get; set; } = true;
        [JsonPropertyName("x")] public string X { get; set; } = "x";
        [JsonPropertyName("fill")] public FileModel Fill { get; set; } = new();
        [JsonPropertyName("stroke")] public StrokeModel Stroke { get; set; } = new();
        [JsonPropertyName("xaxis")] public XAxisModel XAxis { get; set; } = new();
        [JsonPropertyName("yaxis")] public YAxisModel YAxis { get; set; } = new();

        public class FileModel
        {
            [JsonPropertyName("color")] public string Color { get; set; } = "#a8dadc";
            [JsonPropertyName("opacity")] public double Opacity { get; set; } = 0.1;
        }

        public class StrokeModel
        {
            [JsonPropertyName("width")] public double Width { get; set; } = 1;
            [JsonPropertyName("dashArray")] public double DashArray { get; set; } = 3;
            [JsonPropertyName("color")] public string Color { get; set; } = "#24292e";
            [JsonPropertyName("opacity")] public double Opacity { get; set; } = 0.4;
        }

        public class XAxisModel
        {
            [JsonPropertyName("min")] public double Min { get; set; }
            [JsonPropertyName("max")] public double Max { get; set; }
        }

        public class YAxisModel
        {
            [JsonPropertyName("min")] public double Min { get; set; }
            [JsonPropertyName("max")] public double Max { get; set; }
        }
    }

    public class SparklineModel
    {
        [JsonPropertyName("enabled")] public bool Enabled { get; set; } = false;
    }

    public class ToolbarModel
    {
        [JsonPropertyName("show")] public bool Show { get; set; } = true;
        [JsonPropertyName("offsetX")] public int OffsetX { get; set; } = 0;
        [JsonPropertyName("offsetY")] public int OffsetY { get; set; } = 0;
        [JsonPropertyName("tools")] public ToolsModel Tools { get; set; } = new();
        [JsonPropertyName("export")] public ExportModel Export { get; set; } = new();
        [JsonPropertyName("autoSelected")] public string AutoSelected { get; set; } = "zoom";

        public class ToolsModel
        {
            [JsonPropertyName("download")] public bool Download { get; set; } = true;
            [JsonPropertyName("selection")] public bool Selection { get; set; } = true;
            [JsonPropertyName("zoom")] public bool Zoom { get; set; } = true;
            [JsonPropertyName("zoomin")] public bool ZoomIn { get; set; } = true;
            [JsonPropertyName("zoomout")] public bool ZoomOut { get; set; } = true;
            [JsonPropertyName("pan")] public bool Pan { get; set; } = true;
            [JsonPropertyName("reset")] public bool Reset { get; set; } = true;
        }

        public class ExportModel
        {
            [JsonPropertyName("csv")] public CsvModel Csv { get; set; } = new();
            [JsonPropertyName("svg")] public SvgModel Svg { get; set; } = new();
            [JsonPropertyName("png")] public PngModel Png { get; set; } = new();

            public class CsvModel
            {
                [JsonPropertyName("filename")] public string Filename { get; set; } = string.Empty;
                [JsonPropertyName("columnDelimiter")] public string ColumnDelimiter { get; set; } = ",";
                [JsonPropertyName("headerCategory")] public string HeaderCategory { get; set; } = "category";
                [JsonPropertyName("headerValue")] public string HeaderValue { get; set; } = "value";
                // TODO: Add formatter.
            }

            public class SvgModel
            {
                [JsonPropertyName("filename")] public string Filename { get; set; } = string.Empty;
            }

            public class PngModel
            {
                [JsonPropertyName("filename")] public string Filename { get; set; } = string.Empty;
            }
        }
    }

    public class ZoomModel
    {
        [JsonPropertyName("enabled")] public bool Enabled { get; set; } = true;
        [JsonPropertyName("type")] public string Type { get; set; } = "x";
        [JsonPropertyName("autoScaleYaxis")] public bool AutoScaleYaxis { get; set; } = true;
        [JsonPropertyName("zoomedArea")] public ZoomedAreaModel ZoomedArea { get; set; } = new();
        [JsonPropertyName("stroke")] public StrokeModel Stroke { get; set; } = new();

        public class ZoomedAreaModel
        {
            [JsonPropertyName("fill")] public FillModel Fill { get; set; } = new();

            public class FillModel
            {
                [JsonPropertyName("color")] public string Color { get; set; } = "#90CAF9";
                [JsonPropertyName("opacity")] public double Opacity { get; set; } = 0.1;
            }
        }

        public class StrokeModel
        {
            [JsonPropertyName("color")] public string Color { get; set; } = "#0D47A1";
            [JsonPropertyName("opacity")] public double Opacity { get; set; } = 0.4;
            [JsonPropertyName("width")] public double Width { get; set; } = 1;
        }
    }
}