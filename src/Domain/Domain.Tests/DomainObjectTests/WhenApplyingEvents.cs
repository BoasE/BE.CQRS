using System.Collections.Generic;
using BE.CQRS.Domain.Events;
using Xunit;

namespace BE.CQRS.Domain.Tests.DomainObjectTests
{
    public class WhenApplyingEvents : GivenDomainObject
    {
        private readonly TestDomainObject sut;

        public WhenApplyingEvents()
        {
            sut = GetSut("11");

            var @event = new TestEvent();
            var events = new List<IEvent>
            {
                @event
            };

            sut.ApplyEvents(events);
        }

        [Fact]
        public void ItIncreasesTheVersion()
        {
            Assert.Equal(1, sut.Version);
        }

        [Fact]
        public void ItIncreasesTheOriginVersion()
        {
            Assert.Equal(1, sut.OriginVersion);
        }
    }
}