using BE.CQRS.Domain.DomainObjects;

namespace DiTests
{
    public sealed class CustomerDomainObject : DomainObjectBase
    {
        public CustomerDomainObject(string id) : base(id)
        {
        }
    }
}