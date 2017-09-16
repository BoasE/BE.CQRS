using BE.CQRS.Domain.Commands;

namespace BE.CQRS.Domain.Tests.InMemoryCommandBusTests
{
    public class GivenInMemoryBus
    {
        public InMemoryCommandBus GetSut(ICommandPipeline pipeline)
        {
            var sut = new InMemoryCommandBus(pipeline);
            return sut;
        }
    }
}