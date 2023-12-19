using System.Text.Json.Serialization;

namespace CleanArchitecture.Blazor.Server.UI.Models;

public class DiscordInvite
{
    [JsonPropertyName("approximate_member_count")]
    public int ApproximateMemberCount { get; set; }

    [JsonPropertyName("approximate_presence_count")]
    public int ApproximatePresenceCount { get; set; }
}