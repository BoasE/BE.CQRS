using BE.CQRS.Domain.DomainObjects;
using BE.FluentGuard;

namespace BE.CQRS.Domain.Denormalization
{
    public sealed class DenormalizerDiContext
    {
        public IDenormalizerActivator DenormalizerActivator { get; }
        
        public DenormalizerDiContext(IDenormalizerActivator denormalizerActivator)
        {
            Precondition.For(denormalizerActivator, nameof(denormalizerActivator)).NotNull();
            DenormalizerActivator = denormalizerActivator;
        }
    }
}