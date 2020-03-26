using System.Threading.Tasks;
using BE.CQRS.Domain.Events;

namespace BE.CQRS.Domain.Denormalization
{
    public sealed class NoopImmediateConventionDenormalizer :IImmediateConventionDenormalizer
    {
        public int HandlerCount { get; } = 0;
        public Task HandleAsync(IEvent @event)
        {
            return Task.CompletedTask;
        }
    }
}