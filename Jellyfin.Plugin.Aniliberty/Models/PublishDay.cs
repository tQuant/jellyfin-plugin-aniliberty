using System.Text.Json.Serialization;

namespace Jellyfin.Plugin.Aniliberty.Models;

#pragma warning disable CS1591

public class PublishDay
{
    [JsonPropertyName("value")]
    public int? Value { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }
}
