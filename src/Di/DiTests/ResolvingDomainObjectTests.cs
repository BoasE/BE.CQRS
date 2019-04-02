using System;
using BE.CQRS.Domain.DomainObjects;
using Xunit;

namespace DiTests
{
    public abstract class ResolvingDomainObjectTests
    {
        protected abstract IDomainObjectActivator GetSut();

        private readonly string customerId = Guid.NewGuid().ToString();
        private readonly CustomerDomainObject domainObject;

        protected ResolvingDomainObjectTests()
        {
            IDomainObjectActivator sut = GetSut();

            domainObject = sut.Resolve<CustomerDomainObject>(customerId);
        }

        [Fact]
        public void ItWasCreated()
        {
            Assert.NotNull(domainObject);
        }

        [Fact]
        public void ItHasTheId()
        {
            Assert.Equal(customerId, domainObject.Id);
        }

        [Fact]
        public void VersionIsZero()
        {
            Assert.Equal(0, domainObject.Version);
        }

        [Fact]
        public void CommitVersionIsZero()
        {
            Assert.Equal(0, domainObject.CommitVersion);
        }

        [Fact]
        public void UnComittedEventsNotNull()
        {
            Assert.NotNull(domainObject.GetUncommittedEvents());
        }
    }
}