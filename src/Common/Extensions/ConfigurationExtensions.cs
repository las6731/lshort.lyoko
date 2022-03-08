using Microsoft.Extensions.Configuration;

namespace LShort.Lyoko.Common.Extensions;

public static class ConfigurationExtensions
{
    /// <summary>
    /// Bind the configuration to a new instance of <see cref="T"/>.
    /// </summary>
    /// <param name="configuration">The configuration.</param>
    /// <typeparam name="T">The type to bind the configuration to.</typeparam>
    /// <returns>The bound <see cref="T"/>.</returns>
    public static T BindAs<T>(this IConfiguration configuration)
        where T : new()
    {
        var config = new T();
        configuration.Bind(config);

        return config;
    }
}