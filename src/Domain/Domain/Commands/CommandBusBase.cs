using System;
using System.Threading.Tasks;
using BE.FluentGuard;

namespace BE.CQRS.Domain.Commands
{
    public abstract class CommandBusBase : ICommandBus
    {
        private Predicate<ICommand> condition;

        public void WithCondition(Predicate<ICommand> predicate)
        {
            condition = predicate;
        }

        public Task<CommandBusResult> EnqueueAsync(ICommand cmd)
        {
            Precondition.For(cmd, nameof(cmd)).NotNull();
            Precondition.For(cmd.DomainObjectId, nameof(cmd.DomainObjectId)).NotNullOrWhiteSpace();
            
            if (condition == null || condition(cmd))
            {
                return EnqueueInternalAsync(cmd);
            }

            return Task.FromResult(CommandBusResult.Filtered());
        }

        protected abstract Task<CommandBusResult> EnqueueInternalAsync(ICommand cmd);
    }
}