using BE.CQRS.Domain.Configuration;
using BE.CQRS.Domain.DomainObjects;
using BE.CQRS.Domain.States;

namespace BE.CQRS.Domain.Tests.DomainObjectRepositoryBaseTests
{
    public class GivenDomainObjectRepository
    {
        protected DomainObjectRepositoryBase GetSut(IDomainObjectActivator activator,IStateActivator stateActivator)
        {
            var repo = new FakeRepository(new EventSourceConfiguration(),new EventsourceDIContext(activator,stateActivator));
            return repo;
        }
    }
}