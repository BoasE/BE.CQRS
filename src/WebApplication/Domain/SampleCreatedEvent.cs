using BE.CQRS.Domain.Events;

namespace WebApplication.Domain
{
    public sealed  class SampleCreatedEvent : EventBase
    {
        public string Value { get; set; }
    }
}