using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Jellyfin.Plugin.Aniliberty.Models;
using MediaBrowser.Controller.Entities.TV;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Providers;
using Microsoft.Extensions.Logging;

namespace Jellyfin.Plugin.Aniliberty.Providers;

#pragma warning disable CS1591

public class SeriesProvider(ILogger<SeriesProvider> logger) : IRemoteMetadataProvider<Series, SeriesInfo>, IHasOrder
{
    private readonly AnilibertyApi _api = new();

    /// <inheritdoc />
    public int Order => -2;

    /// <inheritdoc />
    public string Name => "Aniliberty";

    /// <inheritdoc />
    public async Task<MetadataResult<Series>> GetMetadata(SeriesInfo info, CancellationToken cancellationToken)
    {
        var result = new MetadataResult<Series>();
        var id = info.ProviderIds.GetValueOrDefault(SeriesExternalId.ProviderKey);
        CatalogRelease? release = null;
        if (!string.IsNullOrEmpty(id))
        {
            logger.LogInformation("Aniliberty... Searching by id({Id})", id);
            release = await _api.GetRelease(id, cancellationToken).ConfigureAwait(false);
        }
        else
        {
            logger.LogInformation("Aniliberty... Searching by name({Id}, {Year})", info.Name, info.Year);
            var releases = await _api.SearchReleases(info.Name, info.Year, cancellationToken).ConfigureAwait(false);
            if (releases.Count > 0)
            {
                release = releases[0];
            }
        }

        if (release is not null)
        {
            result.HasMetadata = true;
            result.QueriedById = id != null;
            result.Item = release.ToSeries();
            result.Provider = SeriesExternalId.ProviderKey;
        }

        return result;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<RemoteSearchResult>> GetSearchResults(SeriesInfo searchInfo, CancellationToken cancellationToken)
    {
        if (Plugin.Instance == null)
        {
            throw new InvalidOperationException("AnilibertyPlugin instance is not set");
        }

        var config = Plugin.Instance.Configuration;
        var results = new List<RemoteSearchResult>();

        var aid = searchInfo.ProviderIds.GetValueOrDefault(SeriesExternalId.ProviderKey);
        if (!string.IsNullOrEmpty(aid))
        {
            logger.LogInformation("Aniliberty... Searching by id({Id})", aid);
            CatalogRelease? release = await _api.GetRelease(aid, cancellationToken).ConfigureAwait(false);
            if (release is not null)
            {
                results.Add(release.ToSearchResult(config.ApiHost));
            }
        }

        if (!string.IsNullOrEmpty(searchInfo.Name))
        {
            logger.LogInformation("Aniliberty... Searching by name({Id}, {Year})", searchInfo.Name, searchInfo.Year);
            var releases = await _api.SearchReleases(searchInfo.Name, searchInfo.Year, cancellationToken).ConfigureAwait(false);
            foreach (var release in releases)
            {
                results.Add(release.ToSearchResult(config.ApiHost));
            }
        }

        return results;
    }

    /// <inheritdoc />
    public async Task<HttpResponseMessage> GetImageResponse(string url, CancellationToken cancellationToken)
    {
        if (Plugin.Instance == null)
        {
            throw new InvalidOperationException("AnilibertyPlugin instance is not set");
        }

        var httpClient = Plugin.Instance.GetHttpClient();

        return await httpClient.GetAsync(url, cancellationToken).ConfigureAwait(false);
    }
}
