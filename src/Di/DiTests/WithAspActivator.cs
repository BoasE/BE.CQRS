using BE.CQRS.Di.AspCore;
using BE.CQRS.Domain.DomainObjects;

namespace DiTests
{
    public sealed class WithAspActivator : ResolvingDomainObjectTests
    {
        protected override IDomainObjectActivator GetSut()
        {
            return new ServiceCollectionActivator();
        }
    }
}