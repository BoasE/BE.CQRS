using System.Threading.Tasks;
using BE.CQRS.Domain.Commands;

namespace BE.CQRS.Domain.Processes
{
    public abstract class ReadProcessBase<TCommand, TContent> : IReadProcess<TCommand, TContent>
        where TCommand : ICommand
    {
        protected ICommandBus CommandBus { get; }

        protected IDomainObjectRepository Repository { get; }

        protected ReadProcessBase(ICommandBus commandBus, IDomainObjectRepository repository)
        {
            CommandBus = commandBus;
            Repository = repository;
        }

        public Task<ContentProcessResult<TContent>> ExecuteAsync(TCommand command)
        {
            return ExecuteInternalAsync(command);
        }

        protected abstract Task<ContentProcessResult<TContent>> ExecuteInternalAsync(TCommand command);
    }
}