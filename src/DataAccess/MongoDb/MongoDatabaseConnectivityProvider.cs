using LShort.Lyoko.DataAccess.Abstractions.Configuration;
using MongoDB.Driver;

namespace LShort.Lyoko.DataAccess.MongoDb;

public class MongoDatabaseConnectivityProvider
{
    private readonly IDatabaseConfiguration config;

    public readonly IMongoClient Client;

    /// <summary>
    /// Initializes a new instance of the <see cref="MongoDatabaseConnectivityProvider"/> class.
    /// </summary>
    /// <param name="config">The database configuration.</param>
    public MongoDatabaseConnectivityProvider(IDatabaseConfiguration config)
    {
        this.config = config;

        this.Client = new MongoClient($"mongodb://{this.config.Username}:{this.config.Password}@{this.config.Host}");
    }
}