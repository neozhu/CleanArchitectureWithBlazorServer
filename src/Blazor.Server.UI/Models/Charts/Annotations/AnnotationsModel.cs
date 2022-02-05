using System.Text.Json.Serialization;

namespace Blazor.Server.UI.Models.Charts.Annotations;

public class AnnotationsModel
{
    [JsonPropertyName("position")] public string Position { get; set; } = "front";
    [JsonPropertyName("yAxis")] public List<YAxisModel> YAxis { get; set; } = new();
    [JsonPropertyName("xAxis")] public List<XAxisModel> XAxis { get; set; } = new();
    [JsonPropertyName("points")] public List<PointsModel> Points { get; set; } = new();
    [JsonPropertyName("texts")] public List<TextsModel> Texts { get; set; } = new();
    [JsonPropertyName("images")] public List<ImagesModel> Images { get; set; } = new();

    public class YAxisModel
    {
        [JsonPropertyName("y")] public int Y { get; set; } = 0;
        [JsonPropertyName("y2")] public int? Y2 { get; set; }
        [JsonPropertyName("strokeDashArray")] public int StrokeDashArray { get; set; } = 1;
        [JsonPropertyName("borderColor")] public string BorderColor { get; set; } = "#c2c2c2";
        [JsonPropertyName("fillColor")] public string FillColor { get; set; } = "#c2c2c2";
        [JsonPropertyName("opacity")] public double Opacity { get; set; } = 0.3;
        [JsonPropertyName("offsetX")] public int OffsetX { get; set; } = 0;
        [JsonPropertyName("offsetY")] public int OffsetY { get; set; } = -3;
        [JsonPropertyName("width")] public string Width { get; set; } = "100%";
        [JsonPropertyName("yAxisIndex")] public int YAxisIndex { get; set; } = 0;
        [JsonPropertyName("label")] public LabelModel Label { get; set; } = new();

        public class LabelModel
        {
            [JsonPropertyName("borderColor")] public string BorderColor { get; set; } = "#c2c2c2";
            [JsonPropertyName("borderWidth")] public int BorderWidth { get; set; } = 1;
            [JsonPropertyName("borderRadius")] public int BorderRadius { get; set; } = 2;
            [JsonPropertyName("text")] public string Text { get; set; } = string.Empty;
            [JsonPropertyName("textAnchor")] public string TextAnchor { get; set; } = "end";
            [JsonPropertyName("position")] public string Position { get; set; } = "right";
            [JsonPropertyName("offsetX")] public int OffsetX { get; set; } = 0;
            [JsonPropertyName("offsetY")] public int OffsetY { get; set; } = 0;
            [JsonPropertyName("style")] public StyleModel Style { get; set; } = new();

            public class StyleModel
            {
                [JsonPropertyName("background")] public string Background { get; set; } = "#fff";
                [JsonPropertyName("color")] public string Color { get; set; } = "#777";
                [JsonPropertyName("fontSize")] public string FontSize { get; set; } = "12px";
                [JsonPropertyName("fontWeight")] public int FontWeight { get; set; } = 400;
                [JsonPropertyName("fontFamily")] public string FontFamily { get; set; } = string.Empty;

                [JsonPropertyName("cssClass")]
                public string CssClass { get; set; } = "apexcharts-yaxis-annotation-label";

                [JsonPropertyName("padding")] public PaddingModel Padding { get; set; } = new();
            }

            public class PaddingModel
            {
                [JsonPropertyName("left")] public int Left { get; set; } = 5;
                [JsonPropertyName("right")] public int Right { get; set; } = 5;
                [JsonPropertyName("top")] public int Top { get; set; } = 0;
                [JsonPropertyName("bottom")] public int Bottom { get; set; } = 2;
            }
        }
    }

    public class XAxisModel
    {
        [JsonPropertyName("x")] public int X { get; set; } = 0;
        [JsonPropertyName("x2")] public int? X2 { get; set; }
        [JsonPropertyName("strokeDashArray")] public int StrokeDashArray { get; set; } = 1;
        [JsonPropertyName("borderColor")] public string BorderColor { get; set; } = "#c2c2c2";
        [JsonPropertyName("fillColor")] public string FillColor { get; set; } = "#c2c2c2";
        [JsonPropertyName("opacity")] public double Opacity { get; set; } = 0.3;
        [JsonPropertyName("offsetX")] public int OffsetX { get; set; } = 0;
        [JsonPropertyName("offsetY")] public int OffsetY { get; set; } = 0;
        [JsonPropertyName("label")] public LabelModel Label { get; set; } = new();

        public class LabelModel
        {
            [JsonPropertyName("borderColor")] public string BorderColor { get; set; } = "#c2c2c2";
            [JsonPropertyName("borderWidth")] public int BorderWidth { get; set; } = 1;
            [JsonPropertyName("borderRadius")] public int BorderRadius { get; set; } = 2;
            [JsonPropertyName("text")] public string Text { get; set; } = string.Empty;
            [JsonPropertyName("textAnchor")] public string TextAnchor { get; set; } = "middle";
            [JsonPropertyName("position")] public string Position { get; set; } = "top";
            [JsonPropertyName("offsetX")] public int OffsetX { get; set; } = 0;
            [JsonPropertyName("offsetY")] public int OffsetY { get; set; } = 0;
            [JsonPropertyName("style")] public StyleModel Style { get; set; } = new();

            public class StyleModel
            {
                [JsonPropertyName("background")] public string Background { get; set; } = "#fff";
                [JsonPropertyName("color")] public string Color { get; set; } = "#777";
                [JsonPropertyName("fontSize")] public string FontSize { get; set; } = "12px";
                [JsonPropertyName("fontWeight")] public int FontWeight { get; set; } = 400;
                [JsonPropertyName("fontFamily")] public string FontFamily { get; set; } = string.Empty;

                [JsonPropertyName("cssClass")]
                public string CssClass { get; set; } = "apexcharts-yaxis-annotation-label";
            }
        }
    }

    public class PointsModel
    {
        [JsonPropertyName("x")] public int X { get; set; } = 0;
        [JsonPropertyName("y")] public double? Y { get; set; }
        [JsonPropertyName("yAxisIndex")] public int YAxisIndex { get; set; } = 0;
        [JsonPropertyName("seriesIndex")] public int SeriesIndex { get; set; } = 0;
        [JsonPropertyName("marker")] public MarkerModel Marker { get; set; } = new();
        [JsonPropertyName("label")] public LabelModel Label { get; set; } = new();
        [JsonPropertyName("image")] public ImageModel Image { get; set; } = new();

        public class MarkerModel
        {
            [JsonPropertyName("size")] public int Size { get; set; } = 0;
            [JsonPropertyName("fillColor")] public string FillColor { get; set; } = "#fff";
            [JsonPropertyName("strokeColor")] public string StrokeColor { get; set; } = "#333";
            [JsonPropertyName("strokeWidth")] public int StrokeWidth { get; set; } = 3;
            [JsonPropertyName("shape")] public string Shape { get; set; } = "circle";
            [JsonPropertyName("radius")] public int Radius { get; set; } = 2;
            [JsonPropertyName("offsetX")] public int OffsetX { get; set; } = 0;
            [JsonPropertyName("offsetY")] public int OffsetY { get; set; } = 0;
            [JsonPropertyName("cssClass")] public string CssClass { get; set; } = string.Empty;
        }

        public class LabelModel
        {
            [JsonPropertyName("borderColor")] public string BorderColor { get; set; } = "#c2c2c2";
            [JsonPropertyName("borderWidth")] public int BorderWidth { get; set; } = 1;
            [JsonPropertyName("borderRadius")] public int BorderRadius { get; set; } = 2;
            [JsonPropertyName("text")] public string Text { get; set; } = string.Empty;
            [JsonPropertyName("textAnchor")] public string TextAnchor { get; set; } = "middle";
            [JsonPropertyName("offsetX")] public int OffsetX { get; set; } = 0;
            [JsonPropertyName("offsetY")] public int OffsetY { get; set; } = -15;
            [JsonPropertyName("style")] public StyleModel Style { get; set; } = new();

            public class StyleModel
            {
                [JsonPropertyName("background")] public string Background { get; set; } = "#fff";
                [JsonPropertyName("color")] public string Color { get; set; } = "#777";
                [JsonPropertyName("fontSize")] public string FontSize { get; set; } = "12px";
                [JsonPropertyName("fontWeight")] public int FontWeight { get; set; } = 400;
                [JsonPropertyName("fontFamily")] public string FontFamily { get; set; } = string.Empty;

                [JsonPropertyName("cssClass")]
                public string CssClass { get; set; } = "apexcharts-point-annotation-label";

                [JsonPropertyName("padding")] public PaddingModel Padding { get; set; } = new();

                public class PaddingModel
                {
                    [JsonPropertyName("left")] public int Left { get; set; } = 5;
                    [JsonPropertyName("right")] public int Right { get; set; } = 5;
                    [JsonPropertyName("top")] public int Top { get; set; } = 0;
                    [JsonPropertyName("bottom")] public int Bottom { get; set; } = 2;
                }
            }
        }

        public class ImageModel
        {
            [JsonPropertyName("path")] public string Path { get; set; } = string.Empty;
            [JsonPropertyName("width")] public int Width { get; set; } = 20;
            [JsonPropertyName("height")] public int Height { get; set; } = 20;
            [JsonPropertyName("offsetX")] public int OffsetX { get; set; } = 0;
            [JsonPropertyName("offsetY")] public int OffsetY { get; set; } = 0;
        }
    }

    public class TextsModel
    {
        [JsonPropertyName("x")] public double X { get; set; } = 0;
        [JsonPropertyName("y")] public double Y { get; set; } = 0;
        [JsonPropertyName("text")] public string Text { get; set; } = string.Empty;
        [JsonPropertyName("textAnchor")] public string TextAnchor { get; set; } = "start";
        [JsonPropertyName("foreColor")] public string ForeColor { get; set; } = string.Empty;
        [JsonPropertyName("fontSize")] public string FontSize { get; set; } = "13px";
        [JsonPropertyName("fontFamily")] public string FontFamily { get; set; } = string.Empty;
        [JsonPropertyName("fontWeight")] public int FontWeight { get; set; } = 400;
        [JsonPropertyName("appendTo")] public string AppendTo { get; set; } = ".apexcharts-annotations";
        [JsonPropertyName("backgroundColor")] public string BackgroundColor { get; set; } = "transparent";
        [JsonPropertyName("borderColor")] public string BorderColor { get; set; } = "#c2c2c2";
        [JsonPropertyName("borderRadius")] public int BorderRadius { get; set; } = 0;
        [JsonPropertyName("borderWidth")] public int BorderWidth { get; set; } = 0;
        [JsonPropertyName("paddingLeft")] public int PaddingLeft { get; set; } = 4;
        [JsonPropertyName("paddingRight")] public int PaddingRight { get; set; } = 4;
        [JsonPropertyName("paddingTop")] public int PaddingTop { get; set; } = 2;
        [JsonPropertyName("paddingBottom")] public int PaddingBottom { get; set; } = 2;
    }

    public class ImagesModel
    {
        [JsonPropertyName("path")] public string Path { get; set; } = string.Empty;
        [JsonPropertyName("x")] public double X { get; set; } = 0;
        [JsonPropertyName("y")] public double Y { get; set; } = 0;
        [JsonPropertyName("width")] public int Width { get; set; } = 20;
        [JsonPropertyName("height")] public int Height { get; set; } = 20;
        [JsonPropertyName("appendTo")] public string AppendTo { get; set; } = ".apexcharts-annotations";
    }
}