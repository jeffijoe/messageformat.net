// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Ported from Roslyn, see: https://github.com/dotnet/roslyn/blob/main/src/Dependencies/PooledObjects/ObjectPool%601.cs.

using System;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Jeffijoe.MessageFormat;

/// <summary>
/// Generic implementation of object pooling pattern with predefined pool size limit. The main purpose
/// is that limited number of frequently used objects can be kept in the pool for further recycling.
/// </summary>
/// <typeparam name="T">The type of objects to pool.</typeparam>
internal sealed class ObjectPool<T>(Func<T> factory, int size)
    where T : class
{
    private readonly Element[] _items = new Element[size - 1];
    private T? _firstItem;

    public ObjectPool(Func<T> factory)
        : this(factory, Environment.ProcessorCount * 2)
    {
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T Allocate()
    {
        var item = _firstItem;

        if (item is null || item != Interlocked.CompareExchange(ref _firstItem, null, item))
        {
            item = AllocateSlow();
        }

        return item;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Free(T obj)
    {
        if (_firstItem is null)
        {
            _firstItem = obj;
        }
        else
        {
            FreeSlow(obj);
        }
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private T AllocateSlow()
    {
        foreach (ref var element in _items.AsSpan())
        {
            var instance = element.Value;

            if (instance is null)
            {
                continue;
            }

            if (instance == Interlocked.CompareExchange(ref element.Value, null, instance))
            {
                return instance;
            }
        }


        return factory();
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private void FreeSlow(T obj)
    {
        foreach (ref var element in _items.AsSpan())
        {
            if (element.Value is not null)
            {
                continue;
            }

            element.Value = obj;
            break;
        }
    }

    private struct Element
    {
        internal T? Value;
    }
}
