using System;
using System.Reflection;
using BE.CQRS.Domain;
using BE.CQRS.Domain.Commands;
using BE.CQRS.Domain.Configuration;
using BE.CQRS.Domain.DomainObjects;
using EventStore.ClientAPI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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