using System;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;
using BE.CQRS.Domain.States;
using BE.FluentGuard;

namespace BE.CQRS.Domain.DomainObjects
{
    public sealed class ActivatorDomainObjectActivator : IDomainObjectActivator,IStateActivator
    {
        private static readonly TypeInfo DomainObjectInfo = typeof(IDomainObject).GetTypeInfo();

        // Caches to reduce reflection overhead and delegate allocations
        private static readonly ConcurrentDictionary<Type, Func<string, IDomainObject>> DomainObjectFactories = new();
        private static readonly ConcurrentDictionary<Type, Func<IState>> StateFactories = new();

        public T Resolve<T>(string id) where T : class, IDomainObject
        {
            Precondition.For(id, nameof(id)).NotNullOrWhiteSpace();

            var factory = DomainObjectFactories.GetOrAdd(typeof(T), CreateDomainObjectFactory);
            return (T)factory(id);
        }

        public IDomainObject Resolve(Type domainObjectType, string id)
        {
            Precondition.For(domainObjectType, nameof(domainObjectType))
                .NotNull()
                .True(i => DomainObjectInfo.IsAssignableFrom(i.GetTypeInfo()));

            Precondition.For(id, nameof(id)).NotNullOrWhiteSpace();

            var factory = DomainObjectFactories.GetOrAdd(domainObjectType, CreateDomainObjectFactory);
            return factory(id);
        }

        public T ResolveState<T>() where T : class,IState
        {
            var factory = StateFactories.GetOrAdd(typeof(T), CreateStateFactory);
            return (T)factory();
        }

        public IState ResolveState(Type stateType)
        {
            var factory = StateFactories.GetOrAdd(stateType, CreateStateFactory);
            return factory();
        }

        // Factory for domain objects with (string id) constructor
        private static Func<string, IDomainObject> CreateDomainObjectFactory(Type type)
        {
            // Expect exactly one (string) constructor
            var ctor = type.GetConstructor(new[] { typeof(string) });
            if (ctor == null)
            {
                // Fallback, if no matching ctor exists
                return id => (IDomainObject)Activator.CreateInstance(type, id);
            }

            var idParam = Expression.Parameter(typeof(string), "id");
            var newExpr = Expression.New(ctor, idParam);
            var lambda = Expression.Lambda<Func<string, IDomainObject>>(Expression.Convert(newExpr, typeof(IDomainObject)), idParam);
            return lambda.Compile();
        }

        // Factory for states with parameterless constructor
        private static Func<IState> CreateStateFactory(Type type)
        {
            var ctor = type.GetConstructor(Type.EmptyTypes);
            if (ctor == null)
            {
                return () => (IState)Activator.CreateInstance(type);
            }

            var newExpr = Expression.New(ctor);
            var lambda = Expression.Lambda<Func<IState>>(Expression.Convert(newExpr, typeof(IState)));
            return lambda.Compile();
        }
    }
}