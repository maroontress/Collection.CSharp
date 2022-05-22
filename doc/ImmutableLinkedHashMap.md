# ImmutableLinkedHashMap

## Summary

The `ImmutableLinkedHashMap<K, V>` class implements the
`IImmutableDictionary<K, V>` interface. It has `ImmutableDictionary<K, V>` and
`ImmutableArray<T>` (where `T` stands for `KeyValuePair<K, V>`) to maintain the
elements so that it has predictable iteration order, like that of the
`LinkedHashMap` class in Java.

## Remarks

The `ImmutableArray<T>` keeps the iteration order corresponding to the
_insertion order_ in which you insert key-value pairs into the map. Note that
the insertion order is not affected when you re-inserted any key into it (i.e.,
with the `SetItem(K, V)` method).
