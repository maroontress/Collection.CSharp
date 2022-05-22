# InternMap

The `InternMap` class provides the canonical value object corresponding to the
specified key. An example:

```csharp
var map = new InternMap<int, string>(i => i.ToString());
var s1 = map.Intern(123);
var s2 = map.Intern(123);
```

where `s1` and `s2` refer the same object.

This class has the `ConcurrentDictionary` class and just wraps its `GetOrAdd`
methods.
