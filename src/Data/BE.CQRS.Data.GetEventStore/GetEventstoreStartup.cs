using BE.CQRS.Domain.Configuration;
using EventStore.ClientAPI;

namespace BE.CQRS.Data.GetEventStore
{
    public static class GetEventstoreStartup
    {
        public static EventSourceConfiguration SetEventstoreEventsource(this EventSourceConfiguration config, IEventStoreConnection connection)
        {
            EventStoreContext context = EventStoreContext.CreateDefault(config.Prefix, connection, config.Activator);

            var repo = new EventStoreRepository(context);
            config.DomainObjectRepository = repo;

            return config;
        }
    }
}