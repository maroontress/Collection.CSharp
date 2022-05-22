# LinkedHashSet

The `LinkedHashSet<T>` class implements the `ISet<T>` interface. Its instance
has both the hash table and the doubly-linked list to maintain the elements so
that it has predictable iteration order, like that of the `LinkedHashSet` class
in Java.

The linked list keeps the iteration order corresponding to the _insertion
order_ in which you insert elements into the set. Note that the insertion
order is not affected when you re-inserted any into it (i.e., the
`Add(T)` method returned `false`).

The hash table uses [Separate Chaining][separate-chainging] for collision
resolution and has the _capacity_, _size_, and _load factor_. The capacity
represents the number of entries in the hash table. In this implementation, it
is a power of two. The size is the number of elements that the hash table
contains. The set rehashes the hash table when the size divided by the capacity
is close to the load factor. Rehashing makes the capacity of the hash table
double.

To check whether two sets are equal, with iteration order taken into account,
use the [`Enumerable.SequenceEqual<T>(this IEnumerable<T>,
IEnumerable<T>)`][system.linq.enumerable.sequenceequal] method as follows:

```cs
public static bool AreEqualAndHaveSameInsertionOrder<T>(
    LinkedHashSet<T> s1, LinkedHashSet<T> s2)
{
    return s1.SequenceEqual(s2);
}
```

Note that the
[`SetEquals(IEnumerable<T>)`][system.collections.generic.hashset-1.setequals]
and `SetEquals(LinkedHashSet<T>)` methods ignore the iteration order and return
set equality; the [`Equals(object)`][system.object.equals] method returns
reference equality.

The minimum and maximum capacities are `DefaultInitialCapacity` (`16`) and
`DefaultMaxCapacity` (`0x40000000`), respectively.

As mentioned, if the number of elements in the set exceeds the product of the
capacity and the load factor, it rehashes its hash table with the capacity
updated to double. However, this implementation restricts the maximum capacity
to `DefaultMaxCapacity` (`0x40000000`). So, once the capacity reaches its
maximum, rehashing will no longer occur. Note that since the implementation
uses Separate Chaining, it is possible to add up to
[`int.MaxValue`][system.int32.maxvalue] (`0x7fffffff`) elements to the set even
after the capacity reaches its maximum unless it throws an
[`OutOfMemoryException`][system.outofmemoryexception].

If the number of elements in the set reaches the maximum value
([`int.MaxValue`][system.int32.maxvalue]), any attempt to add elements throws
an [`InsufficientMemoryException`][system.insufficientmemoryexception] (e.g.,
with the `Add(T)` method).

## Benchmarks

Let's show the performances of `HashSet`, `SimpleLinkedHashSet`, and
`LinkedHashSet` classes. We performed the following measurements on a laptop
with an Intel&reg; Core&trade; i5-6200U CPU. The
[`SimpleLinkedHashSet<T>`][simple-lhs] is just a reference, a simplified
implementation of the `ISet<T>` interface, which has both
`HashMap<T, LinkedListNode<T>>` and `LinkedList<T>` to have predictable
iteration order, unlike `LinkedHashSet`. In most cases, `LinkedHashSet` is
slightly faster than `SimpleLinkedHashSet`, and even in the worst case, it
makes no difference.

The following chart shows how long it took to add 50,000 elements to each set:

![Add][chart-add]

"With rehash" starts each set with a capacity of 16. "Without rehash" starts
each with that of 100,000 to avoid rehashing. Both results show that the
`SimpleLinkedHashSet` and `LinkedHashSet` classes are two or more times slower
than the `HashSet` class concerning the `Add` method because they create a
_node_ instance every time adding an element, but `HashSet` does not.

The following chart shows how long it took to get the intersection of two sets
(where each of them contains 50,000 elements and the intersection does 25,000
ones):

![IntersectWith][chart-intersect-with]

Concerning the `IntersectWith` methods, `LinkedHashSet` is about even with
`HashSet` since they only remove elements.

The following chart shows how long it took to get the symmetric difference of
two sets containing 50,000 elements (and the symmetric difference contains
50,000 elements):

![SymmetricExceptWith][chart-symmetric-except-with]

Concerning the `SymmetricExceptWith` methods, `LinkedHashSet` is slower than
`HashSet` since they also add elements, unlike the `IntersectWith` methods.

The following chart shows how long it took to get whether the smaller one of
the two sets is the subset of the bigger one, containing 50,000 and 50,001
elements, respectively:

![IsSubsetOf][chart-is-subset-of]

Concerning the `IsSubsetOf(IEnumerable<T>)` method, `LinkedHashSet` is about
even with `HashSet`. The `IsSubsetOf` method with two `LinkedHashSet`s is
slightly faster than with two `HashSet`s.

The following chart shows how long it took to get whether the bigger one of the
two sets is the superset of the smaller one, containing 50,000 and 49,999
elements, respectively:

![IsSupersetOf][chart-is-superset-of]

The `IsSupersetOf` methods show similar results as the `IsSubsetOf` methods.

[separate-chainging]:
  https://en.wikipedia.org/wiki/Hash_table#Separate_chaining
[open-addressing]:
  https://en.wikipedia.org/wiki/Hash_table#Open_addressing
[system.linq.enumerable.sequenceequal]:
  https://docs.microsoft.com/en-us/dotnet/api/system.linq.enumerable.sequenceequal?view=net-6.0
[system.collections.generic.hashset-1.setequals]:
  https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.hashset-1.setequals?view=net-6.0
[system.object.equals]:
  https://docs.microsoft.com/en-us/dotnet/api/system.object.equals?view=net-6.0#system-object-equals(system-object)
[system.int32.maxvalue]:
  https://docs.microsoft.com/en-us/dotnet/api/system.int32.maxvalue?view=net-6.0
[system.outofmemoryexception]:
  https://docs.microsoft.com/en-us/dotnet/api/system.outofmemoryexception?view=net-6.0
[system.insufficientmemoryexception]:
  https://docs.microsoft.com/en-us/dotnet/api/system.insufficientmemoryexception?view=net-6.0
[simple-lhs]:
  ../Collection.Test/Maroontress/Collection/SimpleLinkedHashSet.cs
[chart-add]:
  https://docs.google.com/spreadsheets/d/e/2PACX-1vRqsvt4rfe9OtsJxo_umIdgNllEqFjY6g6yeHqFKeY9Oq6eSzSRJ6_hx57AqBbtan_NL_vUk14O-Jyx/pubchart?oid=1349208269&format=image
[chart-intersect-with]:
  https://docs.google.com/spreadsheets/d/e/2PACX-1vRqsvt4rfe9OtsJxo_umIdgNllEqFjY6g6yeHqFKeY9Oq6eSzSRJ6_hx57AqBbtan_NL_vUk14O-Jyx/pubchart?oid=780952512&format=image
[chart-symmetric-except-with]:
  https://docs.google.com/spreadsheets/d/e/2PACX-1vRqsvt4rfe9OtsJxo_umIdgNllEqFjY6g6yeHqFKeY9Oq6eSzSRJ6_hx57AqBbtan_NL_vUk14O-Jyx/pubchart?oid=2046347697&format=image
[chart-is-subset-of]:
  https://docs.google.com/spreadsheets/d/e/2PACX-1vRqsvt4rfe9OtsJxo_umIdgNllEqFjY6g6yeHqFKeY9Oq6eSzSRJ6_hx57AqBbtan_NL_vUk14O-Jyx/pubchart?oid=682726058&format=image
[chart-is-superset-of]:
  https://docs.google.com/spreadsheets/d/e/2PACX-1vRqsvt4rfe9OtsJxo_umIdgNllEqFjY6g6yeHqFKeY9Oq6eSzSRJ6_hx57AqBbtan_NL_vUk14O-Jyx/pubchart?oid=1780540214&format=image
