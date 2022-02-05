using System.Text.Json.Serialization;

namespace MudDemo.Server.Models.Charts.Grid;

public class GridModel
{
    [JsonPropertyName("show")] public bool Show { get; set; } = true;
    [JsonPropertyName("borderColor")] public string BorderColor { get; set; } = "#90A4AE";
    [JsonPropertyName("strokeDashArray")] public int StrokeDashArray { get; set; } = 0;
    [JsonPropertyName("position")] public string Position { get; set; } = "back";
    [JsonPropertyName("xaxis")] public XAxisModel XAxis { get; set; } = new();
    [JsonPropertyName("yaxis")] public YAxisModel YAxis { get; set; } = new();
    [JsonPropertyName("row")] public RowModel Row { get; set; } = new();
    [JsonPropertyName("column")] public ColumnModel Column { get; set; } = new();
    [JsonPropertyName("padding")] public PaddingModel Padding { get; set; } = new();

    public class XAxisModel
    {
        [JsonPropertyName("lines")] public LinesModel Lines { get; set; } = new();

        public class LinesModel
        {
            [JsonPropertyName("show")] public bool Show { get; set; } = false;
        }
    }

    public class YAxisModel
    {
        [JsonPropertyName("lines")] public LinesModel Lines { get; set; } = new();

        public class LinesModel
        {
            [JsonPropertyName("show")] public bool Show { get; set; } = false;
        }
    }

    public class RowModel
    {
        //[JsonPropertyName("colors")] public List<string> Colors { get; set; } = new(); // TODO...
        [JsonPropertyName("opacity")] public double Opacity { get; set; } = 0.5;
    }

    public class ColumnModel
    {
        //[JsonPropertyName("colors")] public List<string> Colors { get; set; } = new(); // TODO...
        [JsonPropertyName("opacity")] public double Opacity { get; set; } = 0.5;
    }

    public class PaddingModel
    {
        [JsonPropertyName("top")] public int Top { get; set; } = 0;
        [JsonPropertyName("right")] public int Right { get; set; } = 0;
        [JsonPropertyName("bottom")] public int Bottom { get; set; } = 0;
        [JsonPropertyName("left")] public int Left { get; set; } = 0;
    }
}