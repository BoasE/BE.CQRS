using System.Threading.Tasks;
using BE.CQRS.Domain.Configuration;
using BE.CQRS.Domain.Events;
using BE.CQRS.Domain.Events.Handlers;

namespace BE.CQRS.Domain.Denormalization
{
    public interface IImmediateConvetionDenormalizer : IEventHandler
    {
    }

    public sealed class ImmediateConvetionDenormalizer : IImmediateConvetionDenormalizer
    {
        private readonly ConventionEventHandler conventionHandler;
        public int HandlerCount { get; }

        public ImmediateConvetionDenormalizer(DenormalizerConfiguration config, IDenormalizerActivator activator)
        {
            conventionHandler = new ConventionEventHandler(activator, config.DenormalizerAssemblies);
        }

        public Task HandleAsync(IEvent @event)
        {
            return conventionHandler.HandleAsync(@event);
        }
    }
}