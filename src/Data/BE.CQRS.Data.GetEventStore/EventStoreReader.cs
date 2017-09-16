using System;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using BE.CQRS.Data.GetEventStore.Transformation;
using BE.CQRS.Domain.Events;
using BE.FluentGuard;
using EventStore.ClientAPI;

namespace BE.CQRS.Data.GetEventStore
{
    public sealed class EventStoreReader : IEventReader
    {
        private readonly IEventStoreConnection connection;
        private readonly IEventTransformer eventTransformer;
        private readonly BatchReader batchReader;

        public EventStoreReader(IEventStoreConnection connection, IEventTransformer transformer)
        {
            this.connection = connection;
            eventTransformer = transformer;
            batchReader = new BatchReader(connection, transformer, 10);
        }

        public async Task<bool> ExistsStreamAsync(string streamName)
        {
            StreamEventsSlice result = await connection.ReadStreamEventsForwardAsync(streamName, 0, 1, false);

            return result.Status == SliceReadStatus.Success;
        }

        public IObservable<IEvent> ReadEvents(string streamName)
        {
            return ReadEvents(streamName, CancellationToken.None);
        }

        public async Task<long> ReadVersion(string streamName)
        {
            StreamEventsSlice result = await connection.ReadStreamEventsBackwardAsync(streamName, 0, 1, false);

            return result.LastEventNumber;
        }

        public IObservable<IEvent> ReadEvents(string streamName, CancellationToken cancellationToken)
        {
            Precondition.For(streamName, nameof(streamName)).NotNullOrWhiteSpace();

            cancellationToken.ThrowIfCancellationRequested();

            return Observable.Create<IEvent>(async observer =>
            {
                await batchReader.ExecuteAsync(streamName, observer, cancellationToken);
                return () =>
                {
                };
            });
        }
    }
}