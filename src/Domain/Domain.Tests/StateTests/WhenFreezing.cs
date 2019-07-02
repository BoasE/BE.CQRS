using System.Collections.Generic;
using BE.CQRS.Domain.Events;
using BE.CQRS.Domain.Tests.EventBaseTests;
using Xunit;

namespace BE.CQRS.Domain.Tests.StateTests
{
    public class WhenVisitingEvents : GivenState
    {
        private readonly List<IEvent> source = new List<IEvent>
        {
            new SampleEvent(),
            new SampleEvent()
        };

        private readonly SampleState State;

        public WhenVisitingEvents()
        {
            State = ResolveState(source);
        }

        [Fact]
        public void ItIteratesThroughAllEvents()
        {
            Assert.Equal(source.Count, State.Total);
        }

        [Fact]
        public void ItCountsTheSampleEvents()
        {
            Assert.Equal(2, State.SampleCount);
        }

        [Fact]
        public void FrozenIsTrue()
        {
            Assert.Equal(true, State.IsFrozen);
        }
    }
}