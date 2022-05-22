namespace Maroontress.Collection.Test.SimpleLinkedHashSet;

[TestClass]
public sealed class SetConformanceTest : AbstractSetConformanceTest
{
    protected override ISet<string> NewSet()
    {
        return new SimpleLinkedHashSet<string>(16);
    }
}
