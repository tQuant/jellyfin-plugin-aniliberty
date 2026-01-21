using System.Text.Json.Serialization;

namespace Jellyfin.Plugin.Aniliberty.Models;

#pragma warning disable CS1591,SA1300

public class CatalogReleaseName
{
    /// <summary>
    /// Название релиза на русском языке.
    /// </summary>
    [JsonPropertyName("main")]
    public string? Main { get; set; }

    /// <summary>
    /// Оригинальное название релиза.
    /// </summary>
    [JsonPropertyName("english")]
    public string? English { get; set; }

    [JsonPropertyName("alternative")]
    public string? Alternative { get; set; }
}
