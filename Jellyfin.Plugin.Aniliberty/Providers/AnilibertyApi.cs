using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Jellyfin.Plugin.Aniliberty.Configuration;
using Jellyfin.Plugin.Aniliberty.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Jellyfin.Plugin.Aniliberty.Providers;

#pragma warning disable CS1591

public class AnilibertyApi(IMemoryCache cache, ILogger<AnilibertyApi> logger)
{
    /// <summary>
    /// Возвращает информацию о релизе по его ID.
    /// </summary>
    /// <param name="id">ID релиза.</param>
    /// <param name="cancellationToken">CancellationToken.</param>
    /// <returns>CatalogRelease.</returns>
    public async Task<AlRelease?> GetRelease(string id, CancellationToken cancellationToken)
    {
        return await CachedWebRequest<AlRelease>("/anime/releases/" + id, cancellationToken).ConfigureAwait(false);
    }

    public async Task<AlEpisode?> GetEpisode(string id, CancellationToken cancellationToken)
    {
        return await CachedWebRequest<AlEpisode>("/anime/releases/episodes/" + id, cancellationToken).ConfigureAwait(false);
    }

    public async Task<List<CatalogRelease>> SearchReleases(string name, int? year, PluginConfiguration config, CancellationToken cancellationToken)
    {
        var response = await CachedWebRequest<CatalogList>("/anime/catalog/releases?limit=" + (config.AntiBlock ? 2 : 10) + "&f[search]=" + name + "&f[years][from_year]=" + (year != null ? year : string.Empty), cancellationToken).ConfigureAwait(false);
        if (response == null)
        {
            return new List<CatalogRelease>();
        }

        return response.Data;
    }

    private Task<T?> CachedWebRequest<T>(string path, CancellationToken cancellationToken)
    {
        return cache.GetOrCreateAsync(path, entry =>
        {
            logger.LogInformation("Aniliberty... Entry '{Path}' not found in cache, requesting from server", path);
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);

            return WebRequest<T>(path, cancellationToken);
        });
    }

    private async Task<T?> WebRequest<T>(string path, CancellationToken cancellationToken)
    {
        if (Plugin.Instance == null)
        {
            throw new InvalidOperationException("AnilibertyPlugin instance is not set");
        }

        var httpClient = Plugin.Instance.GetHttpClient();
        var baseApiUrl = Plugin.Instance.Configuration.ApiHost + "/api/v1";

        using var response = await httpClient.GetAsync(baseApiUrl + path, cancellationToken).ConfigureAwait(false);
        using var responseStream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);

        return await JsonSerializer.DeserializeAsync<T>(responseStream, cancellationToken: cancellationToken).ConfigureAwait(false);
    }
}
