using BE.CQRS.Domain.States;
using Domain.Events;

namespace Domain.States
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
