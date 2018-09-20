using System;
using System.Security.Cryptography;
using System.Threading.Tasks;
using BE.FluentGuard;
using Microsoft.Extensions.Logging;

namespace BE.CQRS.Domain.Commands
{
    public abstract class CommandBusBase : ICommandBus
    {
        private Predicate<ICommand> condition;
        private readonly ILogger logger;

        protected CommandBusBase(ILoggerFactory loggerFactory)
        {            
            Precondition.For(loggerFactory, nameof(loggerFactory)).NotNull("CommandBus Requires a loggerFactory");
            logger = loggerFactory.CreateLogger(GetType());
        }

        public void WithCondition(Predicate<ICommand> condition)
        {
            Precondition.For(condition, nameof(condition))
                .NotNull("When a condition has to be set it should not be null");

            this.condition = condition;
        }

        public Task<CommandBusResult> EnqueueAsync(ICommand cmd)
        {
            Precondition.For(cmd, nameof(cmd)).IsValidCommand();
            var type = cmd.GetType();

            if (condition == null || condition(cmd))
            {
                logger.LogTrace("Sending \"{type}\" command.", type);
                return EnqueueInternalAsync(cmd);
            }

            logger.LogTrace("Command \"{type}\" not set due to filter.", type);
            return Task.FromResult(CommandBusResult.Filtered());
        }

        protected abstract Task<CommandBusResult> EnqueueInternalAsync(ICommand cmd);
    }
}