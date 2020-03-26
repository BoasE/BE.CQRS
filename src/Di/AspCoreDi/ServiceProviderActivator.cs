using System;
using BE.CQRS.Domain.Denormalization;
using BE.CQRS.Domain.DomainObjects;
using BE.CQRS.Domain.States;
using Microsoft.Extensions.DependencyInjection;

namespace BE.CQRS.Di.AspCore
{
    public class ServiceCollectionActivator : IDomainObjectActivator, IDenormalizerActivator, IStateActivator
    {
        private IServiceProvider provider;

        public ServiceCollectionActivator()
        {
        }

        public ServiceCollectionActivator(IServiceProvider provider)
        {
            this.provider = provider;
        }


        public void UseProvider(IServiceProvider providerInstance)
        {
            provider = providerInstance;
        }

        public T Resolve<T>(string id) where T : class, IDomainObject
        {
            return Resolve(typeof(T), id) as T;
        }

        public IDomainObject Resolve(Type domainObjectType, string id)
        {
            return ActivatorUtilities.CreateInstance(provider, domainObjectType, id) as IDomainObject;
        }

        public T ResolveDenormalizer<T>() where T : class
        {
            return ResolveDenormalizer(typeof(T)) as T;
        }

        public object ResolveDenormalizer(Type denormalizerType)
        {
            return ActivatorUtilities.CreateInstance(provider, denormalizerType);
        }

        public T ResolveState<T>() where T : class, IState
        {
            return ResolveState(typeof(T)) as T;
        }

        public IState ResolveState(Type denormalizerType)
        {
            return (IState) ActivatorUtilities.CreateInstance(provider, denormalizerType);
        }
    }
}