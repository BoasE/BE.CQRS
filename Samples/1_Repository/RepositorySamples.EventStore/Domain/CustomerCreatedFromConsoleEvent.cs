using System;
using System.Collections.Generic;
using System.Text;
using BE.CQRS.Domain.Events;

namespace RepositorySamples.EventStore.Domain
{
    public sealed class CustomerCreatedFromConsoleEvent : EventBase
    {
        public string Name { get; set; }
        public CustomerCreatedFromConsoleEvent()
        {
        }
    }
}
