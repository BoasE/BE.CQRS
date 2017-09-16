using System;
using System.Threading;
using System.Threading.Tasks;
using BE.CQRS.Domain;
using BE.CQRS.Domain.DomainObjects;
using BE.CQRS.Domain.Events;
using EventStore.ClientAPI;

namespace BE.CQRS.Data.GetEventStore
{
    public sealed class EventStoreRepository : DomainObjectRepositoryBase
    {
        private readonly EventStoreContext context;

        public EventStoreRepository(EventStoreContext context) : base(context.Activator)
        {
            this.context = context;
        }

        protected override IObservable<IEvent> ReadEvents(string streamName, CancellationToken token)
        {
            return context.Reader.ReadEvents(streamName, token);
        }

        protected override string ResolveStreamName(string id, Type aggregateType)
        {
            return context.StreamNamer.Resolve(aggregateType, id);
        }

        protected override Task<long> GetVersion(string streamName)
        {
            return context.Reader.ReadVersion(streamName);
        }

        protected override Task<bool> ExistsStream(string streamName)
        {
            return context.Reader.ExistsStreamAsync(streamName);
        }

        protected override async Task<AppendResult> SaveUncomittedEventsAsync<T>(T domainObject, bool versionCheck)
        {
            string streamName = context.StreamNamer.Resolve(domainObject);

            long expectedVersion = versionCheck ? ExpectedVersionFromDo(domainObject) : ExpectedVersion.Any;

            AppendResult result = await context.Writer.AppendAsync(streamName, domainObject.GetUncommittedEvents(),
                expectedVersion);

            return result;
        }

        private long ExpectedVersionFromDo(IDomainObject domainObject)
        {
            long result = domainObject.OriginVersion == 0 ? ExpectedVersion.NoStream : domainObject.OriginVersion - 1;

            return result;
        }
    }
}