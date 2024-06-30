using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using BE.CQRS.Data.MongoDb.Repositories;
using BE.CQRS.Domain;
using BE.CQRS.Domain.Conventions;
using BE.CQRS.Domain.DomainObjects;
using BE.CQRS.Domain.Events;
using BE.CQRS.Domain.Serialization;
using BE.FluentGuard;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;

namespace BE.CQRS.Data.MongoDb.Commits
{
    public sealed class MongoCommitRepository : MongoRepositoryBase<EventCommit>
    {
        private readonly MongoGlobalIdentifier identifier;
        private readonly EventMapper Mapper;

        private readonly bool UseTransactions;
        private readonly bool DeactivateTimeoutOnRead;
        private readonly ILogger<MongoCommitRepository> logger;

        public MongoCommitRepository(IMongoDatabase db, IEventHash hash, IEventSerializer eventSerializer,
            ILogger<MongoCommitRepository> logger,
            bool useTransactions, bool deactivateTimeoutOnRead) : base(db,
            "es_Commits")
        {
            this.logger = logger;
            UseTransactions = useTransactions;
            DeactivateTimeoutOnRead = deactivateTimeoutOnRead;
            Mapper = new EventMapper(eventSerializer, hash);
            identifier = new MongoGlobalIdentifier(db);
            PrepareCollection(Collection).Wait();
        }

        private async Task PrepareCollection(IMongoCollection<EventCommit> collection)
        {
            List<CreateIndexModel<EventCommit>> indexModels = IndexDefinitions.ProvideIndexModels().ToList();

            try
            {
                await collection.Indexes.CreateManyAsync(indexModels);
            }
            catch (MongoCommandException e) when (e.CodeName.Equals("IndexOptionsConflict",
                StringComparison.OrdinalIgnoreCase))
            {
                logger.LogWarning("Dropping existing index due to conflicts...");
                await collection.Indexes.DropAllAsync();

                logger.LogWarning("Recreating indexes");
                await collection.Indexes.CreateManyAsync(indexModels);
            }
        }

        public Task<bool> Exists(string type, string id)
        {
            FilterDefinition<EventCommit> query = CommitFilters.ByAggregate(type, id);
            return Collection.Find(query).AnyAsync();
        }

        public IAsyncEnumerable<EventCommit> EnumerateAllCommits(
            EnumerateDirection direction = EnumerateDirection.Ascending, int? limit = null,
            CancellationToken token = default)
        {
            return Enumerate(CommitFilters.All, direction, limit, token);
        }

        public IAsyncEnumerable<EventCommit> EnumerateCommits(string type, string id,
            EnumerateDirection direction = EnumerateDirection.Ascending,
            int? limit = null,
            CancellationToken token = default)
        {
            FilterDefinition<EventCommit> query = CommitFilters.ByAggregate(type, id);
            return Enumerate(query, direction, limit, token);
        }

        public IAsyncEnumerable<EventCommit> EnumerateCommits(string type, string id, long maxversion,
            EnumerateDirection direction = EnumerateDirection.Ascending, int? limit = null,
            CancellationToken token = default)
        {
            FilterDefinition<EventCommit> query = Filters.And(CommitFilters.ByAggregate(type, id),
                Filters.Lte(x => x.VersionEvents, maxversion));

            return Enumerate(query, direction, limit, token);
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

            List<EventCommit> commits = await Collection
                .Find(query)
                .ToListAsync();

            if (commits == null || commits.Count == 0)
            {
                return 0;
            }

            return commits
                .SelectMany(x => x.Events)
                .Count();
        }

        public async Task<AppendResult> SaveAsync(IDomainObject domainObject, bool versionCheck)
        {
            EventCommit commit = Mapper.ToCommit(domainObject.Id, domainObject.GetType(), domainObject.OriginVersion,
                domainObject.CommitVersion + 1,
                domainObject.GetUncommittedEvents().ToList());

            if (commit.Events.Count == 0)
            {
                logger.LogTrace("Nothing to Update");
                return AppendResult.NoUpdate;
            }

            logger.LogTrace("Saving domainObject \"{Id}\" , VersionCheck: {VersionCheck}", domainObject.Id,versionCheck);
            AppendResult result;


            result = await InsertEvent(commit, versionCheck);


            return result;
        }

        private async Task<AppendResult> InsertEvent(EventCommit commit, bool versionCheck)
        {
            IClientSessionHandle session = null;
            if (UseTransactions)
            {
                session = await Database.Client.StartSessionAsync(new ClientSessionOptions());
                session.StartTransaction(new TransactionOptions());
            }

            var watch = Stopwatch.StartNew();
            commit.Ordinal = await identifier.Next("commit");

            AppendResult result = AppendResult.NoUpdate;
            try
            {
                var currentVersion = await GetVersion(commit.AggregateType, commit.AggregateId);

                if (versionCheck && currentVersion != commit.ExpectedPreviousVersion)
                {
                    logger.LogWarning("Event Version check requested and version was wrong . was: {0} expected: {1}",
                        currentVersion, commit.ExpectedPreviousVersion);
                    //TODO If version check throw exception!
                    result = AppendResult.WrongVersion(commit.VersionCommit);
                }

                commit.VersionCommit = currentVersion + commit.Events.Count;

                await Collection.InsertOneAsync(commit);
                result = new AppendResult(commit.Id.ToString(), false, commit.VersionCommit, "SUCCESS");
            }
            catch (MongoWriteException e)
            {
                if (e.Message.Contains("E11000 duplicate key"))
                {
                    result = AppendResult.WrongVersion(commit.VersionCommit);
                }
                else
                {
                    logger.LogError(e, "Error when saving a commit for {type} {id}", commit.AggregateType,
                        commit.AggregateId);
                    throw;
                }
            }
            finally
            {
                await CommitOrAbortTx(session, result);
                watch.Stop();
                logger.LogDebug("{Count} events for {Type} - {Id} handled. Result: {Result}.", commit.Events.Count,
                    commit.AggregateType, commit.AggregateId, result.CommitId);
            }

            return result;
        }

        private static async Task CommitOrAbortTx(IClientSessionHandle session, AppendResult result)
        {
            if (session != null)
            {
                if (result.HadWrongVersion || string.IsNullOrWhiteSpace(result.CommitId) || result.CurrentVersion <= 0)
                {
                    await session.AbortTransactionAsync();
                }
                else
                {
                    await session.CommitTransactionAsync();
                }

                session.Dispose();
            }
        }


        private IAsyncEnumerable<EventCommit> Enumerate(FilterDefinition<EventCommit> query)
        {
            return Enumerate(query);
        }


        private async IAsyncEnumerable<EventCommit> Enumerate(FilterDefinition<EventCommit> query,
            EnumerateDirection direction, int? limit,
            [EnumeratorCancellation] CancellationToken token)
        {
            FindOptions options;

            if (DeactivateTimeoutOnRead)
            {
                options = new FindOptions()
                {
                    NoCursorTimeout = true,
                    MaxTime = TimeSpan.MaxValue,
                    MaxAwaitTime = TimeSpan.MaxValue,
                };
            }
            else
            {
                options = new FindOptions();
            }

            SortDefinition<EventCommit> sort;
            if (direction == EnumerateDirection.Ascending)
            {
                sort = Sorts.Ascending(x => x.Ordinal);
            }
            else
            {
                sort = Sorts.Descending(x => x.Ordinal);
            }

            var find = Collection
                .Find(query, options)
                .Sort(sort);


            if (limit.HasValue && limit > 0)
            {
                find = find.Limit(limit);
            }

            IAsyncCursor<EventCommit> cursor = await find
                .ToCursorAsync(token);

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

        public async Task<EventCommit> ByInternalId(string commitId)
        {
            Precondition.For(commitId, nameof(commitId)).NotNullOrWhiteSpace("CommitId must not be null!");
            BsonObjectId id = BsonObjectId.Create(commitId);

            
            var query = Filters.Eq(x => x.Id, id);

            var commit = await Collection.Find(query).SortBy(x => x.Ordinal).FirstAsync();

            return commit;
        }
    }
}