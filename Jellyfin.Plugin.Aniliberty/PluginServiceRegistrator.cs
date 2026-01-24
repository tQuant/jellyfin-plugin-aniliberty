using Jellyfin.Plugin.Aniliberty.Providers;
using MediaBrowser.Controller;
using MediaBrowser.Controller.Plugins;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Jellyfin.Plugin.Aniliberty;

/// <inheritdoc />
public class PluginServiceRegistrator : IPluginServiceRegistrator
{
    /// <inheritdoc />
    public void RegisterServices(IServiceCollection serviceCollection, IServerApplicationHost applicationHost)
    {
        serviceCollection.AddSingleton((sp) => new AnilibertyApi(
            sp.GetRequiredService<IMemoryCache>(),
            sp.GetRequiredService<ILogger<AnilibertyApi>>()));
    }
}
