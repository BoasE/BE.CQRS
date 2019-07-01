using BE.CQRS.Domain.States;
using Xunit;

namespace BE.CQRS.Domain.Tests.DomainObjectStateRuntimeTests
{
    public class WhenCreatingState : GivenRuntime
    {
        [Fact]
        public void ConventionWithInterfaceWasCalled()
        {
            DomainObjectStateRuntime sut = GetSut();

            var state = sut.State<TestState>(true);

            Assert.Equal(1, state.Counter);
        }
    }
}