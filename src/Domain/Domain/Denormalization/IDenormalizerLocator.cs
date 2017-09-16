using System;
using System.Collections.Generic;
using System.Reflection;

namespace BE.CQRS.Domain.Denormalization
{
    public interface IDenormalizerLocator
    {
        IEnumerable<Type> DenormalizerFromAsm(Assembly asm);
    }
}