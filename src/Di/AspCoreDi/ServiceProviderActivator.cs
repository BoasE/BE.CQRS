using System;
using BE.CQRS.Domain.Denormalization;
using BE.CQRS.Domain.DomainObjects;
using BE.CQRS.Domain.Logging;
using BE.CQRS.Domain.States;
using BE.FluentGuard;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BE.CQRS.Di.AspCore
{
    public class ServiceCollectionActivator : IDomainObjectActivator, IDenormalizerActivator, IStateActivator
    {
        private readonly IServiceProvider provider;
        private readonly ILogger _logger;

        public ServiceCollectionActivator(IServiceProvider provider, ILogger logger)
        {
            Precondition.For(provider, nameof(provider)).NotNull();
            this._logger = logger;
            this.provider = provider;
        }

        public ServiceCollectionActivator(IServiceProvider provider) : this(provider, NoopLogger.Instance)
        {
        }

        public T Resolve<T>(string id) where T : class, IDomainObject
        {
            return Resolve(typeof(T), id) as T;
        }

        public IDomainObject Resolve(Type domainObjectType, string id)
        {
            var resolved = ActivatorUtilities.CreateInstance(provider, domainObjectType, id) as IDomainObject;

            if (resolved != null) return resolved;

            _logger.LogError("DomainObject Type \"{type}\" could not be resolved!", domainObjectType.FullName);
            throw new InvalidOperationException($"Could not resolve to required type. {domainObjectType.FullName}");
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
            return (IState)ActivatorUtilities.CreateInstance(provider, denormalizerType);
        }
    }
}