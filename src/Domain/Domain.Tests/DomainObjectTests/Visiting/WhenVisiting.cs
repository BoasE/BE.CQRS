using System.Collections.Generic;
using BE.CQRS.Domain.Events;
using Xunit;

namespace BE.CQRS.Domain.Tests.DomainObjectTests.Visiting
{
    public class WhenVisiting : GivenDomainObject
    {
        public SampleVisitor VisitorState { get; set; }

        public SampleVisitor VisitorStateWithSuppres { get; set; }

        public WhenVisiting()
        {
            TestDomainObject sut = GetSut("11");

            var @event = new TestEvent();
            var events = new List<IEvent>
            {
                @event
            };

            sut.ApplyEvents(events);

            sut.RaiseEvent();
            VisitorState = sut.State<SampleVisitor>();
            VisitorStateWithSuppres = sut.State<SampleVisitor>(true);
        }

        [Fact]
        public void ItProcessedAppliedAndUncommittedEvents()
        {
            Assert.Equal(2, VisitorState.TestEventCount);
        }

        [Fact]
        public void ItCanSuppressUncommittedEvents()
        {
            Assert.Equal(1, VisitorStateWithSuppres.TestEventCount);
        }
    }
}