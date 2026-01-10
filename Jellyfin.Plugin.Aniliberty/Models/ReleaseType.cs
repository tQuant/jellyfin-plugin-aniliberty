namespace Jellyfin.Plugin.Aniliberty.Models;

#pragma warning disable CS1591

public enum ReleaseType
{
    [System.Runtime.Serialization.EnumMember(Value = @"TV")]
    TV,

    [System.Runtime.Serialization.EnumMember(Value = @"ONA")]
    ONA,

    [System.Runtime.Serialization.EnumMember(Value = @"WEB")]
    WEB,

    [System.Runtime.Serialization.EnumMember(Value = @"OVA")]
    OVA,

    [System.Runtime.Serialization.EnumMember(Value = @"OAD")]
    OAD,

    [System.Runtime.Serialization.EnumMember(Value = @"MOVIE")]
    MOVIE,

    [System.Runtime.Serialization.EnumMember(Value = @"DORAMA")]
    DORAMA,

    [System.Runtime.Serialization.EnumMember(Value = @"SPECIAL")]
    SPECIAL
}
