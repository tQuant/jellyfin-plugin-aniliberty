using System;
using System.Collections.Generic;
using System.Globalization;
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
/// Предоставляет информацию об эпизодах.
/// </summary>
/// <param name="logger">ILogger.</param>
/// <param name="httpClientFactory">IHttpClientFactory.</param>
/// <param name="api">AnilibertyApi.</param>
public class EpisodeProvider(ILogger<EpisodeProvider> logger, IHttpClientFactory httpClientFactory, AnilibertyApi api) : IRemoteMetadataProvider<Episode, EpisodeInfo>, IHasOrder
{
    /// <inheritdoc />
    public int Order => 0;

    /// <inheritdoc />
    public string Name => "Aniliberty";

    /// <inheritdoc />
    public async Task<MetadataResult<Episode>> GetMetadata(EpisodeInfo info, CancellationToken cancellationToken)
    {
        if (Plugin.Instance == null)
        {
            throw new InvalidOperationException("AnilibertyPlugin instance is not set");
        }

        var metadataResult = new MetadataResult<Episode>();
        // var config = Plugin.Instance.Configuration;

        // Allowing this will dramatically increase scan times
        if (info.IsMissingEpisode)
        {
            return metadataResult;
        }

        var logKey = new Random().Next();
        AlEpisode? episode = null;

        // ID эпизода
        info.ProviderIds.TryGetValue(EpisodeExternalId.ProviderKey, out string? id);
        if (!string.IsNullOrEmpty(id?.Trim()))
        {
            logger.LogInformation("Aniliberty...[{Key}]... Searching episode by id({Id})", logKey, id);
            episode = await api.GetEpisode(id, cancellationToken).ConfigureAwait(false);
        }

        if (episode == null)
        {
            info.SeasonProviderIds.TryGetValue(SeasonExternalId.ProviderKey, out string? seasonId);
            if (string.IsNullOrEmpty(seasonId?.Trim()))
            {
                return metadataResult;
            }

            var episodeNumber = info.IndexNumber;
            if (!episodeNumber.HasValue)
            {
                return metadataResult;
            }

            if (info.IndexNumberEnd.HasValue)
            {
                // Несколько эпизодов в одном. Пока пропускаем
                return metadataResult;
            }

            logger.LogInformation("Aniliberty...[{Key}]... Load release by id({Id}) for episode({Num})", logKey, id, episodeNumber.Value);
            var release = await api.GetRelease(seasonId, cancellationToken).ConfigureAwait(false);
            if (release is null)
            {
                return metadataResult;
            }

            foreach (var alEpisode in release.Episodes)
            {
                if (alEpisode.NumberIsRound() && alEpisode.Number() == episodeNumber.Value)
                {
                    episode = alEpisode;
                    break;
                }
            }
        }

        if (episode is null)
        {
            logger.LogInformation("Aniliberty...[{Key}]... No matches", logKey);
            return metadataResult;
        }

        logger.LogInformation("Aniliberty...[{Key}]... Found episode {Name}", logKey, episode.Name);
        metadataResult.HasMetadata = true;
        metadataResult.QueriedById = !string.IsNullOrEmpty(id);
        metadataResult.Item = new Episode
        {
            IndexNumber = episode.Number(),
            IndexNumberEnd = info.IndexNumberEnd,
            Name = episode.Name,
            ProviderIds = new Dictionary<string, string>() { { EpisodeExternalId.ProviderKey, episode.Id.ToString(CultureInfo.InvariantCulture) } }
        };
        metadataResult.Provider = EpisodeExternalId.ProviderKey;

        return metadataResult;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<RemoteSearchResult>> GetSearchResults(EpisodeInfo searchInfo, CancellationToken cancellationToken)
    {
        // The search query must either provide an episode number or date
        if (!searchInfo.IndexNumber.HasValue)
        {
            return Enumerable.Empty<RemoteSearchResult>();
        }

        var metadataResult = await GetMetadata(searchInfo, cancellationToken).ConfigureAwait(false);

        if (!metadataResult.HasMetadata)
        {
            return Enumerable.Empty<RemoteSearchResult>();
        }

        var item = metadataResult.Item;

        return new[]
        {
            new RemoteSearchResult
            {
                IndexNumber = item.IndexNumber,
                Name = item.Name,
                ParentIndexNumber = item.ParentIndexNumber,
                PremiereDate = item.PremiereDate,
                ProductionYear = item.ProductionYear,
                ProviderIds = item.ProviderIds,
                SearchProviderName = Name,
                IndexNumberEnd = item.IndexNumberEnd
            }
        };
    }

    /// <inheritdoc />
    public Task<HttpResponseMessage> GetImageResponse(string url, CancellationToken cancellationToken)
    {
        return httpClientFactory.CreateClient(NamedClient.Default).GetAsync(url, cancellationToken);
    }
}
