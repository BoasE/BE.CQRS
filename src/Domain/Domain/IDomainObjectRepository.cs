using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BE.CQRS.Domain.DomainObjects;
using BE.CQRS.Domain.EventDescriptions;
using BE.CQRS.Domain.Events;

namespace BE.CQRS.Domain
{
    public interface IDomainObjectRepository
    {
        IAsyncEnumerable<DescribedEvent> EnumerateAllDescribed(CancellationToken token);
        
        IDomainObject New(Type domainObjectType, string id);

        Task<T> Get<T>(string id) where T : class, IDomainObject;

        Task<T> Get<T>(string id, long version) where T : class, IDomainObject;
        Task<T> Get<T>(string id, long version, CancellationToken token) where T : class, IDomainObject;

        Task<T> Get<T>(string id, CancellationToken token) where T : class, IDomainObject;

        Task<T> Get<T>(string id, ISet<Type> eventTypes, CancellationToken token) where T : class, IDomainObject;

        Task<IDomainObject> Get(string id, Type domainObjectType);

        Task<IDomainObject> Get(string id, Type domainObjectType, CancellationToken token);

        Task<AppendResult> SaveAsync<T>(T domainObject) where T : class, IDomainObject;

        Task<AppendResult> SaveAsync<T>(T domainObject, bool preventVersionCheck) where T : class, IDomainObject;

        Task<bool> Exists<T>(T domainobject) where T : class, IDomainObject;

        Task<bool> Exists<T>(string id) where T : class, IDomainObject;

        IAsyncEnumerable<IEvent> EnumerateAll(CancellationToken token);

        Task Remove<T>(string id) where T : class, IDomainObject;

        Task<List<DescribedEvent>> GetDescribedHistory<T>(string id) where T : class, IDomainObject;
    }
}