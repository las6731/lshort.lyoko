using LShort.Lyoko.DataAccess.MongoDb;
using LShort.Lyoko.Messaging.Abstractions;
using MongoDB.Driver;
using ILogger = Serilog.ILogger;

namespace RabbitConsumer.DataAccess;

public class StoredEventMessage : EventMessage, IDatabaseObject
{
    Guid IDatabaseObject.Id => this.Id;
}

[Collection("Events")]
public class EventRepository : MongoRepository<StoredEventMessage>
{
    public EventRepository(MongoSessionProvider sessionProvider, ILogger logger) : base(sessionProvider, logger)
    {
    }

    protected override void EnsureIndexes()
    {
        var indexes = Builders<StoredEventMessage>.IndexKeys
            .Ascending(e => e.AggregateId)
            .Ascending(e => e.AggregateVersion);
        var options = new CreateIndexOptions()
        {
            Name = "ux_aggregateId_aggregateVersion",
            Unique = true
        };

        this.Collection.Indexes.CreateOne(new CreateIndexModel<StoredEventMessage>(indexes, options));
    }
}