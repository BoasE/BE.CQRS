using BE.CQRS.Domain.States;

namespace BE.CQRS.Domain.Tests.DomainObjectStateRuntimeTests
{
    public class TestState : StateBase
    {
        public int Counter { get; set; }

        public void On(ITestEvent @event)
        {
            Counter++;
        }
    }
}