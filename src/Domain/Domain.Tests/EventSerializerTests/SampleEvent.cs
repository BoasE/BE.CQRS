using BE.CQRS.Domain.Events;

namespace BE.CQRS.Domain.Tests.EventSerializerTests
{
    public sealed class SampleEvent : EventBase
    {
        public string Text { get; set; } = "Ipsum Lorem";

        public int Value { get; set; } = 2143242;

        public SampleEvent()
        {
        }

        public SampleEvent(EventHeader header) : base(header)
        {
        }
    }
}