using System.Threading.Tasks;
using BE.CQRS.Domain.Commands;
using FakeItEasy;
using Tests.Fakes;
using Xunit;

namespace BE.CQRS.Domain.Tests.InMemoryCommandBusTests
{
    public class WhenHavingCondition : GivenInMemoryBus
    {
        [Fact]
        public async Task ItExecutesWhenConditionIsFullfilled()
        {
            var pipeline = A.Fake<ICommandPipeline>();

            var command = new SampleCommand
            {
                Value = 42
            };

            CommandBusResult result = await Execute(pipeline, command);

            Assert.True(result.WasSuccessfull);
        }

        [Fact]
        public async Task ItDoesNothingWhenCondingIsNotMet()
        {
            var pipeline = A.Fake<ICommandPipeline>();

            var command = new SampleCommand
            {
                Value = 5
            };

            CommandBusResult result = await Execute(pipeline, command);

            Assert.False(result.WasSuccessfull);
        }

        private Task<CommandBusResult> Execute(ICommandPipeline pipeline, ICommand command)
        {
            InMemoryCommandBus bus = GetSut(pipeline);
            bus.WithCondition(i =>
            {
                var temp = i as SampleCommand;
                return temp.Value == 42;
            });

            return bus.EnqueueAsync(command);
        }
    }
}