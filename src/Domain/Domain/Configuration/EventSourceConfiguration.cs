﻿using System.Reflection;
using BE.CQRS.Domain.Commands;
using BE.CQRS.Domain.DomainObjects;
using BE.CQRS.Domain.Logging;
using BE.CQRS.Domain.States;
using Microsoft.Extensions.Logging;

namespace BE.CQRS.Domain.Configuration
{
    public sealed class EventSourceConfiguration
    {
        public string Prefix { get; set; }

        public IEventMapper EventMapper { get; set; }

        public IDomainObjectActivator Activator { get; set; }

        public IStateActivator StateActivator { get; set; }

        public ICommandBus CommandBus { get; set; }

        public IDomainObjectRepository DomainObjectRepository { get; set; }

        public Assembly[] DomainObjectAssemblies { get; set; }

        public ILoggerFactory LoggerFactory { get; set; } = new NoopLoggerFactory();
    }
}