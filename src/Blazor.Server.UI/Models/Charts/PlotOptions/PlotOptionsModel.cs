using System.Text.Json.Serialization;

namespace Blazor.Server.UI.Models.Charts.PlotOptions;

public class PlotOptionsModel
{
    [JsonPropertyName("area")] public AreaModel Area { get; set; } = new();
    [JsonPropertyName("bar")] public BarModel Bar { get; set; } = new();
    [JsonPropertyName("bubble")] public BubbleModel Bubble { get; set; } = new();
    [JsonPropertyName("candlestick")] public CandlestickModel Candlestick { get; set; } = new();
    [JsonPropertyName("boxPlot")] public BoxPlotModel BoxPlot { get; set; } = new();
    [JsonPropertyName("heatmap")] public HeatmapModel Heatmap { get; set; } = new();
    [JsonPropertyName("treemap")] public TreemapModel Treemap { get; set; } = new();
    [JsonPropertyName("pie")] public PieModel Pie { get; set; } = new();
    [JsonPropertyName("polarArea")] public PolarAreaModel PolarArea { get; set; } = new();
    [JsonPropertyName("radar")] public RadarModel Radar { get; set; } = new();
    [JsonPropertyName("radialBar")] public RadialBarModel RadialBar { get; set; } = new();

    public class AreaModel
    {
        [JsonPropertyName("fillTo")] public string FillTo { get; set; } = "origin";
    }

    public class BarModel
    {
        [JsonPropertyName("horizontal")] public bool Horizontal { get; set; } = false;
        [JsonPropertyName("borderRadius")] public int BorderRadius { get; set; } = 0;
        [JsonPropertyName("columnWidth")] public string ColumnWidth { get; set; } = "90%";
        [JsonPropertyName("barHeight")] public string BarHeight { get; set; } = "70%";
        [JsonPropertyName("distributed")] public bool Distributed { get; set; } = false;
        [JsonPropertyName("rangeBarOverlap")] public bool RangeBarOverlap { get; set; } = true;

        [JsonPropertyName("rangeBarGroupRows")]
        public bool RangeBarGroupRows { get; set; } = false;

        [JsonPropertyName("colors")] public ColorsModel Colors { get; set; } = new();

        public class ColorsModel
        {
            [JsonPropertyName("ranges")] public List<RangeModel> Ranges { get; set; } = new();

            [JsonPropertyName("backgroundBarColors")]
            public List<string> BackgroundBarColors { get; set; } = new();

            [JsonPropertyName("backgroundBarOpacity")]
            public double BackgroundBarOpacity { get; set; } = 1;

            [JsonPropertyName("backgroundBarRadius")]
            public int BackgroundBarRadius { get; set; } = 0;

            public class RangeModel
            {
                [JsonPropertyName("from")] public double From { get; set; } = 0;
                [JsonPropertyName("to")] public double To { get; set; } = 0;
                [JsonPropertyName("color")] public string Color { get; set; } = string.Empty;
            }
        }
    }

    public class BubbleModel
    {
        [JsonPropertyName("minBubbleRadius")] public double MinBubbleRadius { get; set; }
        [JsonPropertyName("maxBubbleRadius")] public double MaxBubbleRadius { get; set; }
    }

    public class CandlestickModel
    {
        [JsonPropertyName("colors")] public ColorsModel Colors { get; set; } = new();
        [JsonPropertyName("wick")] public WickModel Wick { get; set; } = new();

        public class ColorsModel
        {
            [JsonPropertyName("upward")] public string Upward { get; set; } = "#00B746";
            [JsonPropertyName("downward")] public string Downward { get; set; } = "#EF403C";
        }

        public class WickModel
        {
            [JsonPropertyName("useFillColor")] public bool UseFillColor { get; set; } = true;
        }
    }

    public class BoxPlotModel
    {
        [JsonPropertyName("colors")] public ColorsModel Colors { get; set; } = new();

        public class ColorsModel
        {
            [JsonPropertyName("upper")] public string Upper { get; set; } = "#00E396";
            [JsonPropertyName("lower")] public string Lower { get; set; } = "#008FFB";
        }
    }

    public class HeatmapModel
    {
        [JsonPropertyName("radius")] public double Radius { get; set; } = 2;
        [JsonPropertyName("enableShades")] public bool EnableShades { get; set; } = true;
        [JsonPropertyName("shadeIntensity")] public double ShadeIntensity { get; set; } = 0.5;

        [JsonPropertyName("reverseNegativeShade")]
        public bool ReverseNegativeShade { get; set; } = true;

        [JsonPropertyName("distributed")] public bool Distributed { get; set; } = false;

        [JsonPropertyName("useFillColorAsStroke")]
        public bool UseFillColorAsStroke { get; set; } = false;

        [JsonPropertyName("colorScale")] public ColorScaleModel ColorScale { get; set; } = new();

        public class ColorScaleModel
        {
            [JsonPropertyName("ranges")] public List<RangeModel> Ranges { get; set; } = new();
            [JsonPropertyName("inverse")] public bool Inverse { get; set; } = false;
            [JsonPropertyName("min")] public double Min { get; set; }
            [JsonPropertyName("max")] public double Max { get; set; }

            public class RangeModel
            {
                [JsonPropertyName("from")] public double From { get; set; } = 0;
                [JsonPropertyName("to")] public double To { get; set; } = 0;
                [JsonPropertyName("color")] public string Color { get; set; } = string.Empty;
                [JsonPropertyName("foreColor")] public string ForeColor { get; set; } = string.Empty;
                [JsonPropertyName("name")] public string Name { get; set; } = string.Empty;
            }
        }
    }

    public class TreemapModel
    {
        [JsonPropertyName("enableShades")] public bool EnableShades { get; set; } = true;
        [JsonPropertyName("shadeIntensity")] public double ShadeIntensity { get; set; } = 0.5;

        [JsonPropertyName("reverseNegativeShade")]
        public bool ReverseNegativeShade { get; set; } = true;

        [JsonPropertyName("distributed")] public bool Distributed { get; set; } = false;

        [JsonPropertyName("useFillColorAsStroke")]
        public bool UseFillColorAsStroke { get; set; } = false;

        [JsonPropertyName("colorScale")] public ColorScaleModel ColorScale { get; set; } = new();

        public class ColorScaleModel
        {
            [JsonPropertyName("ranges")] public List<RangeModel> Ranges { get; set; } = new();
            [JsonPropertyName("inverse")] public bool Inverse { get; set; } = false;
            [JsonPropertyName("min")] public double Min { get; set; }
            [JsonPropertyName("max")] public double Max { get; set; }

            public class RangeModel
            {
                [JsonPropertyName("from")] public double From { get; set; } = 0;
                [JsonPropertyName("to")] public double To { get; set; } = 0;
                [JsonPropertyName("color")] public string Color { get; set; } = string.Empty;
                [JsonPropertyName("foreColor")] public string ForeColor { get; set; } = string.Empty;
                [JsonPropertyName("name")] public string Name { get; set; } = string.Empty;
            }
        }
    }

    public class PieModel
    {
        [JsonPropertyName("startAngle")] public int StartAngle { get; set; } = 0;
        [JsonPropertyName("endAngle")] public int EndAngle { get; set; } = 360;
        [JsonPropertyName("expandOnClick")] public bool ExpandOnClick { get; set; } = true;
        [JsonPropertyName("offsetX")] public int OffsetX { get; set; } = 0;
        [JsonPropertyName("offsetY")] public int OffsetY { get; set; } = 0;
        [JsonPropertyName("customScale")] public double CustomScale { get; set; } = 1;
        [JsonPropertyName("dataLabels")] public DataLabelsModel DataLabels { get; set; } = new();
        [JsonPropertyName("donut")] public DonutModel Donut { get; set; } = new();

        public class DataLabelsModel
        {
            [JsonPropertyName("offset")] public int Offset { get; set; } = 0;

            [JsonPropertyName("minAngleToShowLabel")]
            public int MinAngleToShowLabel { get; set; } = 10;
        }

        public class DonutModel
        {
            [JsonPropertyName("size")] public string Size { get; set; } = "65%";
            [JsonPropertyName("background")] public string Background { get; set; } = "transparent";
            [JsonPropertyName("labels")] public LabelsModel Labels { get; set; } = new();

            public class LabelsModel
            {
                [JsonPropertyName("show")] public bool Show { get; set; } = false;
                [JsonPropertyName("name")] public NameModel Name { get; set; } = new();
                [JsonPropertyName("value")] public ValueModel Value { get; set; } = new();
                [JsonPropertyName("total")] public TotalModel Total { get; set; } = new();

                public class NameModel
                {
                    [JsonPropertyName("show")] public bool Show { get; set; } = true;
                    [JsonPropertyName("fontSize")] public string FontSize { get; set; } = "22px";

                    [JsonPropertyName("fontFamily")]
                    public string FontFamily { get; set; } = "Helvetica, Arial, sans-serif";

                    [JsonPropertyName("fontWeight")] public string FontWeight { get; set; } = "600";
                    [JsonPropertyName("color")] public string Color { get; set; } = "var(--mud-palette-text-primary)";

                    [JsonPropertyName("offsetY")] public int OffsetY { get; set; } = -10;
                    // TODO: Add formatter.
                }

                public class ValueModel
                {
                    [JsonPropertyName("show")] public bool Show { get; set; } = true;
                    [JsonPropertyName("fontSize")] public string FontSize { get; set; } = "16px";

                    [JsonPropertyName("fontFamily")]
                    public string FontFamily { get; set; } = "Helvetica, Arial, sans-serif";

                    [JsonPropertyName("fontWeight")] public string FontWeight { get; set; } = "400";
                    [JsonPropertyName("color")] public string Color { get; set; } = "var(--mud-palette-text-primary)";

                    [JsonPropertyName("offsetY")] public int OffsetY { get; set; } = 16;
                    // TODO: Add formatter.
                }

                public class TotalModel
                {
                    [JsonPropertyName("show")] public bool Show { get; set; } = false;
                    [JsonPropertyName("showAlways")] public bool ShowAlways { get; set; } = false;
                    [JsonPropertyName("label")] public string Label { get; set; } = "Total";
                    [JsonPropertyName("fontSize")] public string FontSize { get; set; } = "22px";

                    [JsonPropertyName("fontFamily")]
                    public string FontFamily { get; set; } = "Helvetica, Arial, sans-serif";

                    [JsonPropertyName("fontWeight")] public string FontWeight { get; set; } = "600";

                    [JsonPropertyName("color")] public string Color { get; set; } = "var(--mud-palette-text-primary)";
                    // TODO: Add formatter.
                }
            }
        }
    }

    public class PolarAreaModel
    {
        [JsonPropertyName("rings")] public RingsModel Rings { get; set; } = new();
        [JsonPropertyName("spokes")] public SpokesModel Spokes { get; set; } = new();

        public class RingsModel
        {
            [JsonPropertyName("strokeWidth")] public int StrokeWidth { get; set; } = 1;
            [JsonPropertyName("strokeColor")] public string StrokeColor { get; set; } = "#e8e8e8";
        }

        public class SpokesModel
        {
            [JsonPropertyName("strokeWidth")] public int StrokeWidth { get; set; } = 1;
            [JsonPropertyName("connectorColors")] public string ConnectorColors { get; set; } = "#e8e8e8";
        }
    }

    public class RadarModel
    {
        [JsonPropertyName("size")] public int Size { get; set; }
        [JsonPropertyName("offsetX")] public int OffsetX { get; set; } = 0;
        [JsonPropertyName("offsetY")] public int OffsetY { get; set; } = 0;
        [JsonPropertyName("polygons")] public PolygonsModel Polygons { get; set; } = new();

        public class PolygonsModel
        {
            [JsonPropertyName("strokeColors")] public string StrokeColors { get; set; } = "#e8e8e8";
            [JsonPropertyName("strokeWidth")] public int StrokeWidth { get; set; } = 1;
            [JsonPropertyName("connectorColors")] public string ConnectorColors { get; set; } = "#e8e8e8";
            [JsonPropertyName("fill")] public FillModel Fill { get; set; } = new();

            public class FillModel
            {
                [JsonPropertyName("colors")] public List<string> Colors { get; set; } = new();
            }
        }
    }

    public class RadialBarModel
    {
        [JsonPropertyName("inverseOrder")] public bool InverseOrder { get; set; } = false;
        [JsonPropertyName("startAngle")] public int StartAngle { get; set; } = 0;
        [JsonPropertyName("endAngle")] public int EndAngle { get; set; } = 360;
        [JsonPropertyName("offsetX")] public int OffsetX { get; set; } = 0;
        [JsonPropertyName("offsetY")] public int OffsetY { get; set; } = 0;
        [JsonPropertyName("hollow")] public HollowModel Hollow { get; set; } = new();
        [JsonPropertyName("track")] public TrackModel Track { get; set; } = new();
        [JsonPropertyName("dataLabels")] public DataLabelsModel DataLabels { get; set; } = new();


        public class HollowModel
        {
            [JsonPropertyName("margin")] public int Margin { get; set; } = 5;
            [JsonPropertyName("size")] public string Size { get; set; } = "50%";
            [JsonPropertyName("background")] public string Background { get; set; } = "transparent";
            [JsonPropertyName("image")] public string Image { get; set; } = string.Empty;
            [JsonPropertyName("imageWidth")] public int ImageWidth { get; set; } = 150;
            [JsonPropertyName("imageHeight")] public int ImageHeight { get; set; } = 150;
            [JsonPropertyName("imageOffsetX")] public int ImageOffsetX { get; set; } = 0;
            [JsonPropertyName("imageOffsetY")] public int ImageOffsetY { get; set; } = 0;
            [JsonPropertyName("imageClipped")] public bool ImageClipped { get; set; } = true;
            [JsonPropertyName("position")] public string Position { get; set; } = "front";
            [JsonPropertyName("dropShadow")] public DropShadowModel DropShadow { get; set; } = new();

            public class DropShadowModel
            {
                [JsonPropertyName("enabled")] public bool Enabled { get; set; } = false;
                [JsonPropertyName("top")] public int Top { get; set; } = 0;
                [JsonPropertyName("left")] public int Left { get; set; } = 0;
                [JsonPropertyName("blur")] public int Blur { get; set; } = 3;
                [JsonPropertyName("opacity")] public double Opacity { get; set; } = 0.5;
            }
        }

        public class TrackModel
        {
            [JsonPropertyName("show")] public bool Show { get; set; } = false;
            [JsonPropertyName("startAngle")] public int StartAngle { get; set; }
            [JsonPropertyName("endAngle")] public int EndAngle { get; set; }
            [JsonPropertyName("background")] public string Background { get; set; } = "#f2f2f2";
            [JsonPropertyName("strokeWidth")] public int StrokeWidth { get; set; } = 1;
            [JsonPropertyName("opacity")] public double Opacity { get; set; } = 1;
            [JsonPropertyName("dropShadow")] public DropShadowModel DropShadow { get; set; } = new();

            public class DropShadowModel
            {
                [JsonPropertyName("enabled")] public bool Enabled { get; set; } = false;
                [JsonPropertyName("top")] public int Top { get; set; } = 0;
                [JsonPropertyName("left")] public int Left { get; set; } = 0;
                [JsonPropertyName("blur")] public double Blur { get; set; } = 3;
                [JsonPropertyName("opacity")] public double Opacity { get; set; } = 0.5;
            }
        }

        public class DataLabelsModel
        {
            [JsonPropertyName("show")] public bool Show { get; set; } = true;
            [JsonPropertyName("name")] public NameModel Name { get; set; } = new();
            [JsonPropertyName("value")] public ValueModel Value { get; set; } = new();
            [JsonPropertyName("total")] public TotalModel Total { get; set; } = new();

            public class NameModel
            {
                [JsonPropertyName("show")] public bool Show { get; set; } = true;
                [JsonPropertyName("fontSize")] public string FontSize { get; set; } = "14px";
                [JsonPropertyName("fontFamily")] public string FontFamily { get; set; } = string.Empty;
                [JsonPropertyName("fontWeight")] public string FontWeight { get; set; } = "600";
                [JsonPropertyName("color")] public string Color { get; set; } = string.Empty;
                [JsonPropertyName("offsetY")] public int OffsetY { get; set; } = -10;
            }

            public class ValueModel
            {
                [JsonPropertyName("show")] public bool Show { get; set; } = true;
                [JsonPropertyName("fontSize")] public string FontSize { get; set; } = "14px";
                [JsonPropertyName("fontFamily")] public string FontFamily { get; set; } = string.Empty;
                [JsonPropertyName("fontWeight")] public string FontWeight { get; set; } = "400";
                [JsonPropertyName("color")] public string Color { get; set; } = string.Empty;

                [JsonPropertyName("offsetY")] public int OffsetY { get; set; } = 16;
                // TODO: Add formatter.
            }

            public class TotalModel
            {
                [JsonPropertyName("show")] public bool Show { get; set; } = false;
                [JsonPropertyName("label")] public string Label { get; set; } = "Total";
                [JsonPropertyName("color")] public string Color { get; set; } = "#373d3f";
                [JsonPropertyName("fontSize")] public string FontSize { get; set; } = "16px";
                [JsonPropertyName("fontFamily")] public string FontFamily { get; set; } = string.Empty;

                [JsonPropertyName("fontWeight")] public string FontWeight { get; set; } = "600";
                // TODO: Add formatter.
            }
        }
    }
}