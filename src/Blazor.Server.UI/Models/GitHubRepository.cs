using System.Text.Json.Serialization;

namespace Blazor.Server.UI.Models;

public class GitHubRepository
{
    [JsonPropertyName("stargazers_count")]
    public int StargazersCount { get; set; }
}
