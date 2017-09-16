using BE.CQRS.Domain.DomainObjects;

namespace Tests.Fakes
{
    class PrivateFakeObject : DomainObjectBase
    {
        public PrivateFakeObject(string id) : base(id)
        {
        }
    }
}