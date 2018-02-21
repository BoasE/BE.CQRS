using System;

namespace BE.CQRS.Domain.States
{
    public interface IStateActivator
    {
        T ResolveState<T>() where T : class,IState;

        IState ResolveState(Type denormalizerType);
    }
}