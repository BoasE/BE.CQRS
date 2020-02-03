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
                new CreateIndexModel<EventCommit>(Builders<EventCommit>.IndexKeys
                        .Descending(x => x.Ordinal),
                    new CreateIndexOptions { Unique = true, Name = "cqrs_ord" }),

                new CreateIndexModel<EventCommit>(Builders<EventCommit>.IndexKeys
                        .Descending(x => x.AggregateId).Descending(x => x.AggregateType),
                    new CreateIndexOptions() { Name = "cqrs_aid_aty" }),

                new CreateIndexModel<EventCommit>(Builders<EventCommit>.IndexKeys
                        .Descending(x => x.AggregateId).Descending(x => x.AggregateType)
                        .Ascending(x => x.AllEventTypes),
                    new CreateIndexOptions() { Name = "cqrs_aid,aty_aet" }),

                new CreateIndexModel<EventCommit>(Builders<EventCommit>.IndexKeys
                        .Descending(x => x.AggregateId).Descending(x => x.AggregateType)
                        .Descending(x => x.Ordinal),
                    new CreateIndexOptions { Unique = true, Name = "cqrs_aid_aty_ord" })
            };
        }
    }
}