using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
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

        private Task PrepareCollection(IMongoCollection<EventCommit> collection)
        {
            List<CreateIndexModel<EventCommit>> indexModels = IndexDefinitions.ProvideIndexModels().ToList();

            var tasks = new List<Task>(indexModels.Count);
            foreach (CreateIndexModel<EventCommit> model in indexModels)
                tasks.Add(collection.Indexes.CreateOneAsync(model));

            return Task.WhenAll(tasks);
        }

        public Task<bool> Exists(string type, string id)
        {
            FilterDefinition<EventCommit> query = CommitFilters.ByAggregate(type, id);
            return Collection.Find(query).AnyAsync();
        }

        public IAsyncEnumerable<EventCommit> EnumerateAllCommits(CancellationToken token)
        {
            return Enumerate(CommitFilters.All, token);
        }

        public IAsyncEnumerable<EventCommit> EnumerateCommits(string type, string id, CancellationToken token)
        {
            FilterDefinition<EventCommit> query = CommitFilters.ByAggregate(type, id);
            return Enumerate(query, token);
        }

        public IAsyncEnumerable<EventCommit> EnumerateCommits(string type, string id, long maxversion, CancellationToken token)
        {
            FilterDefinition<EventCommit> query = Filters.And(CommitFilters.ByAggregate(type, id), Filters.Lte(x => x.VersionEvents, maxversion));

            return Enumerate(query, token);
        }

        public IAsyncEnumerable<EventCommit> EnumerateCommits(string type, string id, ISet<Type> eventTypes)
        {
            FilterDefinition<EventCommit> query = CommitFilters.ByAggregateAnyType(type, id, eventTypes);

            return Enumerate(query);
        }

        public IAsyncEnumerable<EventCommit> EnumerateCommits(string type, string id, ISet<Type> eventTypes,
            int maxVersion)
        {
            FilterDefinition<EventCommit> query =
                CommitFilters.ByAggregateAnyTypeBelowOrdinal(type, id, eventTypes, maxVersion);

            return Enumerate(query);
        }

        public IAsyncEnumerable<EventCommit> EnumerateStartingAfter(long ordinal)
        {
            FilterDefinition<EventCommit> query = Filters.Gt(x => x.Ordinal, ordinal);
            return Enumerate(query);
        }

        public async Task<long> GetVersion(string type, string id)
        {
            FilterDefinition<EventCommit> query = CommitFilters.ByAggregate(type, id);

            EventCommit result = await Collection.Find(query).Sort(Sorts.Descending(x => x.VersionEvents))
                .FirstOrDefaultAsync();

            return result.VersionEvents;
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

        private IAsyncEnumerable<EventCommit> Enumerate(FilterDefinition<EventCommit> query)
        {
            return Enumerate(query, CancellationToken.None);
        }

        private async IAsyncEnumerable<EventCommit> Enumerate(FilterDefinition<EventCommit> query,
            CancellationToken token)
        {
            IAsyncCursor<EventCommit> cursor = await Collection.Find(query).SortBy(x => x.Ordinal).ToCursorAsync();

            while (await cursor.MoveNextAsync(token) && !token.IsCancellationRequested)
                foreach (EventCommit item in cursor.Current)
                {
                    token.ThrowIfCancellationRequested();
                    yield return item;
                }
        }

        public Task Delete(string type, string id)
        {
            FilterDefinition<EventCommit> query = CommitFilters.ByAggregate(type, id);

            return Collection.DeleteManyAsync(query);
        }

        public Task<long> Count()
        {
            return Collection.CountDocumentsAsync(Filters.Empty);
        }
    }
}