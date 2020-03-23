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
        public string EventSecret { get; set; } = null;
        
        public string Prefix { get; set; }
        public Assembly[] DomainObjectAssemblies { get; set; }
        
        public Action<IEvent> PostSavePipeline { get; set; }

        /// <summary>
        /// Eventhandler that  immediately calls denormalizations without any message-bus in between
        /// </summary>
        public IEventHandler ImmediateDenormalizationHandler { get; set; }
    }
}