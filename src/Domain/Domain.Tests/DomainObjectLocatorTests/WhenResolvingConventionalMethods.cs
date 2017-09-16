using System.Linq;
using BE.CQRS.Domain.Commands;
using BE.CQRS.Domain.DomainObjects;
using Tests.Fakes;
using Xunit;

namespace BE.CQRS.Domain.Tests.DomainObjectLocatorTests
{
    public sealed class WhenResolvingConventionalMethods : GivenLocator
    {
        [Fact]
        public void ItFindsTheCreateMethod()
        {
            DomainObjectLocator sut = GetSut();

            CommandMethodMapping[] result = sut.ResolveConventionalMethods(typeof(FakeObject)).ToArray();

            Assert.True(result.Any());
        }

        //[Fact]
        //public void ItFindsTheUpdateMethod()
        //{
        //    var sut = GetSut();

        //    var result = sut.ResolveConventionalMethods(typeof(FakeObject)).ToArray();

        //    Assert.True(result.Any((i) => i.Name.Equals("FooTaskUpdate", StringComparison.OrdinalIgnoreCase)));

        //}

        //[Fact]
        //public void ItFindsTheCreateMethodWithiTask()
        //{
        //    var sut = GetSut();

        //    var result = sut.ResolveConventionalMethods(typeof(FakeObject)).ToArray();

        //    Assert.True(result.Any((i) => i.Name.Equals("FooTask", StringComparison.OrdinalIgnoreCase)));

        //}
    }
}