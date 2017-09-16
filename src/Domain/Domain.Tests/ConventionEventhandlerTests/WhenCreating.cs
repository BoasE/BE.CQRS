using System;
using System.Reflection;
using BE.CQRS.Domain.Events.Handlers;
using Xunit;

namespace BE.CQRS.Domain.Tests.ConventionEventhandlerTests
{
    public class WhenCreating : GivenConventionEventHandler
    {
        [Fact]
        public void ItThrowsWhenAssembliesNull()
        {
            Assert.Throws<ArgumentNullException>(() => GetSut(null));
        }

        [Fact]
        public void ItThrowsWhenAssembliesAreEmpty()
        {
            Assert.Throws<ArgumentException>(() => GetSut());
        }

        [Fact]
        public void ItGetsCreatedIfAssemblyIsPresent()
        {
            ConventionEventHandler sut = GetSut(GetType().GetTypeInfo().Assembly);

            Assert.NotNull(sut);
        }
    }
}