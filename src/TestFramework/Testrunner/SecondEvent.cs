using BE.CQRS.Domain.Events;

namespace Testrunner
{
    public sealed class SecondEvent : EventBase
    {
        public int Value { get; set; } = 42;

        public string Subject { get; set; } = "Foo";
    }
}