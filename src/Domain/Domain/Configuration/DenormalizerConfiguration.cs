using System;
using System.Reflection;
using BE.CQRS.Domain.Denormalization;

namespace BE.CQRS.Domain.Configuration
{
    public sealed class DenormalizerConfiguration
    {
        public IEventSubscriber Subscriber { get; set; }

        public IStreamPositionGateway StreamPositionGateway { get; set; }

        public EventDenormalizer EventDenormalizer { get; set; }

        public Assembly[] DenormalizerAssemblies { get; set; }

        public Func<Type, object> Factory { get; set; }  // TODO Extract Object Factory like the DomainObjectFacotry
   
    }
}