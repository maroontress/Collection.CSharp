namespace Maroontress.Collection.Test.HashSet;

[TestClass]
public sealed class SetConformanceTest : AbstractSetConformanceTest
{
    protected override ISet<string> NewSet()
    {
        return new HashSet<string>();
    }
}
