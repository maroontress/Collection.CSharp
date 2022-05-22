namespace Maroontress.Collection.Test.ImmutableDictionary;

using System.Collections.Immutable;

[TestClass]
public sealed class ImmutableDictionaryConformanceTest
    : AbstractImmutableDictionaryConformanceTest
{
    protected override IImmutableDictionary<int, string> NewMap()
    {
        return ImmutableDictionary<int, string>.Empty;
    }
}
