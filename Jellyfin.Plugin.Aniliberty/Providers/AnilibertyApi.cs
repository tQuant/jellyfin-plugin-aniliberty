using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Jellyfin.Plugin.Aniliberty.Models;
using MediaBrowser.Model.Tasks;

namespace Jellyfin.Plugin.Aniliberty.Providers;

#pragma warning disable CS1591

public class AnilibertyApi
{
    private const string BaseApiUrl = "https://aniliberty.top/api/v1";

    /// <summary>
    /// Возвращает информацию о релизе по его ID.
    /// </summary>
    /// <param name="id">ID релиза.</param>
    /// <param name="cancellationToken">CancellationToken.</param>
    /// <returns>CatalogRelease.</returns>
    public async Task<CatalogRelease?> GetRelease(string id, CancellationToken cancellationToken)
    {
        return await WebRequest<CatalogRelease>("/anime/releases/" + id, cancellationToken).ConfigureAwait(false);
    }

    public async Task<List<CatalogRelease>> SearchReleases(string name, int? year, CancellationToken cancellationToken)
    {
        var response = await WebRequest<CatalogList>("/anime/catalog/releases?limit=2&f[search]=" + name + "&f[years][from_year]=" + (year != null ? year : string.Empty), cancellationToken).ConfigureAwait(false);
        if (response == null)
        {
            return new List<CatalogRelease>();
        }

        return response.Data;
    }

    public async Task<T?> WebRequest<T>(string path, CancellationToken cancellationToken)
    {
        if (Plugin.Instance == null)
        {
            throw new InvalidOperationException("AnilibertyPlugin instance is not set");
        }

        var httpClient = Plugin.Instance.GetHttpClient();

        using var response = await httpClient.GetAsync(BaseApiUrl + path, cancellationToken).ConfigureAwait(false);
        using var responseStream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);

        return await JsonSerializer.DeserializeAsync<T>(responseStream, cancellationToken: cancellationToken).ConfigureAwait(false);
    }
}
