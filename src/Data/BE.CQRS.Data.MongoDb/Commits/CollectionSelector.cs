using System.Collections.Concurrent;
using System.Threading.Tasks;
using BE.FluentGuard;
using MongoDB.Driver;

namespace BE.CQRS.Data.MongoDb.Commits
{
    public sealed class CollectionSelector
    {
        private static readonly IndexKeysDefinitionBuilder<EventCommit> Indexes = Builders<EventCommit>.IndexKeys;
        private readonly ConcurrentDictionary<string, IMongoCollection<EventCommit>> cache = new ConcurrentDictionary<string, IMongoCollection<EventCommit>>();
        private readonly IMongoDatabase db;

        public CollectionSelector(IMongoDatabase db)
        {
            this.db = db;
        }

        public IMongoCollection<EventCommit> GetCollectionByAggregateType(string streamName)
        {
            Precondition.For(streamName, nameof(streamName)).NotNullOrWhiteSpace();

            string name = GetCollectionNameByCommit(streamName);

            IMongoCollection<EventCommit> collection = cache.GetOrAdd(name, x => ResolveNew(name).Result);

            return collection;
        }

        private async Task<IMongoCollection<EventCommit>> ResolveNew(string name)
        {
            IMongoCollection<EventCommit> collection = db.GetCollection<EventCommit>(name);
            await PrepareCollection(collection);
            return collection;
        }

        private Task PrepareCollection(IMongoCollection<EventCommit> collection)
        {
            return Task.WhenAll(
                collection.Indexes.CreateOneAsync(Indexes.Descending(x => x.Ordinal), new CreateIndexOptions
                {
                    Unique = true
                }),
                collection.Indexes.CreateOneAsync(Indexes.Descending(x => x.AggregateId)),
                collection.Indexes.CreateOneAsync(Indexes.Descending(x => x.AggregateType)),
                collection.Indexes.CreateOneAsync(Indexes.Descending(x => x.AggregateId)
                    .Descending(x => x.AggregateType)),
                collection.Indexes.CreateOneAsync(
                    Indexes.Descending(x => x.AggregateId).Descending(x => x.AggregateType)
                        .Descending(x => x.VersionEvents),
                    new CreateIndexOptions
                    {
                        Unique = true
                    }),
                collection.Indexes.CreateOneAsync(
                    Indexes.Descending(x => x.AggregateId).Descending(x => x.AggregateType)
                        .Descending(x => x.VersionCommit),
                    new CreateIndexOptions
                    {
                        Unique = true
                    })
            );
        }

        public static string GetCollectionNameByCommit(string type)
        {
            return $"ag_{type}";
        }
    }
}