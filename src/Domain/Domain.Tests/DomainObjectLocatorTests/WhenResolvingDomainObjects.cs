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

            Assert.Null(result.FirstOrDefault(i =>
                i.Name.Equals("PrivateFakeObject", StringComparison.OrdinalIgnoreCase)));
        }

        [Fact]
        public void ItDoesntIncludeNonDomainObjects()
        {
            List<Type> result = Resolve().ToList();

            Assert.Null(result.FirstOrDefault(i => i.Name.Equals("DummyClass", StringComparison.OrdinalIgnoreCase)));
        }

        [Fact]
        public void ItDoesNotResolveDomainObjectsTwice()
        {
            Assembly fakeAssembly = typeof(FakeObject).GetTypeInfo().Assembly;
            var assemblies = new HashSet<Assembly> { fakeAssembly, fakeAssembly };

            List<Type> result = Resolve(assemblies).ToList();

            int uniqueTypeCount = result.Distinct().Count();
            Assert.Equal(uniqueTypeCount, result.Count);
        }

        private IEnumerable<Type> Resolve()
        {
            DomainObjectLocator sut = GetSut();

            var asm = new HashSet<Assembly>() { typeof(FakeObject).GetTypeInfo().Assembly };
            return sut.ResolveDomainObjects(asm);
        }

        private IEnumerable<Type> Resolve(ISet<Assembly> assemblies)
        {
            DomainObjectLocator sut = GetSut();
            return sut.ResolveDomainObjects(assemblies);
        }
    }
}