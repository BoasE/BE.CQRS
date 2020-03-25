using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reactive.Subjects;
using System.Reflection;
using System.Threading.Tasks;
using BE.CQRS.Domain.Conventions;
using BE.CQRS.Domain.DomainObjects;
using BE.FluentGuard;
using Microsoft.Extensions.Logging;

namespace BE.CQRS.Domain.Commands
{
    public sealed class InMemoryCommandBus : CommandBusBase
    {
        private readonly Subject<ICommand> subject = new Subject<ICommand>();
        private readonly ICommandPipeline handler;
        private readonly ILogger logger;

        public InMemoryCommandBus(ICommandPipeline handler, ILoggerFactory loggerFactory) : base(loggerFactory)
        {
            Precondition.For(() => handler).NotNull();
            logger = loggerFactory.CreateLogger<InMemoryCommandBus>();
            this.handler = handler;

            subject.Subscribe(async msg => await HandleCommand(msg));
        }

        private async Task<CommandBusResult> HandleCommand(ICommand cmd)
        {
            CommandBusResult result;
            try
            {
                await handler.ExecuteAsync(cmd);
                result = CommandBusResult.Succeeded();
            }
            catch (Exception err)
            {
                var type = cmd.GetType();
                var msg = err.Message;
                logger.LogError(err, "Error handling command \"{type}\" - {msg}", type, msg);

                result = CommandBusResult.Failed(err);
            }

            return result;
        }

        protected override async Task<CommandBusResult> EnqueueInternalAsync(ICommand cmd)
        {
            Precondition.For(cmd, nameof(cmd)).IsValidCommand();
            CommandBusResult result = await HandleCommand(cmd);
            return result;
        }

        public static InMemoryCommandBus CreateConventionCommandBus(IDomainObjectRepository repository,
            ILoggerFactory loggerFactory,
            IEnumerable<Assembly> domainObjectAssemblies)
        {
            Precondition.For(repository, nameof(repository)).NotNull();
            Precondition.For(loggerFactory, nameof(loggerFactory)).NotNull();

            var invoker = new ConventionCommandInvoker(repository,loggerFactory);
            var handler = new ConventionCommandPipeline(invoker, new DomainObjectLocator(), loggerFactory,
                domainObjectAssemblies);
            
            var bus = new InMemoryCommandBus(handler, loggerFactory);

            return bus;
        }
    }
}