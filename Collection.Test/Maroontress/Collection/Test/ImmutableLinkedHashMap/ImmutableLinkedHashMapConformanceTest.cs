namespace Maroontress.Collection.Test.ImmutableLinkedHashMap;

using System.Collections.Immutable;

[TestClass]
public sealed class ImmutableLinkedHashMapConformanceTest
    : AbstractImmutableDictionaryConformanceTest
{
    protected override IImmutableDictionary<int, string> NewMap()
    {
        return ImmutableLinkedHashMap<int, string>.Empty;
    }
}
