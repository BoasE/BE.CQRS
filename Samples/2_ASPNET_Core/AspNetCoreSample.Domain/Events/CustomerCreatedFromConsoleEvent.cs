using BE.CQRS.Domain.Events;

namespace AspNetCoreSample.Domain.Events
{
    public sealed class CustomerCreatedFromConsoleEvent : EventBase
    {
        public string Name { get; set; }
        public CustomerCreatedFromConsoleEvent()
        {
        }
    }
}
