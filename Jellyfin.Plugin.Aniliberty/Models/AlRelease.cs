using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Jellyfin.Plugin.Aniliberty.Models;

#pragma warning disable CS1591
#pragma warning disable CA1002

public class AlRelease : CatalogRelease
{
    [JsonPropertyName("episodes")]
    public required List<AlEpisode> Episodes { get; init; }

    // [JsonPropertyName("torrents")]
    // public required List<AlTorrent> Torrents { get; init; }
}
