using System.Text.Json.Serialization;

namespace Jellyfin.Plugin.Aniliberty.Models;

#pragma warning disable CS1591

public class TimeRange
{
    [JsonPropertyName("start")]
    public int? Start { get; set; }

    [JsonPropertyName("stop")]
    public int? Stop { get; set; }
}
