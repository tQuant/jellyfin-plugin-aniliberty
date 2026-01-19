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
public class ImageProvider(ILogger<SeriesProvider> logger, IHttpClientFactory httpClientFactory) : IRemoteImageProvider, IHasOrder
{
    private readonly AnilibertyApi _api = new();

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
        var id = item.GetProviderId(SeriesExternalId.ProviderKey);
        if (string.IsNullOrWhiteSpace(id))
        {
            return Enumerable.Empty<RemoteImageInfo>();
        }

        logger.LogInformation("Aniliberty... Searching by id({Id}) for image", id);
        var release = await _api.GetRelease(id, cancellationToken).ConfigureAwait(false);

        if (string.IsNullOrEmpty(release?.Poster?.src))
        {
            return Enumerable.Empty<RemoteImageInfo>();
        }

        logger.LogInformation("Aniliberty... image found");
        // the poster url is sometimes higher quality than the poster api
        return new[]
        {
            new RemoteImageInfo
            {
                ProviderName = Name,
                Url = config.ApiHost + release.Poster.src
            }
        };
    }

    /// <inheritdoc />
    public Task<HttpResponseMessage> GetImageResponse(string url, CancellationToken cancellationToken)
    {
        return httpClientFactory.CreateClient(NamedClient.Default).GetAsync(url, cancellationToken);
    }

    /// <inheritdoc />
    public bool Supports(BaseItem item)
        => item is Series || item is Movie;
}
