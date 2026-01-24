using System;
using System.Text.Json.Serialization;

namespace Jellyfin.Plugin.Aniliberty.Models;

#pragma warning disable CS1591

public class AlEpisode
{
    [JsonPropertyName("id")]
    public required string Id { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("ordinal")]
    public float Ordinal { get; set; }

    [JsonPropertyName("ending")]
    public TimeRange? Ending { get; set; }

    [JsonPropertyName("opening")]
    public TimeRange? Opening { get; set; }

    [JsonPropertyName("preview")]
    public Image? Preview { get; set; }

    [JsonPropertyName("duration")]
    public int? Duration { get; set; }

    [JsonPropertyName("release_id")]
    public int? ReleaseId { get; set; }

    [JsonPropertyName("name_english")]
    public string? NameEnglish { get; set; }

    public int Number()
    {
        return Convert.ToInt32(MathF.Round(Ordinal * 10)) / 10;
    }

    public bool NumberIsRound()
    {
        return Convert.ToInt32(MathF.Round(Ordinal * 10)) % 10 == 0;
    }

    public int SegmentsCount()
    {
        return (this.Opening is { Start: not null, Stop: not null } ? 1 : 0)
               + (this.Ending is { Start: not null, Stop: not null } ? 1 : 0);
    }
}
