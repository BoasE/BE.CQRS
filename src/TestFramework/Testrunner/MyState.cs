using BE.CQRS.Domain.States;

namespace Testrunner
{
    public sealed class MyState : StateBase
    {
        public void On(MyEvent @event)
        {
        }
    }
}