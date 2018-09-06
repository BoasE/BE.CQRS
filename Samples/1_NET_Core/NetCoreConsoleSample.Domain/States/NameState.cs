using BE.CQRS.Domain.States;
using NetCoreConsoleSample.Domain.Events;

namespace NetCoreConsoleSample.Domain.States
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
