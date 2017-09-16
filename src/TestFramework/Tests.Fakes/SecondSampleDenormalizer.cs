using System.Threading.Tasks;
using BE.CQRS.Domain.Conventions;
using BE.CQRS.Domain.Events;

namespace Tests.Fakes
{
    [Denormalizer]
    public class SampleDenormalizer
    {
        public void On(IEvent @event)
        {
        }

        public void On<TDenormalmizerEvent>(dynamic @event)
        {
        }

        public Task On(DenormalizerEvent @event)
        {
            return Task.FromResult(true);
        }

        public Task On(DenormalizerEvent @event, bool invalidParam)
        {
            return Task.FromResult(true);
        }
    }
}