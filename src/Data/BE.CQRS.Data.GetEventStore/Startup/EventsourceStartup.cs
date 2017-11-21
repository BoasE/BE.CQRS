using System;
using BE.CQRS.Domain.Configuration;
using EventStore.ClientAPI;

namespace BE.CQRS.Data.GetEventStore.Startup
{
    public static class EventsourceStartup
    {
        public static EventSourceConfiguration SetEventstoreDomainObjectRepository(this EventSourceConfiguration config, IEventStoreConnection db)
        {
            // TODO
            throw new NotImplementedException();
        }
    }
}