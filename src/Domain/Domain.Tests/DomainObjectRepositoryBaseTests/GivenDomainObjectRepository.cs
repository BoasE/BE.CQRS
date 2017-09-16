using BE.CQRS.Domain.DomainObjects;

namespace BE.CQRS.Domain.Tests.DomainObjectRepositoryBaseTests
{
    public class GivenDomainObjectRepository
    {
        protected DomainObjectRepositoryBase GetSut(IDomainObjectActivator activator)
        {
            return new FakeRepository(activator);
        }
    }
}