using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Providers;
using Episode = MediaBrowser.Controller.Entities.TV.Episode;

namespace Jellyfin.Plugin.Aniliberty.Providers;

/// <inheritdoc />
public class EpisodeExternalId : IExternalId
{
    /// <summary>
    /// Название провайдера.
    /// </summary>
    public const string ProviderKey = "Aniliberty";

    /// <inheritdoc />
    public string ProviderName
        => "Aniliberty";

    /// <inheritdoc />
    public string Key
        => ProviderKey;

    /// <inheritdoc />
    public ExternalIdMediaType? Type
        => ExternalIdMediaType.Episode;

    /// <inheritdoc />
    public bool Supports(IHasProviderIds item)
        => item is Episode;
}
