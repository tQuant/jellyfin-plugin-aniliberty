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
/// <param name="api">AnilibertyApi.</param>
public class SeasonProvider(ILogger<SeriesProvider> logger, IHttpClientFactory httpClientFactory, AnilibertyApi api) : IRemoteMetadataProvider<Season, SeasonInfo>, IHasOrder
{
    private readonly Resolver _resolver = new();

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

        var result = new MetadataResult<Season>();

        if (info.IndexNumber == null || info.IndexNumber < 1)
        {
            return result;
        }

        var config = Plugin.Instance.Configuration;
        var id = info.ProviderIds.GetValueOrDefault(SeasonExternalId.ProviderKey);
        var parentId = info.SeriesProviderIds.GetValueOrDefault(SeriesExternalId.ProviderKey);
        var logKey = new Random().Next();

        if (string.IsNullOrEmpty(id) && string.IsNullOrEmpty(parentId))
        {
            return result;
        }

        CatalogRelease? release = null;
        if (!string.IsNullOrEmpty(id))
        {
            logger.LogInformation("Aniliberty...[{Key}]... Searching season by id({Id})", logKey, id);
            release = await api.GetRelease(id, cancellationToken).ConfigureAwait(false);
        }
        else if (!string.IsNullOrEmpty(parentId))
        {
            logger.LogInformation("Aniliberty...[{Key}]... Search season parent release({Id})", logKey, parentId);
            var parent = await api.GetRelease(parentId, cancellationToken).ConfigureAwait(false);
            if (parent is null)
            {
                logger.LogInformation("Aniliberty...[{Key}]... Not found", logKey);
                return result;
            }

            if (parent.Name is null || string.IsNullOrEmpty(parent.Name?.English))
            {
                logger.LogInformation("Aniliberty...[{Key}]... parent release has empty orig name", logKey);
                return result;
            }

            // Первый сезон совпадает с родительским релизом
            if (info.IndexNumber == 1)
            {
                release = parent;
            }
            else
            {
                logger.LogInformation("Aniliberty...[{Key}]... Searching season by name and saeson index({Name}, {Index})", logKey, parent.Name?.English, info.IndexNumber);
                var releases = await api.SearchReleases(parent.Name?.English + " " + info.IndexNumber, null, config, cancellationToken).ConfigureAwait(false);
                if (releases.Count == 0)
                {
                    logger.LogInformation("Aniliberty...[{Key}]... Not found", logKey);
                    return result;
                }

                logger.LogInformation("Aniliberty...[{Key}]... Found {X} releases", logKey, releases.Count);
#pragma warning disable CS8604 // Possible null reference argument.
                release = _resolver.FilterSeason(releases, parent.Name?.English, (int)info.IndexNumber, logger);
#pragma warning restore CS8604 // Possible null reference argument.
            }
        }

        if (release is not null)
        {
            logger.LogInformation("Aniliberty...[{Key}]... Found release {Name}", logKey, release.Name?.English);
            result.HasMetadata = true;
            result.QueriedById = !string.IsNullOrEmpty(id);
            result.Item = release.ToSeason((int)info.IndexNumber);
            result.Provider = SeasonExternalId.ProviderKey;
        }
        else
        {
            logger.LogInformation("Aniliberty...[{Key}]... No matches", logKey);
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
