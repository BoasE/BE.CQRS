using System.IO;
using BE.CQRS.Di.AspCore;
using BE.CQRS.Domain.DomainObjects;
using Microsoft.Extensions.DependencyInjection;

namespace DiTests
{
    public sealed class WithAspActivator : ResolvingDomainObjectTests
    {
        protected override IDomainObjectActivator GetSut()
        {
            var services = new ServiceCollection();
            var provider = services.BuildServiceProvider();

            return new ServiceCollectionActivator(provider);
        }
    }
}