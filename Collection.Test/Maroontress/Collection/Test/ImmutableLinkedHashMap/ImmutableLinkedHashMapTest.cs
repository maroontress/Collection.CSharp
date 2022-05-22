namespace Maroontress.Collection.Test.ImmutableLinkedHashMap;

using System.Collections.Immutable;

[TestClass]
public sealed class ImmutableLinkedHashMapTest
{
    private static IImmutableDictionary<int, string> Empty { get; }
        = ImmutableLinkedHashMap<int, string>.Empty;

    private static KeyValuePair<int, string> Pair123 { get; } = NewPair(123);

    private static KeyValuePair<int, string> Pair456 { get; } = NewPair(456);

    private static KeyValuePair<int, string> Pair789 { get; } = NewPair(789);

    [TestMethod]
    public void Clear()
    {
        var m = Empty.AddRange(
            ImmutableArray.Create(
                Pair123,
                Pair456,
                Pair789));
        Assert.AreSame(Empty, m.Clear());
    }

    [TestMethod]
    public void Keys()
    {
        var m = Empty.AddRange(
            ImmutableArray.Create(
                Pair123,
                Pair456,
                Pair789));
        Assert.IsTrue(
            Enumerable.SequenceEqual(
                m.Keys,
                ImmutableArray.Create(123, 456, 789)));
    }

    [TestMethod]
    public void Values()
    {
        var m = Empty.AddRange(
            ImmutableArray.Create(
                Pair123,
                Pair456,
                Pair789));
        AssertSameValues(
            m.Values,
            ImmutableArray.Create(
                Pair123.Value,
                Pair456.Value,
                Pair789.Value));
    }

    [TestMethod]
    public void Count_Empty()
    {
        Assert.AreEqual(0, Empty.Count);
    }

    [TestMethod]
    public void Count()
    {
        var m = Empty.AddRange(
            ImmutableArray.Create(
                Pair123,
                Pair456,
                Pair789));
        Assert.AreEqual(3, m.Count);
    }

    [TestMethod]
    public void Indexer()
    {
        var m = Empty.AddRange(
            ImmutableArray.Create(
                Pair123,
                Pair456,
                Pair789));
        Assert.AreSame(Pair123.Value, m[123]);
        Assert.AreSame(Pair456.Value, m[456]);
        Assert.AreSame(Pair789.Value, m[789]);
    }

    [TestMethod]
    public void AddRange()
    {
        var m = Empty.AddRange(
            ImmutableArray.Create(
                Pair123,
                Pair456,
                Pair789,
                NewPair(789),
                NewPair(456),
                NewPair(123)));
        AssertSamePairs(
            ImmutableArray.Create(
                Pair123,
                Pair456,
                Pair789),
            m);
    }

    [TestMethod]
    public void AddRange_Double()
    {
        var m1 = Empty.AddRange(
            ImmutableArray.Create(
                Pair123,
                Pair456,
                Pair789,
                NewPair(789),
                NewPair(456),
                NewPair(123)));
        var m2 = Empty.AddRange(
            ImmutableArray.Create(
                Pair123,
                Pair789))
            .AddRange(m1);
        AssertSamePairs(
            ImmutableArray.Create(
                Pair123,
                Pair789,
                Pair456),
            m2);
    }

    [TestMethod]
    public void AddRange_Empty()
    {
        var m1 = Empty.AddRange(
            ImmutableArray.Create(
                Pair123,
                Pair456,
                Pair789));
        var m2 = m1.AddRange(Empty);
        Assert.AreSame(m1, m2);
    }

    [TestMethod]
    public void Remove_AndThen_Add()
    {
        var m0 = Empty.AddRange(
            ImmutableArray.Create(
                Pair123,
                Pair456,
                Pair789));
        var m1 = m0.Remove(456);
        var m2 = Empty.AddRange(
            ImmutableArray.Create(
                Pair123,
                Pair789));
        AssertSamePairs(m2, m1);
        var m3 = m1.Add(456, Pair456.Value);
        AssertSamePairs(
            ImmutableArray.Create(
                Pair123,
                Pair789,
                Pair456),
            m3);
    }

    [TestMethod]
    public void RemoveRange()
    {
        var pair12 = NewPair(12);
        var m0 = Empty.AddRange(
            ImmutableArray.Create(
                Pair123,
                Pair456,
                Pair789,
                pair12));
        var m1 = m0.RemoveRange(
            ImmutableArray.Create(
                456,
                789));
        AssertSamePairs(
            ImmutableArray.Create(
                Pair123,
                pair12),
            m1);
    }

    [TestMethod]
    public void SetItem_Append()
    {
        var pair12 = NewPair(12);
        var m0 = Empty.AddRange(
            ImmutableArray.Create(
                Pair123,
                Pair456,
                Pair789));
        var m1 = m0.SetItem(pair12.Key, pair12.Value);
        AssertSamePairs(
            ImmutableArray.Create(
                Pair123,
                Pair456,
                Pair789,
                pair12),
            m1);
    }

    [TestMethod]
    public void SetItem_Overwrite()
    {
        var newPair456 = NewPair(456, "45");
        var m0 = Empty.AddRange(
            ImmutableArray.Create(
                Pair123,
                Pair456,
                Pair789));
        var m1 = m0.SetItem(newPair456.Key, newPair456.Value);
        AssertSamePairs(
            ImmutableArray.Create(
                Pair123,
                newPair456,
                Pair789),
            m1);
    }

    [TestMethod]
    public void SetItem_Overwrite_EqualValue()
    {
        var newPair456 = NewPair(456);
        var m0 = Empty.AddRange(
            ImmutableArray.Create(
                Pair123,
                Pair456,
                Pair789));
        var m1 = m0.SetItem(newPair456.Key, newPair456.Value);
        Assert.AreSame(m0, m1);
    }

    [TestMethod]
    public void SetItems()
    {
        var pair12 = NewPair(12);
        var newPair123 = NewPair(123, "23");
        var m0 = Empty.AddRange(
            ImmutableArray.Create(
                Pair123,
                Pair456,
                Pair789));
        var m1 = m0.SetItems(
            ImmutableArray.Create(
                pair12,
                NewPair(456, "456"),
                newPair123));
        AssertSamePairs(
            ImmutableArray.Create(
                newPair123,
                Pair456,
                Pair789,
                pair12),
            m1);
    }

    [TestMethod]
    public void SetItems_AppendOnly()
    {
        var m0 = Empty.AddRange(
            ImmutableArray.Create(
                Pair123,
                Pair456,
                Pair789));
        var pair12 = NewPair(12);
        var pair34 = NewPair(34);
        var m1 = m0.SetItems(
            ImmutableArray.Create(
                pair12,
                pair34));
        AssertSamePairs(
            ImmutableArray.Create(
                Pair123,
                Pair456,
                Pair789,
                pair12,
                pair34),
            m1);
    }

    private static void AssertSameValues(
        IEnumerable<string> s1, IEnumerable<string> s2)
    {
        var left = s1.ToArray();
        var right = s2.ToArray();
        var n = left.Length;
        Assert.AreEqual(n, right.Length);
        for (var k = 0; k < n; ++k)
        {
            Assert.AreSame(left[k], right[k]);
        }
    }

    private static void AssertSamePairs(
        IEnumerable<KeyValuePair<int, string>> s1,
        IEnumerable<KeyValuePair<int, string>> s2)
    {
        var left = s1.ToArray();
        var right = s2.ToArray();
        var n = left.Length;
        Assert.AreEqual(n, right.Length);
        for (var k = 0; k < n; ++k)
        {
            var leftOne = left[k];
            var rightOne = right[k];
            Assert.AreEqual(leftOne.Key, rightOne.Key);
            Assert.AreSame(leftOne.Value, rightOne.Value);
        }
    }

    private static KeyValuePair<int, string> NewPair(int i)
        => new(i, i.ToString());

    private static KeyValuePair<int, string> NewPair(int i, string s)
        => new(i, s);
}
