using BE.CQRS.Domain.Events;
using BE.CQRS.Domain.States;
using BE.CQRS.Domain.Tests.EventBaseTests;

namespace BE.CQRS.Domain.Tests.StateTests
{
    public sealed class SampleState : StateBase
    {
        public int Total { get; set; }

        public int SampleCount { get; set; }

        public int Breaks { get; set; }

        protected override void On(IEvent @event)
        {
            base.On(@event);
            Total++;
        }

        public void On(SampleEvent @event)
        {
            SampleCount++;
        }

        public void On(BreakEvent @event)
        {
            Breaks++;
            Break();
        }
    }
}