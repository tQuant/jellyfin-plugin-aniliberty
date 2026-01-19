using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Jellyfin.Plugin.Aniliberty.Models;
using MediaBrowser.Common.Net;
using MediaBrowser.Controller.Entities.TV;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Providers;
using Microsoft.Extensions.Logging;

namespace Jellyfin.Plugin.Aniliberty.Providers;

/// <summary>
/// Provide season info.
/// </summary>
/// <param name="logger">ILogger.</param>
/// <param name="httpClientFactory">IHttpClientFactory.</param>
public class SeasonProvider(ILogger<SeriesProvider> logger, IHttpClientFactory httpClientFactory) : IRemoteMetadataProvider<Season, SeasonInfo>, IHasOrder
{
    private readonly AnilibertyApi _api = new();

    /// <inheritdoc />
    public string Name => "Aniliberty";

    /// <inheritdoc />
    public int Order => 0;

    /// <inheritdoc />
    public async Task<MetadataResult<Season>> GetMetadata(SeasonInfo info, CancellationToken cancellationToken)
    {
        if (Plugin.Instance == null)
        {
            throw new InvalidOperationException("AnilibertyPlugin instance is not set");
        }

        var config = Plugin.Instance.Configuration;
        var id = info.ProviderIds.GetValueOrDefault(SeasonExternalId.ProviderKey);
        CatalogRelease? release = null;
        if (!string.IsNullOrEmpty(id))
        {
            logger.LogInformation("Aniliberty... Searching season by id({Id})", id);
            release = await _api.GetRelease(id, cancellationToken).ConfigureAwait(false);
        }
        else
        {
            logger.LogInformation("Aniliberty... Searching season by name({Id}, {Year})", info.Name, info.Year);
            var releases = await _api.SearchReleases(info.Name, info.Year, config, cancellationToken).ConfigureAwait(false);
            if (releases.Count > 0)
            {
                logger.LogInformation("Aniliberty... Found {X} releases", releases.Count);
                release = releases[0];
            }
        }

        var result = new MetadataResult<Season>();
        if (release is not null)
        {
            logger.LogInformation("Aniliberty... Found release {Name}, but ignore for now", release.Name);
            // result.HasMetadata = true;
            // result.QueriedById = id != null;
            // result.Item = new Season { IndexNumber = seasonNumber, Overview = seasonResult.Overview, PremiereDate = seasonResult.AirDate, ProductionYear = seasonResult.AirDate?.Year };
            // result.Provider = SeasonExternalId.ProviderKey;
        }
        else
        {
            logger.LogInformation("Aniliberty... No results");
        }

        return result;
    }

    /// <inheritdoc />
    public Task<IEnumerable<RemoteSearchResult>> GetSearchResults(SeasonInfo searchInfo, CancellationToken cancellationToken)
    {
        return Task.FromResult(Enumerable.Empty<RemoteSearchResult>());
    }

    /// <inheritdoc />
    public Task<HttpResponseMessage> GetImageResponse(string url, CancellationToken cancellationToken)
    {
        return httpClientFactory.CreateClient(NamedClient.Default).GetAsync(url, cancellationToken);
    }
}
