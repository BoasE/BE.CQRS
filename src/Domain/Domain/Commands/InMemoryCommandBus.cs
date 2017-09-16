using System;
using System.Diagnostics;
using System.Reactive.Subjects;
using System.Reflection;
using System.Threading.Tasks;
using BE.CQRS.Domain.Conventions;
using BE.CQRS.Domain.DomainObjects;
using BE.FluentGuard;

namespace BE.CQRS.Domain.Commands
{
    public sealed class InMemoryCommandBus : CommandBusBase
    {
        private readonly Subject<ICommand> subject = new Subject<ICommand>();
        private readonly ICommandPipeline handler;

        public InMemoryCommandBus(ICommandPipeline handler)
        {
            Precondition.For(() => handler).NotNull();
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
                Debug.WriteLine($"Error handling command \"{cmd.GetType().Name}\" - {err.Message}");
                result = CommandBusResult.Failed(err);
            }

            return result;
        }

        protected override async Task<CommandBusResult> EnqueueInternalAsync(ICommand cmd)
        {
            Precondition.For(cmd, nameof(cmd)).NotNull();
            CommandBusResult result = await HandleCommand(cmd);
            return result;
        }

        public static InMemoryCommandBus CreateConventionCommandBus(IDomainObjectRepository repository,
            params Assembly[] domainObjectAssemblies)
        {
            Precondition.For(repository, nameof(repository)).NotNull();

            var invoker = new ConventionCommandInvoker(repository);
            var handler = new ConventionCommandPipeline(invoker, new DomainObjectLocator(), domainObjectAssemblies);
            var bus = new InMemoryCommandBus(handler);

            return bus;
        }
    }
}