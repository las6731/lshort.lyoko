using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace LShort.Lyoko.DataAccess.Postgres.Extensions;

/// <summary>
///
/// </summary>
public static class StartupExtensions
{
    /// <summary>
    /// Add an Entity Framework <see cref="DbContext"/> using Postgres.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="config">The config section containing the database connection strings.</param>
    /// <typeparam name="TContext">the <see cref="DbContext"/> to add.</typeparam>
    /// <returns>The service collection.</returns>
    public static IServiceCollection AddPostgresDbContext<TContext>(this IServiceCollection services, IConfiguration config)
        where TContext : DbContext
    {
        services.AddDbContext<TContext>(options =>
            options.UseNpgsql(config.GetConnectionString(typeof(TContext).Name)));

        return services;
    }

    /// <summary>
    /// Ensure the databases for all registered <see cref="DbContext"/>.
    /// </summary>
    /// <param name="services">The service provider.</param>
    /// <returns>The service provider.</returns>
    public static IServiceProvider EnsureDbContexts(this IServiceProvider services)
    {
        var logger = services.GetRequiredService<ILogger>();
        var contextTypes = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => a
                .GetTypes()
                .Where(t => t.IsSubclassOf(typeof(DbContext))));

        foreach (var type in contextTypes)
        {
            var context = (DbContext) services.GetService(type);
            if (context is null) continue;
            context.Database.EnsureCreated();
            logger.ForContext("dbContext", type.Name).Information("Postgres database ensured.");
        }

        return services;
    }
}