namespace Maroontress.Collection.Test.LinkedHashSet;

using System.Collections.Immutable;

[TestClass]
public sealed class HugeElementsTest
{
    private const int MaxSize = 0x8000;

    [TestMethod]
    public void RoastReset()
    {
        var s = new CustomLinkedHashSet<int>(MaxSize);
        s.UnionWith(Enumerable.Range(0, MaxSize));
        s.IntersectWith(Enumerable.Range(0x1000, 0x6000));
        s.SymmetricExceptWith(Enumerable.Range(0, MaxSize));
        var r = Enumerable.Range(0, 0x1000)
            .Concat(Enumerable.Range(0x7000, 0x1000));
        Assert.IsTrue(s.SetEquals(r));
        Assert.IsTrue(s.IsProperSubsetOf(Enumerable.Range(0, MaxSize)));
        Assert.IsTrue(s.IsProperSupersetOf(Enumerable.Range(0, 0x1000)));
    }

    [TestMethod]
    public void LoadFactorOne()
    {
        var s = new CustomLinkedHashSet<int>(MaxSize, 1.0f);
        s.UnionWith(Enumerable.Range(0, MaxSize));
    }

    [TestMethod]
    public void AddInCriticalState()
    {
        var n = MaxSize;

        LinkedHashSet<int> NewFullSet()
        {
            var s = new CustomLinkedHashSet<int>(n);
            s.UnionWith(Enumerable.Range(0, n));
            return s;
        }

        {
            var s = NewFullSet();
            Assert.ThrowsException<InsufficientMemoryException>(
                () => _ = s.Add(-1));
            Assert.IsTrue(s.SetEquals(Enumerable.Range(0, n)));
        }

        {
            var right = ImmutableArray.Create(0, -1, -2);
            var s = NewFullSet();
            Assert.ThrowsException<InsufficientMemoryException>(
                () => s.SymmetricExceptWith(right));
            Assert.IsTrue(s.SetEquals(Enumerable.Range(1, n - 1).Append(-1)));
        }

        {
            var right = ImmutableArray.Create(0, -1, -2);
            var s = NewFullSet();
            var t = new LinkedHashSet<int>();
            t.UnionWith(right);
            Assert.ThrowsException<InsufficientMemoryException>(
                () => s.SymmetricExceptWith(t));
            Assert.IsTrue(s.SetEquals(Enumerable.Range(1, n - 1).Append(-1)));
        }
    }

    private class CustomLinkedHashSet<T> : LinkedHashSet<T>
        where T : notnull
    {
        public CustomLinkedHashSet(int initialCapacity)
            : base(Constants, initialCapacity, DefaultLoadFactor)
        {
        }

        public CustomLinkedHashSet(int initialCapacity, float loadFactor)
            : base(Constants, initialCapacity, loadFactor)
        {
        }

        private static HashTableConstants Constants { get; }
            = new(MaxSize, MaxSize, 2);
    }
}
