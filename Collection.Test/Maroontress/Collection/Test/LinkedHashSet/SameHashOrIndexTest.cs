namespace Maroontress.Collection.Test.LinkedHashSet;

using System.Collections.Immutable;
using System.Linq;

[TestClass]
public sealed class SameHashOrIndexTest
{
    private const int Capacity
        = LinkedHashSet<FreeHashString>.DefaultInitialCapacity;

    [TestMethod]
    public void Add()
    {
        var all = ImmutableArray.Create(
            new FreeHashString("foo", 1),
            new FreeHashString("bar", Capacity + 1),
            new FreeHashString("baz", 1));
        var s = NewSet();
        s.UnionWith(all);
        Assert.IsTrue(s.SequenceEqual(all));
    }

    [TestMethod]
    public void AddAndThenRemove()
    {
        var all = ImmutableArray.Create(
            new FreeHashString("foo", 1),
            new FreeHashString("bar", Capacity + 1),
            new FreeHashString("baz", 1));
        var s = NewSet();
        s.UnionWith(all);
        foreach (var i in all)
        {
            Assert.IsTrue(s.Remove(i));
        }
        Assert.AreEqual(s.Count, 0);
    }

    [TestMethod]
    public void AddAndThenRemove_ReverseOrder()
    {
        var all = ImmutableArray.Create(
            new FreeHashString("foo", 1),
            new FreeHashString("bar", Capacity + 1),
            new FreeHashString("baz", 1));
        var s = NewSet();
        s.UnionWith(all);
        foreach (var i in all.Reverse())
        {
            Assert.IsTrue(s.Remove(i));
        }
        Assert.AreEqual(s.Count, 0);
    }

    [TestMethod]
    public void AddAndThenRemoveOther()
    {
        var all = ImmutableArray.Create(
            new FreeHashString("foo", 1),
            new FreeHashString("bar", Capacity + 1),
            new FreeHashString("baz", 1));
        var s = NewSet();
        s.UnionWith(all);
        foreach (var i in ImmutableArray.Create(
            new FreeHashString("fooBar", 1)))
        {
            Assert.IsFalse(s.Remove(i));
        }
        Assert.AreEqual(s.Count, all.Length);
    }

    [TestMethod]
    public void AddAndThenRemoveInReverseOrder()
    {
        var all = ImmutableArray.Create(
            new FreeHashString("foo", 1),
            new FreeHashString("bar", Capacity + 1),
            new FreeHashString("baz", 1));
        var s = NewSet();
        s.UnionWith(all);
        foreach (var i in all.Reverse())
        {
            s.Remove(i);
        }
        Assert.AreEqual(s.Count, 0);
    }

    [TestMethod]
    public void Contains()
    {
        var all = ImmutableArray.Create(
            new FreeHashString("foo", 1),
            new FreeHashString("bar", Capacity + 1),
            new FreeHashString("baz", 1));
        var s = NewSet();
        s.UnionWith(all);
        Assert.IsTrue(all.All(i => s.Contains(i)));
    }

    [TestMethod]
    public void IntersectWith()
    {
        var left = ImmutableArray.Create(
            new FreeHashString("foo", 1),
            new FreeHashString("bar", Capacity + 1),
            new FreeHashString("baz", 1));
        var right = ImmutableArray.Create(
            new FreeHashString("fooBaz", Capacity + 1));
        var s1 = NewSet();
        s1.UnionWith(left);
        var s2 = NewSet();
        s2.UnionWith(right);
        s1.IntersectWith(s2);
        Assert.AreEqual(s1.Count, 0);
    }

    [TestMethod]
    public void SymmetryExceptWith()
    {
        var left = ImmutableArray.Create(
            new FreeHashString("fooBaz", Capacity + 1));
        var right = ImmutableArray.Create(
            new FreeHashString("foo", 1),
            new FreeHashString("bar", Capacity + 1),
            new FreeHashString("baz", 1));
        var s = NewSet();
        s.UnionWith(left);
        s.SymmetricExceptWith(right);
        Assert.IsTrue(s.SequenceEqual(left.Concat(right)));
    }

    private static LinkedHashSet<FreeHashString> NewSet()
    {
        return new LinkedHashSet<FreeHashString>();
    }
}
