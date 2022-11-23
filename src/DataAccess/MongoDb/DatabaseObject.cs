using MongoDB.Bson.Serialization.Attributes;

namespace LShort.Lyoko.DataAccess.MongoDb;

public interface IDatabaseObject
{
    [BsonId]
    public Guid Id { get; }
}