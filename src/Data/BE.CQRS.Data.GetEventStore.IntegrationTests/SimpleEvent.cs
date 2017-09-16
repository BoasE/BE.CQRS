using BE.CQRS.Domain.Events;

namespace BE.CQRS.Data.GetEventStore.IntegrationTests
{
    public sealed class SimpleEvent : EventBase
    {
        public string Value { get; set; }
    }
}