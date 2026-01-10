using MediaBrowser.Model.Plugins;

namespace Jellyfin.Plugin.Aniliberty.Configuration;

/// <summary>
/// Plugin configuration.
/// </summary>
public class PluginConfiguration : BasePluginConfiguration
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PluginConfiguration"/> class.
    /// </summary>
    public PluginConfiguration()
    {
        // set default options here
        AntiBlock = true;
        ApiHost = "https://aniliberty.top";
    }

    /// <summary>
    /// Gets or sets a value indicating whether some true or false setting is enabled..
    /// </summary>
    public bool AntiBlock { get; set; }

    /// <summary>
    /// Gets or sets a string setting.
    /// </summary>
    public string ApiHost { get; set; }
}
