using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using BE.CQRS.Data.MongoDb.Commits;
using BE.CQRS.Data.MongoDb.Repositories;
using BE.CQRS.Data.MongoDb.Streams;
using BE.CQRS.Domain;
using BE.CQRS.Domain.Configuration;
using BE.CQRS.Domain.Denormalization;
using BE.CQRS.Domain.Events;
using BE.CQRS.Domain.Serialization;
using BE.CQRS.Domain.States;
using Microsoft.Extensions.Logging;

namespace BE.CQRS.Data.MongoDb
{
    public sealed class MongoDomainObjectRepository : DomainObjectRepositoryBase
    {
        private readonly MongoCommitRepository repository;
        private readonly StreamNamer namer = new();
        private readonly EventMapper mapper;
        private readonly ILogger<MongoDomainObjectRepository> logger;

        public MongoDomainObjectRepository(EventSourceConfiguration configuration,
            MongoEventsourceDataContext dataContext,
            EventsourceDIContext diContext,
            IEventSerializer eventSerializer, IEventHash eventHash,
            IImmediateConventionDenormalizerPipeline denormalizerPipeline,
            IStateEventMapping stateEventMapping, ILoggerFactory logger)
            : base(configuration, denormalizerPipeline, diContext, stateEventMapping, logger)
        {
            this.logger = logger.CreateLogger<MongoDomainObjectRepository>();

            mapper = new EventMapper(eventSerializer, eventHash);
            repository = new MongoCommitRepository(dataContext.Database, eventHash, eventSerializer,
                logger.CreateLogger<MongoCommitRepository>(),
                dataContext.UseTransactions, dataContext.DeactivateTimoutOnCommitScan);

            LogStartup(denormalizerPipeline);
        }

        private void LogStartup(IImmediateConventionDenormalizerPipeline denormalizerPipeline)
        {
            string message = "MongoDomainObjectRepository started.";
            if (denormalizerPipeline != null)
            {
                message += Environment.NewLine + "Immediate denormalization pipeline attached!";
            }

            logger.LogInformation(message);
        }

        public Task<long> GetCommitCount()
        {
            return repository.Count();
        }

        protected override Task<long> GetVersion(string streamName)
        {
            string id = namer.IdByStreamName(streamName);
            string type = namer.TypeNameByStreamName(streamName);

            return repository.GetVersion(type, id);
        }

        protected override Task<bool> ExistsStream(string streamName)
        {
            string id = namer.IdByStreamName(streamName);
            string type = namer.TypeNameByStreamName(streamName);

            return repository.Exists(type, id);
        }

        protected override Task RemoveStream(Type domainObjectType, string id)
        {
            return repository.Delete(domainObjectType.FullName, id);
        }

        protected override string ResolveStreamName(string id, Type aggregateType)
        {
            return namer.ResolveStreamName(id, aggregateType);
        }

        protected override Task<AppendResult> SaveUncomittedEventsAsync<T>(T domainObject, bool versionCheck)
        {
            return repository.SaveAsync(domainObject, versionCheck);
        }

        protected override async IAsyncEnumerable<IEvent> ReadEvents(string streamName, long maxVersion,
            [EnumeratorCancellation] CancellationToken token = default)
        {
            string id = namer.IdByStreamName(streamName);
            string type = namer.TypeNameByStreamName(streamName);

            await foreach (EventCommit x in repository.EnumerateCommits(type, id, maxVersion,
                EnumerateDirection.Ascending, null, token))
            {
                IEnumerable<IEvent> events = mapper.ExtractEvents(x);

                foreach (IEvent @event in events)
                    yield return @event;
            }
        }

        protected override async Task<List<IEvent>> ByAppendResult(AppendResult result)
        {
            var commit = await repository.ByInternalId(result.CommitId);

            List<IEvent> events = mapper.ExtractEvents(commit).ToList();
            return events;
        }

        protected override async IAsyncEnumerable<IEvent> ReadEvents(string streamName, ISet<Type> eventTypes,
            [EnumeratorCancellation] CancellationToken token)
        {
            string id = namer.IdByStreamName(streamName);
            string type = namer.TypeNameByStreamName(streamName);

            await foreach (EventCommit x in repository.EnumerateCommits(type, id, eventTypes))
            {
                IEnumerable<IEvent> events = mapper.ExtractEvents(x);

                foreach (IEvent @event in events)
                    yield return @event;
            }
        }

        public override async IAsyncEnumerable<IEvent> EnumerateAll([EnumeratorCancellation] CancellationToken token)
        {
            await foreach (EventCommit x in repository.EnumerateAllCommits(EnumerateDirection.Ascending, null
                , token))
            {
                if (token.IsCancellationRequested)
                {
                    break;
                }

                IEnumerable<IEvent> events = mapper.ExtractEvents(x);

                foreach (IEvent @event in events)
                {
                    yield return @event;
                }


                if (token.IsCancellationRequested)
                {
                    break;
                }
            }
        }

        public override async IAsyncEnumerable<IEvent> Enumerate(EnumerateDirection direction, int limit,
            [EnumeratorCancellation] CancellationToken token)
        {
            int count = 0;
            await foreach (EventCommit x in repository.EnumerateAllCommits(direction, limit, token))
            {
                if (token.IsCancellationRequested || count >= limit)
                {
                    break;
                }


                IEnumerable<IEvent> events = mapper.ExtractEvents(x);

                if (direction == EnumerateDirection.Ascending)
                {
                    events = events.OrderBy(@event => @event.Headers.Created);
                }
                else
                {
                    events = events.OrderByDescending(@event => @event.Headers.Created);
                }

                foreach (IEvent @event in events)
                {
                    if (count >= limit)
                    {
                        break;
                    }

                    count++;
                    yield return @event;
                }


                if (token.IsCancellationRequested || count >= limit)
                {
                    break;
                }
            }
        }

        protected override async IAsyncEnumerable<IEvent> ReadEvents(string streamName,
            [EnumeratorCancellation] CancellationToken token)
        {
            string id = namer.IdByStreamName(streamName);
            string type = namer.TypeNameByStreamName(streamName);

            await foreach (EventCommit x in repository.EnumerateCommits(type, id, EnumerateDirection.Ascending, null,
                token))
            {
                IEnumerable<IEvent> events = mapper.ExtractEvents(x);

                foreach (IEvent @event in events)
                    yield return @event;
            }
        }
    }
}