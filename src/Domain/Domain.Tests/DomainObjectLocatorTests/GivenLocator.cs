using System;
using System.Linq;
using BE.CQRS.Domain.DomainObjects;
using Xunit;

namespace BE.CQRS.Domain.Tests.DomainObjectLocatorTests
{
    public class GivenLocator
    {
        protected DomainObjectLocator GetSut()
        {
            return new DomainObjectLocator();
        }

        [Fact]
        public void ItThroesWhenNull()
        {
            DomainObjectLocator sut = GetSut();

            Assert.Throws<ArgumentNullException>(() =>
            {
                
                return sut.ResolveDomainObjects(null).ToList();
            });
        }
    }
}