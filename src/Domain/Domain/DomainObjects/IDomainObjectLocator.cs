using System;
using System.Collections.Generic;
using System.Reflection;
using BE.CQRS.Domain.Commands;

namespace BE.CQRS.Domain.DomainObjects
{
    public interface IDomainObjectLocator
    {
        IEnumerable<Type> ResolveDomainObjects(IList<Assembly> source);

        IEnumerable<CommandMethodMapping> ResolveConventionalMethods(Type domainObjectType);
    }
}