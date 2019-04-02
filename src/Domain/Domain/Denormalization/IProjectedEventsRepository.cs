using System.Collections.Specialized;
using System.Threading.Tasks;
using BE.CQRS.Domain.DomainObjects;
using BE.CQRS.Domain.Events;

namespace BE.CQRS.Domain.Denormalization
{
    public interface IProjectedEventsRepository
    {
        Task<long> Increment<TDomainObject>(string domainObjectId, IEvent processesEvent) where TDomainObject : IDomainObject;

        Task<bool> WasProcessed<TDomainObject>(string domainObjectId, IEvent @event) where TDomainObject : IDomainObject;
    }
}