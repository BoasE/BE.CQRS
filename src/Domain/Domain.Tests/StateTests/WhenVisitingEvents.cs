using System;
using System.Collections.Generic;
using BE.CQRS.Domain.Events;
using BE.CQRS.Domain.Tests.EventBaseTests;
using Xunit;

namespace BE.CQRS.Domain.Tests.StateTests
{
    public class WhenFreezing : GivenState
    {
        private readonly List<IEvent> source = new List<IEvent>
        {
            new SampleEvent(),
            new SampleEvent()
        };

        private readonly SampleState State;

        public WhenFreezing()
        {
            State = ResolveState(source);
            State.Freeze();
        }

        [Fact]
        public void ItIsFrozen()
        {
            Assert.True(State.IsFrozen);
        }

        [Fact]
        public void ItDoesNotVisitEvents()
        {
            Assert.Throws<InvalidOperationException>(() => State.Execute(source));
        }
    }
}