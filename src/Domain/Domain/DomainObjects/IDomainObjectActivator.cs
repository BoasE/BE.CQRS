using System;

namespace BE.CQRS.Domain.DomainObjects
{
    public interface IDomainObjectActivator
    {
        T Resolve<T>(string id) where T : class, IDomainObject;

        IDomainObject Resolve(Type domainObjectType, string id);
    }
}