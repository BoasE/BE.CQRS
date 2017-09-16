using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BE.CQRS.Domain.Conventions;

namespace BE.CQRS.Domain.Denormalization
{
    public sealed class DenormalizerLocator : IDenormalizerLocator
    {
        public IEnumerable<Type> DenormalizerFromAsm(Assembly asm)
        {
            return asm.ExportedTypes.Where(IsAccessableDenormalizer);
        }

        private static bool IsAccessableDenormalizer(Type type)
        {
            TypeInfo nfo = type.GetTypeInfo();

            return nfo.IsClass && !nfo.IsAbstract && nfo.GetCustomAttribute<DenormalizerAttribute>() != null;
        }
    }
}