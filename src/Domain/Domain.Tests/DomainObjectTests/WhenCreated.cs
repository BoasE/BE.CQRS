using System;
using Xunit;

namespace BE.CQRS.Domain.Tests.DomainObjectTests
{
    public class WhenCreated : GivenDomainObject
    {
        public string Id { get; } = Guid.NewGuid().ToString();

        private readonly TestDomainObject sut;

        public WhenCreated()
        {
            sut = GetSut();
        }

        [Fact]
        public void ItHasTheId()
        {
            Assert.Equal(Id, sut.Id);
        }

        [Fact]
        public void ItHasAEmptyVersion()
        {
            Assert.Equal(0, sut.Version);
        }

        [Fact]
        public void ItHasAEmptyOriginVersion()
        {
            Assert.Equal(0, sut.OriginVersion);
        }

        [Fact]
        public void OriginVersionEqualsCurrentVersion()
        {
            Assert.Equal(sut.Version, sut.OriginVersion);
        }

        [Fact]
        public void ItHasNoPendingEvents()
        {
            Assert.Equal(0, sut.GetUncommittedEvents().Count);
        }

        protected TestDomainObject GetSut()
        {
            return GetSut(Id);
        }
    }
}