using System.Threading.Tasks;
using BE.CQRS.Domain.DomainObjects;
using BE.CQRS.Domain.Events;

namespace BE.CQRS.Domain.Denormalization
{
    public interface IReadVersionRepository
    {
        Task<long> Increment<TDomainObject>(string domainObjectId, IEvent processesEvent) where TDomainObject : IDomainObject;

        Task<long> Version<TDomainObject>(string domainObjectId) where TDomainObject : IDomainObject;
    }
}