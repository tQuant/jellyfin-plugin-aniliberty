using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Jellyfin.Plugin.Aniliberty.Models;

#pragma warning disable CA1002

/// <summary>
/// Ответ АПИ со списком релизов.
/// </summary>
public class CatalogList
{
    /// <summary>
    /// Список релизов.
    /// </summary>
    [JsonPropertyName("data")]
    public required List<CatalogRelease> Data { get; init; }
}
