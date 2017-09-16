using System.Collections.Generic;
using BE.CQRS.Domain.Events;
using Xunit;

namespace BE.CQRS.Domain.Tests.DomainObjectTests
{
    public class WhenRaisingEvents : GivenDomainObject
    {
        private readonly TestDomainObject sut;

        public WhenRaisingEvents()
        {
            sut = Execute();
        }

        [Fact]
        public void ItIncreasesCurrentVersion()
        {
            Assert.Equal(1, sut.Version);
        }

        [Fact]
        public void ItKeepsOriginVersionUntouched()
        {
            Assert.Equal(0, sut.OriginVersion);
        }

        [Fact]
        public void ItHasPendingEvents()
        {
            IReadOnlyCollection<IEvent> events = sut.GetUncommittedEvents();

            Assert.Equal(1, events.Count);
        }

        private TestDomainObject Execute()
        {
            TestDomainObject domainObject = GetSut("22");

            domainObject.RaiseEvent();
            return domainObject;
        }
    }
}