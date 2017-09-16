using System;
using BE.CQRS.Domain.DomainObjects;
using Microsoft.Extensions.DependencyInjection;

namespace BE.CQRS.Di.AspCore
{
    public class ServiceCollectionActivator : IDomainObjectActivator
    {
        private IServiceProvider provider;

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
    }
}