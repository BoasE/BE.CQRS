using System.Threading.Tasks;
using BE.CQRS.Domain.Commands;
using Microsoft.Extensions.Logging;

namespace BE.CQRS.Domain.Processes
{
    public abstract class ProcessBase<TCommand> : IProcess<TCommand> where TCommand : ICommand
    {
        private readonly ILogger logger;
        private readonly string processName; 
        
        protected ICommandBus CommandBus { get; }

        protected IDomainObjectRepository Repository { get; }

        protected ProcessBase(ICommandBus commandBus, IDomainObjectRepository repository,ILoggerFactory factory)
        {
            processName = this.GetType().Name;
            logger = factory.CreateLogger<ProcessBase<TCommand>>();
            CommandBus = commandBus;
            Repository = repository;
        }

        public Task<ProcessResult> ExecuteAsync(TCommand command)
        {
            
            logger.LogTrace("Running Process {processName} is started",processName);
            return ExecuteInternalAsync(command);
        }

        protected abstract Task<ProcessResult> ExecuteInternalAsync(TCommand command);
    }
}