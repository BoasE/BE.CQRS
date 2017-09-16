using BE.CQRS.Domain.Events;

namespace Testrunner
{
    public sealed class MyEvent : EventBase
    {
        public string Id { get; set; }
    }
}