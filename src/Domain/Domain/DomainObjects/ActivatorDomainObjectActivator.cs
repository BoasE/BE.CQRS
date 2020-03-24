using System;
using System.Reflection;
using BE.CQRS.Domain.States;
using BE.FluentGuard;

namespace BE.CQRS.Domain.DomainObjects
{
    public sealed class ActivatorDomainObjectActivator : IDomainObjectActivator,IStateActivator
    {
        private static readonly TypeInfo DomainObjectInfo = typeof(IDomainObject).GetTypeInfo();

        public T Resolve<T>(string id) where T : class, IDomainObject
        {
            Precondition.For(id, nameof(id)).NotNullOrWhiteSpace();

            return (T)Activator.CreateInstance(typeof(T), id);
        }

        public IDomainObject Resolve(Type domainObjectType, string id)
        {
            Precondition.For(domainObjectType, nameof(domainObjectType))
                .NotNull()
                .True(i => DomainObjectInfo.IsAssignableFrom(i.GetTypeInfo()));

            Precondition.For(id, nameof(id)).NotNullOrWhiteSpace();

            return (IDomainObject)Activator.CreateInstance(domainObjectType, id);
        }

        public T ResolveState<T>() where T : class,IState
        {
            return (T)Activator.CreateInstance(typeof(T));
        }

        public IState ResolveState(Type stateType)
        {
            return (IState)Activator.CreateInstance(stateType);
        }
    }
}