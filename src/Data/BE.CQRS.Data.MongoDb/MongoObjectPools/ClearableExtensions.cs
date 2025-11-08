using System;
using System.Collections.Generic;
using System.Linq;

namespace BE.CQRS.Data.MongoDb.MongoObjectPools;

internal static class ClearableExtensions<T> where T : class
{
    private static readonly Action<T>[] _clearActions;
    private static readonly Dictionary<Type, Action<object>> _collectionClearers;

    public static void Clear(T obj)
    {
        foreach (var action in _clearActions)
        {
            action(obj);
        }
    }

    static ClearableExtensions()
    {
        var properties = typeof(T).GetProperties()
            .Where(p => p.CanWrite)
            .ToArray();

        _clearActions = new Action<T>[properties.Length];

        for (int i = 0; i < properties.Length; i++)
        {
            var prop = properties[i];
            var type = prop.PropertyType;

            // Für normale Properties: Defaultwert setzen
            var defaultValue = type.IsValueType ? Activator.CreateInstance(type) : null;
            _clearActions[i] = obj => prop.SetValue(obj, defaultValue);
        }
    }
}