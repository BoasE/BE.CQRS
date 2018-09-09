using BE.CQRS.Domain.Events;

namespace AspNetCoreSample.Domain.Events
{
    public sealed class CustomerCreatedFromApiEvent : EventBase
    {
        public string Name { get; set; }
        public CustomerCreatedFromApiEvent()
        {
        }
    }
}
