using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using BE.CQRS.Data.MongoDb.Repositories;
using BE.CQRS.Domain.DomainObjects;
using BE.CQRS.Domain.Events;
using BE.CQRS.Domain.Serialization;
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

        public MongoCommitRepository(IMongoDatabase db, IEventHash hash, IEventSerializer eventSerializer,
            bool useTransactions, bool deactivateTimeoutOnRead) : base(db,
            "es_Commits")
        {
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
                Console.WriteLine("Dropping existing index due to conflicts...");
                await collection.Indexes.DropAllAsync();

                Console.WriteLine("Recreating indexes");
                await collection.Indexes.CreateManyAsync(indexModels);
            }
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

        public IAsyncEnumerable<EventCommit> EnumerateCommits(string type, string id, long maxversion,
            CancellationToken token)
        {
            FilterDefinition<EventCommit> query = Filters.And(CommitFilters.ByAggregate(type, id),
                Filters.Lte(x => x.VersionEvents, maxversion));

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

            if (result == null)
            {
                return 0;
            }

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
                return new AppendResult("", false, commit.VersionEvents);
            }

            Debug.WriteLine($"Saving domainObject \"{domainObject.Id}\"", "BE.CQRS");
            AppendResult result;


            result = await InsertEvent(commit);


            return result;
        }

        private async Task<AppendResult> InsertEvent(EventCommit commit)
        {
            IClientSessionHandle session = null;
            if (UseTransactions)
            {
                session = await Database.Client.StartSessionAsync(new ClientSessionOptions());
                session.StartTransaction(new TransactionOptions());
            }

            commit.Ordinal = await identifier.Next("commit");

            var currentVersion = await GetVersion(commit.AggregateType, commit.AggregateId);

            AppendResult result = AppendResult.NoUpdate;
            try
            {
                if (currentVersion != commit.ExpectedPreviousVersion)
                {
                    result = AppendResult.WrongVersion(commit.VersionCommit);
                }
                else
                {
                    await Collection.InsertOneAsync(commit);
                    result = new AppendResult(commit.Id.ToString(), false, commit.VersionCommit);
                }
            }
            catch (MongoWriteException e)
            {
                if (e.Message.Contains("E11000 duplicate key"))
                {
                    result = AppendResult.WrongVersion(commit.VersionCommit);
                }
                else
                {
                    throw;
                }
            }
            finally
            {
                await CommitOrAbortTx(session, result);
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
            return Enumerate(query, CancellationToken.None);
        }


        private async IAsyncEnumerable<EventCommit> Enumerate(FilterDefinition<EventCommit> query,
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


            IAsyncCursor<EventCommit> cursor = await Collection.Find(query, options)
                .SortBy(x => x.Ordinal)
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
            BsonObjectId id = BsonObjectId.Create(commitId);
            var query = Filters.Eq(x => x.Id, id);

            var commit = await Collection.Find(query).SortBy(x => x.Ordinal).FirstAsync();

            return commit;
        }
    }
}