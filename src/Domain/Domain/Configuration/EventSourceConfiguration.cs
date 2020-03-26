using System;
using System.Collections.Generic;
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
        public List<Assembly> DomainObjectAssemblies { get; } = new List<Assembly>();
        
        public Action<IEvent> PostSavePipeline { get; set; } //TODO Extract to a IEventHandler component

        public void AddDomainObjectAssembly(params  Assembly[] assemblies)
        {
            DomainObjectAssemblies.AddRange(assemblies);
        }
    }
}