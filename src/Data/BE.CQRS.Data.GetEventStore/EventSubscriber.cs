using System;
using System.Diagnostics;
using System.Reactive.Subjects;
using BE.CQRS.Data.GetEventStore.Transformation;
using BE.CQRS.Domain.Denormalization;
using BE.CQRS.Domain.Events;
using EventStore.ClientAPI;
using EventStore.ClientAPI.SystemData;

namespace BE.CQRS.Data.GetEventStore
{
    [DebuggerDisplay("Denormalizer - {streamName}")]
    public sealed class EventSubscriber : IEventSubscriber
    {
        public string StreamName { get; }

        private readonly IEventStoreConnection connection;

        private readonly CatchUpSubscriptionSettings settings =
            new CatchUpSubscriptionSettings(int.MaxValue, 500, false, true);

        private readonly UserCredentials creds;
        private EventStoreCatchUpSubscription activeSub;
        private Subject<OccuredEvent> eventSub;
        private readonly IEventTransformer transformer;

        public EventSubscriber(IEventStoreConnection connection, UserCredentials creds, string streamName,
            IEventTransformer transformer)
        {
            this.connection = connection;
            this.creds = creds;
            StreamName = streamName;
            this.transformer = transformer;
        }

        public IObservable<OccuredEvent> Start(long? position)
        {
            eventSub?.Dispose();
            eventSub = new Subject<OccuredEvent>();
            activeSub = connection.SubscribeToStreamFrom(StreamName, position, settings, Handle);

            return eventSub;
        }

        public void Stop()
        {
            activeSub?.Stop();
        }

        public void Handle(EventStoreCatchUpSubscription sub, ResolvedEvent @event)
        {
            IEvent result = null;
            try
            {
                result = transformer.FromResolvedEvent(@event);
            }
            catch (Exception err)
            {
                Console.WriteLine(
                    $"Error parsing event {@event.OriginalStreamId}/{@event.OriginalEventNumber}; Msg: {err.Message}");
            }

            if (result != null)
            {
                long pos = @event.OriginalEventNumber;
                eventSub.OnNext(new OccuredEvent(pos, result));
            }
        }
    }
}