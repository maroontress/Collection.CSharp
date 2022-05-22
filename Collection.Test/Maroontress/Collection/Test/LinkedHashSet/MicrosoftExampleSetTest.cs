namespace Maroontress.Collection.Test.LinkedHashSet;

[TestClass]
public sealed class MicrosoftExampleSetTest
    : AbstractMicrosoftExampleSetTest
{
    protected override ISet<int> NewSet()
    {
        return new LinkedHashSet<int>();
    }
}
