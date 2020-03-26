using System.Reflection;
using BE.CQRS.Domain.Denormalization;

namespace BE.CQRS.Domain.Configuration
{
    public sealed class DenormalizerConfiguration
    { 
        public Assembly[] DenormalizerAssemblies { get; set; }
    }
}