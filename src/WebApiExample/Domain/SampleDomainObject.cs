using BE.CQRS.Domain;
using BE.CQRS.Domain.DomainObjects;

namespace WebApiExample
{
    public class SampleDomainObject : DomainObjectBase
    {
        public SampleDomainObject(string id) : base(id)
        {
        }
    }
}