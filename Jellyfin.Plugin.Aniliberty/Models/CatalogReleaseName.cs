using System.Text.Json.Serialization;

namespace Jellyfin.Plugin.Aniliberty.Models;

#pragma warning disable CS1591,SA1300

public class CatalogReleaseName
{
    [JsonPropertyName("main")]
    public string? Main { get; set; }

    [JsonPropertyName("english")]
    public string? English { get; set; }

    [JsonPropertyName("alternative")]
    public string? Alternative { get; set; }
}
