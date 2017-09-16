using System;
using BE.CQRS.Domain.DomainObjects;
using BE.CQRS.Domain.Tests.DomainObjectTests;
using Xunit;

namespace BE.CQRS.Domain.Tests.ActivatorDomainObjectActivatorTests
{
    public class WhenIvalidActivations : GivenActivatorDomainObjectActivator
    {
        [Fact]
        public void ItThroesWhenTypeIsNull()
        {
            ActivatorDomainObjectActivator sut = GetSut();

            Assert.Throws<ArgumentNullException>(() => sut.Resolve(null, "1213"));
        }

        [Fact]
        public void ItThroesWhenIdIsNull()
        {
            ActivatorDomainObjectActivator sut = GetSut();

            Assert.Throws<ArgumentNullException>(() => sut.Resolve(typeof(TestDomainObject), null));
        }

        [Fact]
        public void ItThroesWhenIdIsNullGeneric()
        {
            ActivatorDomainObjectActivator sut = GetSut();

            Assert.Throws<ArgumentNullException>(() => sut.Resolve<TestDomainObject>(null));
        }

        [Fact]
        public void ItThroesWhenIdIsEmptylGeneric()
        {
            ActivatorDomainObjectActivator sut = GetSut();

            Assert.Throws<ArgumentException>(() => sut.Resolve<TestDomainObject>(" "));
        }

        [Fact]
        public void ItThroesWhenIdIsEmpty()
        {
            ActivatorDomainObjectActivator sut = GetSut();

            Assert.Throws<ArgumentException>(() => sut.Resolve(typeof(TestDomainObject), "  "));
        }

        [Fact]
        public void ItThroesWhenTypeIsNotADomainObject()
        {
            ActivatorDomainObjectActivator sut = GetSut();

            Assert.Throws<ArgumentException>(() => sut.Resolve(typeof(WhenIvalidActivations), "  "));
        }
    }
}