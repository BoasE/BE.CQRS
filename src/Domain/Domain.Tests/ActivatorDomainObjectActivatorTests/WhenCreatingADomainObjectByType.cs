using BE.CQRS.Domain.DomainObjects;
using BE.CQRS.Domain.Tests.DomainObjectTests;
using Xunit;

namespace BE.CQRS.Domain.Tests.ActivatorDomainObjectActivatorTests
{
    public class WhenCreatingADomainObjectByType : GivenActivatorDomainObjectActivator
    {
        private readonly TestDomainObject domainObject;
        private readonly string id = "123sdfsfrsäü";

        public WhenCreatingADomainObjectByType()
        {
            ActivatorDomainObjectActivator sut = GetSut();
            domainObject = sut.Resolve(typeof(TestDomainObject), id) as TestDomainObject;
        }

        [Fact]
        public void ItIsNotNull()
        {
            Assert.NotNull(domainObject);
        }

        [Fact]
        public void ItHasTheId()
        {
            Assert.Equal(id, domainObject.Id);
        }

        [Fact]
        public void ItHasTheInitialVersion()
        {
            Assert.Equal(0, domainObject.Version);
        }

        [Fact]
        public void ItHasTheInitialOriginalVersion()
        {
            Assert.Equal(0, domainObject.OriginVersion);
        }
    }
}