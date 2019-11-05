﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using BE.CQRS.Data.MongoDb.Repositories;
using BE.CQRS.Domain.DomainObjects;
using BE.CQRS.Domain.Events;
using BE.CQRS.Domain.Serialization;
using MongoDB.Driver;

namespace BE.CQRS.Data.MongoDb.Commits
{
    public sealed class MongoCommitRepository : MongoRepositoryBase<EventCommit>
    {
        private readonly MongoGlobalIdentifier identifier;
        private readonly EventMapper Mapper;

        public MongoCommitRepository(IMongoDatabase db) : base(db, "es_Commits")
        {
            Mapper = new EventMapper(new JsonEventSerializer(new EventTypeResolver()));
            identifier = new MongoGlobalIdentifier(db);
            PrepareCollection(Collection).Wait();
        }

        public Task<bool> Exists(string type, string id)
        {
            FilterDefinition<EventCommit> query = CommitFilters.ByAggregate(type, id);
            return Collection.Find(query).AnyAsync();
        }

        public async Task EnumerateAllCommits(Func<EventCommit, Task> consumer)
        {
            FilterDefinition<EventCommit> query = CommitFilters.All;

            await Collection.Find(query, new FindOptions() { NoCursorTimeout = true, MaxTime = TimeSpan.MaxValue, MaxAwaitTime = TimeSpan.MaxValue }).ForEachAsync(async x => await consumer(x));
        }

        public async Task EnumerateCommits(string type, string id, Action<EventCommit> consumer, Action completed)
        {
            FilterDefinition<EventCommit> query = CommitFilters.ByAggregate(type, id);
            await Enumerate(consumer, query);

            completed();
        }

        public async Task EnumerateCommits(string type, string id, ISet<Type> eventTypes, Action<EventCommit> consumer, Action completed)
        {
            FilterDefinition<EventCommit> query = CommitFilters.ByAggregateAnyType(type, id, eventTypes);

            await Enumerate(consumer, query);

            completed();
        }

        public async Task EnumerateCommits(string type, string id, ISet<Type> eventTypes, int maxVersion, Action<EventCommit> consumer, Action completed)
        {
            FilterDefinition<EventCommit> query = CommitFilters.ByAggregateAnyTypeBelowOrdinal(type, id, eventTypes, maxVersion);

            await Enumerate(consumer, query);

            completed();
        }

        public Task EnumerateStartingAfter(long ordinal, Action<EventCommit> consumer)
        {
            FilterDefinition<EventCommit> query = Filters.Gt(x => x.Ordinal, ordinal);
            return Enumerate(consumer, query);
        }

        public async Task<long> GetVersion(string type, string id)
        {
            FilterDefinition<EventCommit> query = CommitFilters.ByAggregate(type, id);

            EventCommit result = await Collection.Find(query).Sort(Sorts.Descending(x => x.VersionEvents))
                .FirstOrDefaultAsync();

            return result.VersionEvents;
        }

        private async Task PrepareCollection(IMongoCollection<EventCommit> collection)
        {
            List<CreateIndexModel<EventCommit>> indexModels = IndexDefinitions.ProvideIndexModels().ToList();
            await indexModels.ForEachAsync(async model =>
            {
                await collection.Indexes.CreateOneAsync(model);
            });
        }

        public async Task<AppendResult> SaveAsync(IDomainObject domainObject, bool versionCheck)
        {
            EventCommit commit = Mapper.ToCommit(domainObject.Id, domainObject.GetType(), domainObject.OriginVersion,
                domainObject.CommitVersion + 1,
                domainObject.GetUncommittedEvents().ToList());

            if (commit.Events.Count == 0)
            {
                Debug.WriteLine("Nothing to Update", "BE.CQRS");
                return new AppendResult(false, commit.VersionEvents);
            }

            Debug.WriteLine($"Saving domainObject \"{domainObject.Id}\" - VersionCheck {versionCheck}", "BE.CQRS");
            AppendResult result;

            result = await InsertEvent(commit, versionCheck);

            return result;
        }

        private async Task<AppendResult> InsertEvent(EventCommit commit, bool versionCheck)
        {
            commit.Ordinal = await identifier.Next("commit");

            try
            {
                await Collection.InsertOneAsync(commit);
            }
            catch (MongoWriteException e)
            {
                if (e.Message.Contains("E11000 duplicate key "))
                    return AppendResult.WrongVersion(commit.VersionCommit);

                throw;
            }

            return new AppendResult(false, commit.VersionCommit);
        }

        [Obsolete]
        private async Task<UpdateResult> InsertIfNoNewer(EventCommit commit)
        {
            long ordinal = await identifier.Next("commit");
            UpdateDefinition<EventCommit> update = Updates
                .SetOnInsert(x => x.Events, commit.Events)
                .SetOnInsert(x => x.Ordinal, ordinal)
                .SetOnInsert(x => x.VersionEvents, commit.VersionEvents)
                .SetOnInsert(x => x.VersionCommit, commit.VersionCommit)
                .SetOnInsert(x => x.AggregateId, commit.AggregateId)
                .SetOnInsert(x => x.AggregateType, commit.AggregateType)
                .SetOnInsert(x => x.AggregateTypeShort, commit.AggregateTypeShort)
                .SetOnInsert(x => x.AggregatePackage, commit.AggregatePackage)
                .SetOnInsert(x => x.Timestamp, commit.Timestamp)
                .CurrentDate(x => x.ServerTimestamp);

            FilterDefinition<EventCommit> versionQuery =
                Filters.And(
                    Filters.Eq(x => x.AggregateId, commit.AggregateId),
                    Filters.Gt(x => x.VersionEvents, commit.VersionEvents));

            Debug.WriteLine($"EventsVersion Gte {commit.VersionEvents}", "BE.CQRS");
            UpdateResult result;

            result = await Collection.UpdateOneAsync(versionQuery, update, new UpdateOptions
            {
                IsUpsert = true
            });

            return result;
        }

        private Task Enumerate(Action<EventCommit> consumer, FilterDefinition<EventCommit> query)
        {
            return Collection.Find(query).SortBy(x => x.Ordinal).ForEachAsync(consumer);
        }

        public Task<long> Count()
        {
            return Collection.CountDocumentsAsync(Filters.Empty);
        }

        public Task Delete(string type, string id)
        {
            FilterDefinition<EventCommit> query = CommitFilters.ByAggregate(type, id);

            return Collection.DeleteManyAsync(query);
        }
    }

    public static class ListExtensions
    {
        public static async Task ForEachAsync<T>(this List<T> enumerable, Action<T> action)
        {
            foreach (T item in enumerable)
                await Task.Run(() =>
                {
                    action(item);
                }).ConfigureAwait(false);
        }
    }

    public static class IndexDefinitions
    {
        public static IEnumerable<CreateIndexModel<EventCommit>> ProvideIndexModels()
        {
            return new List<CreateIndexModel<EventCommit>>
            {
                new CreateIndexModel<EventCommit>(
                    Builders<EventCommit>.IndexKeys.Descending(x => x.Ordinal), new CreateIndexOptions { Unique = true }),

                new CreateIndexModel<EventCommit>(
                    Builders<EventCommit>.IndexKeys.Descending(x => x.AggregateId)),

                new CreateIndexModel<EventCommit>(Builders<EventCommit>.IndexKeys.Descending(x => x.AggregateType)),

                new CreateIndexModel<EventCommit>(Builders<EventCommit>.IndexKeys
                    .Descending(x => x.AggregateId).Descending(x => x.AggregateType)),

                new CreateIndexModel<EventCommit>(
                    Builders<EventCommit>.IndexKeys.Descending(x => x.AggregateId).Descending(x => x.AggregateType)
                        .Descending(x => x.VersionEvents), new CreateIndexOptions { Unique = true }),

                new CreateIndexModel<EventCommit>(
                    Builders<EventCommit>.IndexKeys.Descending(x => x.AggregateId).Descending(x => x.AggregateType)
                        .Descending(x => x.VersionCommit), new CreateIndexOptions { Unique = true }),

                new CreateIndexModel<EventCommit>(
                    Builders<EventCommit>.IndexKeys.Descending(x => x.AggregateId).Descending(x => x.AggregateType)
                        .Ascending(x => x.AllEventTypes)),

                new CreateIndexModel<EventCommit>(
                    Builders<EventCommit>.IndexKeys.Descending(x => x.AggregateId).Descending(x => x.AggregateType)
                        .Ascending(x => x.AllEventTypes).Descending(x => x.Ordinal))
            };
        }
    }
}