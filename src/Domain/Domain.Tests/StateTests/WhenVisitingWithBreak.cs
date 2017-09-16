using System.Collections.Generic;
using BE.CQRS.Domain.Events;
using BE.CQRS.Domain.Tests.EventBaseTests;
using Xunit;

namespace BE.CQRS.Domain.Tests.StateTests
{
    public sealed class WhenVisitingWithBreak : GivenState
    {
        private readonly List<IEvent> source = new List<IEvent>
        {
            new SampleEvent(),
            new SampleEvent(),
            new SampleEvent(),
            new BreakEvent(),
            new SampleEvent(),
            new SampleEvent(),
            new BreakEvent()
        };

        private readonly SampleState State;

        public WhenVisitingWithBreak()
        {
            State = ResolveState(source);
        }

        [Fact]
        public void ItStopsOnBreak()
        {
            Assert.Equal(4, State.Total);
        }

        [Fact]
        public void ItOnlyEventBeforeBreak()
        {
            Assert.Equal(3, State.SampleCount);
        }

        [Fact]
        public void ItHasOnlyOneBreak()
        {
            Assert.Equal(1, State.Breaks);
        }
    }
}