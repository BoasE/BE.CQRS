using BE.CQRS.Domain.DomainObjects;

namespace Tests.Fakes
{
    public abstract class AbstractFakeObject : DomainObjectBase
    {
        public AbstractFakeObject(string id) : base(id)
        {
        }
    }
}