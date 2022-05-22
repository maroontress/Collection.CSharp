namespace Maroontress.Collection;

using System;
using System.Collections.Concurrent;

/// <summary>
/// The <see cref="InternMap{K, V}"/> class provides the canonical value object
/// corresponding to the specified key.
/// </summary>
/// <typeparam name="K">
/// The type of the key.
/// </typeparam>
/// <typeparam name="V">
/// The type of the value.
/// </typeparam>
public sealed class InternMap<K, V>
    where K : notnull
    where V : class
{
    private const int DefaultCapacity = 31;

    /// <summary>
    /// Initializes a new instance of the <see cref="InternMap{K, V}"/> class.
    /// </summary>
    /// <param name="newValue">
    /// The function that returns a new value object corresponding to the
    /// specified argument.
    /// </param>
    public InternMap(Func<K, V> newValue)
        : this(newValue, DefaultCapacity, DefaultConcurrencyLevel)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="InternMap{K, V}"/> class.
    /// </summary>
    /// <param name="newValue">
    /// The function that returns a new value object corresponding to the
    /// specified argument.
    /// </param>
    /// <param name="initialCapacity">
    /// The initial capacity.
    /// </param>
    public InternMap(Func<K, V> newValue, int initialCapacity)
        : this(newValue, initialCapacity, DefaultConcurrencyLevel)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="InternMap{K, V}"/> class.
    /// </summary>
    /// <param name="newValue">
    /// The function that returns a new value object corresponding to the
    /// specified argument.
    /// </param>
    /// <param name="initialCapacity">
    /// The initial capacity.
    /// </param>
    /// <param name="concurrencyLevel">
    /// The concurrency level.
    /// </param>
    public InternMap(
        Func<K, V> newValue, int initialCapacity, int concurrencyLevel)
    {
        if (newValue is null)
        {
            throw new ArgumentNullException(nameof(newValue));
        }
        Map = new(concurrencyLevel, initialCapacity);
        NewValue = newValue;
    }

    private static int DefaultConcurrencyLevel { get; }
        = Environment.ProcessorCount;

    /// <summary>
    /// Gets the map from a key to the value.
    /// </summary>
    private ConcurrentDictionary<K, V> Map { get; }

    private Func<K, V> NewValue { get; }

    /// <summary>
    /// Gets the canonical value object corresponding to the specified key
    /// object. If the canonical value object does not exist in the internal
    /// object pool, creates a new value object with the function specified
    /// with the constructor.
    /// </summary>
    /// <remarks>
    /// The function <c>newValue</c> specified with the constructor can be
    /// called concurrently with the equal keys if the multiple threads call
    /// this method. Even so, this method returns only one canonical object
    /// corresponding to the specified key.
    /// </remarks>
    /// <param name="key">
    /// The key object.
    /// </param>
    /// <returns>
    /// The canonical value object.
    /// </returns>
    public V Intern(K key)
    {
        return Map.GetOrAdd(key, NewValue);
    }
}
