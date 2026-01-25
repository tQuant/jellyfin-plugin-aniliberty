using System.Collections.Generic;
using System.Text.Json.Serialization;
using MediaBrowser.Model.Entities;

namespace Jellyfin.Plugin.Aniliberty.Models;

#pragma warning disable CS1591
#pragma warning disable CA1002

public class AlRelease : CatalogRelease
{
    [JsonPropertyName("episodes")]
    public required List<AlEpisode> Episodes { get; init; }

    // [JsonPropertyName("torrents")]
    // public required List<AlTorrent> Torrents { get; init; }

    /// <inheritdoc />
    protected override int? GetTotalDuration()
    {
        if (Episodes.Count > 0 && IsOngoing.HasValue && !IsOngoing.Value)
        {
            int duration = 0;
            foreach (var episode in Episodes)
            {
                // Если хоть у одного из эпизодов не указана продолжительность, то считать ее таким образом нельзя. Возвращаемся к изначальному методу.
                if (episode.Duration is null || episode.Duration < 1)
                {
                    return base.GetTotalDuration();
                }
                else
                {
                    duration += episode.Duration ?? 0;
                }
            }

            if (duration > 0)
            {
                return duration;
            }
        }

        return base.GetTotalDuration();
    }
}
