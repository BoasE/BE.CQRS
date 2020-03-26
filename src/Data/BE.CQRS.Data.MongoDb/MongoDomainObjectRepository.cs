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
using BE.CQRS.Domain.DomainObjects;
using BE.CQRS.Domain.Events;
using BE.CQRS.Domain.Serialization;
using BE.CQRS.Domain.States;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;

namespace BE.CQRS.Data.MongoDb
{
    public sealed class MongoDomainObjectRepository : DomainObjectRepositoryBase
    {
        private readonly MongoCommitRepository repository;
        private readonly StreamNamer namer = new StreamNamer();
        private readonly EventMapper mapper;
        private readonly IEventSerializer eventSerializer;
        private readonly IEventHash eventHash;

        public MongoDomainObjectRepository(EventSourceConfiguration configuration, MongoEventsourceDataContext dataContext,
            EventsourceDIContext diContext,
            IEventSerializer eventSerializer,IEventHash eventHash,IImmediateConventionDenormalizer denormalizer,
            IStateEventMapping stateEventMapping,ILoggerFactory logger) 
            : base(configuration,denormalizer,diContext,stateEventMapping,logger)
        {
            this.eventSerializer = eventSerializer;
            this.eventHash = eventHash;
            mapper = new EventMapper(eventSerializer,eventHash);
            repository = new MongoCommitRepository(dataContext.Database,eventHash,eventSerializer);
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

        protected override async IAsyncEnumerable<IEvent> ReadEvents(string streamName, long maxVersion, [EnumeratorCancellation] CancellationToken token)
        {
            string id = namer.IdByStreamName(streamName);
            string type = namer.TypeNameByStreamName(streamName);

            await foreach (EventCommit x in repository.EnumerateCommits(type, id, maxVersion, token))
            {
                IEnumerable<IEvent> events = mapper.ExtractEvents(x);

                foreach (IEvent @event in events)
                    yield return @event;
                ;
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
                ;
            }
        }

        public override async IAsyncEnumerable<IEvent> EnumerateAll([EnumeratorCancellation] CancellationToken token)
        {
            await foreach (EventCommit x in repository.EnumerateAllCommits(token))
            {
                IEnumerable<IEvent> events = mapper.ExtractEvents(x);

                foreach (IEvent @event in events)
                    yield return @event;
            }
        }

        protected override async IAsyncEnumerable<IEvent> ReadEvents(string streamName, [EnumeratorCancellation] CancellationToken token)
        {
            string id = namer.IdByStreamName(streamName);
            string type = namer.TypeNameByStreamName(streamName);

            await foreach (EventCommit x in repository.EnumerateCommits(type, id, token))
            {
                IEnumerable<IEvent> events = mapper.ExtractEvents(x);

                foreach (IEvent @event in events)
                    yield return @event;
            }
        }
    }
}