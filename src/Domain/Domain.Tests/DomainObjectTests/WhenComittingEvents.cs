using System.Collections.Generic;
using BE.CQRS.Domain.Events;
using Xunit;

namespace BE.CQRS.Domain.Tests.DomainObjectTests
{
    public class WhenComittingEvents : GivenDomainObject
    {
        private readonly TestDomainObject sut;

        public WhenComittingEvents()
        {
            sut = GetSut("222");

            sut.RaiseEvent();

            sut.CommitChanges(1);
        }

        [Fact]
        public void ItHasNoPendingEvents()
        {
            IReadOnlyCollection<IEvent> pending = sut.GetUncommittedEvents();

            Assert.Equal(0, pending.Count);
        }

        [Fact]
        public void OriginVersionIsIncreased()
        {
            Assert.Equal(1, sut.OriginVersion);
        }

        [Fact]
        public void CurrentAndOriginVersionAresame()
        {
            Assert.Equal(sut.Version, sut.OriginVersion);
        }
    }
}