namespace Maroontress.Collection.Test.LinkedHashSet;

[TestClass]
public sealed class SetConformanceTest : AbstractSetConformanceTest
{
    protected override ISet<string> NewSet()
    {
        return new LinkedHashSet<string>();
    }
}
