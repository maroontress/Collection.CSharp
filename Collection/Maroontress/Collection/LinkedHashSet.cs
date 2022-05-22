namespace Maroontress.Collection;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// The <see cref="LinkedHashSet{T}"/> class implements the <see
/// cref="ISet{T}"/> interface. Its instance has both the hash table and the
/// doubly-linked list to maintain the elements so that it has predictable
/// iteration order, like that of the <c>LinkedHashSet</c> class in Java.
/// </summary>
/// <remarks>
/// <para>
/// The linked list keeps the iteration order corresponding to the <i>insertion
/// order</i> in which you insert elements into the set. Note that the
/// insertion order is not affected when you re-inserted any into it (i.e., the
/// <see cref="Add(T)"/> method returned <c>false</c>).
/// </para>
/// <para>
/// The hash table uses
/// <a href="https://en.wikipedia.org/wiki/Hash_table#Separate_chaining">
/// Separate Chaining</a> for collision resolution and has the <i>capacity</i>,
/// <i>size</i>, and <i>load factor</i>. The capacity represents the number of
/// entries in the hash table. In this implementation, it is a power of two.
/// The size is the number of elements that the hash table contains. The set
/// rehashes the hash table when the size divided by the capacity is close to
/// the load factor.Rehashing makes the capacity of the hash table double.
/// </para>
/// <para>
/// To check whether two sets are equal, with iteration order taken into
/// account, use the
/// <see cref="Enumerable.SequenceEqual{T}(IEnumerable{T}, IEnumerable{T})"/>
/// method as follows:
/// </para>
/// <pre>
/// public static bool AreEqualAndHaveSameInsertionOrder&lt;T&gt;(
///     LinkedHashSet&lt;T&gt; s1, LinkedHashSet&lt;T&gt; s2)
/// {
///     return s1.SequenceEqual(s2);
/// }</pre>
/// <para>
/// Note that the <see cref="SetEquals(IEnumerable{T})"/> and <see
/// cref="SetEquals(LinkedHashSet{T})"/> methods ignore the iteration order and
/// return set equality; the <see cref="object.Equals(object)"/> method returns
/// reference equality.
/// </para>
/// <para>
/// The minimum and maximum capacities are <see cref="DefaultInitialCapacity"/>
/// (<c>16</c>) and <see cref="DefaultMaxCapacity"/> (<c>0x40000000</c>),
/// respectively.
/// </para>
/// <para>
/// As mentioned, if the number of elements in the set exceeds the product of
/// the capacity and the load factor, it rehashes its hash table with the
/// capacity updated to double. However, this implementation restricts the
/// maximum capacity to <see cref="DefaultMaxCapacity"/> (<c>0x40000000</c>).
/// So, once the capacity reaches its maximum, rehashing will no longer occur.
/// Note that since the implementation uses Separate Chaining, it is possible
/// to add up to <see cref="int.MaxValue"/> (<c>0x7fffffff</c>) elements to the
/// set even after the capacity reaches its maximumunless it throws an <see
/// cref="OutOfMemoryException"/>.
/// </para>
/// <para>
/// If the number of elements in the set reaches the maximum value (<see
/// cref="int.MaxValue"/>), any attempt to add elements throws an <see
/// cref="InsufficientMemoryException"/> (e.g., with the <see cref="Add(T)"/>
/// method).
/// </para>
/// </remarks>
/// <typeparam name="T">
/// The type of elements maintained by this set.
/// </typeparam>
public class LinkedHashSet<T> : ISet<T>
    where T : notnull
{
    /// <summary>
    /// The default maximum capacity.
    /// </summary>
    public const int DefaultMaxCapacity = 0x4000_0000;

    /// <summary>
    /// The default maximum size.
    /// </summary>
    public const int DefaultMaxSize = int.MaxValue;

    /// <summary>
    /// The default initial capacity. This is also the minimum capacity.
    /// </summary>
    public const int DefaultInitialCapacity = 16;

    /// <summary>
    /// The default load factor.
    /// </summary>
    public const float DefaultLoadFactor = 0.75f;

    private static readonly HashTableConstants DefaultHashConstants
        = new(DefaultMaxCapacity, DefaultMaxSize, int.MaxValue);

    /// <summary>
    /// Initializes a new instance of the <see cref="LinkedHashSet{T}"/>
    /// class.
    /// </summary>
    /// <param name="initialCapacity">
    /// The initial capacity.
    /// </param>
    /// <param name="loadFactor">
    /// The load factor.
    /// </param>
    /// <remarks>
    /// The capacity is the smallest power-of-two number equal to or greater
    /// than the specified <paramref name="initialCapacity"/>.
    /// </remarks>
    /// <exception cref="ArgumentException">
    /// If the initial capacity is negative or the load factor is nonpositive.
    /// </exception>
    public LinkedHashSet(
            int initialCapacity = DefaultInitialCapacity,
            float loadFactor = DefaultLoadFactor)
        : this(DefaultHashConstants, initialCapacity, loadFactor)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LinkedHashSet{T}"/> class.
    /// </summary>
    /// <param name="constants">
    /// The constants for the hash table.
    /// </param>
    /// <param name="initialCapacity">
    /// The initial capacity.
    /// </param>
    /// <param name="loadFactor">
    /// The load factor.
    /// </param>
    /// <exception cref="ArgumentException">
    /// If the <paramref name="initialCapacity"/> is negative or the <paramref
    /// name="loadFactor"/> is nonpositive.
    /// </exception>
    /// <exception cref="ArgumentNullException">
    /// If the <paramref name="constants"/> is <c>null</c>.
    /// </exception>
    protected LinkedHashSet(
        HashTableConstants constants, int initialCapacity, float loadFactor)
    {
        if (constants is null)
        {
            throw new ArgumentNullException(nameof(constants));
        }
        if (initialCapacity < 0)
        {
            throw new ArgumentException(
                $"Illegal initial capacity: {initialCapacity}");
        }
        if (loadFactor <= 0 || float.IsNaN(loadFactor))
        {
            throw new ArgumentException(
                $"Illegal load factor: {loadFactor}");
        }
        var rawCapacity = Math.Max(initialCapacity, DefaultInitialCapacity);
        var size = constants.GetCapacity(rawCapacity);
        LoadFactor = Math.Max(Math.Min(loadFactor, 1.0f), 0.125f);
        Constants = constants;
        Limit = GetLimit(size);
        Nodes = new Node[size];
        IndexMask = size - 1;
        Head = null;
        Tail = null;
        Size = 0;
    }

    /// <summary>
    /// The read-only node in the hash table and the doubly-linked list.
    /// </summary>
    /// <remarks>
    /// Do not use this class for anything other than unit testing.
    /// </remarks>
    protected interface ProtectedNode
    {
        /// <summary>
        /// Gets the value that this node has.
        /// </summary>
        public T Value { get; }

        /// <summary>
        /// Gets the hash code.
        /// </summary>
        public int Hash { get; }

        /// <summary>
        /// Gets the roast.
        /// </summary>
        public int Roast { get; }

        /// <summary>
        /// Gets the parent node in Separate Chaining.
        /// </summary>
        public ProtectedNode? ParentNode { get; }

        /// <summary>
        /// Gets the previous node in the doubly-linked node.
        /// </summary>
        public ProtectedNode? PreviousNode { get; }

        /// <summary>
        /// Gets the next node in the doubly-linked node.
        /// </summary>
        public ProtectedNode? NextNode { get; }
    }

    /// <summary>
    /// Gets the load factor.
    /// </summary>
    public float LoadFactor { get; }

    /// <inheritdoc/>
    public int Count => Size;

    /// <inheritdoc/>
    public bool IsReadOnly => false;

    /// <summary>
    /// Gets the capacity and limit.
    /// </summary>
    /// <returns>
    /// The tuple containing the capacity and the limit.
    /// </returns>
    protected (int Capacity, int Limit) CapacityAndLimit
    {
        get => (Nodes.Length, Limit);
    }

    /// <summary>
    /// Gets the first and last nodes, or <c>null</c> when this set is empty.
    /// </summary>
    /// <returns>
    /// The tuple containing the first and last nodes.
    /// </returns>
    protected (ProtectedNode? First, ProtectedNode? Last) FirstAndLastNode
    {
        get => (Head, Tail);
    }

    private static Func<int, int, bool> LessThan { get; }
        = (i, j) => i < j;

    private static Func<int, int, bool> LessThanOrEqual { get; }
        = (i, j) => i <= j;

    private static Func<int, int, bool> Equal { get; }
        = (i, j) => i == j;

    private HashTableConstants Constants { get; }

    private int Limit { get; set; }

    private Node?[] Nodes { get; set; }

    private Node? Head { get; set; }

    private Node? Tail { get; set; }

    private int Size { get; set; }

    private int IndexMask { get; set; }

    /// <inheritdoc/>
    public bool Add(T item)
    {
        if (Append(item, item.GetHashCode()))
        {
            ++Size;
            Extend();
            return true;
        }
        return false;
    }

    /// <inheritdoc/>
    public void Clear()
    {
        /*
            var e = Head;
            while (e is not null)
            {
                var i = ToIndex(e.Hash, Nodes.Length);
                Nodes[i] = null;
                e = e.Next;
            }
        */
        Array.Clear(Nodes, 0, Nodes.Length);
        Size = 0;
        Head = null;
        Tail = null;
    }

    /// <inheritdoc/>
    public bool Contains(T item)
        => Contains(item, item.GetHashCode());

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
            || array.Length < Size
            || arrayIndex > array.Length - Size)
        {
            throw new ArgumentException(
                "Too small array length");
        }
        var i = arrayIndex;
        for (var e = Head;  e is not null; e = e.Next)
        {
            array[i] = e.Value;
            ++i;
        }
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
    }

    /// <inheritdoc/>
    public IEnumerator<T> GetEnumerator()
    {
        var e = Head;
        while (e is not null)
        {
            var next = e.Next;
            yield return e.Value;
            e = next;
        }
    }

    /// <inheritdoc/>
    public void IntersectWith(IEnumerable<T> other)
    {
        if (other is null)
        {
            throw new ArgumentNullException(nameof(other));
        }
        if (ReferenceEquals(other, this))
        {
            return;
        }
        if (Size is 0)
        {
            Clear();
            return;
        }
        var newSize = 0;
        var newRoast = GetNewRoast();
        foreach (var item in other)
        {
            var hash = item.GetHashCode();
            if (!(FindNode(item, hash) is {} node))
            {
                continue;
            }
            node.Roast = newRoast;
        }
        for (var e = Head; e is not null; e = e.Next)
        {
            if (e.Roast != newRoast)
            {
                DeleteNode(e);
                continue;
            }
            ++newSize;
        }
        Size = newSize;
    }

    /// <inheritdoc/>
    public bool IsProperSubsetOf(IEnumerable<T> other)
    {
        if (other is null)
        {
            throw new ArgumentNullException(nameof(other));
        }
        return !ReferenceEquals(other, this)
            && ProperSubset(other);
    }

    /// <inheritdoc/>
    public bool IsProperSupersetOf(IEnumerable<T> other)
    {
        if (other is null)
        {
            throw new ArgumentNullException(nameof(other));
        }
        return !ReferenceEquals(other, this)
            && Size is not 0
            && IsSupersetOf(other, LessThan);
    }

    /// <inheritdoc/>
    public bool IsSubsetOf(IEnumerable<T> other)
    {
        if (other is null)
        {
            throw new ArgumentNullException(nameof(other));
        }
        return ReferenceEquals(other, this)
            || Subset(other);
    }

    /// <inheritdoc/>
    public bool IsSupersetOf(IEnumerable<T> other)
    {
        if (other is null)
        {
            throw new ArgumentNullException(nameof(other));
        }
        return EqualToOrSupersetOf(other, LessThanOrEqual);
    }

    /// <inheritdoc/>
    public bool Overlaps(IEnumerable<T> other)
    {
        if (other is null)
        {
            throw new ArgumentNullException(nameof(other));
        }
        return Size is not 0
            && (ReferenceEquals(other, this)
                || other.Any(Contains));
    }

    /// <inheritdoc/>
    public bool Remove(T item)
    {
        var hash = item.GetHashCode();
        if (!(TakeNode(item, hash) is {} node))
        {
            return false;
        }
        Uncouple(node);
        --Size;
        return true;
    }

    /// <inheritdoc/>
    public bool SetEquals(IEnumerable<T> other)
    {
        if (other is null)
        {
            throw new ArgumentNullException(nameof(other));
        }
        return EqualToOrSupersetOf(other, Equal);
    }

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
        if (Size is 0)
        {
            UnionWith(other);
            return;
        }
        var removedList = new OneWayList();
        var addedList = new OneWayList();
        var newRoast = GetNewRoast();
        try
        {
            foreach (var item in other)
            {
                var lastTail = addedList.Tail;
                if (!(FindOrAttachNode(item, addedList) is {} node))
                {
                    var newTail = addedList.Tail;
                    newTail!.Previous = lastTail;
                    newTail!.Roast = newRoast;
                    ++Size;
                    if (Size > Limit)
                    {
                        DoubleCapacity();
                        Reparent(addedList.Head);
                        Reparent(removedList.Head);
                    }
                    continue;
                }
                if (node.Roast == newRoast)
                {
                    continue;
                }
                --Size;
                Uncouple(node);
                removedList.Append(node);
                node.Roast = newRoast;
            }
        }
        finally
        {
            if (removedList.Tail is {} tail)
            {
                tail.Next = null;
                for (var e = removedList.Head; e is not null; e = e.Next)
                {
                    DetachNode(e);
                }
            }
            var head = addedList.Head;
            if (Tail is not null)
            {
                Tail.Next = head;
            }
            else
            {
                Head = head;
            }
            if (head is not null)
            {
                head.Previous = Tail;
                Tail = addedList.Tail;
            }
            if (Head is not null)
            {
                Head.Roast = newRoast;
            }
        }
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
    }

    /// <inheritdoc/>
    void ICollection<T>.Add(T item) => Add(item);

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <summary>
    /// Modifies the current <see cref="LinkedHashSet{T}"/> object to contain
    /// only elements that are present in that object and in the specified
    /// <see cref="LinkedHashSet{T}"/>.
    /// </summary>
    /// <param name="otherSet">
    /// The set to compare to the current <see cref="LinkedHashSet{T}"/>
    /// object.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// If the <paramref name="otherSet"/> is <c>null</c>.
    /// </exception>
    public void IntersectWith(LinkedHashSet<T> otherSet)
    {
        if (otherSet is null)
        {
            throw new ArgumentNullException(nameof(otherSet));
        }
        if (ReferenceEquals(otherSet, this))
        {
            return;
        }
        if (Size is 0 || otherSet.Size is 0)
        {
            Clear();
            return;
        }
        var count = 0;
        for (var e = Head; e is not null; e = e.Next)
        {
            if (otherSet.Contains(e.Value, e.Hash))
            {
                continue;
            }
            DeleteNode(e);
            ++count;
        }
        Size -= count;
    }

    /// <summary>
    /// Modifies the current <see cref="LinkedHashSet{T}"/> object to contain
    /// all elements that are present in itself, the specified set, or both.
    /// </summary>
    /// <param name="otherSet">
    /// The collection to compare to the current <see cref="LinkedHashSet{T}"/>
    /// object.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// If the <paramref name="otherSet"/> is <c>null</c>.
    /// </exception>
    public void UnionWith(LinkedHashSet<T> otherSet)
    {
        if (otherSet is null)
        {
            throw new ArgumentNullException(nameof(otherSet));
        }
        if (Size is not 0)
        {
            for (var e = otherSet.Head; e is not null; e = e.Next)
            {
                if (Append(e.Value, e.Hash))
                {
                    ++Size;
                    Extend();
                }
            }
            return;
        }
        var s = otherSet.Size;
        if (s > Limit)
        {
            var m = (double)Constants.MaxSize;
            var n = (int)Math.Min((double)s / LoadFactor, m);
            var nextLength = Constants.GetCapacity(n);
            Nodes = new Node[nextLength];
            IndexMask = nextLength - 1;
            Limit = GetLimit(nextLength);
        }
        for (var e = otherSet.Head; e is not null; e = e.Next)
        {
            var i = IndexMap(e.Hash);
            ref var slot = ref Nodes[i];
            var root = slot;
            var node = Node.Of(e.Value, e.Hash, root);
            AppendNode(node);
            slot = node;
        }
        Size = s;
    }

    /// <summary>
    /// Modifies the current <see cref="LinkedHashSet{T}"/> object to contain
    /// only elements that are present either in that object or in the
    /// specified set, but not both.
    /// </summary>
    /// <param name="otherSet">
    /// The set to compare to the current <see cref="LinkedHashSet{T}"/>
    /// object.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// If the <paramref name="otherSet"/> is <c>null</c>.
    /// </exception>
    public void SymmetricExceptWith(LinkedHashSet<T> otherSet)
    {
        if (otherSet is null)
        {
            throw new ArgumentNullException(nameof(otherSet));
        }
        if (ReferenceEquals(otherSet, this))
        {
            Clear();
            return;
        }
        if (Size is 0)
        {
            UnionWith(otherSet);
            return;
        }
        for (var e = otherSet.Head; e is not null; e = e.Next)
        {
            var v = e.Value;
            var hash = e.Hash;
            if (DeleteOrAppendNode(v, hash) is {} node)
            {
                --Size;
                continue;
            }
            ++Size;
        }
    }

    /// <summary>
    /// Determines whether a <see cref="LinkedHashSet{T}"/> object and the
    /// specified set contain the same elements.
    /// </summary>
    /// <param name="otherSet">
    /// The collection to compare to the current <see cref="LinkedHashSet{T}"/>
    /// object.
    /// </param>
    /// <returns>
    /// <c>true</c> if the <see cref="LinkedHashSet{T}"/> object is equal to
    /// other; otherwise, <c>false</c>.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// If the <paramref name="otherSet"/> is <c>null</c>.
    /// </exception>
    public bool SetEquals(LinkedHashSet<T> otherSet)
    {
        if (otherSet is null)
        {
            throw new ArgumentNullException(nameof(otherSet));
        }
        return ReferenceEquals(otherSet, this)
            || (otherSet.Size == Size && ContainsAll(otherSet));
    }

    /// <summary>
    /// Determines whether the current <see cref="LinkedHashSet{T}"/> object
    /// and a specified set share common elements.
    /// </summary>
    /// <param name="otherSet">
    /// The set to compare to the current <see cref="LinkedHashSet{T}"/>
    /// object.
    /// </param>
    /// <returns>
    /// <c>true</c> if the <see cref="LinkedHashSet{T}"/> object and
    /// <paramref name="otherSet"/> share at least one common element;
    /// otherwise, <c>false</c>.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// If the <paramref name="otherSet"/> is <c>null</c>.
    /// </exception>
    public bool Overlaps(LinkedHashSet<T> otherSet)
    {
        if (otherSet is null)
        {
            throw new ArgumentNullException(nameof(otherSet));
        }
        if (Size is 0 || otherSet.Size is 0)
        {
            return false;
        }
        if (ReferenceEquals(otherSet, this))
        {
            return true;
        }
        var (e, s) = (Size < otherSet.Size)
            ? (Head, otherSet)
            : (otherSet.Head, this);
        for (; e is not null; e = e.Next)
        {
            if (s.Contains(e.Value, e.Hash))
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Determines whether a <see cref="LinkedHashSet{T}"/> object is a
    /// subset of the specified set.
    /// </summary>
    /// <param name="otherSet">
    /// The collection to compare to the current <see
    /// cref="LinkedHashSet{T}"/> object.
    /// </param>
    /// <returns>
    /// <c>true</c> if the <see cref="LinkedHashSet{T}"/> object is a subset
    /// of <paramref name="otherSet"/>; otherwise, <c>false</c>.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// If the <paramref name="otherSet"/> is <c>null</c>.
    /// </exception>
    public bool IsSubsetOf(LinkedHashSet<T> otherSet)
    {
        if (otherSet is null)
        {
            throw new ArgumentNullException(nameof(otherSet));
        }
        return Size is 0
            || ReferenceEquals(otherSet, this)
            || (otherSet.Size >= Size && otherSet.ContainsAll(this));
    }

    /// <summary>
    /// Determines whether a <see cref="LinkedHashSet{T}"/> object is a
    /// superset of the specified set.
    /// </summary>
    /// <param name="otherSet">
    /// The set to compare to the current <see cref="LinkedHashSet{T}"/>
    /// object.
    /// </param>
    /// <returns>
    /// <c>true</c> if the <see cref="LinkedHashSet{T}"/> object is a
    /// superset of <paramref name="otherSet"/>; otherwise, <c>false</c>.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// If the <paramref name="otherSet"/> is <c>null</c>.
    /// </exception>
    public bool IsSupersetOf(LinkedHashSet<T> otherSet)
    {
        if (otherSet is null)
        {
            throw new ArgumentNullException(nameof(otherSet));
        }
        return otherSet.Size is 0
            || ReferenceEquals(otherSet, this)
            || (otherSet.Size <= Size && ContainsAll(otherSet));
    }

    /// <summary>
    /// Determines whether a <see cref="LinkedHashSet{T}"/> object is a
    /// proper subset of the specified set.
    /// </summary>
    /// <param name="otherSet">
    /// The set to compare to the current <see cref="LinkedHashSet{T}"/>
    /// object.
    /// </param>
    /// <returns>
    /// <c>true</c> if the <see cref="LinkedHashSet{T}"/> object is a proper
    /// subset of <paramref name="otherSet"/>; otherwise, <c>false</c>.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// If the <paramref name="otherSet"/> is <c>null</c>.
    /// </exception>
    public bool IsProperSubsetOf(LinkedHashSet<T> otherSet)
    {
        if (otherSet is null)
        {
            throw new ArgumentNullException(nameof(otherSet));
        }
        return (Size is 0 && otherSet.Size > 0)
            || (!ReferenceEquals(otherSet, this)
                && otherSet.Size > Size
                && otherSet.ContainsAll(this));
    }

    /// <summary>
    /// Determines whether a <see cref="LinkedHashSet{T}"/> object is a
    /// proper superset of the specified set.
    /// </summary>
    /// <param name="otherSet">
    /// The set to compare to the current <see cref="LinkedHashSet{T}"/>
    /// object.
    /// </param>
    /// <returns>
    /// <c>true</c> if the <see cref="LinkedHashSet{T}"/> object is a proper
    /// superset of <paramref name="otherSet"/>; otherwise, <c>false</c>.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// If the <paramref name="otherSet"/> is <c>null</c>.
    /// </exception>
    public bool IsProperSupersetOf(LinkedHashSet<T> otherSet)
    {
        if (otherSet is null)
        {
            throw new ArgumentNullException(nameof(otherSet));
        }
        return (otherSet.Size is 0 && Size > 0)
            || (!ReferenceEquals(otherSet, this)
                && otherSet.Size < Size
                && ContainsAll(otherSet));
    }

    /// <summary>
    /// Gets the backend array of the hash table.
    /// </summary>
    /// <returns>
    /// The backend array.
    /// </returns>
    protected IEnumerable<ProtectedNode?> GetNodes()
        => Nodes.AsEnumerable();

    private int GetNewRoast()
    {
        var roast = Head!.Roast;
        if (roast < Constants.MaxRoast)
        {
            return roast + 1;
        }
        for (var e = Head; e is not null; e = e.Next)
        {
            e.Roast = 0;
        }
        return 1;
    }

    private int IndexMap(int hash)
    {
        return hash & IndexMask;
    }

    private bool ProperSubset(IEnumerable<T> other)
    {
        if (Head is null)
        {
            return other.Any();
        }
        var newRoast = GetNewRoast();
        try
        {
            var includeSize = 0;
            var excludes = false;
            var containsAll = false;
            foreach (var i in other)
            {
                var hash = i.GetHashCode();
                if (!(FindNode(i, hash) is {} node))
                {
                    excludes = true;
                    if (containsAll)
                    {
                        return true;
                    }
                    continue;
                }
                if (node.Roast == newRoast)
                {
                    continue;
                }
                node.Roast = newRoast;
                ++includeSize;
                if (includeSize < Size)
                {
                    continue;
                }
                containsAll = true;
                if (excludes)
                {
                    return true;
                }
            }
            return false;
        }
        finally
        {
            Head.Roast = newRoast;
        }
    }

    private bool Subset(IEnumerable<T> other)
    {
        if (Head is null)
        {
            return true;
        }
        var newRoast = GetNewRoast();
        try
        {
            var includeSize = 0;
            foreach (var i in other)
            {
                var hash = i.GetHashCode();
                if (!(FindNode(i, hash) is {} node))
                {
                    continue;
                }
                if (node.Roast == newRoast)
                {
                    continue;
                }
                node.Roast = newRoast;
                ++includeSize;
                if (includeSize < Size)
                {
                    continue;
                }
                return true;
            }
            return false;
        }
        finally
        {
            Head.Roast = newRoast;
        }
    }

    private bool EqualToOrSupersetOf(
        IEnumerable<T> other, Func<int, int, bool> compare)
    {
        return ReferenceEquals(other, this)
            || ((Size is 0)
                ? !other.Any()
                : IsSupersetOf(other, compare));
    }

    private bool IsSupersetOf(
        IEnumerable<T> other, Func<int, int, bool> compare)
    {
        var newRoast = GetNewRoast();
        var size = 0;
        foreach (var i in other)
        {
            var hash = i.GetHashCode();
            if (!(FindNode(i, hash) is {} node))
            {
                return false;
            }
            if (node.Roast == newRoast)
            {
                continue;
            }
            node.Roast = newRoast;
            ++size;
        }
        return compare(size, Size);
    }

    private bool ContainsAll(LinkedHashSet<T> s)
    {
        for (var e = s.Head; e is not null; e = e.Next)
        {
            if (!Contains(e.Value, e.Hash))
            {
                return false;
            }
        }
        return true;
    }

    private bool Contains(T item, int hash)
    {
        var i = IndexMap(hash);
        for (var e = Nodes[i]; e is not null; e = e.Parent)
        {
            if (e.Hash == hash && e.Value.Equals(item))
            {
                return true;
            }
        }
        return false;
    }

    private void Uncouple(Node e)
    {
        var previous = e.Previous;
        var next = e.Next;
        if (previous is null)
        {
            Head = next;
        }
        else
        {
            previous.Next = next;
        }
        if (next is null)
        {
            Tail = previous;
        }
        else
        {
            next.Previous = previous;
        }
    }

    private void DetachNode(Node node)
    {
        var hash = node.Hash;
        var i = IndexMap(hash);
        ref var slot = ref Nodes[i];
        var root = slot;
        if (ReferenceEquals(root, node))
        {
            slot = node.Parent;
            return;
        }
        var child = root;
        var e = root!.Parent;
        for (;;)
        {
            var parent = e!.Parent;
            if (ReferenceEquals(e, node))
            {
                child!.Parent = parent;
                return;
            }
            child = e;
            e = parent;
        }
    }

    private void DeleteNode(Node node)
    {
        DetachNode(node);
        Uncouple(node);
    }

    private Node? TakeNode(T item, int hash)
    {
        var i = IndexMap(hash);
        ref var slot = ref Nodes[i];
        var root = slot;
        if (root is null)
        {
            return null;
        }
        if (root.Hash == hash && root.Value.Equals(item))
        {
            slot = root.Parent;
            return root;
        }
        var child = root;
        var e = root.Parent;
        while (e is not null)
        {
            var parent = e.Parent;
            if (e.Hash == hash && e.Value.Equals(item))
            {
                child.Parent = parent;
                return e;
            }
            child = e;
            e = parent;
        }
        return null;
    }

    private Node? FindNode(T item, int hash)
    {
        var i = IndexMap(hash);
        for (var e = Nodes[i]; e is not null; e = e.Parent)
        {
            if (e.Hash == hash && e.Value.Equals(item))
            {
                return e;
            }
        }
        return null;
    }

    private Node? DeleteOrAppendNode(T item, int hash)
    {
        var i = IndexMap(hash);
        ref var slot = ref Nodes[i];
        var root = slot;
        if (root is not null)
        {
            if (root.Hash == hash && root.Value.Equals(item))
            {
                slot = root.Parent;
                Uncouple(root);
                return root;
            }
            var child = root;
            var e = root.Parent;
            while (e is not null)
            {
                var parent = e.Parent;
                if (e.Hash == hash && e.Value.Equals(item))
                {
                    child.Parent = parent;
                    Uncouple(e);
                    return e;
                }
                child = e;
                e = parent;
            }
        }
        if (Size == Constants.MaxSize)
        {
            throw new InsufficientMemoryException();
        }
        var node = Node.Of(item, hash, root);
        AppendNode(node);
        slot = node;
        return null;
    }

    private void AppendNode(Node node)
    {
        if (Tail is null)
        {
            Head = node;
        }
        else
        {
            Tail.Next = node;
            node.Previous = Tail;
        }
        Tail = node;
    }

    private Node? FindOrAttachNode(T item, OneWayList nodeList)
    {
        var hash = item.GetHashCode();
        var i = IndexMap(hash);
        ref var slot = ref Nodes[i];
        var root = slot;
        for (var e = root; e is not null; e = e.Parent)
        {
            if (e.Hash == hash && e.Value.Equals(item))
            {
                return e;
            }
        }
        if (Size == Constants.MaxSize)
        {
            throw new InsufficientMemoryException();
        }
        var node = Node.Of(item, hash, root);
        slot = node;
        nodeList.Append(node);
        return null;
    }

    private bool Append(T item, int hash)
    {
        var i = IndexMap(hash);
        ref var slot = ref Nodes[i];
        var root = slot;
        for (var e = root; e is not null; e = e.Parent)
        {
            if (e.Hash == hash && e.Value.Equals(item))
            {
                return false;
            }
        }
        if (Size == Constants.MaxSize)
        {
            throw new InsufficientMemoryException();
        }
        var node = Node.Of(item, hash, root);
        AppendNode(node);
        slot = node;
        return true;
    }

    private void Extend()
    {
        if (Size <= Limit)
        {
            return;
        }
        DoubleCapacity();
    }

    private void DoubleCapacity()
    {
        var length = Nodes.Length;
        var nextLength = (length > Constants.MaxCapacity / 2)
            ? Constants.MaxCapacity
            : length * 2;
        Nodes = new Node[nextLength];
        IndexMask = nextLength - 1;
        Reparent(Head);
        Limit = (nextLength == Constants.MaxCapacity)
            ? Constants.MaxSize
            : GetLimit(nextLength);
    }

    private int GetLimit(int length)
    {
        var limit = (double)length * LoadFactor;
        return (limit >= Constants.MaxSize)
            ? Constants.MaxSize
            : (int)limit;
    }

    private void Reparent(Node? head)
    {
        for (var e = head; e is not null; e = e.Next)
        {
            var i = e.Hash & IndexMask;
            ref var slot = ref Nodes[i];
            e.Parent = slot;
            slot = e;
        }
    }

    private sealed class OneWayList
    {
        public Node? Head { get; set; }

        public Node? Tail { get; set; }

        public int Size { get; private set; }

        public void Append(Node node)
        {
            if (Size is int.MaxValue)
            {
                throw new InsufficientMemoryException();
            }
            if (Tail is {} tail)
            {
                tail.Next = node;
            }
            else
            {
                Head = node;
            }
            Tail = node;
            ++Size;
        }
    }

    /// <summary>
    /// The node in the hash table and the doubly-linked list.
    /// </summary>
    private sealed class Node : ProtectedNode
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Node"/> class.
        /// </summary>
        /// <param name="value">
        /// The value that this node has.
        /// </param>
        /// <param name="hash">
        /// The hash code of <paramref name="value"/>.
        /// </param>
        /// <param name="parent">
        /// The parent node in Separate Chaining.
        /// </param>
        public Node(T value, int hash, Node? parent)
        {
            Value = value;
            Hash = hash;
            Parent = parent;
            Previous = null;
            Next = null;
        }

        /// <summary>
        /// Gets the value that this node has.
        /// </summary>
        public T Value { get; }

        /// <summary>
        /// Gets the hash code of <see cref="Value"/>.
        /// </summary>
        public int Hash { get; }

        /// <summary>
        /// Gets or sets the parent node in Separate Chaining.
        /// </summary>
        public Node? Parent { get; set; }

        /// <summary>
        /// Gets or sets the previous node in the doubly-linked node.
        /// </summary>
        public Node? Previous { get; set; }

        /// <summary>
        /// Gets or sets the next node in the doubly-linked node.
        /// </summary>
        public Node? Next { get; set; }

        /// <summary>
        /// Gets or sets the roast.
        /// </summary>
        /// <remarks>
        /// The roast values that all the nodes in a set have must be equal to
        /// or less than that of <see cref="LinkedHashSet{T}.Head"/>.
        /// </remarks>
        public int Roast { get; set; }

        /// <inheritdoc/>
        public ProtectedNode? ParentNode => Parent;

        /// <inheritdoc/>
        public ProtectedNode? PreviousNode => Previous;

        /// <inheritdoc/>
        public ProtectedNode? NextNode => Next;

        /// <summary>
        /// Gets a new node that has the specifie value, its hash value,
        /// and the specified parent node.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="hash">
        /// The hash value.
        /// </param>
        /// <param name="parent">
        /// The parent node or <c>null</c>.
        /// </param>
        /// <returns>
        /// The new node.
        /// </returns>
        public static Node Of(T value, int hash, Node? parent)
            => new(value, hash, parent);
    }
}
