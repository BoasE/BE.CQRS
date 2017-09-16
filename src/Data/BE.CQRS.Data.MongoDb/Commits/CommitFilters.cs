using MongoDB.Driver;

namespace BE.CQRS.Data.MongoDb.Commits
{
    internal static class CommitFilters
    {
        private static FilterDefinitionBuilder<EventCommit> Filters { get; } = Builders<EventCommit>.Filter;

        internal static FilterDefinition<EventCommit> ByAggregate(string type, string id)
        {
            FilterDefinition<EventCommit> query = Filters.And(Filters.Eq(x => x.AggregateType, type), Filters.Eq(x => x.AggregateId, id));
            return query;
        }

        internal static FilterDefinition<EventCommit> StartingAtOrdinal(long ordinal)
        {
            return Filters.Gte(x => x.Ordinal, ordinal);
        }
    }
}