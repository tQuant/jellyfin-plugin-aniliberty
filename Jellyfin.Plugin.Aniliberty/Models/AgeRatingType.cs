namespace Jellyfin.Plugin.Aniliberty.Models;

#pragma warning disable CS1591

public enum AgeRatingType
{
    [System.Runtime.Serialization.EnumMember(Value = @"R0_PLUS")]
    R0plus,

    [System.Runtime.Serialization.EnumMember(Value = @"R6_PLUS")]
    R6plus,

    [System.Runtime.Serialization.EnumMember(Value = @"R12_PLUS")]
    R12plus,

    [System.Runtime.Serialization.EnumMember(Value = @"R16_PLUS")]
    R16plus,

    [System.Runtime.Serialization.EnumMember(Value = @"R18_PLUS")]
    R18plus,
}
