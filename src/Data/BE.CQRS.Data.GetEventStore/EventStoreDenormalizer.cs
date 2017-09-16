using System;
using System.Reflection;
using BE.CQRS.Data.GetEventStore.Transformation;
using BE.CQRS.Domain.Denormalization;
using BE.CQRS.Domain.Events;
using BE.CQRS.Domain.Events.Handlers;
using BE.CQRS.Domain.Serialization;
using EventStore.ClientAPI;
using EventStore.ClientAPI.SystemData;

namespace BE.CQRS.Data.GetEventStore
{
    public static class EventStoreDenormalizer
    {
        public static EventDenormalizer AttachDenormalizer(this IEventStoreConnection connection,
            IStreamPositionGateway gtw, UserCredentials creds, string streamName, Func<Type, object> normalizerFactory,
            params Assembly[] normalizerAssemblies)
        {
            var subscriber = new EventSubscriber(connection, creds, streamName,
                new EventTransformator(new JsonEventSerializer(new EventTypeResolver())));
            var eventHandler = new ConventionEventHandler(normalizerFactory, normalizerAssemblies);
            var result = new EventDenormalizer(subscriber, eventHandler, gtw);

            return result;
        }
    }
}