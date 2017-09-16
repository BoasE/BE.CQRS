using System;
using System.Linq;
using System.Reflection;
using BE.CQRS.Domain.Denormalization;
using Tests.Fakes;
using Xunit;

namespace BE.CQRS.Domain.Tests.DenormalizerLocatorTests
{
    public sealed class WhenLocatingDomainObjects : GivenDenormalizerLocator
    {
        [Fact]
        public void ItFindsDomainObjects()
        {
            Type[] types = ResolveDenormalizers();

            Assert.True(types.Contains(typeof(SampleDenormalizer)));
        }

        [Fact]
        public void ItDoesntReturnAbstractTypes()
        {
            Type[] types = ResolveDenormalizers();

            Assert.False(types.Contains(typeof(AbstractNormalizer)));
        }

        [Fact]
        public void ItDoesntFindsInternalClasses()
        {
            Type[] types = ResolveDenormalizers();

            Assert.False(types.Any(i => i.Name.Equals("InternalDenormalizer")));
        }

        private Type[] ResolveDenormalizers()
        {
            Type type = typeof(SampleDenormalizer);
            DenormalizerLocator sut = GetSut();
            Assembly asm = type.GetTypeInfo().Assembly;

            return sut.DenormalizerFromAsm(asm).ToArray();
        }
    }
}