using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using BE.CQRS.Data.MongoDb.Commits;
using BE.CQRS.Data.MongoDb.Repositories;
using BE.CQRS.Data.MongoDb.Streams;
using BE.CQRS.Domain;
using BE.CQRS.Domain.Configuration;
using BE.CQRS.Domain.Events;
using BE.CQRS.Domain.Serialization;
using MongoDB.Driver;

namespace BE.CQRS.Data.MongoDb
{
    public sealed class MongoDomainObjectRepository : DomainObjectRepositoryBase
    {
        private readonly MongoCommitRepository repository;
        private readonly StreamNamer namer = new StreamNamer();
        private readonly EventMapper mapper = new EventMapper(new JsonEventSerializer(new EventTypeResolver()));

        public MongoDomainObjectRepository(EventSourceConfiguration configuration, IMongoDatabase db) : base(configuration)
        {
            repository = new MongoCommitRepository(db);
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

        protected override IObservable<IEvent> ReadEvents(string streamName, ISet<Type> eventTypes, CancellationToken token)
        {
            string id = namer.IdByStreamName(streamName);
            string type = namer.TypeNameByStreamName(streamName);

            return Observable.Create<IEvent>(async observer =>
            {
                await repository.EnumerateCommits(type, id, eventTypes,
                    x =>
                    {
                        IEnumerable<IEvent> events = mapper.ExtractEvents(x);

                        foreach (IEvent @event in events)
                            observer.OnNext(@event);
                    }, observer.OnCompleted);
                return () =>
                {
                };
            });
        }

        public override Task EnumerateAll(Func<IEvent, Task> callback)
        {
            return repository.EnumerateAllCommits(async x =>
            {
                IEnumerable<IEvent> events = mapper.ExtractEvents(x);

                foreach (IEvent @event in events)
                    await callback(@event);
            });
        }

        protected override IObservable<IEvent> ReadEvents(string streamName, CancellationToken token)
        {
            string id = namer.IdByStreamName(streamName);
            string type = namer.TypeNameByStreamName(streamName);

            return Observable.Create<IEvent>(async observer =>
            {
                await repository.EnumerateCommits(type, id,
                    x =>
                    {
                        IEnumerable<IEvent> events = mapper.ExtractEvents(x);

                        foreach (IEvent @event in events)
                            observer.OnNext(@event);
                    }, observer.OnCompleted);
                return () =>
                {
                };
            });
        }
    }
}