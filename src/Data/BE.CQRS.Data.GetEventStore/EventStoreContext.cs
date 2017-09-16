using BE.CQRS.Data.GetEventStore.Transformation;
using BE.CQRS.Domain.DomainObjects;
using BE.CQRS.Domain.Events;
using BE.CQRS.Domain.Serialization;
using BE.FluentGuard;
using EventStore.ClientAPI;

namespace BE.CQRS.Data.GetEventStore
{
    public sealed class EventStoreContext
    {
        public IEventStoreConnection Connection { get; }

        public IEventReader Reader { get; }

        public IEventWriter Writer { get; }

        public IDomainObjectActivator Activator { get; }

        public IStreamNamer StreamNamer { get; }

        private EventStoreContext(IEventStoreConnection connection, IStreamNamer streamNamer, IEventReader reader,
            IEventWriter writer, IDomainObjectActivator activator)
        {
            Precondition.For(connection, nameof(connection)).NotNull();
            Precondition.For(streamNamer, nameof(streamNamer)).NotNull();
            Precondition.For(reader, nameof(reader)).NotNull();
            Precondition.For(writer, nameof(writer)).NotNull();
            Precondition.For(activator, nameof(activator)).NotNull();

            Connection = connection;
            Reader = reader;
            Writer = writer;
            Activator = activator;
            StreamNamer = streamNamer;
        }

        public static EventStoreContext CreateDefault(string prefix, IEventStoreConnection connection,
            IDomainObjectActivator activator = null)
        {
            var namer = new StreamTypeNamer(prefix);
            var eventSerializer = new JsonEventSerializer(new EventTypeResolver());
            var transformer = new EventTransformator(eventSerializer);

            if (activator == null)
            {
                activator = new ActivatorDomainObjectActivator();
            }

            return new EventStoreContext(connection, namer, new EventStoreReader(connection, transformer),
                new EventStoreWriter(connection, transformer), activator);
        }
    }
}