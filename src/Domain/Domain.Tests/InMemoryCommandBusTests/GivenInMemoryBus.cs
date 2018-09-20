using BE.CQRS.Domain.Commands;
using BE.CQRS.Domain.Logging;

namespace BE.CQRS.Domain.Tests.InMemoryCommandBusTests
{
    public class GivenInMemoryBus
    {
        public InMemoryCommandBus GetSut(ICommandPipeline pipeline)
        {
            var sut = new InMemoryCommandBus(pipeline,new NoopLoggerFactory());
            return sut;
        }
    }
}