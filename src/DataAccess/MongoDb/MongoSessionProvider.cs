using LShort.Lyoko.DataAccess.Abstractions.Configuration;
using MongoDB.Driver;

namespace LShort.Lyoko.DataAccess.MongoDb;

public class MongoSessionProvider : IAsyncDisposable
{
    private readonly IDatabaseConfiguration config;

    public readonly IClientSessionHandle Session;

    public IMongoDatabase Database => this.Session.Client.GetDatabase(this.config.Database);

    public MongoSessionProvider(MongoDatabaseConnectivityProvider connectivityProvider, IDatabaseConfiguration config)
    {
        this.Session = connectivityProvider.Client.StartSession();
        this.config = config;
    }

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        if (this.Session.IsInTransaction) await this.Session.AbortTransactionAsync();

        this.Session.Dispose();
    }
}