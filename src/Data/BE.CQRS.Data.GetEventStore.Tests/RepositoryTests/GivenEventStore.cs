using BE.CQRS.Domain.Configuration;
using EventStore.ClientAPI;

namespace BE.CQRS.Data.GetEventStore.Tests.RepositoryTests
{
    public class GivenEventStore
    {
        private const string Prefix = "GivenEventStore.";

        protected virtual EventStoreRepository GetSut(string prefix, IEventStoreConnection connection)
        {
            EventStoreContext context = EventStoreContext.CreateDefault(prefix, connection);
            return new EventStoreRepository(context,new EventSourceConfiguration());
        }
    }
}