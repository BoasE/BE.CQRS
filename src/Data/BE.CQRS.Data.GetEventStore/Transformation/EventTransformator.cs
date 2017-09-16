using System.Text;
using BE.CQRS.Domain.Events;
using BE.CQRS.Domain.Serialization;
using BE.FluentGuard;
using EventStore.ClientAPI;

namespace BE.CQRS.Data.GetEventStore.Transformation
{
    public sealed class EventTransformator : IEventTransformer
    {
        private static readonly Encoding eventEncoding = Encoding.UTF8;

        private readonly IEventSerializer serializer;

        public EventTransformator(IEventSerializer serializer)
        {
            this.serializer = serializer;
        }

        public EventData ToEventData(IEvent @event)
        {
            Precondition.For(@event, nameof(@event)).NotNull();

            byte[] headers = eventEncoding.GetBytes(serializer.SerializeHeader(@event.Headers));
            byte[] content = eventEncoding.GetBytes(serializer.SerializeEvent(@event));

            string typeName = @event.Headers.GetString(EventHeaderKeys.EventType);
            return new EventData(@event.Headers.EventId, typeName, true, content, headers);
        }

        public IEvent FromResolvedEvent(ResolvedEvent @event)
        {
            string headerVal = eventEncoding.GetString(@event.Event.Metadata);
            string contentVal = eventEncoding.GetString(@event.Event.Data);

            IEvent result = serializer.DeserializeEvent(headerVal, contentVal);

            if (result == null)
                return null;

            result.Headers.Set(EventHeaderKeys.EventNumber, @event.OriginalEventNumber);
            return result;
        }
    }
}