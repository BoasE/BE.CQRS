using System;
using System.Threading;
using Microsoft.Extensions.ObjectPool;

namespace BE.CQRS.Data.MongoDb.MongoObjectPools;

public interface IRentable
{
    void Clear();
}

internal sealed class MongoDtoPool<T>(ObjectPool<T> pool) : IDisposable
    where T : class, IRentable, new()
{
    private int _count = 0;
    private bool _disposed = false;

    public int Count => _count;
    
    public static MongoDtoPool<T> Instance { get; } = new (
        new DefaultObjectPool<T>(
            new ClearableDtoPolicy<T>()
        ));

    public T Rent()
    {
        Interlocked.Increment(ref _count);
        var dto = pool.Get();
        return dto;
    }

    public T Rent(Action<T> init)
    {
        var dto = Rent();

        init(dto);

        return dto;
    }

    public void Return(T dto)
    {
        Interlocked.Decrement(ref _count);
        dto.Clear();
        pool.Return(dto);
    }

    private void ThrowIfDisposed()
    {
        if (_disposed)
        {
            throw new ObjectDisposedException(nameof(MongoDtoPool<T>));
        }
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            _disposed = true;
            if (pool is IDisposable disposablePool)
            {
                disposablePool.Dispose();
            }

            _disposed = true;
        }
    }
}