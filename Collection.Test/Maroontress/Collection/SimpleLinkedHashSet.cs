namespace Maroontress.Collection;

using System;
using System.Collections;
using System.Collections.Generic;

public sealed class SimpleLinkedHashSet<T> : ISet<T>
    where T : notnull
{
    private Func<ISet<T>> keySet;

    /// <summary>
    /// Initializes a new instance of the <see cref="SimpleLinkedHashSet{T}"/>
    /// class.
    /// </summary>
    /// <param name="initialCapacity">
    /// The initial capacity.
    /// </param>
    public SimpleLinkedHashSet(int initialCapacity)
    {
        Map = new(initialCapacity);
        NewKeySet = () =>
        {
            var set = new HashSet<T>(Map.Keys);
            keySet = () => set;
            return set;
        };
        keySet = NewKeySet;
    }

    /// <inheritdoc/>
    public int Count => List.Count;

    /// <inheritdoc/>
    public bool IsReadOnly => false;

    private LinkedList<T> List { get; } = new();

    private Dictionary<T, LinkedListNode<T>> Map { get; set; }

    private Func<ISet<T>> NewKeySet { get; }

    /// <inheritdoc/>
    public bool Add(T item)
    {
        if (Map.ContainsKey(item))
        {
            return false;
        }
        var listNode = List.AddLast(item);
        Map[item] = listNode;
        keySet = NewKeySet;
        return true;
    }

    /// <inheritdoc/>
    public void Clear()
    {
        Map.Clear();
        List.Clear();
        keySet = NewKeySet;
    }

    /// <inheritdoc/>
    public bool Contains(T item) => Map.ContainsKey(item);

    /// <inheritdoc/>
    public void CopyTo(T[] array, int arrayIndex)
    {
        if (array is null)
        {
            throw new ArgumentNullException(nameof(array));
        }
        if (arrayIndex < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(arrayIndex),
                $"Illegal array index: {arrayIndex}");
        }
        if (arrayIndex > array.Length
            || array.Length < Count
            || arrayIndex > array.Length - Count)
        {
            throw new ArgumentException(
                "Too small array length");
        }
        List.CopyTo(array, arrayIndex);
    }

    /// <inheritdoc/>
    public void ExceptWith(IEnumerable<T> other)
    {
        if (other is null)
        {
            throw new ArgumentNullException(nameof(other));
        }
        foreach (var e in other)
        {
            Remove(e);
        }
        keySet = NewKeySet;
    }

    /// <inheritdoc/>
    public IEnumerator<T> GetEnumerator() => List.GetEnumerator();

    /// <inheritdoc/>
    public void IntersectWith(IEnumerable<T> other)
    {
        if (other is null)
        {
            throw new ArgumentNullException(nameof(other));
        }
        var newMap = new Dictionary<T, LinkedListNode<T>>();
        foreach (var e in other)
        {
            if (!Map.Remove(e, out var node))
            {
                continue;
            }
            newMap[e] = node;
        }
        foreach (var v in Map.Values)
        {
            List.Remove(v);
        }
        Map = newMap;
        keySet = NewKeySet;
    }

    /// <inheritdoc/>
    public bool IsProperSubsetOf(IEnumerable<T> other)
        => keySet().IsProperSubsetOf(other);

    /// <inheritdoc/>
    public bool IsProperSupersetOf(IEnumerable<T> other)
        => keySet().IsProperSupersetOf(other);

    /// <inheritdoc/>
    public bool IsSubsetOf(IEnumerable<T> other)
        => keySet().IsSubsetOf(other);

    /// <inheritdoc/>
    public bool IsSupersetOf(IEnumerable<T> other)
        => keySet().IsSupersetOf(other);

    /// <inheritdoc/>
    public bool Overlaps(IEnumerable<T> other)
        => keySet().Overlaps(other);

    /// <inheritdoc/>
    public bool Remove(T item)
    {
        if (!Map.Remove(item, out var listNode))
        {
            return false;
        }
        List.Remove(listNode);
        keySet = NewKeySet;
        return true;
    }

    /// <inheritdoc/>
    public bool SetEquals(IEnumerable<T> other)
        => keySet().SetEquals(other);

    /// <inheritdoc/>
    public void SymmetricExceptWith(IEnumerable<T> other)
    {
        if (other is null)
        {
            throw new ArgumentNullException(nameof(other));
        }
        if (ReferenceEquals(other, this))
        {
            Clear();
            return;
        }
        var set = new HashSet<T>();
        foreach (var e in other)
        {
            if (!set.Add(e))
            {
                continue;
            }
            if (!Remove(e))
            {
                Add(e);
            }
        }
        keySet = NewKeySet;
    }

    /// <inheritdoc/>
    public void UnionWith(IEnumerable<T> other)
    {
        if (other is null)
        {
            throw new ArgumentNullException(nameof(other));
        }
        foreach (var e in other)
        {
            Add(e);
        }
        keySet = NewKeySet;
    }

    /// <inheritdoc/>
    void ICollection<T>.Add(T item) => Add(item);

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
