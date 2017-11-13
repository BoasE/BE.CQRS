using System.Threading.Tasks;
using BE.CQRS.Domain;
using BE.CQRS.Domain.Commands;
using BE.FluentGuard;
using Microsoft.AspNetCore.Mvc;

namespace BE.CQRS.AspCore
{
    public abstract class EventSourcedControllerBase : ControllerBase
    {
        protected ICommandBus CommandBus { get; private set; }

        protected IDomainObjectRepository DomainObjectRepository { get; private set; }

        protected EventSourcedControllerBase(ICommandBus bus, IDomainObjectRepository domainObjectRepository)
        {
            Precondition.For(() => bus).NotNull();

            CommandBus = bus;
            DomainObjectRepository = domainObjectRepository;
        }

        protected async Task<ActionResult> AcceptCommand(ICommand command)
        {
            await CommandBus.EnqueueAsync(command);

            return Accepted();
        }

        protected async Task<ActionResult> AcceptCommand(ICommand command, string uri)
        {
            await CommandBus.EnqueueAsync(command);

            return Accepted(uri);
        }

        protected async Task<ActionResult> AcceptCommand(ICommand command, string uri, object value)
        {
            await CommandBus.EnqueueAsync(command);

            return Accepted(uri, value);
        }
    }
}