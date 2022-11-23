using System.Reflection;
using MongoDB.Driver;
using Serilog;

namespace LShort.Lyoko.DataAccess.MongoDb;

public abstract class MongoRepository<T>
    where T : IDatabaseObject
{
    protected readonly MongoSessionProvider SessionProvider;

    protected IMongoCollection<T> Collection =>
        this.SessionProvider.Database.GetCollection<T>(((Collection)GetType().GetCustomAttribute(typeof(Collection)))?.Name);

    protected readonly ILogger logger;

    public MongoRepository(MongoSessionProvider sessionProvider, ILogger logger)
    {
        this.SessionProvider = sessionProvider;
        this.logger = logger.ForContext<MongoRepository<T>>();

        this.EnsureIndexes();
    }

    protected abstract void EnsureIndexes();

    protected void TryStartTransaction()
    {
        try
        {
            this.SessionProvider.Session.StartTransaction();
        }
        catch (InvalidOperationException)
        {
            // if this fails, then a session has already been started
        }
    }

    public async Task<T?> Get(Guid id)
    {
        var result = (await this.Collection.FindAsync(o => o.Id == id)).FirstOrDefault();
        if (result is null) this.logger.Warning("Failed to find database object with id: {id}", id);

        return result;
    }

    public async Task<List<T>> GetAll()
    {
        return await this.Collection.AsQueryable().ToListAsync();
    }

    public async Task Insert(T o)
    {
        this.TryStartTransaction();

        await this.Collection.InsertOneAsync(this.SessionProvider.Session, o);
    }

    public async Task InsertMany(IEnumerable<T> objects)
    {
        this.TryStartTransaction();

        await this.Collection.InsertManyAsync(objects);
    }

    public async Task Update(T o)
    {
        this.TryStartTransaction();

        await this.Collection.ReplaceOneAsync(this.SessionProvider.Session, d => d.Id == o.Id, o, new ReplaceOptions { IsUpsert = true });
    }

    public Task UpdateMany(IEnumerable<T> objects)
    {
        this.TryStartTransaction();

        return Task.WhenAll(objects.Select(this.Update));
    }

    public async Task<bool> Delete(Guid id)
    {
        this.TryStartTransaction();

        var result = await this.Collection.DeleteOneAsync(o => o.Id == id);
        return result.IsAcknowledged;
    }

    public async Task CommitTransaction()
    {
        await this.SessionProvider.Session.CommitTransactionAsync();
    }

    public async Task Watch(Func<T, Task> action)
    {
        var pipeline = new EmptyPipelineDefinition<ChangeStreamDocument<T>>()
            .Match(x => x.OperationType == ChangeStreamOperationType.Insert);
        using var cursor = await this.Collection.WatchAsync(pipeline);
        await cursor.ForEachAsync(async change => await action(change.FullDocument));
    }
}