using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BE.CQRS.Domain.DomainObjects;
using Tests.Fakes;
using Xunit;

namespace BE.CQRS.Domain.Tests.DomainObjectLocatorTests
{
    public class WhenResolvingDomainObjects : GivenLocator
    {
        [Fact]
        public void ItFindsTheAggregate()
        {
            List<Type> result = Resolve().ToList();

            Assert.True(result.Contains(typeof(FakeObject)));
        }

        [Fact]
        public void ItDoesntIncludeAbstractClasses()
        {
            List<Type> result = Resolve().ToList();

            Assert.False(result.Contains(typeof(AbstractFakeObject)));
        }

        [Fact]
        public void ItDoesntIncludePrivateClasses()
        {
            List<Type> result = Resolve().ToList();

            Assert.Null(result.FirstOrDefault(
                i => i.Name.Equals("PrivateFakeObject", StringComparison.OrdinalIgnoreCase)));
        }

        [Fact]
        public void ItDoesntIncludeNonDomainObjects()
        {
            List<Type> result = Resolve().ToList();

            Assert.Null(result.FirstOrDefault(i => i.Name.Equals("DummyClass", StringComparison.OrdinalIgnoreCase)));
        }

        private IEnumerable<Type> Resolve()
        {
            DomainObjectLocator sut = GetSut();

            var asm = new List<Assembly>() {typeof(FakeObject).GetTypeInfo().Assembly};
            return sut.ResolveDomainObjects(asm);
        }
    }
}