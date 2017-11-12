using System;
using BE.CQRS.Domain.DomainObjects;

namespace BE.CQRS.Domain.Denormalization
{
    public interface IDenormalizerActivator
    {
        T ResolveDenormalizer<T>() where T : class;

        object ResolveDenormalizer(Type denormalizerType);
    }
}