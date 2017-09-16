using System.Threading.Tasks;
using BE.CQRS.Domain.Commands;

namespace BE.CQRS.Domain.Processes
{
    public abstract class ProcessBase<TCommand> : IProcess<TCommand> where TCommand : ICommand
    {
        protected ICommandBus CommandBus { get; }

        protected IDomainObjectRepository Repository { get; }

        protected ProcessBase(ICommandBus commandBus, IDomainObjectRepository repository)
        {
            CommandBus = commandBus;
            Repository = repository;
        }

        public Task<ProcessResult> ExecuteAsync(TCommand command)
        {
            return ExecuteInternalAsync(command);
        }

        protected abstract Task<ProcessResult> ExecuteInternalAsync(TCommand command);
    }
}