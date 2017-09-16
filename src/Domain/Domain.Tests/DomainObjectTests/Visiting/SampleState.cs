using BE.CQRS.Domain.States;

namespace BE.CQRS.Domain.Tests.DomainObjectTests.Visiting
{
    public sealed class SampleVisitor : StateBase
    {
        public int TestEventCount;

        public void On(TestEvent @event)
        {
            TestEventCount++;
        }
    }
}