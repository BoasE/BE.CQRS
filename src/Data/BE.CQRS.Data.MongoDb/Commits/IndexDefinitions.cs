using System.Collections.Generic;
using MongoDB.Driver;

namespace BE.CQRS.Data.MongoDb.Commits
{
    public static class IndexDefinitions
    {
        public static IEnumerable<CreateIndexModel<EventCommit>> ProvideIndexModels()
        {
            return new List<CreateIndexModel<EventCommit>>
            {
                new CreateIndexModel<EventCommit>(
                    Builders<EventCommit>.IndexKeys.Descending(x => x.Ordinal), new CreateIndexOptions {Unique = true}),

                new CreateIndexModel<EventCommit>(
                    Builders<EventCommit>.IndexKeys.Descending(x => x.AggregateId)),

                new CreateIndexModel<EventCommit>(Builders<EventCommit>.IndexKeys.Descending(x => x.AggregateType)),

                new CreateIndexModel<EventCommit>(Builders<EventCommit>.IndexKeys
                    .Descending(x => x.AggregateId).Descending(x => x.AggregateType)),

                new CreateIndexModel<EventCommit>(
                    Builders<EventCommit>.IndexKeys.Descending(x => x.AggregateId).Descending(x => x.AggregateType)
                        .Descending(x => x.VersionEvents), new CreateIndexOptions {Unique = true}),

                new CreateIndexModel<EventCommit>(
                    Builders<EventCommit>.IndexKeys.Descending(x => x.AggregateId).Descending(x => x.AggregateType)
                        .Descending(x => x.VersionCommit), new CreateIndexOptions {Unique = true}),

                new CreateIndexModel<EventCommit>(
                    Builders<EventCommit>.IndexKeys.Descending(x => x.AggregateId).Descending(x => x.AggregateType)
                        .Ascending(x => x.AllEventTypes)),

                new CreateIndexModel<EventCommit>(
                    Builders<EventCommit>.IndexKeys.Descending(x => x.AggregateId).Descending(x => x.AggregateType)
                        .Ascending(x => x.AllEventTypes).Descending(x => x.Ordinal),new CreateIndexOptions {Unique = true}),
                
                new CreateIndexModel<EventCommit>(
                    Builders<EventCommit>.IndexKeys.Descending(x => x.AggregateId).Descending(x => x.AggregateType)
                    .Descending(x => x.Ordinal), new CreateIndexOptions {Unique = true})
            };
        }
    }
}