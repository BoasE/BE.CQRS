using System;
using System.Security.Cryptography;
using System.Threading.Tasks;
using BE.FluentGuard;

namespace BE.CQRS.Domain.Commands
{
    public abstract class CommandBusBase : ICommandBus
    {
        private Predicate<ICommand> condition;

        public void WithCondition(Predicate<ICommand> condition)
        {
            Precondition.For(condition, nameof(condition)).NotNull("When a condition has to be set it should not be null");
            this.condition = condition;
        }

        public Task<CommandBusResult> EnqueueAsync(ICommand cmd)
        {
            Precondition.For(cmd, nameof(cmd)).IsValidCommand();

            if (condition == null || condition(cmd))
            {
                return EnqueueInternalAsync(cmd);
            }

            return Task.FromResult(CommandBusResult.Filtered());
        }

        protected abstract Task<CommandBusResult> EnqueueInternalAsync(ICommand cmd);
    }
}