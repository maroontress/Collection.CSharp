namespace Maroontress.Collection;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

/// <summary>
/// This class implements the <see cref="IImmutableDictionary{K, V}"/>
/// interface. It has <see cref="ImmutableDictionary{K, V}"/> and <see
/// cref="ImmutableArray{T}"/> to maintain the elements so that it has
/// predictable iteration order, like that of the <c>LinkedHashMap</c>
/// class in Java.
/// </summary>
/// <remarks>
/// The <see cref="ImmutableArray{T}"/> keeps the iteration order corresponding
/// to the <em>insertion order</em> in which you insert key-value pairs into
/// the map. Note that the insertion order is not affected when you re-inserted
/// any key into it (i.e., with the <see cref="SetItem(K, V)"/> method).
/// </remarks>
/// <typeparam name="K">
/// The type of keys maintained by this dictionary.
/// </typeparam>
/// <typeparam name="V">
/// The type of mapped values.
/// </typeparam>
public sealed class ImmutableLinkedHashMap<K, V> : IImmutableDictionary<K, V>
    where K : notnull
{
    /// <summary>
    /// Gets an empty <see cref="ImmutableLinkedHashMap{K, V}"/>.
    /// </summary>
    public static readonly ImmutableLinkedHashMap<K, V> Empty = new();

    private static readonly IEqualityComparer<KeyValuePair<K, V>>
        KeyCompare = new KeyComparer();

    private ImmutableLinkedHashMap()
    {
        List = Enumerable.Empty<KeyValuePair<K, V>>();
        Map = ImmutableDictionary<K, V>.Empty;
    }

    private ImmutableLinkedHashMap(
        IEnumerable<KeyValuePair<K, V>> newList,
        ImmutableDictionary<K, V> newMap)
    {
        List = newList;
        Map = newMap;
    }

    /// <inheritdoc/>
    public IEnumerable<K> Keys => List.Select(p => p.Key);

    /// <inheritdoc/>
    public IEnumerable<V> Values => List.Select(p => p.Value);

    /// <inheritdoc/>
    public int Count => Map.Count;

    private IEnumerable<KeyValuePair<K, V>> List { get; }

    private ImmutableDictionary<K, V> Map { get; }

    /// <inheritdoc/>
    public V this[K key] => Map[key];

    /// <inheritdoc/>
    public IImmutableDictionary<K, V> Add(K key, V value)
    {
        var newMap = Map.Add(key, value);
        if (ReferenceEquals(newMap, Map))
        {
            return this;
        }
        var pair = new KeyValuePair<K, V>(key, value);
        var newList = List.Append(pair);
        return new ImmutableLinkedHashMap<K, V>(newList, newMap);
    }

    /// <inheritdoc/>
    public IImmutableDictionary<K, V> AddRange(
        IEnumerable<KeyValuePair<K, V>> pairs)
    {
        var newMap = Map.AddRange(pairs);
        if (ReferenceEquals(newMap, Map))
        {
            return this;
        }
        var delta = pairs.Where(i => !Map.ContainsKey(i.Key))
            .Distinct(KeyCompare)
            .ToImmutableArray();
        var newList = List.Concat(delta);
        return new ImmutableLinkedHashMap<K, V>(newList, newMap);
    }

    /// <inheritdoc/>
    public IImmutableDictionary<K, V> Clear()
        => Empty;

    /// <inheritdoc/>
    public bool Contains(KeyValuePair<K, V> pair)
    {
        return Map.TryGetValue(pair.Key, out var value)
            && Equals(value, pair.Value);
    }

    /// <inheritdoc/>
    public bool ContainsKey(K key) => Map.ContainsKey(key);

    /// <inheritdoc/>
    public IEnumerator<KeyValuePair<K, V>> GetEnumerator()
        => List.GetEnumerator();

    /// <inheritdoc/>
    public IImmutableDictionary<K, V> Remove(K key)
    {
        var newMap = Map.Remove(key);
        if (ReferenceEquals(newMap, Map))
        {
            return this;
        }
        var newList = List.Where(i => !i.Key.Equals(key))
            .ToImmutableArray();
        return new ImmutableLinkedHashMap<K, V>(newList, newMap);
    }

    /// <inheritdoc/>
    public IImmutableDictionary<K, V> RemoveRange(IEnumerable<K> keys)
    {
        var newMap = Map.RemoveRange(keys);
        if (ReferenceEquals(newMap, Map))
        {
            return this;
        }
        var newList = List.Where(i => newMap.ContainsKey(i.Key))
            .ToImmutableArray();
        return new ImmutableLinkedHashMap<K, V>(newList, newMap);
    }

    /// <inheritdoc/>
    public IImmutableDictionary<K, V> SetItem(K key, V value)
    {
        var newMap = Map.SetItem(key, value);
        if (ReferenceEquals(newMap, Map))
        {
            return this;
        }
        var newPair = new KeyValuePair<K, V>(key, value);
        var newList = Map.ContainsKey(key)
            ? List.Select(i => i.Key.Equals(key) ? newPair : i)
                .ToImmutableArray()
            : List.Append(newPair);
        return new ImmutableLinkedHashMap<K, V>(newList, newMap);
    }

    /// <inheritdoc/>
    public IImmutableDictionary<K, V> SetItems(
        IEnumerable<KeyValuePair<K, V>> items)
    {
        if (items is null)
        {
            throw new ArgumentNullException(nameof(items));
        }
        var newMap = Map.SetItems(items);
        if (ReferenceEquals(newMap, Map))
        {
            return this;
        }
        var deltaList = new List<KeyValuePair<K, V>>();
        var deltaMap = new Dictionary<K, KeyValuePair<K, V>>();
        foreach (var i in items)
        {
            if (!Map.TryGetValue(i.Key, out var currentValue))
            {
                deltaList.Add(i);
                continue;
            }
            if (Equals(i.Value, currentValue))
            {
                continue;
            }
            deltaMap[i.Key] = i;
        }
        var newList = List.Select(
                i => deltaMap.TryGetValue(i.Key, out var v) ? v : i)
            .Concat(deltaList.Distinct(KeyCompare))
            .ToImmutableArray();
        return new ImmutableLinkedHashMap<K, V>(newList, newMap);
    }

    /// <inheritdoc/>
    public bool TryGetKey(K equalKey, out K actualKey)
        => Map.TryGetKey(equalKey, out actualKey);

    /// <inheritdoc/>
    public bool TryGetValue(K key, out V value)
        => Map.TryGetValue(key, out value!);

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();

    private class KeyComparer : IEqualityComparer<KeyValuePair<K, V>>
    {
        public bool Equals(KeyValuePair<K, V> x, KeyValuePair<K, V> y)
        {
            return x.Key.Equals(y.Key);
        }

        public int GetHashCode(KeyValuePair<K, V> o)
        {
            return o.Key.GetHashCode();
        }
    }
}
