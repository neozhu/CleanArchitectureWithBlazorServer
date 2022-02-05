using System.Text.Json.Serialization;

namespace MudDemo.Server.Models.Charts.States;

public class StatesModel
{
    [JsonPropertyName("normal")] public NormalModel Normal { get; set; } = new();
    [JsonPropertyName("hover")] public HoverModel Hover { get; set; } = new();
    [JsonPropertyName("active")] public ActiveModel Active { get; set; } = new();

    public class NormalModel
    {
        [JsonPropertyName("filter")] public FilterModel Filter { get; set; } = new();

        public class FilterModel
        {
            [JsonPropertyName("type")] public string Type { get; set; } = "none";
            [JsonPropertyName("value")] public double Value { get; set; } = 0;
        }
    }

    public class HoverModel
    {
        [JsonPropertyName("filter")] public FilterModel Filter { get; set; } = new();

        public class FilterModel
        {
            [JsonPropertyName("type")] public string Type { get; set; } = "lighten";
            [JsonPropertyName("value")] public double Value { get; set; } = 0.15;
        }
    }

    public class ActiveModel
    {
        [JsonPropertyName("filter")] public FilterModel Filter { get; set; } = new();

        public class FilterModel
        {
            [JsonPropertyName("type")] public string Type { get; set; } = "darken";
            [JsonPropertyName("value")] public double Value { get; set; } = 0.35;
        }
    }
}