using BE.CQRS.Di.Unity;
using BE.CQRS.Domain.DomainObjects;
using Microsoft.Practices.Unity;

namespace DiTests
{
    public sealed class WithUnity : ResolvingDomainObjectTests
    {
        protected override IDomainObjectActivator GetSut()
        {
            var container = new UnityContainer();
            container.RegisterType<ISampleComponent, SampleComponent>();
            return new UnityDomainObjectActivator(container);
        }
    }
}