using System.Text.Json.Serialization;

namespace Jellyfin.Plugin.Aniliberty.Models;

#pragma warning disable CS1591

public class Genre
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("image")]
    public Image? Image { get; set; }

    [JsonPropertyName("total_releases")]
    public int? TotalReleases { get; set; }
}
