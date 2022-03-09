using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace LShort.Lyoko.Common.Extensions;

/// <summary>
/// Extension methods that are useful for hosting applications.
/// </summary>
public static class HostingExtensions
{
    /// <summary>
    /// Add Serilog to the application, configured from app settings.
    /// </summary>
    /// <param name="builder">The application builder.</param>
    /// <param name="configureLogging">An optional method to perform further configuration.</param>
    /// <returns>The application builder with Serilog configured.</returns>
    public static WebApplicationBuilder UseSerilog(this WebApplicationBuilder builder, Action<IConfiguration, LoggerConfiguration>? configureLogging = null)
    {
        builder.Host.UseSerilog((ctx, cfg) =>
        {
            cfg.ReadFrom.Configuration(ctx.Configuration);
            configureLogging?.Invoke(ctx.Configuration, cfg);
        });

        return builder;
    }
}