using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BE.CQRS.Domain.Conventions;

namespace BE.CQRS.Domain.Denormalization
{
    public sealed class DenormalizerLocator : IDenormalizerLocator
    {
        public IEnumerable<Denormalizer> DenormalizerFromAsm(Assembly asm)
        {
            var types = asm.ExportedTypes.Where(IsAccessableDenormalizer);

            return types.Select(x =>
            {
                var background = x.GetCustomAttribute<DenormalizerAttribute>()!.IsBackground;
                return new Denormalizer(x, background);
            });
        }

        private static bool IsAccessableDenormalizer(Type type)
        {
            TypeInfo nfo = type.GetTypeInfo();

            return nfo.IsClass && !nfo.IsAbstract && nfo.GetCustomAttribute<DenormalizerAttribute>() != null;
        }
    }
}