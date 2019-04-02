using System.Collections.Generic;
using System.Threading.Tasks;
using BE.CQRS.Domain.Events;
using BE.CQRS.Domain.Events.Handlers;
using BE.FluentGuard;

namespace BE.CQRS.Domain.Denormalization
{
    public sealed class DirectEventDenormalizer
    {
        private readonly IEventHandler handler;

        public DirectEventDenormalizer(IEventHandler handler)
        {
            Precondition.For(handler, nameof(handler)).NotNull("The event header must be passed!");
            this.handler = handler;
        }

        public IEnumerable<Task> Next(IEnumerable<IEvent> events)
        {
            foreach (var @event in events)
            {
                yield return handler.HandleAsync(@event);
            }
        }
    }
}