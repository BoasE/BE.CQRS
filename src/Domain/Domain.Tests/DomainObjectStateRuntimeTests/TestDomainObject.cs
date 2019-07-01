using BE.CQRS.Domain.DomainObjects;

namespace BE.CQRS.Domain.Tests.DomainObjectStateRuntimeTests
{
    public class TestDomainObject : DomainObjectBase
    {
        public TestDomainObject(string id) : base(id)
        {
        }
    }
}