namespace Jellyfin.Plugin.Aniliberty.Models;

#pragma warning disable SA1300,CS1591

public class Image
{
    public string? src { get; set; }

    public string? preview { get; set; }

    public string? thumbnail { get; set; }

    public Image? optimized { get; set; }
}
