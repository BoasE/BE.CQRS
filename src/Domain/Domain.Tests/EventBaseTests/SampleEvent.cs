using BE.CQRS.Domain.Events;

namespace BE.CQRS.Domain.Tests.EventBaseTests
{
    public sealed class SampleEvent : EventBase
    {
        public SampleEvent()
        {
        }

        public SampleEvent(EventHeader header) : base(header)
        {
        }
    }
}