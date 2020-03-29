using BE.CQRS.Domain.Configuration;
using BE.CQRS.Domain.DomainObjects;
using BE.CQRS.Domain.States;

namespace BE.CQRS.Domain.Tests.DomainObjectStateRuntimeTests
{
    public class GivenRuntime
    {
        protected static DomainObjectStateRuntime GetSut()
        {
            var domainObject = new TestDomainObject("1");

            domainObject.ApplyEvent(new TestEvent());

            var activator = new ActivatorDomainObjectActivator();
            return new DomainObjectStateRuntime(
                domainObject, new EventsourceDIContext(activator, activator), new StateEventMapping(),
                new EventSourceConfiguration());
        }
    }
}