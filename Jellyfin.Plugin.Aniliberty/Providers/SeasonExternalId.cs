using MediaBrowser.Controller.Entities.TV;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Providers;

namespace Jellyfin.Plugin.Aniliberty.Providers;

/// <inheritdoc />
public class SeasonExternalId : IExternalId
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
        => ExternalIdMediaType.Season;

    /// <inheritdoc />
    public bool Supports(IHasProviderIds item)
        => item is Season;
}
