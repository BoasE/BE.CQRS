using BE.CQRS.Domain;
using BE.CQRS.Domain.Conventions;
using BE.CQRS.Domain.DomainObjects;

namespace WebApplication.Domain
{
    public sealed class DomainObjectSample : DomainObjectBase
    {
        public DomainObjectSample(string id) : base(id)
        {
        }

        [Create]
        public void Create(CreateCommand cmd )
        {
            RaiseEvent<SampleCreatedEvent>(x=>x.Value = cmd.Value);
        }
    }
}