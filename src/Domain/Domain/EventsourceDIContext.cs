using BE.CQRS.Domain.DomainObjects;
using BE.CQRS.Domain.States;
using BE.FluentGuard;

namespace BE.CQRS.Domain
{
    public sealed class EventsourceDIContext
    {
        public IDomainObjectActivator DomainObjectActivator { get;  }

        public IStateActivator StateActivator { get;  }

        public EventsourceDIContext(IDomainObjectActivator domainObjectActivator,IStateActivator stateActivator)
        {
            Precondition.For(domainObjectActivator, nameof(domainObjectActivator)).NotNull();
            Precondition.For(stateActivator, nameof(stateActivator)).NotNull();
            
            DomainObjectActivator = domainObjectActivator;
            StateActivator = stateActivator;
        }
    }
}