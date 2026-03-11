using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Jellyfin.Plugin.Aniliberty.Models;
using Microsoft.Extensions.Logging;

namespace Jellyfin.Plugin.Aniliberty.Providers;

#pragma warning disable CA1002

/// <summary>
/// .
/// </summary>
public class Resolver
{
    /// <summary>
    /// .
    /// </summary>
    /// <param name="catalogReleases">Список релизов.</param>
    /// <param name="originalTitle">Оригинальное название.</param>
    /// <param name="name">Переведенное название.</param>
    /// <returns>Подходящий релиз.</returns>
    public CatalogRelease? FilterSeries(List<CatalogRelease> catalogReleases, string? originalTitle, string? name)
    {
        originalTitle = originalTitle != null ? RemoveSpecialCharacters(originalTitle) : null;
        name = name != null ? RemoveSpecialCharacters(name) : null;

        // Сначала ищем релиз по оригинальному названию (release.Name?.English)
        foreach (var release in catalogReleases)
        {
            if (!string.IsNullOrEmpty(release.Name?.English))
            {
                var releaseName = RemoveSpecialCharacters(release.Name.English);
                // logger.LogInformation("Aniliberty... Filter by name({OriginalTitle} or {Name} = {ReleaseName})", originalTitle, name, releaseName);
                if (
                    string.Equals(releaseName, originalTitle, StringComparison.OrdinalIgnoreCase)
                    || string.Equals(releaseName, name, StringComparison.OrdinalIgnoreCase)
                )
                {
                    return release;
                }
            }
        }

        // Если таких нет, то смотрим русское название (release.Name?.Main)
        foreach (var release in catalogReleases)
        {
            if (!string.IsNullOrEmpty(release.Name?.Main))
            {
                var releaseName = RemoveSpecialCharacters(release.Name.Main);
                // logger.LogInformation("Aniliberty... Filter by name({OriginalTitle} or {Name} = {ReleaseName})", originalTitle, name, releaseName);
                if (
                    string.Equals(releaseName, originalTitle, StringComparison.OrdinalIgnoreCase)
                    || string.Equals(releaseName, name, StringComparison.OrdinalIgnoreCase)
                )
                {
                    return release;
                }
            }
        }

        return null;
    }

    /// <summary>
    /// .
    /// </summary>
    /// <param name="catalogReleases">Список релизов.</param>
    /// <param name="originalTitle">Оригинальное название.</param>
    /// <param name="seasonNumber">Номер сезонаs.</param>
    /// <param name="logger">Logger.</param>
    /// <returns>Подходящий релиз.</returns>
    public CatalogRelease? FilterSeason(List<CatalogRelease> catalogReleases, string originalTitle, int seasonNumber, ILogger logger)
    {
        var pattern = Regex.Replace(Regex.Escape(originalTitle), "(?:\\\\ )*(:|-|,|!|\\.)(?:\\\\ )*", "\\s*$1?\\s*")
                      + "\\s+(Season\\s+)?" + seasonNumber + "(\\w{1,2})?(\\s+Season)?";
        logger.LogInformation("Aniliberty... Compare with pattern: {Pattern}", pattern);

        // Сравниваем релизы по оригинальному названию
        foreach (var release in catalogReleases)
        {
            if (!string.IsNullOrEmpty(release.Name?.English) && Regex.IsMatch(release.Name.English, pattern, RegexOptions.IgnoreCase))
            {
                return release;
            }
        }

        return null;
    }

    private string RemoveSpecialCharacters(string name)
    {
        return Regex.Replace(Regex.Replace(name, "(?:\\\\ )*(:|-|,|!|\\.)(?:\\\\ )*", " "), "\\s{2,}", " ");
    }
}
