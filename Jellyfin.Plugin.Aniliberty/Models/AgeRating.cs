using System.Text.Json.Serialization;

namespace Jellyfin.Plugin.Aniliberty.Models;

#pragma warning disable CS1591

public class AgeRating
{
    [JsonPropertyName("value")]
    public required AgeRating Value { get; set; }

    [JsonPropertyName("label")]
    public required string Label { get; set; }

    [JsonPropertyName("is_adult")]
    public required bool IsAdult { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }
}
