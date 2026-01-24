using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using MediaBrowser.Common.Net;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Entities.Movies;
using MediaBrowser.Controller.Entities.TV;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Providers;
using Microsoft.Extensions.Logging;

namespace Jellyfin.Plugin.Aniliberty.Providers;

/// <summary>
/// Провайдер изображений.
/// </summary>
public class ImageProvider(ILogger<SeriesProvider> logger, IHttpClientFactory httpClientFactory, AnilibertyApi api) : IRemoteImageProvider, IHasOrder
{
    /// <inheritdoc />
    public int Order => 0;

    /// <inheritdoc />
    public string Name => "Aniliberty";

    /// <inheritdoc />
    public IEnumerable<ImageType> GetSupportedImages(BaseItem item)
    {
        yield return ImageType.Primary;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<RemoteImageInfo>> GetImages(BaseItem item, CancellationToken cancellationToken)
    {
        if (Plugin.Instance == null)
        {
            throw new InvalidOperationException("AnilibertyPlugin instance is not set");
        }

        var config = Plugin.Instance.Configuration;
        string? id = item is Episode
            ? item.GetProviderId(EpisodeExternalId.ProviderKey)
            : (item is Season
                ? item.GetProviderId(SeasonExternalId.ProviderKey)
                : item.GetProviderId(SeriesExternalId.ProviderKey)
            );
        if (string.IsNullOrWhiteSpace(id))
        {
            return Enumerable.Empty<RemoteImageInfo>();
        }

        logger.LogInformation("Aniliberty... Searching by id({Id}) for image", id);
        Models.Image? image = null;
        if (item is Episode)
        {
            var episode = await api.GetEpisode(id, cancellationToken).ConfigureAwait(false);
            image = episode?.Preview;
        }
        else
        {
            var release = await api.GetRelease(id, cancellationToken).ConfigureAwait(false);
            image = release?.Poster;
        }

        if (string.IsNullOrEmpty(image?.src))
        {
            return Enumerable.Empty<RemoteImageInfo>();
        }

        logger.LogInformation("Aniliberty... image found");
        return new[] { new RemoteImageInfo { ProviderName = Name, Url = config.ApiHost + image.src } };
    }

    /// <inheritdoc />
    public Task<HttpResponseMessage> GetImageResponse(string url, CancellationToken cancellationToken)
    {
        return httpClientFactory.CreateClient(NamedClient.Default).GetAsync(url, cancellationToken);
    }

    /// <inheritdoc />
    public bool Supports(BaseItem item)
        => item is Series || item is Movie || item is Season || item is Episode;
}
