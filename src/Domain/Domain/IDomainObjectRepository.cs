using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BE.CQRS.Domain.DomainObjects;
using BE.CQRS.Domain.Events;

namespace BE.CQRS.Domain
{
    public interface IDomainObjectRepository
    {
        IDomainObject New(Type domainObjectType, string id);

        IObservable<T> Get<T>(string id) where T : class, IDomainObject;

        IObservable<T> Get<T>(string id, CancellationToken token) where T : class, IDomainObject;

        IObservable<T> Get<T>(string id, ISet<Type> eventTypes, CancellationToken token) where T : class, IDomainObject;

        IObservable<IDomainObject> Get(string id, Type domainObjectType);

        IObservable<IDomainObject> Get(string id, Type domainObjectType, CancellationToken token);

        Task<AppendResult> SaveAsync<T>(T domainObject) where T : class, IDomainObject;

        Task<AppendResult> SaveAsync<T>(T domainObject, bool preventVersionCheck) where T : class, IDomainObject;

        Task<bool> Exists<T>(T domainobject) where T : class, IDomainObject;

        Task<bool> Exists<T>(string id) where T : class, IDomainObject;

        Task EnumerateAll(Func<IEvent, Task> callback);
    }
}