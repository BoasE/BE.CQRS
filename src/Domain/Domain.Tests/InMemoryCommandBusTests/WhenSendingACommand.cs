using System;
using System.Threading.Tasks;
using BE.CQRS.Domain.Commands;
using FakeItEasy;
using Tests.Fakes;
using Xunit;

namespace BE.CQRS.Domain.Tests.InMemoryCommandBusTests
{
    public class WhenSendingACommand : GivenInMemoryBus
    {
        [Fact]
        public async Task ItCallsThepipeline()
        {
            var cmd = new SampleCommand();
            var pipeline = A.Fake<ICommandPipeline>();

            await Execute(pipeline, cmd);

            A.CallTo(() => pipeline.ExecuteAsync(A<ICommand>.That.IsSameAs(cmd))).MustHaveHappened();
        }

        [Fact]
        public async Task ItIndicatesSucesss()
        {
            var cmd = new SampleCommand();
            var pipeline = A.Fake<ICommandPipeline>();

            CommandBusResult result = await Execute(pipeline, cmd);
            Assert.True(result.WasSuccessfull);
        }

        [Fact]
        public Task ItThrowsWhenCommandIsNull()
        {
            var pipeline = A.Fake<ICommandPipeline>();

            return Assert.ThrowsAsync<ArgumentNullException>(async () => await Execute(pipeline, null));
        }

        private async Task<CommandBusResult> Execute(ICommandPipeline pipeline, ICommand cmd)
        {
            InMemoryCommandBus sut = GetSut(pipeline);
            CommandBusResult result = await sut.EnqueueAsync(cmd);

            return result;
        }
    }
}