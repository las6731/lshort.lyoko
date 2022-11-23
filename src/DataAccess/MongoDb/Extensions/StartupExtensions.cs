using LShort.Lyoko.Common.Extensions;
using LShort.Lyoko.DataAccess.Abstractions.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LShort.Lyoko.DataAccess.MongoDb.Extensions;

public static class StartupExtensions
{
    public static IServiceCollection UseMongoDb(this IServiceCollection services, IConfiguration configuration)
    {
        return services.AddSingleton<IDatabaseConfiguration>(configuration.BindAs<DatabaseConfiguration>())
            .AddSingleton<MongoDatabaseConnectivityProvider>()
            .AddScoped<MongoSessionProvider>();
    }

    public static WebApplicationBuilder UseMongoDb(this WebApplicationBuilder builder)
    {
        builder.Services.UseMongoDb(builder.Configuration.GetSection("MongoDb"));

        return builder;
    }

    public static IServiceCollection AddRepository<TRepository, TObject>(this IServiceCollection services)
        where TRepository : MongoRepository<TObject>
        where TObject : IDatabaseObject
    {
        return services.AddTransient<TRepository>();
    }
}