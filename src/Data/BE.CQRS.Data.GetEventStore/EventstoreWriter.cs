using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BE.CQRS.Data.GetEventStore.Transformation;
using BE.CQRS.Domain.Events;
using EventStore.ClientAPI;
using EventStore.ClientAPI.Exceptions;

namespace BE.CQRS.Data.GetEventStore
{
    public sealed class EventStoreWriter : IEventWriter
    {
        private readonly IEventStoreConnection connection;
        private readonly IEventTransformer transformer;

        public EventStoreWriter(IEventStoreConnection connection, IEventTransformer transformer)
        {
            this.connection = connection;
            this.transformer = transformer;
        }

        public async Task<AppendResult> AppendAsync(string streamName, IEnumerable<IEvent> events, long expectedVersion)
        {
            EventData[] eventsToInsert = events
                .Select(@event => transformer.ToEventData(@event))
                .ToArray();

            try
            {
                WriteResult result = await connection.AppendToStreamAsync(streamName, expectedVersion, eventsToInsert);
                return new AppendResult(false, result.NextExpectedVersion);
            }
            catch (WrongExpectedVersionException)
            {
                return new AppendResult(true, 0);
            }
        }
    }
}