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

public class SeriesProvider(ILogger<SeriesProvider> logger, AnilibertyApi api) : IRemoteMetadataProvider<Series, SeriesInfo>, IHasOrder
{
    private readonly Resolver _resolver = new();

    /// <inheritdoc />
    public int Order => 0;

    /// <inheritdoc />
    public string Name => "Aniliberty";

    /// <inheritdoc />
    public async Task<MetadataResult<Series>> GetMetadata(SeriesInfo info, CancellationToken cancellationToken)
    {
        if (Plugin.Instance == null)
        {
            throw new InvalidOperationException("AnilibertyPlugin instance is not set");
        }

        var config = Plugin.Instance.Configuration;
        var result = new MetadataResult<Series>();
        var id = info.ProviderIds.GetValueOrDefault(SeriesExternalId.ProviderKey);
        var logKey = new Random().Next();

        CatalogRelease? release = null;
        if (!string.IsNullOrEmpty(id))
        {
            logger.LogInformation("Aniliberty...[{Key}]... Searching by id({Id})", logKey, id);
            release = await api.GetRelease(id, cancellationToken).ConfigureAwait(false);
        }
        else
        {
            logger.LogInformation("Aniliberty...[{Key}]... Searching by name({Id}, {Year})", logKey, !string.IsNullOrEmpty(info.OriginalTitle) ? info.OriginalTitle : info.Name, info.Year);
            var releases = await api.SearchReleases(info.Name, info.Year, config, cancellationToken).ConfigureAwait(false);
            if (releases.Count > 0)
            {
                logger.LogInformation("Aniliberty...[{Key}]... Found {X} releases", logKey, releases.Count);
                release = _resolver.FilterSeries(releases, info.OriginalTitle, info.Name);
            }
        }

        if (release is not null)
        {
            logger.LogInformation("Aniliberty...[{Key}]... Found release: {Name} / {Name2}", logKey, release.Name?.Main, release.Name?.English);
            result.HasMetadata = true;
            result.QueriedById = id != null;
            result.Item = release.ToSeries();
            result.Provider = SeriesExternalId.ProviderKey;
        }
        else
        {
            logger.LogInformation("Aniliberty...[{Key}]... No matches", logKey);
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
            CatalogRelease? release = await api.GetRelease(aid, cancellationToken).ConfigureAwait(false);
            if (release is not null)
            {
                results.Add(release.ToSearchResult(config));
            }
        }

        if (!string.IsNullOrEmpty(searchInfo.Name))
        {
            logger.LogInformation("Aniliberty... Searching by name({Id}, {Year})", searchInfo.Name, searchInfo.Year);
            var releases = await api.SearchReleases(searchInfo.Name, searchInfo.Year, config, cancellationToken).ConfigureAwait(false);
            foreach (var release in releases)
            {
                results.Add(release.ToSearchResult(config));
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
