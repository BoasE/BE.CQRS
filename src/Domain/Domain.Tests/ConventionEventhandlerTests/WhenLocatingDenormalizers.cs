using System.Reflection;
using BE.CQRS.Domain.Events.Handlers;
using Tests.Fakes;
using Xunit;

namespace BE.CQRS.Domain.Tests.ConventionEventhandlerTests
{
    public class WhenLocatingDenormalizers : GivenConventionEventHandler

    {
        private readonly ConventionEventHandler sut;

        public WhenLocatingDenormalizers()
        {
            sut = GetSut(typeof(SampleDenormalizer).GetTypeInfo().Assembly);
        }

        [Fact]
        public void TheCountIsRight()
        {
            Assert.Equal(2, sut.HandlerCount);
        }
    }
}