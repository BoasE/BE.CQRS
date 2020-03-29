using System;
using System.Reflection;
using BE.CQRS.Domain.DomainObjects;
using BE.CQRS.Domain.States;
using Microsoft.Practices.Unity;

namespace BE.CQRS.Di.Unity
{
    public sealed class UnityDomainObjectActivator : IDomainObjectActivator,IStateActivator
    {
        private static readonly TypeInfo DomainObjectInfo = typeof(IDomainObject).GetTypeInfo();
        private readonly IUnityContainer container;

        public UnityDomainObjectActivator(IUnityContainer container)
        {
            this.container = container;
        }

        public T Resolve<T>(string id) where T : class, IDomainObject
        {
            return container.Resolve<T>(new OrderedParametersOverride(id));
        }

        public IDomainObject Resolve(Type domainObjectType, string id)
        {
            return (IDomainObject)container.Resolve(domainObjectType, new OrderedParametersOverride(id));
        }

        public T ResolveState<T>() where T : class, IState
        {
            return container.Resolve<T>();
        }

        public IState ResolveState(Type denormalizerType)
        {
            return (IState) container.Resolve(denormalizerType);
        }
    }
}