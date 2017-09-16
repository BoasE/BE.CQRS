using System;
using System.Threading.Tasks;
using BE.CQRS.Domain.Commands;
using FakeItEasy;
using Tests.Fakes;
using Xunit;

namespace BE.CQRS.Domain.Tests.InMemoryCommandBusTests
{
    public sealed class WhenPipelineThrows : GivenInMemoryBus
    {
        private readonly InMemoryCommandBus sut;

        public WhenPipelineThrows()
        {
            var fake = A.Fake<ICommandPipeline>();
            A.CallTo(() => fake.ExecuteAsync(A<ICommand>.Ignored)).Throws<InvalidOperationException>();

            sut = GetSut(fake);
        }

        [Fact]
        public Task TheBusDoesntCrash()
        {
            return Execute();
        }

        [Fact]
        public async Task ItIndicatesFail()
        {
            CommandBusResult result = await Execute();

            Assert.False(result.WasSuccessfull);
            Assert.False(result.WasFiltered);
            Assert.False(result.WasExecuted);
        }

        [Fact]
        public async Task ItHasAException()
        {
            CommandBusResult result = await Execute();

            Assert.NotNull(result.Exception);
        }

        private async Task<CommandBusResult> Execute()
        {
            CommandBusResult result = await sut.EnqueueAsync(new SampleCommand());
            return result;
        }
    }
}