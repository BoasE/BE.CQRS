using AspNetCoreSample.Domain.Events;
using BE.CQRS.Domain.States;

namespace AspNetCoreSample.Domain.States
{
    public class NameState : StateBase
    {
        public string Name { get; set; }

        public void On(CustomerCreatedFromConsoleEvent @event)
        {
            Name = @event.Name;
        }
    }
}
