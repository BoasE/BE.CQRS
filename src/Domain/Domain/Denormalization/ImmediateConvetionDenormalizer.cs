using System.Threading.Tasks;
using BE.CQRS.Domain.Configuration;
using BE.CQRS.Domain.Events;
using BE.CQRS.Domain.Events.Handlers;
using Microsoft.Extensions.Logging;

namespace BE.CQRS.Domain.Denormalization
{
    public interface IImmediateConventionDenormalizerPipeline : IEventHandlerPipeline
    {
    }

    public sealed class ImmediateConventionDenormalizerPipeline : IImmediateConventionDenormalizerPipeline
    {
        private readonly ConventionEventHandler conventionHandler;

        public int HandlerCount => conventionHandler.HandlerCount;

        public ImmediateConventionDenormalizerPipeline(DenormalizerConfiguration config,
            DenormalizerDiContext diContext,
            ILogger<ConventionEventHandler> logger, IBackgroundEventQueue queue = null)
        {
            conventionHandler = new ConventionEventHandler(diContext, logger, queue,
                config.DenormalizerAssemblies);
        }

        public Task HandleAsync(IEvent @event)
        {
            return conventionHandler.HandleAsync(@event);
        }
    }
}