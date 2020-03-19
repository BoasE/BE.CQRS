using System;
using System.Reflection;
using BE.CQRS.Domain.Commands;
using BE.CQRS.Domain.DomainObjects;
using BE.CQRS.Domain.Events;
using BE.CQRS.Domain.Events.Handlers;
using BE.CQRS.Domain.Logging;
using BE.CQRS.Domain.Serialization;
using BE.CQRS.Domain.States;
using Microsoft.Extensions.Logging;

namespace BE.CQRS.Domain.Configuration
{
    public sealed class EventSourceConfiguration
    {
        public string Prefix { get; set; }

        public IEventMapper EventMapper { get; set; }

        public Assembly[] DomainObjectAssemblies { get; set; }

        

        public IStateEventMapping StateToEventMapper { get; set; } = new StateEventMapping();

        public Action<IEvent> PostSavePipeline { get; set; }

        public IEventHandler DirectDenormalizers { get; set; }

        public IEventHash EventHash { get; set; } = null;

        public IEventSerializer EventSerializer { get; set; } = new JsonEventSerializer(new EventTypeResolver());
    }
}