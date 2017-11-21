using System;
using System.Collections.Generic;
using BE.CQRS.Domain.Events;
using BE.CQRS.Domain.Tests.EventBaseTests;
using Xunit;

namespace BE.CQRS.Domain.Tests.StateTests
{
    public class WhenVisitingTwice : GivenState
    {
        private readonly List<IEvent> source = new List<IEvent>
        {
            new SampleEvent(),
            new SampleEvent()
        };

        private readonly SampleState State;

        public WhenVisitingTwice()
        {
            State = GetSut();
            State.Execute(source);
        }

        [Fact]
        public void ItThrows()
        {
            Assert.Throws<InvalidOperationException>(() => State.Execute(source));
        }
    }
}