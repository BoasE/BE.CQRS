using System.Linq;
using BE.CQRS.Domain.Events.Handlers;
using Tests.Fakes;
using Xunit;

namespace BE.CQRS.Domain.Tests.EventMethodCoventionTests
{
    public class GivenEventMethodConvetion
    {
        private readonly EventHandlerMethod[] sutState;

        protected OnPrefixEventMethodConvetion GetSut()
        {
            return new OnPrefixEventMethodConvetion();
        }

        public GivenEventMethodConvetion()
        {
            OnPrefixEventMethodConvetion sut = GetSut();
            sutState = sut.ResolveEventMethods(typeof(SampleDenormalizer)).ToArray();
        }

        [Fact]
        public void ItFindsTheExpectedMethod()
        {
            Assert.Equal(2, sutState.Length);
            Assert.Equal("On", sutState.First().Method.Name);
        }
    }
}