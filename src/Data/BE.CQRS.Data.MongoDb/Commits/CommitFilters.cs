using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Driver;

namespace BE.CQRS.Data.MongoDb.Commits
{
    internal static class CommitFilters
    {
        private static FilterDefinitionBuilder<EventCommit> Filters { get; } = Builders<EventCommit>.Filter;

        public static FilterDefinition<EventCommit> All => Filters.Empty;

        internal static FilterDefinition<EventCommit> ByAggregate(string type, string id)
        {
            FilterDefinition<EventCommit> query = Filters.And(Filters.Eq(x => x.AggregateType, type), Filters.Eq(x => x.AggregateId, id));
            return query;
        }

        internal static FilterDefinition<EventCommit> StartingAtOrdinal(long ordinal)
        {
            return Filters.Gte(x => x.Ordinal, ordinal);
        }

        public static FilterDefinition<EventCommit> ByAggregateAnyTypeBelowOrdinal(string type, string id, ISet<Type> eventTypes, long maxVersion)
        {
            var items = eventTypes.Select(x => x.FullName).ToList();
            FilterDefinition<EventCommit> query = Filters.And(
                Filters.Eq(x => x.AggregateType, type),
                Filters.Eq(x => x.AggregateId, id),
                Filters.Lte(x => x.VersionCommit, maxVersion),
                Filters.AnyIn(x => x.AllEventTypes, items));

            return query;
        }

        public static FilterDefinition<EventCommit> ByAggregateAnyType(string type, string id, ISet<Type> eventTypes)
        {
            var items = eventTypes.Select(x => x.AssemblyQualifiedName).ToList();
            FilterDefinition<EventCommit> query = Filters.And(
                Filters.Eq(x => x.AggregateType, type),
                Filters.Eq(x => x.AggregateId, id),
                Filters.AnyIn(x => x.AllEventTypes, items));

            return query;
        }
    }
}