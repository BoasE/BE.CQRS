using BE.CQRS.Di.Unity;
using BE.CQRS.Domain.Configuration;
using Microsoft.Practices.Unity;

namespace BE.CQRS.Di.AspCore
{
    public static class EventsourceDiStartup
    {
        public static void SetServiceCollectionActivator(this EventSourceConfiguration config, IUnityContainer container)
        {
            config.Activator = new UnityDomainObjectActivator(container);
        }
    }
}