namespace Maroontress.Collection;

using System;

/// <summary>
/// This class provides constants for customizing the behavior of a <see
/// cref="LinkedHashSet{T}"/> instance.
/// </summary>
/// <remarks>
/// Do not use this class for anything other than unit testing.
/// </remarks>
public sealed class HashTableConstants
{
    /// <summary>
    /// Initializes a new instance of the <see cref="HashTableConstants"/>
    /// class.
    /// </summary>
    /// <param name="maxCapacity">
    /// The maximum capacity that represents the maximum length of the array.
    /// </param>
    /// <param name="maxSize">
    /// The maximum size.
    /// </param>
    /// <param name="maxRoast">
    /// The maximum roast.
    /// </param>
    /// <exception cref="ArgumentException">
    /// If the <paramref name="maxCapacity"/> or <paramref name="maxSize"/>
    /// is not positive.
    /// </exception>
    public HashTableConstants(int maxCapacity, int maxSize, int maxRoast)
    {
        if (maxCapacity <= 0)
        {
            throw new ArgumentException(
                $"{nameof(maxCapacity)} must be positive");
        }
        if (maxSize <= 0)
        {
            throw new ArgumentException(
                $"{nameof(maxSize)} must be positive");
        }
        if (maxRoast <= 0)
        {
            throw new ArgumentException(
                $"{nameof(maxRoast)} must be positive");
        }
        MaxCapacity = maxCapacity;
        MaxSize = maxSize;
        MaxRoast = maxRoast;
    }

    /// <summary>
    /// Gets the maximum capacity.
    /// </summary>
    public int MaxCapacity { get; }

    /// <summary>
    /// Gets the maximum size.
    /// </summary>
    public int MaxSize { get; }

    /// <summary>
    /// Gets the maximum roast.
    /// </summary>
    public int MaxRoast { get; }

    /// <summary>
    /// Gets the initial capacity which is the smallest power-of-two number
    /// equal to or greater then the specified value. If the value is greater
    /// than <see cref="MaxCapacity"/>, returns <see cref="MaxCapacity"/>.
    /// </summary>
    /// <param name="n">
    /// The preferred capacity.
    /// </param>
    /// <returns>
    /// The initial capacity.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// If the <paramref name="n"/> is not positive.
    /// </exception>
    public int GetCapacity(int n)
    {
        if (n <= 0)
        {
            throw new ArgumentException($"{nameof(n)} must be positive");
        }
        if (n >= MaxCapacity)
        {
            return MaxCapacity;
        }
        var m = n;
        m |= m >> 1;
        m |= m >> 2;
        m |= m >> 4;
        m |= m >> 8;
        m |= m >> 16;
        m -= m >> 1;
        return (m == n) ? m : m << 1;
    }
}
