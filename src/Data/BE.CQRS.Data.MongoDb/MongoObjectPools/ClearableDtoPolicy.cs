using System;
using Microsoft.Extensions.ObjectPool;

namespace BE.CQRS.Data.MongoDb.MongoObjectPools;

internal sealed class ClearableDtoPolicy<T> : IPooledObjectPolicy<T>
    where T : class, IRentable, new()
{
    private readonly Func<T> factory;

    public ClearableDtoPolicy()
    {
        factory = () =>
        {
            var result = new T();

            result.Clear();
            return result;
        };
    }

    public ClearableDtoPolicy(Func<T> objectFactory)
    {

        factory = () =>
        {
            var result = objectFactory();

            result.Clear();
            return result;
        };
    }

    public T Create() => factory();

    public bool Return(T? obj)
    {
        if (obj == null) return false;

        try
        {
            obj.Clear();
            return true;
        }
        catch
        {
            return false;
        }
    }
}