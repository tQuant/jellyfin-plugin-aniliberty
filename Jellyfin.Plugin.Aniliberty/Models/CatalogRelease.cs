using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.Json.Serialization;
using Jellyfin.Plugin.Aniliberty.Configuration;
using Jellyfin.Plugin.Aniliberty.Providers;
using MediaBrowser.Controller.Entities.TV;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Providers;

#pragma warning disable CA1002
#pragma warning disable CS1591

namespace Jellyfin.Plugin.Aniliberty.Models;

public class CatalogRelease
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("type")]
    public CatalogReleaseType? Type { get; set; }

    [JsonPropertyName("year")]
    public int? Year { get; set; }

    [JsonPropertyName("name")]
    public CatalogReleaseName? Name { get; set; }

    // public string? alias { get; set; }
    // public Object season { get; set; }

    [JsonPropertyName("poster")]
    public Image? Poster { get; set; }

    // public string? fresh_at { get; set; }
    // public string? created_at { get; set; }
    // public string? updated_at { get; set; }

    [JsonPropertyName("is_ongoing")]
    public bool? IsOngoing { get; set; }

    [JsonPropertyName("age_rating")]
    public AgeRating? AgeRating { get; set; }

    [JsonPropertyName("publish_day")]
    public PublishDay? PublishDay { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    // public string? notification { get; set; }
    // public int? episodes_total { get; set; }
    // public string? external_player { get; set; }

    [JsonPropertyName("is_in_production")]
    public bool? IsInProduction { get; set; }

    // public bool? is_blocked_by_geo { get; set; }
    // public bool? is_blocked_by_copyrights { get; set; }

    [JsonPropertyName("average_duration_of_episode")]
    public float? AverageDurationOfEpisode { get; set; }

    // public int? added_in_users_favorites { get; set; }
    // public int? added_in_planned_collection { get; set; }
    // public int? added_in_watched_collection { get; set; }
    // public int? added_in_watching_collection { get; set; }
    // public int? added_in_postponed_collection { get; set; }
    // public int? added_in_abandoned_collection { get; set; }

    [JsonPropertyName("genres")]
    public List<Genre>? Genres { get; init; }

    /// <summary>
    /// Преобразует релиз в Series.
    /// </summary>
    /// <returns>Series.</returns>
    public Series ToSeries()
    {
        return new Series
        {
            Name = Name?.Main,
            OriginalTitle = Name?.English,
            Overview = Description,
            ProductionYear = Year,
            OfficialRating = AgeRating?.Label,
            Status = IsInProduction.HasValue && IsInProduction.Value ? SeriesStatus.Unreleased : (IsOngoing.HasValue ? (IsOngoing.Value ? SeriesStatus.Continuing : SeriesStatus.Ended) : null),
            Genres = Genres?.Select(genre => genre.Name).ToArray(),
            RunTimeTicks = AverageDurationOfEpisode.HasValue ? TimeSpan.FromMinutes((double)AverageDurationOfEpisode).Ticks : null,
            ProviderIds = new Dictionary<string, string>() { { SeriesExternalId.ProviderKey, Id.ToString(CultureInfo.InvariantCulture) } }
        };
    }

    /// <summary>
    /// Преобразует релиз в Season.
    /// </summary>
    /// <param name="indexNumber">Номер сезона.</param>
    /// <returns>Series.</returns>
    public Season ToSeason(int indexNumber)
    {
        return new Season
        {
            IndexNumber = indexNumber,
            Name = Name?.Main,
            OriginalTitle = Name?.English,
            Overview = Description,
            ProductionYear = Year,
            OfficialRating = AgeRating?.Label,
            Genres = Genres?.Select(genre => genre.Name).ToArray(),
            RunTimeTicks = AverageDurationOfEpisode.HasValue ? TimeSpan.FromMinutes((double)AverageDurationOfEpisode).Ticks : null,
            ProviderIds = new Dictionary<string, string>() { { SeasonExternalId.ProviderKey, Id.ToString(CultureInfo.InvariantCulture) } }
        };
    }

    /// <summary>
    /// Преобразует релиз в RemoteSearchResult.
    /// </summary>
    /// <param name="config">Plugin config.</param>
    /// <returns>RemoteSearchResult.</returns>
    public RemoteSearchResult ToSearchResult(PluginConfiguration config)
    {
        string? image = config.AntiBlock
            ? (Poster?.optimized?.thumbnail ?? Poster?.thumbnail ?? null)
            : Poster?.src;
        return new RemoteSearchResult
        {
            Name = Name?.Main,
            Overview = Description,
            ProductionYear = Year,
            ImageUrl = image != null ? config.ApiHost + image : null,
            SearchProviderName = SeriesExternalId.ProviderKey,
            ProviderIds = new Dictionary<string, string>() { { SeriesExternalId.ProviderKey, Id.ToString(CultureInfo.InvariantCulture) } }
        };
    }
}
