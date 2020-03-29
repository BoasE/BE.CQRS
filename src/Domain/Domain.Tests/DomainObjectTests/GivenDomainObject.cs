using BE.CQRS.Domain.Configuration;
using BE.CQRS.Domain.DomainObjects;
using BE.CQRS.Domain.States;

namespace BE.CQRS.Domain.Tests.DomainObjectTests
{
    public class GivenDomainObject
    {
        protected TestDomainObject GetSut(string id)
        {
            var bo =  new TestDomainObject(id);
            var activator = new ActivatorDomainObjectActivator();
            bo.ApplyConfig(new EventSourceConfiguration(), new EventsourceDIContext(activator,activator),new StateEventMapping(), null);

            return bo;
        }
    }
}