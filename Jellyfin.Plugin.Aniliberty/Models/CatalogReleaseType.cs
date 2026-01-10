using System.Runtime.Serialization;

namespace Jellyfin.Plugin.Aniliberty.Models;

#pragma warning disable CS1591

public class CatalogReleaseType
{
    [DataMember(Name = "value")]
    public ReleaseType? Value { get; set; }

    [DataMember(Name = "description")]
    public string? Description { get; set; }
}
