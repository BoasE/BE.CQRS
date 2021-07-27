using System.Threading;
using System.Threading.Tasks;
using BE.CQRS.Domain.DomainObjects;

namespace BE.CQRS.Domain.Denormalization
{
    public interface IProjectionRebuilder
    {
        Task Execute(CancellationToken token);

        Task RebuildDomainobject<T>(string id) where T : class, IDomainObject;
    }
}