namespace Maroontress.Collection.Test;

[TestClass]
public sealed class InternMapTest
{
    [TestMethod]
    public void Intern()
    {
        var key = 12;
        var map = new InternMap<int, string>(k => k.ToString());
        var c1 = map.Intern(key);
        var c2 = map.Intern(key);
        Assert.AreSame(c1, c2);
        Assert.AreEqual("12", c1);
    }

    [TestMethod]
    public void Ctor_InitialCapacity()
    {
        var map = new InternMap<int, string>(k => $"{k}", 1000);
        for (var k = 0; k < 1000; ++k)
        {
            _ = map.Intern(k);
        }
        var c1 = map.Intern(500);
        var c2 = map.Intern(500);
        Assert.AreEqual("500", c1);
        Assert.AreSame(c1, c2);
    }

    [TestMethod]
    public void Ctor_Null()
    {
        Assert.ThrowsException<ArgumentNullException>(
            () => _ = new InternMap<int, string>(null!));
    }

    [TestMethod]
    public void Ctor_NegativeCapacity()
    {
        Assert.ThrowsException<ArgumentOutOfRangeException>(
            () => _ = new InternMap<int, string>(k => $"{k}", -1));
    }

    [TestMethod]
    public void Ctor_ZeroConcurrencyLevel()
    {
        Assert.ThrowsException<ArgumentOutOfRangeException>(
            () => _ = new InternMap<int, string>(k => $"{k}", 31, 0));
    }
}
