using System.Threading.Tasks;
using BE.CQRS.Domain.Configuration;
using BE.CQRS.Domain.Events;
using BE.CQRS.Domain.Events.Handlers;

namespace BE.CQRS.Domain.Denormalization
{
    public interface IImmediateConventionDenormalizer : IEventHandler
    {
    }

    public sealed class ImmediateConventionDenormalizer : IImmediateConventionDenormalizer
    {
        private readonly ConventionEventHandler conventionHandler;

        public int HandlerCount => conventionHandler.HandlerCount;

        public ImmediateConventionDenormalizer(DenormalizerConfiguration config, IDenormalizerActivator activator)
        {
            conventionHandler = new ConventionEventHandler(activator, config.DenormalizerAssemblies);
        }

        public Task HandleAsync(IEvent @event)
        {
            return conventionHandler.HandleAsync(@event);
        }
    }
}