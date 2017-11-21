using System;

namespace BE.CQRS.Domain.Denormalization
{
    public interface IDenormalizerActivator
    {
        T ResolveDenormalizer<T>() where T : class;

        object ResolveDenormalizer(Type denormalizerType);
    }
}