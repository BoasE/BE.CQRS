using System;
using BE.CQRS.Domain.DomainObjects;

namespace BE.CQRS.Data.GetEventStore
{
    public interface IStreamNamer
    {
        string Resolve<T>(T domainObject) where T : class, IDomainObject;

        string Resolve(Type type, string id, string @namespace = null);
    }
}