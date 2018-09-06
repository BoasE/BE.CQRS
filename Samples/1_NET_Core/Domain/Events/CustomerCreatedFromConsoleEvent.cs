using BE.CQRS.Domain.Events;

namespace Domain.Events
{
    public sealed class CustomerCreatedFromConsoleEvent : EventBase
    {
        public string Name { get; set; }
        public CustomerCreatedFromConsoleEvent()
        {
        }
    }
}
